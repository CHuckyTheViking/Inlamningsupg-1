using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace WeatherStation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
         .MinimumLevel.Debug()
         .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
         .Enrich.FromLogContext()
         .WriteTo.File(@"C:\Users\Jesper\source\repos\Inlämningsupg 1\WeatherStation\WeatherForecast.txt")
         .CreateLogger();

            try
            {
                Log.Information("Starting WeatherStation...");
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"The WeatherStation has crashed, Error:: {ex.Message}");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
