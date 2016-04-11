var emptyProfilePicture = RP3_ROOT_PATH + "/Content/AgendaComercial/img/DefaultPerson.jpg";
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
    try{
        var newImg = $.parseJSON($("#UploadTargetCliente").contents().find("#jsonResult")[0].innerHTML);
        if (newImg.IsValid == true) {              
            cropImage(RP3_ROOT_PATH + newImg.ImagePath, newImg.Width, newImg.Height, 450, 450, "Ajuste de Foto");
            //            var img = document.getElementById("pictureProfile");
            //            img.src = newImg.ImagePath;
            //            $("#pictureProfile").hide();
            //            $("#pictureProfile").fadeIn(500, null);            
        }

        $("#panelLoadingFoto").hide();

    }catch(err){
    }
}

function uploadClienteImage() {
    //$("#panelLoadingImageProfile").show();
    //$("#pictureProfile").hide();
    $("#ImgFormCliente").submit();
}   

function deleteClienteImage(id) {

    var idCliente = parseInt($("#IdCliente").val());
    idClienteContactoFoto = id;

    rp3Post("/General/Cliente/DeleteFoto", { id: idCliente, idContacto: idClienteContactoFoto }, function (data) {

        if (!idClienteContactoFoto) {
            var img = document.getElementById("fotocliente");
            img.src = emptyProfilePicture;
            $("#Foto").val('');
        }
        else {
            var img = document.getElementById("fotocliente" + idClienteContactoFoto);
            img.src = emptyProfilePicture;
        }
    });
}
