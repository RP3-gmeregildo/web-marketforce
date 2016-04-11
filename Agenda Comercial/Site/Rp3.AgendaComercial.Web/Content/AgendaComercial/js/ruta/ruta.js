/*****************************************************************************************************************/
/* GLOBAL VARIABLES **********************************************************************************************/
/*****************************************************************************************************************/
var busquedaNombre = "";
var ruta = 0;
var estado = "";
var lote = "";

var pagina = 1;
var numreg = 10;

var textoAnterior = "";

function showDependenciaMessage() {
    rp3NotifyWarningAsBlock("La Ruta actual será removida de todos los Agentes donde se encuentre asociada y se eliminarán todas las Citas programadas.", "Advertencia", true);
}

$(function () {

    $("#Estado").change(function () {
        var estado = $(this).val();
        var id = parseInt($("#IdRuta").val());

        if (estado == "I" && id > 0) {
            showDependenciaMessage();
        }
    });

    /*******************************************/
    //Load Data
    /*******************************************/
    //Ruta
    ruta = String(document.getElementById('IdRuta').value);
    //Estado
    estado = $("#Estado").select2('val');
    //Lote
    var list_lote = String(document.getElementById('Lotes').value);
    fillMultiSelect("[lotes]", list_lote);

    /*******************************************/
    //Triggers 
    /*******************************************/
    //OnClick
    $("[lotes]").on("change", function (e) {
        pagina = 1;
        loadDetalleGrid(1);
    });

    $("#Estado").on("change", function (e) {
        estado = $("#Estado").select2('val');
    });

    /*******************************************/
    //INCLUIR
    /*******************************************/
    $("#rutaIncluirSearch").autocomplete({
        source: RP3_ROOT_PATH + '/General/Cliente/ClienteAutocomplete',
        minLength: 1,
        open: function () { $('.ui-menu').width(700) },
        select: function (event, ui) {
            clienteAdd("#rutaincluir tbody", "Incluir", ui.item.idCliente, ui.item.idClienteDireccion, ui.item.cliente, ui.item.direccion, ui.item.latitud, ui.item.longitud, ui.item.markerIndex, ui.item.markerstart, ui.item.markerzindex);
            pagina = 1;
            loadDetalleGrid(1);
            $("#rutaIncluirSearch").val('');
            retrieveTotalContent("rutaincluir", "#tot_incluir");
            event.preventDefault();
        }
    });

    $("#rutaIncluirSearch").keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });    

    /*******************************************/
    //EXCLUIR
    /*******************************************/
    $("#rutaExcluirSearch").autocomplete({
        source: RP3_ROOT_PATH + '/General/Cliente/ClienteAutocomplete',
        minLength: 1,
        open: function () { $('.ui-menu').width(700) },
        select: function (event, ui) {
            clienteAdd("#rutaexcluir tbody", "Excluir", ui.item.idCliente, ui.item.idClienteDireccion, ui.item.cliente, ui.item.direccion, ui.item.latitud, ui.item.longitud, ui.item.markerIndex, ui.item.markerstart, ui.item.markerzindex);
            pagina = 1;
            loadDetalleGrid(1);
            $("#rutaExcluirSearch").val('');
            retrieveTotalContent("rutaexcluir", "#tot_excluir");
            event.preventDefault();
        }
    });

    $("#rutaExcluirSearch").keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });
});

/*****************************************************************************************************************/
/* VIEW INITIATION ***********************************************************************************************/
/*****************************************************************************************************************/
function initEdit() {
    //(Remove Button)
    triggerIncluirRemoveButton();
    triggerExcluirRemoveButton();
    loadDetalleGrid(0);


    $('button[action="UPDATE"]').click(function () {
        if (ruta != 0 && ruta != null || ruta == '') {
            var d_lote = String($("[lotes]").val());
            var d_excluir = String(retrieveInputContent('rutaexcluir'));
            var d_incluir = String(retrieveInputContent('rutaincluir'));

            var titulo = ""
            var mensaje = "Si continua, las visitas que aun no se cumplan y que estén asociadas ";
            mensaje += "a clientes que no formen parte de esta ruta serán Eliminadas";
            mensaje += "\n¿Desea Continuar?";

            
            rp3DialogConfirmationMessage(mensaje, titulo, function (data) {

                var descripcion = $("#Descripcion").val();
                var idCalendario = $("#IdCalendario").val();

                    rp3Post("/Ruta/Ruta/GrabarRutaDetalle", {
                        ruta: ruta,
                        descripcion: descripcion,
                        idCalendario: idCalendario,
                        estado: estado,
                        lote: d_lote,
                        excluir: d_excluir,
                        incluir: d_incluir
                    }, function (data) {
                        if (!data.HasError) {
                            window.location.href = RP3_ROOT_PATH + "/Ruta/Ruta/Index";
                        }
                        rp3NotifyAsPopup(data.Messages);
                    });

            }, function (data) {

            });
            
        }
    });
}
function initCreate() {
    //(Remove Button)
    triggerIncluirRemoveButton();
    triggerExcluirRemoveButton();

    $('button[action="CREATE"]').click(function () {
        $('#rutaForm').submit();
    });
}
function initDetail() {
    $("[lotes]").select2("enable", false);

    $('[lotes]').attr('placeholder', '').select2();
}
function initDelete() {
    $('button[action="DELETE"]').click(function () {
        $('#rutaForm').submit();
    });
}
function initProcess() {

}

