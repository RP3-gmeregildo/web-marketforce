var AGENDA_PAGE_DETAIL_MAIN = 1;
var AGENDA_PAGE_DETAIL_TAREAS = 2;
var AGENDA_PAGE_DETAIL_EDITAR = 3;

var current_agenda_detalle_idAgenda = 1;
var current_agenda_detalle_idRuta = 1;


$(function () {

    $("#modal-agenda-detalle-button-back").click(function () {
        setAgendaDetallePage(AGENDA_PAGE_DETAIL_MAIN);
    });

    $("#modal-agenda-detalle-button-capturar").click(function () {

        showPreview(null);
        //html2canvas(document.getElementById("viewMap"), {
        //    proxy: "https://html2canvas.appspot.com/query",
        //    //logging: true,
        //    useCORS: false,
        //    onrendered: function(canvas) {
        //        var dataUrl = canvas.toDataURL("image/png");
        //        showPreview(dataUrl);
        //    },
        //    onerror: function (data) {
        //        alert(data);
        //    }
        //});

        //html2canvas($("#modal-agenda-detalle-content"), {
        //    useCORS: true,
        //    onrendered: function (canvas) {
        //        //theCanvas = canvas;
        //        canvas.toBlob(function (blob) {
        //            saveAs(blob, "agenda.png");

                    
        //        });
        //    }
        //});
    });

    function showPreview(imgData)
    {
        $("#modal-agenda-detalle-content").rp3LoadingPanel();
        window.open(RP3_ROOT_PATH + "/Ruta/Agenda/Preview?idRuta=" + current_agenda_detalle_idRuta + "&idAgenda=" + current_agenda_detalle_idAgenda + "&addressBase=" + RP3_ROOT_PATH, "_blank");
        $("#modal-agenda-detalle-content").rp3LoadingPanel('close');
        //window.open(imgData, "_blank");
        //$("#modal-agenda-detalle-content").rp3LoadingPanel();
        //$.ajax({
        //    url: RP3_ROOT_PATH + "/Ruta/Agenda/Preview",
        //    data: { idRuta: current_agenda_detalle_idRuta, idAgenda: current_agenda_detalle_idAgenda, addressBase: RP3_ROOT_PATH },
        //    type: 'POST',
        //    success: function (data) {
        //        $("#modal-agenda-detalle-content").rp3LoadingPanel('close');
        //        window.open("data:application/pdf;base64, " + escape(data), "_blank");
        //        //var newWindow = window.open("", "_blank");
        //        //newWindow.document.write(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //        $("#modal-agenda-detalle-content").rp3LoadingPanel('close');
        //    }
        //});

        //rp3Get("/Ruta/Agenda/Preview", { idRuta: current_agenda_detalle_idRuta, idAgenda: current_agenda_detalle_idAgenda, mapData: data }, function (data, status, xhr) {
        //    alert(status);
        //    $("#modal-agenda-detalle-content").rp3LoadingPanel('close');
        //});
    }

    function showPedido()
    {
        window.open(RP3_ROOT_PATH + "/Pedido/Pedido/Detail/" + ID_PEDIDO, "_blank");
    }

    $("#modal-agenda-detalle-button-eliminar").click(function () {
        eliminarAgendaPost();
    });
    
    $("#modal-agenda-detalle-button-editar").click(function () {
        openAgendaDetalleEdit();
    });

    $("#modal-agenda-detalle-button-editar-post").click(function () {
        updateAgendaDetallePost();
    });

    $("#modal-agenda-detalle-button-pedido").click(function () {
        showPedido();
    });
    
});

