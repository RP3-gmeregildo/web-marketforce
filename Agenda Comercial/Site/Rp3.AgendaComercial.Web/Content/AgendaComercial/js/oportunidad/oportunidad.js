var OPORTUNIDAD_PAGE_DETAIL_MAIN = 1;
var OPORTUNIDAD_PAGE_DETAIL_ETAPAS = 2;
var OPORTUNIDAD_PAGE_DETAIL_TAREAS = 3;
var OPORTUNIDAD_PAGE_DETAIL_SUBETAPAS = 4;
var OPORTUNIDAD_PAGE_DETAIL_INFO = 5;
var OPORTUNIDAD_PAGE_DETAIL_BITACORA = 6;

var currentFechaInicial = null;
var currentFechaFinal = null;

var CURRENT_PAGE = 1;

var sortField = "calificacion";
var sortMode = 2;

var current_oportunidad_detalle_idOportunidad = null;
var current_oportunidad_detalle_idEtapa = null;
var current_oportunidad_detalle_idSubEtapa = null;

//Listado Busqueda
var sp_busqueda = "";
var sp_pagina = 1;
var sp_num_registros = 10;
var sp_num_paginas = 1;

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

    $("#elegir-Agente").click(function () {
        elegirAgente();
    });

    $('#estado').select2('val', 'A');

    $('#estado').change(function () {
        showOportunidadListado(currentFechaInicial, currentFechaFinal);
    });

    $('#diasInactividad').change(function () {
        showOportunidadListado(currentFechaInicial, currentFechaFinal);
    });

    $('#tipo').change(function () {
        showOportunidadListado(currentFechaInicial, currentFechaFinal);
    });

    $("#modal-oportunidad-detalle-button-eliminar").click(function () {
        eliminarOportunidadPost();
    });

    $("#modal-oportunidad-detalle-button-capturar").click(function () {

        window.open(RP3_ROOT_PATH + "/Oportunidad/Oportunidad/Preview?idOportunidad=" + current_oportunidad_detalle_idOportunidad + "&addressBase=" + RP3_ROOT_PATH, "_blank");

        //$("#canvasArea").html($("#modal-oportunidad-detalle-content").html());

        //html2canvas($("#modal-oportunidad-detalle-content"), {
        //    useCORS:true,
        //    onrendered: function (canvas) {

        //        //var imgString = canvas.toDataURL("image/png");
        //        //window.open(imgString);

        //        theCanvas = canvas;
        //        canvas.toBlob(function (blob) {
        //            saveAs(blob, "oportunidad.png");
        //        });

        //        //window.location.href = 'mailto:?subject=Oportunidad&body=Captura de oportunidad&Attachment="C:\\oportunidad.png"';
        //    }
        //});
    });

    $("#modal-oportunidad-detalle-button-back").click(function () {

        if (CURRENT_PAGE == OPORTUNIDAD_PAGE_DETAIL_ETAPAS) {
            CURRENT_PAGE = OPORTUNIDAD_PAGE_DETAIL_MAIN;
        } else if (CURRENT_PAGE == OPORTUNIDAD_PAGE_DETAIL_SUBETAPAS) {
            CURRENT_PAGE = OPORTUNIDAD_PAGE_DETAIL_ETAPAS;
        } else if (CURRENT_PAGE == OPORTUNIDAD_PAGE_DETAIL_TAREAS) {
            CURRENT_PAGE = OPORTUNIDAD_PAGE_DETAIL_ETAPAS;
        } else if (CURRENT_PAGE == OPORTUNIDAD_PAGE_DETAIL_INFO) {
            CURRENT_PAGE = OPORTUNIDAD_PAGE_DETAIL_MAIN;
        } else if (CURRENT_PAGE == OPORTUNIDAD_PAGE_DETAIL_BITACORA) {
            CURRENT_PAGE = OPORTUNIDAD_PAGE_DETAIL_MAIN;
        }

        setOportunidadDetallePage(CURRENT_PAGE);
    });

    initMain();
});

