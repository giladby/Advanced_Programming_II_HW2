using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FlightControlWeb.Models
{

    public class FlightPlan
    {
        [JsonPropertyName("passengers")]
        [JsonProperty("passengers")]
        public int Passengers { get; set; }

        [JsonPropertyName("company_name")]
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        [JsonPropertyName("initial_location")]
        [JsonProperty("initial_location")]
        public FlightLocation InitialLocation { get; set; }

        [JsonPropertyName("segments")]
        [JsonProperty("segments")]
        public List<FlightSegment> Segments { get; set; }

        public FlightPlan() { }

        public FlightPlan(int passengersInput, string companyNameInput,
            FlightLocation initialLocationInput, List<FlightSegment> segmentsInput)
        {
            Passengers = passengersInput;
            CompanyName = companyNameInput;
            InitialLocation = initialLocationInput;
            Segments = segmentsInput;
        }
    }
}
