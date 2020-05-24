// send a 'DELETE' request with the given flight id to the server
function deleteFlightFunc(flightId) {
    let flightUrl = "../api/Flights/" + flightId;
    $.ajax({
        type: 'delete',
        url: flightUrl,
        contentType: "application/json; charset=utf-8",
        traditional: true,
        error: function (error) {
            printError(error.responseText);
        }
    });
}