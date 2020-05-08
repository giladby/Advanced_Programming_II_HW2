function DisplayMap() {
    var myKey = 'nw9X5t1VORpRClqJavkK~XN8n9COo6PKhDoEQJKEZQA~AtnOCAe8bf0NUKZkOULqwpGPW-7-diFVfJClHL_VUDdmlzB-SYSECuKD1K98KyLS';
    map = new ol.Map({
        layers: [
            new ol.layer.Tile({
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
}

function AddAirplane(flightId, latitude, longitude) {
    var airplane = new ol.layer.Vector({
        name: flightId,
        source: new ol.source.Vector({
            features: [new ol.Feature({
                geometry: new ol.geom.Point(ol.proj.fromLonLat([longitude, latitude])),
            })]
        }),
        style: new ol.style.Style({
            image: new ol.style.Icon(({
                src: 'Images/airplane.png',
                scale: 0.05
            }))
        })
    });
    map.addLayer(airplane);
}

function ChangeAirplaneLocation(flightId, latitude, longitude) {
    map.getLayers().forEach(function (layer) {
        if (layer.get('name') === flightId) {
            layer.getSource().getFeatures()[0].setGeometry(
                new ol.geom.Point(ol.proj.fromLonLat([longitude, latitude])));
            return;
        }
    })
}

function DeleteAirplane(flightId) {
    map.getLayers().forEach(function (layer) {
        if (layer.get('name') === flightId) {
            map.removeLayer(layer);
        }
    })
}