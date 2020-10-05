using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace SampleApiWebApp.Infrastructure
{
    public static class LoggingConfiguration
    {
        public static void ConfigureLogging(
            this IServiceCollection services,
            IConfiguration configuration,
            LogEventLevel minimumLevel,
            string seqUri,
            string seqApiKey)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var serilogLevelSwitch = new LoggingLevelSwitch
            {
                MinimumLevel = minimumLevel
            };

            var config = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .MinimumLevel.ControlledBy(serilogLevelSwitch)
                .WriteTo.Console()
                .WriteTo.Seq(seqUri, apiKey: seqApiKey, controlLevelSwitch: serilogLevelSwitch)
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.FromLogContext();

            Log.Logger = config.CreateLogger();

            Log.Logger.Information($"================= Application Started =================");

            services.AddLogging(builder => builder.AddSerilog(dispose: true));
        }
    }
}
