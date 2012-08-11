/// <reference path="../DragAndDrop/DragAndDrop.js" />
$(function () {
    var dragAndDrop = new DragAndDrop();
    var qs = new QuickSearchPage({
        //pageSize: 3,
        generateRedirectUrl: function (searchParameters) {
            return urlGenerator.action("Index", "Applicants", searchParameters);
        },
        generateSearchUrl: function (searchParameters) {
            return urlGenerator.action("List", "Applicants", searchParameters);
        },
        fillOtherSearchParameters: function (searchParameters) {
            if ($("#HighlightedCheck").prop("checked"))
                searchParameters.Highlighted = true;
            if ($("#HaveInterviewCheck").prop("checked"))
                searchParameters.HaveInterview = true;
            if ($("#HaveTechnicalInterviewCheck").prop("checked"))
                searchParameters.HaveTechnicalInterview = true;
            if ($("#SearchInAttachmentsCheck").prop("checked"))
                searchParameters.SearchInAttachments = true;
        },
        prepareNewCard: function ($card) {
            dragAndDrop.prepareFileDropzone($card, {
                done: function (e, data) {
                    window.location = data.result.editUrl;
                }
            });
        },
        prepareResultCards: function ($cards) {
            dragAndDrop.prepareFileDropzone($cards, {
                //done: 
            });
        }
    });

    $("#HighlightedCheck, #HaveInterviewCheck, #HaveTechnicalInterviewCheck, #SearchInAttachmentsCheck").change(function () {
        qs.search();
    });

    $("#quickSearchSubmit").click(function () {
        //It also catch "enter"s in form inputs
        qs.redirect();
    });

    $(".results").on("click", ".clickable", function (e) {
        e.preventDefault();
        window.location = $(this).find(".clickable-link").attr("href");
    });

    qs.search();
});