/*****************************************************************************************************************/
/* PARTIAL VIEW INITIATION ***************************************************************************************/
/*****************************************************************************************************************/
function init_Datos() {
    
}
function init_Excluir() {

}
function init_Incluir() {

}
function init_Detalle() {
    $("input[select-registro]").select2('val', numreg);
    $("#search-table").val(textoAnterior);
}



/*****************************************************************************************************************/
/* MAIN FUNCTIONS ************************************************************************************************/
/*****************************************************************************************************************/


var refreshing = false;
var refreshpending = false;

var isSearchFocus = false;
//CONSULTAR
function loadDetalleGrid(isfilter) {
    var d_search = busquedaNombre;
    var d_lote = String($("[lotes]").val());
    var d_excluir = String(retrieveInputContent('rutaexcluir'));
    var d_incluir = String(retrieveInputContent('rutaincluir'));

    isSearchFocus = $("#search-table").is(":focus")

    if ((ruta > 0) || isfilter == 1) {
        if (!refreshing) {
            refreshing = true;

            rp3ShowLoadingPanel('#GridDetalle');

            rp3Get("/Ruta/Ruta/ConsultaRutaDetalle", {
                ruta: ruta,
                lote: d_lote,
                excluir: d_excluir,
                incluir: d_incluir,
                pagina: pagina,
                numreg: numreg,
                isfilter: isfilter,
                buscar: d_search
            }, function (data) {
                $("#GridDetalle").html(data);
                refreshing = false;
                rp3HideLoadingPanel('#GridDetalle');

                if (isSearchFocus)
                    $("#search-table").focus();

                if (refreshpending) {
                    refreshpending = false;
                    loadDetalleGrid(isfilter);
                }

            });
        } else {
            refreshpending = true;
        }
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
            loadDetalleGrid(1);
            busquedaNombre = "";
            //textoAnterior = texto;
        }, 700);
    }
}


/*****************************************************************************************************************/
/* OTHER FUNCTIONS ***********************************************************************************************/
/*****************************************************************************************************************/


//REMOVE BUTTON
function triggerExcluirRemoveButton() {
    $('[rutaExcluirRemoveButton] button').unbind("click");
    $('[rutaExcluirRemoveButton] button').click(function (e) {
        var row = $(this).parents('tr');
        row.remove();
        verifyExistItems("#rutaexcluir tbody");
        e.preventDefault();
        refreshMarkerIndex("#rutaexcluir tbody");
        loadDetalleGrid(1);
        refreshMapClient();
        retrieveTotalContent("rutaexcluir", "#tot_excluir");
    });
    retrieveTotalContent("rutaexcluir", "#tot_excluir");
};

function triggerIncluirRemoveButton() {
    $('[rutaIncluirRemoveButton] button').unbind("click");
    $('[rutaIncluirRemoveButton] button').click(function (e) {
        var row = $(this).parents('tr');
        row.remove();
        verifyExistItems("#rutaincluir tbody");
        e.preventDefault();
        refreshMarkerIndex("#rutaincluir tbody");
        loadDetalleGrid(1);
        refreshMapClient();
        retrieveTotalContent("rutaincluir", "#tot_incluir");
    });

    retrieveTotalContent("rutaincluir", "#tot_incluir");
};

//FILL MULTISELECT
function fillMultiSelect(obj, value) {
    var str = value;
    if (typeof str === "string" || str === null) {
        if (str != null && str != "") {
            var selectedValues = str.split("-");
            if (typeof selectedValues !== 'undefined' && selectedValues.length > 0) {
                $.each($(obj), function () {
                    $(this).select2('val', selectedValues);
                });
            }
        }
    }
    else {
        if (typeof value !== 'undefined' && value.length > 0) {
            $.each($(obj), function () {
                $(this).select2('val', value);
            });
        }
    }
}

//RETRIEVE DATA FROM TABLE
function retrieveInputContent(tableID) {    
    var data = Array();
    $("#" + tableID + " tr input").each(function (i, v) {
        data[i] = Array();
        data[i] = $(this).val();
    })
    return data;
}
function retrieveTotalContent(tableID, fieldname) {
    var datacount = 0;
    $("#" + tableID + " tr input").each(function (i, v) {        
        datacount++;
    })
    $(fieldname).text(datacount);
}

