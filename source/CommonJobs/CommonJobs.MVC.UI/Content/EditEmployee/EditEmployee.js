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
        },
        updateSalaries: function () {
            var sortedSalaries = _.chain(this.get("SalaryChanges").toJSON()).sortBy(function (x) { return x.RealDate; }).pluck("Salary");
            this.set("CurrentSalary", sortedSalaries.last().value());
            this.set("InitialSalary", sortedSalaries.first().value());
        },
        initialize: function () {
            this.initCollectionField("Notes");
            this.initCollectionField("SalaryChanges");
            this.get("SalaryChanges").on("add remove reset change", this.updateSalaries, this);
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
                //Photo: { control: "picture" },
                IsGraduated: { controlLink: "Options", options: [{ value: false, text: "No recibido" }, { value: true, text: "Recibido"}] },
                BirthDate: { controlLink: "Date", valueToViewText: formatLongDateWithYears },
                MaritalStatus: { controlLink: "Options", options: [{ value: 0, text: "Soltero" }, { value: 1, text: "Casado" }, { value: 2, text: "Divorciado"}] },
                HiringDate: { controlLink: "Date", valueToViewText: formatLongDateWithYears },
                WorkingHours: { controlLink: "Int" },
                Lunch: { controlLink: "Options", options: [{ value: false, text: "No" }, { value: true, text: "Si"}] },
                Notes:
                {
                    controlLink: "Collection",
                    item:
                    {
                        controlLink: "Compound",
                        template: _.template('<span data-bind="date"></span> | <span data-bind="text"></span>'),
                        items:
                        [
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
                        template: _.template('<span data-bind="date"></span> | Salary: <span data-bind="salary"></span> | Note: <span data-bind="note"></span>'),
                        items:
                        [
                            { controlLink: "Date", name: "date", field: "RealDate" },
                            { controlLink: "Int", name: "salary", field: "Salary", valueToViewText: formatSalary },
                            { controlLink: "Text", name: "note", field: "Note" }
                        ]
                    }
                },
                CurrentSalary: { controlLink: "ReadOnlyText", valueToViewText: formatSalary },
                InitialSalary: { controlLink: "ReadOnlyText", valueToViewText: formatSalary }
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
            "click .editionFullEdit": "editionFullEdit"
        },
        saveEmployee: function () {
            var me = this;
            $.ajax({
                url: ViewData.saveEmployeeUrl,
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
                url: ViewData.getEmployeeUrl,
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
        editionNormal: function () { this.dataBinder.editionMode("normal"); },
        editionReadonly: function () { this.dataBinder.editionMode("readonly"); },
        editionFullEdit: function () { this.dataBinder.editionMode("full-edit"); }
    });

}).call(this);


$(function () {
    App.appView = new App.EditEmployeeAppView({
        el: $("#EditEmployeeApp"),
        model: new App.Employee(ViewData.employee)
    });
});
