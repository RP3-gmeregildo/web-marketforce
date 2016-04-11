var pagina = 1;
var numreg = 3;

var idlote = "";
var zona = "";
var tipocliente = "";
var canal = "";

function initEdit() {

    initDatos();
    initGrid();
    initMap();
}

function initDatos() {

    $("[zonas]").on("change", function (e) {
        zona = String(ConvToStr($("[zonas]").val()));
        $('#Zonas').val(zona);
        idlote = null;
        LoadGrid();
    });
    $("[tipoclientes]").on("change", function (e) {
        tipocliente = String(ConvToStr($("[tipoclientes]").val()));
        $('#TipoClientes').val(tipocliente);
        idlote = null;
        LoadGrid();
    });
    $("[canales]").on("change", function (e) {
        canal = String(ConvToStr($("[canales]").val()));
        $("#Canales").val(canal);
        idlote = null;
        LoadGrid();
    });

}
function initGrid() {

}
function initMap() {

}

function LoadGrid() {
    rp3Get("/Ruta/Lote/GetLoteCliente", {
        IdLote: null,
        Zona: zona,
        TipoCliente: tipocliente,
        Canal: canal,
        Pagina: pagina,
        NumReg: numreg
    }, function (data) {
        if (!data.HasError) {
            $("#content-detalle").html(data);
        }
        rp3NotifyAsPopup(data.Messages);
    });
}
function ListadoNextPage(sp_total_paginas) {
    if ((pagina + 1) <= sp_total_paginas) {
        pagina++;
        LoadGrid();
    }
    else { }
}
function ListadoPreviousPage(sp_total_paginas) {
    if ((pagina - 1) >= 1) {
        pagina--;
        LoadGrid();
    }
    else { }
}
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
        if (String(val) == null)
            return "";
        else
            return val
    } else {
        return val;
    }
}