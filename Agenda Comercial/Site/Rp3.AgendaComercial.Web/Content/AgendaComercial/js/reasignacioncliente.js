var origenbusquedaNombre = ""
var origentextoAnterior = "";
var origenpagina = 1;
var origennumreg = 10;

var destinobusquedaNombre = ""
var destinotextoAnterior = "";
var destinopagina = 1;
var destinonumreg = 10;

function resize() {
    setTimeout(function () {

        var heightPanel = $(window).height() - $('.block-flat').offset().top - 20;
        $('.block-flat').css('height', heightPanel + 'px');

        if ($('#IdRutaOrigen').val()) {
            if (window["rp3datatables_rutaorigentabla"].fnSettings().oScroll.sY != 140) {
                resizeTableHeight('rutaorigentabla', 140);
            }
        }
        else {
            resizeTable('rutaorigentabla');
        }

        if ($('#IdRutaDestino').val()) {
            if (window["rp3datatables_rutadestinotabla"].fnSettings().oScroll.sY != 140) {
                resizeTableHeight('rutadestinotabla', 140);
            }
        }
        else {
            resizeTable('rutadestinotabla');
        }

        resizeTable("rutaorigendetalle");
        resizeTable("rutadestinodetalle");

        rp3DataTableAjustsColumns('#rutaorigentabla');
        rp3DataTableAjustsColumns('#rutadestinotabla');
    }, 100);
}

function resizeTable(id) {
    if ($("#" + id) && $("#" + id).offset()) {
        var table = window["rp3datatables_" + id];
        if ($(table).parents(".dataTables_wrapper").find(".dataTables_filter").length > 0) {
            var height = $(window).height() - $("#" + id).offset().top - 70;
            rp3DataTableSetScrollBodyHeight(id, height);
            $("#" + id + '_wrapper .dataTables_scroll').css('height', height + 30);
        }
        else {
            var height = $(window).height() - $("#" + id).offset().top - 30;
            rp3DataTableSetScrollBodyHeight(id, height);
        }
    }
}

function resizeTableHeight(id, height) {
    if ($("#" + id)) {
        var table = window["rp3datatables_" + id];
        if ($(table).parents(".dataTables_wrapper").find(".dataTables_filter").length > 0) {
            rp3DataTableSetScrollBodyHeight(id, height);
            $("#" + id + '_wrapper .dataTables_scroll').css('height', height + 30);
        }
        else {
            rp3DataTableSetScrollBodyHeight(id, height);
        }
    }
}

$(function () {

    $(window).resize(function () {
        resize();
    });

    $(window).load(function () {
        resize();
    });

    setTimeout(function () { resize(); }, 500);

    $("#rutaremovertabla tbody").sortable({
        cursor: "cursor",
        handle: ".tr"
    }).disableSelection();

    $('button[action="FILTER"]').click(function (e) {
        e.preventDefault();
        showFilters(!$('#rutaOrigenRow').is(':visible'));
    });

    function showFilters(visible) {

        var rutaOrigenRow = $('#rutaOrigenRow');
        var rutaDestinoRow = $('#rutaDestinoRow');

        var i = $('button[action="FILTER"]').children("i");

        if (!visible) {

            var idRutaOrigen = $('#IdRutaOrigen').val();
            var idRutaDestino = $('#IdRutaDestino').val();

            if (idRutaOrigen && idRutaDestino) {
                i.removeClass('fa fa-minus');
                i.addClass('fa fa-plus');

                rutaOrigenRow.hide();
                rutaDestinoRow.hide();
            }
            else {
                rp3NotifyWarningAsPopup('Debe elegir el Origen y el Destino antes de ocultar los filtros.');
            }
        }
        else {
            i.removeClass('fa fa-plus');
            i.addClass('fa fa-minus');

            rutaOrigenRow.show();
            rutaDestinoRow.show();
        }

        resize();
    }

    $("#rutaorigentabla tr td").click(function () {

        var idRutaOrigen = $(this).parent().attr("idRuta");
        var idRutaDestino = $('#IdRutaDestino').val();
        if (idRutaOrigen && idRutaDestino) {
            if (idRutaOrigen == idRutaDestino) {
                rp3NotifyWarningAsPopup('La Ruta de Origen y Destino no pueden ser las mismas.');
                return;
            }
        }

        var remover = $(this).hasClass("selected-item");

        if (!remover) {
            $("#rutaorigentabla_wrapper tr td").removeClass("selected-item");
            var idRuta = $(this).parent().attr("idRuta");
            $("#rutaorigentabla_wrapper tr[idRuta='" + idRuta + "'] td").addClass("selected-item");

            $('#IdRutaOrigen').val(idRuta);

            var descripcion = $(this).parent().attr("descripcion");

            $("#origenColumn .title-block h5").text('ORIGEN: ' + descripcion);
        }
        else {
            $("#rutaorigentabla_wrapper tr td").removeClass("selected-item");
            $('#IdRutaOrigen').val('');
            $("#origenColumn .title-block h5").text('ORIGEN');
        }

        consultarorigen();
    });

    $("#rutadestinotabla tr td").click(function () {

        var idRutaDestino = $(this).parent().attr("idRuta");
        var idRutaOrigen = $('#IdRutaOrigen').val();
        if (idRutaOrigen && idRutaDestino) {
            if (idRutaOrigen == idRutaDestino) {
                rp3NotifyWarningAsPopup('La Ruta de Origen y Destino no pueden ser las mismas.');
                return;
            }
        }

        var remover = $(this).hasClass("selected-item");

        if (!remover) {
            $("#rutadestinotabla_wrapper tr td").removeClass("selected-item");
            var idRuta = $(this).parent().attr("idRuta");
            $("#rutadestinotabla_wrapper tr[idRuta='" + idRuta + "'] td").addClass("selected-item");

            $('#IdRutaDestino').val(idRuta);

            var descripcion = $(this).parent().attr("descripcion");

            $("#destinoColumn .title-block h5").text('DESTINO: ' + descripcion);
        }
        else {
            $("#rutadestinotabla_wrapper tr td").removeClass("selected-item");
            $('#IdRutaDestino').val('');
            $("#destinoColumn .title-block h5").text('DESTINO');
        }

        consultardestino();
    });
});

