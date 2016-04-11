//function OnAppointmentFormSave(s, e) {
//    if (IsValidAppointment())
//        RutaScheduler.AppointmentFormSave();
//}
//function IsValidAppointment() {
//    $.validator.unobtrusive.parse(document);
//    return $("form").valid();
//}

var mapDate;

function onSelectionChanged(s, e) {
    var date = s.selection.interval.start;

    var newDate = _aspxGetInvariantDateTimeString(date);

    if (newDate != mapDate) {
        mapDate = newDate;

        var idProgramacionRuta = parseInt($('#IdProgramacionRuta').val());
        ubicacionMapMarker(idProgramacionRuta, mapDate);
    }
};

function ubicacionMapMarker(idProgramacionRuta, fecha) {
    rp3Get("/Ruta/ProgramacionRuta/UbicacionMapMarkerDay", { idProgramacionRuta: idProgramacionRuta, fecha: fecha }, function (data) {
        $("#content_map").html(data);
    });
};

function loadClientes(IdProgramacionRuta) {
    rp3Get("/Ruta/ProgramacionRuta/ProgramacionRutaDetalle", { IdProgramacionRuta: IdProgramacionRuta }, function (data) {
        $("#content_programacionrutadetalle").html(data);
        triggerDraggable();
    });
};

function triggerDraggable() {

    var c = {};

    $('.draggable').draggable({
        helper: 'clone', appendTo: 'body', zIndex: 100, start: function (event, ui) {
            c.tr = this;
            c.helper = ui.helper;
        }
    });
};

function InitalizejQuery(s, e) {

    triggerDraggable();

    $('.droppable').droppable({
        activeClass: "dropTargetActive",
        hoverClass: "dropTargetHover",

        drop: function (ev, ui) {

            //var idProgramacionRuta = $('#IdProgramacionRuta').val();            
            var idClienteDireccion = $(ui.draggable).find("input[type='hidden']").val();

            // Calculate an active time cell
            var cell = RutaScheduler.CalcHitTest(ev).cell;

            // Initiate a scheduler callback to create an appointment based on a cell interval
            if (cell != null) {

                RutaScheduler.InitializeCell(cell);
                RutaScheduler.PerformCallback({ start: _aspxGetInvariantDateTimeString(cell.interval.start), idClienteDireccion: idClienteDireccion });

            } else
                alert('Drop the dragged item on a specific time cell.');
        }

    });
};