/*****************************************************************************************************************/
/* GLOBAL VARIABLES **********************************************************************************************/
/*****************************************************************************************************************/
var d_tarea = 0;
var d_tipo = 0;
var d_estado = "";

var mdl_idtarea = 0;
var mdl_idtareaactividad = 0;

var isNewItemUsed = false;
var numberNewItems = 0;
var txtNuevo = "Nuevo Registro";

var mdl_index = 0;
var mdl_idorden = 0;
var mdl_opcion = "";

function rp3MaxAttr(selector, attr, defaultValue) {
    var idArray = $(selector).map(function () {
        return $(this).attr(attr);
    }).get();

    var newId = defaultValue;
    if (idArray.length > 0)
        newId = Math.max.apply(Math, idArray);

    return newId;
}

function showDependenciaMessage() {
    rp3NotifyWarningAsBlock("La Tarea actual será removida de todos las Citas programadas.", "Advertencia", true);
}

$(function () {

    $('.tree').treegrid();

    $("[checkreadOnly]").iCheck('disable');

    $("input[name='nuevocheckall']").on('ifChecked || ifUnchecked', function () {
        var checkall = this.checked;
        $("input[name='nuevocampo'][allowcheck]").each(function () {
            if (checkall) {
                $(this).iCheck('check');
            }
            else {
                $(this).iCheck('uncheck');
            }
        });
    });

    $("input[name='existentecheckall']").on('ifChecked || ifUnchecked', function () {
        var checkall = this.checked;
        $("input[name='existentecampo'][allowcheck]").each(function () {
            if (checkall) {
                $(this).iCheck('check');
            }
            else {
                $(this).iCheck('uncheck');
            }
        });
    });

    $("input[name='gestioncheckall']").on('ifChecked || ifUnchecked', function () {
        var checkall = this.checked;
        $("input[name='gestioncampo'][allowcheck]").each(function () {
            if (checkall) {
                $(this).iCheck('check');
            }
            else {
                $(this).iCheck('uncheck');
            }
        });
    });

    $("#PermitirCreacion").on('ifChecked || ifUnchecked', function () {
        creacionEnable(this.checked);
    });

    $("#PermitirCreacion").each(function () {
        creacionEnable(this.checked);
    });

    function creacionEnable(checked) {
        if (!checked) {
            $('[name="nuevocheckall"]').attr('disabled', true);
            $('[name="nuevocheckall"]').iCheck('uncheck');

            $('[name="nuevocampo"][allowcheck]').each(function () {
                $(this).attr('disabled', true);
                $(this).iCheck('uncheck');
            });
        }
        else {
            $('[name="nuevocheckall"]').removeAttr('disabled');

            $('[name="nuevocampo"][allowcheck]').each(function () {
                $(this).removeAttr('disabled');
            });
        }
    }

    $("#PermitirModificacion").on('ifChecked || ifUnchecked', function () {
        modificacionEnable(this.checked);
    });

    $("#PermitirModificacion").each(function () {
        modificacionEnable(this.checked);
    });

    function modificacionEnable(checked) {
        if (!checked) {
            $('[name="existentecheckall"]').attr('disabled', true);
            $('[name="existentecheckall"]').iCheck('uncheck');

            $('[name="existentecampo"][allowcheck]').each(function () {
                $(this).attr('disabled', true);
                $(this).iCheck('uncheck');
            });
        }
        else {
            $('[name="existentecheckall"]').removeAttr('disabled');

            $('[name="existentecampo"][allowcheck]').each(function () {
                $(this).removeAttr('disabled');
            });
        }
    }

    $("#SiempreEditarEnGestion").on('ifChecked || ifUnchecked', function () {
        if(this.checked)
            $("#SoloFaltantesEditarEnGestion").iCheck('uncheck');

        gestionEnable();
    });

    $("#SoloFaltantesEditarEnGestion").on('ifChecked || ifUnchecked', function () {
        if (this.checked)
            $("#SiempreEditarEnGestion").iCheck('uncheck');

        gestionEnable();
    });

    gestionEnable();

    function gestionEnable() {
        var checked = false;

        checked = $("#SiempreEditarEnGestion").is(":checked");

        if (!checked)
            checked = $("#SoloFaltantesEditarEnGestion").is(":checked");

        if (!checked) {
            $('[name="gestioncheckall"]').attr('disabled', true);
            $('[name="gestioncheckall"]').iCheck('uncheck');

            $('[name="gestioncampo"][allowcheck]').each(function () {
                $(this).attr('disabled', true);
                $(this).iCheck('uncheck');
            });
        }
        else {
            $('[name="gestioncheckall"]').removeAttr('disabled');

            $('[name="gestioncampo"][allowcheck]').each(function () {
                $(this).removeAttr('disabled');
            });
        }
    }


    $("#Estado").change(function () {
        var estado = $(this).val();
        var id = parseInt($("#IdTarea").val());

        if (estado == "I" && id > 0) {
            showDependenciaMessage();
        }
    });

    $("input[name='rutacheckall']").on('ifChecked || ifUnchecked', function () {

        var checkall = this.checked;

        $("input[name='rutachk'][allowcheck]").each(function () {
            if (checkall) {
                $(this).iCheck('check');
            }
            else {
                $(this).iCheck('uncheck');
            }
        });
    });

    $("#AplicaTodasLasRutas").each(function () {
        rutaDisable(this.checked);
    });

    $("#AplicaTodasLasRutas").on('ifChecked || ifUnchecked', function () {
        rutaDisable(this.checked);
    });

    function rutaDisable(checked) {
        if (checked) {
            $('[name="rutacheckall"]').attr('disabled', true);
            $('[name="rutacheckall"]').iCheck('uncheck');

            $('[name="rutachk"][allowcheck]').each(function () {
                $(this).attr('disabled', true);
                $(this).iCheck('uncheck');
            });
        }
        else {
            $('[name="rutacheckall"]').removeAttr('disabled');

            $('[name="rutachk"][allowcheck]').each(function () {
                $(this).removeAttr('disabled');
            });
        }
    }

    $("#EsVigenciaIndefinida").each(function () {
        evaluateDisable(this.checked);
    });

    $("#EsVigenciaIndefinida").on('ifChecked || ifUnchecked', function () {
        evaluateDisable(this.checked);
    });

    function evaluateDisable(checked) {
        $('#FechaVigenciaHastaEdit').removeAttr('disabled');

        if (checked) {
            $('#FechaVigenciaHastaEdit').attr('disabled', true);

            $('#FechaVigenciaHastaEdit').val('');
        }
    }

    $('[data-toggle="tab"][href="#tabruta"]').click(function () {
        setTimeout(function () {
            rp3DataTableAjustsColumns("#tabruta");
        }, 100);
    });

    $("#tree-actividad").nestable({
        maxDepth: 2
    });

    $("#TipoActividad").on("change", function (e) {
        $("#idopts li").remove();
        loadOpciones($(this).val());
    });

    $("#TipoTarea").on("change", function (e) {
        $("#tree-actividad li").each(function (i, v) {
            v.remove();
        });

        evaluateTipoTarea();
    });

    evaluateTipoTarea();

    function evaluateTipoTarea() {
        if ($("#TipoTarea").val() == "E" || $("#TipoTarea").val() == "CO") {
            $('[href="#tabactividad"]').closest('li').show();
        } else {
            $('[href="#tabactividad"]').closest('li').hide();
        }
    }

    $("button[action='nueva-opcion']").click(function (e) {
        setOpcionDialog();
        e.preventDefault();
    });

    $("button[action='nueva-actividad']").click(function () {
        var newId = rp3MaxAttr('#tree-actividad li', 'data-id', 1) + 1;

        var numdefault = 0;
        var textdefault = '';
        var newTr = "";

        newTr += '<li class="dd-item dd3-item selectable-item" data-opt="parent" data-padre="0" data-id="' + newId.toString() + '" data-idtarea="' + numdefault + '" data-id-actividad="' + numdefault + '"';
        newTr += ' data-descripcion="' + textdefault + '" data-idtipo="' + numdefault + '" data-tipo="' + textdefault + '" data-opciones="' + textdefault + '" data-idorden="' + numdefault + '" data-valor="' + numdefault + '" data-numero="' + numdefault + '">';
        newTr += '<div class="dd-handle dd3-handle"></div>';
        newTr += '<div class="dd3-content">';
        newTr += '<div class="row"><div class="col-md-6"> <div onclick="loadModalDatos(' + newId.toString() + ')"><span class="tarea-orden" orden="1" ordenli></span>';
        newTr += '<span class="truncate-text" descripcion>' + txtNuevo + '</span></div>';
        newTr += '</div><div class="col-md-2"> <div class="column2" tipoactividad onclick="loadModalDatos(' + newId.toString() + ')"></div>';
        newTr += '</div><div class="col-md-2"> <div class="truncate-text column3" onclick="loadModalDatos(' + newId.toString() + ')" opciones></div>';
        newTr += '</div><div class="col-md-1"> <div class="column4">';
        newTr += '<button class="btn-xs btn btn-danger" onclick="removeActividad(' + newId.toString() + ')" remove-actividad=""><i class="fa fa-times"></i> </button>';
        newTr += '</div>';
        newTr += '</div></div></div>';
        newTr += '</li>';

        $("#tree-actividad > ol").append(newTr);
        $('#tree-content').scrollTop($('#tree-content')[0].scrollHeight);

        SetOrderTable();

    });

    /*************************************************************************/
    //SELECT2 WORKAROUND (PATCH)
    $("div.md-overlay").on('click', function () {
        $("#TipoActividad").select2("close");
    });
    $("div.modal-body").on('click', function () {
        $("#TipoActividad").select2("close");
    });
    $("button").on('click', function () {
        $("#TipoActividad").select2("close");
    });
    /*************************************************************************/

    $('#tree-actividad').on('change', function (e) {
        SetOrderTable();
    });
});


