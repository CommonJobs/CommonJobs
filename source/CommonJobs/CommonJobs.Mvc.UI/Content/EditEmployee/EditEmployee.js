/// <reference path="../../Scripts/jquery-1.7.2-vsdoc.js" />
/// <reference path="../../Scripts/underscore.js" />
/// <reference path="../../Scripts/backbone.js" />
/// <reference path="../../Scripts/url-generator.js" />

(function () {
    var dragAndDrop = new DragAndDrop();

    //Attachments utilities
    var getEmployeeAttachmentsBySlot = function (employee) {
        var filledById = {};
        if (employee) {
            _.chain(employee.get('AttachmentsBySlot').models)
            .filter(function (filledItem) {
                return !!filledItem.get('Attachment');
            })
            .each(function (filledItem) {
                filledById[filledItem.get('SlotId')] = filledItem;
            });
        }
        return filledById;
    };

    //Extend UploadModal
    var slotBtnTemplate = null;
    var slotsTemplate = null;
    var previousInit = UploadModal.prototype._init;
    UploadModal.prototype._init = function ($modal) {
        slotBtnTemplate = _.template($("#slot-button-template").text());
        slotsTemplate = _.template($("#available-slots-template").text());

        _.bind(previousInit, this)($modal);
        this.$(".slots").empty();
        this.closeButtonText("Cerrar");
    };
    UploadModal.prototype.drawSlots = function ($el, employee) {
        var me = this;

        var filledById = getEmployeeAttachmentsBySlot(employee);

        var singleFile = this._files.length == 1;
        var $slots = $(slotsTemplate({ model: { singleFile: singleFile} }));

        if (singleFile) {
            var btns = {};

            _.each(ViewData.attachmentSlots, function (slot) {
                var filled = filledById[slot.Id];
                var $btn;
                if (filled && filled.get('Attachment')) {
                    $btn = $(slotBtnTemplate({ model: { caption: slot.Name + ": " + filled.get('Attachment').FileName} }));
                    $btn.prop("disabled", true);
                } else {
                    $btn = $(slotBtnTemplate({ model: { caption: slot.Name} }));
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
                while ($btn = btns[key].shift()) {
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
    UploadModal.prototype.personDetail = function (employee, $el) {
        //employee: backbone model object
        var result = this
        // remove old photo
            .$("img.uploadPicture", function () { this.remove(); });

        var photo = employee.get('Photo');

        if (photo && photo.photo.Thumbnail && photo.Thumbnail.Id) {
            result = result.$(".modal-header", function () {
                this.prepend($('<img />')
                    .attr("src", urlGenerator.action("Get", "Attachments", photo.Thumbnail.Id, { returnName: false }))
                    .addClass("uploadPicture")
                );
            });
        }

        return result.text(".person-name", employee.get('LastName') + ", " + employee.get('FirstName'))
    };

    var prepareAttachmentZone = function (dropZone, model) {
        dragAndDrop.prepareFileDropzone("#EditApp", {
            input: this.$(".dropzoneinput"),
            url: urlGenerator.action("Post", "Attachments", model.get('Id')),
            add: function (e, data, $el) {
                if ((data.result && data.result.attachments && data.result.attachments.length)
                    || (data.files && data.files.length)) {
                        new UploadModal($('#generic-modal'))
                        .personDetail(model, $el)
                        .title("Adjuntar Archivos")
                        .files(data)
                        .drawSlots($el, model)
                        .closeButtonText("Cancelar")
                        .modal();
                }
            },
            done: function (e, data, $el) {
                var modal = new UploadModal($('#generic-modal'))
                    .personDetail(model, $el)
                    .files(data);

                if (data.formData.slot) {
                    modal.title("Archivos subidos (agregados a slot)")
                    var slots = model.get("AttachmentsBySlot");
                    var filtered = slots.filter(function (slot) { return slot.get("SlotId") == data.formData.slot; })
                    if (filtered.length > 0) {
                        filtered[0].set("Date", new Date().toJSON());
                        filtered[0].set("Attachment", data.result.attachments[0]);
                    } else {
                        slots.add({
                            SlotId: data.formData.slot,
                            Attachment: data.result.attachments[0]
                        });
                    }
                } else {
                    modal.title("Archivos subidos (agregados a las notas)")
                    var notes = model.get("Notes");
                    _.each(data.result.attachments, function (attachment) {
                        notes.add({
                            Note: "QuickAttachment!",
                            Attachment: attachment
                        });
                    });
                }

                modal.modal();
            },
            fail: function (e, data, $el) {
                new UploadModal($('#generic-modal'))
                    .personDetail(model, $el)
                    .title("Error subiendo archivos")
                    .error()
                    .files(data)
                    .modal();
            }
        });
    };

    var App = this.App = {};

    App.Note = Backbone.Model.extend({
        defaults: function () {
            return {
                RealDate: new Date().toJSON(),
                //TODO: move RegisterDate to a better place
                RegisterDate: new Date().toJSON(),
                Note: "",
                Attachment: null
            }
        }
    });

    App.Notes = Backbone.Collection.extend({
        model: App.Note
    });

    var millisecondsPerDay = 1000 * 60 * 60 * 24;
    var dateToCleanMilliseconds = function (str) {
        var year = str.substring(0, 4);
        var month = str.substring(5, 7);
        var day = str.substring(8, 10);
        var str = year + "-" + month + "-" + day;
        return Date.parse(str);
    };

    App.Vacation = Backbone.Model.extend({
        defaults: {
            Period: "",
            From: "",
            To: ""
        },
        getDays: function () {
            var from = dateToCleanMilliseconds(this.get('From'));
            var to = dateToCleanMilliseconds(this.get('To'));
            var dayDifference = (to - from) / millisecondsPerDay;
            dayDifference = 1 + Math.floor(dayDifference);
            return dayDifference > 0 ? dayDifference : 0;
        }
    });

    App.Vacations = Backbone.Collection.extend({
        model: App.Vacation,
        getTotalDays: function () {
            return this.chain()
                .map(function (x) { return x.getDays(); })
                .reduce(function (intermediateState, days) { return intermediateState + days; }, 0)
                .value();
        }
    });

    App.Employee = Backbone.Model.extend({
        defaults: function () {
            return {
            }
        },
        initCollectionField: function (fieldName, fieldType) {
            fieldType = fieldType || Backbone.Collection;
            this.set(fieldName, new fieldType(this.get(fieldName)));
            this.get(fieldName).on("add remove reset change", function () { this.trigger("change"); }, this);
            this.get(fieldName).parentModel = this;
        },
        updateSalaries: function () {
            var sortedSalaries = _.chain(this.get("SalaryChanges").toJSON()).sortBy(function (x) { return x.RealDate; }).pluck("Salary");
            this.set("CurrentSalary", sortedSalaries.last().value());
            this.set("InitialSalary", sortedSalaries.first().value());
        },
        initialize: function () {
            this.initCollectionField("Notes", App.Notes);
            this.initCollectionField("Certifications");
            this.initCollectionField("AttachmentsBySlot");

            //TODO: move related logic to App.Salary and App.Salaries models
            this.initCollectionField("SalaryChanges", App.Notes);
            this.get("SalaryChanges").on("add remove reset change", this.updateSalaries, this);

            //TODO: move related logic to App.Vacation and App.Vacations models
            this.initCollectionField("Vacations", App.Vacations);
        }
    });

    var formatLongDateWithYears = function (value) {
        // date format: yyyy-mm-dd
        var year = value.substring(0, 4);
        var month = value.substring(5, 7);
        var day = value.substring(8, 10);

        var date = new Date(year, month - 1, day, 0, 0, 0, 0);
        var age = (new Date() - date) / 365.25 / 24 / 60 / 60 / 1000;
        var ageInt = parseInt(age);
        var casi = "";
        if (age - ageInt > 0.7) {
            casi = "casi ";
            ageInt++;
        }
        var tiempo = ageInt < 1
                        ? "menos de un año"
                        : ageInt == 1
                            ? casi + "un año"
                            : casi + ageInt + " años";
        return Globalize.format(date, "d' de 'MMMM' de 'yyyy") + " (" + tiempo + ")";
    };

    var formatSalary = function (value) {
        return "$ " + value;
    };

    var formatTotalDays = function (value) {
        if (value == 1)
            return "1 día";
        else if (value > 1)
            return value + " días";
        else
            return " - ";
    };

    Nervoustissue.UILinking.CjVacation = Nervoustissue.UILinking.Compound.extend({
        template: _.template(
        '<td data-bind="Period" class="vacations-period-column"></td><td data-bind="From" class="vacations-from-column"></td><td data-bind="To" class="vacations-to-column"></td><td class="vacation-days"></td>'
        ),
        items:
        [
            { controlLink: "Int", name: "Period", field: "Period" },
            { controlLink: "Date", name: "From", field: "From" },
            { controlLink: "Date", name: "To", field: "To" },
        ],
        refresh: function () {
            this.$(".vacation-days").text(formatTotalDays(this.model.getDays()));
        }
    });

    Nervoustissue.UILinking.CjVacationList = Nervoustissue.UILinking.Collection.extend({
        item: { controlLink: "CjVacation" },
        template: _.template(
            '<table>' +
            '<thead><tr><th></th><th><table><tr><th class="vacations-period-column">Periodo</th><th class="vacations-from-column">Desde</th><th class="vacations-to-column">Hasta</th><th class="vacation-days">Total</th></tr></table></th></tr></thead>' +
            '<tbody class="list-editable">' +
            '</tbody>' +
            '<tfoot><tr><td><button class="add-button">+</button></td><td><table><tr><td class="vacations-period-column"></td><td class="vacations-from-column"></td><td class="vacations-to-column"></td><td class="vacation-items-total vacation-days"></td></tr></table></td></tr></tfoot>' +
            '</table>'
            ),
        subtemplate: _.template(
            '<tr>' +
            '<td><button class="remove-button">&#x2717;</button></td>' +
            '<td><table><tr class="editable-field" data-bind="item"><td></td></tr></table></td>' +
            '</tr>'),
        refresh: function () {
            var total = formatTotalDays(this.linkedData.read().getTotalDays());
            this.$(".vacation-items-total").text(total);
        }
    });


    Nervoustissue.UILinking.CjEmployeePicture = Nervoustissue.UILinking.Attachment.extend({
        //TODO: generalize it
        allowedExtensions: ["jpg", "jpeg", "gif", "png"],
        accept: "image/*",
        uploadUrl: function () { return urlGenerator.action("SavePhoto", "Employees", this.model.get('Id')); },
        cropUrl: function (value, x, y, width, height) { return urlGenerator.action("CropImageAttachment", "Attachments", value.Original.Id, { x: parseInt(x), y: parseInt(y), width: parseInt(width), height: parseInt(height) }); },
        attachedUrl: function (value) { return urlGenerator.action("Get", "Attachments", value.Original.Id, { returnName: false }) },
        template: _.template('<div class="upload-element">'
                           + '    <img class="view-editable-empty" width="100" height="100" alt="No Photo" src="' + urlGenerator.content("Images/NoPicture.png") + '" title="No Photo" style="display:none"/>'
                           + '</div>'
                           + '<span class="view-attached" style="display: none;">'
                           + '    <div class="view-editable-content"></div>'
                           + '    <button class="view-editable-clear">&#x2717;</button>'
                           + '</span>'
                           + '<div class="cropDialog">'
                           + '    <div class="originalImage"></div>'
                           + '    <div class="croppedImage"></div>'
                           + '    <input type="hidden" class="cropX" />'
                           + '    <input type="hidden" class="cropY" />'
                           + '    <input type="hidden" class="cropWidth" />'
                           + '    <input type="hidden" class="cropHeight" />'
                           + '</div>'
                           ),
        valueToContent: function (value) {
            if (!value) { return ""; }
            return $("<a />")
                .attr("href", this.attachedUrl(value))
                .attr("target", "_blank")
                .addClass("photoLink")
                .append($("<img />").attr("src", urlGenerator.action("Get", "Attachments", value.Thumbnail.Id, { returnName: false }))
                .attr("width", "100").attr("height", "100"));
        },
        cropImage: function (attachment, x, y, w, h, callback) {
            var me = this;
            var cropUrl = me.cropUrl(attachment, x, y, w, h);
            $.ajax({
                url: cropUrl,
                success: function (data) { callback(data); }
            });
        },
        uploadFinished: function (attachment) {
            var me = this;
            var $cropDialog = this.$('.cropDialog');
            var url = this.attachedUrl(attachment);
            $cropDialog.find('.originalImage').append($('<img />').attr('src', url)).css({
                float: "left"
            });
            var $cropImage = $cropDialog.find('.croppedImage');
            $cropImage.append($('<img />').attr('src', url)).css({
                float: "left",
                width: 100,
                height: 100,
                overflow: 'hidden',
                'margin-left': 5
            });
            $cropDialog.dialog({
                buttons: {
                    "Ok": function () {
                        var x = $cropDialog.find('input.cropX').val();
                        var y = $cropDialog.find('input.cropY').val();
                        var w = $cropDialog.find('input.cropWidth').val();
                        var h = $cropDialog.find('input.cropHeight').val();
                        me.cropImage(attachment, x, y, w, h, function (newImage) {
                            $cropDialog.dialog("close");
                            me.linkedData.write(newImage);
                            me.refreshView();
                        });
                    },
                    "No cortar": function () {
                        $(this).dialog("close");
                    }
                },
                draggable: true,
                maxHeight: 500,
                modal: true,
                resizable: false,
                title: "Cortar imagen",
                width: '80%'
            });

            var $originalImage = $cropDialog.find('.originalImage img');
            var $preview = $cropDialog.find('.croppedImage img');

            $originalImage.Jcrop({
                aspectRatio: 1,
                onChange: function (coords) {
                    if (parseInt(coords.w) > 0) {
                        $cropDialog.find('input.cropX').val(coords.x);
                        $cropDialog.find('input.cropY').val(coords.y);
                        $cropDialog.find('input.cropWidth').val(coords.w);
                        $cropDialog.find('input.cropHeight').val(coords.h);

                        var rx = 100 / coords.w;
                        var ry = 100 / coords.h;

                        $preview.css({
                            width: Math.round(rx * parseInt($originalImage.css("width"))),
                            height: Math.round(ry * parseInt($originalImage.css("height"))),
                            marginLeft: '-' + Math.round(rx * coords.x) + 'px',
                            marginTop: '-' + Math.round(ry * coords.y) + 'px'
                        });
                    }
                }
            });
        }
    });

    Nervoustissue.UILinking.CjEmployeeAttachment = Nervoustissue.UILinking.Attachment.extend({
        template: _.template('<span class="upload-element">'
                                   + '    <span class="view-editable-empty">Sin archivo adjunto</span>'
                                   + '</span>'
                                   + '<span class="view-attached" style="display: none;">'
                                   + '    Adjunto: <span class="view-editable-content"></span>'
                                   + '<button class="view-editable-clear">-</button>'
                                   + '</span>'),
        uploadUrl: function () { return urlGenerator.action("Post", "Attachments", /* TODO */this.model.collection.parentModel.get('Id')); },
        attachedUrl: function (value) { return urlGenerator.action("Get", "Attachments", value.Id); }
    });

    App.EditEmployeeAppViewDataBinder = Nervoustissue.FormBinder.extend({
        dataBindings:
            {
                fullName:
                {
                    controlLink: "Text",
                    dataLink: "FullName",
                    lastNameField: "LastName",
                    firstNameField: "FirstName"
                },
                Photo: { controlLink: "CjEmployeePicture" },
                IsGraduated: { controlLink: "Options", options: [{ value: false, text: "No recibido" }, { value: true, text: "Recibido"}] },
                BirthDate: { controlLink: "Date", valueToContent: formatLongDateWithYears },
                MaritalStatus: { controlLink: "Options", options: [{ value: 0, text: "Soltero" }, { value: 1, text: "Casado" }, { value: 2, text: "Divorciado"}] },
                HiringDate: { controlLink: "Date", valueToContent: formatLongDateWithYears },
                WorkingHours: { controlLink: "Int" },
                Lunch: { controlLink: "Options", options: [{ value: false, text: "No" }, { value: true, text: "Si"}] },
                Notes:
                {
                    controlLink: "Collection",
                    item:
                    {
                        controlLink: "Compound",
                        template: _.template('<span data-bind="date"></span> | <span data-bind="attachment"></span> <div data-bind="text"></div>'),
                        items:
                        [
                            { controlLink: "CjEmployeeAttachment", name: "attachment", field: "Attachment" },
                            { controlLink: "Date", name: "date", field: "RealDate" },
                            { controlLink: "Markdown", name: "text", field: "Note" }
                        ]
                    }
                },
                SalaryChanges:
                {
                    controlLink: "Collection",
                    item: {
                        controlLink: "Compound",
                        template: _.template('<span data-bind="date"></span> | Sueldo: <span data-bind="salary"></span><br /> Nota: <span data-bind="note"></span>'),
                        items:
                        [
                            { controlLink: "Date", name: "date", field: "RealDate" },
                            { controlLink: "Int", name: "salary", field: "Salary", valueToContent: formatSalary },
                            { controlLink: "Text", name: "note", field: "Note" }
                        ]
                    }
                },
                CurrentSalary: { controlLink: "ReadOnlyText", valueToContent: formatSalary },
                InitialSalary: { controlLink: "ReadOnlyText", valueToContent: formatSalary },
                Vacations: { controlLink: "CjVacationList" },
                Certifications:
                {
                    controlLink: "Collection",
                    item: { controlLink: "Text", name: "description", field: "Description" }
                }
            }
    });

    App.EmployeeSlotsView = Backbone.View.extend({
        events: {
            "click .file-delete": "deleteFile"
        },
        deleteFile: function () {

        },
        initialize: function () {
            this.emptySlotTemplate = _.template($("#empty-slot-template").html());
            this.regularSlotTemplate = _.template($("#regular-slot-template").html());

            this.model.attachmentsBySlot.on('add remove', this.render, this);

            this.render();
        },
        render: function () {
            //this.model.slots -- normal object
            //this.model.attachmentsBySlot -- backbone model
            this.$el.html(null);
            var collection = this.model.attachmentsBySlot;
            var view = this;
            var bindDeleteEvent = function (renderedTemplate, attachment) {
                return $(renderedTemplate).on("click", ".file-delete", function () {
                    collection.remove(attachment);
                    view.render();
                    return false;
                });
            };

            var $attachmentSlotsDiv = $("<div/>").addClass("attachment-slots");
            // for each slot
            _.each(this.model.slots, function (slot) {
                // find if any attachment is associated to it
                var attachment = _.find(collection.models, function (a) {
                    return a.get('SlotId') == slot.Id;
                });
                var isEmpty = !attachment;
                var templateToUse = isEmpty ? this.emptySlotTemplate : this.regularSlotTemplate;

                // add proper template for slot
                var newRender = templateToUse({
                    SlotName: slot.Name,
                    FileName: isEmpty ? null : attachment.get('Attachment').FileName,
                    Url: isEmpty ? null : urlGenerator.action("Get", "Attachments", attachment.get('Attachment').Id)
                });

                $attachmentSlotsDiv.append(bindDeleteEvent(newRender, attachment));
            }, this);
            $attachmentSlotsDiv.children().last().addClass("last");

            // get the rest of the attachments, not present in any slot
            var attachmentsNotInSlots = _.filter(collection.models, function (a) {
                return !a.get('SlotId');
            });

            this.$el.append($attachmentSlotsDiv);
        }
    });

    App.EditEmployeeAppView = Backbone.View.extend({
        prepareSlots: function () {
            this.attachmentSlotView = new App.EmployeeSlotsView({
                el: this.$(".attachment-container"),
                model: {
                    attachmentsBySlot: this.model.get('AttachmentsBySlot'),
                    slots: ViewData.attachmentSlots
                }
            });
            prepareAttachmentZone($(this.el).find(".files-data"), this.model);
        },
        setModel: function (model) {
            this.model = model;
            this.dataBinder.setModel(model);
            this.prepareSlots();
        },
        initialize: function () {
            this.dataBinder = new App.EditEmployeeAppViewDataBinder({ el: this.el, model: this.model });
            this.prepareSlots();
        },
        events: {
            "click .saveEmployee": "saveEmployee",
            "click .reloadEmployee": "reloadEmployee",
            "click .editionNormal": "editionNormal",
            "click .editionReadonly": "editionReadonly",
            "click .editionFullEdit": "editionFullEdit",
            "click .deleteEmployee": "deleteEmployee",
            "click .confidential-info-title": "toggleConfidentialVisibility"
        },
        saveEmployee: function () {
            var me = this;
            $.ajax({
                url: urlGenerator.action("Post", "Employees"),
                type: 'POST',
                dataType: 'json',
                data: JSON.stringify(App.appView.model.toJSON()),
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    me.editionNormal();
                    me.setModel(new App.Employee(result));
                }
            });
        },
        reloadEmployee: function () {
            var me = this;
            $.ajax({
                url: urlGenerator.action("Get", "Employees"),
                type: 'GET',
                dataType: 'json',
                data: { id: ViewData.employee.Id },
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    me.editionNormal();
                    me.setModel(new App.Employee(result));
                }
            });
        },
        deleteEmployee: function () {
            if (confirm("¿Está seguro de que desea eliminar este empleado?")) {
                window.location = urlGenerator.action("Delete", "Employees", this.model.get('Id'));
            }
        },
        editionNormal: function () {
            this.dataBinder.editionMode("normal");
            this.$el.removeClass("edition-readonly edition-full-edit");
            this.$el.addClass("edition-normal");
        },
        editionReadonly: function () {
            this.dataBinder.editionMode("readonly");
            this.$el.removeClass("edition-normal edition-full-edit");
            this.$el.addClass("edition-readonly");
        },
        editionFullEdit: function () {
            this.dataBinder.editionMode("full-edit");
            this.$el.removeClass("edition-readonly edition-normal");
            this.$el.addClass("edition-full-edit");
        },
        toggleConfidentialVisibility: function (event) {
            $(event.target).parent().next(".confidential-info-data").toggle();
        }
    });

}).call(this);


$(function () {
    App.appView = new App.EditEmployeeAppView({
        el: $("#EditApp"),
        model: new App.Employee(ViewData.employee)
    });
});
