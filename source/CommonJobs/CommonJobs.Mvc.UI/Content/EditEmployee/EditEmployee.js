/// <reference path="../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../Scripts/underscore.js" />
/// <reference path="../../Scripts/backbone.js" />
/// <reference path="../../Scripts/url-generator.js" />

(function () {
    var App = this.App = {};

    App.Note = Backbone.Model.extend({
        defaults: function () {
            return {
                RealDate: new Date().toJSON(),
                //TODO: move RegisterDate to a better place
                RegisterDate: new Date().toJSON(),
                Note: ""
            }
        }
    });

    App.Notes = Backbone.Collection.extend({
        model: App.Note
    });

    App.Employee = Backbone.Model.extend({
        defaults: function () {
            return {
            }
        },
        initCollectionField: function (fieldName) {
            this.set(fieldName, new App.Notes(this.get(fieldName)));
            this.get(fieldName).on("add remove reset change", function () { this.trigger("change"); }, this);
            this.get(fieldName).parentModel = this;
        },
        updateSalaries: function () {
            var sortedSalaries = _.chain(this.get("SalaryChanges").toJSON()).sortBy(function (x) { return x.RealDate; }).pluck("Salary");
            this.set("CurrentSalary", sortedSalaries.last().value());
            this.set("InitialSalary", sortedSalaries.first().value());
        },
        updateVacations: function () {
            var totalDays = _.chain(this.get("Vacations").models)
                .map(function (v) {
                    var millisecondsPerDay = 1000 * 60 * 60 * 24;

                    var from = Date.parse(v.get('From'));
                    var to = Date.parse(v.get('To'));

                    var dayDifference = (to - from) / millisecondsPerDay;
                    dayDifference = Math.floor(dayDifference);
                    v.set('TotalDays', dayDifference);
                    return dayDifference;
                })
                .reduce(function (intermediateState, days) {
                    return intermediateState + days
                }, 0)
                .value();
            this.set('VacationsTotalDays', totalDays);
        },
        initialize: function () {
            this.initCollectionField("Notes");

            this.initCollectionField("SalaryChanges");
            this.get("SalaryChanges").on("add remove reset change", this.updateSalaries, this);

            this.initCollectionField("Vacations");
            this.get("Vacations").on("add remove reset change", this.updateVacations, this);
        }
    });

    var formatLongDateWithYears = function (value) {
        var date = new Date(value);
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
        return value + " días";
    };

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
                           + '    <button class="view-editable-clear">-</button>'
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
                            { controlLink: "MultilineText", name: "text", field: "Note" }
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
                Vacations:
                {
                    controlLink: "Collection",
                    item: {
                        controlLink: "Compound",
                        template: _.template('Periodo <span data-bind="Period"></span>: <span data-bind="From"></span> a <span data-bind="To"></span> (<span data-bind="TotalDays"></span>)'),
                        items:
                        [
                            { controlLink: "Int", name: "Period", field: "Period" },
                            { controlLink: "Date", name: "From", field: "From" },
                            { controlLink: "Date", name: "To", field: "To" },
                            { controlLink: "ReadOnlyText", name: "TotalDays", field: "TotalDays", valueToContent: formatTotalDays }
                        ]
                    }
                },
                VacationsTotalDays: { controlLink: "ReadOnlyText", valueToContent: function (value) { return "(" + formatTotalDays(value) + " en total)"; } }
            }
    });

    App.EditEmployeeAppView = Backbone.View.extend({
        setModel: function (model) {
            this.model = model;
            this.dataBinder.setModel(model);
        },
        initialize: function () {
            this.dataBinder = new App.EditEmployeeAppViewDataBinder({ el: this.el, model: this.model });
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
