using Autofac;
using AutoMapper;
using EntityManagement;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleApiWebApp.Data;
using SampleApiWebApp.Infrastructure;

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
            _ = builder.RegisterModule(new EntityManagementModule<DatabaseContext>());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<DatabaseContext>(options =>
                options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            services.AddAutoMapper(typeof(Startup).Assembly);

            services.AddControllers(options => options.Filters.Add(new ExceptionFilter()))
                .AddFluentValidation(options => options.RegisterValidatorsFromAssembly(typeof(Startup).Assembly));

            services.ConfigureProblemDetails();

            services.ConfigureSwagger(ApiName, this.apiVersions);
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
    }
}