
async function FlightsLoop() {
    while (true) {
        GetFlightPlans();
        await Sleep(250);
    }
}

function Sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function GetFlightPlans() {
    var dateAndTime = makeDateAndTime();
    var flightUrl = "../api/Flights?relative_to=" + dateAndTime;
    $.get(flightUrl, function (data) {
        $("#flightsTableBody").empty();
        data.forEach(function (flight) {
            //$("#myFlightsTable").append("<tr><td><button onclick='deleteFlight(" + flight.flightId +
            //    ")'>" + X + "</button></td>" + "<td>" + flight.flightId + "</td>" + "<td>" + flight.companyName + "</td>" + "</tr>")
            var tr = "<tr><td>" + flight.flightId + "</td>" + "<td>" + flight.companyName + "</td>" + "<td>" + flight.latitude + "</td>" +"</tr>";
            $("#flightsTableBody").append(tr);
        });
    });
}

function makeDateAndTime() {
    //    5/7/2020, 1:24:06 PM   =>    2020-05-07T13:24:06Z
    var date = new Date();
    var dateAndTime = new Date(date.getTime()).toISOString();
    dateAndTime = dateAndTime.substr(0, (dateAndTime.length - 5)) + "Z";
    return dateAndTime;
}

function deleteFlight(flightId) {
    console.log(flightId);
}