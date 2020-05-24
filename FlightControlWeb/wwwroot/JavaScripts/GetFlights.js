// the main get flights loop
async function flightsLoop() {
    while (true) {
        // get flights every 1 second
        getFlightsFunc();
        await sleep(1000);
    }
}

var flightsArr = [];
function getFlightsFunc() {
    // get the current date and time
    let dateAndTime = makeDateAndTime();
    let dummyArr = [];
    let flightUrl = "../api/Flights?relative_to=" + dateAndTime + "&sync_all";
    // send a flights 'GET' request with the current date and time to the server
    $.get(flightUrl, function (data) {
        analyzeFlightsData(data, dummyArr);
    }).fail(function (error) {
        printError(error.responseText);
    });
}

function analyzeFlightsData(data, dummyArr) {
    if (!data) {
        printError("Received invalid flight");
        return;
    }
    data.forEach(function (flight) {
        forEachFlightFunc(flight, dummyArr);
    });
    // check if any flights needs to be deleted from the map and tables
    flightsArr.forEach(function (item) {
        forEachFlightsArrRemoveFunc(item, dummyArr);
    });
    flightsArr = dummyArr.slice();
}

function forEachFlightFunc(flight, dummyArr) {
    if (!isFlightValid(flight)) {
        printError("Received invalid flight");
        return;
    }
    // add that flight to the map and to the relevant table
    addFlightToMapAndTables(flight, dummyArr);
}

function forEachFlightsArrRemoveFunc(item, dummyArr) {
    // if this flight needs to be removed
    if (!item[1]) {
        deleteAirplane(item[0], false);
        $("#" + item[0]).remove();
    } else {
        dummyArr.push([item[0], false]);
    }
}

function addFlightToMapAndTables(flight, dummyArr) {
    let id = flight.flight_id;
    let latitude = flight.latitude;
    let longitude = flight.longitude;
    let external = flight.is_external;
    let company = flight.company_name;
    let exist = false;
    // check if the flight already exists
    flightsArr.forEach(function (item) {
        if (forEachFlightsArrChangeLocation(item, id, latitude, longitude)) {
            exist = true;
        }
    });
    // if need to add new flight
    if (!exist) {
        // adds the flight to the relevant table
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

// if the given item has the given id so change his location and return true
function forEachFlightsArrChangeLocation(item, id, latitude, longitude, exist) {
    if (item[0] == id) {
        item[1] = true;
        changeAirplaneLocation(id, latitude, longitude);
        return true;
    } else {
        return false;
    }
}

// deletes the given flight id
function deleteByButton(id) {
    event.stopPropagation();
    if (currentMarked != null && currentMarked.get('name') == id) {
        emptyCurrentMarked(false);
    }
    deleteFlightFunc(id);
}
