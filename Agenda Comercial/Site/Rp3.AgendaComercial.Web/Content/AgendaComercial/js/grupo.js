function showDependenciaMessage() {
    rp3NotifyWarningAsBlock("Se quitará la asociación del Grupo actual con todos los Agentes.", "Advertencia", true);
}

$(function () {

    $("#Estado").change(function () {
        var estado = $(this).val();
        var id = parseInt($("#IdGrupo").val());

        if (estado == "I" && id > 0) {
            showDependenciaMessage();
        }
    });

    verifyExistItems();
    triggerRemoveButton();

    $("#grupoform #agenteText").autocomplete({
        source: function (request, response) {
            $.getJSON(RP3_ROOT_PATH + "/General/Agente/AgenteGrupoAutocomplete", { term: request.term }, response);
        },
        minLength: 3,
        select: function (event, ui) {

            addRow(ui.item.id, ui.item.label);

            $(event.target).val('');
            event.preventDefault();
        },
        open: function () {

        }
    });

    $('#searchAgenteButton').on('click', function () {

        rp3ModalShow("modal-agente-search");
        $("#modal-agente-search-content").rp3LoadingPanel();

        rp3Get("/Marcacion/Grupo/GetSeleccionAgente", null, function (data) {

            $("#modal-agente-search-content").rp3LoadingPanel('close');
            $("#modal-agente-search-content").html('');
            $("#modal-agente-search-content").html(data);

            $("#modal-agente-search-content #searchagentetable tr td").click(function () {

                var idAgente = parseInt($(this).attr('idAgente'));
                var descripcion = $(this).attr('descripcion')

                addRow(idAgente, descripcion);

                rp3ModalHide("modal-agente-search");
            });

            $("#modal-agente-search-content #searchagentetable").rp3DataTable()
        });
    });

    //setTimeout(function () {
    //    $("#agentes_wrapper .dataTables_scrollBody").css("height", "550px");
    //}, 300);

    //setTimeout(function () {
    //    rp3DataTableSetScrollBodyHeight("agentes", 550);
    //}, 100);
});


function triggerRemoveButton() {

    $('[RemoveButton] button').unbind("click");

    $('[RemoveButton] button').click(function (e) {
        var row = $(this).parents('tr');
        row.remove();

        verifyExistItems();
        reorder();

        e.preventDefault();
    });
};

function verifyExistItems() {
    var table = $('#agentes tbody');

    var rowCount = $(table).children('tr').children('[agente]input').length;

    if (rowCount == 0) {
        $(table).append('<tr class="odd"><td valign="top" colspan="3" class="dataTables_empty">Ningún dato disponible en esta tabla</td></tr>');
    }
}

function addRow(id, label) {
    var table = $('#agentes tbody');
    var countExist = $(table).children().children('[value="' + id + '"]input').length;

    if (countExist > 0)
        return;

    var empty = $('#agentes tbody').children('tr').children('[class="dataTables_empty"]td');

    if (empty) {
        var row = $(empty).parents('tr');
        row.remove();
    }

    var index = $(table).prop('rows').length;

    $(table).append('<tr class="odd">' +
    '<input type="hidden" name="agentes" value="' + id + '" agente>' +
    '<td RemoveButton="" class="text-center selectable-item" style="width:50px">' +
    '<button class="btn-xs btn btn-primary"><i class="fa fa-times"></i></button></td>' +
    '<td agentedescripcion>' + label + ' </td>' +
    '</tr>');

    triggerRemoveButton();
}