/*OPEN DIALOG :: VIEW*/
function openOportunidadDetalleView(idOportunidad, estado) {
    current_oportunidad_detalle_idOportunidad = idOportunidad;

    //CURRENT_PAGE = OPORTUNIDAD_PAGE_DETAIL_MAIN;

    $("#modal-oportunidad-content-general").empty();
    rp3ModalShow("modal-oportunidad-detalle");

    $("#modal-oportunidad-detalle-content").rp3LoadingPanel();
    $("#modal-oportunidad-footer-back").hide();

    rp3Get("/Oportunidad/Oportunidad/GetDatos", { idOportunidad: idOportunidad }, function (data) {
        $("#modal-oportunidad-detalle-content").rp3LoadingPanel("close");
        $("#modal-oportunidad-content-general").html(data);

        $("[idetapa]").click(function () {
            var idEtapa = $(this).attr("idetapa");

            openOportunidadEtapaDetalleView(current_oportunidad_detalle_idOportunidad, idEtapa);
        });

        $('[oportunidad-gestion-detail-content] .fancybox').fancybox();
        $('[oportunidad-gestion-detail-content] .roleTree').treegrid();

        $('[oportunidad-gestion-detail-content] [map-expand]').click(function () {
            openOportunidadDetalleMapExpand();
        });

        $('[oportunidad-gestion-detail-content] [more-info]').click(function () {
            openOportunidadDetalleInfo(idOportunidad);
        });

        $('[oportunidad-gestion-detail-content] [bitacora]').click(function () {
            openOportunidadDetalleBitacora(idOportunidad);
        });

        setOportunidadDetallePage(OPORTUNIDAD_PAGE_DETAIL_MAIN);
    });
};

function openOportunidadDetalleInfo(idOportunidad) {
    rp3Get("/Oportunidad/Oportunidad/GetMoreInfo", { idOportunidad: idOportunidad }, function (data) {

        $("#modal-oportunidad-content-info").html(data);

        $('[oportunidad-gestion-info-content] .fancybox').fancybox();

        setOportunidadDetallePage(OPORTUNIDAD_PAGE_DETAIL_INFO);
    });
}

function openOportunidadDetalleBitacora(idOportunidad) {
    rp3Get("/Oportunidad/Oportunidad/GetBitacora", { idOportunidad: idOportunidad }, function (data) {

        $("#modal-oportunidad-content-bitacora").html(data);

        $('[oportunidad-gestion-info-bitacora] .fancybox').fancybox();

        setOportunidadDetallePage(OPORTUNIDAD_PAGE_DETAIL_BITACORA);
    });
}

function openOportunidadEtapaDetalleView(idOportunidad, idEtapa) {

    //CURRENT_PAGE = OPORTUNIDAD_PAGE_DETAIL_ETAPAS;

    current_oportunidad_detalle_idOportunidad = idOportunidad;
    current_oportunidad_detalle_idEtapa = idEtapa;

    rp3Get("/Oportunidad/Oportunidad/GetEtapas", { idOportunidad: idOportunidad, idEtapa: idEtapa }, function (data) {

        $("#modal-oportunidad-content-etapas").html(data);

        setOportunidadDetallePage(OPORTUNIDAD_PAGE_DETAIL_ETAPAS);

        $("[idsubetapa]").click(function () {
            var idSubEtapa = $(this).attr("idsubetapa");

            openOportunidadSubEtapaDetalleView(current_oportunidad_detalle_idOportunidad, idSubEtapa);
        });

        $("[idtarea]").click(function () {
            var idTarea = $(this).attr("idtarea");

            openOportunidadTareaDetalleView(current_oportunidad_detalle_idOportunidad, idEtapa, idTarea);
        });
    });
}

