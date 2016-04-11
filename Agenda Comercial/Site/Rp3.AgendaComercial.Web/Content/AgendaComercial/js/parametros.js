function cambiarEdit(control)
{
    var val = $(control).attr("valor");

    var originalValue = $("#parametro" + val).val();

    $("#parametro" + val).attr("originalvalue", originalValue);
    $("#parametro" + val).removeAttr("disabled");

    $("#edit" + val).css("display", "none");
    $("#accept" + val).css("display", "block");
    $("#cancel" + val).css("display", "block");
}

function guardarValor(control) {
    var val = $(control).attr("valor");
    var dataval = $("#parametro" + val).val();
    var originalvalue = $("#parametro" + val).attr("originalvalue");

    rp3JSONDataPost("/Parametros/SaveParametro", { IdParametro: val, Valor: dataval }, function (data) {

        if (!data.HasError) {
            $("#parametro" + val).attr("originalvalue", dataval);
            $("#parametro" + val).attr("disabled", "true");

            $("#edit" + val).css("display", "block");
            $("#accept" + val).css("display", "none");
            $("#cancel" + val).css("display", "none");
        }
        else {
            var tipo = $("#parametro" + val).attr("tipo");

            if(tipo != 3)
                $("#parametro" + val).val(originalvalue);
            else 
                $("#parametro" + val).select2('val', originalvalue);
        }

        rp3NotifyAsPopup(data.Messages);
    });
   
}

function cancelarValor(control) {

    var val = $(control).attr("valor");

    $("#mainform").validate().resetForm();

    var originalvalue = $("#parametro" + val).attr("originalvalue");
    var tipo = $("#parametro" + val).attr("tipo");

    if (tipo != 3)
        $("#parametro" + val).val(originalvalue);
    else
        $("#parametro" + val).select2('val', originalvalue);

    $("#parametro" + val).attr("disabled", "true");

    $("#edit" + val).css("display", "block");
    $("#accept" + val).css("display", "none");
    $("#cancel" + val).css("display", "none");

}

function setUbicacion(control) {
    var item = $(control);

    var latitud = parseFloat($(item).attr('latitud'));
    var longitud = parseFloat($(item).attr('longitud'));
    var markerIndex = parseInt($(item).attr('markerIndex'));
    var markerStart = parseInt($(item).attr('markerstart'));
    var markerZIndex = parseInt($(item).attr('markerzindex'));
    var titulo = $(item).attr('titulo');

    openUbicacionDialog(markerIndex, latitud, longitud, titulo);
}

function openUbicacionDialog(markerIndex, latitud, longitud, titulo) {
    setUbicacionDialog(markerIndex, latitud, longitud, titulo, 850, 510);
};

function setUbicacionDialog(markerIndex, latitudCentro, longitudCentro, titulo, width, height) {
    rp3Get("/General/Parametros/SetUbicacion", { markerIndex: markerIndex, latitud: latitudCentro, longitud: longitudCentro }, function (data) {
        $("#setUbicacionDialogContent").html(data);
        rp3ModalShow("setUbicacionDialog");
    });
}

function postSetUbicacion() {

    var latitud = $("#LatitudString").val();
    var longitud = $("#LongitudString").val();

    rp3Post("/General/Parametros/SaveUbicacion", { latitud: latitud, longitud: longitud }, function (data) {

        rp3ModalHide("setUbicacionDialog");

        rp3NotifyAsPopup(data.Messages);

        $("[ubicacion]").attr('latitud', latitud);
        $("[ubicacion]").attr('longitud', longitud);

        rp3Get("/General/Cliente/UbicacionMapMarkerClient", null, function (data) {
            $("#content_mapClient").html(data);
        });

    });
}
