function rp3MaxAttr(selector, attr, defaultValue) {
    var idArray = $(selector).map(function () {
        return $(this).attr(attr);
    }).get();

    var newId = defaultValue;
    if (idArray.length > 0)
        newId = Math.max.apply(Math, idArray);

    return newId;
}

var txtNuevo = "Nueva Etapa";

function resize() {
    var height = $(window).height() - $("#tree-content").offset().top - 50;

    $("#tree-content").css("height", height);
    $("#tree-content").css("min-height", height);

    var heightTarea = $(window).height() - $("#tarea-tree-content").offset().top - 50;

    $("#tarea-tree-content").css("height", heightTarea);
    $("#tarea-tree-content").css("min-height", heightTarea);

    var heightNoTarea = $(window).height() - $("#Notarea").offset().top - 50;

    $("#Notarea").css("height", heightNoTarea);
    $("#Notarea").css("min-height", heightNoTarea);
}

$(function () {

    $("#tree-etapa").nestable({
        maxDepth: 2
    });

    $("#tree-tarea").nestable({
        maxDepth: 1
    });

    $(window).resize(function () {
        resize();
    });

    $(window).load(function () {
        resize();
    });

    setTimeout(function () { resize(); }, 500);

    $("#tareaText").attr('readonly', 'readonly');

    $("button[action='nueva-etapa']").click(function () {
        var newId = rp3MaxAttr('#tree-etapa li', 'data-id', 1) + 1;

        var numdefault = 0;
        var textdefault = '';
        var newTr = "";

        newTr += '<li class="dd-item dd3-item selectable-item" data-opt="parent" data-padre="0" data-id="' + newId.toString() + '"';
        newTr += ' data-descripcion="' + txtNuevo + '" data-idorden="' + numdefault + '" data-tarea="" data-tipo="' + txtIdOportunidadTipo + '" data-new="1" data-esvariable="0">';
        newTr += '<div class="dd-handle dd3-handle"></div>';
        newTr += '<div class="dd3-content">';
        newTr += '<div class="row"><div class="col-md-9"> <div class="clickedit"><span class="tarea-orden" orden="1" ordenli></span>';
        newTr += '<span class="truncate-text" descripcion>' + txtNuevo + '</span>';
        newTr += '<span class="truncate-text" line> - </span>';
        newTr += '<span class="truncate-text" dias> 0 Día(s)</span></div>';
        newTr += '</div><div class="col-md-3"> <div class="column3">';
        newTr += '<button class="btn-xs btn btn-primary"  action="editetapa"><i class="fa fa-pencil-square-o"></i> </button>';
        newTr += '<button class="btn-xs btn btn-danger"  action="removeetapa"><i class="fa fa-times"></i> </button>';
        newTr += '</div>';
        newTr += '</div></div></div>';
        newTr += '</li>';

        $("#tree-etapa > ol").append(newTr);
        $('#tree-content').scrollTop($('#tree-content')[0].scrollHeight);

        SetOrderTable("#tree-etapa");

        triggerModal();
    });

    $('#tree-etapa').on('change', function (e) {
        SetOrderTable("#tree-etapa");
    });

    $('#tree-tarea').on('change', function (e) {
        SetOrderTable("#tree-tarea");
        setTareaData();
    });

    triggerModal();

    $("#tareaText").autocomplete({
        source: function (request, response) {
            $.getJSON(RP3_ROOT_PATH + "/General/Tarea/TareaAutocomplete", { term: request.term }, response);
        },
        minLength: 3,
        select: function (event, ui) {

            addTarea(ui.item.id, ui.item.label);

            $(event.target).val('');
            event.preventDefault();
        },
        open: function () {
            
        }
    });

    $('#searchTareaButton').on('click', function () {

        var idEtapa = $("#currentetapa").val();

        if (idEtapa) {

            rp3ModalShow("modal-tarea-search");
            $("#modal-tarea-search-content").rp3LoadingPanel();

            rp3Get("/Oportunidad/Etapa/GetSeleccionTarea", null, function (data) {

                $("#modal-tarea-search-content").rp3LoadingPanel('close');
                $("#modal-tarea-search-content").html('');
                $("#modal-tarea-search-content").html(data);

                $("#modal-tarea-search-content #searchtareatable tr td").click(function () {

                    var idTarea = parseInt($(this).attr('idTarea'));
                    var descripcion = $(this).attr('descripcion')

                    addTarea(idTarea, descripcion);

                    rp3ModalHide("modal-tarea-search");
                });

                $("#modal-tarea-search-content #searchtareatable").rp3DataTable()
            });
        }
    });

    $('button[action="UPDATE"]').click(function () {
        save();
    });
    $('button[action="CREATE"]').click(function () {
        save();
    });
});

