function showDependenciaMessage() {
    rp3NotifyWarningAsBlock("El Cliente actual será removido de todos los Lotes, Rutas y Citas programadas.", "Advertencia", true);
}

function showModificarMessage() {
    rp3NotifyWarningAsBlock("Al modificar el Tipo, Canal o Calificación del Cliente actual, los Lotes y Rutas donde se encuentre asociado podrán cambiar.", "Advertencia", true);
}

var isChecking = false;

function initEsPrincipal() {
    //$('[esprincipal]').unbind('ifChecked || ifUnchecked');
    //$("[esprincipal]").on('ifChecked || ifUnchecked', function () {
    //    if (!isChecking) {

    //        isChecking = true;
    //        var principal = $(this);

    //        if (this.checked) {
    //            $("[esprincipal]").each(function () {
    //                if (principal != $(this)) {
    //                    $(this).iCheck('uncheck');
    //                }
    //            });
    //        }

    //        //evaluatePrincipal();

    //        isChecking = false;
    //    }
    //});
}

function evaluatePrincipal() {
    var exist = false;
    var first;

    $("[esprincipal]").each(function () {
        if (!first)
            first = $(this);

        if (this.checked)
            exist = true;
    });

    if (!exist && first)
        $(first).iCheck('check');
}

$(function () {
    $('[data-toggle="tab"][href="#tabloteruta"]').click(function () {
        setTimeout(function () {
            rp3DataTableAjustsColumns("#tabloteruta");
        }, 100);
    });

    evaluateTipoPersona();

    $("#TipoPersona").change(function () {
        evaluateTipoPersona();
    });

    function evaluateTipoPersona(){
        var tipoPersona = $("#TipoPersona").val();     

        if (tipoPersona == "N") {

            setTimeout(function () {
                $('#IdTipoIdentificacion').select2("enable", true);
            }, 100);

            $("[personaNatural]").css('display', 'block');
            $("[personaJuridica]").css('display', 'none');
        }
        else {

            $('#IdTipoIdentificacion').select2('val', 1); //RUC

            setTimeout(function () {
                $('#IdTipoIdentificacion').select2("enable", false);
            }, 100);

            $("[personaJuridica]").css('display', 'block');
            $("[personaNatural]").css('display', 'none');
        }
    }

    $("#Estado").change(function () {
        var estado = $(this).val();
        var id = parseInt($("#IdCliente").val());

        if (estado == "I" && id > 0) {
            showDependenciaMessage();
        }
    });

    $("#Calificacion").change(function () {
        var id = parseInt($("#IdCliente").val());
        if (id > 0) {
            showModificarMessage();
        }
    });

    $("#IdCanal").change(function () {
        var id = parseInt($("#IdCliente").val());
        if (id > 0) {
            showModificarMessage();
        }
    });

    $("#IdTipoCliente").change(function () {
        var id = parseInt($("#IdCliente").val());
        if (id > 0) {
            showModificarMessage();
        }
    });
});

