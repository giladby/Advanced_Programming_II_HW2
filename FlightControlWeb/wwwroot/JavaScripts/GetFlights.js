async function flightsLoop() {
    while (true) {
        getFlightsFunc();
        await sleep(1000);
    }
}

var flightsArr = [];
function getFlightsFunc() {
    let dateAndTime = makeDateAndTime();
    let dummyArr = [];
    let flightUrl = "../api/Flights?relative_to=" + dateAndTime + "&sync_all";
    $.get(flightUrl, function (data) {
        if (!data) {
            printError("Received invalid flight");
            return;
        } 
        data.forEach(function (flight) {
            if (!isFlightValid) {
                printError("Received invalid flight");
                return;
            }
            addFlightToMapAndTables(flight, dummyArr);
        });
        flightsArr.forEach(function (item) {
            if (!item[1]) {
                deleteAirplane(item[0], false);
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

function addFlightToMapAndTables(flight, dummyArr) {
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
            changeAirplaneLocation(id, latitude, longitude);
            return;
        }
    });
    if (!exist) {
        if (external) {
            let tr = "<tr id=\"" + id + "\">" +
                "<td>" + id + "</td><td>" + company + "</td></tr>";
            $("#externalTableBody").append(tr);

        } else {
            let tr = "<tr id=\"" + id + "\">" +
                "<td>" + id + "</td><td>" + company + "</td>" +
                "<td><input type=\"button\" class=\"btn btn-danger btn-sm\"" +
                "value =\"X\" onclick=\"deleteByButton('" +
                id + "')\"/></td></tr>";
            $("#flightsTableBody").append(tr);
        }
        let source = createSource(latitude, longitude);
        let row = document.getElementById(id);
        row.onclick = function () {
            tryMarkAirplane(id, source);
        }
        dummyArr.push([id, false]);
        addAirplane(id, source, defaultAirplaneImage, defaultAirplaneScale);
    }
}

function deleteByButton(id) {
    event.stopPropagation();
    if (currentMarked != null && currentMarked.get('name') == id) {
        emptyCurrentMarked(false);
    }
    deleteFlightFunc(id);
}