function triggerModal() {
    $(".clickedit").unbind("click");
    $("button[action='editetapa']").unbind("click");
    $("button[action='removeetapa']").unbind("click");

    $(".clickedit").click(function () {
        var id = $(this).parents(".dd-item").attr('data-id');
        var descripcion = $(this).parents(".dd-item").attr('data-descripcion');
        var tareadata = $(this).parents(".dd-item").attr('data-tarea');
        
        setTareaEtapa(id, descripcion, tareadata);
    });

    $("button[action='editetapa']").click(function () {
        var id = $(this).parents(".dd-item").attr('data-id');
        loadModalDatos(id);
    });

    $("button[action='removeetapa']").click(function () {
        var id = $(this).parents(".dd-item").attr('data-id');

        rp3DialogConfirmationMessage("¿Está seguro de eliminar esta Etapa?", "Confirmación", function () {
            removeEtapa(id);

            clearTarea();
        }, null);
    });
}

function triggerTarea() {
    $("button[action='removetarea']").unbind("click");

    $("button[action='removetarea']").click(function () {
        var id = $(this).parents(".dd-item").attr('data-id');
        rp3DialogConfirmationMessage("¿Está seguro de eliminar esta Tarea?", "Confirmación", function () {
            
            removeTarea(id);
        }, null);
    });
}

function removeEtapa(id) {
    $("#tree-etapa li[data-id='" + id + "']").remove();
    SetOrderTable("#tree-etapa");
}

function removeTarea(id) {
    $("#tree-tarea li[data-id='" + id + "']").remove();
    SetOrderTable("#tree-tarea");

    setTareaData();

    evaluateNoTarea();
}

function setTareaEtapa(id, descripcion, data) {

    clearTarea();

    $("#Notarea").hide();
    $("#tarea-content-treelist").show();
    $("#colTarea .title-block h5").text("TAREAS DE: " + descripcion);
    $("#tareaText").removeAttr('readonly');

    $("#currentetapa").val(id);

    var array = data.split('~');

    $.each(array, function (i, tareaData) {
        if (tareaData) {
            var tarea = tareaData.split('|');
            addTarea(tarea[0], tarea[1]);
        }
    });

    evaluateNoTarea();
}

function setTareaData() {
    var idEtapa = $("#currentetapa").val();
    var data = "";

    $("#tree-tarea li").each(function () {

        var id = $(this).attr("data-id");
        var descripcion = $(this).attr("data-descripcion");

        if (data.length > 0)
            data += "~";

        data += id + "|" + descripcion;
    });

    $("#tree-etapa li[data-id='" + idEtapa + "']").attr("data-tarea", data);
}

function clearTarea() {
    $("#colTarea .title-block h5").text("TAREAS");
    $("#tree-tarea li").remove();
    $("#currentetapa").val('');
    $("#tareaText").attr('readonly', 'readonly');

    evaluateNoTarea();
}

function SetOrderTable(id) {
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

    $(id + " li").each(function (i, v) {
        currentnode = $(this);

        var nodeContainer = $(currentnode).find("ol");

        if (nodeContainer.length > 0) {

            $(currentnode).attr("data-opt", "parent");

            $(currentnode).attr("data-idorden", groupOrder);
            $(currentnode).attr("data-padre", null);

            tempgroup = $(currentnode).data("id");

            groupOrder++;
            itemOrder = 1;

            $(nodeContainer).children('li').length;

            parentcount = 0;
            parentcant = $(nodeContainer).children('li').length; 
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

            groupOrder++;
        }

        if (parentnode && parentcount >= parentcant) {
            parentnode = null;
        }

    });

    $(id + " li").each(function (i, v) {
        var elemOrden = $(v).find("[ordenli]");
        var orden = $(v).attr("data-idorden");
        $(elemOrden).text(orden);
        $(elemOrden).attr("orden", orden);
    });
}

function loadModalDatos(index) {
    //CLEAR DATA
    document.getElementById("etapaNombre").value = "";
    document.getElementById("etapaDias").value = 0;
    var esvariable_control = document.getElementsByName("etapaEsVariable");
    $(esvariable_control).iCheck('uncheck');

    $(esvariable_control).on('ifChecked || ifUnchecked', function () {
        if($(this).is(":checked"))
        {
            $("input[rp3numericinput-bindinginput='etapaDias']").attr('disabled', true);
        }
        else
        {
            $("input[rp3numericinput-bindinginput='etapaDias']").removeAttr('disabled');
        }
    });

    mdl_index = index;

    var linea = $("#tree-etapa li[data-id='" + index + "']");
    var descripcion = $(linea).data("descripcion");
    var dias = $(linea).data("dias");
    var esvariable = $(linea).data("esvariable");
    if (esvariable != 0)
    {
        $(esvariable_control).iCheck('check');
    }

    var opt = $(linea).data("opt");

    document.getElementById("etapaNombre").value = descripcion;
    document.getElementById("etapaDias").value = dias;
    //document.getElementById("etapaEsVariable").value = esvariable;

    var proof = $("input[rp3numericinput-bindinginput='etapaDias']");
    proof.val(dias);

    rp3ModalShow("ModalDialog");
}

