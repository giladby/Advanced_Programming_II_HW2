using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class MyFlightsManager
    {
        
        private IMemoryCache myCache;

        public MyFlightsManager(IMemoryCache cache)
        {
            myCache = cache;
        }

        private void setCache(Dictionary<string, FlightPlan> flightIds, List<FlightPlan> flightPlans)
        {
            myCache.Set("flightPlans", flightPlans);
            myCache.Set("flightIds", flightIds);
        }

        private Dictionary<string, FlightPlan> getFlightIds()
        {
            Dictionary<string, FlightPlan> flightIds = new Dictionary<string, FlightPlan>();
            if (!myCache.TryGetValue("flightIds", out flightIds))
            {
                if (flightIds == null)
                {
                    flightIds = new Dictionary<string, FlightPlan>();
                }
            }
            myCache.Set("flightIds", flightIds);
            return flightIds;
        }

        private List<FlightPlan> getFlightPlans()
        {
            List<FlightPlan> flightPlans = new List<FlightPlan>();
            if (!myCache.TryGetValue("flightPlans", out flightPlans))
            {
                if (flightPlans == null)
                {
                    flightPlans = new List<FlightPlan>();
                }
            }
            myCache.Set("flightPlans", flightPlans);
            return flightPlans;
        }

        private void addFlightPlanToCache(FlightPlan fp, string id)
        {
            List<FlightPlan> flightPlans = getFlightPlans();
            Dictionary<string, FlightPlan> flightIds = getFlightIds();
            
            flightIds.Add(id, fp);
            flightPlans.Add(fp);

            setCache(flightIds, flightPlans);
        }

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
        
        public void AddFlightPlan(FlightPlan fp)
        {
            string id;
            Dictionary<string, FlightPlan> flightIds = getFlightIds();
            while (true)
            {
                id = GenerateFlightId();
                if (!flightIds.ContainsKey(id))
                {
                    break;
                }
            }
            
            addFlightPlanToCache(fp, id);
        }

        public void DeleteFlightPlan(string id)
        {
            List<FlightPlan> flightPlans = getFlightPlans();
            Dictionary<string, FlightPlan> flightIds = getFlightIds();

            if (flightIds.ContainsKey(id))
            {
                FlightPlan myFlightPlan = flightIds[id];
                flightPlans.Remove(myFlightPlan);
                flightIds.Remove(id);
                setCache(flightIds, flightPlans);
            }
        }
       
        public FlightPlan GetFlightPlan(string id)
        {
            Dictionary<string, FlightPlan> flightIds = getFlightIds();
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
            Dictionary<string, FlightPlan> flightIds = getFlightIds();
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
                try
                {
                    time = time.AddSeconds(fs.TimespanSeconds);
                }
                catch
                {
                    return DateTime.MaxValue;
                }
            }
            return time;
        }

        public ArrayList GetFlightsByTime(DateTime dt)
        {
            dt = dt.ToUniversalTime();
            ArrayList flights = new ArrayList();
            List<FlightPlan> flightPlans = getFlightPlans();
            foreach (FlightPlan fp in flightPlans)
            {
                DateTime end = GetEndTime(fp);
                if ((DateTime.Compare(dt, fp.InitialLocation.MyDateTime) >= 0) && (DateTime.Compare(dt, end) <= 0))
                {
                   flights.Add(GetFlight(fp, dt));
                }
            }
            return flights;

        }
    }
}
