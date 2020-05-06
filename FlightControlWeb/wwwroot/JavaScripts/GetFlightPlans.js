var flightUrl = "../api/Flights?relative_to=2020-05-06T10:00:00Z";
function GetFlightPlans() {
    $.get(flightUrl, function (data) {
        console.log(myFlightsTable);
        $("#myFlightsTable").empty()
        
        data.forEach(function (flight) {
            console.log(flight);
            $("#myFlightsTable").append("<tr><td>" + flight.flightId + "</td>" + "<td>" + flight.companyName + "</td>" + "</tr>")
        });
    });
}
