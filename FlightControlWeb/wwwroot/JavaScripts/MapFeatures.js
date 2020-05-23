let currentMarked = null;
function displayMap() {
    let myKey = 'nw9X5t1VORpRClqJavkK~XN8n9COo6PKhDoEQJKEZQA~' +
        'AtnOCAe8bf0NUKZkOULqwpGPW-7-diFVfJClHL_VUDdmlzB-SYSECuKD1K98KyLS';
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
    map.on("click", function (e) {
        let isMapLayer = true;
        let found = false;
        map.forEachFeatureAtPixel(e.pixel, function (feature, layer) {
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

function tryMarkAirplane(name, source) {
    if ((currentMarked != null) && (currentMarked.get('name') == name)) {
        return;
    }
    unmarkAirplane(true);
    getFlightPlanAndMark(name, source);
}

function getFlightPlanAndMark(flightId, source) {
    let flightPlanUrl = "../api/FlightPlan/" + flightId;
    $.get(flightPlanUrl, function (data) {
        if (!isFlightPlanValid(data)) {
            printError("Received invalid flight plan");
            return;
        }
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

function changeAirplaneLocation(flightId, latitude, longitude) {
    map.getLayers().forEach(function (layer) {
        if (layer.get('name') == flightId) {
            layer.getSource().getFeatures()[0].setGeometry(
                new ol.geom.Point(ol.proj.fromLonLat([longitude, latitude])));
            return;
        }
    })
}

function deleteAirplane(flightId, update) {
    let layerToDelete = getLayer(flightId);
    if (layerToDelete == currentMarked) {
        emptyCurrentMarked(update);
    }
    map.removeLayer(layerToDelete);
}

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

function markAirplane(data, name, source) {
    deleteAirplane(name, true);
    let newAirplane = addAirplane(name, source, 'Images/markedAirplane.png', 0.1);
    let endInfo = getEndInformation(data);
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
    let myFlightsRow = document.getElementById(name);
    myFlightsRow.style.backgroundColor = "#ffd800";
    myFlightsRow.style.color = "#272727";
    drawRouteLines(data);
    currentMarked = newAirplane;
}

function unmarkAirplane(update) {
    if (currentMarked == null) {
        return;
    }
    let name = currentMarked.get('name');
    let previous = currentMarked;
    deleteAirplane(name, update);
    addAirplane(name, previous.getSource(), defaultAirplaneImage, defaultAirplaneScale);
}

function emptyCurrentMarked(update) {
    if (!update) {
        $("#FlightDetailsBody").empty();
    }
    let id = currentMarked.get('name');
    let myFlightsRow = document.getElementById(id);
    myFlightsRow.style.backgroundColor = "";
    myFlightsRow.style.color = "";
    removeRouteLines();
    currentMarked = null;
}

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
                opacity: 0.1,
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

function removeRouteLines() {
    let layerToDelete = getLayer("lines");
    map.removeLayer(layerToDelete);
}

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
