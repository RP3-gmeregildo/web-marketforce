$(function () {

    $.datepicker.regional['es'] = {
        closeText: 'Cerrar',
        prevText: 'Ant',
        nextText: 'Sig',
        currentText: 'Hoy',
        monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
        monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
        dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
        dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
        dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };

    $.datepicker.setDefaults($.datepicker.regional['es']);

    $('#FechaEdit').change(function () {
        LoadData($('#FechaTicks').val());
    });

    LoadData($('#FechaTicks').val());

    $('#exportarExcel').click(function (e) {
        exportToExcel($('#dvData').html(), 'Marcaciones.xls');
        e.preventDefault();
    });
});

var currentIdAgente;
var currentFechaInicioByAgente;
var currentFechaFinByAgente;

function LoadData(fechaTicks) {
    rp3ShowLoadingPanel('#agente-content-container');

    rp3Get("/Marcacion/Marcacion/GetData", {
        FechaTicks: fechaTicks,
    }, function (data) {
        $('#agente-content-container').html(data);
        rp3HideLoadingPanel('#agente-content-container');

        $("#agente-detalle-table .selectable-item").click(function () {

            var idagente = $(this).parent().attr("idagente");
            var descripcion = $(this).parent().attr("descripcion");
            var direccionmarcacion = $(this).parent().attr("direccionmarcacion");

            var fechaTicks = $('#FechaTicks').val();
            var fechatext = $('#FechaEdit').val();

            currentIdAgente = idagente;
            currentFechaInicioByAgente = fechaTicks;

            $("#model-detalle-titulo").text(descripcion + ' - ' + fechatext);
            $("#model-detalle-direccion-marcacion").text(direccionmarcacion);

            rp3ModalShow("modal-detalle-map");

            $("#modal-detalle-map-content").rp3LoadingPanel();
            rp3Get("/Marcacion/Marcacion/GetMap", { Idagente: idagente, FechaTicks: fechaTicks }, function (data) {
                $("#modal-detalle-map-content").rp3LoadingPanel('close');
                $("#modal-detalle-map-content").html(data);
            });

            $("#modal-detalle-permiso-content").rp3LoadingPanel();
            rp3Get("/Marcacion/Marcacion/GetPermisos", { Idagente: idagente, FechaTicks: fechaTicks }, function (data) {
                $("#modal-detalle-permiso-content").rp3LoadingPanel('close');
                $("#modal-detalle-permiso-content").html(data);
            });

        });
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


function openTrazabilidad() {
    if (currentIdAgente) {
        var fecha = $("#fechastabla_wrapper tr td.selected-item").parent().attr("fecha");
        window.open(window.location.origin + "/" + window.location.pathname.split('/')[1] + "/Ruta/InformeParada/Index?IdAgente=" + currentIdAgente + "&FechaTicks=" + currentFechaInicioByAgente, "_blank");
    }
    else {
        rp3DialogInfoMessage("Debe seleccionar un Agente para ir a su informe de Trazabilidad", "Estimado usuario", null);
    }
}