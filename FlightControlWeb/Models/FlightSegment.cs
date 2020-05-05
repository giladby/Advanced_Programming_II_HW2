using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightSegment
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int TimespanSeconds { get; set; }
    }
}
