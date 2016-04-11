$(function () {
    triggerInput();
    verifyExistItems();
    triggerRemoveButton();

    evaluateTipoActividad($('#Tipo').prop('value'));

    $('#Tipo').change(function (e) {
        tipoActividadChange($(this).prop('value'));
    });
});

function evaluateTipoActividad(tipoActividad)
{
    $('#opcionText').removeAttr('disabled');

    if(tipoActividad != 'S' && tipoActividad != 'M')
        $('#opcionText').attr('disabled', true);
}

function tipoActividadChange(tipoActividad) {

    var table = $('#opciones tbody');
    $(table).children('tr').remove();
        
    verifyExistItems();

    evaluateTipoActividad(tipoActividad);
};

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
};

function verifyExistItems() {
    var table = $('#opciones tbody');

    var rowCount = $(table).children('tr').children('[opcion]input').length;

    if (rowCount == 0) {
        $(table).append('<tr class="odd"><td valign="top" colspan="3" class="dataTables_empty">Ningún dato disponible en esta tabla</td></tr>');
    }
}

function addRow(label) {
    var table = $('#opciones tbody');
    //var countExist = $(table).children().children('[value="' + id + '"]input').length;

    var empty = $('#opciones tbody').children('tr').children('[class="dataTables_empty"]td');

    if (empty) {
        var row = $(empty).parents('tr');
        row.remove();
    }

    var index = $(table).prop('rows').length;

    $(table).append('<tr class="odd">' +
    '<input type="hidden" name="opciones" value="' + label + '" opcion>' +
    '<td RemoveButton="" class="text-center " style="width:50px">' +
    '<button class="btn-sm btn btn-primary"><i class="icon-tag"></i> Remover</button></td>' +
    '<td opcionvalue class=" ">' + label + ' </td>' +
    '</tr>');

    triggerRemoveButton();
}