function update() {
    var name = $("#Descripcion").val();
    var tipoTarea = $("#TipoTarea").val();
    var estado = $("#Estado").val();
    var fechaVigenciaDesdeTicks = $("#FechaVigenciaDesdeTicks").val();
    var fechaVigenciaHastaTicks = $("#FechaVigenciaHastaTicks").val();
    var esVigenciaIndefinida = $("#EsVigenciaIndefinida").is(":checked");
}

/*****************************************************************************************************************/
/* VIEW INITIATION ***********************************************************************************************/
/*****************************************************************************************************************/
function initEdit() {
}
function initCreate() {
    $('input[action="CREATE"]').click(function () {
        $('#tareaForm').submit();
    });
}
function initDetail() {
}
function initDelete() {
    $('input[action="DELETE"]').click(function () {
        $('#tareaForm').submit();
    });
}

function loadOpciones(id) {
    rp3Get("/General/Tarea/GetTipoActividadOpciones", {
        id: id
    }, function (data) {
        if (!data.HasError) {
            $("#idopts li").remove();
            mdl_opcion = data;
            var datainner = data.split(",");
            var index; var resultt = "";

            if (data == "TIPON") {
                mdl_opcion = "";
                $("#rowopciones").hide();
                $("#rowvalor").hide();
                $("#rowlimite").show();
            }
            else if (data == "TIPOV") {
                mdl_opcion = "";
                $("#rowopciones").hide();
                $("#rowvalor").show();
                $("#rowlimite").hide();
            }
            else {
                $("#rowvalor").hide();
                $("#rowlimite").hide();
                if (data.length > 0) {
                    $("#rowopciones").show();
                    for (index = 0; index < datainner.length; index++) {
                        if (data != "" && data != null) {
                            $("#idopts").append("<li class='select2-search-choice'><div>" + datainner[index] + "</div><a href='#' onclick='return false;' class='select2-search-choice-close' tabindex='-1'></a></li>");
                        }
                    }
                }
                else {
                    $("#rowopciones").hide();
                }
            }
        }
    });
}