var origendestino = true;

function triggerDragAndDrop() {
    
    if ($("#rutaorigendetalle tr[idCliente]").length == 0 && $("#rutadestinodetalle tr[idCliente]").length == 0) {
        return;
    }

    $(".dragdrop tbody").sortable({
        cursor: "move",
        helper: "clone",//fixHelper,
        connectWith: '.linked tbody',
        start: function (event, ui) {
            if (ui.item.parent().prop('id') == "rutaorigendetalle-content") {
                origendestino = true;
            }
            else if (ui.item.parent().prop('id') == "rutadestinodetalle-content") {
                origendestino = false;
            }
        },
        beforeStop: function (event, ui) {
            if (origendestino && ui.item.parent().prop('id') == "rutaorigendetalle-content") {
                $(this).sortable("cancel");
            } else if (!origendestino && ui.item.parent().prop('id') == "rutadestinodetalle-content") {
                $(this).sortable("cancel");
            }
        },
        stop: function (event, ui) {
            var idRutaOrigen;
            var idRutaDestino;
            var clientes = [];

            if (origendestino) {
                idRutaOrigen = $('#IdRutaOrigen').val();
                idRutaDestino = $('#IdRutaDestino').val();

                clientes.push(ui.item.attr('idCliente'));

                $("#rutaorigendetalle tbody tr td[move].selected-item").each(function () {
                    clientes.push($(this).parent().attr('idCliente'));
                });
            }
            else {
                idRutaOrigen = $('#IdRutaDestino').val();
                idRutaDestino = $('#IdRutaOrigen').val();

                clientes.push(ui.item.attr('idCliente'));

                $("#rutadestinodetalle tbody tr td[move].selected-item").each(function () {
                    clientes.push($(this).parent().attr('idCliente'));
                });
            }

            if (ui.item.parent().prop('id') == "rutaremoverdetalle-content") {
                idRutaDestino = 0;
            }

            rp3Post("/Ruta/ReasignacionCliente/Reasignar", { idRutaOrigen: idRutaOrigen, idRutaDestino: idRutaDestino, clientes: clientes }, function (data) {
                rp3NotifyAsPopup(data.Messages);

                consultarorigen();
                consultardestino();
            });
        }
    }).disableSelection();

    //$("#rutadestinodetalle tbody").sortable({
    //    cursor: "cursor",
    //    handle: ".tr"
    //}).disableSelection();

    $("#rutaremovertabla tbody").sortable({
        cursor: "cursor",
        handle: ".tr"
    }).disableSelection();
};

function triggerOrigenClick() {
    $("#rutaorigendetalle tr td").unbind('click');
    $("#rutaorigendetalle tr td").click(function () {
        var remover = $(this).hasClass("selected-item");

        var idCliente = $(this).parent().attr("idCliente");

        if (!remover) {
            $("#rutaorigendetalle_wrapper tr[idCliente='" + idCliente + "'] td").addClass("selected-item");
        }
        else {
            $("#rutaorigendetalle_wrapper tr[idCliente='" + idCliente + "'] td").removeClass("selected-item");
        }
    });
}

