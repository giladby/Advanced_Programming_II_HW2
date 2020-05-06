using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class ServerFlightsManager
    {
        
        private static List<FlightPlan> flightPlans = new List<FlightPlan>();
        private static Dictionary<string, FlightPlan> flightIds = new Dictionary<string, FlightPlan>();


        private string GenerateFlightId()
        {
            string id = "";
            Random rnd = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            
            for (int i = 0; i < 2; i++)
            {
                id += chars[rnd.Next(chars.Length)];
            }

            for (int i = 0; i < 4; i++)
            {
                id += rnd.Next(0, 10).ToString();
            }

            return id;
        }
        
        public void AddFlightPlan(FlightPlan f)
        {
            string id;
            while (true)
            {
                id = GenerateFlightId();
                if (!flightIds.ContainsKey(id))
                {
                    break;
                }
            }
            flightIds[id] = f;
            flightPlans.Add(f);
        }

        public void DeleteFlightPlan(string id)
        {
            if (flightIds.ContainsKey(id))
            {
                FlightPlan myFlightPlan = flightIds[id];
                flightPlans.Remove(myFlightPlan);
                flightIds.Remove(id);
            }
        }
       
        public FlightPlan GetFlightPlan(string id)
        {
            if (flightIds.ContainsKey(id))
            {
                return flightIds[id]; 
            }
            return null;
        }

        private Tuple<double, double> GetLocation(FlightPlan fp, DateTime dt)
        {
            double longitude = fp.InitialLocation.Longitude;
            double latitude = fp.InitialLocation.Latitude;
            DateTime start = fp.InitialLocation.MyDateTime;
            double timeGap = (dt - start).TotalSeconds;
            double distance;
            double ratio;
            if (timeGap == 0)
            {
                return new Tuple<double, double>(longitude, latitude);
            }
            foreach (FlightSegment currentSegment in fp.Segments)
            {
                if(timeGap <= currentSegment.TimespanSeconds)
                {
                    ratio = timeGap / currentSegment.TimespanSeconds;
                    distance = currentSegment.Longitude - longitude;
                    longitude += ratio * distance;
                    distance = currentSegment.Latitude - latitude;
                    latitude += ratio * distance;
                    break;
                }
                timeGap -= currentSegment.TimespanSeconds;
                longitude = currentSegment.Longitude;
                latitude = currentSegment.Latitude;
            }
            return new Tuple<double, double>(longitude, latitude);
        }

        public Flight GetFlight(FlightPlan fp, DateTime dt)
        {
            Tuple<double, double> location = GetLocation(fp, dt);
            double lon = location.Item1;
            double lat = location.Item2;
            string id = flightIds.FirstOrDefault(x => x.Value == fp).Key;
            if(id == null)
            {
                return null;
            }
            return new Flight(id, lon, lat, fp.Passengers, fp.CompanyName, dt, false);
        }

        private DateTime GetEndTime(FlightPlan fp)
        {
            DateTime time = fp.InitialLocation.MyDateTime;
            foreach (FlightSegment fs in fp.Segments)
            {
                time = time.AddSeconds(fs.TimespanSeconds);
            }
            return time;
        }

        public ArrayList GetFlightsByTime(DateTime dt)
        {
            dt = dt.ToUniversalTime();
            ArrayList flights = new ArrayList();
            foreach (FlightPlan fp in flightPlans)
            {
                if ((DateTime.Compare(dt, fp.InitialLocation.MyDateTime) >= 0) && (DateTime.Compare(dt, GetEndTime(fp)) <= 0))
                {
                   flights.Add(GetFlight(fp, dt));
                }
            }
            return flights;

        }
    }
}
