using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        
        public string FlightId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int Passangers { get; set; }
        public string CompanyName { get; set; }
        public DateTime MyDateTime { get; set; }
        public bool IsExternal { get; set; }
    }
}
