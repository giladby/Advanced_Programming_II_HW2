function DeleteFlightFunc(flightId) {
    let flightUrl = "../api/Flights/" + flightId;
    $.ajax({
        type: 'delete',
        url: flightUrl,
        contentType: "application/json; charset=utf-8",
        traditional: true,
        error: function () {
            printError("Failed deleting flight from server");
        }
    });
}