function SetOrderTable() {
    var data = Array();
    var groupOrder = 1;
    var itemOrder = 1;
    var tempgroup = null;
    var idtipoGrupo = 1;
    var idtipoTexto = 2;

    var currentnode;
    var parentnode;
    var parentcount;
    var parentcant;

    $("#tree-actividad li").each(function (i, v) {
        currentnode = $(this);

        var nodeContainer = $(currentnode).find("ol");

        if (nodeContainer.length > 0) {

            var idtarea = $(currentnode).attr("data-idtarea");

            $(currentnode).attr("data-opt", "parent");
            $(currentnode).attr("data-idtipo", idtipoGrupo);
            $(currentnode).attr("data-tipo", "Grupo");
            $(currentnode).attr("data-opciones", "");

            $(currentnode).attr("data-idorden", groupOrder);
            $(currentnode).attr("data-padre", null);
            tempgroup = $(currentnode).data("idActividad");
            groupOrder++;
            itemOrder = 1;

            $(nodeContainer).children('li').length;

            parentcount = 0;
            parentcant = $(nodeContainer).children('li').length; //$('[data-padre="' + idtarea + '"]').length;
            parentnode = currentnode;
        }
        else if (parentnode) {
            $(currentnode).attr("data-opt", "child");
            $(currentnode).attr("data-idorden", itemOrder);
            $(currentnode).attr("data-padre", tempgroup);
            itemOrder++;
            parentcount++;
        }
        else {
            $(currentnode).attr("data-opt", "parent");
            $(currentnode).attr("data-idorden", groupOrder);
            $(currentnode).attr("data-padre", null);

            if ($(currentnode).attr("data-idtipo") == idtipoGrupo) {
                $(currentnode).attr("data-idtipo", idtipoTexto);
                $(currentnode).attr("data-tipo", "Texto");
            }

            groupOrder++;
        }

        if (parentnode && parentcount >= parentcant) {
            parentnode = null;
        }

    });

    $("#tree-actividad li").each(function (i, v) {
        var elemOrden = $(v).find("[ordenli]");
        var orden = $(v).attr("data-idorden");
        $(elemOrden).text(orden);
        $(elemOrden).attr("orden", orden);

        var elemTipo = $(v).find("[tipoactividad]");
        var tipo = $(v).attr("data-tipo");
        $(elemTipo).text(tipo);

        var elemOpcion = $(v).find("[opciones]");
        var opciones = $(v).attr("data-opciones");
        $(elemOpcion).text(opciones);
    });
}

