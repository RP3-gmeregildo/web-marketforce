/*****************************************************************************************************************/
/* GLOBAL VARIABLES **********************************************************************************************/
/*****************************************************************************************************************/
//Calendario
var currentIdRutaAgenda = 0;
var agendaInicioFormatDate = RP3_DATE_FORMAT;// + " hh:ii";
var currentFechaInicial = null;
var currentFechaFinal = null;
var currentMode = null;
var currentGroupMode = null;
var todosMisAgentes = false;

//Listado Busqueda
var sp_busqueda = "";
var sp_pagina = 1;
var sp_num_registros = 10;
var sp_num_paginas = 1;

var sortField = "fecha";
var sortMode = 1;

//Other
var current_agenda_detalle_idRuta = 0;
var current_agenda_detalle_idRuta = 0;

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

    resizeCalendario();
});

$(window).resize(function () {
    resizeCalendario();
});

function resizeCalendario() {
    $("#calendarcolumn").css("width", $(window).width() - $("#navigatecolumn").width() - 80);
}

/*****************************************************************************************************************/
/* MAIN VIEW INITIATION ***********************************************************************************************/
/*****************************************************************************************************************/
function initMain(datoRuta) {

    currentIdRutaAgenda = datoRuta;

    if (datoRuta)
        todosMisAgentes = false;
    else
        todosMisAgentes = true;

    $("[action='show-list']").click(function () {
        ModoAgendaLista();
    });

    function ModoAgendaLista() {
        limpiarBotonesCalendario();
        $("[action='show-list']").addClass("btn-primary");
        $("[action='show-calendar']").addClass("btn-default");
        $("[action='show-photobook']").addClass("btn-default");
        showAgendaLista();
    }

    $("[action='show-calendar']").click(function () {
        limpiarBotonesCalendario();
        $(this).addClass("btn-primary");
        $("[action='show-list']").addClass("btn-default");
        $("[action='show-photobook']").addClass("btn-default");
        showAgendaCalendario();
        
    });

    $("[action='show-photobook']").click(function () {
        limpiarBotonesCalendario();
        $(this).addClass("btn-primary");
        $("[action='show-list']").addClass("btn-default");
        $("[action='show-calendar']").addClass("btn-default");
        showPhotoBookLista();

    });

    $("#modal-agenda-detalle [actions-modal]").hide();
    $("#modal-agenda-footer-create").show();

    ModoAgendaLista();
}

