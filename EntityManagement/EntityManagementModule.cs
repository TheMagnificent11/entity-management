using Autofac;
using Autofac.Core;
using Microsoft.EntityFrameworkCore;

namespace EntityManagement
{
    /// <summary>
    /// Entity Management Autofac Module
    /// </summary>
    /// <typeparam name="T">Database context type</typeparam>
    public class EntityManagementModule<T> : Module
        where T : DbContext, IDatabaseContext
    {
        /// <summary>
        /// Registers Entity Management classes
        /// </summary>
        /// <param name="builder">The builder through which components can be registered</param>
        protected override void Load(ContainerBuilder builder)
        {
            var key = nameof(T);

            builder.RegisterType<T>()
                .Keyed<IDatabaseContext>(key)
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(DatabaseContextScope<>))
                .WithParameter(new ResolvedParameter(
                    (info, context) => info.ParameterType == typeof(IDatabaseContext),
                    (info, context) => context.ResolveKeyed<IDatabaseContext>(key)))
                .As(typeof(IDatabaseContextScope<>));
        }
    }
}