function openOportunidadSubEtapaDetalleView(idOportunidad, idEtapa) {
    //current_oportunidad_detalle_idOportunidad = idOportunidad;
    current_oportunidad_detalle_idSubEtapa = idEtapa;

    //CURRENT_PAGE = OPORTUNIDAD_PAGE_DETAIL_SUBETAPAS;

    rp3Get("/Oportunidad/Oportunidad/GetEtapas", { idOportunidad: idOportunidad, idEtapa: idEtapa }, function (data) {
        $("#modal-oportunidad-content-subetapas").html(data);

        setOportunidadDetallePage(OPORTUNIDAD_PAGE_DETAIL_SUBETAPAS);

        $("[idtarea]").click(function () {
            var idTarea = $(this).attr("idtarea");

            openOportunidadTareaDetalleView(current_oportunidad_detalle_idOportunidad, idEtapa, idTarea);
        });
    });
}

function openOportunidadTareaDetalleView(idOportunidad, idEtapa, idTarea) {
    //current_oportunidad_detalle_idOportunidad = idOportunidad;
    //current_oportunidad_detalle_idEtapa = idEtapa;

    //CURRENT_PAGE = OPORTUNIDAD_PAGE_DETAIL_TAREAS;

    rp3Get("/Oportunidad/Oportunidad/GetTareas", { idOportunidad: idOportunidad, idEtapa: idEtapa, idTarea: idTarea }, function (data) {
        $("#modal-oportunidad-content-tareas").html(data);

        setOportunidadDetallePage(OPORTUNIDAD_PAGE_DETAIL_TAREAS);
    });
}

function init_Modal_Tareas() {
    $('#modal-oportunidad-content-tareas .roleTree').treegrid();
    //$("[action='but-back']").click(function () {
    //    setOportunidadDetallePage(OPORTUNIDAD_PAGE_DETAIL_ETAPAS);
    //});
}

function setOportunidadDetallePage(opt) {

    CURRENT_PAGE = opt;

    if (opt == OPORTUNIDAD_PAGE_DETAIL_MAIN) {
        $("#modal-oportunidad-content-general").show();
        $("#modal-oportunidad-content-etapas").hide();
        $("#modal-oportunidad-content-subetapas").hide();
        $("#modal-oportunidad-content-tareas").hide();
        $("#modal-oportunidad-content-info").hide();
        $("#modal-oportunidad-content-bitacora").hide();

        $("#modal-oportunidad-footer-main").show();
        $("#modal-oportunidad-footer-back").hide();
    }
    if (opt == OPORTUNIDAD_PAGE_DETAIL_ETAPAS) {
        $("#modal-oportunidad-content-general").hide();
        $("#modal-oportunidad-content-etapas").show();
        $("#modal-oportunidad-content-subetapas").hide();
        $("#modal-oportunidad-content-tareas").hide();
        $("#modal-oportunidad-content-info").hide();
        $("#modal-oportunidad-content-bitacora").hide();

        $("#modal-oportunidad-footer-main").hide();
        $("#modal-oportunidad-footer-back").show();
    }
    if (opt == OPORTUNIDAD_PAGE_DETAIL_SUBETAPAS) {
        $("#modal-oportunidad-content-general").hide();
        $("#modal-oportunidad-content-etapas").hide();
        $("#modal-oportunidad-content-subetapas").show();
        $("#modal-oportunidad-content-tareas").hide();
        $("#modal-oportunidad-content-info").hide();
        $("#modal-oportunidad-content-bitacora").hide();

        $("#modal-oportunidad-footer-main").hide();
        $("#modal-oportunidad-footer-back").show();
    }
    if (opt == OPORTUNIDAD_PAGE_DETAIL_TAREAS) {
        $("#modal-oportunidad-content-general").hide();
        $("#modal-oportunidad-content-etapas").hide();
        $("#modal-oportunidad-content-subetapas").hide();
        $("#modal-oportunidad-content-tareas").show();
        $("#modal-oportunidad-content-info").hide();
        $("#modal-oportunidad-content-bitacora").hide();

        $("#modal-oportunidad-footer-main").hide();
        $("#modal-oportunidad-footer-back").show();
    }
    if (opt == OPORTUNIDAD_PAGE_DETAIL_INFO) {
        $("#modal-oportunidad-content-general").hide();
        $("#modal-oportunidad-content-etapas").hide();
        $("#modal-oportunidad-content-subetapas").hide();
        $("#modal-oportunidad-content-tareas").hide();
        $("#modal-oportunidad-content-info").show();
        $("#modal-oportunidad-content-bitacora").hide();

        $("#modal-oportunidad-footer-main").hide();
        $("#modal-oportunidad-footer-back").show();
    }
    if (opt == OPORTUNIDAD_PAGE_DETAIL_BITACORA) {
        $("#modal-oportunidad-content-general").hide();
        $("#modal-oportunidad-content-etapas").hide();
        $("#modal-oportunidad-content-subetapas").hide();
        $("#modal-oportunidad-content-tareas").hide();
        $("#modal-oportunidad-content-info").hide();
        $("#modal-oportunidad-content-bitacora").show();

        $("#modal-oportunidad-footer-main").hide();
        $("#modal-oportunidad-footer-back").show();
    }
}