function triggerEvents() {

    $('#direccionDetalle input[type=checkbox]').rp3CheckBox();

    initEsPrincipal();

    $('[ubicacion]').unbind("click");    
    
    $('[ubicacion]').click(function (e) {
        //var item = $(this).parents('[ubicacion]');
        var item = $(this);

        e.preventDefault();

        openUbicacionDialog($(item).attr('idCliente'), $(item).attr('idClienteDireccion'), $(item).attr('markerIndex'), $(item).attr('latitud'), $(item).attr('longitud'), $(item).attr('titulo'));
    });

    $('[editardireccion]').unbind("click");
    $('[editardireccion]').click(function (e) {
        e.preventDefault();
        var item = $(this);

        var idCliente = parseInt($(item).attr('idCliente'));
        var idClienteDireccion = parseInt($(item).attr('idClienteDireccion'));
        var titulo = $(item).attr('titulo');

        setDireccionDialog(idCliente, idClienteDireccion, titulo);
    });

    $('[agregardireccion]').unbind("click");
    $('[agregardireccion]').click(function (e) {

        var item = $(this);

        var idCliente = parseInt($(item).attr('idCliente'));
        var titulo = $(item).attr('titulo');

        openUbicacionDialog(idCliente, 0, 1, null, null, '');

        //setDireccionDialog(idCliente, 0, titulo);

        e.preventDefault();

    });

    $('[eliminardireccion]').unbind("click");
    $('[eliminardireccion]').click(function (e) {
        var item = $(this);

        e.preventDefault();

        var idCliente = parseInt($(item).attr('idCliente'));
        var idClienteDireccion = parseInt($(item).attr('idClienteDireccion'));
        var titulo = $(item).attr('titulo');
        var mensaje = $(item).attr('mensaje');

        rp3DialogConfirmationMessage(mensaje, titulo, function (){
            postEliminarDireccion(idCliente, idClienteDireccion);
        });
    });

    $('[editarcontacto]').unbind("click");
    $('[editarcontacto]').click(function (e) {
        var item = $(this);
        e.preventDefault();

        var idCliente = parseInt($(item).attr('idCliente'));
        var idClienteContacto = parseInt($(item).attr('idClienteContacto'));
        var titulo = $(item).attr('titulo');

        setContactoDialog(idCliente, idClienteContacto, titulo);
    });

    $('[agregarcontacto]').unbind("click");
    $('[agregarcontacto]').click(function (e) {
        var item = $(this);
        e.preventDefault();

        var idCliente = parseInt($(item).attr('idCliente'));
        var titulo = $(item).attr('titulo');

        setContactoDialog(idCliente, 0, titulo);
    });

    $('[eliminarcontacto]').unbind("click");
    $('[eliminarcontacto]').click(function (e) {
        var item = $(this);
        e.preventDefault();
        var idCliente = parseInt($(item).attr('idCliente'));
        var idClienteContacto = parseInt($(item).attr('idClienteContacto'));
        var titulo = $(item).attr('titulo');
        var mensaje = $(item).attr('mensaje');

        rp3DialogConfirmationMessage(mensaje, titulo, function () {
            postEliminarContacto(idCliente, idClienteContacto);
        });
        
    });
};

function postEliminarDireccion(idCliente, idClienteDireccion) {
    rp3Post("/General/Cliente/EliminarDireccion", { idCliente: idCliente, idClienteDireccion: idClienteDireccion }, function (data) {

        rp3NotifyAsPopup(data.Messages);

        rp3Get("/General/Cliente/DireccionesDetalle", { idCliente: idCliente }, function (data) {
            $("#content_direcciones").html(data);
        });

        rp3Get("/General/Cliente/UbicacionMapMarkerClient", null, function (data) {
            $("#content_mapClient").html(data);
        });

        rp3Get("/General/Cliente/LoteRuta", { idCliente: idCliente }, function (data) {
            $("#content_loteruta").html(data);
        });
    });
};

function setDireccionDialog(idCliente, idClienteDireccion, titulo, latitud, longitud, place) {

    rp3Get("/General/Cliente/SetDireccion", {
        idCliente: idCliente, idClienteDireccion: idClienteDireccion,
        latitud: latitud, longitud: longitud, place: place
    }, function (data) {

        $("#setDireccionDialogContent").html(data);
        rp3ModalShow("setDireccionDialog");
    });

};

function postSetDireccion(idCliente) {
    if ($('#setdireccionform').valid())
    {
        
        rp3Post("/General/Cliente/SetDireccion", $('#setdireccionform').serialize(), function (data) {

            rp3NotifyAsPopup(data.Messages);

            if (!data.HasError) {
                rp3ModalHide("setDireccionDialog");

                rp3Get("/General/Cliente/DireccionesDetalle", { idCliente: idCliente }, function (data) {
                    $("#content_direcciones").html(data);
                });

                rp3Get("/General/Cliente/UbicacionMapMarkerClient", null, function (data) {
                    $("#content_mapClient").html(data);
                });

                rp3Get("/General/Cliente/LoteRuta", { idCliente: idCliente }, function (data) {
                    $("#content_loteruta").html(data);
                });
            }
        });
    }

};

function postEliminarContacto(idCliente, idClienteContacto) {
    rp3Post("/General/Cliente/EliminarContacto", { idCliente: idCliente, idClienteContacto: idClienteContacto }, function (data) {

        rp3NotifyAsPopup(data.Messages);

        rp3Get("/General/Cliente/ContactosDetalle", { idCliente: idCliente }, function (data) {
            $("#content_contactos").html(data);
        });
    });
};

