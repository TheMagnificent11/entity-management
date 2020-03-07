using System;
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
        private readonly TDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseHealthCheck{TDbContext}"/> class
        /// </summary>
        /// <param name="dbContext">Database context</param>
        public DatabaseHealthCheck(TDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private static Type DbContextType => typeof(TDbContext);

        private static string HealthCheckName => $"DatabaseHealthCheck-{typeof(TDbContext).FullName}";

        /// <summary>
        /// Runs a health check to determine database connectivity
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
                var canConnect = await this.dbContext.Database.CanConnectAsync(cancellationToken);

                return canConnect
                    ? HealthCheckResult.Healthy($"{DbContextType.Name} is available.")
                    : HealthCheckResult.Unhealthy($"{DbContextType.Name} is unavailable.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message);
            }
        }
    }
}