function removeActividad(id) {
    $("#tree-actividad li[data-id='" + id + "']").remove();
}

function loadModalDatos(index) {
    //CLEAR DATA
    document.getElementById("actividadNombre").value = "";
    document.getElementById("actividadValor").value = 0;
    document.getElementById("actividadLimite").value = 0;
    $("#TipoActividad").select2("val", 2)
    $("#idopts li").remove();

    mdl_index = index;

    var linea = $("#tree-actividad li[data-id='" + index + "']");

    var idtarea = $(linea).data("idtarea");
    var idactividad = $(linea).data("idActividad");
    var descripcion = $(linea).data("descripcion");
    var idtipo = $(linea).data("idtipo");
    var opciones = $(linea).data("opciones");
    var opt = $(linea).data("opt");
    var valor = $(linea).data("valor");
    var limite = $(linea).data("numero");

    document.getElementById("actividadNombre").value = descripcion;
    document.getElementById("actividadValor").value = valor;
    document.getElementById("actividadLimite").value = limite;
    var proof = $("input[rp3numericinput-bindinginput='actividadValor']");
    proof.val(valor);
    var proof2 = $("input[rp3numericinput-bindinginput='actividadLimite']");
    proof2.val(limite);

    if (idtipo != 0) {
        $("#TipoActividad").select2("val", idtipo);
    }

    loadOpciones(idtipo);

    if($("#tree-actividad li[data-id='" + index + "']").find("ol").length > 0){
        $("#rowtipoactividad").hide();
    } else {
        $("#rowtipoactividad").show();
    }


    rp3ModalShow("ModalDialog");
}

