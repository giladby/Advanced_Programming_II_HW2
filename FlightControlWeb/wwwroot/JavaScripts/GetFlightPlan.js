let myData;

function GetFlightPlanFunc(flightId) {
    var flightPlanUrl = "../api/FlightPlan/" + flightId;
    $.get(flightPlanUrl, function (data) {
        console.log(data);
        getData(data);
    });

    console.log(myData);
    return myData;
}

function getData(data) {
    myData = data
}