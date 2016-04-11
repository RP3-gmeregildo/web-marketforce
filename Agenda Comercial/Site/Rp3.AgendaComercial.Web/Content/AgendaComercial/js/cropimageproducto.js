var cropImageApi = null;
var cropFileName = null;
var imageWidth, imageHeight;
function cropImage(file, width, height, imgWidth, imgHeight, title) {
    imageWidth = width;
    imageHeight = height;


    var img = document.getElementById("cropImage");
    cropFileName = file;
    if (cropImageApi == null) {
        img.src = file;
        var jcrop_api = $('#cropImage').Jcrop({
            allowResize: false,
            allowSelect: false,
            boxWidth: imgWidth,
            boxHeight: imgHeight,
            setSelect: [0, 0, width, height],
            minSize: [width, height],
            maxSize: [width, height],
            onChange: showCoords,
            onSelect: showCoords
        }, function () {
            cropImageApi = this;
        });
    }
    else {
        cropImageApi.setImage(file);
        cropImageApi.setSelect([0, 0, width, height]);
    }

    rp3ModalShow("cropImageDialog");
}

function showCoords(c) {
    $('#xcrop').val(c.x);
    $('#ycrop').val(c.y);
    //$('#wcrop').val(c.x2);
    //$('#y2').val(c.y2);
    $('#wcrop').val(c.w);
    $('#hcrop').val(c.h);
};

function saveClienteImage() {
    try {
        var fileName = cropFileName;
        var x = parseInt($("#xcrop").val());
        var y = parseInt($("#ycrop").val());
        var width = parseInt($("#wcrop").val());
        var height = parseInt($("#hcrop").val());

        if (x < 0)
            x = 0;

        if (y < 0)
            y = 0;

        if (width > imageWidth)
            width = imageWidth;

        if (height > imageHeight)
            height = imageHeight;

        var idCliente = parseInt($("#IdProducto").val());

        rp3Post("/Pedido/Producto/SaveFoto", { id: idCliente, fileName: fileName, x: x, y: y, width: width, height: height }, function (data) {

                $("#fotoproducto").show();
                $("#fotoproducto").attr('src', RP3_ROOT_PATH + data.ThumbnailImagePath);

                $("#URLFoto").val(data.ImageNamePath);
            
        });
    } catch (err) {
        alert(err.message);
    }
}