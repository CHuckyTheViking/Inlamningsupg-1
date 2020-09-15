using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WeatherStation
{
  

    public class Rootobject
    {
        public WeatherData[] Property1 { get; set; }
    }

    public class WeatherData
    {
        public DateTime DateTime { get; set; }
        public int EpochDateTime { get; set; }
        public int WeatherIcon { get; set; }
        public string IconPhrase { get; set; }
        public bool HasPrecipitation { get; set; }
        public bool IsDaylight { get; set; }
        public Temperature Temperature { get; set; }
        public int PrecipitationProbability { get; set; }
        public string MobileLink { get; set; }
        public string Link { get; set; }
    }

    public class Temperature
    {
        public float Value { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
    }

    




    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string _url = "http://dataservice.accuweather.com/forecasts/v1/hourly/1hour/311006?apikey=nD05B05OOKhWwYDyEg5LDbibbALIeQRR&language=en-us&details=false&metric=true";

        private HttpClient _client;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }


        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _client = new HttpClient();
            _logger.LogInformation("The WeatherStation is now up and running...");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Dispose();
            _logger.LogInformation("The WeatherStation has been turned off...");
            return base.StopAsync(cancellationToken);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string _result = await _client.GetStringAsync(_url);
                    List<WeatherData> weather =  JsonConvert.DeserializeObject<List<WeatherData>>(_result);
                    foreach (WeatherData v in weather)
                    {
                        _logger.LogInformation($"The weather in Köping is {v.Temperature.Value}{v.Temperature.Unit} and {v.IconPhrase}");
                        if (v.Temperature.Value > 30)
                            _logger.LogInformation("It's really hot outside, so no need for a jacket");
                        if (v.Temperature.Value < 10)
                            _logger.LogInformation("It's getting cold outside, so grab the jacket");

                    }

                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"The WeatherStation have hit a bump. Error: {ex.Message}");
                }

                Random random = new Random();
                int minutes = random.Next(20,60);

                await Task.Delay(minutes * 60000, stoppingToken);
            }
        }
    }
}
