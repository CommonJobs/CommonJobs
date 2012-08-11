/// <reference path="../DragAndDrop/DragAndDrop.js" />
$(function () {
    var dragAndDrop = new DragAndDrop();

    function errorUploading(e, data, $el) {
        var modal = $('#upload-error-modal');
        var title = "Error adjuntando archivos";
        if ($el.hasClass("item-card")) {
            title = title + " a " + $el.find(".name").text();
        } else if ($el.hasClass("add-new-card")) {
            title = title + " a nuevo postulante";
        }
        modal.find(".title").text(title);

        if (data.files) {
            var html = [];
            for (var i in data.files) {
                html.push("<li>");
                html.push(data.files[i].name);
                html.push("</li>");
            }
            modal.find("ul.file-list").html(html.join(""));
        }
        modal.modal();
    };

    function successfulUploading(e, data, $el) {
        console.debug($el);
        var modal = $('#upload-ok-modal');
        var title = "Archivos subidos";
        var linkText = "Ver detalle del postulante";
        if ($el.hasClass("item-card")) {
            var name = $el.find(".name").text();
            title = title + " a " + name;
            linkText = "Ver detalle de " + name;
        } else if ($el.hasClass("add-new-card")) {
            title = title + " a nuevo postulante";
            linkText = "Ver detalle del nuevo postulante";
        }
        modal.find(".title").text(title);
        
        var html = [];
        for (var i in data.result.attachments) {
            html.push("<li>");
            html.push(data.result.attachments[i].FileName);
            html.push("</li>");
        }
        modal.find("ul.file-list").html(html.join(""));

        modal.find("a.detail-link").attr("href", data.result.editUrl).text(linkText);

        modal.modal();
    };

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
                fail: errorUploading
            });
        },
        prepareResultCards: function ($cards) {
            dragAndDrop.prepareFileDropzone($cards, {
                done: successfulUploading,
                fail: errorUploading
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
