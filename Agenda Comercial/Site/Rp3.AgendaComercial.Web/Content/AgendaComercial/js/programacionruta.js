var MESSAGE_SIN_INICIO;
var MESSAGE_SIN_FIN;
var MESSAGE_FIN_INICIO;
var MESSAGE_SIN_DIAS;

var busquedaNombre = "";
var ruta;
var pagina = 1;
var numreg = 5;
var textoAnterior = "";

$(function () {

    ruta = document.getElementById('IdRuta').value;

    $(window).resize(function () {      
        rp3DataTableAjustsColumns();      
    });

    $("[action='process']").click(function () {

        rp3DialogConfirmationMessage("Programación", "¿Está seguro que desea procesar la Ruta actual?", function (data) {

            rp3ShowLoadingPanel('#rowBody');

            $("[action='process']").attr('disabled', 'disabled');

            var idRuta = parseInt($('#IdRuta').val());

            rp3Post("/Ruta/Ruta/RutaProcess", { IdRuta: idRuta }, function (data) {

                rp3NotifyAsPopup(data.Messages);
                rp3HideLoadingPanel('#rowBody');

                $("[action='process']").removeAttr('disabled');
            });
        });
    });

    $("#saveProgramacion").click(function (e) {
        e.preventDefault();
        saveProgramacion();
    });

    $('#canal').change(function () {
        loadDetalleGrid();
    });
    $('#tipocliente').change(function () {
        loadDetalleGrid();
    });
    $('#lote').change(function () {
        loadDetalleGrid();
    });

    loadDetalleGrid();
});

function init_Detalle() {
    $("input[select-registro]").select2('val', numreg);
    $("#search-table").val(textoAnterior);
}

var refreshing = false;
var refreshpending = false;
var isSearchFocus = false;

function loadDetalleGrid() {
    var d_search = busquedaNombre;

    isSearchFocus = $("#search-table").is(":focus")
    var canal = $("#canal").val();
    var tipocliente = $("#tipocliente").val();
    var lote = $("#lote").val();

    if (!refreshing) {
        refreshing = true;

        rp3ShowLoadingPanel('#GridDetalle');

        rp3Get("/Ruta/Ruta/ConsultaRutaScheduleDetalle", {
            ruta: ruta,
            pagina: pagina,
            numreg: numreg,
            buscar: d_search,
            idcanal: canal,
            idtipocliente: tipocliente,
            idlote: lote
        }, function (data) {
            $("#GridDetalle").html(data);
            refreshing = false;
            rp3HideLoadingPanel('#GridDetalle');

            if (isSearchFocus)
                $("#search-table").focus();

            if (refreshpending) {
                refreshpending = false;
                loadDetalleGrid();
            }
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
            pagina = 1;
            loadDetalleGrid();
            busquedaNombre = "";
        }, 700);
    }
}

function ListadoNextPage(sp_total_paginas) {
    if ((pagina + 1) <= sp_total_paginas) {
        pagina++;
        loadDetalleGrid();
    }

    evaluateNavigationButtons(sp_total_paginas);
}
function ListadoPreviousPage(sp_total_paginas) {
    if ((pagina - 1) >= 1) {
        pagina--;
        loadDetalleGrid();
    }
    evaluateNavigationButtons(sp_total_paginas);
}

function evaluateNavigationButtons(total_paginas) {
    $("#listado_prev").removeAttr('disabled');
    $("#listado_next").removeAttr('disabled');

    if (pagina - 1 < 1) {
        $("#listado_prev").attr('disabled', 'disabled');
    }

    if (pagina + 1 > total_paginas) {
        $("#listado_next").attr('disabled', 'disabled');
    }
}

function UpdateNumReg(num) {
    pagina = 1;
    numreg = num;
    loadDetalleGrid();
}

function evaluateDisable(checked) {
    $('#FechaFinEdit').removeAttr('disabled');

    if (checked) {
        $('#FechaFinEdit').attr('disabled', true);

        $('#FechaFinEdit').val('');
    }
}