function init_Calendario() {
    var bodyD = document.body,
    htmlD = document.documentElement;

    var heightD = Math.max(bodyD.scrollHeight, bodyD.offsetHeight,
                           htmlD.clientHeight, htmlD.scrollHeight, htmlD.offsetHeight);

    //$('.md-trigger').rp3Modal();

    $(".ui-datepicker").datepicker({
        onSelect: function (dateText) {

            var date = $(".ui-datepicker").datepicker("getDate");
            $('#agenda-calendar').fullCalendar('gotoDate', date);

            //$('#agenda-calendar').fullCalendar('gotoDate', new Date(this.value));
        }
    });

    $('#external-events div.external-event').each(function () {
        // create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
        // it doesn't need to have a start or end
        var eventObject = {
            title: $.trim($(this).text()) // use the element's text as the event title
        };
        // store the Event Object in the DOM element so we can get to it later
        $(this).data('eventObject', eventObject);
        // make the event draggable using jQuery UI
        $(this).draggable({
            zIndex: 999,
            revert: true,      // will cause the event to go back to its
            revertDuration: 0  //  original position after the drag
        });
    });

    var date = new Date();
    var d = date.getDate();
    var m = date.getMonth();
    var y = date.getFullYear();



    $('#agenda-calendar').fullCalendar({
        header: {
            left: 'title',
            center: '',
            right: 'month,agendaWeek,agendaDay,today, prev,next'
        },
        defaultView: 'agendaWeek',
        editable: true,
        eventDurationEditable: false,
        //eventColor: '#378006',
        axisFormat: 'H:mm',
        timeFormat: 'H:mm',
        firstHour: 8,
        slotMinutes: 15,
        businessHours: true,
        contentHeight: heightD - 260,
        events: RP3_ROOT_PATH + "/Ruta/Agenda/GetAllData/",
        eventDrop: function (event, dayDelta, minuteDelta, allDay, revertFunc) {
            var idevento = event.id;

            var today = new Date();
            var ayer = new Date();
            today.setDate(today.getDate());
            ayer.setDate(ayer.getDate() - 1);

            var a = $.fullCalendar.formatDate(event.start, "yyyy-MM-dd HH:mm:ss");
            var b = $.fullCalendar.formatDate(event.end, "yyyy-MM-dd HH:mm:ss");
            var c = new Date($.fullCalendar.formatDate(event.start, "yyyy-MM-dd"));

            var fechaoriginal = new Date();
            fechaoriginal.setDate(c.getDate() + 1 - dayDelta);

            var fechacambio = new Date();
            fechacambio.setDate(c.getDate() + 1);



            if (fechaoriginal < ayer) {
                rp3NotifyWarningAsPopup("El evento ya ocurrió", "Operación No Permitida");
                revertFunc();
            }
            else if (fechacambio < ayer) {
                rp3NotifyWarningAsPopup("El evento ya ocurrió", "Operación No Permitida");
                revertFunc();
            }
            else if (c <= ayer) {
                rp3NotifyWarningAsPopup("El evento ya ocurrió", "Operación No Permitida");
                revertFunc();
            }
            else {
                updateAppointment(idevento, a, b, 1);
            }
        },
        eventResize: function (event, dayDelta, delta, revertFunc) {
            var idevento = event.id;
            var a = $.fullCalendar.formatDate(event.start, "yyyy-MM-dd HH:mm:ss");
            var b = $.fullCalendar.formatDate(event.end, "yyyy-MM-dd HH:mm:ss");
            updateAppointment(idevento, a, b, 2);
        },
        eventClick: function (calEvent, jsEvent, view) {
            var a = $.fullCalendar.formatDate(calEvent.start, "yyyy-MM-dd HH:mm:ss");
            var b = $.fullCalendar.formatDate(calEvent.end, "yyyy-MM-dd HH:mm:ss");
            openAgendaDetalleView(calEvent.ruta, calEvent.id);
        },
        droppable: true,
        drop: function (date, allDay) {
            var originalEventObject = $(this).data('eventObject');
            var copiedEventObject = $.extend({}, originalEventObject);
            copiedEventObject.start = date;
            copiedEventObject.allDay = allDay;
            $('#calendar').fullCalendar('renderEvent', copiedEventObject, true);
            if ($('#drop-remove').is(':checked')) {
                $(this).remove();
            }
        }
    });
}