function triggerDestinoClick() {
    $("#rutadestinodetalle tr td").unbind('click');
    $("#rutadestinodetalle tr td").click(function () {
        var remover = $(this).hasClass("selected-item");

        var idCliente = $(this).parent().attr("idCliente");

        if (!remover) {
            $("#rutadestinodetalle_wrapper tr[idCliente='" + idCliente + "'] td").addClass("selected-item");
        }
        else {
            $("#rutadestinodetalle_wrapper tr[idCliente='" + idCliente + "'] td").removeClass("selected-item");
        }
    });
}

function consultarorigen() {
    origenpagina = 1;
    origenbusquedaNombre = ""
    origentextoAnterior = "";
    $("#search-table-origen").val('');
    loadOrigenDetalleGrid();
};

function consultardestino() {
    destinopagina = 1;
    destinobusquedaNombre = ""
    destinotextoAnterior = "";
    $("#search-table-destino").val('');
    loadDestinoDetalleGrid();
};


//ORIGEN

function init_OrigenDetalle() {
    $("input[select-registro-origen]").select2('val', origennumreg);
    $("#search-table-origen").val(origentextoAnterior);
}

var origenrefreshing = false;
var origenrefreshpending = false;
//CONSULTAR
function loadOrigenDetalleGrid() {
    var d_search = origenbusquedaNombre;

    var idRuta = String(document.getElementById('IdRutaOrigen').value);

    if (idRuta == "") {
        $("#GridDetalleOrigen").html("");
        resize();
        return;
    }

    if (!origenrefreshing) {
        origenrefreshing = true;

        resize();

        rp3ShowLoadingPanel('#GridDetalleOrigen');

        rp3Get("/Ruta/ReasignacionCliente/ConsultaRutaOrigenDetalle", {
            ruta: idRuta,
            pagina: origenpagina,
            numreg: origennumreg,
            isfilter: 0,
            buscar: d_search
        }, function (data) {
            $("#GridDetalleOrigen").html(data);
            origenrefreshing = false;
            rp3HideLoadingPanel('#GridDetalleOrigen');

            triggerDragAndDrop();
            triggerOrigenClick();

            $("#search-table-origen").focus();
            resize();

            if (origenrefreshpending) {
                origenrefreshpending = false;
                loadOrigenDetalleGrid();
            }
        });
    } else {
        origenrefreshpending = true;
    }
}

var aplicarOrigenBusquedaTable;
function busquedaOrigenTableTrigger(texto) {
    if (texto != origentextoAnterior) {
        if (aplicarOrigenBusquedaTable) clearTimeout(aplicarOrigenBusquedaTable);
        origenbusquedaNombre = texto;
        origentextoAnterior = texto;
        aplicarOrigenBusquedaTable = setTimeout(function () {
            //Load   
            origenpagina = 1;
           // origenbusquedaNombre = texto;
            loadOrigenDetalleGrid();
            origenbusquedaNombre = "";
            //origentextoAnterior = texto;
        }, 700);
    }
}

//NEXT AND BEFORE BUTTONS
function OrigenListadoNextPage(sp_total_paginas) {
    if ((origenpagina + 1) <= sp_total_paginas) {
        origenpagina++;
        loadOrigenDetalleGrid();
    }

    evaluateOrigenNavigationButtons(total_paginas);
}

function OrigenListadoNextEndPage(sp_total_paginas) {
    if ((origenpagina + 1) <= sp_total_paginas) {
        origenpagina = sp_total_paginas;
        loadOrigenDetalleGrid();
    }
    else { }

    evaluateOrigenNavigationButtons(sp_total_paginas);
}

function OrigenListadoPreviousPage(sp_total_paginas) {
    if ((origenpagina - 1) >= 1) {
        origenpagina--;
        loadOrigenDetalleGrid();
    }

    evaluateOrigenNavigationButtons(total_paginas);
}

function OrigenListadoPreviousBeginPage(sp_total_paginas) {
    if ((origenpagina - 1) >= 1) {
        origenpagina = 1;
        loadOrigenDetalleGrid();
    }
    else { }

    evaluateOrigenNavigationButtons(sp_total_paginas);
}

function evaluateOrigenNavigationButtons(total_paginas) {
    $('button[action="origenlistado_prev_begin"').removeAttr('disabled');
    $('button[action="origenlistado_prev"').removeAttr('disabled');
    $('button[action="origenlistado_next_end"').removeAttr('disabled');
    $('button[action="origenlistado_next"').removeAttr('disabled');

    if (origenpagina - 1 < 1) {
        $('button[action="origenlistado_prev_begin"').attr('disabled', 'disabled');
        $('button[action="origenlistado_prev"').attr('disabled', 'disabled');
    }

    if (origenpagina + 1 > total_paginas) {
        $('button[action="origenlistado_next_end"').attr('disabled', 'disabled');
        $('button[action="origenlistado_next"').attr('disabled', 'disabled');
    }
}

