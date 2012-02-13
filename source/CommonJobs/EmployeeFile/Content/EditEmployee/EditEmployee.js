/// <reference path="../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../Scripts/underscore.js" />
/// <reference path="../../Scripts/backbone.js" />
window.App = { };

App.Note = Backbone.Model.extend({
    defaults: function () {
        return {
            Date: new Date(),
            Text: ""
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
    initialize: function () {
        this.set("Notes", new App.Notes(this.get("Notes")));
    }
});

App.EditEmployeeAppView = Backbone.View.extend({
    dataBindings: {
        fullName: { control: "text", lastNameField: "LastName", firstNameField: "FirstName", modelBinder: "fullName" },
        Photo: { control: "picture" },
        IsGraduated: { control: "options", options: [{ key: false, value: "No recibido" }, { key: true, value: "Recibido"}] },
        BirthDate: { control: "date" },
        MaritalStatus: { control: "options", options: [{ key: 0, value: "Soltero" }, { key: 1, value: "Casado" }, { key: 2, value: "Divorciado"}] },
        HiringDate: { control: "date" },
        WorkingHours: { control: "int" },
        Lunch: { control: "options", options: [{ key: false, value: "No" }, { key: true, value: "Si"}] },
        Notes: { control: "datedNotes", item: { control: "text", field: "Text"} }
    },
    initialize: function () {
        //this.autoDataBind();
    },
    events: {
        "click .saveEmployee": "saveEmployee",
        "click .reloadEmployee": "reloadEmployee"
    },
    saveEmployee: function () {
        var me = this;
        $.ajax({
            url: Model.saveEmployeeUrl,
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
            url: Model.getEmployeeUrl,
            type: 'GET',
            dataType: 'json',
            data: { id: Model.employee.Id },
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
    App.appView.setModel(new App.Employee(Model.employee));
});


