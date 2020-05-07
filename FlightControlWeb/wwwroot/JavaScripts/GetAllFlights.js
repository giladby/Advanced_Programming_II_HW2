
async function FlightsLoop() {
    while (true) {
        GetFlightPlans();
        await Sleep(250);
    }
}

function Sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

var flightsArr = [];

function GetFlightPlans() {

    var dateAndTime = makeDateAndTime();
    var dummyArr = [];
    var flightUrl = "../api/Flights?relative_to=" + dateAndTime;
    $.get(flightUrl, function (data) {
        data.forEach(function (flight) {
            var id = flight.flightId;
            var exist = false;
            flightsArr.forEach(function (item) {
                if (item[0] == id) {
                    item[1] = true;
                    exist = true;
                    return;
                }
            });
            if (!exist) {
                var tr = "<tr id=\"" + id + "\"><td><input type =\"button\" value=\"X\" onclick=\"deleteFlight('" + id + "')\"/></td>" +
                    "<td>" + id + "</td><td>" + flight.companyName + "</td></tr>";
                $("#flightsTableBody").append(tr);
                dummyArr.push([id, false]);
            }
        });
        flightsArr.forEach(function (item) {
            if (!item[1]) {
                $("#" + item[0]).remove();
            } else {
                dummyArr.push([item[0], false]);
            }
        });
        flightsArr = dummyArr.slice();
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
    var flightUrl = "../api/Flights/" + flightId;
    $.ajax({
        type: 'delete',
        url: flightUrl,
        contentType: "application/json; charset=utf-8",
        traditional: true
    });
}