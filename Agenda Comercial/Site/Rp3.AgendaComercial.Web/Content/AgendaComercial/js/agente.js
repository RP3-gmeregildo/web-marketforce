function showCambioRutaMessage() {
    rp3NotifyWarningAsBlock("Al cambiar la Ruta del Agente actual ser eliminarán todas las Citas programadas.", "Advertencia", true);
}

function showDependenciaMessage() {
    rp3NotifyWarningAsBlock("Se eliminarán todas Citas programadas del Agente actual y la Ruta actual podrá ser asignada a otro agente.", "Advertencia", true);
}

$(function () {

    $("#Estado").change(function () {
        var estado = $(this).val();
        var id = parseInt($("#IdAgente").val());

        if (estado == "I" && id > 0) {
            showDependenciaMessage();
        }
    });

    $("#IdRuta").change(function () {
        var estado = $(this).val();
        var id = parseInt($("#IdAgente").val());

        if (id > 0) {
            showCambioRutaMessage();
        }
    });

    $('#IdRuta').change(function (e) {
        ubicacionMapMarker($(this).prop('value'));
    });
});

function ubicacionMapMarker(idRuta) {
    rp3Get("/General/Agente/UbicacionMapMarker", { idRuta: idRuta }, function (data) {
        $("#content_map").html(data);
    });
};