function getProgramacion(control) {
    var id = $(control).attr('idProgramacionRuta');
    var idRuta = parseInt($('#IdRuta').val());//$(control).parents("[idruta]").attr('idruta');;
    var idCliente = $(control).parents("[idcliente]").attr('idcliente');;
    var idClienteDireccion = $(control).parents("[idclientedireccion]").attr('idclientedireccion');

    rp3Get("/Ruta/Ruta/GetProgramacion", { id: id, idCliente : idCliente, idClienteDireccion: idClienteDireccion, idRuta : idRuta }, function (data) {
        $("#setProgramacionDialogContent").html(data);        
        $("#form-programacion select").select2();
        $("#form-programacion .rp3DateInput").each(function (i, val) {
            $(val).rp3DatePicker();
        });
        $("#form-programacion input:radio, #form-programacion input:checkbox").rp3CheckBox();

        $("#ConFechaFin").on('ifChecked || ifUnchecked', function () { setFechaFin(this) });
        $("#TipoMensual1").on('ifChecked || ifUnchecked', function () { showMensualDia(this) });
        $("#TipoMensual2").on('ifChecked || ifUnchecked', function () { showMensualDia(this) });

        if ($("#ConFechaFin").is(':checked'))
            $("#FechaFinEdit").removeAttr('disabled');
        else
            $("#FechaFinEdit").attr('disabled', 'disabled');

        if ($("#TipoMensual1").is(':checked'))
            showMensualDia($("#TipoMensual1"));
        else
            showMensualDia($("#TipoMensual2"));
        rp3ModalShow("setProgramacionDialog");
    });
}

function showMensualDia(control)
{
    if ($(control).val() == 2)
    {
        $("#mensualDivDia #DiaMes").attr('disabled', 'disabled');
        $("#mensualDivDia #Frecuencia").attr('disabled', 'disabled');
        $("#mensualDivEl #Semana").removeAttr('disabled');
        $("#mensualDivEl #diaString").removeAttr('disabled');
        $("#mensualDivEl #Frecuencia").removeAttr('disabled');
    }
    else {
        $("#mensualDivEl #Semana").attr('disabled', 'disabled');
        $("#mensualDivEl #diaString").attr('disabled', 'disabled');
        $("#mensualDivEl #Frecuencia").attr('disabled', 'disabled');
        $("#mensualDivDia #DiaMes").removeAttr('disabled');
        $("#mensualDivDia #Frecuencia").removeAttr('disabled');
    }
}

function setFechaFin(control)
{
    if ($(control).is(':checked'))
        $("#FechaFinEdit").removeAttr('disabled');
    else
        $("#FechaFinEdit").attr('disabled', 'disabled');
}

