$(function () {
    $('[data-toggle="tab"][href="#tabprogramacion"]').click(function () {
        setTimeout(function () {
            rp3DataTableAjustsColumns("#tabprogramacion");
        }, 100);
    });
});

function init_Calendario() {
    var bodyD = document.body,
    htmlD = document.documentElement;

    var heightD = 800;//Math.max(bodyD.scrollHeight, bodyD.offsetHeight, htmlD.clientHeight, htmlD.scrollHeight, htmlD.offsetHeight);

    $('#external-events div.external-event').each(function () {
        // create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
        // it doesn't need to have a start or end
        var eventObject = {
            title: $.trim($(this).text()) // use the element's text as the event title
        };
        // store the Event Object in the DOM element so we can get to it later
        $(this).data('eventObject', eventObject);
        // make the event draggable using jQuery UI
        $(this).draggable({
            zIndex: 999,
            revert: true,      // will cause the event to go back to its
            revertDuration: 0  //  original position after the drag
        });
    });

    var date = new Date();
    var d = date.getDate();
    var m = date.getMonth();
    var y = date.getFullYear();

    var idRuta = parseInt($("#IdRuta").val());
    var tempVar = "";

    $('#agenda-calendar').fullCalendar({
        header: {
            left: 'title',
            center: '',
            right: 'month, prev,next'//agendaWeek,agendaDay,today,
        },
        defaultView: 'month',
        //editable: true,
        axisFormat: 'H:mm',
        timeFormat: '',
        firstHour: 8,
        slotMinutes: 15,
        businessHours: true,
        contentHeight: heightD - 260,
        events: {
            url: RP3_ROOT_PATH + "/Ruta/Ruta/GetPreview/",
            type: 'GET',
            data: {
                idRuta: idRuta
            },
            success: function (doc) {
                if ($(doc).length > 0) {
                    loadMapMarker($(doc)[0].start);
                }
            }
        },
        eventClick: function (calEvent, jsEvent, view) {
            loadMapMarker($.fullCalendar.formatDate(calEvent.start, "yyyy-MM-dd")); //HH:mm:ss
        },
        dayClick: function (date, allDay, jsEvent, view) {
            var fecha = $.fullCalendar.formatDate(date, "yyyy-MM-dd");

            loadMapMarker(fecha);
        }
    });
}


function refreshCalendar() {
    $('#agenda-calendar').fullCalendar('refetchEvents');
    //var idRuta = parseInt($("[idruta]").attr("idRuta"));
    loadMapMarker(currentMapDate);
}

var currentMapDate;

var refreshing = false;
var refreshpending = false;

function loadMapMarker(fecha) {
    var idRuta = parseInt($("#IdRuta").val());
    currentMapDate = fecha;   

    if (!refreshing) {
        refreshing = true;

        var fechafrom = fecha.substring(0, 10).split("-");
        var newDate = new Date(fechafrom[0], fechafrom[1] - 1, fechafrom[2]);

        var fechaTitulo = $.fullCalendar.formatDate(newDate, "dd 'de' MMMM 'de' yyyy");

        $('.fc-day').css('background', 'white');
        $('.fc-day[data-date="' + $.fullCalendar.formatDate(newDate, "yyyy-MM-dd") + '"]').css('background', '#F3F781');
        $("#fechaRuta").html(fechaTitulo);

        rp3ShowLoadingPanel("#content_map");
        rp3Get("/Ruta/Ruta/GetMapPreview", { idRuta: idRuta, fecha: fecha }, function (data) {
            $("#content_map").html(data);
            rp3HideLoadingPanel("#content_map");

            refreshing = false;

            if (refreshpending) {
                refreshpending = false;
                loadMapMarker(currentMapDate);
            }
        });
    }else {
        refreshpending = true;
    }
}