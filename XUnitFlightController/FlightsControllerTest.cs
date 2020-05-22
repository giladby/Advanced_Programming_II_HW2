using FlightControlWeb.Controllers;
using FlightControlWeb.Models;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Moq;
using System;
using System.Net.WebSockets;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Threading.Tasks;

namespace XUnitFlightController
{
    public class FlightsControllerTest
    {
        [Fact]
        public async Task GetFlightsAllTests()
        {
            await GetFlightsTestSuccess();
            await GetFlightsTestMyFlightsFail();
            await GetFlightsTestServersFail();
        }

        private async Task GetFlightsTestSuccess()
        {
            await GetFlightsTest(false, false);
        }

        private async Task GetFlightsTestMyFlightsFail()
        {
            await GetFlightsTest(true, false);
        }

        private async Task GetFlightsTestServersFail()
        {
            await GetFlightsTest(false, true);
        }
        
        private async Task GetFlightsTest(bool myFlightsFailFlag, bool serversFailFlag)
        {
            //Arrange
            string relativeTo = "2020-10-10T10:10:10Z";
            DateTime dateTime = DateTime.ParseExact(relativeTo, "yyyy-MM-ddTHH:mm:ssZ",
                System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
            Flight flight1 = new Flight("AA1111", 10, 10, 100, "El-Al", dateTime, false);
            Flight flight2 = new Flight("BB2222", 20, 20, 200, "Arkia", dateTime, false);
            Flight flight3 = new Flight("CC3333", 30, 30, 300, "SwissAir", dateTime, true);
            Flight flight4 = new Flight("DD4444", 40, 40, 400, "Aeroflot", dateTime, true);
            Flight flight5 = new Flight("EE5555", 50, 50, 500, "Lufthansa", dateTime, true);
            Flight flight6 = new Flight("FF6666", 60, 60, 600, "QatarAirways", dateTime, true);
            ArrayList myFlights = new ArrayList { flight1, flight2, flight3 };
            ArrayList externalFlights = new ArrayList { flight4, flight5, flight6 };
            var flightsMock = new Mock<IFlightsManager>();
            var serversMock = new Mock<IServersManager>();
            if (myFlightsFailFlag)
            {
                flightsMock.Setup(flightsMock => flightsMock.GetFlightsByTime(dateTime)).Throws(new Exception());
            } else
            {
                flightsMock.Setup(flightsMock => flightsMock.GetFlightsByTime(dateTime)).Returns(myFlights);
            }
            serversMock.Setup(serversMock => serversMock.GetExternalFlights(dateTime))
                .Returns(new Tuple<bool, ArrayList>(serversFailFlag, externalFlights));
            var context = new Mock<HttpContext>();
            context.SetupGet(x => x.Request.QueryString).Returns(new QueryString("?sync_all"));
            var controllerContext = new ControllerContext()
            {
                HttpContext = context.Object,
            };
            var flightsController = new FlightsController(flightsMock.Object, serversMock.Object)
            {
                ControllerContext = controllerContext,
            };
            // Act
            var flights = await flightsController.GetFlights(relativeTo);
            // Assert
            if (myFlightsFailFlag)
            {
                Assert.IsType<BadRequestObjectResult>(flights);
                var badRequestResult = flights as BadRequestObjectResult;
                Assert.Equal("Failed receiving flights", badRequestResult.Value);
            }
            else if (serversFailFlag)
            {
                Assert.IsType<BadRequestObjectResult>(flights);
                var badRequestResult = flights as BadRequestObjectResult;
                Assert.Equal("Failed receiving flights from servers", badRequestResult.Value);
            }
            else
            {
                Assert.IsType<OkObjectResult>(flights);
                var okResult = flights as OkObjectResult;
                Assert.IsType<ArrayList>(okResult.Value);
                var list = okResult.Value as ArrayList;
                Assert.Equal(6, list.Count);
                foreach (object obj in list)
                {
                    Assert.IsType<Flight>(obj);
                    var flight = obj as Flight;
                    Assert.IsType<string>(flight.FlightId);
                    Assert.IsType<double>(flight.Longitude);
                    Assert.IsType<double>(flight.Latitude);
                    Assert.IsType<int>(flight.Passengers);
                    Assert.IsType<string>(flight.CompanyName);
                    Assert.IsType<DateTime>(flight.MyDateTime);
                    Assert.IsType<bool>(flight.IsExternal);
                }
            }
        }
    }
}
