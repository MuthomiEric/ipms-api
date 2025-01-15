using Serilog;

namespace IPMS.API.Extensions
{
    public static class LoggerSetupExtension
    {
        public static WebApplicationBuilder ConfigureSerilogLogger(this WebApplicationBuilder builder)
        {
            var logPath = builder.Configuration["LogPath"];

            builder.Host.UseSerilog();

            Log.Logger = new
                    LoggerConfiguration().WriteTo.File(logPath!,
                    rollingInterval: RollingInterval.Day)
                .MinimumLevel.Warning()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
                .CreateLogger();

            return builder;
        }
    }
}
