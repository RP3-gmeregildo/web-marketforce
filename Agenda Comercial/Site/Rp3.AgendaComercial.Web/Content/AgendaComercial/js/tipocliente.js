function showDependenciaMessage() {
    rp3NotifyWarningAsBlock("Se quitará la asociación del Tipo de Cliente actual con todos los Clientes y Lotes", "Advertencia", true);
}

$(function () {

    $("#Estado").change(function () {
        var estado = $(this).val();
        var id = parseInt($("#IdTipoCliente").val());

        if (estado == "I" && id > 0) {
            showDependenciaMessage();
        }
    });
});
