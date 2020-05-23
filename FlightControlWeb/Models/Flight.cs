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
        [JsonPropertyName("flight_id")]
        [JsonProperty("flight_id")]
        public string FlightId { get; set; }

        [JsonPropertyName("longitude")]
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("latitude")]
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("passengers")]
        [JsonProperty("passengers")]
        public int Passengers { get; set; }

        [JsonPropertyName("company_name")]
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        [JsonPropertyName("date_time")]
        [JsonProperty("date_time")]
        public DateTime MyDateTime { get; set; }

        [JsonPropertyName("is_external")]
        [JsonProperty("is_external")]
        public bool IsExternal { get; set; }

        public Flight(string flightIdInput, double longitudeInput, double latitudeInput,
            int passengersInput,string companyNameInput, DateTime myDateTimeInput,
            bool isExternalInput)
        {
            FlightId = flightIdInput;
            Longitude = longitudeInput;
            Latitude = latitudeInput;
            Passengers = passengersInput;
            CompanyName = companyNameInput;
            MyDateTime = myDateTimeInput;
            IsExternal = isExternalInput;
        }
    }
}
