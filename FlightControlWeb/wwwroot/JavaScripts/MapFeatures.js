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
        let operate = false;

        map.forEachFeatureAtPixel(e.pixel, function (feature, layer) {

            operate = true;

            if (!layer.get('external')) {
                
                if (currentMarked != null && currentMarked.get('name') == layer.get('name')) {
                    
                    return;
                }
                UnmarkAirplane();
                getFlightPlanFunc(layer.get('name'), layer);
            } else {

            }
        })
        if (!operate) {
            UnmarkAirplane();
        }
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

function DeleteAirplane(flightId) {
    map.getLayers().forEach(function (layer) {
        if (layer.get('name') == flightId) {
            map.removeLayer(layer);
        }
    })
}

function MarkAirplane(data, airplane) {
    let source = airplane.getSource();
    let name = airplane.get('name');
    let external = airplane.get('external');
    DeleteAirplane(name);
    let newAirplane = AddAirplane(name, source, external, 'Images/markedAirplane.png', 0.1);

    let endTriple = getEndTriple(data);

    let time = convertTime(data.initial_location.date_time, 0);

    let tr = "<tr><td>[" + data.initial_location.latitude + "," + data.initial_location.longitude + "]</td>" +
            "<td>[" + endTriple[0] + "," + endTriple[1] + "]</td>" +
        "<td>" + data.initial_location.date_time + "</td>" +
            "<td>" + endTriple[2] + "</td>" +
                "<td>" + data.company_name + "</td>" +
            "<td>" + data.passengers + "</td></tr>";
    $("#FlightDetailsBody").append(tr);

    currentMarked = newAirplane;
}

function UnmarkAirplane() {
    if (currentMarked == null) {
        return;
    }
    $("#FlightDetailsBody").empty();

    let name = currentMarked.get('name');
    DeleteAirplane(name);
    AddAirplane(name, currentMarked.getSource(), currentMarked.get('external'), defaultAirplaneImage, defaultAirplaneScale);

    currentMarked = null;
}

function convertTime(dt, plus) {
    dt = dt.substring(0, dt.length - 1);
    dt = new Date(dt);
    dt = new Date(dt.getTime() + 1000 * plus);
    dt = new Date(dt.getTime()).toISOString();
    dt = dt.substr(0, (dt.length - 5)) + "Z";
    return dt;
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

    dt = convertTime(data.initial_location.date_time, totalSeconds);
    
    return [latitude, longitude, dt];
}

function getFlightPlanFunc(flightId, airplane) {
    var flightPlanUrl = "../api/FlightPlan/" + flightId;
    $.get(flightPlanUrl, function (data) {
        MarkAirplane(data, airplane);
    });
}