function editModalDatos() {
    var index = mdl_index;
    var m_descripcion = document.getElementById("actividadNombre").value;
    var m_idtipo = $("#TipoActividad").select2("val");
    var m_tipo = $("#TipoActividad").select2('data').text;
    var m_valor = document.getElementById("actividadValor").value;
    var m_limite = document.getElementById("actividadLimite").value;

    var linea = $("#tree-actividad li[data-id='" + index + "']");

    //DATA ATTRIB
    $(linea).data("descripcion", m_descripcion);
    $(linea).data("idtipo", m_idtipo);
    $(linea).data("tipo", m_tipo);
    $(linea).data("valor", m_valor);
    $(linea).data("numero", m_limite);
    $(linea).data("opciones", mdl_opcion);

    $(linea).attr("data-descripcion", m_descripcion);
    $(linea).attr("data-idtipo", m_idtipo);
    $(linea).attr("data-tipo", m_tipo);
    $(linea).attr("data-valor", m_valor);
    $(linea).attr("data-numero", m_limite);
    $(linea).attr("data-opciones", mdl_opcion);

    //VALUES
    $("#tree-actividad li[data-id='" + index + "'] [descripcion]").html(m_descripcion);
    $("#tree-actividad li[data-id='" + index + "'] [tipoactividad]").html(m_tipo);
    $("#tree-actividad li[data-id='" + index + "'] [opciones]").html(mdl_opcion);
}

function exportToJson(isnew) {
    var g_idtarea = $("#IdTarea").val();

    //TAB 1
    var g_name = $("#Descripcion").val();
    var g_tipoTarea = $("#TipoTarea").val();
    var g_estado = $("#Estado").val();
    var g_fechaVigenciaDesdeTicks = $("#FechaVigenciaDesdeTicks").val();
    var g_fechaVigenciaHastaTicks = $("#FechaVigenciaHastaTicks").val();
    var g_esVigenciaIndefinida = $("#EsVigenciaIndefinida").is(":checked");

    //TAB 2
    var g_todasRutas = $("#AplicaTodasLasRutas").is(":checked");
    var Json_RutasAplica = new Array();
    var countRuta = 0;
    $("#rutatable-tab-content tbody tr").each(function (i, v) {
        countRuta++;
        if ($("#rutatable-tab-content tbody tr[data-index='" + countRuta + "'] td label input").is(':checked')) {
            var g_idruta = $(this).data('ruta');
            var g_nombre = $(this).data('nombre');
            var g_agente = $(this).data('agente');
            var g_aplica = $(this).data('aplica');

            Json_RutasAplica.push({
                IdRuta: g_idruta,
                Nombre: g_nombre,
                Agente: g_agente,
                Aplica: g_aplica
            });
        }
    });

    //TAB 3
    var Json_Actividades = new Array();
    $("#tree-actividad li").each(function (i, v) {
        var g_index = $(this).data('id');
        var g_idtarea = $(this).data("idtarea");
        var g_idactividad = $(this).data("idActividad");
        var g_descripcion = $(this).data("descripcion");
        var g_idtipo = $(this).data("idtipo");
        var g_tipo = $(this).data("tipo");
        var g_opciones = $(this).data("opciones");
        var g_padre = $(this).data("padre");
        var g_orden = $(this).data("idorden");
        var g_valor = $(this).data("valor");
        var g_limite = $(this).data("numero");

        Json_Actividades.push({
            IdTareaActividad: g_idactividad,
            Descripcion: g_descripcion,
            TipoActividad: g_tipo,
            Orden: g_orden,
            IdTipoActividad: g_idtipo,
            Opciones: g_opciones,
            IdTareaActividadPadre: g_padre,
            Valor: g_valor,
            Limite: g_limite
        });
    });

    //Campos
    var PermitirCreacion = $("#PermitirCreacion").is(":checked");
    var PermitirModificacion = $("#PermitirModificacion").is(":checked");
    var SiempreEditarEnGestion = $("#SiempreEditarEnGestion").is(":checked");
    var SoloFaltantesEditarEnGestion = $("#SoloFaltantesEditarEnGestion").is(":checked");

    var Json_Campos = new Array();

    var nuevoExist = false;
    var existenteExist = false;
    var gestionExist = false;

    $("input[name='nuevocampo']").each(function () {
        if (this.checked) {
            Json_Campos.push({
                IdTarea: g_idtarea,
                IdCampo: this.value,
                Creacion: true,
                Modificacion: false,
                Gestion: false
            });

            nuevoExist = true;
        }
    });

    $("input[name='existentecampo']").each(function () {
        if (this.checked) {
            Json_Campos.push({
                IdTarea: g_idtarea,
                IdCampo: this.value,
                Creacion: false,
                Modificacion: true,
                Gestion: false
            });

            existenteExist = true;
        }
    });

    $("input[name='gestioncampo']").each(function () {
        if (this.checked) {
            Json_Campos.push({
                IdTarea: g_idtarea,
                IdCampo: this.value,
                Creacion: false,
                Modificacion: false,
                Gestion: true
            });

            gestionExist = true;
        }
    });

    if (PermitirCreacion && !nuevoExist) {
        rp3NotifyWarningAsPopup("Para permitir creación de clientes debe elegir al menos un campo.");
        return;
    }

    if (PermitirModificacion && !existenteExist) {
        rp3NotifyWarningAsPopup("Para permitir modificación de clientes debe elegir al menos un campo.");
        return;
    }

    if ((SiempreEditarEnGestion || SoloFaltantesEditarEnGestion) && !gestionExist) {
        rp3NotifyWarningAsPopup("Para permitir actualización de clientes en la gestión debe elegir al menos un campo.");
        return;
    }

    var model = {
        IdTarea: g_idtarea,
        Descripcion: g_name,
        TipoTarea: g_tipoTarea,
        Estado: g_estado,
        FechaVigenciaDesdeTicks: g_fechaVigenciaDesdeTicks,
        FechaVigenciaHastaTicks: g_fechaVigenciaHastaTicks,
        AplicaTodasLasRutas: g_todasRutas,
        //ReadOnly: false,
        //EstadoDescripcion: ,
        EsVigenciaIndefinida: g_esVigenciaIndefinida,
        TareaRutasAplica: Json_RutasAplica,
        Actividades: Json_Actividades,
        TareaClienteActualizacionCampos: Json_Campos,
        PermitirCreacion: PermitirCreacion,
        PermitirModificacion: PermitirModificacion,
        SiempreEditarEnGestion: SiempreEditarEnGestion,
        SoloFaltantesEditarEnGestion: SoloFaltantesEditarEnGestion
    };

    //alert(JSON.stringify(model));

    if (isnew) {
        rp3JSONDataPost("/General/Tarea/InsertTarea", {
            tareaView: model
        }, function (data) {
            rp3NotifyAsPopup(data.Messages);
            if (!data.HasError) {
                //var url = '@Url.Action("Index", "Tarea")';
                window.location.href = "Index";
            }
        });
    }
    else {
        rp3JSONDataPost("/General/Tarea/UpdateTarea", {
            tareaView: model
        }, function (data) {
            rp3NotifyAsPopup(data.Messages);
            if (!data.HasError) {
                //var url = '@Url.Action("Index", "Tarea")';
                window.location.href = "Index";
            }
        });
    }
}

