using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace SampleApiWebApp.Infrastructure
{
    public static class ProblemDetalisConfiguration
    {
        public static void ConfigureProblemDetails(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = (int)HttpStatusCode.BadRequest,
                        Detail = "Please refer to the errors property for additional details"
                    };

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/json" }
                    };
                };
            });
        }
    }
}
