
var currentFechaInicial = null;
var currentFechaFinal = null;
var inicioTicks = null;
var finTicks = null;

var currentMode = 'A';
var esSupervisor = false;
var originalcurrentIdAgente;
var currentIdAgente;
var currentMapMode = 'RUT'
var modechanged = true;

$(function () {

    $(window).resize(function () {
        resize();
    });

    $(window).load(function () {
        resize();
    });

    var cb = function (start, end) {
        $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));

        currentFechaInicial = start.format('YYYY-MM-D');
        currentFechaFinal = end.format('YYYY-MM-D');
        inicioTicks = rp3GetTicks(new Date(currentFechaInicial + " "));
        finTicks = rp3GetTicks(new Date(currentFechaFinal + " "));

        if (isExpanded)
            colapsePanel();

        consultar();
    };

    var optionSet2 = {
        locale: 'es',
        startDate: moment(),
        endDate: moment(),
        opens: 'left',
        ranges: {
            'Hoy': [moment(), moment()],
            'Ayer': [moment().subtract('days', 1), moment().subtract('days', 1)],
            'Próx. 30 Días': [moment(), moment().subtract('days', -29)],
            'Último Mes': [moment().subtract('days', 30), moment()],
            'Últimos 3 Meses': [moment().subtract('days', 90), moment()],
            'Últimos 6 Meses': [moment().subtract('days', 180), moment()],
            'Últimos 12 Meses': [moment().subtract('days', 360), moment()]
        }
    };

    $('#reportrange span').html(moment().format('MMMM D, YYYY') + ' - ' + moment().format('MMMM D, YYYY'));

    currentFechaInicial = moment().format('YYYY-MM-D');
    currentFechaFinal = moment().format('YYYY-MM-D');
    inicioTicks = rp3GetTicks(new Date(currentFechaInicial + " "));
    finTicks = rp3GetTicks(new Date(currentFechaFinal + " "));

    $('#reportrange').daterangepicker(optionSet2, cb);

    $('button[action="BACK"]').click(function () {
        currentMode = 'S';
        currentIdAgente = originalcurrentIdAgente;

        $("#labelagente").hide();
        $("#backbutton").hide();

        modechanged = true;
        consultar();
    });

    $("[action='show-ruta']").click(function () {
        limpiarBotonesMapa();
        $(this).addClass("btn-primary");
        $("[action='show-recorrido']").addClass("btn-default");
        currentMapMode = "RUT";
        consultaMap();
    });

    $("[action='show-recorrido']").click(function () {
        limpiarBotonesMapa();
        $(this).addClass("btn-primary");
        $("[action='show-ruta']").addClass("btn-default");
        currentMapMode = "REC";
        consultaMap();
    });

    $("#labelagente").hide();
    $("#backbutton").hide();

    if (esSupervisor)
        currentMode = 'S';
    else 
        currentMode = 'A';

    currentIdAgente = originalcurrentIdAgente;

    consultar();
});

function resize() {
    if (!isExpanded) {
        var totalsize = $(window).height() - $("#main-content").offset().top;
        var panelsize = totalsize / 2 - 45;

        if (panelsize < 200)
            panelsize = 200;

        $(".datapanel").css("height", panelsize);
        $(".datapanelmap").css("height", panelsize - 15);
        $(".datapanellist").css("height", panelsize - 15);

        setTimeout(function () {
            resizeTable("agente-resumen-table", "agente-list-content");
        }, 100);
    }
}

function resizeExpand(id) {
    var totalsize = $(window).height() - $("#expand-container").offset().top;
    $(id).css("height", totalsize);
}

function resizeTable(id, containerId) {
    if (containerId) {
        var table = window["rp3datatables_" + id];
        var height = parseInt($("#" + containerId).css('height').replace('px', '')) - 75;
        rp3DataTableSetScrollBodyHeight(id, height);
        $("#" + id + '_wrapper .dataTables_scroll').css('height', height + 30);
    }
    else {
        if ($("#" + id) && $("#" + id).offset()) {
            var table = window["rp3datatables_" + id];
            var height = $(window).height() - $("#" + id).offset().top - 75;
            rp3DataTableSetScrollBodyHeight(id, height);
            $("#" + id + '_wrapper .dataTables_scroll').css('height', height + 30);
        }
    }
}

function consultar() {
    if (currentMode == 'A')
        consultarAgente();
    else
        consultarSupervisor();

    modechanged = false;
    resize();
}

function consultarAgente() {

    $("#mapcol").removeClass('col-md-12');
    $("#mapcol").addClass('col-md-11');

    $("#mapbuttoncol").show();

    consultaGestionChart();
    consultaEfectividadChart();

    if (modechanged) {
        consultaClienteList();
        consultaMap();
    }
}