function setAgendaDetallePage(opt) {
    if (opt == AGENDA_PAGE_DETAIL_MAIN) {
        $("#modal-agenda-content-general").show();
        $("#modal-agenda-content-tareas").hide();
        $("#modal-agenda-content-editar").hide();

        $("#modal-agenda-footer-main").show();
        $("#modal-agenda-footer-back").hide();
        $("#modal-agenda-footer-update").hide();
    }
    if (opt == AGENDA_PAGE_DETAIL_TAREAS) {
        $("#modal-agenda-content-general").hide();
        $("#modal-agenda-content-tareas").show();
        $("#modal-agenda-content-editar").hide();

        $("#modal-agenda-footer-main").hide();
        $("#modal-agenda-footer-back").show();
        $("#modal-agenda-footer-update").hide();
    }
    if (opt == AGENDA_PAGE_DETAIL_EDITAR) {
        $("#modal-agenda-content-general").hide();
        $("#modal-agenda-content-tareas").hide();
        $("#modal-agenda-content-editar").show();

        $("#modal-agenda-footer-main").hide();
        $("#modal-agenda-footer-back").hide();
        $("#modal-agenda-footer-update").show();
    }
}

/*OPEN DIALOG :: VIEW*/
function openAgendaDetalleView(idRuta, idAgenda, estado) {
    current_agenda_detalle_idRuta = idRuta;
    current_agenda_detalle_idAgenda = idAgenda;

    $("#modal-agenda-content-general").empty();
    
    rp3ModalShow("modal-agenda-detalle");

    $("#modal-agenda-detalle-content").rp3LoadingPanel();

    $("#modal-agenda-footer-main").hide();
    $("#modal-agenda-footer-back").hide();
    $("#modal-agenda-footer-update").hide();
    $("#modal-agenda-detalle-button-eliminar").hide();
    $("#modal-agenda-detalle-button-editar").hide();


    rp3Get("/Ruta/Agenda/GetDatos", { idRuta: idRuta, idAgenda: idAgenda }, function (data) {
        $("#modal-agenda-detalle-content").rp3LoadingPanel("close");
        $("#modal-agenda-content-general").html(data);
        
        $('[agenda-gestion-detail-content] .fancybox').fancybox();
        $('[agenda-gestion-detail-content] .roleTree').treegrid();        

        $('[agenda-gestion-detail-content] [map-expand]').click(function () {
            openAgendaDetalleMapExpand();
        });

        setAgendaDetallePage(AGENDA_PAGE_DETAIL_MAIN);

        
    });
};


/*CYCLE DIALOG :: TAREA*/
function openAgendaDetalleTarea(idRuta, idAgenda, idTarea) {
    current_agenda_detalle_idRuta = idRuta;
    current_agenda_detalle_idAgenda = idAgenda;
    rp3Get("/Ruta/Agenda/GetTareas", { idRuta: idRuta, idAgenda: idAgenda, idTarea: idTarea }, function (data) {
        $("#modal-agenda-content-tareas").html(data);
        setAgendaDetallePage(AGENDA_PAGE_DETAIL_TAREAS);
    });
};

/*CYCLE DIALOG :: EDIT*/
function openAgendaDetalleEdit() {
    rp3Get("/Ruta/Agenda/Update", { idRuta: current_agenda_detalle_idRuta, idAgenda: current_agenda_detalle_idAgenda }, function (data) {
        $("#modal-agenda-content-editar").html(data);
        setAgendaDetallePage(AGENDA_PAGE_DETAIL_EDITAR);
    });
};

//DELETE
function eliminarAgendaPost() {

    rp3ModalHide("modal-agenda-detalle");

    rp3DialogConfirmationMessage("¿Está seguro que desea eliminar la visita?", "Agenda", function (data) {
        rp3Post("/Ruta/Agenda/Delete", {
            idRuta: current_agenda_detalle_idRuta,
            idAgenda: current_agenda_detalle_idAgenda
        }, function (data) {
            if (data.HasError)
                rp3NotifyAsPopup(data.Messages);
            else
                updateAppointments();            
        });
    });
}

