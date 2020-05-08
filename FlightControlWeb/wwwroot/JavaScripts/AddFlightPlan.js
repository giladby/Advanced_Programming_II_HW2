var flightPlanUrl = "../api/FlightPlan";
function AddFlightPlanFunc(flightPlan) {
    $.ajax({
        type: 'post',
        url: flightPlanUrl,
        data: JSON.stringify(flightPlan),
        contentType: "application/json; charset=utf-8",
        traditional: true
    });
}