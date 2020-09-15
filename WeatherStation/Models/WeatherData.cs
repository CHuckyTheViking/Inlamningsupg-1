using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherStation.Models
{
    public class Current
    {
        public string observation_time { get; set; }
        public int temperature { get; set; }
     
    }

    public class WeatherData
    {
        public Current current { get; set; }

    }
}
