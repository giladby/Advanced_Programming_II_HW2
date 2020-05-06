var flightPlanUrl = "../api/FlightPlan";
function AddFlightPlan(flightPlan) {
    //$.post(flightPlanUrl, flightPlan);
    $.ajax({
        type: 'post',
        url: flightPlanUrl,
        data: JSON.stringify(flightPlan),
        contentType: "application/json; charset=utf-8",
        traditional: true
    });
}