function changeOcurrency(control) {
    var id = $(control).val();
    if (id == 'S') {
        $("#diarioDiv").css('display', 'none');
        $("#semanalDivCol").css('display', 'block');
        $("#semanalDivRow").css('display', 'block');
        $("#mensualDiv").css('display', 'none');
    }
    if (id == 'D') {
        $("#diarioDiv").css('display', 'block');
        $("#semanalDivCol").css('display', 'none');
        $("#semanalDivRow").css('display', 'none');
        $("#mensualDiv").css('display', 'none');
    }
    if (id == 'M') {
        $("#diarioDiv").css('display', 'none');
        $("#semanalDivCol").css('display', 'none');
        $("#semanalDivRow").css('display', 'none');
        $("#mensualDiv").css('display', 'block');
    }
}
function saveProgramacion()
{
    var idprogramacionRuta = $("#IdProgramacionRuta").val();
    var idRuta = $("#IdRuta").val();
    var idCliente = $("#IdCliente").val();
    var idClienteDireccion = $("#IdClienteDireccion").val();
    var patron = $("#Patron").val();
    var fechaInicioTicks = $("#FechaInicioTicks").val();
    var fechaFinTicks = $("#FechaFinTicks").val();
    var duracion = $("#DuracionVisita").val();
    var frecuencia = null;
    var diaMes = null;
    var lunes = false;
    var martes = false;
    var miercoles = false;
    var jueves = false;
    var viernes = false;
    var sabado = false;
    var domingo = false;
    var cldia = false;
    var dialaboral = false;
    var findesemana = false;
    var tipoMensual = null;
    var semana = null;
    var tareas = [];

    var tareasSelected = $("#tareasSelect").val();
    for (var i = 0, l = tareasSelected.length; i < l; i++) {
        tareas.push({ IdProgramacionRuta: idprogramacionRuta, IdTarea: tareasSelected[i] });
    }

    if (fechaInicioTicks == 0)
    {
        rp3NotifyErrorAsBlock(MESSAGE_SIN_INICIO, "", true, $("#block_error"));
        return;
    }

    if (!$("#ConFechaFin").is(':checked'))
    {
        fechaFinTicks = 0;
    }
    else
    {
        if(fechaFinTicks == 0)
        {
            rp3NotifyErrorAsBlock(MESSAGE_SIN_FIN, "", true, $("#block_error"));
            return;
        }
        if(fechaInicioTicks > fechaFinTicks)
        {
            rp3NotifyErrorAsBlock(MESSAGE_FIN_INICIO, "", true, $("#block_error"));
            return;
        }
    }
    if (patron == 'D')
    {
        frecuencia = $("#diarioDiv #Frecuencia").val();
    }
    if (patron == 'S') {
        frecuencia = $("#semanalDivCol #Frecuencia").val();
        var semanaDias = $("#diasSemanaSelect").val();
        if (semanaDias == null || semanaDias.length <= 0)
        {
            rp3NotifyErrorAsBlock(MESSAGE_SIN_DIAS, "", true, $("#block_error"));
            return;
        }
        for (var i = 0, l = semanaDias.length; i < l; i++) {
            switch (semanaDias[i]) {
                case "1": lunes = true; break;
                case "2": martes = true; break;
                case "3": miercoles = true; break;
                case "4": jueves = true; break;
                case "5": viernes = true; break;
                case "6": sabado = true; break;
                case "7": domingo = true; break;
            }
        }
    }
    if (patron == 'M') {
        if($("#TipoMensual1").is(":checked"))
        {
            tipoMensual = "1";
            diaMes = $("#mensualDivDia #DiaMes").val();
            frecuencia = $("#mensualDivDia #Frecuencia").val();
        }
        else
        {
            tipoMensual = "2";
            semana = $("#mensualDivEl #Semana").val();
            frecuencia = $("#mensualDivEl #Frecuencia").val();
            var dia = $("#diaString").val();
            switch (dia) {
                case "1": lunes = true; break;
                case "2": martes = true; break;
                case "3": miercoles = true; break;
                case "4": jueves = true; break;
                case "5": viernes = true; break;
                case "6": sabado = true; break;
                case "7": domingo = true; break;
                case "8": cldia = true; break;
                case "9": dialaboral = true; break;
                case "10": findesemana = true; break;
            }
        }
    }
    rp3JSONDataPost("/Ruta/Ruta/SaveProgramacion", {
        IdRuta: idRuta, IdProgramacionRuta: idprogramacionRuta, IdCliente: idCliente, IdClienteDireccion: idClienteDireccion,
        Patron: patron, FechaInicioTicks: fechaInicioTicks, FechaFinTicks: fechaFinTicks, Frecuencia: frecuencia,
        Lunes: lunes, Martes: martes, Miercoles: miercoles, Jueves: jueves, Viernes: viernes, Sabado: sabado, Domingo: domingo,
        Dia: cldia, DiaLaboral: dialaboral, DiaFinDeSemana: findesemana, DiaMes: diaMes, TipoMensual: tipoMensual,
        DuracionVisita: duracion, Semana: semana, ProgramacionRutaTareas: tareas}, function (data) {
            rp3ModalHide("setProgramacionDialog");
            $("#" + idCliente + "_" + idClienteDireccion).empty();
            $("#" + idCliente + "_" + idClienteDireccion).html(data);

            changed = true;
    });
}

function deleteProgramacion(control)
{
    var id = $(control).attr('idProgramacionRuta');
    var idRuta = parseInt($('#IdRuta').val()); //var idRuta = $(control).parents("[idruta]").attr('idruta');;
    var idCliente = $(control).parents("[idcliente]").attr('idcliente');;
    var idClienteDireccion = $(control).parents("[idclientedireccion]").attr('idclientedireccion');;
    rp3DialogConfirmationMessage("Desea borrar esta programación?", "Borrar", function () {
        rp3JSONDataPost("/Ruta/Ruta/DeleteProgramacion", { id: id }, function (data) {
            $("#" + idCliente + "_" + idClienteDireccion).empty();
            $("#" + idCliente + "_" + idClienteDireccion).html(data);

            changed = true;
            rp3DialogConfirmationMessage("Desea eliminar las agendas sin gestionar de esta programación?", "Borrar Agendas", function () {
                rp3JSONDataPost("/Ruta/Ruta/DeleteProgramacionAgendas", { id: id }, function (data) {
                });
            }, null);
        });
    }, null);
}