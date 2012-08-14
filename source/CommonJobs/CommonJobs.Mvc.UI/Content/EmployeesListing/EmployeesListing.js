/// <reference path="../../Scripts/jquery-1.7.1-vsdoc.js" />
$(function () {
    var dragAndDrop = new DragAndDrop();

    var qs = new QuickSearchPage({
        //pageSize: 3,
        generateRedirectUrl: function (searchParameters) {
            return urlGenerator.action("Index", "Employees", searchParameters);
        },
        generateSearchUrl: function (searchParameters) {
            return urlGenerator.action("List", "Employees", searchParameters);
        },
        fillOtherSearchParameters: function (searchParameters) {
            if ($("#SearchInAttachmentsCheck").prop("checked"))
                searchParameters.searchInAttachments = true;
            if ($("#SearchInNotesCheck").prop("checked"))
                searchParameters.searchInNotes = true;
        },
        prepareResultCards: function ($cards) {
            dragAndDrop.prepareFileDropzone($cards, {
                singleFileUploads: true, //Un cuadro de dialogo por cada archivo
                add: function (e, data, $el) {
                    if ($el.hasClass("item-card")) {
                        new UploadModal($('#generic-modal'))
                            .person($el)
                            .title("Adjuntar Archivo")
                            .files(data)
                            .show();
                    }
                },
                done: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .person($el)
                        .title("Archivos subidos")
                        .files(data)
                        .show();

                    //TODO:
                    //modal.find("a.detail-link").attr("href", data.result.editUrl).text("Ver...");

                },
                fail: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .person($el)
                        .title("Error subiendo archivos")
                        .error()
                        .files(data)
                        .show();
                }
            });
        }
    });

    $("#SearchInAttachmentsCheck, #SearchInNotesCheck").change(function () {
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
