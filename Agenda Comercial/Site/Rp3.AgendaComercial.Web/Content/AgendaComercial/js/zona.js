var MESSAGE;
var cloneCount = 0;
var padre = "";
var idPadre = 0;

function showDependenciaMessage() {
    rp3NotifyWarningAsBlock("La Zona actual será removida de todos los Lotes donde se encuentre asociada.", "Advertencia", true);
}

function showModificarMessage() {
    rp3NotifyWarningAsBlock("Al modificar la Zona actual, los Lotes donde se encuentre asociada podrán cambiar.", "Advertencia", true);
}

$(function () {

    $("#Estado").change(function () {
        var estado = $(this).val();
        var id = parseInt($("#Id").val());

        if (estado == "I" && id > 0){
            showDependenciaMessage();
        }
    });

    $("[name=provinciaSelect]").change(function () {
        ciudadShow();
    });

    $("button[actionName=AgregarZona]").click(function () {
        agregarZona();
    });

    $("input[name='ciudadcheckall']").on('ifChecked || ifUnchecked', function () {

        var checkall = this.checked;
        var app = $("[name=provinciaSelect]").val();

        $("tr[parentgeopoliticalstructureid='" + app + "'] input[name='ciudades']").each(function () {
            if (checkall) {
                $(this).iCheck('check');
            }
            else {
                $(this).iCheck('uncheck');
            }
        });
    });

    ciudadShow();
});

function ciudadShow() {
    var app = $("[name=provinciaSelect]").val();
    $("table [parentgeopoliticalstructureid]").hide();
    $("table [parentgeopoliticalstructureid='" + app + "']").show();

    rp3UpdateMainScroll();
    rp3DataTableAjustsColumns();
}

function agregarZona()
{
    var request = new ZonaGroup();
    if (request.Lista.length == 0)
    {
        rp3NotifyErrorAsPopup(MESSAGE);
        return;
    }
    if ($("#" + idPadre + "Select").length != 0)
        $("#" + idPadre + "Select").parent().remove();
    var options = ""
    $("#lastSelect option").each(function ($this) {
        var select = "";
        var optionValue = this.value;
        $(request.Lista).each(function ($this) {
            if (optionValue == this)
                select = " selected";
        });
        options = options + '<option value="' + this.value + '"' + select + '>' + this.label + '</option>';
    });
    if ($("#groupsIds").length == 0)
        $("#form-zona").append('<input type="hidden" value="' + idPadre + '" id="groupsIds" name="groupsIds"/>');
    else
        $("#groupsIds").val($("#groupsIds").val() + "," + idPadre);
    $("#zoneGroup").append('<div class="form-group"><label class="col-sm-6" style="text-align: left !important; margin-left: 10px">' + padre + '</label><select multiple style="margin-left: 10px" class="col-sm-10 controls" id="' + idPadre + 'Select" name="' + idPadre + 'Select" >' + options);
    var i = 2;
    while ($("#selectZona" + i).length != 0) {
        $("#selectZona" + i).remove();
        i++;
    }
    $("#" + idPadre + "Select").select2();
    $("#lastSelect").remove();
    $('#zonaAgregar').css('display', 'none');
    $("[name='paisSelect']").val(null).trigger("change");
    idPadre = 0;
    padre = "";
}

function ZonaGroup()
{
    var self = this;
    self.Lista = $.map($('#lastSelect option:selected'),
        function (e) { return $(e).val(); });
    self.IdParent = 0;
    self.ParentsName = "";
    rp3NotifyAsPopup()
}

function getNextLevel(control) {
    var val = control.value;
    rp3JSONDataPost("/Zona/GetNextLevel", { id: val }, function (data) {
        var i = data.Content.Content.TypeId;
        $("#lastSelect").remove();
        while ($("#selectZona" + i).length != 0) {
            $("#selectZona" + i).remove();
            i++;
        }
        var options = "";
        $(data.Content.Content.lista).each(function($this){
            options = options + '<option value="' + this.Value + '">' + this.Text + '</option>';
        });
        if (data.Content.Content.EsUltimo)
        {
            if ($("#" + data.Content.Content.Id + "Select").length == 0)
                $(control).parent().parent().parent().append('<div id="lastSelect"> <div class="form-group"><label class="control-label col-sm-3" for="">' + data.Content.Content.label + '</label><div class="col-sm-9 controls"><select multiple class="" id="' + data.Content.Content.label + 'Select" name="' + data.Content.Content.label + 'Select" >' + options + '</div>');
            else
            {
                options = "";
                $("#" + data.Content.Content.Id + "Select option").each(function ($this) {
                    var select = "";
                    var optionValue = this.value;
                    this.selected == true ? select = " selected" : "";
                    options = options + '<option value="' + this.value + '"' + select + '>' + this.label + '</option>';
                });
                $(control).parent().parent().parent().append('<div id="lastSelect"> <div class="form-group"><label class="control-label col-sm-3" for="">' + data.Content.Content.label + '</label><div class="col-sm-9 controls"><select multiple class="" id="' + data.Content.Content.label + 'Select" name="' + data.Content.Content.label + 'Select" >' + options + '</div>');
            }
            padre = data.Content.Content.ParentName;
            idPadre = data.Content.Content.Id;
            $('#zonaAgregar').css('display', 'block');
        }
        else {
            options = options + '<option value="" selected disabled hidden>Seleccione ' + data.Content.Content.label + ' </option>';
            $(control).parent().parent().parent().append('<div id="selectZona' + data.Content.Content.TypeId + '"<div class="form-group"><label class="control-label col-sm-3" for="">' + data.Content.Content.label + '</label><div class="col-sm-9 controls"><select class="" id="' + data.Content.Content.label + 'Select" name="' + data.Content.Content.label + 'Select" onchange="getNextLevel(this);">' + options);
        }
        $("#" + data.Content.Content.label + "Select").select2();
            
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
}

function openUbicacionDialog(markerIndex, latitud, longitud, titulo) {
    setUbicacionDialog(markerIndex, latitud, longitud, titulo, 850, 510);
};

function setUbicacionDialog(markerIndex, latitudCentro, longitudCentro, titulo, width, height) {
    rp3Get("/General/Zona/SetUbicacion", { markerIndex: markerIndex, latitud: latitudCentro, longitud: longitudCentro }, function (data) {
        $("#setUbicacionDialogContent").html(data);
        rp3ModalShow("setUbicacionDialog");
    });
}

function postSetUbicacion() {

    var latitud = $("#LatitudString").val();
    var longitud = $("#LongitudString").val();

    $("#latitudZona").remove();
    $("#longitudZona").remove();
    $("#form-zona").append('<input type="hidden" value="' + latitud + '" id="latitudZona" name="latitudZona"/>');
    $("#form-zona").append('<input type="hidden" value="' + longitud + '" id="longitudZona" name="longitudZona"/>');
    rp3ModalHide("setUbicacionDialog");

    var point = new google.maps.LatLng(latitud, longitud);

    boundsviewMap.extend(point);
    markerviewMap.setPosition(point);
    googleMapviewMap.setCenter(point);
    googleMapviewMap.setZoom(17);
}