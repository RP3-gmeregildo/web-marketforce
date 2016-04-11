var emptyProfilePicture = RP3_ROOT_PATH + "/Content/AgendaComercial/img/default.jpg";
var idClienteContactoFoto = null;

$(function () {
    triggerFotoEvents();
});

function triggerFotoEvents() {
    $(".img_Foto_Border").hover(function () {
        var idFoto = $(this).attr("idFoto");
        $('.pictureActions[idFoto="' + idFoto + '"]').show();
    }, function () {
        var idFoto = $(this).attr("idFoto");
        $('.pictureActions[idFoto="' + idFoto + '"]').hide();
    });
}

function selectClienteImage(id) {
    idClienteContactoFoto = id;
    $("#ImgFormCliente input:file").click();
}

function refreshClienteImage() {
    try {
        var newImg = $.parseJSON($("#UploadTargetCliente").contents().find("#jsonResult")[0].innerHTML);
        if (newImg.IsValid == true) {
            cropImage(RP3_ROOT_PATH + newImg.ImagePath, newImg.Width, newImg.Height, 450, 450, "Ajuste de Foto");
            //            var img = document.getElementById("pictureProfile");
            //            img.src = newImg.ImagePath;
            //            $("#pictureProfile").hide();
            //            $("#pictureProfile").fadeIn(500, null);            
        }

        $("#panelLoadingFoto").hide();

    } catch (err) {
    }
}

function uploadClienteImage() {
    //$("#panelLoadingImageProfile").show();
    //$("#pictureProfile").hide();
    $("#ImgFormCliente").submit();
}

function deleteClienteImage(id) {

    var idCliente = parseInt($("#IdProducto").val());

    rp3Post("/Pedido/Producto/DeleteFoto", { id: idCliente }, function (data) {

        var img = document.getElementById("fotoproducto");
        img.src = emptyProfilePicture;
        $("#Foto").val('');

    });
}
