using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    
    public class Utils
    {
        // return an array with the valid flights only
        public JArray GetValidFlightsJArray(JArray flightsArray)
        {
            var array = new JArray();
            foreach (var flightJtoken in flightsArray)
            {
                if (IsFlightJtokenValid(flightJtoken))
                {
                    array.Add(flightJtoken);
                }
            }
            return array;
        }

        // check that the given flight jtoken is valid
        private bool IsFlightJtokenValid(JToken flightJson)
        {
            double latitude;
            double longitude;
            bool longitudeFlag = false;
            bool latitudeFlag = false;
            bool passengersFlag = false;
            bool companyNameFlag = false;
            bool myDateTimeFlag = false;
            bool idFlag = false;
            bool externalFlag = false;
            // holds the latitude and longitude validation check
            bool success = true;
            var flightElements = flightJson.ToArray();
            if (flightElements.Length != 7)
            {
                return false;
            }
            foreach (JProperty element in flightElements)
            {
                switch (element.Name)
                {
                    case "longitude":
                        longitudeFlag = true;
                        longitude = (double)element.Value;
                        // check if the given longitude is in the valid range
                        success = success && IsLongitudeValid(longitude);
                        break;
                    case "latitude":
                        latitudeFlag = true;
                        latitude = (double)element.Value;
                        // check if the given latitude is in the valid range
                        success = success && IsLatitudeValid(latitude);
                        break;
                    case "passengers":
                        passengersFlag = true;
                        break;
                    case "company_name":
                        companyNameFlag = true;
                        break;
                    case "date_time":
                        myDateTimeFlag = true;
                        break;
                    case "is_external":
                        externalFlag = true;
                        break;
                    case "flight_id":
                        idFlag = true;
                        break;
                    default:
                        return false;
                }
            }
            if (!(longitudeFlag && latitudeFlag && idFlag && externalFlag && myDateTimeFlag
                && passengersFlag && companyNameFlag))
            {
                return false;
            }
            return success;
        }

        // check if the given flight has valid parameters
        public bool IsFlightValid(Flight flight)
        {
            if (flight == null)
            {
                return false;
            }
            if ((flight.MyDateTime == null) || (flight.FlightId == null)
                || (flight.CompanyName == null))
            {
                return false;
            }
            return true;
        }

        // check if the given flight plan jobject is valid
        public bool IsFlightPlanJObjectValid(JObject flightPlanJson)
        {
            // check that all properties exist
            if (!(flightPlanJson.ContainsKey("passengers")
                && flightPlanJson.ContainsKey("company_name")
                && flightPlanJson.ContainsKey("initial_location")
                && flightPlanJson.ContainsKey("segments")))
            {
                return false;
            }
            var initialLocation = flightPlanJson["initial_location"];
            // check that the initial location properties exist
            if (!IsInitialLocationValid(initialLocation))
            {
                return false;
            }
            // trying to parse the segments into an array and each segment to segment elements
            try
            {
                var segmentsArray = (JArray)flightPlanJson["segments"];
                return IsSegmentsArrayValid(segmentsArray);
            }
            catch
            {
                return false;
            }
        }

        // check that the given initial location is valid
        private bool IsInitialLocationValid(JToken initialLocation)
        {
            double latitude;
            double longitude;
            bool longitudeFlag = false;
            bool latitudeFlag = false;
            bool dateTimeFlag = false;
            // holds the latitude and longitude validation check
            bool success = true;
            var locationElements = initialLocation.ToArray();
            if (locationElements.Length != 3)
            {
                return false;
            }
            foreach (JProperty element in locationElements)
            {
                switch (element.Name)
                {
                    case "longitude":
                        longitudeFlag = true;
                        longitude = (double)element.Value;
                        // check if the given longitude is in the valid range
                        success = success && IsLongitudeValid(longitude);
                        break;
                    case "latitude":
                        latitudeFlag = true;
                        latitude = (double)element.Value;
                        // check if the given latitude is in the valid range
                        success = success && IsLatitudeValid(latitude);
                        break;
                    case "date_time":
                        dateTimeFlag = true;
                        break;
                    default:
                        return false;
                }
            }
            if (!(longitudeFlag && latitudeFlag && dateTimeFlag))
            {
                return false;
            }
            return success;
        }

        private bool IsSegmentsArrayValid(JArray segmentsArray)
        {
            // for each segment, check that all of his properties exist
            foreach (var segment in segmentsArray)
            {
                if (!IsSegmentValid(segment))
                {
                    return false;
                }
            }
            return true;
        }

        // check that the given segment is valid
        private bool IsSegmentValid(JToken segment)
        {
            double latitude;
            double longitude;
            bool longitudeFlag = false;
            bool latitudeFlag = false;
            bool timespanFlag = false;
            // holds the latitude and longitude validation check
            bool success = true;
            var segmentElements = segment.ToArray();
            if (segmentElements.Length != 3)
            {
                return false;
            }
            foreach (JProperty element in segmentElements)
            {
                switch (element.Name)
                {
                    case "longitude":
                        longitudeFlag = true;
                        longitude = (double)element.Value;
                        // check if the given longitude is in the valid range
                        success = success && IsLongitudeValid(longitude);
                        break;
                    case "latitude":
                        latitudeFlag = true;
                        latitude = (double)element.Value;
                        // check if the given latitude is in the valid range
                        success = success && IsLatitudeValid(latitude);
                        break;
                    case "timespan_seconds":
                        timespanFlag = true;
                        break;
                    default:
                        return false;
                }
            }
            if (!(longitudeFlag && latitudeFlag && timespanFlag))
            {
                return false;
            }
            return success;
        }

        // check if the given flight plan has valid parameters
        public bool IsFlightPlanValid(FlightPlan flightPlan)
        {
            if (flightPlan == null)
            {
                return false;
            }
            if ((flightPlan.CompanyName == null) || (flightPlan.InitialLocation == null)
                || (flightPlan.Segments == null))
            {
                return false;
            }
            if (flightPlan.InitialLocation.MyDateTime == null)
            {
                return false;
            }
            return true;
        }

        // check if the given latitude is in the valid range
        private bool IsLatitudeValid(double latitude)
        {
            return ((latitude <= 90) && (latitude >= -90));
        }

        // check if the given longitude is in the valid range
        private bool IsLongitudeValid(double longitude)
        {
            return ((longitude <= 180) && (longitude >= -180));
        }
    }
}
