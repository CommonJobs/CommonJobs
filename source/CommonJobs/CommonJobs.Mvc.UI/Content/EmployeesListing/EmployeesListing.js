$(function () {

    $(".results").on("click", ".card", function (e) {
        e.preventDefault();
        window.location = $(this).find(".card-link").attr("href");
    });

    function setResultCount() {
        $("#resultCount").html($(".card").length - 1);
    }

    var loadResultsXHR = null;
    $(".results").ajaxSend(function (event, jqXHR, ajaxOptions) {
        if (loadResultsXHR)
            loadResultsXHR.abort();
        loadResultsXHR = jqXHR;
    });

    function search(ajax) {
        //TODO Fix hardcoded URLs

        var query = "?term=" + encodeURI($("#quickSearch").val());
        if ($("#SearchInAttachmentsCheck").prop("checked"))
            query += "&SearchInAttachments=true";
        if ($("#SearchInNotesCheck").prop("checked"))
            query += "&SearchInNotes=true";

        if (ajax) {
            $(".results").load("/Employees/List" + query, null, setResultCount);
        } else {
            location.href = "/Employees" + query;
        }
    }

    $("#quickSearch").keyup(function (e) {
        search(e.keyCode != 13);
    });

    $("#SearchInAttachmentsCheck, #SearchInNotesCheck").change(function () {
        search(true);
    });

    $("#quickSearchSubmit").click(function () {
        search(false);
    });

    setResultCount();
});