function setContactoDialog(idCliente, idClienteContacto, titulo) {
    rp3Get("/General/Cliente/SetContacto", { idCliente: idCliente, idClienteContacto: idClienteContacto }, function (data) {
        $("#setContactoDialogContent").html(data);
        rp3ModalShow("setContactoDialog");
    });
};

function postSetContacto(idCliente) {
    if ($('#setcontactoform').valid()) {
        rp3Post("/General/Cliente/SetContacto", $('#setcontactoform').serialize(), function (data) {

            rp3NotifyAsPopup(data.Messages);

            if (!data.HasError) {

                rp3ModalHide("setContactoDialog");

                rp3Get("/General/Cliente/ContactosDetalle", { idCliente: idCliente }, function (data) {
                    $("#content_contactos").html(data);
                });

            }
        });
    }
};

function openUbicacionDialog(idCliente, idClienteDireccion, markerIndex, latitud, longitud, titulo) {
    setUbicacionDialog(idCliente, idClienteDireccion, markerIndex, latitud, longitud, titulo, 850, 510);
};

function setUbicacionDialog(idCliente, idClienteDireccion, markerIndex, latitudCentro, longitudCentro, titulo, width, height) {
    rp3Get("/General/Cliente/SetUbicacion", { idCliente: idCliente, idClienteDireccion: idClienteDireccion, markerIndex: markerIndex, latitud: latitudCentro, longitud: longitudCentro }, function (data) {
        $("#setUbicacionDialogContent").html(data);
        rp3ModalShow("setUbicacionDialog");
    });
}

function postSetUbicacion(idCliente, idClienteDireccion, markerIndex) {

    var latitud = $("#LatitudString").val();
    var longitud = $("#LongitudString").val();
    var place = $("#PlaceString").val();

    if (idClienteDireccion != 0) {

        //rp3Post("/General/Cliente/SetUbicacion", { idCliente: idCliente, idClienteDireccion: idClienteDireccion, latitud: latitud, longitud: longitud }, function (data) {

        //    rp3ModalHide("setUbicacionDialog");

        //    rp3NotifyAsPopup(data.Messages);

        //    rp3Get("/General/Cliente/DireccionesDetalle", { idCliente: idCliente }, function (data) {
        //        $("#content_direcciones").html(data);
        //    });
        //});

        rp3ModalHide("setUbicacionDialog");

        var content = $("#direccioncontent" + idClienteDireccion);
        $(content).find("[latitud]").val(latitud);
        $(content).find("[longitud]").val(longitud);

        var ubicacionButton = $("button[idClienteDireccion=" + idClienteDireccion + "]");

        $(ubicacionButton).attr('latitud', latitud);
        $(ubicacionButton).attr('longitud', longitud);

        refreshMap(idCliente, idClienteDireccion, latitud, longitud, markerIndex, place);
    }
    else {
        rp3ModalHide("setUbicacionDialog");

        setDireccionDialog(idCliente, 0, '', latitud, longitud, place);
    }
}

function refreshMap(idCliente, idClienteDireccion, latitud, longitud, markerIndex, titulo) {
    rp3Get("/General/Cliente/UbicacionMapMarkerSingle", { idCliente: idCliente, idClienteDireccion: idClienteDireccion, latitud: latitud, longitud: longitud, markerIndex: markerIndex, titulo: titulo }, function (data) {
        $("#contentmap_viewMap" + idClienteDireccion).html(data);
    });
}

