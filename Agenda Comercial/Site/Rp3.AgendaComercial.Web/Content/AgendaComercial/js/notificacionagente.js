$(function () {
    $("input[name='agentecheckall']").on('ifChecked || ifUnchecked', function () {

        var checkall = this.checked;

        $("input[name='agentes']").each(function () {
            if (checkall) {
                $(this).iCheck('check');
            }
            else {
                $(this).iCheck('uncheck');
            }
        });
    });
});