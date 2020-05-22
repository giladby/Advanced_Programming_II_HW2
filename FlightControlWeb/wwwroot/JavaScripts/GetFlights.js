
async function FlightsLoop() {
    while (true) {
        GetFlightsFunc();
        await Sleep(1000);
    }
}


var flightsArr = [];
function GetFlightsFunc() {
    let dateAndTime = MakeDateAndTime();
    let dummyArr = [];
    let flightUrl = "../api/Flights?relative_to=" + dateAndTime + "&sync_all";
    $.get(flightUrl, function (data) {
        data.forEach(function (flight) {
            let id = flight.flight_id;
            let latitude = flight.latitude;
            let longitude = flight.longitude;
            let external = flight.is_external;
            let company = flight.company_name;
            let exist = false;
            flightsArr.forEach(function (item) {
                if (item[0] == id) {
                    item[1] = true;
                    exist = true;
                    ChangeAirplaneLocation(id, latitude, longitude);
                    return;
                }
            });
            if (!exist) {
                if (external) {
                    let tr = "<tr id=\"" + id + "\">" +
                        "<td>" + id + "</td><td>" + company + "</td></tr>";
                    $("#externalTableBody").append(tr);

                } else {
                    let tr = "<tr id=\"" + id + "\"><td><input type =\"button\" value=\"X\" onclick=\"deleteByButton('"
                        + id + "')\"/></td>" +
                        "<td>" + id + "</td><td>" + company + "</td></tr>";
                    $("#flightsTableBody").append(tr);
                }
                
                let source = createSource(latitude, longitude);
                let row = document.getElementById(id);
                row.onclick = function () {
                    tryMarkAirplane(id, source, external);
                }
                dummyArr.push([id, false]);
                AddAirplane(id, source, external, defaultAirplaneImage, defaultAirplaneScale);
            }
        });
        flightsArr.forEach(function (item) {
            if (!item[1]) {
                DeleteAirplane(item[0], false);
                $("#" + item[0]).remove();
            } else {
                dummyArr.push([item[0], false]);
            }
        });
        flightsArr = dummyArr.slice();
    }).fail(function (error) {
        printError(error.responseText);
    });
}

function deleteByButton(id) {
    event.stopPropagation();
    if (currentMarked != null && currentMarked.get('name') == id) {
        emptyCurrentMarked(false);
    }
    DeleteFlightFunc(id);
}

function MakeDateAndTime() {
    let date = new Date();
    let dateAndTime = new Date(date.getTime()).toISOString();
    dateAndTime = dateAndTime.substr(0, (dateAndTime.length - 5)) + "Z";
    return dateAndTime;
}
