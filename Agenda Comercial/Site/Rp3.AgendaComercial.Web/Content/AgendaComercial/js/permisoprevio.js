var currentFechaInicial = null;
var currentFechaFinal = null;

var inicioFormatDate = RP3_DATE_FORMAT;// + " hh:ii";
var currentMode;
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

    //$('#estado').select2('val', 'A');

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

    initMain();
});


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
        startDate: moment(),
        endDate: moment().subtract('days', -29),
        opens: 'left',
        ranges: {
            'Próx. Mes': [moment(), moment().subtract('days', -29)],
            'Último Mes': [moment().subtract('days', 29), moment()],
            'Último Trimestre': [moment().subtract('days', 89), moment()],
            'Último Semestre': [moment().subtract('days', 179), moment()],
            'Último Año': [moment().subtract('days', 364), moment()],
        }
    };

    //initGroup();

    showPermisoListado(moment().format('YYYY-MM-D hh:mm:ss'), moment().subtract('days', -29).format('YYYY-MM-D hh:mm:ss'));
    $('#reportrange span').html(moment().format('MMMM D, YYYY') + ' - ' + moment().subtract('days', -29).format('MMMM D, YYYY'));

    $('#reportrange').daterangepicker(optionSet2, cb);
}

//LISTADO
function showPermisoLista() {
    currentMode = 'L';
    rp3ShowLoadingPanel('#content-permiso');
    rp3Get("/Marcacion/PermisoPrevio/GetLista", null, function (data) {
        $('#content-permiso').html(data);
        rp3HideLoadingPanel('#content-permiso');

        rp3ShowLoadingPanel('#content-listado');

        $("[action='sort-list']").click(function () {

            rp3ModalShow("modal-permiso-sort");

            $("#modal-permiso-sort-content").rp3LoadingPanel();

            rp3Get("/Marcacion/PermisoPrevio/GetSort", null, function (data) {

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
    currentMode = 'L';
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

    $('#content-permiso').addClass("panel-loading");
    rp3ShowLoadingPanel('#content-permiso');
    rp3Get("/Marcacion/PermisoPrevio/GetListado", {
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

    rp3Get("/Marcacion/PermisoPrevio/Detail", { id: id }, function (data) {
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

    //if (estado != 'P' && estado != 'A') {
    //    $('#permisocreateform [observacionsupervisor]').attr('disabled', 'disabled');
    //}

    //$('#modal-permiso-detalle-button-cancelar').show();

    //if (estado == 'C') {
    //    $('#modal-permiso-detalle-button-cancelar').hide();
    //}

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

//MODAL
/*OPEN DIALOG :: CREATE*/
function crearPermisoShow() {
    rp3Get("/Marcacion/PermisoPrevio/Crear", null, function (data) {
        $("#getModalDialogCrearContent").html('');
        $("#getModalDialogCrearContent").html(data);

        init_Modal_Create();

        rp3ModalShow("getModalDialogCrear");
    });

};

function createPermisoPost() {
    var idAgente = $("#permisocreateform [idAgente]").val();
    var idGrupo = $("#permisocreateform [idGrupo]").val();
    
    var tipo = $('#permisocreateform select[tipo]').select2('val');
    var motivo = $('#permisocreateform select[motivo]').select2('val');

    var observacion = $("#permisocreateform [observacion]").val();
    var observacionSupervisor = $("#permisocreateform [observacionsupervisor]").val();

    var inicio = getTicks(new Date(Date.parseExact($("#permisocreateform [startdate1] input").val() + " " + $("#permisocreateform [starthour1] input").val(), "dd/MM/yyyy HH:mm")));
    var fin = getTicks(new Date(Date.parseExact($("#permisocreateform [enddate1] input").val() + " " + $("#permisocreateform [endhour1] input").val(), "dd/MM/yyyy HH:mm")));
    
    if (observacionSupervisor) {
        rp3Post("/Marcacion/PermisoPrevio/Crear", {
            IdAgente: idAgente,
            IdGrupo: idGrupo,
            FechaInicioTicks: inicio,
            FechaFinTicks: fin,
            Tipo: tipo,
            Motivo: motivo,
            Observacion: observacion,
            ObservacionSupervisor: observacionSupervisor
        }, function (data) {
            if (!data.HasError) {
                rp3ModalHide("getModalDialogCrear");
                //updateAppointments();

                showPermisoListado(currentFechaInicial, currentFechaFinal);
            }
            rp3NotifyAsPopup(data.Messages);
        });
    }
    else {
        rp3NotifyWarningAsPopup("Debe ingresar la observación del supervisor", "Permiso")
    }
}

function init_Modal_Create() {

    $('#searchPermisoButton').on('click', function () {

        rp3ModalShow("modal-permiso-agente-search");
        $("#modal-permiso-agente-search-content").rp3LoadingPanel();

        rp3Get("/Marcacion/PermisoPrevio/GetSeleccionAgente", null, function (data) {

            $("#modal-permiso-agente-search-content").rp3LoadingPanel('close');
            $("#modal-permiso-agente-search-content").html('');
            $("#modal-permiso-agente-search-content").html(data);

            $("#modal-permiso-agente-search-content #searchagentetable tr td").click(function () {
                var idAgente = parseInt($(this).attr('idAgente'));
                var idGrupo = parseInt($(this).attr('idGrupo'));
                var descripcion = $(this).attr('descripcion')

                $("#permisocreateform [idAgente]").val(0);
                $("#permisocreateform [idGrupo]").val(0);

                $("#permisocreateform [idAgente]").val(idAgente);
                $("#permisocreateform [idGrupo]").val(idGrupo);

                $("#permisocreateform #searchPermisoTo").val(descripcion);

                rp3ModalHide("modal-permiso-agente-search");

            });

            $("#modal-permiso-agente-search-content #searchagentetable").rp3DataTable()
        });
    });

    $('#permisocreateform [startdate1]').datetimepicker({
        maxView: 2,
        minView: 2,
        startView: 2,
        datepicker: true,
        timePicker: false,
        language: 'es',
        format: inicioFormatDate,
        startDate: formatDate(dateYesterday),
        initialDate: formatDate(dateToday),
        autoclose: true
    });

    $('#permisocreateform [startdate1] input').val(formatDate(dateToday));

    $('#permisocreateform [enddate1]').datetimepicker({
        maxView: 2,
        minView: 2,
        startView: 2,
        datepicker: true,
        timePicker: false,
        language: 'es',
        format: inicioFormatDate,
        startDate: formatDate(dateYesterday),
        initialDate: formatDate(dateToday),
        autoclose: true
    });

    $('#permisocreateform [enddate1] input').val(formatDate(dateToday));

    $('#permisocreateform [starthour1]').datetimepicker({
        datepicker: false,
        maxView: 1,
        minView: 0,
        startView: 1,
        language: 'es',
        format: 'hh:ii',
        minuteStep: 15,
        autoclose: true
    }).click(function () {
        removeHeader();
    });

    $('#permisocreateform [starthour1] input').val(formatTime(dateToday));

    $('#permisocreateform [endhour1]').datetimepicker({
        datepicker: false,
        maxView: 1,
        minView: 0,
        startView: 1,
        language: 'es',
        format: 'hh:ii',
        minuteStep: 15,
        autoclose: true
    }).click(function () {
        removeHeader();
    });

    $('#permisocreateform [endhour1] input').val(formatTime(dateToday));


    $('#permisocreateform select[tipo]').select2();
    $('#permisocreateform select[motivo]').select2();


    $("#permisocreateform #searchPermisoTo").autocomplete({
        source: function (request, response) {
            $.getJSON(RP3_ROOT_PATH + "/Marcacion/PermisoPrevio/AgenteAutocomplete", { term: request.term }, response);
        },
        minLength: 3,
        select: function (event, ui) {

            $("#permisocreateform [idAgente]").val(0);
            $("#permisocreateform [idGrupo]").val(0);

            $("#permisocreateform [idAgente]").val(ui.item.idAgente);
            $("#permisocreateform [idGrupo]").val(ui.item.idGrupo);

            $(event.target).val(ui.item.value);
            event.preventDefault();
        },
        open: function () {
            $("#permisocreateform .ui-autocomplete").width($("#permisocreateform #searchAgendaTo").width() - 10);
        }
    });

    //$("#agendacreateform #searchAgendaTo").autocomplete("widget").insertAfter($("#agendaeditform"));

}

//APROBAR
function aprobarPermisoPost() {

    var observacionSupervisor = $("#permisocreateform [observacionsupervisor]").val();

    if (observacionSupervisor) {

        rp3ModalHide("modal-permiso-detalle");

        rp3DialogConfirmationMessage("¿Está seguro que desea Aprobar el Permiso?", "Permiso", function (data) {
            rp3Post("/Marcacion/PermisoPrevio/Aprobar", {
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
        rp3NotifyWarningAsPopup("Debe ingresar la observación del supervisor", "Permiso")
    }
}

//RECHAZAR
function rechazarPermisoPost() {

    var observacionSupervisor = $("#permisocreateform [observacionsupervisor]").val();

    if (observacionSupervisor) {
        rp3ModalHide("modal-permiso-detalle");

        rp3DialogConfirmationMessage("¿Está seguro que desea Rechazar el Permiso?", "Permiso", function (data) {
            rp3Post("/Marcacion/PermisoPrevio/Rechazar", {
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
        rp3NotifyWarningAsPopup("Debe ingresar la observación del supervisor", "Permiso")
    }
}


//CANCELAR
function cancelarPermisoPost() {

    var observacionSupervisor = $("#permisocreateform [observacionsupervisor]").val();

    if (observacionSupervisor) {
        rp3ModalHide("modal-permiso-detalle");

        rp3DialogConfirmationMessage("¿Está seguro que desea Reversar el Permiso?", "Permiso", function (data) {
            rp3Post("/Marcacion/PermisoPrevio/Cancelar", {
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
        rp3NotifyWarningAsPopup("Debe ingresar la observación del supervisor", "Permiso")
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