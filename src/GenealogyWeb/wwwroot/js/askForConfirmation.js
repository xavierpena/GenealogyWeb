$(document).ready(function () {

    /**
    * Ask for confirmation
    **/
    var handleAskfordelete = function (e) {
        e.preventDefault();

        var href = this.getAttribute("data-href");
        var sure = confirm("Are you sure you want to delete this item?");
        if (!sure) return;
        document.location = href;
    }

    $("a.askforconfirmation")
        .click(handleAskfordelete)
        .bind("contextmenu", handleAskfordelete)
        .dblclick(handleAskfordelete)
        .each(function () {
            var href = this.href;
            this.setAttribute("data-href", href);
            this.href = "javascript:void('Navigate to " + href.replace("'", "") + "')";
        })

});