/*****************************************************************************************************************/
/* GLOBAL VARIABLES *********************************************************************************************/
/*****************************************************************************************************************/
var pagina = 1;
var numreg = 5;
var idlote = "";
var zona = "";
var tipocliente = "";
var canal = "";
var calificacion = 0;

var isbegin = 1;

function showDependenciaMessage() {
    rp3NotifyWarningAsBlock("El Lote actual será removido de todos las Rutas donde se encuentre asociado.", "Advertencia", true);
}

$(function () {
    
    $("#Estado").change(function () {
        var estado = $(this).val();
        var id = parseInt($("#IdLote").val());

        if (estado == "I" && id > 0) {
            showDependenciaMessage();
        }
    });

    /*******************************************/
    //Load Data
    /*******************************************/
    idlote = ConvToStr(document.getElementById("IdLote").value);

    var list_zona = document.getElementById('Zonas').value;
    var list_tipo_cliente = document.getElementById('TipoClientes').value;
    var list_canales = document.getElementById('Canales').value;
    

    fillMultiSelect("[zonas]", list_zona);
    fillMultiSelect("[tipoclientes]", list_tipo_cliente);
    fillMultiSelect("[canales]", list_canales);

    zona = String(ConvToStr($("[zonas]").val()));
    tipocliente = String(ConvToStr($("[tipoclientes]").val()));
    canal = String(ConvToStr($("[canales]").val()));



    /*******************************************/
    //Triggers 
    /*******************************************/
    
    $("[zonas]").on("change", function (e) {
        zona = String(ConvToStr($("[zonas]").val()));
        $('#Zonas').val(zona);
        idlote = null;
        pagina = 1;
        LoadGrid();
    });
    $("[tipoclientes]").on("change", function (e) {
        tipocliente = String(ConvToStr($("[tipoclientes]").val()));
        $('#TipoClientes').val(tipocliente);
        idlote = null;
        pagina = 1;
        LoadGrid();
    });
    $("[canales]").on("change", function (e) {
        canal = String(ConvToStr($("[canales]").val()));
        $("#Canales").val(canal);
        idlote = null;
        pagina = 1;
        LoadGrid();
    }); 
    $("[rate-widget] [rate-widget-step]").click(function () {
        if (!readOnly) {
            var ratevalue = $(this).attr("rate-value");
            var widget = $(this).parents("[rate-widget]");
            calificacion = ratevalue;
            //setRateValue(widget, ratevalue);
            idlote = null;
            pagina = 1;
            LoadGrid();
        }
    });
    $("[rate-widget-content] [rate-widget-cancel]").click(function (e) {
        /*e.preventDefault();
        var widget = $(this).parents("[rate-widget-content]").find("[rate-widget]");
        setRateValue(widget, 0);*/
        calificacion = 0;
        idlote = null;
        pagina = 1;
        LoadGrid();
    });
    
});

/*****************************************************************************************************************/
/* VIEW INITIATION ***********************************************************************************************/
/*****************************************************************************************************************/
function initEdit() {    
    $('button[action="UPDATE"]').click(function () {
        $('#loteForm').submit();
    });
    calificacion = document.getElementById('Calificacion').value;
    isbegin = 0;    
    LoadGrid();
}
function initCreate() {
    $('button[action="CREATE"]').click(function () {
        $('#loteForm').submit();
    });
}
function initDetail() {
    $("[zonas]").select2("enable", false);
    $("[tipoclientes]").select2("enable", false);
    $("[canales]").select2("enable", false);

    $('[zonas]').attr('placeholder', '').select2();
    $('[tipoclientes]').attr('placeholder', '').select2();
    $('[canales]').attr('placeholder', '').select2();

    LoadGrid();
}
function initDelete() {
    $('button[action="DELETE"]').click(function () {
        $('#loteForm').submit();
    });
}
function initProcess() {

}

/*****************************************************************************************************************/
/* PARTIAL VIEW INITIATION ***************************************************************************************/
/*****************************************************************************************************************/
function init_Datos() {

}
function init_LoteDetalle() {
    $("input[select-registro]").select2('val', numreg);
}

/*****************************************************************************************************************/
/* MAIN FUNCTIONS ************************************************************************************************/
/*****************************************************************************************************************/
var scrollposition;

function LoadGrid() {
    scrollposition = $(document).scrollTop();

    rp3ShowLoadingPanel('#content-detalle');

    rp3Get("/Ruta/Lote/GetLoteCliente", {
        IdLote: idlote,
        Zona: zona,
        TipoCliente: tipocliente,
        Canal: canal,
        Pagina: pagina,
        NumReg: numreg,
        isbegin: isbegin,
        calificacion: calificacion
    }, function (data) {
        if (!data.HasError) {
            $("#content-detalle").html(data);
            isbegin = 1;

            $(document).scrollTop(scrollposition);

            $("#lotedetalle").rp3DataTable();
        }
        rp3NotifyAsPopup(data.Messages);

        rp3HideLoadingPanel('#content-detalle');
    });
}

/*****************************************************************************************************************/
/* OTHER FUNCTIONS ***********************************************************************************************/
/*****************************************************************************************************************/

//NEXT AND BEFORE BUTTONS
//function ListadoNextPage(sp_total_paginas) {
//    if ((pagina + 1) <= sp_total_paginas) {
//        pagina++;
//        LoadGrid();
//    }
//    else { }

//    evaluateNavigationButtons(sp_total_paginas);
//}
//function ListadoPreviousPage(sp_total_paginas) {
//    if ((pagina - 1) >= 1) {
//        pagina--;
//        LoadGrid();
//    }
//    else { }

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
        LoadGrid();
    }
    else { }

    evaluateNavigationButtons(sp_total_paginas);
}

function ListadoNextEndPage(sp_total_paginas) {
    if ((pagina + 1) <= sp_total_paginas) {
        pagina = sp_total_paginas;
        LoadGrid();
    }
    else { }

    evaluateNavigationButtons(sp_total_paginas);
}

function ListadoPreviousPage(sp_total_paginas) {
    if ((pagina - 1) >= 1) {
        pagina--;
        LoadGrid();
    }
    else { }

    evaluateNavigationButtons(sp_total_paginas);
}

function ListadoPreviousBeginPage(sp_total_paginas) {
    if ((pagina - 1) >= 1) {
        pagina = 1;
        LoadGrid();
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

//LOAD TABLE FILTER (# OF ROWS)
function UpdateNumReg(num) {
    numreg = num;
    pagina = 1;
    LoadGrid();
}

//CONVERT TO STRING
function ConvToStr(val) {
    if (arguments.length === 0) {
        return "";
    } else if (typeof val === "undefined") {
        return "undefined";
    } else if (val === null) {
        return "";
    } else if (typeof val === "boolean") {
        return val ? "true" : "false";
    } else if (typeof val === "number") {
        // super complex rules
    } else if (typeof val === "string") {
        if(String(val) == null)
            return "";
        else
            return val
    } else {
        return val;
    }
}