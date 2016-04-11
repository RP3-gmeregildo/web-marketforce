var countDiasNoLaborables = 1;
var readOnly = false;

$(function () {

    $('#preview').click(function (e) {
        PreviewReport();
    });
});

function PreviewReport() {
    window.open(window.location.origin + "/" + window.location.pathname.split('/')[1] + "/Producto/Preview", "_blank");
}

function evaluateChecked(control) {
    var id = $(control).attr("idDia");
    if (!$(control).is(':checked')) {
        $('#desdeJornada2_' + id).attr('disabled', true);
        $('#hastaJornada2_' + id).attr('disabled', true);
        $('#desdeJornada2_' + id).val('');
        $('#hastaJornada2_' + id).val('');
    }
    else {
        $('#desdeJornada2_' + id).removeAttr('disabled');
        $('#hastaJornada2_' + id).removeAttr('disabled');
    }
}

function evaluateCheckedLaboral(control) {
    var id = $(control).attr("idDia");
    if (!$(control).is(':checked')) {
        $('#desdeJornada1_' + id).attr('disabled', true);
        $('#hastaJornada1_' + id).attr('disabled', true);

        $("#jornada2_" + id).attr('disabled', true);
        $('#desdeJornada2_' + id).attr('disabled', true);
        $('#hastaJornada2_' + id).attr('disabled', true);

        $('#desdeJornada1_' + id).val('');
        $('#hastaJornada1_' + id).val('');
        $("#jornada2_" + id).iCheck('uncheck');
        $('#desdeJornada2_' + id).val('');
        $('#hastaJornada2_' + id).val('');
    }
    else {
        $('#desdeJornada1_' + id).removeAttr('disabled');
        $('#hastaJornada1_' + id).removeAttr('disabled');
        $("#jornada2_" + id).removeAttr('disabled');

        if ($("#jornada2_" + id).is(':checked')) {
            $('#desdeJornada2_' + id).removeAttr('disabled');
            $('#hastaJornada2_' + id).removeAttr('disabled');
        }
    }
}

function triggerDiaParcial() {
    $("input[esdiaparcial]").unbind('ifChecked || ifUnchecked');

    $("input[esdiaparcial]").on('ifChecked || ifUnchecked', function () {

        var id = $(this).attr("name").split('_')[1];

        if (!$(this).is(':checked')) {

            $('#nolabInicio_' + id).attr('disabled', true);
            $('#nolabFin_' + id).attr('disabled', true);

            $('#nolabInicio_' + id).val('');
            $('#nolabFin_' + id).val('');
        }
        else {
            $('#nolabInicio_' + id).removeAttr('disabled');
            $('#nolabFin_' + id).removeAttr('disabled');
        }
    });
}

function toTimePicker(control) {
    $(control).datetimepicker({
        datepicker: false,
        maxView: 1,
        minView: 0,
        startView: 1,
        format: 'hh:ii',
        minuteStep: 15,
        autoclose: true
    }).click(function () {
        removeHeader();
    });
}

function toDatePicker(control) {
    $(control).datetimepicker({
        datepicker: true,
        maxView: 2,
        minView: 2,
        startView: 2,
        language: 'es',
        format: 'dd/mm/yyyy',
        autoclose: true
    });

    $(control).datetimepicker('update');
}

function removeHeader() {
    $(".datetimepicker .datetimepicker-hours .switch").css('visibility', 'hidden');
    $(".datetimepicker .datetimepicker-hours .prev").css('visibility', 'hidden');
    $(".datetimepicker .datetimepicker-hours .next").css('visibility', 'hidden');
    $(".datetimepicker .datetimepicker-hours .prev").html("");
    $(".datetimepicker .datetimepicker-hours .next").html("");

    $(".datetimepicker .datetimepicker-minutes .switch").css('visibility', 'hidden');
    $(".datetimepicker .datetimepicker-minutes .prev").css('visibility', 'hidden');
    $(".datetimepicker .datetimepicker-minutes .next").css('visibility', 'hidden');
    $(".datetimepicker .datetimepicker-minutes .prev").html("");
    $(".datetimepicker .datetimepicker-minutes .next").html("");
}

function removerDescuento(control) {
    var id = $(control).attr("idDescuento");
    $("#group-descuento #descuento" + id).remove();
}

function agregarDescuento() {
    countDiasNoLaborables = $('[diaNoLaborable]').length + 1;

    $("#group-dias").append('<div id="dia' + countDiasNoLaborables + '" diaNoLaborable></div>');

    var newControl = $("#each-dia").children().clone();
    $(newControl).css('display', 'block');
    $(newControl).appendTo("#dia" + countDiasNoLaborables);

    $("#dia" + countDiasNoLaborables + " #nolaboral").attr("name", "nolaboral" + countDiasNoLaborables);
    toDatePicker($("#dia" + countDiasNoLaborables + " #nolaboral"));

    $("#dia" + countDiasNoLaborables + " #elimButton").attr("idDia", countDiasNoLaborables);

    $("#dia" + countDiasNoLaborables + " #esteAnioDiv").append("<input type='checkbox' id='esteAnio" + countDiasNoLaborables + "' />");
    $("#dia" + countDiasNoLaborables + " #esteAnio" + countDiasNoLaborables).attr("name", "esteAnio" + countDiasNoLaborables);
    $("#dia" + countDiasNoLaborables + " #esteAnio" + countDiasNoLaborables).rp3CheckBox();

    $("#dia" + countDiasNoLaborables + " #esDiaParcialDiv").append("<input type='checkbox' id='esDiaParcial" + countDiasNoLaborables + "' esDiaParcial=''/>");
    $("#dia" + countDiasNoLaborables + " #esDiaParcial" + countDiasNoLaborables).attr("name", "esDiaParcial_" + countDiasNoLaborables);
    $("#dia" + countDiasNoLaborables + " #esDiaParcial" + countDiasNoLaborables).attr("esDiaParcial");
    $("#dia" + countDiasNoLaborables + " #esDiaParcial" + countDiasNoLaborables).rp3CheckBox();

    $("#dia" + countDiasNoLaborables + " #nolabInicio").attr("name", "nolabInicio_" + countDiasNoLaborables);
    $("#dia" + countDiasNoLaborables + " #nolabInicio").attr("disabled", "true");
    toTimePicker($("#dia" + countDiasNoLaborables + " #nolabInicio"));
    $("#dia" + countDiasNoLaborables + " #nolabInicio").attr("id", "nolabInicio_" + countDiasNoLaborables);

    $("#dia" + countDiasNoLaborables + " #nolabFin").attr("name", "nolabFin_" + countDiasNoLaborables);
    $("#dia" + countDiasNoLaborables + " #nolabFin").attr("disabled", "true");
    toTimePicker($("#dia" + countDiasNoLaborables + " #nolabFin"));
    $("#dia" + countDiasNoLaborables + " #nolabFin").attr("id", "nolabFin_" + countDiasNoLaborables);

    triggerDiaParcial();

    countDiasNoLaborables++;
}
