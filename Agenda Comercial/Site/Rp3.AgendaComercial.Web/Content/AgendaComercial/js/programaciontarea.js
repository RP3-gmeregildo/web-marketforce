$(function () {
    $("#EsIndefinida").each(function () {
        evaluateDisable(this.checked);
    });

    $("#EsIndefinida").on('ifChecked || ifUnchecked', function () {
        evaluateDisable(this.checked);
    });

    $("input[name='rutacheckall']").on('ifChecked || ifUnchecked', function () {

        var checkall = this.checked;

        $("input[name='rutas']").each(function () {
            if (checkall) {
                $(this).iCheck('check');
            }
            else {
                $(this).iCheck('uncheck');
            }
        });
    });

    $("input[name='tareacheckall']").on('ifChecked || ifUnchecked', function () {

        var checkall = this.checked;

        $("input[name='tareas']").each(function () {
            if (checkall) {
                $(this).iCheck('check');
            }
            else {
                $(this).iCheck('uncheck');
            }
        });
    });
});

function evaluateDisable(checked) {    
    $('#FechaFinEdit').removeAttr('disabled');

    if (checked) {        
        $('#FechaFinEdit').attr('disabled', true);
        
        $('#FechaFinEdit').val('');
    }
}