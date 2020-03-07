using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EntityManagement
{
    /// <summary>
    /// Database Health Check
    /// </summary>
    /// <typeparam name="TDbContext">Database context type</typeparam>
    public class DatabaseHealthCheck<TDbContext> : IHealthCheck
        where TDbContext : DbContext
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseHealthCheck{TDbContext}"/> class
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        public DatabaseHealthCheck(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private static Type DbContextType => typeof(TDbContext);

        private static string HealthCheckName => $"DatabaseHealthCheck-{typeof(TDbContext).FullName}";

        /// <summary>
        /// Runs the health check, returning the status of the component being checked
        /// </summary>
        /// <param name="context">A context object associated with the current execution</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> that can be used to cancel the health check
        /// </param>
        /// <returns>
        /// A <see cref="Task{HealthCheckResult}"/> that completes when the health check has finished yielding the
        /// status of the component being checked
        /// </returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Registration.Name != HealthCheckName)
                context.Registration.Name = HealthCheckName;

            return await this.Check(cancellationToken);
        }

        private async Task<HealthCheckResult> Check(CancellationToken cancellationToken)
        {
            try
            {
                var databaseExists = await this.CheckIfDatabaseExists(cancellationToken);

                return databaseExists
                    ? HealthCheckResult.Healthy($"{DbContextType.Name} is available.")
                    : HealthCheckResult.Unhealthy($"{DbContextType.Name} is unavailable.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message);
            }
        }

        private async Task<bool> CheckIfDatabaseExists(CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = new SqlConnection(this.connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var command = new SqlCommand("SELECT * FROM information_schema.tables where TABLE_SCHEMA != 'sys'", connection))
                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        var result = await reader.ReadAsync(cancellationToken);
                        if (!result)
                            return false;
                    }
                }

                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }
    }
}
