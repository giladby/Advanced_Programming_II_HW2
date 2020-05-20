using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        public Flight(string id, double lon, double lat, int pass, string comp, DateTime dt, bool ex)
        {
            FlightId = id;
            Longitude = lon;
            Latitude = lat;
            Passangers = pass;
            CompanyName = comp;
            MyDateTime = dt;
            IsExternal = ex;
        }

        [JsonPropertyName("flight_id")]
        [JsonProperty("flight_id")]
        public string FlightId { get; set; }

        [JsonPropertyName("longitude")]
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("latitude")]
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("passangers")]
        [JsonProperty("passangers")]
        public int Passangers { get; set; }

        [JsonPropertyName("company_name")]
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        [JsonPropertyName("date_time")]
        [JsonProperty("date_time")]
        public DateTime MyDateTime { get; set; }

        [JsonPropertyName("is_external")]
        [JsonProperty("is_external")]
        public bool IsExternal { get; set; }
    }
}
