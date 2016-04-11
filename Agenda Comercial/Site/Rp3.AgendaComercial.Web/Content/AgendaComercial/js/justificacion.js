var currentFechaInicial = null;
var currentFechaFinal = null;

var inicioFormatDate = RP3_DATE_FORMAT;// + " hh:ii";
var currentMode = 'P';
var current_idPermiso;

var sortField = "";
var sortMode = 1;

//Fechas
var dateToday = new Date();
var dateYesterday = new Date();
dateYesterday.setDate(dateYesterday.getDate() - 1);
var diffMinutes = 30;

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

    //$('#estado').select2('val', 'P');

    $('#estado').change(function () {
        showPermisoListado(currentFechaInicial, currentFechaFinal);
    });

    $('#agente').change(function () {
        showPermisoListado(currentFechaInicial, currentFechaFinal);
    });

    $("#modal-permiso-detalle-button-cancelar").click(function () {
        cancelarPermisoPost();
    });

    $("#modal-permiso-detalle-button-aprobar").click(function () {
        aprobarPermisoPost();
    });

    $("#modal-permiso-detalle-button-rechazar").click(function () {
        rechazarPermisoPost();
    });

    $("[action='show-pending']").click(function () {
        limpiarBotones();
        $(this).addClass("btn-primary");
        $("[action='show-history']").addClass("btn-default");

        currentMode = 'P'

        enabledFilters(false);

        showPermisoListado(currentFechaInicial, currentFechaFinal);
    });

    $("[action='show-history']").click(function () {
        limpiarBotones();
        $(this).addClass("btn-primary");
        $("[action='show-pending']").addClass("btn-default");

        currentMode = 'F'

        enabledFilters(true);

        showPermisoListado(currentFechaInicial, currentFechaFinal);
    });

    initMain();

    enabledFilters(false);
});

function enabledFilters(enabled) {
    if (enabled) {
        $("#reportrange").removeClass("disableDIV");
        $('#agente').removeAttr("disabled");
        $('#estado').removeAttr("disabled");
    }
    else {
        $("#reportrange").addClass("disableDIV");
        $('#agente').attr('disabled', 'disabled');
        $('#estado').attr('disabled', 'disabled');
    }
}

function limpiarBotones() {
    $("[action='show-pending']").removeClass("btn-primary");
    $("[action='show-pending']").removeClass("btn-default");
    $("[action='show-history']").removeClass("btn-primary");
    $("[action='show-history']").removeClass("btn-default");
}

/*****************************************************************************************************************/
/* MAIN VIEW INITIATION ***********************************************************************************************/
/*****************************************************************************************************************/
function initMain() {
    showPermisoLista();
}

function init_Listado() {
    var cb = function (start, end) {
        $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
        $("#txtbusqueda").val('');
        showPermisoListado(start.format('YYYY-MM-D hh:mm:ss'), end.format('YYYY-MM-D hh:mm:ss'));
    }

    var optionSet2 = {
        locale: 'es',
        startDate: moment().subtract('days', 29),
        endDate: moment(),
        opens: 'left',
        ranges: {
            //'Próx. Mes': [moment(), moment().subtract('days', -29)],
            'Último Mes': [moment().subtract('days', 29), moment()],
            'Último Trimestre': [moment().subtract('days', 89), moment()],
            'Último Semestre': [moment().subtract('days', 179), moment()],
            'Último Año': [moment().subtract('days', 364), moment()],
        }
    };

    //initGroup();

    showPermisoListado(moment().subtract('days', 29).format('YYYY-MM-D hh:mm:ss'), moment().format('YYYY-MM-D hh:mm:ss'));
    $('#reportrange span').html(moment().subtract('days', 29).format('MMMM D, YYYY') + ' - ' + moment().format('MMMM D, YYYY'));

    $('#reportrange').daterangepicker(optionSet2, cb);
}

