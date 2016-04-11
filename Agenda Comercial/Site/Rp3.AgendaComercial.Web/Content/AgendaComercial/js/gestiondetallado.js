$(function () {
    $('[tarea]').click(function (e) {
        var item = $(this);

        e.preventDefault();

        var idRuta = $(item).attr('idRuta');
        var idAgenda = $(item).attr('idAgenda');

        openTareaDialog(idRuta, idAgenda);
    });
});

function initTareaDialog() {
    $('[actividad]').click(function (e) {
        var item = $(this);

        e.preventDefault();

        closeTareaDialog();

        var idRuta = $(item).attr('idRuta');
        var idAgenda = $(item).attr('idAgenda');
        var idTarea = $(item).attr('idTarea');

        openActividadDialog(idRuta, idAgenda, idTarea);
    });
};


function openActividadDialog(idRuta, idAgenda, idTarea) {
    rp3Get("/Ruta/GestionDetallado/DetalleActividad", { idRuta: idRuta, idAgenda: idAgenda, idTarea: idTarea }, function (data) {
        $("#detalleActividadDialog").dialog({
            autoOpen: false,
            show: "fast",
            height: 600,
            width: 850,
            modal: true,
            title: 'Actvidades',
            resizable: false,
            open: function () {
                $("#detalleActividadDialog").html(data);
            },
            close: function () { }
        });

        $("#detalleActividadDialog").dialog("open");
    });
};

function closeActividadDialog() {
    $("#detalleActividadDialog").dialog("close");
};

function openTareaDialog(idRuta, idAgenda) {
    rp3Get("/Ruta/GestionDetallado/DetalleTarea", { idRuta: idRuta, idAgenda: idAgenda }, function (data) {
        $("#detalleTareaDialog").dialog({
            autoOpen: false,
            show: "fast",
            height: 300,
            width: 850,
            modal: true,
            title: 'Tareas',
            resizable: false,
            open: function () {
                $("#detalleTareaDialog").html(data);
            },
            close: function () { }
        });

        $("#detalleTareaDialog").dialog("open");
    });
};

function closeTareaDialog() {
    $("#detalleTareaDialog").dialog("close");
};