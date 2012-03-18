/// <reference path="../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../Scripts/underscore.js" />
/// <reference path="../../Scripts/backbone.js" />
(function () {
    var App = this.App = {};

    App.Note = Backbone.Model.extend({
        defaults: function () {
            return {
                RealDate: new Date().toJSON(),
                //TODO: move RegisterDate to a better place
                RegisterDate: new Date().toJSON(),
                Note: "",
                NoteType: 0,
                Attachment: null
            }
        }
    });

    App.Notes = Backbone.Collection.extend({
        model: App.Note
    });

    App.Applicant = Backbone.Model.extend({
        defaults: function () {
            return {
            }
        },
        initCollectionField: function (fieldName) {
            this.set(fieldName, new App.Notes(this.get(fieldName)));
            this.get(fieldName).on("add remove reset change", function () { this.trigger("change"); }, this);
            this.get(fieldName).parentModel = this;
        },
        initialize: function () {
            this.initCollectionField("Notes");
            this.initCollectionField("CompanyHistory");
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

    Nervoustissue.UILinking.CjApplicantPicture = Nervoustissue.UILinking.Attachment.extend({
        //TODO: generalize it
        allowedExtensions: ["jpg", "jpeg", "gif", "png"],
        accept: "image/*",
        uploadUrl: function () { return "/Applicants/SavePhoto/" + this.model.get('Id'); },
        attachedUrl: function (value) { return "/Attachments/Get/" + value.Original.Id + "?returnName=false"; },
        template: _.template('<div class="upload-element">'
                           + '    <img class="view-editable-empty" alt="No Photo" src="/Content/Images/NoPicture.png" title="No Photo" style="display:none"/>'
                           + '</div>'
                           + '<span class="view-attached" style="display: none;">'
                           + '    <div class="view-editable-content"></div>'
                           + '    <button class="view-editable-clear">-</button>'
                           + '</span>'),
        valueToContent: function (value) {
            if (!value) { return ""; }
            return $("<a />")
                .attr("href", this.attachedUrl(value))
                .attr("target", "_blank")
                .append($("<img />").attr("src", "/Attachments/Get/" + value.Thumbnail.Id + "?returnName=false")
                );
        }
    });

    Nervoustissue.UILinking.CjApplicantAttachment = Nervoustissue.UILinking.Attachment.extend({
        template: _.template('<span class="upload-element">'
                                   + '    <span class="view-editable-empty">Sin archivo adjunto</span>'
                                   + '</span>'
                                   + '<span class="view-attached" style="display: none;">'
                                   + '    Adjunto: <span class="view-editable-content"></span>'
                                   + '<button class="view-editable-clear">-</button>'
                                   + '</span>'),
        uploadUrl: function () { return "/Attachments/Post/" + /* TODO */this.model.collection.parentModel.get('Id'); },
        attachedUrl: function (value) { return "/Attachments/Get/" + value.Id; }
    });

    App.EditApplicantAppViewDataBinder = Nervoustissue.FormBinder.extend({
        dataBindings:
        {
            fullName:
            {
                controlLink: "Text",
                dataLink: "FullName",
                lastNameField: "LastName",
                firstNameField: "FirstName"
            },
            Photo: { controlLink: "CjApplicantPicture" },
            IsHighlighted:
            {
                controlLink: "Toggle",
                onTemplate: _.template('<img border="0" class="on" src="/Content/Images/GreenTick.png" alt="Resaltado" title="Resaltado">'),
                offTemplate: _.template('<img border="0" class="on" src="/Content/Images/GrayTick.png" alt="Resaltado" title="Resaltado">')
            },
            BirthDate: { controlLink: "Date", valueToContent: formatLongDateWithYears },
            MaritalStatus: { controlLink: "Options", options: [{ value: 0, text: "Soltero" }, { value: 1, text: "Casado" }, { value: 2, text: "Divorciado"}] },
            IsGraduated: { controlLink: "Options", options: [{ value: false, text: "No recibido" }, { value: true, text: "Recibido"}] },
            CompanyHistory:
            {
                controlLink: "Collection",
                item:
                {
                    controlLink: "Compound",
                    template: _.template('De <span data-bind="StartDate"></span> a <span data-bind="EndDate"></span>: <span data-bind="CompanyName"></span> (<span data-bind="IsCurrent"></span>)'),
                    items:
                    [
                        { controlLink: "Date", name: "StartDate", field: "StartDate" },
                        { controlLink: "Date", name: "EndDate", field: "EndDate" },
                        { controlLink: "Text", name: "CompanyName", field: "CompanyName" },
                        { controlLink: "Options", name: "IsCurrent", field: "IsCurrent", options: [{ value: false, text: "Anterior" }, { value: true, text: "Actual"}] }
                    ]
                }
            },
            Notes:
            {
                controlLink: "Collection",
                item:
                {
                    controlLink: "Compound",
                    template: _.template('<span data-bind="date"></span> (<span data-bind="NoteType"></span>) | <span data-bind="attachment"></span> <div data-bind="text"></div> '),
                    items:
                    [
                        { controlLink: "Date", name: "date", field: "RealDate" },
                        { controlLink: "CjApplicantAttachment", name: "attachment", field: "Attachment" },
                        { controlLink: "MultilineText", name: "text", field: "Note" },
                        { controlLink: "Options", name: "NoteType", field: "NoteType", options: [{ value: 0, text: "Nota Genérica" }, { value: 1, text: "Nota de entrevista" }, { value: 2, text: "Nota de entrevista técnica"}] }
                    ]
                }
            }
        }
    });


    App.EditApplicantAppView = Backbone.View.extend({
        setModel: function (model) {
            this.model = model;
            this.dataBinder.setModel(model);
        },
        initialize: function () {
            this.dataBinder = new App.EditApplicantAppViewDataBinder({ el: this.el, model: this.model });
            this.model.on("change:IsHighlighted", this.refreshHighlightedView, this);
            this.refreshHighlightedView();
        },
        events: {
            "click .saveApplicant": "saveApplicant",
            "click .reloadApplicant": "reloadApplicant",
            "click .editionNormal": "editionNormal",
            "click .editionReadonly": "editionReadonly",
            "click .editionFullEdit": "editionFullEdit",
            "click .deleteApplicant": "deleteApplicant"
        },
        saveApplicant: function () {
            var me = this;
            $.ajax({
                url: ViewData.saveApplicantUrl,
                type: 'POST',
                dataType: 'json',
                data: JSON.stringify(App.appView.model.toJSON()),
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    me.editionNormal();
                    me.setModel(new App.Applicant(result));
                }
            });
        },
        reloadApplicant: function () {
            var me = this;
            $.ajax({
                url: ViewData.getApplicantUrl,
                type: 'GET',
                dataType: 'json',
                data: { id: ViewData.applicant.Id },
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    me.editionNormal();
                    me.setModel(new App.Applicant(result));
                }
            });
        },
        deleteApplicant: function () {
            if (confirm("¿Está seguro de que desea eliminar este postulante?")) {
                window.location = ViewData.deleteApplicantUrl + this.model.get('Id');
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
        refreshHighlightedView: function () {
            //TODO how to avoid making references out of the view?
            var $highlight = $('body').add('.editing-bar', this.$el);
            if (this.model.get('IsHighlighted'))
                $highlight.addClass('highlighted');
            else
                $highlight.removeClass('highlighted');
        }
    });

}).call(this);


$(function () {
    App.appView = new App.EditApplicantAppView({
        el: $("#EditApp"),
        model: new App.Applicant(ViewData.applicant)
    });
});