//UPDATE 
function updateAgendaDetallePost() {
    var idAgenda = $("#agendaupdateform [idAgenda]").val();
    var idCliente = $("#agendaupdateform [idCliente]").val();
    var idContacto = $("#agendaupdateform [idContacto]").val();
    var motivo = $("#agendaupdateform [motivo]").val();
    var idClienteDireccion = "default";

    //var inicio = getTicks(new Date(Date.parseExact(String($("#agendaupdateform [startdateA] input").val()).replace(". ", "").replace(".", ""), "dd/MM/yyyy HH:mm")));
    var inicio = getTicks(new Date(Date.parseExact($("#agendaupdateform [startdateA] input").val() + " " + $("#agendaupdateform [starthourA] input").val(), "dd/MM/yyyy HH:mm")));
    var duracion = $('#agendaupdateform select[duracion]').select2('val');
    //var fin = getTicks(new Date(Date.parseExact(String($("#agendaupdateform [startdateB] input").val()).replace(". ", "").replace(".", ""), "dd/MM/yyyy HH:mm")));

    var tareas = $("#agendaupdateform [tareas]").select2('val').toString();
    rp3Post("/Ruta/Agenda/Update", {
        IdRuta: currentIdRutaAgenda,
        IdCliente: idCliente,
        IdAgenda: idAgenda,
        IdContacto: idContacto,
        IdClienteDireccion: idClienteDireccion,
        FechaInicioTicks: inicio,
        //FechaFinTicks: fin,
        DuracionVisita: duracion,
        Motivo: motivo,
        TareasSeleccion: tareas
    }, function (data) {
        if (!data.HasError) {
            rp3ModalHide("modal-agenda-detalle");
            updateAppointments();
        }
        rp3NotifyAsPopup(data.Messages);
    });
}

/*****************************************************************************************************************/
/* MODAL VIEW INITIATION ***************************************************************************************/
/*****************************************************************************************************************/

