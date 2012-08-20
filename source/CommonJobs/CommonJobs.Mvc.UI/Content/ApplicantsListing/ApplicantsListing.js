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
                },
                fail: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .error()
                        .person($el)
                        .text(".person-name", "Crear postulante con adjuntos")
                        .title("Error adjuntando archivos")
                        .files(data)
                        .modal();
                }
            });
        },
        prepareResultCard: function ($card, item) {
            dragAndDrop.prepareFileDropzone($card, {
                done: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .person($el)
                        .title("Archivos subidos")
                        .files(data)
                        .show(".detail-link")
                        .modal();
                },
                fail: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .error()
                        .person($el)
                        .title("Error adjuntando archivos")
                        .files(data)
                        .show(".detail-link")
                        .modal();
                }
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
