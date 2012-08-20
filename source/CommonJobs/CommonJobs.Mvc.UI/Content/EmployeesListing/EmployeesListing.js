/// <reference path="../../Scripts/underscore.js" />
/// <reference path="../../Scripts/jquery-1.7.1-vsdoc.js" />
$(function () {
    var dragAndDrop = new DragAndDrop();

    //Attachments utilities
    var getEmployeeAttachmentsBySlot = function (employee) {
        var filledById = {};
        if (employee) {
            _.chain(employee.AttachmentsBySlot)
            .filter(function (filledItem) {
                return !!filledItem.Attachment;
            })
            .each(function (filledItem) {
                filledById[filledItem.SlotId] = filledItem;
            });
        }
        return filledById;
    };

    //Extend UploadModal
    var slotBtnTemplate = _.template($("#slot-button-template").text());
    var slotsTemplate = _.template($("#available-slots-template").text());
    var previousInit = UploadModal.prototype._init;
    UploadModal.prototype._init = function ($modal) {
        _.bind(previousInit, this)($modal);
        this.$(".slots").empty();
        this.closeButtonText("Cerrar");
    };
    UploadModal.prototype.drawSlots = function ($el, employee) {
        var me = this;

        var filledById = getEmployeeAttachmentsBySlot(employee);
        
        var singleFile = this._files.length == 1;
        var $slots = $(slotsTemplate({ model: { singleFile: singleFile } }));

        if (singleFile) {
            var btns = {};

            _.each(ViewData.attachmentSlots, function (slot) {
                var filled = filledById[slot.Id];
                var $btn;
                if (filled && filled.Attachment) {
                    $btn = $(slotBtnTemplate({ model: { caption: slot.Name + ": " + filled.Attachment.FileName } }));
                    $btn.prop("disabled", true);
                } else {
                    $btn = $(slotBtnTemplate({ model: { caption: slot.Name } }));
                    $btn.on("click", function () {
                        me.data.formData = { slot: slot.Id };
                        me.data.submit();
                    });
                }
                
                var key = ".slots-necessity-" + slot.Necessity;
                if (!btns[key])
                    btns[key] = [];
                btns[key].push($btn);
            });

            for (var key in btns) {
                var group = $slots.find(key);
                group.show();
                var title = group.find("h5");
                var $btn;
                while($btn = btns[key].shift()) { 
                    title.after($btn);
                }
            }
        }

        $slots.find(".slot-general").on("click", function () {
            me.data.submit();
        });

        this.$(".slots").html($slots);
        
        return this;
    };
    UploadModal.prototype.closeButtonText = function (text) {
        this.$(".close-button", function () {
            this.text(text);
        });
        return this;
    }


    var needAttachmentsMarkTemplate = _.template($("#need-attachments-mark-template").text());
    var markEmployeesThatNeedsAttachments = function ($card, employee) {
        var filledById = getEmployeeAttachmentsBySlot(employee);
        var missedSlots = _.filter(ViewData.attachmentSlots, function (slot) {
            return slot.Necessity == 2 && !filledById[slot.Id];
        });

        $card.removeClass("need-attachments");
        $card.find(".need-attachments-element").remove();

        if (missedSlots.length > 0) {
            var names = _.map(missedSlots, function (slot) { return "`" + slot.Name + "`"; });
            var message = "Slots requeridos: " + names.join(", ");
            $card.prepend(needAttachmentsMarkTemplate({ model: { message: message } }));
            $card.addClass("need-attachments");
        }
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
        prepareNewCard: function ($card) {
            dragAndDrop.prepareFileDropzone($card, {
                add: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .person($el)
                        .title("Adjuntar Archivos")
                        .text(".person-name", "Crear empleado con adjuntos")
                        .files(data)
                        .drawSlots($el, null)
                        .closeButtonText("Cancelar")
                        .modal();
                },
                done: function (e, data) {
                    window.location = data.result.editUrl;
                },
                fail: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .person($el)
                        .text(".person-name", "Crear empleado con adjuntos")
                        .title("Error subiendo archivos")
                        .error()
                        .files(data)
                        .modal();
                }
            });
        },
        prepareResultCard: function ($card, item) {
            markEmployeesThatNeedsAttachments($card, item);
            dragAndDrop.prepareFileDropzone($card, {
                add: function (e, data, $el) {               
                    if ($el.hasClass("item-card")) {
                        new UploadModal($('#generic-modal'))
                            .person($el)
                            .title("Adjuntar Archivos")
                            .files(data)
                            .drawSlots($el, item)
                            .show(".detail-link")
                            .closeButtonText("Cancelar")
                            .modal();
                    }
                },
                done: function (e, data, $el) {
                    if (data.result.added) {
                        if (item.AttachmentsBySlot == null)
                            item.AttachmentsBySlot = [];
                        item.AttachmentsBySlot.push(data.result.added);
                    }
                    markEmployeesThatNeedsAttachments($card, item);
                    new UploadModal($('#generic-modal'))
                        .person($el)
                        .title("Archivos subidos")
                        .files(data)
                        .show(".detail-link")
                        .modal();
                },
                fail: function (e, data, $el) {
                    new UploadModal($('#generic-modal'))
                        .person($el)
                        .title("Error subiendo archivos")
                        .error()
                        .files(data)
                        .show(".detail-link")
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