//LISTADO
function showPermisoLista() {
    //currentMode = 'L';
    rp3ShowLoadingPanel('#content-permiso');
    rp3Get("/Marcacion/Justificacion/GetLista", null, function (data) {
        $('#content-permiso').html(data);
        rp3HideLoadingPanel('#content-permiso');

        rp3ShowLoadingPanel('#content-listado');

        $("[action='sort-list']").click(function () {

            rp3ModalShow("modal-permiso-sort");

            $("#modal-permiso-sort-content").rp3LoadingPanel();

            rp3Get("/Marcacion/Justificacion/GetSort", null, function (data) {

                $("#modal-permiso-sort-content").rp3LoadingPanel('close');
                $("#modal-permiso-sort-content").html('');
                $("#modal-permiso-sort-content").html(data);

                if (sortField != "") {
                    var tr = $('[field="' + sortField + '"]');
                    var button;

                    if (sortMode == 1) {
                        button = $(tr).find("[action='order-asc']");

                    } else {
                        button = $(tr).find("[action='order-desc']");
                    }

                    if (button) {
                        $(button).removeClass("btn-default");
                        $(button).addClass("btn-primary");
                    }
                }
                else {
                    button = $('[field="fechainicio"]').find("[action='order-desc']");

                    if (button) {
                        $(button).removeClass("btn-default");
                        $(button).addClass("btn-primary");
                    }
                }

                $("#modal-permiso-sort-content [action='order-asc']").click(function () {
                    sortField = $(this).parent().parent().attr('field');
                    sortMode = 1;

                    rp3ModalHide("modal-permiso-sort");

                    showPermisoListado(currentFechaInicial, currentFechaFinal);
                });

                $("#modal-permiso-sort-content [action='order-desc']").click(function () {
                    sortField = $(this).parent().parent().attr('field');
                    sortMode = 2;

                    rp3ModalHide("modal-permiso-sort");

                    showPermisoListado(currentFechaInicial, currentFechaFinal);
                });
            });
        });
    });
}

function showPermisoListado(fechaInicial, fechaFinal) {
    //currentMode = 'L';
    currentFechaInicial = fechaInicial;
    currentFechaFinal = fechaFinal;

    var agentegrupo = $("#agente").val();

    var agente;
    var grupo;

    if (agentegrupo) {
        var array = agentegrupo.split('-');

        if (array[0] == "A") {
            agente = array[1];
        } else {
            grupo = array[1];
        }
    }

    var estado = $("#estado").val();

    if (currentMode == 'P') {
        fechaInicial = null;
        fechaFinal = null;
        agente = null;
        grupo = null;
        estado = 'P';
    }

    $('#content-permiso').addClass("panel-loading");
    rp3ShowLoadingPanel('#content-permiso');
    rp3Get("/Marcacion/Justificacion/GetListado", {
        fechaInicial: fechaInicial, fechaFinal: fechaFinal, sortField: sortField, sortMode: sortMode, agente: agente, grupo: grupo, estado: estado
    }, function (data) {
        $('#content-listado').html(data);
        rp3HideLoadingPanel('#content-permiso');
        $('#content-permiso').removeClass("panel-loading");
    });
}

//MODAL
/*OPEN DIALOG :: DETAIL*/
function openPermisoDetalleView(id, estado) {

    current_idPermiso = id;

    rp3Get("/Marcacion/Justificacion/Detail", { id: id }, function (data) {
        $("#modal-permiso-detalle-content").html('');
        $("#modal-permiso-detalle-content").html(data);

        init_Modal_Detail(estado);

        rp3ModalShow("modal-permiso-detalle");
    });

};

function init_Modal_Detail(estado) {
    $("#permisocreateform #searchPermisoTo").attr('disabled', 'disabled');

    $('#permisocreateform select[tipo]').select2();
    $('#permisocreateform select[motivo]').select2();

    $('#permisocreateform select[tipo]').attr('disabled', 'disabled');
    $('#permisocreateform select[motivo]').attr('disabled', 'disabled');

    $('#permisocreateform [startdate1] input').attr('disabled', 'disabled');
    $('#permisocreateform [enddate1] input').attr('disabled', 'disabled');
    $('#permisocreateform [starthour1] input').attr('disabled', 'disabled');
    $('#permisocreateform [endhour1] input').attr('disabled', 'disabled');

    $('#permisocreateform [observacion]').attr('disabled', 'disabled');

    if (estado != 'P') {
        $('#permisocreateform [observacionsupervisor]').attr('disabled', 'disabled');
    }

    $('#modal-permiso-detalle-button-cancelar').hide();
    $('#modal-permiso-detalle-button-aprobar').hide();
    $('#modal-permiso-detalle-button-rechazar').hide();

    if (estado == 'P') {
        $('#modal-permiso-detalle-button-aprobar').show();
        $('#modal-permiso-detalle-button-rechazar').show();
    }

    if (estado == 'A' || estado == 'R') {
        $('#modal-permiso-detalle-button-cancelar').show();
    }
}



