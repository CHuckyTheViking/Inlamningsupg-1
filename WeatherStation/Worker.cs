using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeatherStation.Models;

namespace WeatherStation
{

    public class Worker : BackgroundService
    {
      
        private readonly ILogger<Worker> _logger;
        private readonly string _url = "http://api.weatherstack.com/current?access_key=e699857f79960f12fc50911e6203d374&query=Koping";

        private HttpClient _client;
        Random random = new Random();

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
                    var response = await _client.GetAsync(_url);
                    if (response.IsSuccessStatusCode)
                    {
                        WeatherData data = JsonConvert.DeserializeObject<WeatherData>(await response.Content.ReadAsStringAsync());
                        _logger.LogInformation($"The weather in Köping is {data.current.temperature}C and was measured {data.current.observation_time}");
                        if (data.current.temperature > 30)
                        {
                            _logger.LogInformation($"Leave your sweater at home, it's very warm today!");
                        }
                        if (data.current.temperature < 10)
                        {
                            _logger.LogInformation($"Ps, take a warm jacket, it's cold outside!");
                        }

                    }
                   
                      
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"The WeatherStation have hit a bump. Error: {ex.Message}");
                }

                
                int minutes = random.Next(20,40);

                await Task.Delay(minutes * 60000, stoppingToken);
            }
        }
    }
}
