

$(function () {

    var cb = function (start, end) {
        $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
        currentFechaInicial = start.format('YYYY-MM-D');
        currentFechaFinal = end.format('YYYY-MM-D');
    };

    var optionSet2 = {
        locale: 'es',
        startDate: moment().subtract('days', 180),
        endDate: moment(),
        opens: 'left',
        ranges: {
            'Último Mes': [moment().subtract('days', 30), moment()],
            'Últimos 3 Meses': [moment().subtract('days', 90), moment()],
            'Últimos 6 Meses': [moment().subtract('days', 180), moment()],
            //'Últimos 9 Meses': [moment().subtract('days', 270), moment()],
            'Últimos 12 Meses': [moment().subtract('days', 360), moment()]
        }
    };

    $('#reportrange span').html(moment().subtract('days', 180).format('MMMM D, YYYY') + ' - ' + moment().format('MMMM D, YYYY'));

    currentFechaInicial = moment().subtract('days', 180).format('YYYY-MM-D');
    currentFechaFinal = moment().format('YYYY-MM-D');

    $('#reportrange').daterangepicker(optionSet2, cb);
});

//CONVERT TO STRING
function ConvToStr(val) {
    if (arguments.length === 0) {
        return "";
    } else if (typeof val === "undefined") {
        return "undefined";
    } else if (val === null) {
        return "";
    } else if (typeof val === "boolean") {
        return val ? "true" : "false";
    } else if (typeof val === "number") {
        // super complex rules
    } else if (typeof val === "string") {
        if (String(val) == null)
            return "";
        else
            return val
    } else {
        return val;
    }
}