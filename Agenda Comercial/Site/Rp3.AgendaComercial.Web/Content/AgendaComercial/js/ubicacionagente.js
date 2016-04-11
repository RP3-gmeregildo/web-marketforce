$(function () {
    $("input[name='agentecheckall']").on('ifChecked || ifUnchecked', function () {
         
        var checkall = this.checked;

        $("input[name='agentes']").each(function () {            
            if (checkall) {
                $(this).iCheck('check');
            }
            else {
                $(this).iCheck('uncheck');
            }
        });
    });
});

function filter(filter) {
    var filtervalue = $("#Filter" + filter).val();

    if (filtervalue == "true") {
        $("#Filter" + filter).val("false");
        $("#text" + filter).removeClass("filterText");
    }
    else {
        $("#Filter" + filter).val("true");
        $("#text" + filter).addClass("filterText");
    }
}

var currentIdAgente;

function openNotificacion(idAgente) {

    currentIdAgente = idAgente;

    rp3Get("/Ruta/UbicacionAgente/Notificacion", { }, function (data) {

        $("#modal-notificacion-content").html(data);

        rp3ModalShow("modal-notificacion");
    });
};

function notificacionPost() {
    var titulo = $("#sendnotificacionform [titulo]").val();
    var mensaje = $("#sendnotificacionform [mensaje]").val();

    rp3Post("/Ruta/UbicacionAgente/Notificacion", {
        idAgente: currentIdAgente,
        titulo: titulo,
        mensaje: mensaje
    }, function (data) {

        if (!data.HasError) {
            rp3ModalHide("modal-notificacion");
        }
        rp3NotifyAsPopup(data.Messages);
    });
}

function openRecorrido(idAgente, fecha) {
    var fechaTicks = rp3GetTicks(new Date(fecha + " "));
    window.open(window.location.origin + "/" + window.location.pathname.split('/')[1] + "/Ruta/RecorridoAgente/Index?IdAgente=" + idAgente + "&FechaInicioTicks=" + fechaTicks + '&FechaFinTicks=' + fechaTicks, "_blank");
}

function openTrazabilidad(idAgente, fecha) {
    var fechaTicks = rp3GetTicks(new Date(fecha + " "));
    window.open(window.location.origin + "/" + window.location.pathname.split('/')[1] + "/Ruta/InformeParada/Index?IdAgente=" + idAgente + "&FechaTicks=" + fechaTicks, "_blank");
}
