let currentMarked = null;
function displayMap() {
    // the bing map key
    let myKey = 'nw9X5t1VORpRClqJavkK~XN8n9COo6PKhDoEQJKEZQA~' +
        'AtnOCAe8bf0NUKZkOULqwpGPW-7-diFVfJClHL_VUDdmlzB-SYSECuKD1K98KyLS';
    // adds the map with 0 zoom on israel
    map = new ol.Map({
        layers: [
            new ol.layer.Tile({
                name : 'mapLayer',
                visible: true,
                source: new ol.source.BingMaps({
                    key: myKey,
                    imagerySet: 'Road'
                })
            })
        ],
        controls: ol.control.defaults({
            attribution: false,
            zoom: false,
        }),
        target: 'map',
        view: new ol.View({
            center: ol.proj.transform([32.003657, 34.872770], 'EPSG:4326', 'EPSG:3857'),
            zoom: 0
        })
    });
    // when the map is being clicked
    map.on("click", function (e) {
        let isMapLayer = true;
        let found = false;
        map.forEachFeatureAtPixel(e.pixel, function (feature, layer) {
            // if this is a flight route
            if (layer.get('name') == 'lines' || found) {
                return;
            }
            isMapLayer = false;
            tryMarkAirplane(layer.get('name'), layer.getSource());
            found = true;
        })
        if (isMapLayer) {
            unmarkAirplane(false);
        }
    });
}

// trying to mark the airplane with the given name
function tryMarkAirplane(name, source) {
    if ((currentMarked != null) && (currentMarked.get('name') == name)) {
        return;
    }
    unmarkAirplane(true);
    getFlightPlanAndMark(name, source);
}

// send a flight plan 'GET' request with the given flight id to the server
function getFlightPlanAndMark(flightId, source) {
    let flightPlanUrl = "../api/FlightPlan/" + flightId;
    $.get(flightPlanUrl, function (data) {
        if (!isFlightPlanValid(data)) {
            printError("Received invalid flight plan");
            return;
        }
        // mark the airplane of the flight plan that was received
        markAirplane(data, flightId, source);
    }).fail(function (error) {
        printError(error.responseText);
    });
}

function createSource(latitude, longitude) {
    return new ol.source.Vector({
        features: [new ol.Feature({
            geometry: new ol.geom.Point(ol.proj.fromLonLat([longitude, latitude])),
        })]
    });
}

// adds a new airplane with the given parameters to the map
function addAirplane(flightId, source, image, scale) {
    let airplane = new ol.layer.Vector({
        name: flightId,
        source: source,
        style: new ol.style.Style({
            image: new ol.style.Icon(({
                src: image,
                scale: scale
            }))
        })
    });
    map.addLayer(airplane);
    return airplane;
}

// change the airplane location to the given coordinates
function changeAirplaneLocation(flightId, latitude, longitude) {
    map.getLayers().forEach(function (layer) {
        if (layer.get('name') == flightId) {
            layer.getSource().getFeatures()[0].setGeometry(
                new ol.geom.Point(ol.proj.fromLonLat([longitude, latitude])));
            return;
        }
    })
}

// delete the airplane with the given id
function deleteAirplane(flightId, update) {
    let layerToDelete = getLayer(flightId);
    if (layerToDelete == currentMarked) {
        emptyCurrentMarked(update);
    }
    map.removeLayer(layerToDelete);
}

// get the layer with the given name
function getLayer(name) {
    let myLayer;
    map.getLayers().forEach(function (layer) {
        if (layer.get('name') == name) {
            myLayer = layer;
            return;
        }
    });
    return myLayer;
}

// mark the airplane with the given name
function markAirplane(data, name, source) {
    deleteAirplane(name, true);
    let newAirplane = addAirplane(name, source, 'Images/markedAirplane.png', 0.1);
    // get a triple of latitude, longitude and end time
    let endInfo = getEndInformation(data);
    // adds the details of the flight to the details table
    let tr = "<tr id=\"flightDetailsRow\"><td>[" + data.initial_location.latitude +
             "," + data.initial_location.longitude + "]</td>" +
             "<td>[" + endInfo[0] + "," + endInfo[1] + "]</td>" +
             "<td>" + data.initial_location.date_time + "</td>" +
             "<td>" + endInfo[2] + "</td>" +
             "<td>" + data.company_name + "</td>" +
             "<td>" + data.passengers + "</td></tr>";
    let flightDetailsRow = document.getElementById("flightDetailsRow");
    if (flightDetailsRow) {
        $("#flightDetailsRow").replaceWith(tr);
    } else {
        $("#FlightDetailsBody").append(tr);
    }
    // mark the flight row in the flights table
    let myFlightsRow = document.getElementById(name);
    myFlightsRow.style.backgroundColor = "#ffd800";
    myFlightsRow.style.color = "#272727";
    // draw the flight route lines on the map
    drawRouteLines(data);
    currentMarked = newAirplane;
}

// unmark the current marked airplane
function unmarkAirplane(update) {
    if (currentMarked == null) {
        return;
    }
    let name = currentMarked.get('name');
    let previous = currentMarked;
    deleteAirplane(name, update);
    addAirplane(name, previous.getSource(), defaultAirplaneImage, defaultAirplaneScale);
}

// empty the current marked flight from the map and tables
function emptyCurrentMarked(update) {
    if (!update) {
        $("#FlightDetailsBody").empty();
    }
    let id = currentMarked.get('name');
    let myFlightsRow = document.getElementById(id);
    // unmark the flight row in the flights table
    myFlightsRow.style.backgroundColor = "";
    myFlightsRow.style.color = "";
    removeRouteLines();
    currentMarked = null;
}

// draw the given flight data route lines on the map
function drawRouteLines(data) {
    let pointsArr = [];
    pointsArr.push(ol.proj.fromLonLat([data.initial_location.longitude,
    data.initial_location.latitude]));
    data.segments.forEach(function (segment) {
        pointsArr.push(ol.proj.fromLonLat([segment.longitude, segment.latitude]));
    });
    let lineStyle = [
        new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: '#5162cb',
                lineDash: [4, 8],
                lineCap: 'square',
                width: 5
            })
        })
    ];
    let layer = new ol.layer.Vector({
        source: new ol.source.Vector({
            features: [new ol.Feature({
                geometry: new ol.geom.LineString(pointsArr)
            })]
        }),
        name: 'lines'
    });
    layer.setStyle(lineStyle);
    map.getLayers().insertAt(1, layer);

}

// remove the flight route lines from the map
function removeRouteLines() {
    let layerToDelete = getLayer("lines");
    map.removeLayer(layerToDelete);
}

// returns a triple of latitude, longitude and end time of the given flight data
function getEndInformation(data) {
    let totalSeconds = 0;
    let longitude;
    let latitude;
    data.segments.forEach(function (segment) {
        latitude = segment.latitude;
        longitude = segment.longitude;
        totalSeconds += segment.timespan_seconds;
    });
    endTime = convertTime(data.initial_location.date_time, totalSeconds);
    return [latitude, longitude, endTime];
}