function elegirAgente() {
    window.location.href = RP3_ROOT_PATH + "/Oportunidad/Oportunidad/CambiarAgente";
};

/*****************************************************************************************************************/
/* MAIN VIEW INITIATION ***********************************************************************************************/
/*****************************************************************************************************************/
function initMain() {
    showOportunidadLista();
}

function init_Listado() {
    var cb = function (start, end) {
        $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
        $("#txtbusqueda").val('');
        showOportunidadListado(start.format('YYYY-MM-D hh:mm:ss'), end.format('YYYY-MM-D hh:mm:ss'));
    }

    var optionSet2 = {
        locale: 'es',
        startDate: moment().subtract('days', 364),
        endDate: moment(),
        opens: 'left',
        ranges: {
            //'Hoy': [moment(), moment()],
            //'Ayer': [moment().subtract('days', 1), moment().subtract('days', 1)],
            //'Últimos 15 Días': [moment().subtract('days', 14), moment()],
            'Último Mes': [moment().subtract('days', 29), moment()],
            'Último Trimestre': [moment().subtract('days', 89), moment()],
            'Último Semestre': [moment().subtract('days', 179), moment()],
            'Último Año': [moment().subtract('days', 364), moment()]
        }
    };

    initGroup();

    showOportunidadListado(moment().subtract('days', 364).format('YYYY-MM-D hh:mm:ss'), moment().format('YYYY-MM-D hh:mm:ss'));
    $('#reportrange span').html(moment().subtract('days', 364).format('MMMM D, YYYY') + ' - ' + moment().format('MMMM D, YYYY'));

    $('#reportrange').daterangepicker(optionSet2, cb);

    $("#txtbusqueda").keyup(function () {
        var word = $(this).val();
        $("#spinner-loading").hide().fadeIn(50);

        if (aplicarBusquedaOportunidad) clearTimeout(aplicarBusquedaOportunidad);

        if (word.length > 2) {
            busquedaOportunidadTrigger(word);
            $("#reportrange").addClass("disableDIV");
        }
        else if (word.length > 0) {
            $("#reportrange").addClass("disableDIV");
        }
        else {

            $('#reportrange span').html(moment().subtract('days', 364).format('MMMM D, YYYY') + ' - ' + moment().format('MMMM D, YYYY'));
            showOportunidadListado(moment().subtract('days', 364).format('YYYY-MM-D hh:mm:ss'), moment().format('YYYY-MM-D hh:mm:ss'));

            $("#reportrange").removeClass("disableDIV");
        }
        $("#spinner-loading").fadeOut(300);
    });
}

