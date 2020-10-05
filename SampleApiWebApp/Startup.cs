using System.Reflection;
using Autofac;
using Autofac.Features.Variance;
using AutofacSerilogIntegration;
using AutoMapper;
using EntityManagement;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleApiWebApp.Data;
using SampleApiWebApp.Infrastructure;
using Serilog.Events;

namespace SampleApiWebApp
{
    public class Startup
    {
        private const string ApiName = "Sample API";
        private const string ApiVersion = "v1";

        private readonly string[] apiVersions = { ApiVersion };

        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public static void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterLogger();
            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterModule(new EntityManagementModule<DatabaseContext>());
            ConfigureMediatr(builder);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.ConfigureSwagger(ApiName, this.apiVersions);

            app.MigrationDatabase<DatabaseContext>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var logEventLevel = LogEventLevel.Information;

#if DEBUG
            logEventLevel = LogEventLevel.Debug;
#endif

            services.ConfigureLogging(
                this.configuration,
                logEventLevel,
                this.configuration["SeqSettings:Uri"],
                this.configuration["SeqSettings:Key"]);

            services.AddDbContextPool<DatabaseContext>(options =>
                options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            services.AddAutoMapper(typeof(Startup).Assembly);

            services.AddControllers(options => options.Filters.Add(new ExceptionFilter()));

            services.ConfigureProblemDetails();

            services.ConfigureSwagger(ApiName, this.apiVersions);
        }

        private static void ConfigureMediatr(ContainerBuilder builder)
        {
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly).AsImplementedInterfaces();
        }
    }
}