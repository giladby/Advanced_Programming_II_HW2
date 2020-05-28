using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class MyFlightsManager : IFlightsManager
    {
        private IMemoryCache myCache;

        public MyFlightsManager(IMemoryCache cache)
        {
            myCache = cache;
        }

        private void SaveFlightPlansAndIds(List<FlightPlan> flightPlans,
            Dictionary<string, FlightPlan> flightIds)
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

        // get the flight ids dictionary from the cache
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

        // get the flight plans list from the cache
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

        // generate a random id string of the form 'AA1111'
        private string GenerateFlightId()
        {
            string id = "";
            Random random = new Random();
            string allChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < 2; i++)
            {
                int rand = random.Next(allChars.Length);
                id += allChars[rand];
            }
            for (int i = 0; i < 4; i++)
            {
                id += random.Next(0, 10).ToString();
            }
            return id;
        }
        // add the given flight plan, return false if failed
        public bool AddFlightPlan(FlightPlan flightPlan)
        {
            var myUtils = new Utils();
            // if the flight plan is invalid
            if (!myUtils.IsFlightPlanValid(flightPlan))
            {
                return false;
            }
            string id;
            Dictionary<string, FlightPlan> flightIds = GetFlightIds();
            // generate id until it is unique
            while (true)
            {
                id = GenerateFlightId();
                if (!flightIds.ContainsKey(id))
                {
                    break;
                }
            }
            AddFlightPlanToCache(flightPlan, id);
            return true;
        }

        // delete a flight plan with the given id, return false if the flight was not found
        public bool DeleteFlightPlan(string id)
        {
            bool deleted = false;
            var flightPlans = GetFlightPlans();
            var flightIds = GetFlightIds();
            // if the flight plan was found
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

        // get the location of the given flight plan relative to the given dateTime
        private Tuple<double, double> GetLocation(FlightPlan flightPlan, DateTime dateTime)
        {
            double longitude = flightPlan.InitialLocation.Longitude;
            double latitude = flightPlan.InitialLocation.Latitude;
            var startTime = flightPlan.InitialLocation.MyDateTime;
            double timeGap = (dateTime - startTime).TotalSeconds;
            double distance;
            double ratio;
            // if the given time is the initial time
            if (timeGap == 0)
            {
                return new Tuple<double, double>(longitude, latitude);
            }
            foreach (FlightSegment currentSegment in flightPlan.Segments)
            {
                // if the given time is in this segment
                if (timeGap <= currentSegment.TimespanSeconds)
                {
                    // get the time ratio
                    ratio = timeGap / currentSegment.TimespanSeconds;
                    // calculate the new longitude and latitude with the ratio addition
                    distance = currentSegment.Longitude - longitude;
                    longitude += ratio * distance;
                    distance = currentSegment.Latitude - latitude;
                    latitude += ratio * distance;
                    break;
                }
                // remove this segment time
                timeGap -= currentSegment.TimespanSeconds;
                longitude = currentSegment.Longitude;
                latitude = currentSegment.Latitude;
            }
            return new Tuple<double, double>(longitude, latitude);
        }

        // create a flight with the given dateTime from the given flight plan
        public Flight GetFlight(FlightPlan flightPlan, DateTime dateTime)
        {
            var location = GetLocation(flightPlan, dateTime);
            double longitude = location.Item1;
            double latitude = location.Item2;
            var flightIds = GetFlightIds();
            // get the flight plan id
            string id = flightIds.FirstOrDefault(x => x.Value == flightPlan).Key;
            if (id == null)
            {
                return null;
            }
            return new Flight(id, longitude, latitude, flightPlan.Passengers,
                flightPlan.CompanyName, dateTime, false);
        }

        // calculate the ending time of the given flight plan
        private DateTime GetEndTime(FlightPlan flightPlan)
        {
            var time = flightPlan.InitialLocation.MyDateTime;
            foreach (FlightSegment segment in flightPlan.Segments)
            {
                try
                {
                    time = time.AddSeconds(segment.TimespanSeconds);
                }
                // if the current segment timespan makes time bigger than the max time possible
                catch
                {
                    return DateTime.MaxValue;
                }
            }
            return time;
        }

        // get all the flights relative to the given dateTime
        public ArrayList GetFlightsByTime(DateTime dateTime)
        {
            var flights = new ArrayList();
            var flightPlans = GetFlightPlans();
            foreach (FlightPlan flightPlan in flightPlans)
            {
                var endTime = GetEndTime(flightPlan);
                // if the flight plan is active in the given dateTime
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