function initGroup() {

    currentGroupMode = 'L';

    $("[action='group-list']").click(function () {
        limpiarBotonesGroup();
        $(this).addClass("btn-primary");
        $("[action='group-state']").addClass("btn-default");
        $("[action='group-agent']").addClass("btn-default");
        $("[action='group-calendar']").addClass("btn-default");
        currentGroupMode = 'L';

        if (currentMode == 'L') {
            showOportunidadListado(currentFechaInicial, currentFechaFinal);
        }
    });

    $("[action='group-calendar']").click(function () {
        limpiarBotonesGroup();
        $(this).addClass("btn-primary");
        $("[action='group-state']").addClass("btn-default");
        $("[action='group-agent']").addClass("btn-default");
        $("[action='group-list']").addClass("btn-default");
        currentGroupMode = 'D';

        if (currentMode == 'L') {
            showOportunidadListado(currentFechaInicial, currentFechaFinal);
        }
    });

    $("[action='group-state']").click(function () {
        limpiarBotonesGroup();
        $(this).addClass("btn-primary");
        $("[action='group-calendar']").addClass("btn-default");
        $("[action='group-agent']").addClass("btn-default");
        $("[action='group-list']").addClass("btn-default");
        currentGroupMode = 'E';

        if (currentMode == 'L') {
            showOportunidadListado(currentFechaInicial, currentFechaFinal);
        }
    });

    $("[action='group-agent']").click(function () {
        limpiarBotonesGroup();
        $("[action='group-agent']").addClass("btn-primary");
        $("[action='group-calendar']").addClass("btn-default");
        $("[action='group-state']").addClass("btn-default");
        $("[action='group-list']").addClass("btn-default");
        currentGroupMode = 'A';

        if (currentMode == 'L') {
            showOportunidadListado(currentFechaInicial, currentFechaFinal);
        }
    });
}

//LISTADO :: Global Search
var aplicarBusquedaOportunidad;

function busquedaOportunidadTrigger(texto) {
    if (aplicarBusquedaOportunidad) clearTimeout(aplicarBusquedaOportunidad);
    aplicarBusquedaOportunidad = setTimeout(function () {
        sp_pagina = 1;
        BusquedaOportunidadListado(texto);
    }, 500);
}


function Sort(field) {
    if (field == sortField) {
        if (sortMode == 1)
            sortMode = 2;
        else
            sortMode = 1;
    }
    else
        sortMode = 1

    sortField = field;

    if (currentMode == 'L') {
        showOportunidadListado(currentFechaInicial, currentFechaFinal);
    }
}

function evaluateSortMode() {
    $("[sort-asc]").hide();
    $("[sort-desc]").hide();

    var parent = $("[field='" + sortField + "']");

    if (sortMode == 1)
        parent.find("[sort-asc]").show();
    else
        parent.find("[sort-desc]").show();
}

//LISTADO
function showOportunidadLista() {
    currentMode = 'L';
    rp3ShowLoadingPanel('#content-oportunidad');
    rp3Get("/Oportunidad/Oportunidad/GetLista", null, function (data) {
        $('#content-oportunidad').html(data);
        rp3HideLoadingPanel('#content-oportunidad');

        rp3ShowLoadingPanel('#content-listado');

        $("[action='sort-list']").click(function () {

            rp3ModalShow("modal-oportunidad-sort");

            $("#modal-oportunidad-sort-content").rp3LoadingPanel();

            rp3Get("/Oportunidad/Oportunidad/GetSort", null, function (data) {

                $("#modal-oportunidad-sort-content").rp3LoadingPanel('close');
                $("#modal-oportunidad-sort-content").html('');
                $("#modal-oportunidad-sort-content").html(data);

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
                    button = $('[field="calificacion"]').find("[action='order-desc']");

                    if (button) {
                        $(button).removeClass("btn-default");
                        $(button).addClass("btn-primary");
                    }
                }

                $("#modal-oportunidad-sort-content [action='order-asc']").click(function () {
                    sortField = $(this).parent().parent().attr('field');
                    sortMode = 1;

                    rp3ModalHide("modal-oportunidad-sort");

                    showOportunidadListado(currentFechaInicial, currentFechaFinal);
                });

                $("#modal-oportunidad-sort-content [action='order-desc']").click(function () {
                    sortField = $(this).parent().parent().attr('field');
                    sortMode = 2;

                    rp3ModalHide("modal-oportunidad-sort");

                    showOportunidadListado(currentFechaInicial, currentFechaFinal);
                });
            });
        });
    });
}