function editModalDatos() {
    var index = mdl_index;
    var m_descripcion = document.getElementById("etapaNombre").value;
    var m_dias = document.getElementById("etapaDias").value;
    var esvariable_control = document.getElementsByName("etapaEsVariable");
    var m_esvariable = 0;
    if ($(esvariable_control).is(":checked"))
    {
        m_esvariable = 1;
    }

    var linea = $("#tree-etapa li[data-id='" + index + "']");

    //DATA ATTRIB
    $(linea).data("descripcion", m_descripcion);
    $(linea).data("dias", m_dias);
    $(linea).data("esvariable", m_esvariable);

    $(linea).attr("data-descripcion", m_descripcion);
    $(linea).attr("data-dias", m_dias);
    $(linea).attr("data-esvariable", m_esvariable);

    //VALUES
    $("#tree-etapa li[data-id='" + index + "'] [descripcion]").first().html(m_descripcion);
    if (m_esvariable == 1)
    {
        $("#tree-etapa li[data-id='" + index + "'] [dias]").first().html("Días por definir");
    }
    else
    {
        $("#tree-etapa li[data-id='" + index + "'] [dias]").first().html(m_dias + " Día(s)");
    }
    
}

function addTarea(id, descripcion) {

    var exist = $("#tree-tarea li[data-id='" + id + "']");

    if (exist.length == 0) {

        var newTr = "";

        newTr += '<li class="dd-item dd3-item" data-opt="parent" data-padre="0" data-id="' + id + '"';
        newTr += ' data-descripcion="' + descripcion + '" data-idorden="0"  >';
        if (txtIdOportunidadTipo != 0) {
            newTr += '<div class="dd-handle dd3-handle"></div>';
        }
        newTr += '<div class="dd3-content">';
        newTr += '<div class="row"><div class="col-md-10"> <div><span class="tarea-orden" orden="0" ordenli></span>';
        newTr += '<span class="truncate-text" descripcion>' + descripcion + '</span></div>';
        newTr += '</div><div class="col-md-2"> <div class="column4">';
        if (txtIdOportunidadTipo != 0) {
            newTr += '<button class="btn-xs btn btn-danger"  action="removetarea"><i class="fa fa-times"></i> </button>';
        }
        newTr += '</div>';
        newTr += '</div></div></div>';
        newTr += '</li>';

        $("#tree-tarea > ol").append(newTr);
        $('#tarea-tree-content').scrollTop($('#tarea-tree-content')[0].scrollHeight);

        SetOrderTable("#tree-tarea");
        triggerTarea();

        evaluateNoTarea();

        setTareaData();
    }
}

function evaluateNoTarea() {

    var exist = $("#tree-tarea li");

    if (exist.length == 0) {
        $("#Notarea").show();
        $("#tarea-content-treelist").hide();
    }
    else {
        $("#Notarea").hide();
        $("#tarea-content-treelist").show();
    }

    resize();
}

function save() {
    
    var model = [];
    var tipo_nombre = document.getElementById("OportunidadTipo_Descripcion").value;
    var tipo_estado = document.getElementById("OportunidadTipo_Estado").value;

    var oportunidad_tipo ={ IdOportunidadTipo: txtIdOportunidadTipo, Descripcion: tipo_nombre, Estado: tipo_estado };

    var countEtapa = 0;

    $("#tree-etapa li").each(function () {
        var IdEtapa = $(this).attr('data-id');
        var Descripcion = $(this).attr('data-descripcion');
        var Orden = $(this).attr('data-idorden');
        var IdEtapaPadre = $(this).attr('data-padre');
        var dias = $(this).attr('data-dias');
        var data = $(this).attr('data-tarea');
        var tipo = $(this).attr('data-tipo');
        var nuevo = $(this).attr('data-new');
        var esvariable = $(this).attr('data-esvariable');

        var tareas = [];

        var array = data.split('~');

        $.each(array, function (i, tareaData) {
            if (tareaData) {
                var tarea = tareaData.split('|');
                tareas.push({ IdEtapa: IdEtapa, IdTarea: tarea[0], Descripcion: tarea[1], Orden: i + 1 });
            }
        });
           
        var etapa = {
            IdEtapa: IdEtapa,
            Descripcion: Descripcion,
            Orden: Orden,
            IdEtapaPadre: IdEtapaPadre,
            Tareas: tareas,
            Dias: dias,
            IdOportunidadTipo: tipo,
            Nuevo: nuevo == 1 ? true : false,
            EsVariable: esvariable == 1 ? true : false
        };

        if (!IdEtapaPadre || IdEtapaPadre == "")
            countEtapa ++;

        model.push(etapa);
    });

    if (countEtapa <= 5) {
        rp3JSONDataPost("/Oportunidad/Etapa/UpdateEtapa", {
            etapaView: model, optTipo: oportunidad_tipo
        }, function (data) {

            rp3NotifyAsPopup(data.Messages);

            if (!data.HasError) {
                window.location.href = RP3_ROOT_PATH + "/Oportunidad/Etapa/";
            }
        });

    }
    else {
        rp3NotifyErrorAsPopup("Estan permitidas máximo 5 Etapas.")
    }
}