function deleteTarea() {
    var g_idtarea = $("#IdTarea").val();

    rp3Post("/General/Tarea/DeleteTarea", {
        IdTarea: g_idtarea
    }, function (data) {
        rp3NotifyAsPopup(data.Messages);
        if (!data.HasError) {
            //var url = '@Url.Action("Index", "Tarea")';
            window.location.href = "Index";
        }
    });
}


function setOpcionDialog() {
    rp3Get("/General/Tarea/SetOpciones", null, function (data) {

        $("#setOpcionDialogContent").html(data);
        rp3ModalShow("setOpcionDialog");

        $("#tipoActividades [idtipoActividad]").click(function () {
            loadEditTipoActividad($(this));
        });

    });
};

function loadEditTipoActividad(row) {
    var idTipoActividad = $(row).attr('idtipoActividad');
    var descripcion = $(row).attr('descripcion');
    var tipo = $(row).attr('tipo');
    var estado = $(row).attr('estado');

    $("#IdTipoActividadOpcion").val(idTipoActividad);
    $("#DescripcionOpcion").val(descripcion);
    $("#TipoOpcion").select2("val", tipo);
    $("#EstadoOpcion").select2("val", estado);
    
    clearRows();

    rp3Get("/General/Tarea/GetTipoActividadOpciones", { id: idTipoActividad }, function (data) {

        var values = data.split(",");

        $.each(values, function (i, val) {
            addRow(val);
        });
    });
}