function BusquedaOportunidadListado(busqueda) {
    currentMode = 'L';
    currentGroupMode = 'L';

    limpiarBotonesGroup();

    $("[action='group-list']").addClass("btn-primary");
    $("[action='group-state']").addClass("btn-default");
    $("[action='group-agent']").addClass("btn-default");
    $("[action='group-calendar']").addClass("btn-default");

    var estado = $("#estado").val();

    sp_busqueda = busqueda;
    rp3ShowLoadingPanel('#content-oportunidad');
    rp3Get("/Oportunidad/Oportunidad/GetListadoSearch", { busqueda: busqueda, pagina: sp_pagina, numreg: sp_num_registros, estado: estado }, function (data) {
        $('#content-listado').html(data);
        rp3HideLoadingPanel('#content-oportunidad');
    });
}

function showOportunidadListadoInicial(fechaInicial, fechaFinal) {
    currentMode = 'L';
    currentFechaInicial = fechaInicial;
    currentFechaFinal = fechaFinal;
    var estado = $("#estado").val();

    $('#content-oportunidad').addClass("panel-loading");
    rp3ShowLoadingPanel('#content-agenda');
    rp3Get("/Oportunidad/Oportunidad/GetListadoInicial", { fechaInicial: fechaInicial, fechaFinal: fechaFinal, groupMode: currentGroupMode, estado: estado }, function (data) {
        $('#content-listado').html(data);
        rp3HideLoadingPanel('#content-oportunidad');
        $('#content-oportunidad').removeClass("panel-loading");

        evaluateSortMode();
    });
}

function showOportunidadListado(fechaInicial, fechaFinal) {
    currentMode = 'L';
    currentFechaInicial = fechaInicial;
    currentFechaFinal = fechaFinal;
    var estado = $("#estado").val();
    var tipo = $("#tipo").val();

    var diasInactividad = $("#diasInactividad").val();

    $('#content-oportunidad').addClass("panel-loading");
    rp3ShowLoadingPanel('#content-oportunidad');
    rp3Get("/Oportunidad/Oportunidad/GetListado", {
        fechaInicial: fechaInicial, fechaFinal: fechaFinal, groupMode: currentGroupMode, estado: estado, diasInactividad: diasInactividad, tipo: tipo,
        sortField: sortField, sortMode: sortMode
    }, function (data) {
        $('#content-listado').html(data);
        rp3HideLoadingPanel('#content-oportunidad');
        $('#content-oportunidad').removeClass("panel-loading");

        evaluateSortMode();
    });
}

function limpiarBotonesGroup() {
    $("[action='group-calendar']").removeClass("btn-primary");
    $("[action='group-calendar']").removeClass("btn-default");
    $("[action='group-state']").removeClass("btn-primary");
    $("[action='group-state']").removeClass("btn-default");
    $("[action='group-agent']").removeClass("btn-primary");
    $("[action='group-agent']").removeClass("btn-default");
    $("[action='group-list']").removeClass("btn-primary");
    $("[action='group-list']").removeClass("btn-default");
}

function ListadoNextPage(sp_total_paginas) {
    if ((sp_pagina + 1) <= sp_total_paginas) {
        sp_pagina++;
        BusquedaOportunidadListado(sp_busqueda);
    }

    evaluateListadoNavigationButtons(sp_total_paginas);
}

