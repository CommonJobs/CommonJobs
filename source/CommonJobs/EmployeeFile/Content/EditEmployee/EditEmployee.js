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
    initialize: function () {
        this.dataBind();
    },
    events: {
        "click .saveEmployee": "saveEmployee",
        "click .reloadEmployee": "reloadEmployee"
    },
    dataBind: function () {
        //It will be automatic
        this.bindTextField("Platform");
        this.bindTextField("CurrentProject");
        this.bindTextField("CurrentPosition");
        this.bindTextField("InitialPosition");
        this.bindTextField("Degree");
        this.bindTextField("Address");
        this.bindTextField("Telephones");
        this.bindTextField("Seniority");
        this.bindTextField("EnglishLevel");
        this.bindTextField("College");
        this.bindTextField("Skills");
        this.bindTextField("Certifications");
        this.bindTextField("FileId");
        this.bindTextField("Schedule");
        this.bindTextField("BankAccount");
        this.bindTextField("HealthInsurance");
        this.bindTextField("Agreement");
        this.bindTextField("Vacations");
        this.bindNotesField("Notes");
        this.bindTextField(
            "[LastName, FirstName]",
            {
                getModelValue: function (model, fieldName) {
                    var firstName = model.get("FirstName");
                    var lastName = model.get("LastName");
                    if (firstName) {
                        return lastName + ", " + firstName;
                    } else {
                        return lastName;
                    }
                },
                setModelValue: function (model, fieldName, value) {
                    var firstName = '';
                    var lastName = '';
                    if (value.split) {
                        var aux = value.split(',');
                        if (aux.length > 0) {
                            lastName = $.trim(aux.shift());
                        }
                        if (aux.length > 0) {
                            firstName = $.trim(aux.join(','));
                        }
                    }
                    return model.set({ FirstName: firstName, LastName: lastName });
                },
                bindChangeEvent: function (view, model, fieldName, callback, context) {
                    view.on(model, "change:LastName", callback, view);
                    view.on(model, "change:FirstName", callback, view);
                }
            }
        );
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


