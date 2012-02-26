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
                IsInterviewNote: false
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
            BirthDate: { controlLink: "Date", valueToViewText: formatLongDateWithYears },
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
                    template: _.template('<span data-bind="date"></span> (<span data-bind="NoteType"></span>) <span data-bind="text"></span> '),
                    items:
                    [
                        { controlLink: "Date", name: "date", field: "RealDate" },
                        { controlLink: "Markdown", name: "text", field: "Note" },
                        { controlLink: "Options", name: "NoteType", field: "NoteType", options: [{ value: 0, text: "Nota Genérica" }, { value: 1, text: "Nota de entrevista" }, { value: 2, text: "Nota de entrevistsa técnica"}] }
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
        },
        events: {
            "click .saveApplicant": "saveApplicant",
            "click .reloadApplicant": "reloadApplicant",
            "click .editionNormal": "editionNormal",
            "click .editionReadonly": "editionReadonly",
            "click .editionFullEdit": "editionFullEdit"
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
        editionNormal: function () { this.dataBinder.editionMode("normal"); },
        editionReadonly: function () { this.dataBinder.editionMode("readonly"); },
        editionFullEdit: function () { this.dataBinder.editionMode("full-edit"); }
    });

}).call(this);


$(function () {
    App.appView = new App.EditApplicantAppView({
        el: $("#EditApplicantApp"),
        model: new App.Applicant(ViewData.applicant)
    });
});
