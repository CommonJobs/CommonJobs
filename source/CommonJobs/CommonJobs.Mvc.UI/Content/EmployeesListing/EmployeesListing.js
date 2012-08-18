/// <reference path="../../Scripts/underscore.js" />
/// <reference path="../../Scripts/jquery-1.7.1-vsdoc.js" />
$(function () {
    var dragAndDrop = new DragAndDrop();

    //Extend UploadModal
    var slotsTemplate = _.template($("#available-slots-template").text());
    var previousInit = UploadModal.prototype._init;
    UploadModal.prototype._init = function ($modal) {
        _.bind(previousInit, this)($modal);
        this.$(".slots").empty();
    };
    UploadModal.prototype.drawSlots = function ($el, employee) {
        //TODO: traer esto de la base de datos
        employee.AttachmentsBySlot = [{
            SlotId: "AttachmentSlots/Employee/CV",
            Date: "2012-01-01",
            Attachment: {
                Id: "idididididid",
                FileName: "filename.txt"
            }
        }];
        
        var filledById = {};
        _.each(employee.AttachmentsBySlot, function (filledItem) {
            filledById[filledItem.SlotId] = filledItem;
        });

        var model = {
            fileCount: this._files.length,
            slotsByNecessity: {}
        };

        _.each(ViewData.attachmentSlots, function (slot) {
            slot = _.clone(slot);
            slot.filled = filledById[slot.Id];
            if (!model.slotsByNecessity[slot.Necessity]) {
                model.slotsByNecessity[slot.Necessity] = [];
            }
            model.slotsByNecessity[slot.Necessity].push(slot);
        });

        var $slots = $(slotsTemplate({ model: model }));
        this.$(".slots").html($slots);
        
        return this;
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
        prepareResultCard: function ($card, item) {
            dragAndDrop.prepareFileDropzone($card, {
                add: function (e, data, $el) {
                    //todo: DAR A ELEGIR LOS SLOT DE CADA ARCHIVO O TAL VEZ MANDAR TODO AL GENERL                    
                    if ($el.hasClass("item-card")) {
                        new UploadModal($('#generic-modal'))
                            .person($el)
                            .title("Adjuntar Archivos")
                            .files(data)
                            .drawSlots($el, item)
                            //TODO: attach submit event to slots
                            //TODO: show detail-link with employee link 
                            .hide(".detail-link")
                            /*
                            .$(".detail-link", function () {
                                this.attr("href", data.result.editUrl);
                                this.show();
                            })
                            */
                            .modal(/*function () { data.submit(); }*/);
                    }
                },
                done: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .person($el)
                        .title("Archivos subidos")
                        .files(data)
                        .$(".detail-link", function () {
                            this.attr("href", data.result.editUrl);
                            this.show();
                        })
                        .modal();
                },
                fail: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .person($el)
                        .title("Error subiendo archivos")
                        .hide(".detail-link")
                        .error()
                        .files(data)
                        .modal();
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
