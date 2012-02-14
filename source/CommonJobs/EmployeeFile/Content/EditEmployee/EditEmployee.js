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
    }
});

App.EditEmployeeAppView = Backbone.View.extend({
    dataBindings: {
        fullName: { control: "text", lastNameField: "LastName", firstNameField: "FirstName", modelBinder: "fullName" },
        Photo: { control: "picture" },
        IsGraduated: { control: "options", options: [{ value: false, text: "No recibido" }, { value: true, text: "Recibido"}] },
        BirthDate: { control: "date" },
        MaritalStatus: { control: "options", options: [{ value: 0, text: "Soltero" }, { value: 1, text: "Casado" }, { value: 2, text: "Divorciado"}] },
        HiringDate: { control: "date" },
        WorkingHours: { control: "int" },
        Lunch: { control: "options", options: [{ value: false, text: "No" }, { value: true, text: "Si"}] },
        Notes: {
            control: "collection",
            item: {
                control: "compound",
                template: _.template('<span data-bind="date"></span> <span data-bind="text"></span>'),
                items: [{ name: "date", control: "date", field: "RealDate" }, { name: "text", control: "text", field: "Note"}]
            } 
        }
    },
    initialize: function () {
        //this.autoDataBind();
    },
    afterSetModel: function () {
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


