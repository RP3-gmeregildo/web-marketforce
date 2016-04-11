var googleMapClient = null;
var boundsClient = null;
var latitudCentro = null;
var longitudCentro = null;
var count = 0;

$(document).ready(function () {
    initMapaClient();
});

function initMapaClient() {
    boundsClient = new google.maps.LatLngBounds();
    var options = {
        zoom: 16,
        scrollwheel: false,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    googleMapClient = new google.maps.Map($("#viewMapClient")[0], options);

    var infowindowClient = new google.maps.InfoWindow();

    $('[ubicacion]').each(function (i, val) {

        // var item = $(this).parents('[ubicacion]');

        var item = $(this);

        var latitud = parseFloat($(item).attr('latitud'));
        var longitud = parseFloat($(item).attr('longitud'));
        var markerIndex = parseInt($(item).attr('markerIndex'));
        var markerStart = parseInt($(item).attr('markerstart'));
        var markerZIndex = parseInt($(item).attr('markerzindex'));
        var titulo = $(item).attr('titulo');

        if (latitud != -1 && longitud != -1) {

            var point = new google.maps.LatLng(latitud, longitud);

            boundsClient.extend(point);

            var size = new google.maps.Size(30, 42, "px", "px");
            var origin = new google.maps.Point(markerStart, 0);
            var image = new google.maps.MarkerImage(RP3_ROOT_PATH + "/Content/AgendaComercial/img/style/sprite_numbers_location.png", size, origin);

            var marker = new google.maps.Marker({
                position: point,
                map: googleMapClient,
                title: titulo,
                icon: (markerIndex > 0 ? image : null),
                zIndex: markerZIndex,
                draggable: false
            });

            google.maps.event.addListener(marker, "click", function () {
                infowindowClient.close();
                infowindowClient.setContent('<div><strong>' + titulo + '</strong><br>');                
                infowindowClient.open(googleMapClient, this);                
            });

            latitudCentro = latitud;
            longitudCentro = longitud;

            count++;
        }

    });

    setCenterClient();

    $('a[data-toggle="tab"][targettype="content"]').click(function () {
        setTimeout(function () {
            checkResizeMapClient();
        }, 200);
    });
}

function setCenterClient() {
    if (count == 1) {
        googleMapClient.setCenter(new google.maps.LatLng(latitudCentro, longitudCentro));
    }
    else {
        googleMapClient.fitBounds(boundsClient);
    }
}

function checkResizeMapClient() {
    if (googleMapClient) {
        google.maps.event.trigger(googleMapClient, 'resize');
        setCenterClient();
    }
}