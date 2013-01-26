/// <reference path="../DragAndDrop/DragAndDrop.js" />
$(function () {

    $("#event_filters").on('click', '.event-filter input[type=checkbox]', function (e) {
        var $chk = $(e.target)
        var $btn = $chk.closest('.btn')
        if ($chk.prop("checked")) {
            $btn.addClass('active')
        } else {
            $btn.removeClass('active')
        }
    })

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

    var md = new MarkdownDeep.Markdown();
    md.ExtraMode = true;

    var markFlowEvents = function ($card, employee) {
        if (!employee.Interviews || !employee.Interviews.length)
            return;

        var notesByEvent = _.groupBy(employee.Interviews, "EventTypeSlug");
        $el = $('<span class="flow-marks-element"></span>');
        _.each(notesByEvent, function (notes, slug) {
            var $event = $('<span title="' + notes[0].EventType + '" class="event-tag ' + slug + '">&nbsp;</span>');
            var notesHtml = ["<ul>"];
            _.each(notes, function (note) {
                notesHtml.push("<li><dl class='dl-horizontal'>");

                if (note.RealDate) {
                    notesHtml.push('<dt>Fecha:</dt><dd>');
                    notesHtml.push(moment(note.RealDate).format("D MMMM YYYY"));
                    notesHtml.push('</dd>');
                }
                if (note.Note) {
                    notesHtml.push('<dt>Nota:</dt><dd>');
                    notesHtml.push("<span class='markdown-content'>" + md.Transform(note.Note) + "</span>");
                    notesHtml.push('</dd>');
                }
                if (note.Attachment) {
                    notesHtml.push('<dt>Adjunto:</dt><dd>');
                    notesHtml.push("<a href='" + urlGenerator.action("Get", "Attachments", note.Attachment.Id) + "'>" + note.Attachment.FileName + "</a>");
                    notesHtml.push('</dd>');
                }
                notesHtml.push("</dl></li>");
            });
            notesHtml.push("</ul>");
            $event.attr("data-content", notesHtml.join(""));
            $el.append($event); 
        });

        var $el = $('<span class="bootstrap-scope"></span>').append($el);
        $card.prepend($el);
        $el.find(".flow-marks-element span").popover({
            trigger: 'manual',
            animate: false,
            html: true,
            placement: "bottom",
            template: '<div class="popover" onmouseover="$(this).mouseleave(function() {$(this).hide(); });"><div class="arrow"></div><div class="popover-inner"><button type="button" class="close" onclick="$(this).parent().parent().hide();">&times;</button><h3 class="popover-title"></h3><div class="popover-content"><p></p></div></div></div>'
        }).mouseenter(function (e) {
            $(this).popover('show');
            $(this).mouseleave(function (e) {
                if (!$(e.relatedTarget).parent(".popover").length)
                    $(this).popover('hide');
            })
        });
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
            if ($("#SearchInAttachmentsCheck").prop("checked"))
                searchParameters.SearchInAttachments = true;

            if ($("#HiredInclude").prop("checked")) 
                searchParameters.Hired = "Include";
            if ($("#HiredOnlyHired").prop("checked")) 
                searchParameters.Hired = "OnlyHired";

            var withEvents = []
            $(".event-filter input[name=WithEvents]:checked").each(function () {
                withEvents.push(this.value);
            });
            if (withEvents.length) {
                searchParameters.WithEvents = withEvents;
            }
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
                        .show("#create-applicant-attachment")
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
                        .hide("#create-applicant-attachment")
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
            markFlowEvents($card, item);
            dragAndDrop.prepareFileDropzone($card, {
                done: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .person($el)
                        .subtitle("Archivos subidos")
                        .visibility(".new-applicant", false)
                        .files(data)
                        .hide("#create-applicant-attachment")
                        .show(".detail-link")
                        .modal();
                },
                fail: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .error()
                        .person($el)
                        .subtitle("Error adjuntando archivos")
                        .visibility(".new-applicant", false)
                        .hide("#create-applicant-attachment")
                        .files(data)
                        .show(".detail-link")
                        .modal();
                }
            });
        }

    });

    $("#HiredInclude").change(function () {
        if ($(this).attr("checked"))
            $("#HiredOnlyHired").attr("checked", false);
    });

    $("#HiredOnlyHired").change(function () {
        if ($(this).attr("checked"))
            $("#HiredInclude").attr("checked", false);
    });

    $("#HighlightedCheck, #SearchInAttachmentsCheck, #HiredInclude, #HiredOnlyHired, .event-filter input[name=WithEvents]").change(function () {
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
