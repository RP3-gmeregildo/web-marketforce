var busquedaNombre = ""
var textoAnterior = "";
var pagina = 1;
var numreg = 10;

function resize() {
    setTimeout(function () {
        resizeTable("rutastabla");
        resizeTable("rutadetalle");
    }, 100);
}

function resizeTable(id) {
    if ($("#" + id) && $("#" + id).offset()) {
        var table = window["rp3datatables_" + id];
        if ($(table).parents(".dataTables_wrapper").find(".dataTables_filter").length > 0) {
            var height = $(window).height() - $("#" + id).offset().top - 50;
            rp3DataTableSetScrollBodyHeight(id, height);
            $("#" + id + '_wrapper .dataTables_scroll').css('height', height + 30);
        }
        else {
            var height = $(window).height() - $("#" + id).offset().top - 15;
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

        $('button[action="SEND"]').click(function (e) {
            e.preventDefault();
            consultar();
        });

        $('button[action="FILTER"]').click(function (e) {
            e.preventDefault();
            showFilters(!$('#filterColumn').is(':visible'));
        });

        $("#rutastabla tr td").click(function () {

            var selectable = $(this).hasClass("selectable-item");

            if (selectable) {

                var remover = $(this).hasClass("selected-item");

                if (!remover) {
                    $("#rutastabla_wrapper tr td").removeClass("selected-item");
                    var idRuta = $(this).parent().attr("idRuta");

                    $("#rutastabla_wrapper tr[idRuta='" + idRuta + "'] td").addClass("selected-item");

                    $('#IdRuta').val(idRuta);
                }
                else {
                    $("#rutastabla_wrapper tr td").removeClass("selected-item");
                    $('#IdRuta').val('');
                }

                consultar();
            }
        });

        consultar();
    });

function consultar() {
    pagina = 1;
    busquedaNombre = ""
    textoAnterior = "";
    $("#search-table").val('');
    loadDetalleGrid();
};

function showFilters(visible) {

    var filterColumn = $('#filterColumn');
    var dataColumn = $('#dataColumn');

    var clienteColumn = $('#clienteColumn');
    var mapaColumn = $('#mapaColumn');

    var i = $('button[action="FILTER"]').children("i");

    if (!visible) {
        i.removeClass('fa fa-minus');
        i.addClass('fa fa-plus');

        filterColumn.hide();
        mapaColumn.show();

        dataColumn.removeClass('col-md-8');
        dataColumn.addClass('col-md-12');

        clienteColumn.removeClass('col-md-12');
        clienteColumn.addClass('col-md-7');
    }
    else {
        i.removeClass('fa fa-plus');
        i.addClass('fa fa-minus');

        filterColumn.show();
        mapaColumn.hide();

        dataColumn.removeClass('col-md-12');
        dataColumn.addClass('col-md-8');

        clienteColumn.removeClass('col-md-7');
        clienteColumn.addClass('col-md-12');
    }

    setTimeout(function () {
        rp3DataTableAjustsColumns('rutastabla');
        rp3DataTableAjustsColumns('rutadetalle');

        google.maps.event.trigger(googleMap, "resize");
        setCenter();
    }, 150);
}

function init_Detalle() {
    $("input[select-registro]").select2('val', numreg);
    $("#search-table").val(textoAnterior);
}

var refreshing = false;
var refreshpending = false;
//CONSULTAR
function loadDetalleGrid() {
    var d_search = busquedaNombre;

    var idRuta = String(document.getElementById('IdRuta').value);

    if (!refreshing) {
        refreshing = true;

        rp3ShowLoadingPanel('#GridDetalle');

        rp3Get("/Consulta/ClienteRuta/ConsultaRutaDetalle", {
            ruta: idRuta,
            pagina: pagina,
            numreg: numreg,
            isfilter: 0,
            buscar: d_search
        }, function (data) {
            $("#GridDetalle").html(data);
            refreshing = false;
            rp3HideLoadingPanel('#GridDetalle');

            showFilters($('#filterColumn').is(':visible'));

            $("#search-table").focus();
           
            if (refreshpending) {
                refreshpending = false;
                loadDetalleGrid();
            }

            resize(); //rp3DataTableAjustsColumns('rutastabla');
        });
    } else {
        refreshpending = true;
    }
}

var aplicarBusquedaTable;
function busquedaTableTrigger(texto) {
    if (texto != textoAnterior) {
        if (aplicarBusquedaTable) clearTimeout(aplicarBusquedaTable);
        busquedaNombre = texto;
        textoAnterior = texto;
        aplicarBusquedaTable = setTimeout(function () {
            //Load   
            pagina = 1;
            //busquedaNombre = texto;
            loadDetalleGrid();
            busquedaNombre = "";
            //textoAnterior = texto;
        }, 700);
    }
}

//NEXT AND BEFORE BUTTONS
function ListadoNextPage(sp_total_paginas) {
    if ((pagina + 1) <= sp_total_paginas) {
        pagina++;
        loadDetalleGrid();
    }
    else { }

    evaluateNavigationButtons(sp_total_paginas);
}

function ListadoNextEndPage(sp_total_paginas) {
    if ((pagina + 1) <= sp_total_paginas) {
        pagina = sp_total_paginas;
        loadDetalleGrid();
    }
    else { }

    evaluateNavigationButtons(sp_total_paginas);
}

function ListadoPreviousPage(sp_total_paginas) {
    if ((pagina - 1) >= 1) {
        pagina--;
        loadDetalleGrid();
    }
    else { }

    evaluateNavigationButtons(sp_total_paginas);
}

function ListadoPreviousBeginPage(sp_total_paginas) {
    if ((pagina - 1) >= 1) {
        pagina = 1;
        loadDetalleGrid();
    }
    else { }

    evaluateNavigationButtons(sp_total_paginas);
}

function evaluateNavigationButtons(total_paginas) {
    $('button[action="listado_prev_begin"').removeAttr('disabled');
    $('button[action="listado_prev"').removeAttr('disabled');
    $('button[action="listado_next"').removeAttr('disabled');
    $('button[action="listado_next_end"').removeAttr('disabled');

    if (pagina - 1 < 1) {
        $('button[action="listado_prev"').attr('disabled', 'disabled');
        $('button[action="listado_prev_begin"').attr('disabled', 'disabled');
    }

    if (pagina + 1 > total_paginas) {
        $('button[action="listado_next"').attr('disabled', 'disabled');
        $('button[action="listado_next_end"').attr('disabled', 'disabled');
    }
}

//LOAD TABLE FILTER (# OF ROWS)
function UpdateNumReg(num) {
    pagina = 1;
    numreg = num;
    loadDetalleGrid();
}