function init_Listado() {
    var cb = function (start, end) {
        $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
        $("#txtbusqueda").val('');
        showAgendaListado(start.format('YYYY-MM-D hh:mm:ss'), end.format('YYYY-MM-D hh:mm:ss'));
    }

    var endDate;

    if (!todosMisAgentes)
        endDate = moment().subtract('days', -60);
    else
        endDate = moment();

    var optionSet2 = {
        locale: 'es',
        startDate: moment(),
        endDate: endDate,
        opens: 'left',
        ranges: {
            'Hoy': [moment(), moment()],
            'Ayer': [moment().subtract('days', 1), moment().subtract('days', 1)],
            'Próx. 7 Días': [moment(), moment().subtract('days', -6)],
            'Próx. 15 Días': [moment(), moment().subtract('days', -14)],
            'Próx. 30 Días': [moment(), moment().subtract('days', -29)],
            'Últimos 15 Días': [moment().subtract('days', 14), moment()],
            'Último Mes': [moment().subtract('days', 29), moment()]
        }
    };

    initGroup();

    if (!todosMisAgentes) {
        showAgendaListadoInicial(moment().format('YYYY-MM-D hh:mm:ss'), moment().subtract('days', -60).format('YYYY-MM-D hh:mm:ss'));
        $('#reportrange span').html(moment().format('MMMM D, YYYY') + ' - ' + moment().subtract('days', -60).format('MMMM D, YYYY'));
    } else {
        showAgendaListadoInicial(moment().format('YYYY-MM-D hh:mm:ss'), moment().format('YYYY-MM-D hh:mm:ss'));
        $('#reportrange span').html(moment().format('MMMM D, YYYY') + ' - ' + moment().format('MMMM D, YYYY'));
    }

    $('#reportrange').daterangepicker(optionSet2, cb);

    $("#txtbusqueda").keyup(function () {
        var word = $(this).val();
        $("#spinner-loading").hide().fadeIn(50);

        if (aplicarBusquedaAgenda) clearTimeout(aplicarBusquedaAgenda);

        if (word.length > 2) {
            busquedaAgendaTrigger(word);
            $("#reportrange").addClass("disableDIV");
        }
        else if (word.length > 0) {
            $("#reportrange").addClass("disableDIV");
        }
        else {

            if (!todosMisAgentes) {
                $('#reportrange span').html(moment().format('MMMM D, YYYY') + ' - ' + moment().subtract('days', -60).format('MMMM D, YYYY'));
                showAgendaListadoInicial(moment().format('YYYY-MM-D hh:mm:ss'), moment().subtract('days', -60).format('YYYY-MM-D hh:mm:ss'));
            } else {
                $('#reportrange span').html(moment().format('MMMM D, YYYY') + ' - ' + moment().format('MMMM D, YYYY'));
                showAgendaListadoInicial(moment().format('YYYY-MM-D hh:mm:ss'), moment().format('YYYY-MM-D hh:mm:ss'));
            }

            $("#reportrange").removeClass("disableDIV");
        }
        $("#spinner-loading").fadeOut(300);
    });
}

function init_PhotoBook() {
    var cb = function (start, end) {
        $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
        $("#txtbusqueda").val('');
        showPhotoBookListado(start.format('YYYY-MM-D hh:mm:ss'), end.format('YYYY-MM-D hh:mm:ss'));
    }

    var startDate;

    if (!todosMisAgentes)
        startDate = moment().subtract('days', 60);
    else
        startDate = moment();

    var optionSet2 = {
        locale: 'es',
        startDate: startDate,
        endDate: moment(),
        opens: 'left',
        ranges: {
            'Hoy': [moment(), moment()],
            'Ayer': [moment().subtract('days', 1), moment().subtract('days', 1)],
            'Últimos 7 Días': [moment().subtract('days', 6), moment()],
            'Últimos 15 Días': [moment().subtract('days', 14), moment()],
            'Último Mes': [moment().subtract('days', 29), moment()]
        }
    };

    initGroup();

    if (!todosMisAgentes) {
        showPhotoBookListadoInicial(moment().subtract('days', 60).format('YYYY-MM-D hh:mm:ss'), moment().format('YYYY-MM-D hh:mm:ss'));
        $('#reportrange span').html(moment().subtract('days', 60).format('MMMM D, YYYY') + ' - ' + moment().format('MMMM D, YYYY'));
    } else {
        showPhotoBookListadoInicial(moment().format('YYYY-MM-D hh:mm:ss'), moment().format('YYYY-MM-D hh:mm:ss'));
        $('#reportrange span').html(moment().format('MMMM D, YYYY') + ' - ' + moment().format('MMMM D, YYYY'));
    }

    $('#reportrange').daterangepicker(optionSet2, cb);

    $("#txtbusqueda").keyup(function () {
        var word = $(this).val();
        $("#spinner-loading").hide().fadeIn(50);

        if (aplicarBusquedaAgenda) clearTimeout(aplicarBusquedaAgenda);

        if (word.length > 2) {
            busquedaPhotoBookTrigger(word);
            $("#reportrange").addClass("disableDIV");
        }
        else if (word.length > 0) {
            $("#reportrange").addClass("disableDIV");
        }
        else {

            if (!todosMisAgentes) {
                $('#reportrange span').html(moment().subtract('days', 60).format('MMMM D, YYYY') + ' - ' + moment().format('MMMM D, YYYY'));
                showPhotoBookListadoInicial(moment().subtract('days', 60).format('YYYY-MM-D hh:mm:ss'), moment().format('YYYY-MM-D hh:mm:ss'));
            } else {
                $('#reportrange span').html(moment().format('MMMM D, YYYY') + ' - ' + moment().format('MMMM D, YYYY'));
                showPhotoBookListadoInicial(moment().format('YYYY-MM-D hh:mm:ss'), moment().format('YYYY-MM-D hh:mm:ss'));
            }

            $("#reportrange").removeClass("disableDIV");
        }
        $("#spinner-loading").fadeOut(300);
    });
}

