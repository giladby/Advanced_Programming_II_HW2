let currentMarked = null;

function DisplayMap() {
    var myKey = 'nw9X5t1VORpRClqJavkK~XN8n9COo6PKhDoEQJKEZQA~AtnOCAe8bf0NUKZkOULqwpGPW-7-diFVfJClHL_VUDdmlzB-SYSECuKD1K98KyLS';
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
            zoom: 2
        })
    });

    map.on("click", function (e) {
        let isMapLayer = true;
        map.forEachFeatureAtPixel(e.pixel, function (feature, layer) {
            if (layer.get('name') == 'lines') {
                return;
            }
            isMapLayer = false;
            if (!layer.get('external')) {
                tryMarkAirplane(layer.get('name'), layer.getSource(), layer.get('external'));
            } else {

            }
        })
        if (isMapLayer) {
            UnmarkAirplane(false);
        }
    });
}


function tryMarkAirplane(name, source, external) {
    
    if ((currentMarked != null) && (currentMarked.get('name') == name)) {
        return;
    }
    UnmarkAirplane(true);
    getFlightPlanAndMark(name, source, external);
}

function createSource(latitude, longitude) {
    return new ol.source.Vector({
        features: [new ol.Feature({
            geometry: new ol.geom.Point(ol.proj.fromLonLat([longitude, latitude])),
        })]
    });
}

function AddAirplane(flightId, source, isExternal, image, scale) {
    var airplane = new ol.layer.Vector({
        name: flightId,
        external: isExternal,
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

function ChangeAirplaneLocation(flightId, latitude, longitude) {
    map.getLayers().forEach(function (layer) {
        if (layer.get('name') == flightId) {
            layer.getSource().getFeatures()[0].setGeometry(
                new ol.geom.Point(ol.proj.fromLonLat([longitude, latitude])));
            return;
        }
    })
}


function DeleteAirplane(flightId, update) {
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
        }
    });
    return myLayer;
}

function MarkAirplane(data, name, source, external) {

    DeleteAirplane(name, true);
    
    let newAirplane = AddAirplane(name, source, external, 'Images/markedAirplane.png', 0.1);
    let endTriple = getEndTriple(data);
    let tr = "<tr id=\"flightDetailsRow\"><td>[" + data.initial_location.latitude + "," + data.initial_location.longitude + "]</td>" +
             "<td>[" + endTriple[0] + "," + endTriple[1] + "]</td>" +
             "<td>" + data.initial_location.date_time + "</td>" +
             "<td>" + endTriple[2] + "</td>" +
             "<td>" + data.company_name + "</td>" +
             "<td>" + data.passengers + "</td></tr>";
    let flightDetailsRow = document.getElementById("flightDetailsRow");
    if (flightDetailsRow) {
        $("#flightDetailsRow").replaceWith(tr);
    } else {
        $("#FlightDetailsBody").append(tr);
    }
    
    let myFlightsRow = document.getElementById(name);
    myFlightsRow.style.backgroundColor = "yellow";
    drawRouteLines(data);
    currentMarked = newAirplane;
}

function UnmarkAirplane(update) {
    if (currentMarked == null) {
        return;
    }
    let name = currentMarked.get('name');
    let previous = currentMarked;
    DeleteAirplane(name, update);
    AddAirplane(name, previous.getSource(), previous.get('external'), defaultAirplaneImage, defaultAirplaneScale);
}

function emptyCurrentMarked(update) {
    if (!update) {
        $("#FlightDetailsBody").empty();
    }
    let id = currentMarked.get('name');
    let myFlightsRow = document.getElementById(id);
    myFlightsRow.style.backgroundColor = "";
    removeRouteLines();
    currentMarked = null;
}

function drawRouteLines(data) {
    let pointsArr = [];
    pointsArr.push(ol.proj.fromLonLat([data.initial_location.longitude, data.initial_location.latitude]));
    data.segments.forEach(function (segment) {
        pointsArr.push(ol.proj.fromLonLat([segment.longitude, segment.latitude]));
    });

    var lineStyle = [
        new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'blue',
                lineDash: [4, 8],
                lineCap: 'square',
                width: 5
            })
        })
    ];
    var layer = new ol.layer.Vector({
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

function convertTime(time, secondsToAdd) {
    time = time.substring(0, time.length - 1);
    time = new Date(time);
    time = new Date(time.getTime() + 1000 * secondsToAdd);
    time = new Date(time.getTime()).toISOString();
    time = time.substr(0, (time.length - 5)) + "Z";
    return time;
}

function getEndTriple(data) {
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

function getFlightPlanAndMark(flightId, source, external) {
    var flightPlanUrl = "../api/FlightPlan/" + flightId;
    $.get(flightPlanUrl, function (data) {
        MarkAirplane(data, flightId, source, external);
    });
}