function saveEditCliente() {

    var IdCliente = $("#IdCliente").val();
    var TipoPersona = $("#TipoPersona").val();
    var Calificacion = $("#Calificacion").val();
    var IdTipoIdentificacion = $("#IdTipoIdentificacion").val();
    var Identificacion = $("#Identificacion").val();

    var Nombre1 = $("#Nombre1").val();
    var Nombre2 = $("#Nombre2").val();
    var RazonSocial = $("#RazonSocial").val();
    var Apellido1 = $("#Apellido1").val();
    var Apellido2 = $("#Apellido2").val();

    var CorreoElectronico = $("#CorreoElectronico").val();
    var PaginaWeb = $("#ClienteDato_PaginaWeb").val();
    var FechaNacimiento = $("#ClienteDato_FechaNacimiento").val();
    var Genero = $("#ClienteDato_Genero").val();
    var EstadoCivil = $("#ClienteDato_EstadoCivil").val();

    var IdTipoCliente = $("#IdTipoCliente").val();
    var IdCanal = $("#IdCanal").val();

    var Estado = $("#Estado").val();

    var ActividadEconomica = $("#ClienteDato_ActividadEconomica").val();
    var InicioActividad = $("#ClienteDato_InicioActividad").val();

    var RepresentanteLegal = $("#ClienteDato_RepresentanteLegal").val();
    var RepresentateLegalIdentificacion = $("#ClienteDato_RepresentateLegalIdentificacion").val();

    var clienteDato = {
            PaginaWeb: PaginaWeb,
            FechaNacimiento: FechaNacimiento,
            Genero: Genero,
            EstadoCivil: EstadoCivil,
            ActividadEconomica: ActividadEconomica,
            InicioActividad: InicioActividad,
            RepresentanteLegal: RepresentanteLegal,
            RepresentateLegalIdentificacion: RepresentateLegalIdentificacion
    }

    var clienteDirecciones = [];

    $("[direccionItem]").each(function (i) {

        var IdClienteDireccion = $(this).find('[iddireccion]').val();
        var TipoDireccion = $(this).find('[tipodireccion]').val();

        var Latitud = $(this).find('[latitud]').val();
        var Longitud = $(this).find('[longitud]').val();

        var Direccion = $(this).find('[direccion]').val();
        var Descripcion = $(this).find('[descripcion]').val();
        var Referencia = $(this).find('[referencia]').val();
        var AplicaRuta = $(this).find('[aplicaruta]').is(':checked');
        var IdCiudad = $(this).find('[idciudad]').val();
        var EsPrincipal = $(this).find('[esprincipal]').is(':checked');
        var Telefono1 = $(this).find('[telefono1]').val();
        var Telefono2 = $(this).find('[telefono2]').val();

        clienteDirecciones.push({
            IdCliente: IdCliente,
            IdClienteDireccion: IdClienteDireccion,
            TipoDireccion: TipoDireccion,
            Latitud: Latitud,
            Longitud: Longitud,
            Direccion: Direccion,
            Descripcion: Descripcion,
            Referencia: Referencia,
            AplicaRuta: AplicaRuta,
            IdCiudad: IdCiudad,
            EsPrincipal: EsPrincipal,
            Telefono1: Telefono1,
            Telefono2: Telefono2
        });
    });

    var clienteContactos = [];

    $("[contactoItem]").each(function (i) {

        var IdClienteContacto = $(this).find('[idcontacto]').val();

        var Nombre = $(this).find('[nombre]').val();
        var Apellido = $(this).find('[apellido]').val();
        var IdClienteDireccionContacto = $(this).find('[idclientedireccion]').val();
        var Telefono1 = $(this).find('[telefono1]').val();
        var Telefono2 = $(this).find('[telefono2]').val();
        var Cargo = $(this).find('[cargo]').val();
        var CorreoElectronico = $(this).find('[correoelectronico]').val();

        clienteContactos.push({
            IdCliente: IdCliente,
            IdClienteContacto: IdClienteContacto,
            Nombre: Nombre,
            Apellido: Apellido,
            IdClienteDireccion: IdClienteDireccionContacto,
            Telefono1: Telefono1,
            Telefono2: Telefono2,
            Cargo: Cargo,
            CorreoElectronico: CorreoElectronico
        });
    });

    var cliente = {
        IdCliente: IdCliente,
        TipoPersona: TipoPersona,
        Calificacion: Calificacion,
        IdTipoIdentificacion: IdTipoIdentificacion,
        Identificacion: Identificacion,
        Nombre1: Nombre1,
        Nombre2: Nombre2,
        RazonSocial: RazonSocial,
        Apellido1: Apellido1,
        Apellido2: Apellido2,
        CorreoElectronico: CorreoElectronico,
        IdTipoCliente: IdTipoCliente,
        IdCanal: IdCanal,
        Estado: Estado,
        ClienteDato: clienteDato,
        ClienteDirecciones: clienteDirecciones,
        ClienteContactos: clienteContactos
    };

    rp3Post("/General/Cliente/Edit", cliente, function (data) {

        rp3NotifyAsPopup(data.Messages);

        if (!data.HasError) {
            document.location = RP3_ROOT_PATH + "/General/Cliente/Index"
        }
    });
}