function initGroup() {

    if (!todosMisAgentes) {
        currentGroupMode = 'D';
    }
    else {
        agentGroupMode();
    }

    $("[action='group-calendar']").click(function () {
        limpiarBotonesGroup();
        $(this).addClass("btn-primary");
        $("[action='group-client']").addClass("btn-default");
        $("[action='group-agent']").addClass("btn-default");
        currentGroupMode = 'D';

        if (currentMode == 'L') {
            showAgendaListadoInicial(currentFechaInicial, currentFechaFinal);
        }
        else if (currentMode == 'P') {
            showPhotoBookListadoInicial(currentFechaInicial, currentFechaFinal);
        }
    });

    $("[action='group-client']").click(function () {
        limpiarBotonesGroup();
        $(this).addClass("btn-primary");
        $("[action='group-calendar']").addClass("btn-default");
        $("[action='group-agent']").addClass("btn-default");
        currentGroupMode = 'C';

        if (currentMode == 'L') {
            showAgendaListadoInicial(currentFechaInicial, currentFechaFinal);
        }
        else if (currentMode == 'P') {
            showPhotoBookListadoInicial(currentFechaInicial, currentFechaFinal);
        }
    });

    $("[action='group-agent']").click(function () {
        agentGroupMode();
    });

    function agentGroupMode()
    {
        limpiarBotonesGroup();
        $("[action='group-agent']").addClass("btn-primary");
        $("[action='group-calendar']").addClass("btn-default");
        $("[action='group-client']").addClass("btn-default");

        currentGroupMode = 'A';

        if (currentMode == 'L') {
            showAgendaListadoInicial(currentFechaInicial, currentFechaFinal);
        }
        else if (currentMode == 'P') {
            showPhotoBookListadoInicial(currentFechaInicial, currentFechaFinal);
        }
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
/*****************************************************************************************************************/
/* MAIN FUNCTIONS ************************************************************************************************/
/*****************************************************************************************************************/

//MODAL :: Load Clients in TEXTBOX
var clienteDireccionEditAgendaData = [];
function loadDireccionEditAgenda(idcliente, idclientecontacto) {
    var idprincipal;
    rp3Get("/Ruta/Agenda/GetClienteDirecciones", { idCliente: idcliente, idContacto: idclientecontacto }, function (data) {
        while (clienteDireccionEditAgendaData.length > 0) {
            clienteDireccionEditAgendaData.pop();
        }
        $.each(data, function (i, val) {
            clienteDireccionEditAgendaData.push(val);

            if (val.principal) {
                idprincipal = val.id;
            }
        });

        if (idprincipal) {
            $("#agendacreateform input[select-direccion]").select2('val', idprincipal);
        }
    });
}

//LISTADO :: Global Search
var aplicarBusquedaAgenda;

function busquedaAgendaTrigger(texto) {
    if (aplicarBusquedaAgenda) clearTimeout(aplicarBusquedaAgenda);
    aplicarBusquedaAgenda = setTimeout(function () {
        sp_pagina = 1;
        BusquedaAgendaListado(texto);
    }, 500);
}

function busquedaPhotoBookTrigger(texto) {
    if (aplicarBusquedaAgenda) clearTimeout(aplicarBusquedaAgenda);
    aplicarBusquedaAgenda = setTimeout(function () {
        sp_pagina = 1;
        BusquedaPhotoBookListado(texto);
    }, 500);
}

//CREATE
function createAgendaPost() {
    var idCliente = $("#agendacreateform [idCliente]").val();
    var idContacto = $("#agendacreateform [idContacto]").val();
    var idClienteDireccion = $("#agendacreateform input[select-direccion]").select2('val');

    //var inicio = getTicks(new Date(Date.parseExact(String($("#agendacreateform [startdate1] input").val()).replace(". ", "").replace(".", ""), "dd/MM/yyyy HH:mm")));
    var inicio = getTicks(new Date(Date.parseExact($("#agendacreateform [startdate1] input").val() + " " + $("#agendacreateform [starthour1] input").val(), "dd/MM/yyyy HH:mm")));
    //var fin = getTicks(new Date(Date.parseExact(String($("#agendacreateform [startdate2] input").val()).replace(". ", "").replace(".", ""), "dd/MM/yyyy HH:mm")));
    var duracion = $('#agendacreateform select[duracion]').select2('val');

    var tareas = $("#agendacreateform [tareas]").select2('val').toString();;

    rp3Post("/Ruta/Agenda/Crear", {
        IdRuta: currentIdRutaAgenda,
        IdCliente: idCliente,
        IdContacto: idContacto,
        IdClienteDireccion: idClienteDireccion,
        FechaInicioTicks: inicio,
        //FechaFinTicks: fin,
        DuracionVisita: duracion,
        TareasSeleccion: tareas
    }, function (data) {
        if (!data.HasError) {
            rp3ModalHide("getModalDialogCrear");
            updateAppointments();
        }
        rp3NotifyAsPopup(data.Messages);
    });
}


//CALENDAR
function showAgendaCalendario() {
    currentMode = 'C';

    rp3ShowLoadingPanel('#content-agenda');
    rp3Get("/Ruta/Agenda/GetCalendario", null, function (data) {
        $(".ui-datepicker").datepicker("destroy");
        $("#ui-datepicker-div").remove();
        $('#content-agenda').html(data);
        resizeCalendario();
        rp3HideLoadingPanel('#content-agenda');
    });
}

function updateAppointment(ids, fechaInicio, fechaFin, op) {
    rp3Post('/Agenda/EditData', {
        id: ids,
        fechaInicio: fechaInicio,
        fechaFin: fechaFin,
        opt: String(op)
    }, function (data) {
        $('#agenda-calendar').fullCalendar('refetchEvents');
    });
}

//LISTADO
function showAgendaLista() {
    currentMode = 'L';
    rp3ShowLoadingPanel('#content-agenda');
    rp3Get("/Ruta/Agenda/GetLista", null, function (data) {
        $('#content-agenda').html(data);
        rp3HideLoadingPanel('#content-agenda');

        rp3ShowLoadingPanel('#content-listado');

        //$("[action='sort-list']").click(function () {

        //    rp3ModalShow("modal-agenda-sort");

        //    $("#modal-agenda-sort-content").rp3LoadingPanel();

        //    rp3Get("/Ruta/Agenda/GetSort", null, function (data) {

        //        $("#modal-agenda-sort-content").rp3LoadingPanel('close');
        //        $("#modal-agenda-sort-content").html('');
        //        $("#modal-agenda-sort-content").html(data);

        //        if (sortField != "") {
        //            var tr = $('[field="' + sortField + '"]');
        //            var button;

        //            if (sortMode == 1) {
        //                button = $(tr).find("[action='order-asc']");

        //            } else {
        //                button = $(tr).find("[action='order-desc']");
        //            }

        //            if (button) {
        //                $(button).removeClass("btn-default");
        //                $(button).addClass("btn-primary");
        //            }
        //        }
        //        else {
        //            button = $('[field="fecha"]').find("[action='order-asc']");

        //            if (button) {
        //                $(button).removeClass("btn-default");
        //                $(button).addClass("btn-primary");
        //            }
        //        }

        //        $("#modal-agenda-sort-content [action='order-asc']").click(function () {
        //            sortField = $(this).parent().parent().attr('field');
        //            sortMode = 1;

        //            rp3ModalHide("modal-agenda-sort");

        //            showAgendaListado(currentFechaInicial, currentFechaFinal);
        //        });

        //        $("#modal-agenda-sort-content [action='order-desc']").click(function () {
        //            sortField = $(this).parent().parent().attr('field');
        //            sortMode = 2;

        //            rp3ModalHide("modal-agenda-sort");

        //            showAgendaListado(currentFechaInicial, currentFechaFinal);
        //        });
        //    });
        //});
    });
}

function Sort(field)
{
    if (field == sortField)
    {
        if (sortMode == 1)
            sortMode = 2;
        else
            sortMode = 1;
    }
    else 
        sortMode = 1

    sortField = field;

    if (currentMode == 'L') {
        showAgendaListado(currentFechaInicial, currentFechaFinal);
    }
    else {
        showPhotoBookListado(currentFechaInicial, currentFechaFinal);
    }
}

function evaluateSortMode()
{
    $("[sort-asc]").hide();
    $("[sort-desc]").hide();

    var parent = $("[field='" + sortField + "']");

    if (sortMode == 1)
        parent.find("[sort-asc]").show();
    else 
        parent.find("[sort-desc]").show();
}

function BusquedaAgendaListado(busqueda) {
    currentMode = 'L';

    if(!todosMisAgentes)
        currentGroupMode = 'D';
    else
        currentGroupMode = 'A';

    limpiarBotonesGroup();
    $("[action='group-calendar']").addClass("btn-primary");
    $("[action='group-client']").addClass("btn-default");
    $("[action='group-agent']").addClass("btn-default");

    sp_busqueda = busqueda;
    rp3ShowLoadingPanel('#content-agenda');
    rp3Get("/Ruta/Agenda/GetListadoSearch", { busqueda: busqueda, pagina: sp_pagina, numreg: sp_num_registros }, function (data) {
        $('#content-listado').html(data);
        rp3HideLoadingPanel('#content-agenda');

        evaluateSortMode();
    });
}

function showAgendaListadoInicial(fechaInicial, fechaFinal) {
    currentMode = 'L';
    currentFechaInicial = fechaInicial;
    currentFechaFinal = fechaFinal;

    if (fechaInicial && fechaFinal) {

        $('#content-agenda').addClass("panel-loading");
        rp3ShowLoadingPanel('#content-agenda');
        rp3Get("/Ruta/Agenda/GetListadoInicial", { fechaInicial: fechaInicial, fechaFinal: fechaFinal, sortField: sortField, sortMode: sortMode, groupMode: currentGroupMode }, function (data) {
            $('#content-listado').html(data);
            rp3HideLoadingPanel('#content-agenda');
            $('#content-agenda').removeClass("panel-loading");

            evaluateSortMode();
        });
    }
}

function showAgendaListado(fechaInicial, fechaFinal) {

    currentMode = 'L';
    currentFechaInicial = fechaInicial;
    currentFechaFinal = fechaFinal;
    $('#content-agenda').addClass("panel-loading");
    rp3ShowLoadingPanel('#content-agenda');
    rp3Get("/Ruta/Agenda/GetListado", { fechaInicial: fechaInicial, fechaFinal: fechaFinal, sortField: sortField, sortMode: sortMode, groupMode: currentGroupMode }, function (data) {
        $('#content-listado').html(data);
        rp3HideLoadingPanel('#content-agenda');
        $('#content-agenda').removeClass("panel-loading");

        evaluateSortMode();
    });
}

//PHOTOBOOK
function showPhotoBookLista() {
    currentMode = 'P';
    rp3ShowLoadingPanel('#content-agenda');
    rp3Get("/Ruta/Agenda/GetPhotoBook", null, function (data) {
        $('#content-agenda').html(data);
        rp3HideLoadingPanel('#content-agenda');
        
        //$('#content-photobook').css("height", "300px");
        rp3ShowLoadingPanel('#content-photobook');
    });
}

function BusquedaPhotoBookListado(busqueda) {
    currentMode = 'P';

    if (!todosMisAgentes)
        currentGroupMode = 'D';
    else
        currentGroupMode = 'A';

    limpiarBotonesGroup();
    $("[action='group-calendar']").addClass("btn-primary");
    $("[action='group-client']").addClass("btn-default");
    $("[action='group-agent']").addClass("btn-default");

    sp_busqueda = busqueda;
    rp3ShowLoadingPanel('#content-agenda');
    rp3Get("/Ruta/Agenda/GetPhotoBookSearch", { busqueda: busqueda, pagina: sp_pagina, numreg: 50 }, function (data) {
        $('#content-photobook').html(data);
        rp3HideLoadingPanel('#content-agenda');
    });
}

function showPhotoBookListadoInicial(fechaInicial, fechaFinal) {
    currentMode = 'P';
    currentFechaInicial = fechaInicial;
    currentFechaFinal = fechaFinal;
    rp3ShowLoadingPanel('#content-agenda');
    rp3Get("/Ruta/Agenda/GetPhotoBookInicial", { fechaInicial: fechaInicial, fechaFinal: fechaFinal, sortField: sortField, sortMode: sortMode, groupMode: currentGroupMode }, function (data) {
        $('#content-photobook').html(data);
        rp3HideLoadingPanel('#content-agenda');

        evaluateSortMode();
    });
}

function showPhotoBookListado(fechaInicial, fechaFinal) {
    currentMode = 'P';
    currentFechaInicial = fechaInicial;
    currentFechaFinal = fechaFinal;
    rp3ShowLoadingPanel('#content-agenda');
    rp3Get("/Ruta/Agenda/GetPhotoBookListado", { fechaInicial: fechaInicial, fechaFinal: fechaFinal, sortField: sortField, sortMode: sortMode, groupMode: currentGroupMode }, function (data) {
        $('#content-photobook').html(data);
        rp3HideLoadingPanel('#content-agenda');

        evaluateSortMode();
    });
}

function elegirAgente() {
    window.location.href = RP3_ROOT_PATH + "/Ruta/Agenda/CambiarAgente";
};

//MODAL
/*OPEN DIALOG :: CREATE*/
function crearAgendaShow() {

    if (currentIdRutaAgenda) {
        rp3Get("/Ruta/Agenda/Crear", { idRuta: currentIdRutaAgenda }, function (data) {
            $("#getModalDialogCrearContent").html('');
            $("#getModalDialogCrearContent").html(data);
            rp3ModalShow("getModalDialogCrear");
        });
    }
    else {
        rp3NotifyWarningAsPopup("El agente actual no tiene asignada una ruta");
    }
};





/*****************************************************************************************************************/
/* OTHER FUNCTIONS ***********************************************************************************************/
/*****************************************************************************************************************/
//Refresh Botones
function limpiarBotonesCalendario() {
    $("[action='show-calendar']").removeClass("btn-primary");
    $("[action='show-calendar']").removeClass("btn-default");
    $("[action='show-list']").removeClass("btn-primary");
    $("[action='show-list']").removeClass("btn-default");
    $("[action='show-photobook']").removeClass("btn-primary");
    $("[action='show-photobook']").removeClass("btn-default");
}

function limpiarBotonesGroup() {
    $("[action='group-calendar']").removeClass("btn-primary");
    $("[action='group-calendar']").removeClass("btn-default");
    $("[action='group-client']").removeClass("btn-primary");
    $("[action='group-client']").removeClass("btn-default");
    $("[action='group-agent']").removeClass("btn-primary");
    $("[action='group-agent']").removeClass("btn-default");
}

function updateAppointments() {
    if (currentMode == 'L') {
        showAgendaListadoInicial(currentFechaInicial, currentFechaFinal);
    } else if (currentMode == 'P') {
        showPhotoBookListadoInicial(currentFechaInicial, currentFechaFinal);
    } else {
        $('#agenda-calendar').fullCalendar('refetchEvents');
    }
}

function ListadoNextPage(sp_total_paginas) {
    if ((sp_pagina + 1) <= sp_total_paginas) {
        sp_pagina++;
        BusquedaAgendaListado(sp_busqueda);
    }

    evaluateListadoNavigationButtons(sp_total_paginas);
}

function ListadoNextEndPage(sp_total_paginas) {
    if ((sp_pagina + 1) <= sp_total_paginas) {
        sp_pagina = sp_total_paginas;
        BusquedaAgendaListado(sp_busqueda);
    }
    else { }

    evaluateListadoNavigationButtons(sp_total_paginas);
}

function ListadoPreviousPage(sp_total_paginas) {
    if ((sp_pagina - 1) >= 1) {
        sp_pagina--;
        BusquedaAgendaListado(sp_busqueda);
    }

    evaluateListadoNavigationButtons(sp_total_paginas);
}

function ListadoPreviousBeginPage(sp_total_paginas) {
    if ((sp_pagina - 1) >= 1) {
        sp_pagina = 1;
        BusquedaAgendaListado(sp_busqueda);
    }
    else { }

    evaluateListadoNavigationButtons(sp_total_paginas);
}

function evaluateListadoNavigationButtons(total_paginas) {
    $('button[action="listado_prev_begin"]').removeAttr('disabled');
    $('button[action="listado_prev"]').removeAttr('disabled');
    $('button[action="listado_next"]').removeAttr('disabled');
    $('button[action="listado_next_end"]').removeAttr('disabled');

    if (sp_pagina - 1 < 1) {
        $('button[action="listado_prev"]').attr('disabled', 'disabled');
        $('button[action="listado_prev_begin"]').attr('disabled', 'disabled');
    }

    if (sp_pagina + 1 > total_paginas) {
        $('button[action="listado_next"]').attr('disabled', 'disabled');
        $('button[action="listado_next_end"]').attr('disabled', 'disabled');
    }
}

function PhotoBookNextPage(sp_total_paginas) {
    if ((sp_pagina + 1) <= sp_total_paginas) {
        sp_pagina++;
        BusquedaPhotoBookListado(sp_busqueda);
    }

    evaluatePhotoNavigationButtons(sp_total_paginas);
}

function PhotoBookNextEndPage(sp_total_paginas) {
    if ((sp_pagina + 1) <= sp_total_paginas) {
        sp_pagina = sp_total_paginas;
        BusquedaPhotoBookListado(sp_busqueda);
    }
    else { }

    evaluatePhotoNavigationButtons(sp_total_paginas);
}

function PhotoBookPreviousPage(sp_total_paginas) {
    if ((sp_pagina - 1) >= 1) {
        sp_pagina--;
        BusquedaPhotoBookListado(sp_busqueda);
    }

    evaluatePhotoNavigationButtons(sp_total_paginas);
}

function PhotoBookPreviousBeginPage(sp_total_paginas) {
    if ((sp_pagina - 1) >= 1) {
        sp_pagina = 1;
        BusquedaPhotoBookListado(sp_busqueda);
    }
    else { }

    evaluatePhotoNavigationButtons(sp_total_paginas);
}

function evaluatePhotoNavigationButtons(total_paginas) {
    $('button[action="photo_prev_begin"]').removeAttr('disabled');
    $('button[action="photo_prev"]').removeAttr('disabled');
    $('button[action="photo_next"]').removeAttr('disabled');
    $('button[action="photo_next_end"]').removeAttr('disabled');

    if (sp_pagina - 1 < 1) {
        $('button[action="photo_prev"]').attr('disabled', 'disabled');
        $('button[action="photo_prev_begin"]').attr('disabled', 'disabled');
    }

    if (sp_pagina + 1 > total_paginas) {
        $('button[action="photo_next"]').attr('disabled', 'disabled');
        $('button[action="photo_next_end"]').attr('disabled', 'disabled');
    }
}

function GetSundays(option) {
    rp3Get("/Ruta/Agenda/GetRemainderSundays", null, function (data) {
        $('#content-listado').html(data);
    });
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