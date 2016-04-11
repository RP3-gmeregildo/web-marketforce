
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
            'Último Mes': [moment().subtract('days', 30), moment()]
        }
    };

    $('#reportrange span').html(moment().format('MMMM D, YYYY') + ' - ' + moment().format('MMMM D, YYYY'));

    currentFechaInicial = moment().format('YYYY-MM-D');
    currentFechaFinal = moment().format('YYYY-MM-D');
    inicioTicks = rp3GetTicks(new Date(currentFechaInicial + " "));
    finTicks = rp3GetTicks(new Date(currentFechaFinal + " "));

    $('#reportrange').daterangepicker(optionSet2, cb);

    consultar();

    $('#exportarExcel').click(function (e) {
        exportToExcel($('#dvData').html(), 'AnalisisAgentes.xls');
        e.preventDefault();
    });

});

function consultar() {
    rp3ShowLoadingPanel('#agente-content-container');
    rp3Get("/Marcacion/AnalisisAgente/GetData", { FechaInicioTicks: inicioTicks, FechaFinTicks: finTicks }, function (data) {
        $('#agente-content-container').html(data);
        rp3HideLoadingPanel('#agente-content-container');

        loadGraficoTotal();

        $("#agente-resumen-table tr td").click(function () {

            var selectable = $(this).hasClass("selectable-item");

            if (selectable) {

                var remover = $(this).hasClass("selected-item");

                if (!remover) {

                    $("#agente-resumen-table_wrapper tr td").removeClass("selected-item");
                    $(this).parent().find("td").addClass("selected-item");

                    var idAgente = $(this).parent().attr("idAgente");
                    var descripcion = $(this).parent().attr("descripcion");

                    var asistencias = $(this).parent().attr("asistencias");
                    var ausencias = $(this).parent().attr("ausencias");
                    var atrasos = $(this).parent().attr("atrasos");

                    $('#IdAgente').val(idAgente);

                    graficar(idAgente, descripcion, asistencias, atrasos, ausencias);
                }
                else {

                    $("#agente-resumen-table_wrapper tr td").removeClass("selected-item");
                    $('#IdAgente').val('');

                    loadGraficoTotal();
                }
            }
        });

        $("[detailbutton]").click(function (e) {
            var idAgente = $(this).attr('detailbutton');

            rp3ModalShow("modal-detalle");
            $("#modal-detalle-content").rp3LoadingPanel();
            rp3Get("/Marcacion/AnalisisAgente/GetDetalle", { IdAgente: idAgente, FechaInicioTicks: inicioTicks, FechaFinTicks: finTicks }, function (data) {
                $("#modal-detalle-content").rp3LoadingPanel('close');
                $("#modal-detalle-content").html(data);

                $('#exportarExcelDetalle').click(function (e) {
                    exportToExcelDetalle($('#dvDataDetalle').html(), 'AnalisisAgentesDetalle.xls');
                    e.preventDefault();
                });
            });

            e.preventDefault();
        });
    });
};

function loadGraficoTotal() {

    var asistencias = $('#Asistencias').val();
    var ausencias = $('#Ausencias').val();
    var atrasos = $('#Atrasos').val();

    graficar(null, null, asistencias, atrasos, ausencias);
}

function graficar(idAgente, agente, asistencias, atrasos, ausencias) {
    rp3ShowLoadingPanel('#char-content-container');
    rp3Get("/Marcacion/AnalisisAgente/GetChart", { IdAgente: idAgente, Agente: agente, Asistencias: asistencias, Atrasos: atrasos, Ausencias: ausencias }, function (data) {
        $('#char-content-container').html(data);
        rp3HideLoadingPanel('#char-content-container');
    });
}