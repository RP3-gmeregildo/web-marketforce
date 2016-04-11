function showDependenciaMessage() {
    rp3NotifyWarningAsBlock("Se quitará la asociación del Canal actual con todos los Clientes y Lotes.", "Advertencia", true);
}

$(function () {

    $("#Estado").change(function () {
        var estado = $(this).val();
        var id = parseInt($("#IdCanal").val());

        if (estado == "I" && id > 0) {
            showDependenciaMessage();
        }
    });
});