//APROBAR
function aprobarPermisoPost() {

    var observacionSupervisor = $("#permisocreateform [observacionsupervisor]").val();

    if (observacionSupervisor) {
        rp3ModalHide("modal-permiso-detalle");

        rp3DialogConfirmationMessage("¿Está seguro que desea Aprobar la Justificación?", "Justificación", function (data) {
            rp3Post("/Marcacion/Justificacion/Aprobar", {
                id: current_idPermiso,
                observacionSupervisor: observacionSupervisor
            }, function (data) {
                if (data.HasError)
                    rp3NotifyAsPopup(data.Messages);
                else
                    showPermisoListado(currentFechaInicial, currentFechaFinal);
            });
        });
    }
    else {
        rp3NotifyWarningAsPopup("Debe ingresar la observación del supervisor", "Justificación")
    }
}

//RECHAZAR
function rechazarPermisoPost() {

    var observacionSupervisor = $("#permisocreateform [observacionsupervisor]").val();

    if (observacionSupervisor) {
        rp3ModalHide("modal-permiso-detalle");

        rp3DialogConfirmationMessage("¿Está seguro que desea Rechazar la Justificación?", "Justificación", function (data) {
            rp3Post("/Marcacion/Justificacion/Rechazar", {
                id: current_idPermiso,
                observacionSupervisor: observacionSupervisor
            }, function (data) {
                if (data.HasError)
                    rp3NotifyAsPopup(data.Messages);
                else
                    showPermisoListado(currentFechaInicial, currentFechaFinal);
            });
        });
    }
    else {
        rp3NotifyWarningAsPopup("Debe ingresar la observación del supervisor", "Justificación")
    }
}

//CANCELAR
function cancelarPermisoPost() {

    var observacionSupervisor = $("#permisocreateform [observacionsupervisor]").val();

    if (observacionSupervisor) {
        rp3ModalHide("modal-permiso-detalle");

        rp3DialogConfirmationMessage("¿Está seguro que desea Reversar la Justificación?", "Justificación", function (data) {
            rp3Post("/Marcacion/Justificacion/Cancelar", {
                id: current_idPermiso,
                observacionSupervisor: observacionSupervisor
            }, function (data) {
                if (data.HasError)
                    rp3NotifyAsPopup(data.Messages);
                else
                    showPermisoListado(currentFechaInicial, currentFechaFinal);
            });
        });
    }
    else {
        rp3NotifyWarningAsPopup("Debe ingresar la observación del supervisor", "Justificación")
    }
}




function removeHeader() {
    $(".datetimepicker .datetimepicker-hours .switch").css('visibility', 'hidden');
    $(".datetimepicker .datetimepicker-hours .prev").css('visibility', 'hidden');
    $(".datetimepicker .datetimepicker-hours .next").css('visibility', 'hidden');
    $(".datetimepicker .datetimepicker-hours .prev").html("");
    $(".datetimepicker .datetimepicker-hours .next").html("");

    $(".datetimepicker .datetimepicker-minutes .switch").css('visibility', 'hidden');
    $(".datetimepicker .datetimepicker-minutes .prev").css('visibility', 'hidden');
    $(".datetimepicker .datetimepicker-minutes .next").css('visibility', 'hidden');
    $(".datetimepicker .datetimepicker-minutes .prev").html("");
    $(".datetimepicker .datetimepicker-minutes .next").html("");
}

//Update Dates
function formatDate(value) {
    return leadingCero(value.getDate()) + "/" + leadingCero(value.getMonth() + 1) + "/" + value.getFullYear();// + " " + leadingCero(value.getHours()) + ":" + leadingCero(value.getMinutes());
}
function formatTime(value) {
    return leadingCero(value.getHours()) + ":" + leadingCero(value.getMinutes());
}
function formatDateBack(value) {
    return leadingCero(value.getMonth() + 1) + "/" + leadingCero(value.getDate()) + "/" + value.getFullYear() + " " + leadingCero(value.getHours()) + ":" + leadingCero(value.getMinutes());
}
function formatDateBegin(value) {
    return value.getDate() + "/" + (value.getMonth()) + "/" + value.getFullYear() + " 00:00";
}
function formatDateEnd(value) {
    return leadingCero(value.getDate()) + "/" + leadingCero(value.getMonth() + 1) + "/" + value.getFullYear() + " 23:59";
}
function formatDateEnd2(value) {
    return leadingCero(value.getMonth() + 1) + "/" + leadingCero(value.getDate()) + "/" + value.getFullYear() + " 23:59";
}
function addMinutes(date, minutes) {
    return new Date(date.getTime() + minutes * 60000);
}
function leadingCero(number) {
    var data = number;

    if (number <= 9)
        data = '0' + number;

    return data;
}