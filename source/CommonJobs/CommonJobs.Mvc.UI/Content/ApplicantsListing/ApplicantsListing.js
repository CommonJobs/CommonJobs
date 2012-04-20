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
        if ($("#HighlightedCheck").prop("checked"))
            query += "&Highlighted=true";
        if ($("#HaveInterviewCheck").prop("checked"))
            query += "&HaveInterview=true";
        if ($("#HaveTechnicalInterviewCheck").prop("checked"))
            query += "&HaveTechnicalInterview=true";
        if ($("#SearchInAttachmentsCheck").prop("checked"))
            query += "&SearchInAttachments=true";


        if (ajax) {
            $(".results").load("/Applicants/List" + query, null, setResultCount);
        } else {
            location.href = "/Applicants" + query;
        }
    }

    $("#quickSearch").keyup(function (e) {
        search(e.keyCode != 13);
    });

    $("#HighlightedCheck, #HaveInterviewCheck, #HaveTechnicalInterviewCheck, #SearchInAttachmentsCheck").change(function () {
        search(true);
    });

    $("#quickSearchSubmit").click(function () {
        search(false);
    });

    setResultCount();
});