function ListadoNextEndPage(sp_total_paginas) {
    if ((sp_pagina + 1) <= sp_total_paginas) {
        sp_pagina = sp_total_paginas;
        BusquedaOportunidadListado(sp_busqueda);
    }
    else { }

    evaluateListadoNavigationButtons(sp_total_paginas);
}

function ListadoPreviousPage(sp_total_paginas) {
    if ((sp_pagina - 1) >= 1) {
        sp_pagina--;
        BusquedaOportunidadListado(sp_busqueda);
    }

    evaluateListadoNavigationButtons(sp_total_paginas);
}

function ListadoPreviousBeginPage(sp_total_paginas) {
    if ((sp_pagina - 1) >= 1) {
        sp_pagina = 1;
        BusquedaOportunidadListado(sp_busqueda);
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

function openOportunidadDetalleMapExpand() {
    rp3ModalShow("modal-oportunidad-detalle-map");
    $("#modal-oportunidad-detalle-map-content").rp3LoadingPanel();
    rp3Get("/Oportunidad/Oportunidad/ExpandMap", { idOportunidad: current_oportunidad_detalle_idOportunidad }, function (data) {
        $("#modal-oportunidad-detalle-map-content").rp3LoadingPanel('close');
        $("#modal-oportunidad-detalle-map-content").html(data);
    });
};

//DELETE
function eliminarOportunidadPost() {

    rp3ModalHide("modal-oportunidad-detalle");

    rp3DialogConfirmationMessage("¿Está seguro que desea eliminar la oportunidad?", "Oportunidades", function (data) {
        rp3Post("/Oportunidad/Oportunidad/Delete", {
            idOportunidad: current_oportunidad_detalle_idOportunidad
        }, function (data) {
            if (data.HasError)
                rp3NotifyAsPopup(data.Messages);
            else
                showOportunidadListado(currentFechaInicial, currentFechaFinal);
        });
    });
}


function setUbicacion(control) {
    var item = $(control);

    var latitud = parseFloat($(item).attr('latitud'));
    var longitud = parseFloat($(item).attr('longitud'));
    var markerIndex = parseInt($(item).attr('markerIndex'));
    var markerStart = parseInt($(item).attr('markerstart'));
    var markerZIndex = parseInt($(item).attr('markerzindex'));
    var titulo = $(item).attr('titulo');

    openUbicacionDialog(markerIndex, latitud, longitud, titulo);
    rp3ModalHide("modal-oportunidad-detalle");
}

function openUbicacionDialog(markerIndex, latitud, longitud, titulo) {
    setUbicacionDialog(markerIndex, latitud, longitud, titulo, 850, 510);
};

function setUbicacionDialog(markerIndex, latitudCentro, longitudCentro, titulo, width, height) {
    rp3Get("/Oportunidad/Oportunidad/SetUbicacion", { markerIndex: markerIndex, latitud: latitudCentro, longitud: longitudCentro }, function (data) {
        $("#setUbicacionDialogContent").html(data);
        rp3ModalShow("setUbicacionDialog");
    });
}

function postSetUbicacion() {
    
    var latitud = $("#LatitudString").val();
    var longitud = $("#LongitudString").val();

    rp3Post("/Oportunidad/Oportunidad/SaveUbicacion", { idOportunidad: current_oportunidad_detalle_idOportunidad, latitud: latitud, longitud: longitud }, function (data) {

        rp3ModalHide("setUbicacionDialog");

        rp3NotifyAsPopup(data.Messages);

        //$("[ubicacion]").attr('latitud', latitud);
        //$("[ubicacion]").attr('longitud', longitud);

        //rp3Get("/General/Cliente/UbicacionMapMarkerClient", null, function (data) {
        //    $("#content_mapClient").html(data);
        //});

    });
}
