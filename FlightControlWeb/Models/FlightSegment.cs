﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FlightControlWeb.Models
{
    public class FlightSegment
    {
        [JsonPropertyName("longitude")]
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("latitude")]
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("timespan_seconds")]
        [JsonProperty("timespan_seconds")]
        public double TimespanSeconds { get; set; }

        public FlightSegment() { }

        public FlightSegment(double longitudeInput, double latitudeInput,
            double timespanSecondsInput)
        {
            Longitude = longitudeInput;
            Latitude = latitudeInput;
            TimespanSeconds = timespanSecondsInput;
        }
    }
}
