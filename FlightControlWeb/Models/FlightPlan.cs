using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        public string FlightPlanId { get; set; }
        public int Passengers { get; set; }
        public string CompanyName { get; set; }
        public FlightLocation InitialLocation { get; set;}
        public List<FlightSegment> Segments { get; set; }
    }
}
