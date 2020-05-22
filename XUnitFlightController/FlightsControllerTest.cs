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
        private FlightsController flightsController;
        private Mock<IFlightsManager> flightsMock;
        private Mock<IServerManager> serversMock;

        public FlightsControllerTest()
        {
            flightsMock = new Mock<IFlightsManager>();
            serversMock = new Mock<IServerManager>();

        }

        private async Task GetFlightsTest(bool resultBool)
        {
            //Arrange
            string relativeTo = "2020-05-22T11:14:12Z";
            DateTime time = DateTime.ParseExact(relativeTo, "yyyy-MM-ddTHH:mm:ssZ",
                System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();

            Flight f1 = new Flight("AA1121", 50, 50, 100, "Arkia", time, false);
            Flight f2 = new Flight("AA8121", 55, 12, 100, "El-Al", time, false);
            Flight f3 = new Flight("AB0121", 55, 12, 120, "company", time, true);
            ArrayList myFlights = new ArrayList { f1, f2 };
            ArrayList externalFlights = new ArrayList { f3 };

            var context = new Mock<HttpContext>();
            context.SetupGet(x => x.Request.QueryString).Returns(new QueryString("?sync_all"));

            var controllerContext = new ControllerContext()
            {
                HttpContext = context.Object,
            };

            flightsMock.Setup(flightsMock => flightsMock.GetFlightsByTime(time)).Returns(myFlights);
            serversMock.Setup(serversMock => serversMock.GetExternalFlights(time)).Returns(new Tuple<bool, ArrayList>(resultBool, externalFlights));

            //assign context to controller
            flightsController = new FlightsController(flightsMock.Object, serversMock.Object)
            {
                ControllerContext = controllerContext,
            };

            // Act
            var flights = await flightsController.GetFlights(relativeTo);


            // Assert
            if (resultBool)
            {
                Assert.IsType<BadRequestObjectResult>(flights);
            }
            else
            {
                Assert.IsType<OkObjectResult>(flights);

                var okResult = flights as OkObjectResult;
                Assert.IsAssignableFrom<ArrayList>(okResult.Value);

                var list = okResult.Value as ArrayList;
                Assert.Equal(3, list.Count);
                foreach (object obj in list)
                {
                    Assert.IsType<Flight>(obj);
                }
            }

        }


        [Fact]
        public async Task GetFlightsTestSuccess()
        {
            await GetFlightsTest(false);
        }

        [Fact]
        public async Task GetFlightsTestFailed()
        {
            await GetFlightsTest(true);
        }
    }

}
