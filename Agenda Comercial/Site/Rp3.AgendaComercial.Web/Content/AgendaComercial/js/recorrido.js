function verDetalleGestion(idRuta, idAgenda) {
    rp3Get("/Ruta/Agenda/GetView", { idRuta: idRuta, idAgenda: idAgenda }, function (data) {
        $("#detalle-gestion-content").html(data);
        rp3ModalShow('detalle-gestion-modal');
    });
}

$(function () {
    $("#IncluirClientes, #IncluirRecorrido, #IncluirGestion").on('ifChecked || ifUnchecked', function () {
        actualizarUbicaciones();
    });

    $(window).resize(function () {
        rp3DataTableAjustsColumns();
    });

    currentFechaInicioByAgente = $("#FechaInicioByAgente").val();
    currentFechaFinByAgente = $("#FechaFinByAgente").val();
    currentIdAgente = $("#IdByAgente").val();

    $("#agentestabla tr td").click(function () {
        $("#agentestabla_wrapper tr td").removeClass("selected-item");
        $(this).addClass("selected-item");

        currentIdAgente = $(this).parent().attr("idagente");
        rp3ShowLoadingPanel('#content-fechas');
        var fechaIniParam = null;
        var fechaFinParam = null;
        if (currentFechaInicioByAgente) {
            fechaIniParam = moment(new Date(getDateFromFormat(currentFechaInicioByAgente, RP3_CONVERT_DATE_FORMAT))).format('YYYY-MM-D');
            fechaFinParam = moment(new Date(getDateFromFormat(currentFechaFinByAgente, RP3_CONVERT_DATE_FORMAT))).format('YYYY-MM-D');
        }
        rp3Get("/Ruta/RecorridoAgente/GetFechasRecorrido", { idAgente: currentIdAgente, fechaInicioByAgente: fechaIniParam, fechaFinByAgente: fechaFinParam }, function (data) {
            $("#content-fechas").html(data);
            rp3HideLoadingPanel('#content-fechas');
        });
    });

    $(document).ready(function () {
        if (currentIdAgente) {
            var rows = document.getElementById("agentestabla").rows;
            if (rows.length == 2) {
                rows[1].cells[1].click();
            }
        }
    });

});


function actualizarUbicaciones() {    

    var fecha = $("#fechastabla_wrapper tr td.selected-item").parent().attr("fecha");
    currentIdAgente = $("#agentestabla_wrapper tr td.selected-item").parent().attr("idagente");
    if (currentIdAgente) {
        rp3ShowLoadingPanel('#content_map');
        rp3Get("/Ruta/RecorridoAgente/GetUbicaciones", {
            idagente: currentIdAgente, fecha: fecha,
            incluirClientes: $("#IncluirClientes").is(":checked"),
            incluirRecorrido: $("#IncluirRecorrido").is(":checked"),
            incluirGestion: $("#IncluirGestion").is(":checked")
        }, function (data) {
            $("#content_map").html(data);
            rp3HideLoadingPanel('#content_map');
        });
    }
    var agente = $("#agentestabla_wrapper tr td.selected-item").text();
    var fechaDesc = $("#fechastabla_wrapper tr td.selected-item").text();

    $(".title-block h5").text(agente + ' el ' + fechaDesc);
    // }
}

var currentIdAgente;
var currentFechaInicioByAgente;
var currentFechaFinByAgente;


function openNotificacion() {
    if (currentIdAgente) {
        rp3Get("/Ruta/UbicacionAgente/Notificacion", { }, function (data) {

            $("#modal-notificacion-content").html(data);

            rp3ModalShow("modal-notificacion");
        });
    }
    else {
        rp3DialogInfoMessage("Debe seleccionar un Agente para enviar la notificación", "Estimado usuario", null);
    }

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


function openTrazabilidad() {
    if (currentIdAgente) {
        var fecha = $("#fechastabla_wrapper tr td.selected-item").parent().attr("fecha");
        window.open(window.location.origin + "/" + window.location.pathname.split('/')[1] + "/Ruta/InformeParada/Index?IdAgente=" + currentIdAgente + "&FechaTicks=" + fecha, "_blank");
    }
    else {
        rp3DialogInfoMessage("Debe seleccionar un Agente para ir a su informe de Trazabilidad", "Estimado usuario", null);
    }
}
