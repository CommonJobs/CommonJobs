/// <reference path="../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../Scripts/underscore.js" />
/// <reference path="../../Scripts/backbone.js" />
window.App = { };

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
    initCollectionField: function(fieldName) {
        this.set(fieldName, new App.Notes(this.get(fieldName)));
        this.get(fieldName).on("add remove reset change", function () { this.trigger("change"); }, this);
    },
    initialize: function () {
        this.initCollectionField("Notes");
        this.initCollectionField("SalaryChanges");
    }
});

App.EditEmployeeAppViewDataBinder = Nervoustissue.FormBinder.extend({
    dataBindings:
        {
            fullName:
            {
                controlLink: Nervoustissue.UILinking.Text,
                dataLink: Nervoustissue.DataLinking.FullName,
                lastNameField: "LastName",
                firstNameField: "FirstName"
            },
            //Photo: { control: "picture" },
            //TODO: change 0, 1 by false, true
            IsGraduated: { controlLink: Nervoustissue.UILinking.Options, options: [{ value: false, text: "No recibido" }, { value: true, text: "Recibido"}] },
            BirthDate: { controlLink: Nervoustissue.UILinking.Date },
            MaritalStatus: { controlLink: Nervoustissue.UILinking.Options, options: [{ value: 0, text: "Soltero" }, { value: 1, text: "Casado" }, { value: 2, text: "Divorciado"}] },
            HiringDate: { controlLink: Nervoustissue.UILinking.Date },
            WorkingHours: { controlLink: Nervoustissue.UILinking.Int },
            //TODO: change 0, 1 by false, true
            Lunch: { controlLink: Nervoustissue.UILinking.Options, options: [{ value: false, text: "No" }, { value: true, text: "Si"}] },
            Notes:
            {
                controlLink: Nervoustissue.UILinking.Collection,
                item:
                {
                    controlLink: Nervoustissue.UILinking.Compound,
                    template: _.template('<span data-bind="date"></span> <span data-bind="text"></span>'),
                    items:
                    [
                        { controlLink: Nervoustissue.UILinking.Date, name: "date", field: "RealDate" },
                        { controlLink: Nervoustissue.UILinking.Text, name: "text", field: "Note" }
                    ]
                }
            },
            SalaryChanges: {
                control: "collection",
                item: {
                    control: "compound",
                    template: _.template('<span data-bind="date"></span> | Salary: <span data-bind="salary"></span> | Note: <span data-bind="note"></span>'),
                    items: [{ name: "date", control: "date", field: "RealDate" }, { name: "salary", control: "int", field: "Salary" }, { name: "note", control: "text", field: "Note"}]
                }
            }
        }
});


App.EditEmployeeAppView = Backbone.View.extend({
    setModel: function (model) {
        this.model = model;
        this.dataBinder.setModel(model);
    },
    initialize: function () {
        this.dataBinder = new App.EditEmployeeAppViewDataBinder({ el: this.el });
    },
    events: {
        "click .saveEmployee": "saveEmployee",
        "click .reloadEmployee": "reloadEmployee"
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
                me.setModel(new App.Employee(result));
            }
        });
    }
});

//**/
$(function () {
    App.appView = new App.EditEmployeeAppView({
        el: $("#EditEmployeeApp")
    });
    App.appView.setModel(new App.Employee(ViewData.employee));
});