//NEXT AND BEFORE BUTTONS
//function ListadoNextPage(sp_total_paginas) {
//    if ((pagina + 1) <= sp_total_paginas) {
//        pagina++;
//        loadDetalleGrid(1);
//    }

//    evaluateNavigationButtons(sp_total_paginas);
//}
//function ListadoPreviousPage(sp_total_paginas) {
//    if ((pagina - 1) >= 1) {
//        pagina--;
//        loadDetalleGrid(1);
//    }

//    evaluateNavigationButtons(sp_total_paginas);
//}

//function evaluateNavigationButtons(total_paginas) {
//    $("#listado_prev").removeAttr('disabled');
//    $("#listado_next").removeAttr('disabled');

//    if (pagina - 1 < 1) {
//        $("#listado_prev").attr('disabled', 'disabled');
//    }

//    if (pagina + 1 > total_paginas) {
//        $("#listado_next").attr('disabled', 'disabled');
//    }
//}

function ListadoNextPage(sp_total_paginas) {
    if ((pagina + 1) <= sp_total_paginas) {
        pagina++;
        loadDetalleGrid(1);
    }
    else { }

    evaluateNavigationButtons(sp_total_paginas);
}

function ListadoNextEndPage(sp_total_paginas) {
    if ((pagina + 1) <= sp_total_paginas) {
        pagina = sp_total_paginas;
        loadDetalleGrid(1);
    }
    else { }

    evaluateNavigationButtons(sp_total_paginas);
}

function ListadoPreviousPage(sp_total_paginas) {
    if ((pagina - 1) >= 1) {
        pagina--;
        loadDetalleGrid(1);
    }
    else { }

    evaluateNavigationButtons(sp_total_paginas);
}

function ListadoPreviousBeginPage(sp_total_paginas) {
    if ((pagina - 1) >= 1) {
        pagina = 1;
        loadDetalleGrid(1);
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
    loadDetalleGrid(1);
}

//VALIDATE IF TABLE EXIST
function verifyExistItems(obj) {
    var table = $(obj);

    var rowCount = $(table).children('tr').children('[ubicacion]input').length;

    if (rowCount == 0) {
        $(table).append('<tr class="odd"><td valign="top" colspan="4" class="dataTables_empty">Ningún dato disponible en esta tabla</td></tr>');
    }
}

function refreshDetalle() {
    pagina = 1;
    busquedaNombre = "";
    loadDetalleGrid(1);
}

//REFREZCAR TABLA
function refreshMarkerIndex(obj) {
    var markerIndex = 1;

    $(obj).children('tr').each(function () {
        var input = $(this).children('[ubicacion]input');

        if (input) {
            $(input).attr('markerindex', markerIndex);
            $(input).attr('markerstart', 30 * (markerIndex - 1));
            $(input).attr('markerzindex', 1000 - markerIndex);
        }

        var label = $(this).children('td').children('#icon_location');

        if (label)
            label.html('<strong>' + markerIndex + '</strong>')

        markerIndex++;
    });

}
function refreshMapClient() {
    rp3Get("/Ruta/Ruta/UbicacionMapMarkerClient", null, function (data) {
        $("#content_mapClient").html(data);
    });
}

//ADD CLIENTE
function clienteAdd(obj, objname, idCliente, idClienteDireccion, cliente, direccion, latitud, longitud, markerIndex, markerstart, markerzindex) {
    var table = $(obj);
    var countExist = $(table).children().children('[value="' + idCliente + "-" + idClienteDireccion + '"]input').length;
    var empty = $(obj).children('tr').children('[class="dataTables_empty"]td');

    if (empty) {
        var row = $(empty).parents('tr');
        row.remove();
    }

    if (countExist == 0) {

        var index = $(table).prop('rows').length;

        $(table).append('<tr class="odd">' +
        '<input  name="cliente' + objname + '" type="hidden" value="' + idCliente + '-' + idClienteDireccion + '" ubicacion markerindex="' + markerIndex + '" markerstart="' + markerstart + '" markerzindex="' + markerzindex + '" latitud="' + latitud + '" longitud="' + longitud + '" titulo="' + cliente + '">' +
        '<td ruta' + objname + 'RemoveButton="" class="text-center " style="width:50px">' +
        '<button class="btn-xs btn btn-primary"><i class="fa fa-times"/></button></td>' +
        '<td class=" ">' + cliente + ' </td>' +
        '<td class=" ">' + direccion + ' </td>' +
        '</tr>');

        if (objname == "Incluir") {
            triggerIncluirRemoveButton();
        }
        else {
            triggerExcluirRemoveButton();
        }

        refreshMarkerIndex(obj);
        refreshMapClient();
        rp3DataTableAjustsColumns("#ruta" + objname + "");
    }
}