function triggerRemoveButton() {

    $('[RemoveButton] button').unbind("click");

    $('[RemoveButton] button').click(function (e) {
        var row = $(this).parents('tr');
        row.remove();

        verifyExistItems();

        e.preventDefault();
    });

    var c = {};

    $("#opciones tbody tr").draggable({
        helper: "clone",
        start: function (event, ui) {
            c.tr = this;
            c.helper = ui.helper;
        }
    });


    $("#opciones tbody tr").droppable({
        drop: function (event, ui) {
            var inventor = ui.draggable.text();
            $(this).find("input").val(inventor);

            var rowTarget = $(event).prop('target');
            var rowSource = c.tr;

            var valueTarget = $(rowTarget).children('[opcionvalue]td')[0].innerText;
            $(rowTarget).children('[opcion]input').prop('value', valueTarget);

            var aux = $(rowSource).clone();

            $(rowSource).html($(rowTarget).html());
            $(rowTarget).html($(aux).html());

            $(c.helper).remove();
        }
    });
};

function triggerInput() {
    $("#opcionText").keydown(function (event) {
        if (event.keyCode == 13) {

            var value = $(this).prop('value');

            if (value) {
                if (value.length > 0)
                    addRow($(this).prop('value'));
            }

            $(this).val('')
            event.preventDefault();
            return false;
        }
    });
}

function verifyExistItems() {
    var table = $('#opciones tbody');

    var rowCount = $(table).children('tr').children('[opcion]input').length;

    if (rowCount == 0) {
        $(table).append('<tr class="odd"><td valign="top" colspan="3" class="dataTables_empty">Ningún dato disponible en esta tabla</td></tr>');
    }
}

function clearRows() {
    var table = $('#opciones tbody');

    $('#opciones tbody').children('tr').each(function (i, row) {
        row.remove();
    });

    verifyExistItems();
}

function addRow(label) {
    var table = $('#opciones tbody');

    var empty = $('#opciones tbody').children('tr').children('[class="dataTables_empty"]td');

    if (empty) {
        var row = $(empty).parents('tr');
        row.remove();
    }

    var index = $(table).prop('rows').length;

    $(table).append('<tr class="odd">' +
    '<input type="hidden" value="' + label + '" opcion>' +
    '<td RemoveButton="" class="text-center " style="width:60px">' +
    '<button class="btn-xs btn btn-danger"><i class="fa fa-times"></i></button></td>' +
    '<td opcionvalue>' + label + ' </td>' +
    '</tr>');

    triggerRemoveButton();
}

function postSetOpciones() {
    var opciones = new Array();
    var table = $('#opciones tbody');

    $(table).children('tr').children('[opcion]input').each(function (e) {
        opciones.push($(this).prop('value'));
    });

    var idTipoActividad = $('#IdTipoActividadOpcion').val();
    var descripcion = $('#DescripcionOpcion').val();
    var tipo = $('#TipoOpcion').val();
    var estado = $('#EstadoOpcion').val();

    jQuery.ajaxSettings.traditional = true

    if (opciones.length > 0) {
        if (descripcion && descripcion.length > 0) {
            rp3Post("/General/Tarea/SetOpciones", { idTipoActividad: idTipoActividad, descripcion: descripcion, tipo: tipo, estado: estado, opciones: opciones }, function (data) {

                rp3NotifyAsPopup(data.Messages);

                rp3ModalHide("setOpcionDialog");

                loadTipoActividad();

            });
        } else {
            rp3NotifyWarningAsPopup("Debe ingresar la descripción");
        }
    } else {
        rp3NotifyWarningAsPopup("Debe ingresar al menos una opción");
    }
};

function loadTipoActividad() {

    rp3Get("/General/Tarea/GetTipoActividad", null, function (data) {

        $('#TipoActividad').select2('data', { id: '', text: '' });
        $('#TipoActividad').select2({ height: 'resolve' });
        $('#TipoActividad').empty();

        $.each(data, function (i, val) {
            $("#TipoActividad").append('<option value="' + val.id + '">' +
                    val.text + '</option>');
        });

    });
};
