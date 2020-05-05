using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        public string FlightId { get; set; }
        public double Longtitude { get; set; }
        public double Latitude { get; set; }
        public int Passangers { get; set; }
        public string CompanyName { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsExternal { get; set; }
    }
}
