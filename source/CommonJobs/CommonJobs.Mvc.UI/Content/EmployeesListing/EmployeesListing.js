$(function () {
    var dragAndDrop = new DragAndDrop();

    function errorUploading(e, data, $el) {
        var modal = $('#upload-error-modal');
        var title = "Error adjuntando archivos";
        if ($el.hasClass("item-card")) {
            title = title + " a " + $el.find(".name").text();
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
        var linkText = "Ver detalle del empleado";
        if ($el.hasClass("item-card")) {
            var name = $el.find(".name").text();
            title = title + " a " + name;
            linkText = "Ver detalle de " + name;
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
                add: function(e, data, $el) {
                    console.debug(e);
                    console.debug(data);
                    console.debug($el);
                    alert("TODO: seleccionar slot y llamar al submit");
                },
                done: successfulUploading,
                fail: errorUploading
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