//LOAD TABLE FILTER (# OF ROWS)
function UpdateOrigenNumReg(num) {
    origenpagina = 1;
    origennumreg = num;
    loadOrigenDetalleGrid();
}





//DESTINO

function init_DestinoDetalle() {
    $("input[select-registro-destino]").select2('val', destinonumreg);
    $("#search-table-destino").val(destinotextoAnterior);
}

var destinorefreshing = false;
var destinorefreshpending = false;
//CONSULTAR
function loadDestinoDetalleGrid() {
    var d_search = destinobusquedaNombre;

    var idRuta = String(document.getElementById('IdRutaDestino').value);

    if (idRuta == "") {
        $("#GridDetalleDestino").html("");
        resize();
        return;
    }

    resize();

    if (!destinorefreshing) {
        destinorefreshing = true;

        rp3ShowLoadingPanel('#GridDetalleDestino');

        rp3Get("/Ruta/ReasignacionCliente/ConsultaRutaDestinoDetalle", {
            ruta: idRuta,
            pagina: destinopagina,
            numreg: destinonumreg,
            isfilter: 0,
            buscar: d_search
        }, function (data) {
            $("#GridDetalleDestino").html(data);
            destinorefreshing = false;
            rp3HideLoadingPanel('#GridDetalleDestino');

            triggerDragAndDrop();
            triggerDestinoClick();

            $("#search-table-destino").focus();
            resize();

            if (destinorefreshpending) {
                destinorefreshpending = false;
                loadDestinoDetalleGrid();
            }
        });
    } else {
        destinorefreshpending = true;
    }
}

var aplicarDestinoBusquedaTable;
function busquedaDestinoTableTrigger(texto) {
    if (texto != destinotextoAnterior) {
        if (aplicarDestinoBusquedaTable) clearTimeout(aplicarDestinoBusquedaTable);
        aplicarDestinoBusquedaTable = setTimeout(function () {
            //Load   
            destinopagina = 1;
            destinobusquedaNombre = texto;
            loadDestinoDetalleGrid();
            destinobusquedaNombre = "";
            destinotextoAnterior = texto;
        }, 700);
    }
}

//NEXT AND BEFORE BUTTONS
function DestinoListadoNextPage(sp_total_paginas) {
    if ((destinopagina + 1) <= sp_total_paginas) {
        destinopagina++;
        loadDestinoDetalleGrid();
    }

    evaluateDestinoNavigationButtons(sp_total_paginas);
}

function DestinoListadoNextEndPage(sp_total_paginas) {
    if ((destinopagina + 1) <= sp_total_paginas) {
        destinopagina = sp_total_paginas;
        loadDestinoDetalleGrid();
    }
    else { }

    evaluateDestinoNavigationButtons(sp_total_paginas);
}

function DestinoListadoPreviousPage(sp_total_paginas) {
    if ((destinopagina - 1) >= 1) {
        destinopagina--;
        loadDestinoDetalleGrid();
    }

    evaluateDestinoNavigationButtons(sp_total_paginas);
}

function DestinoListadoPreviousBeginPage(sp_total_paginas) {
    if ((destinopagina - 1) >= 1) {
        destinopagina = 1;
        loadDestinoDetalleGrid();
    }
    else { }

    evaluateDestinoNavigationButtons(sp_total_paginas);
}

function evaluateDestinoNavigationButtons(total_paginas) {
    $('button[action="destinolistado_prev_begin"').removeAttr('disabled');
    $('button[action="destinolistado_prev"').removeAttr('disabled');
    $('button[action="destinolistado_next_end"').removeAttr('disabled');
    $('button[action="destinolistado_next"').removeAttr('disabled');

    if (destinopagina - 1 < 1) {
        $('button[action="destinolistado_prev_begin"').attr('disabled', 'disabled');
        $('button[action="destinolistado_prev"').attr('disabled', 'disabled');
    }

    if (destinopagina + 1 > total_paginas) {
        $('button[action="destinolistado_next_end"').attr('disabled', 'disabled');
        $('button[action="destinolistado_next"').attr('disabled', 'disabled');
    }
}

//LOAD TABLE FILTER (# OF ROWS)
function UpdateDestinoNumReg(num) {
    destinopagina = 1;
    destinonumreg = num;
    loadDestinoDetalleGrid();
}