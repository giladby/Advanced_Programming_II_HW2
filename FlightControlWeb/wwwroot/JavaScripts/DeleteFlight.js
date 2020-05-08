function DeleteFlightFunc(flightId) {
    var flightUrl = "../api/Flights/" + flightId;
    $.ajax({
        type: 'delete',
        url: flightUrl,
        contentType: "application/json; charset=utf-8",
        traditional: true
    });
}