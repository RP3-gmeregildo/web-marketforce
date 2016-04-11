$(function () {
    initstars();
});

function initstars() {
    
    $("[rate-widget]").each(function(i,val){
        var rateValue = $(val).attr("rate-value");
        setRateValue($(val),rateValue);
    });
    

    $("[rate-widget] [rate-widget-step]").click(function () {
        
        var ratevalue = $(this).attr("rate-value");
        var widget = $(this).parents("[rate-widget]");

        setRateValue(widget, ratevalue);
        
    });   

    function setRateValue(widget, ratevalue) {

        widget.children().each(function (i, val) {
            var value = $(val).attr("rate-value");
            if (ratevalue >= value) {
                $(val).attr("class", "star_full");
            } else {
                $(val).attr("class", "star_empty");
            }
        });

        widget.attr("rate-value", ratevalue);

        var target = $("#" + widget.attr("target"));
        if (target) {
            target.val(ratevalue);
        }

    }

    $("[rate-widget-content] [rate-widget-cancel]").click(function (e) {
        e.preventDefault();
        var widget = $(this).parents("[rate-widget-content]").find("[rate-widget]");
        setRateValue(widget,0);
    });
}