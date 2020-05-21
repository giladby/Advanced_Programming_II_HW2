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

        private void SaveFlightPlansAndIds(List<FlightPlan> flightPlans, Dictionary<string, FlightPlan> flightIds)
        {
            SaveFlightPlans(flightPlans);
            SaveFlightIds(flightIds);
        }

        private void SaveFlightPlans(List<FlightPlan> flightPlans)
        {
            myCache.Set("flightPlans", flightPlans);
        }

        private void SaveFlightIds(Dictionary<string, FlightPlan> flightIds)
        {
            myCache.Set("flightIds", flightIds);
        }

        private Dictionary<string, FlightPlan> GetFlightIds()
        {
            Dictionary<string, FlightPlan> flightIds;
            if (!myCache.TryGetValue("flightIds", out flightIds))
            {
                if (flightIds == null)
                {
                    flightIds = new Dictionary<string, FlightPlan>();
                }
            }
            SaveFlightIds(flightIds);
            return flightIds;
        }

        private List<FlightPlan> GetFlightPlans()
        {
            List<FlightPlan> flightPlans;
            if (!myCache.TryGetValue("flightPlans", out flightPlans))
            {
                if (flightPlans == null)
                {
                    flightPlans = new List<FlightPlan>();
                }
            }
            SaveFlightPlans(flightPlans);
            return flightPlans;
        }

        private void AddFlightPlanToCache(FlightPlan fp, string id)
        {
            List<FlightPlan> flightPlans = GetFlightPlans();
            Dictionary<string, FlightPlan> flightIds = GetFlightIds();
            flightIds.Add(id, fp);
            flightPlans.Add(fp);
            SaveFlightPlansAndIds(flightPlans, flightIds);
        }

        private string GenerateFlightId()
        {
            string id = "";
            Random random = new Random();
            string allChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < 2; i++)
            {
                id += allChars[random.Next(allChars.Length)];
            }
            for (int i = 0; i < 4; i++)
            {
                id += random.Next(0, 10).ToString();
            }
            return id;
        }

        public void AddFlightPlan(FlightPlan flightPlan)
        {
            string id;
            Dictionary<string, FlightPlan> flightIds = GetFlightIds();
            while (true)
            {
                id = GenerateFlightId();
                if (!flightIds.ContainsKey(id))
                {
                    break;
                }
            }
            AddFlightPlanToCache(flightPlan, id);
        }

        public bool DeleteFlightPlan(string id)
        {
            bool deleted = false;
            var flightPlans = GetFlightPlans();
            var flightIds = GetFlightIds();
            if (flightIds.ContainsKey(id))
            {
                var myFlightPlan = flightIds[id];
                flightPlans.Remove(myFlightPlan);
                flightIds.Remove(id);
                SaveFlightPlansAndIds(flightPlans, flightIds);
                deleted = true;
            }
            return deleted;
        }

        public FlightPlan GetFlightPlan(string id)
        {
            var flightIds = GetFlightIds();
            if (flightIds.ContainsKey(id))
            {
                return flightIds[id];
            }
            return null;
        }

        private Tuple<double, double> GetLocation(FlightPlan flightPlan, DateTime dateTime)
        {
            double longitude = flightPlan.InitialLocation.Longitude;
            double latitude = flightPlan.InitialLocation.Latitude;
            var startTime = flightPlan.InitialLocation.MyDateTime;
            double timeGap = (dateTime - startTime).TotalSeconds;
            double distance;
            double ratio;
            if (timeGap == 0)
            {
                return new Tuple<double, double>(longitude, latitude);
            }
            foreach (FlightSegment currentSegment in flightPlan.Segments)
            {
                if (timeGap <= currentSegment.TimespanSeconds)
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

        public Flight GetFlight(FlightPlan flightPlan, DateTime dateTime)
        {
            var location = GetLocation(flightPlan, dateTime);
            double longitude = location.Item1;
            double latitude = location.Item2;
            var flightIds = GetFlightIds();
            string id = flightIds.FirstOrDefault(x => x.Value == flightPlan).Key;
            if (id == null)
            {
                return null;
            }
            return new Flight(id, longitude, latitude, flightPlan.Passengers, flightPlan.CompanyName, dateTime, false);
        }

        private DateTime GetEndTime(FlightPlan flightPlan)
        {
            var time = flightPlan.InitialLocation.MyDateTime;
            foreach (FlightSegment segment in flightPlan.Segments)
            {
                try
                {
                    time = time.AddSeconds(segment.TimespanSeconds);
                }
                catch
                {
                    return DateTime.MaxValue;
                }
            }
            return time;
        }

        public ArrayList GetFlightsByTime(DateTime dateTime)
        {
            var flights = new ArrayList();
            var flightPlans = GetFlightPlans();
            foreach (FlightPlan flightPlan in flightPlans)
            {
                var endTime = GetEndTime(flightPlan);
                if ((DateTime.Compare(dateTime, flightPlan.InitialLocation.MyDateTime) >= 0)
                    && (DateTime.Compare(dateTime, endTime) <= 0))
                {
                    flights.Add(GetFlight(flightPlan, dateTime));
                }
            }
            return flights;
        }
    }
}
