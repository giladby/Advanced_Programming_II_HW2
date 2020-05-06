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
        public FlightPlan(int pass, string cm,
            FlightLocation il, List<FlightSegment> segments)
        {
            Passengers = pass;
            CompanyName = cm;
            InitialLocation = il;
            Segments = segments;
        }
        public FlightPlan()
        {

        }

        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }
        [DataMember]
        [JsonPropertyName("company_name")]
        public string CompanyName { get; set; }
        [JsonPropertyName("initial_location")]
        public FlightLocation InitialLocation { get; set;}
        [JsonPropertyName("segments")]
        public List<FlightSegment> Segments { get; set; }
    }
}
