using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FlightControlWeb.Models
{
    public class FlightLocation
    {
        public FlightLocation(double longitude, double latitude, DateTime time)
        {
            Longitude = longitude;
            Latitude = latitude;
            MyDateTime = time;
        }

        public FlightLocation()
        {

        }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("date_time")]
        public DateTime MyDateTime { get; set; }
    }
}
