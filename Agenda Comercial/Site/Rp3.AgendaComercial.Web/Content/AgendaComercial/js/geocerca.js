$(function () {
    $("input[name='zonacheckall']").on('ifChecked || ifUnchecked', function () {

        var checkall = this.checked;

        $("input[name='zonas']").each(function () {
            if (checkall) {
                $(this).iCheck('check');
            }
            else {
                $(this).iCheck('uncheck');
            }
        });
    });
});