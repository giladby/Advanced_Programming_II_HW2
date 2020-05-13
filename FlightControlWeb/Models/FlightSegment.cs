using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FlightControlWeb.Models
{
    public class FlightSegment
    {
        public FlightSegment(double longitude, double latitude, double time)
        {
            Longitude = longitude;
            Latitude = latitude;
            TimespanSeconds = time;
        }

        public FlightSegment()
        {

        }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("timespan_seconds")]
        public double TimespanSeconds { get; set; }
    }
}
