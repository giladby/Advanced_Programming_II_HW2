﻿async function FlightsLoop() {
    while (true) {
        GetFlightPlans();
        await Sleep(250);
    }
}


var flightsArr = [];
function GetFlightPlans() {
    
    var dateAndTime = MakeDateAndTime();
    var dummyArr = [];
    var flightUrl = "../api/Flights?relative_to=" + dateAndTime;
    $.get(flightUrl, function (data) {
        data.forEach(function (flight) {
            var id = flight.flightId;
            var exist = false;
            flightsArr.forEach(function (item) {
                if (item[0] == id) {
                    item[1] = true;
                    exist = true;
                    ChangeAirplaneLocation(id, flight.latitude, flight.longitude);
                    return;
                }
            });
            if (!exist) {
                var tr = "<tr id=\"" + id + "\"><td><input type =\"button\" value=\"X\" onclick=\"DeleteFlightFunc('" + id + "')\"/></td>" +
                    "<td>" + id + "</td><td>" + flight.companyName + "</td></tr>";
                $("#flightsTableBody").append(tr);
                dummyArr.push([id, false]);
                let source = new ol.source.Vector({
                    features: [new ol.Feature({
                        geometry: new ol.geom.Point(ol.proj.fromLonLat([flight.longitude, flight.latitude])),
                    })]
                });
                AddAirplane(id, source, flight.isExternal, defaultAirplaneImage, defaultAirplaneScale);
            }
        });
        flightsArr.forEach(function (item) {
            if (!item[1]) {
                $("#" + item[0]).remove();
                DeleteAirplane(item[0]);
            } else {
                dummyArr.push([item[0], false]);
            }
        });
        flightsArr = dummyArr.slice();
    });
}
    
function MakeDateAndTime() {
    var date = new Date();
    var dateAndTime = new Date(date.getTime()).toISOString();
    dateAndTime = dateAndTime.substr(0, (dateAndTime.length - 5)) + "Z";
    return dateAndTime;
}

function Sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}
