/// <reference path="../../Scripts/underscore.js" />
/// <reference path="../../Scripts/jquery-1.7.1-vsdoc.js" />
$(function () {
    var dragAndDrop = new DragAndDrop();

    //Extend UploadModal
    var slotBtnTemplate = _.template($("#slot-button-template").text());
    var slotsTemplate = _.template($("#available-slots-template").text());
    var previousInit = UploadModal.prototype._init;
    UploadModal.prototype._init = function ($modal) {
        _.bind(previousInit, this)($modal);
        this.$(".slots").empty();
        this.closeButtonText("Cerrar");
        this.hide(".detail-link");
    };
    UploadModal.prototype.drawSlots = function ($el, employee) {
        var me = this;

        var filledById = {};
        _.each(employee.AttachmentsBySlot, function (filledItem) {
            filledById[filledItem.SlotId] = filledItem;
        });

        var singleFile = this._files.length == 1;
        var $slots = $(slotsTemplate({ model: { singleFile: singleFile } }));

        if (singleFile) {
            _.each(ViewData.attachmentSlots, function (slot) {
                var filled = filledById[slot.Id];
                var $btn;
                if (filled) {
                    $btn = $(slotBtnTemplate({ model: { caption: slot.Name + ": " + filled.Attachment.FileName } }));
                    $btn.prop("disabled", true);
                } else {
                    $btn = $(slotBtnTemplate({ model: { caption: slot.Name } }));
                    $btn.on("click", function () {
                        me.data.formData = { slot: slot.Id };
                        me.data.submit();
                        //TODO: fill slot on successful upload
                    });
                }

                $slots.find(".slots-necessity-" + slot.Necessity).show().find(".btn-container").append($btn);
            });
        }

        $slots.find(".slot-general").on("click", function () {
            me.data.submit();
        });

        this.$(".slots").html($slots);
        
        return this;
    };
    UploadModal.prototype.closeButtonText = function (text) {
        this.$(".close-button", function () {
            this.text("Cancelar");
        });
        return this;
    }

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
                            .closeButtonText("Cancelar")
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
