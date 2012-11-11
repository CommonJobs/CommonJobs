/// <reference path="../DragAndDrop/DragAndDrop.js" />
$(function () {

    var previousInit = UploadModal.prototype._init;
    UploadModal.prototype._init = function ($modal) {
        _.bind(previousInit, this)($modal);
        var me = this;
        this.$("#create-applicant-attachment").on("click", function (evt) {
            if (me.runValidations()) {
                me.data.formData = { name: me.$(".person-name").val() };
                me.data.submit();
            } else {
                evt.preventDefault();
            }
        });
    };

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
                add: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .person($el)
                        .subtitle("Adjuntar archivos")
                        .text(".title", "Crear postulante con adjuntos")
                        .visibility(".new-applicant", true)
                        .visibility(".person-name-validation", false)
                        .addValidation(".new-applicant", function (element) {
                            return $(element).find(".person-name").val().length > 0;
                        }, function (element, result) {
                            $(element).find(".person-name-validation").toggle(!result);
                            return result;
                        })
                        .files(data)
                        .closeButtonText("Cancelar")
                        .modal();
                },
                done: function (e, data) {
                    window.location = data.result.editUrl;
                },
                fail: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .error()
                        .person($el)
                        .visibility(".new-applicant", false)
                        .text(".title", "Crear postulante con adjuntos")
                        .subtitle("Error subiendo archivos")
                        .files(data)
                        .modal();
                }
            });

            var $cardButton = $card.find("button.adding-new");
            $card.on("click", function (evt) {
                evt.preventDefault();
                evt.stopPropagation();
                $card.find(".clickable-link").hide();
                $card.find(".adding-new").show();
                $card.find(".new-card-name").focus().on("keyup", function (evt) {
                    var valid = $(".new-card-name").val().length > 0;
                    if (evt.which == 13 && valid)
                        $cardButton.click();
                    if (evt.which == 27) {
                        $card.find(".clickable-link").show();
                        $card.find(".adding-new").hide();
                    }
                    $cardButton.attr("disabled", valid ? null : "disabled");
                });
            });
            $cardButton.on("click", function () {
                window.location = urlGenerator.action("Create", "Applicants", null, { name: $card.find(".new-card-name").val() });
            });
        },
        prepareResultCard: function ($card, item) {
            dragAndDrop.prepareFileDropzone($card, {
                done: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .person($el)
                        .subtitle("Archivos subidos")
                        .visibility(".new-applicant", false)
                        .files(data)
                        .show(".detail-link")
                        .modal();
                },
                fail: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .error()
                        .person($el)
                        .subtitle("Error adjuntando archivos")
                        .visibility(".new-applicant", false)
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
