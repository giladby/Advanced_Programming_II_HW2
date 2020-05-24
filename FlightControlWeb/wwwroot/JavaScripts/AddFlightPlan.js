let flightPlanUrl = "../api/FlightPlan";
// send a 'POST' request with the given flight plan to the server
function addFlightPlanFunc(flightPlan) {
    $.ajax({
        type: 'post',
        url: flightPlanUrl,
        data: JSON.stringify(flightPlan),
        contentType: "application/json; charset=utf-8",
        traditional: true,
        error: function () {
            printError("Received invalid flight plan");
        }
    });
}