function consultarSupervisor() {

    $("#mapbuttoncol").hide();

    $("#mapcol").removeClass('col-md-11');
    $("#mapcol").addClass('col-md-12');

    consultaGestionChart();
    consultaEfectividadChart();
    consultaAgenteList();

    if (modechanged)
        consultaMapUbicacion();
}

function consultaGestionChart() {
    rp3ShowLoadingPanel('#gestion-chart-content');
    rp3Get("/Consulta/HomeDashboard/GestionChart", { Mode: currentMode, Inicio: inicioTicks, Fin: finTicks, IdAgente: currentIdAgente }, function (data) {
        $('#gestion-chart-content').html(data);
        rp3HideLoadingPanel('#gestion-chart-content');
    });
}

function consultaEfectividadChart() {
    rp3ShowLoadingPanel('#efectividad-chart-content');
    rp3Get("/Consulta/HomeDashboard/EfectividadChart", { Mode: currentMode, Inicio: inicioTicks, Fin: finTicks, IdAgente: currentIdAgente }, function (data) {
        $('#efectividad-chart-content').html(data);
        rp3HideLoadingPanel('#efectividad-chart-content');
    });
}

function consultaClienteList() {
    rp3ShowLoadingPanel('#agente-list-content');
    rp3Get("/Consulta/HomeDashboard/ClienteList", { Mode: currentMode, IdAgente: currentIdAgente }, function (data) {
        $('#agente-list-content').html(data);
        rp3HideLoadingPanel('#agente-list-content');
        resize();
    });
}

function consultaAgenteList() {
    rp3ShowLoadingPanel('#agente-list-content');
    rp3Get("/Consulta/HomeDashboard/AgenteList", { Mode: currentMode, Inicio: inicioTicks, Fin: finTicks, IdAgente: currentIdAgente }, function (data) {
        $('#agente-list-content').html(data);
        rp3HideLoadingPanel('#agente-list-content');

        setTimeout(function () {
            resizeTable("agente-resumen-table", "agente-list-content");
        }, 100);

        $("#agente-resumen-table .selectable-item").click(function () {

            if (isExpanded)
                colapsePanel();

            var idagente = $(this).parent().attr("idagente");
            var descripcion = $(this).parent().attr("descripcion");

            $("#labelagente span").text(descripcion);

            currentMode = 'A';
            currentIdAgente = idagente;

            $("#labelagente").show();
            $("#backbutton").show();

            modechanged = true;

            consultar();
        });
    });
}

function consultaMapUbicacion() {
    rp3ShowLoadingPanel('#map-content');
    rp3Get("/Consulta/HomeDashboard/Ubicacion", { Mode: currentMode, IdAgente: currentIdAgente }, function (data) {
        $('#map-content').html(data);
        rp3HideLoadingPanel('#map-content');
    });
}

function consultaMapRuta() {
    rp3ShowLoadingPanel('#map-content');
    rp3Get("/Consulta/HomeDashboard/Ruta", { Mode: currentMode, IdAgente: currentIdAgente }, function (data) {
        $('#map-content').html(data);
        rp3HideLoadingPanel('#map-content');
    });
}

function consultaMapRecorrido() {
    rp3ShowLoadingPanel('#map-content');
    rp3Get("/Consulta/HomeDashboard/Recorrido", { Mode: currentMode, IdAgente: currentIdAgente }, function (data) {
        $('#map-content').html(data);
        rp3HideLoadingPanel('#map-content');
    });
}

var idModelDiv;
var isExpanded = false;

function expandPanel(id) {
    isExpanded = true;

    idModelDiv = id;
    $(id).appendTo("#expand-content");

    $("#main-content").hide();
    $("#expand-container").show();

    resizeExpand(id);
    refresh();
}

function colapsePanel() {
    isExpanded = false;

    if (currentMode == 'S') {
        $("#backcol").hide();
        $("#blankcol").removeClass('col-md-4');
        $("#blankcol").addClass('col-md-6');
    }

    $(idModelDiv).appendTo(idModelDiv + "-container");
    $("#expand-container").hide();

    $("#main-content").show();

    resize();
    refresh();
}

function refresh() {
    if (idModelDiv == "#map-content") {
        checkResizeMap();
    } else if (idModelDiv == "#agente-list-content") {
        setTimeout(function () {
            resizeTable("agente-resumen-table", "agente-list-content");
        }, 100);
    } else if (idModelDiv == "#gestion-chart-content") { 
        var width = $("#gestion-chart-content").css("width").replace('px', '');
        var height = $("#gestion-chart-content").css("height").replace('px', '');
        chart.setSize(width, height);
    }
}

function consultaMap() {
    if (currentMapMode == "RUT")
        consultaMapRuta();
    else
        consultaMapRecorrido();
}

function limpiarBotonesMapa() {
    $("[action='show-ruta']").removeClass("btn-primary");
    $("[action='show-ruta']").removeClass("btn-default");
    $("[action='show-recorrido']").removeClass("btn-primary");
    $("[action='show-recorrido']").removeClass("btn-default");
}