function init_Modal_Update(t_cliente, t_direccion, t_tarea) {

    $("#agendaupdateform input[select-direccion]").select2({ placeholder: "Elija una Dirección", data: function () { return { results: clienteDireccionEditAgendaData }; } });
    $("#agendaupdateform select[fin]").select2();
    $("#agendaupdateform #searchAgendaTo").keydown(function () {
        $("#agendaupdateform [idCliente]").val(0);
        $("#agendaupdateform [idContacto]").val(0);
        $("#agendaupdateform input[select-direccion]").select2('val', '');
    });
    /*************************************************************************/
    //SELECT2 WORKAROUND (PATCH)
    $("div.md-overlay").on('click', function () {
        $(".select2").select2("close");
        $(".select2-container").select2("close");
        $("#agendaeditform [tareas]").select2("close");
        $("#agendaeditform input[select-direccion]").select2("close");
    });
    $("div.modal-body").on('click', function () {
        $(".select2").select2("close");
        $(".select2-container").select2("close");
        $("#agendaupdateform [tareas]").select2("close");
        $("#agendaupdateform input[select-direccion]").select2("close");
    });
    $("button").on('click', function () {
        $(".select2").select2("close");
        $(".select2-container").select2("close");
        $("#agendaupdateform [tareas]").select2("close");
        $("#agendaupdateform input[select-direccion]").select2("close");
    });

    $("#agendaupdateform [tareas]").select2();

    $("#agendaupdateform [motivo]").select2({ placeholder: "Seleccione un motivo", allowClear: true });

    //$("#agendaupdateform [tareas]").select2();

    var esreprogramada = $("#agendaupdateform [esreprogramada]").val();

    if (esreprogramada == "False") {
        $("#agendaupdateform [motivo]").select2("enable", false);
        $("#agendaupdateform [motivo]").select2('val', '');
        $('#agendaupdateform [motivo]').select2({ height: 'resolve', placeholder: "Seleccione un motivo" });

        $('#agendaupdateform [startdateA]').on('change', function () {

            $("#agendaupdateform [motivo]").select2("enable", true);
        });

        $('#agendaupdateform [starthourA]').on('change', function () {

            $("#agendaupdateform [motivo]").select2("enable", true);
        });

        $('#agendaupdateform [duracion]').on('change', function () {

            //$("#agendaupdateform [motivo]").select2("enable", true);
        });
    }

    //$('#agendaupdateform [startdateA]').datetimepicker({
    //    timePicker: true,
    //    timePickerIncrement: 30,
    //    format: agendaInicioFormatDate,
    //    startDate: formatDate(dateToday),
    //    initialDate: formatDate(dateToday),
    //    autoclose: true
    //});

    document.getElementById("txt_clientecontacto").disabled = true;
    document.getElementById("txt_direccion").disabled = true;

    var t_fechaIni = Date.parseExact(String($("#agendaupdateform [fechaInicio]").val()).replace(". ", "").replace(".", ""), "dd/MM/yyyy hh:mm:ss tt");
    var t_tiempoFin = Date.parseExact(String($("#agendaupdateform [fechaFin]").val()).replace(". ", "").replace(".", ""), "dd/MM/yyyy hh:mm:ss tt");

    document.getElementById("txt_clientecontacto").value = t_cliente;
    document.getElementById("txt_direccion").value = t_direccion;

    var str = t_tarea;
    if (str != null && str != "") {
        var selectedValues = str.replace('-', ',').split(",");
        if (typeof selectedValues !== 'undefined' && selectedValues.length > 0) {
            $.each($("#agendaupdateform [tareas]"), function () {
                $(this).select2('val', selectedValues);
            });
        }
    }

    $('#agendaupdateform [startdatea]').datetimepicker({
        //timePicker: true,
        //timePickerIncrement: 30,
        //format: agendaInicioFormatDate,
        //startDate: formatDate(dateYesterday),
        //initialDate: formatDate(dateToday),
        //autoclose: true
        maxView: 2,
        minView: 2,
        startView: 2,
        datepicker: true,
        timePicker: false,
        language: 'es',
        format: agendaInicioFormatDate,
        startDate: formatDate(dateYesterday),
        initialDate: formatDate(new Date(t_fechaIni)),//formatDate(dateToday),
        autoclose: true
    });

    $('#agendaupdateform [startdatea] input').val(formatDate(new Date(t_fechaIni)));

    $('#agendaupdateform [starthoura]').datetimepicker({
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

    $('#agendaupdateform [starthoura] input').val(formatTime(new Date(t_fechaIni)));

    $('#agendaupdateform select[duracion]').select2();

    //$('#agendaupdateform [startdateA] input').val(formatDate(new Date(t_fechaIni)));
    //$('#agendaupdateform [startdateA]').datetimepicker({
    //    timePicker: true,
    //    timePickerIncrement: 30,
    //    format: agendaInicioFormatDate,
    //    startDate: formatDate(dateYesterday),//formatDate(new Date(t_fechaIni)),
    //    initialDate: formatDate(new Date(t_fechaIni)),
    //    autoclose: true
    //});

    //$('#agendaupdateform [startdateB] input').val(formatDate(new Date(t_tiempoFin)));
    //$('#agendaupdateform [startdateB]').datetimepicker({
    //    startView: 1,
    //    minView: 0,
    //    maxView: 1,
    //    format: agendaInicioFormatDate,
    //    startDate: formatDate(addMinutes(new Date(t_fechaIni), 5)),
    //    endDate: formatDateEnd(new Date(t_tiempoFin)),
    //    initialDate: formatDate(new Date(t_tiempoFin)),
    //    autoclose: true
    //});

    //$('#agendaupdateform [startdateA]').datetimepicker().on('changeDate', function (ev) {
    //    var conv_date = Date.parseExact(String($("#agendaupdateform [startdateA] input").val()).replace(". ", "").replace(".", ""), "dd/MM/yyyy HH:mm");
    //    var result = formatDate(addMinutes(conv_date, diffMinutes));
    //    $('#agendaupdateform [startdateB]').datetimepicker('remove');
    //    $('#agendaupdateform [startdateB]').datetimepicker({
    //        startView: 1,
    //        minView: 0,
    //        maxView: 1,
    //        format: agendaInicioFormatDate,
    //        startDate: formatDate(addMinutes(conv_date, 5)),
    //        endDate: formatDateEnd(new Date(conv_date)),
    //        initialDate: formatDate(addMinutes(conv_date, diffMinutes)),
    //        autoclose: true
    //    });

    //    $('#agendaupdateform [startdateB] input').val(result);
    //});
}

function init_Modal_Create() {

    $('#searchAgendaButton').on('click', function () {

        rp3ModalShow("modal-agenda-cliente-search");
        $("#modal-agenda-cliente-search-content").rp3LoadingPanel();
        rp3Get("/Ruta/Agenda/GetSeleccionCliente", { idRuta: currentIdRutaAgenda }, function (data) {
            $("#modal-agenda-cliente-search-content").rp3LoadingPanel('close');
            $("#modal-agenda-cliente-search-content").html('');
            $("#modal-agenda-cliente-search-content").html(data);

            $("#modal-agenda-cliente-search-content #searchclientetable tr td").click(function () {
                //$("#searchclientetable_wrapper tr td").removeClass("selected-item");
                //$(this).addClass("selected-item");


                var idCliente = parseInt($(this).attr('idCliente'));
                var idContacto = parseInt($(this).attr('idContacto'));
                var descripcion = $(this).attr('descripcion')

                if (!idContacto)
                    idContacto = 0;

                $("#agendacreateform [idCliente]").val(0);
                $("#agendacreateform [idContacto]").val(0);

                $("#agendacreateform [idCliente]").val(idCliente);
                $("#agendacreateform [idContacto]").val(idContacto);

                loadDireccionEditAgenda(idCliente, idContacto);

                $("#agendacreateform #searchAgendaTo").val(descripcion);

                rp3ModalHide("modal-agenda-cliente-search");

            });

            $("#modal-agenda-cliente-search-content #searchclientetable").rp3DataTable()
            //$('#modal-agenda-cliente-search-content #searchclientetable').dataTable({
            //    "sScrollY": 280,
            //    "bPaginate": false,
            //    "bFilter": true,
            //    "bInfo": false
            //});
        });

    });

    $('#agendacreateform [startdate1]').datetimepicker({
        //timePicker: true,
        //timePickerIncrement: 30,
        //format: agendaInicioFormatDate,
        //startDate: formatDate(dateYesterday),
        //initialDate: formatDate(dateToday),
        //autoclose: true
        maxView: 2,
        minView: 2,
        startView: 2,
        datepicker: true,
        timePicker: false,
        language: 'es',
        format: agendaInicioFormatDate,
        startDate: formatDate(dateYesterday),
        initialDate: formatDate(dateToday),
        autoclose: true
    });

    $('#agendacreateform [startdate1] input').val(formatDate(dateToday));

    $('#agendacreateform [starthour1]').datetimepicker({
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

    $('#agendacreateform [starthour1] input').val(formatTime(dateToday));

    $('#agendacreateform select[duracion]').select2();

    //$('#agendacreateform [startdate2] input').val(formatDate(addMinutes(new Date(dateToday), diffMinutes)));
    //$('#agendacreateform [startdate2]').datetimepicker({
    //    startView: 1,
    //    minView: 0,
    //    maxView: 1,
    //    format: agendaInicioFormatDate,
    //    startDate: formatDate(addMinutes(new Date(dateToday), 5)),
    //    endDate: formatDateEnd(new Date(dateToday)),
    //    initialDate: formatDate(addMinutes(new Date(dateToday), diffMinutes)),
    //    autoclose: true
    //});

    //$('#agendacreateform [startdate1]').datetimepicker().on('changeDate', function (ev) {
    //    var conv_date = Date.parseExact(String($("#agendacreateform [startdate1] input").val()).replace(". ", "").replace(".", ""), "dd/MM/yyyy HH:mm");
    //    var result = formatDate(addMinutes(conv_date, diffMinutes));

    //    $('#agendacreateform [startdate2]').datetimepicker('remove');
    //    $('#agendacreateform [startdate2]').datetimepicker({
    //        startView: 1,
    //        minView: 0,
    //        maxView: 1,
    //        format: agendaInicioFormatDate,
    //        startDate: formatDate(addMinutes(conv_date, 5)),
    //        endDate: formatDateEnd(new Date(conv_date)),
    //        initialDate: formatDate(addMinutes(conv_date, diffMinutes)),
    //        autoclose: true
    //    });

    //    $('#agendacreateform [startdate2] input').val(result);
    //});

    $("#agendacreateform input[select-direccion]").select2({ placeholder: "Elija una Dirección", data: function () { return { results: clienteDireccionEditAgendaData }; } });
    $("#agendacreateform select[fin]").select2();
    $("#agendacreateform #searchAgendaTo").keydown(function () {
        $("#agendacreateform [idCliente]").val(0);
        $("#agendacreateform [idContacto]").val(0);
        $("#agendacreateform input[select-direccion]").select2('val', '');
    });
    /*************************************************************************/
    //SELECT2 WORKAROUND (PATCH)
    $("div.md-overlay").on('click', function () {
        $(".select2").select2("close");
        $(".select2-container").select2("close");
        $("#agendacreateform [tareas]").select2("close");
        $("#agendacreateform input[select-direccion]").select2("close");
    });
    $("div.modal-body").on('click', function () {
        $(".select2").select2("close");
        $(".select2-container").select2("close");
        $("#agendacreateform [tareas]").select2("close");
        $("#agendacreateform input[select-direccion]").select2("close");
    });
    $("button").on('click', function () {
        $(".select2").select2("close");
        $(".select2-container").select2("close");
        $("#agendacreateform [tareas]").select2("close");
        $("#agendacreateform input[select-direccion]").select2("close");
    });

    $("#agendacreateform #searchAgendaTo").autocomplete({
        source: function (request, response) {
            $.getJSON(RP3_ROOT_PATH + "/General/Cliente/ClienteContactoAutocomplete", { term: request.term, idruta: String(currentIdRutaAgenda) }, response);
        },
        minLength: 3,
        select: function (event, ui) {

            $("#agendacreateform [idCliente]").val(0);
            $("#agendacreateform [idContacto]").val(0);

            $("#agendacreateform [idCliente]").val(ui.item.idCliente);
            $("#agendacreateform [idContacto]").val(ui.item.idContacto);

            loadDireccionEditAgenda(ui.item.idCliente, ui.item.idContacto);

            $(event.target).val(ui.item.value);
            event.preventDefault();
        },
        open: function () {
            //$("#agendaeditform .ui-autocomplete").width($("#agendaeditform #searchAgendaTo").width()-10);
        }
    });
    $("#agendacreateform #searchAgendaTo").autocomplete("widget").insertAfter($("#agendaeditform"));

    $("#agendacreateform [tareas]").select2();
}

function init_Modal_Tareas() {
    $('#modal-agenda-content-tareas .roleTree').treegrid();
    $("[action='but-back']").click(function () {
        setAgendaDetallePage(AGENDA_PAGE_DETAIL_MAIN);
    });
}

function openAgendaDetalleMapExpand() {
    rp3ModalShow("modal-agenda-detalle-map");
    $("#modal-agenda-detalle-map-content").rp3LoadingPanel();
    rp3Get("/Ruta/Agenda/ExpandMap", { idRuta: current_agenda_detalle_idRuta, idAgenda: current_agenda_detalle_idAgenda }, function (data) {
        $("#modal-agenda-detalle-map-content").rp3LoadingPanel('close');
        $("#modal-agenda-detalle-map-content").html(data);        
    });
};