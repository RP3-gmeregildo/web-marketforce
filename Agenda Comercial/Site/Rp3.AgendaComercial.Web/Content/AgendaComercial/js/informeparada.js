var currentFechaInicial = null;
var currentFechaFinal = null;
var inicioTicks = null;
var finTicks = null;

$(function () {
    var cb = function (start, end) {
        $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));

        currentFechaInicial = start.format('YYYY-MM-D');
        currentFechaFinal = end.format('YYYY-MM-D');
        inicioTicks = rp3GetTicks(new Date(currentFechaInicial + " "));
        finTicks = rp3GetTicks(new Date(currentFechaFinal + " "));

        LoadData();
    };

    var fechaInicial = moment();
    var fechaFinal = moment();

    var fechaByAgente = $('#FechaByAgente').val();
    if (fechaByAgente) {
        fechaInicial = moment(new Date(getDateFromFormat(fechaByAgente, RP3_CONVERT_DATE_FORMAT))); 
        fechaFinal = fechaInicial;
    }
    var optionSet2 = {
        locale: 'es',
        startDate: fechaInicial,
        endDate: fechaFinal,
        opens: 'left',
        ranges: {
            'Hoy': [moment(), moment()],
            'Ayer': [moment().subtract('days', 1), moment().subtract('days', 1)],
            'Último Mes': [moment().subtract('days', 30), moment()],
            'Últimos 3 Meses': [moment().subtract('days', 90), moment()],
            'Últimos 6 Meses': [moment().subtract('days', 180), moment()],
            'Últimos 12 Meses': [moment().subtract('days', 360), moment()]
        }
    };

    $('#reportrange span').html(fechaInicial.format('MMMM D, YYYY') + ' - ' + fechaFinal.format('MMMM D, YYYY'));

    currentFechaInicial = fechaInicial.format('YYYY-MM-D');
    currentFechaFinal = fechaInicial.format('YYYY-MM-D');
    inicioTicks = rp3GetTicks(new Date(currentFechaInicial + " "));
    finTicks = rp3GetTicks(new Date(currentFechaFinal + " "));
    $('#reportrange').daterangepicker(optionSet2, cb);

    $('#agente').change(function () {
        LoadData();
    });

    LoadData();

    $('#exportarExcel').click(function (e) {
        exportToExcel($('#dvData').html(), 'InformeTrazabilidad.xls');
        e.preventDefault();
    });

    $('#preview').click(function (e) {
        PreviewReport();
    });

    $('#reportegestion').click(function (e) {
        PreviewReporteGestion();
    });
});

var currentIdAgente;

function LoadData() {   
    currentIdAgente = $("#agente").val();
    rp3ShowLoadingPanel('#agente-content-container');

    rp3Get("/Ruta/InformeParada/GetData", {
        IdAgente: currentIdAgente,
        FechaInicioTicks: inicioTicks,
        FechaFinTicks: finTicks
    }, function (data) {
        $('#agente-content-container').html(data);
        rp3HideLoadingPanel('#agente-content-container');

        $("#agente-detalle-table .selectable-item").click(function () {

            var ubicacion = $(this).parent().attr("ubicacion");
            var agente = $(this).parent().attr("agente");
            var fecha = $(this).parent().attr("fecha");
            var horaentrada = $(this).parent().attr("horaentrada");
            var horasalida = $(this).parent().attr("horasalida");
            var idagenda = $(this).parent().attr("idagenda");
            var idruta = $(this).parent().attr("idruta");
            var tiempo = $(this).parent().attr("tiempo");
            var kms = $(this).parent().attr("kms");

            if (idagenda != null) {

                $("#model-detalle-titulo").text('Duración: ' + tiempo + ' recorrido: ' + kms + ' Kms.');

                rp3ModalShow("modal-detalle-map");

                $("#modal-detalle-map-content").rp3LoadingPanel();
                rp3Get("/Ruta/Agenda/ExpandMap", { idRuta: idruta, idAgenda: idagenda }, function (data) {
                    $("#modal-detalle-map-content").rp3LoadingPanel('close');
                    $("#modal-detalle-map-content").html(data);
                });
            }
            else {
                $("#model-detalle-titulo").text(agente + ' el ' + fecha + ' -   inicio: ' + horaentrada + ' fin: ' + horasalida + ' duración: ' + tiempo + ' recorrido: ' + kms + ' Kms.');

                rp3ModalShow("modal-detalle-map");

                $("#modal-detalle-map-content").rp3LoadingPanel();
                rp3Get("/Ruta/InformeParada/GetMap", { Ubicacion: ubicacion, IdAgente: currentIdAgente }, function (data) {
                    $("#modal-detalle-map-content").rp3LoadingPanel('close');
                    $("#modal-detalle-map-content").html(data);
                });
            }
        });

    });
}

function PreviewReport() {
    window.open(RP3_ROOT_PATH + "/Ruta/InformeParada/Preview?IdAgente=" + currentIdAgente + "&FechaInicioTicks=" + inicioTicks + "&FechaFinTicks=" + finTicks, "_blank");
}

function PreviewReporteGestion() {
    if (currentIdAgente) {
        window.open(RP3_ROOT_PATH + "/Ruta/InformeParada/PreviewReporteGestion?IdAgente=" + currentIdAgente + "&FechaInicioTicks=" + inicioTicks + "&FechaFinTicks=" + finTicks + "&Modo=0&MostrarFotos=false", "_blank");
    }
    else {
        rp3DialogInfoMessage("Debe seleccionar un Agente para ir a su reporte de gestión", "Estimado usuario", null);
    }
}

function graficar(detenido, enmovimiento) {
    rp3ShowLoadingPanel('#char-content-container');
    rp3Get("/Ruta/InformeParada/GetChart", { Detenido: detenido, EnMovimiento: enmovimiento }, function (data) {
        $('#char-content-container').html(data);
        rp3HideLoadingPanel('#char-content-container');
    });
}

function openNotificacion() {
    if (currentIdAgente) {
        rp3Get("/Ruta/UbicacionAgente/Notificacion", {}, function (data) {

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

function openRecorrido() {
    if (currentIdAgente) {
        window.open(RP3_ROOT_PATH +  "/Ruta/RecorridoAgente/Index?IdAgente=" + currentIdAgente + "&FechaInicioTicks=" + inicioTicks + '&FechaFinTicks=' + finTicks, "_blank");
    }
    else {
        rp3DialogInfoMessage("Debe seleccionar un Agente para ir a su recorrido", "Estimado usuario", null);
    }
}