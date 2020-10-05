using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace SampleApiWebApp.Infrastructure
{
    public static class SwaggerConfiguration
    {
        public static void ConfigureSwagger(this IApplicationBuilder app, string apiName, IEnumerable<string> apiVersions)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                apiVersions
                    .ToList()
                    .ForEach(version => c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{apiName} {version}"));
            });
        }

        public static void ConfigureSwagger(
            this IServiceCollection services,
            string apiName,
            IList<string> apiVersions,
            string securitySchemaName = null,
            OpenApiSecurityScheme securityScheme = null)
        {
            services.AddSwaggerGen(c =>
            {
                apiVersions
                    .ToList()
                    .ForEach(version =>
                    {
                        c.SwaggerDoc(version, new OpenApiInfo { Title = $"{apiName} {version}", Version = version });

                        if (securityScheme == null || string.IsNullOrWhiteSpace(securitySchemaName))
                            return;

                        c.AddSecurityDefinition(securitySchemaName, securityScheme);
                    });
            });
        }
    }
}
