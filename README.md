Advanced_Programming_II_HW2

In this project, we built a web application using ASP.NET core.
This application is a flights control system,
showing flight plans on a world map and providing live information about the current active flights.
In addition to the ability to track flights from this project server, the application can connect to other external servers
and get information about their own flights.

The client side is written with JavaScript, and designed with Bootstrap.
The server side is written with C#.



In addition, We implemented a unit test for the FlightsController class.
This unit test is testing the 'GetFlights' function of the FlightsController,
this function is the main function of this controller.

This function uses the 2 managers of the program:
The internal flights manager and the external servers manager.
So in order to test this function of the controller, we made 2 mocks - one for each manager.

The unit test is testing all the 3 return options of this function:

1. The success option: Checks that the function really returns an array of flights,
with flights from our server and flights from the external servers.
2. The internal server failure: If our server returns an error,
checks that the controller returns the according error as well.
3. The external servers failure - If the external servers returns an error,
checks that the controller returns the according error as well.
