/// <reference path="../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../Scripts/underscore.js" />
/// <reference path="../../Scripts/backbone.js" />
window.App = { };

App.Employee = Backbone.Model.extend({
    defaults: function () {
        return {
        }
    },
    initialize: function () {
    }
});

_.extend(Backbone.View.prototype, {
    modelBindings: [],
    on: function (model, event, callback, context) {
        this.modelBindings.push({ model: model, event: event, callback: callback, context: context });
        return model.on(event, callback, context)
    },
    offAll: function () {
        var binding;
        while (binding = this.modelBindings.pop()) {
            binding.model.off(binding.event, binding.callback, binding.context);
        }
    },
    setModel: function (model) {
        if (this.model) {
            this.offAll();
            this.model = null;
        }
        if (model) {
            this.model = model;
        }
        this.dataBind();
    },
    bindTextField_template: _.template('<span class="view-editable-empty">Sin datos</span><span class="view-editable" style="display: none;"></span><input class="editor-editable" type="text" value="" style="display: none;"/>'),
    bindTextField_defaults: {
        getElement: function (view, fieldName) {
            return view.$('[data-bind="' + fieldName + '"]');
        },
        getModel: function (view) {
            return view.model;
        },
        getModelValue: function (model, fieldName) {
            return model.get(fieldName);
        },
        setModelValue: function (model, fieldName, value) {
            return model.set(fieldName, value);
        },
        bindChangeEvent: function (view, model, fieldName, callback, context) {
            return view.on(model, "change:" + fieldName, callback, view);
        }
    },
    bindTextField: function (fieldName, options) {
        var me = this;
        options = _.extend({}, me.bindTextField_defaults, options);
        var model = options.getModel(me);
        var $els = options.getElement(me, fieldName);
        $els.each(function () {
            var $el = $(this);
            $el.off().empty();
            if (model) {
                var getModelValue = function () { return options.getModelValue(model, fieldName); };
                var setModelValue = function (value) { return options.setModelValue(model, fieldName, value); };
                var bindChangeEvent = function (callback) { return options.bindChangeEvent(me, model, fieldName, callback, me); };
                $el.html(me.bindTextField_template());
                var $view = $el.find(".view-editable");
                var $viewEmpty = $el.find(".view-editable-empty");
                var $editor = $el.find(".editor-editable");
                var originalValue;
                var view = function () {
                    $editor.hide();
                    if (getModelValue()) {
                        $viewEmpty.hide();
                        $view.show();
                    } else {
                        $view.hide();
                        $viewEmpty.show();
                    }
                };
                var edit = function () {
                    originalValue = getModelValue();
                    $view.hide();
                    $viewEmpty.hide();
                    $editor.show().focus().select();
                };
                var refresh = function () {
                    var value = getModelValue();
                    $view.text(value);
                    $editor.val(value);
                };
                bindChangeEvent(refresh);
                refresh();
                view();
                $el.on("click", ".view-editable,.view-editable-empty", null, function () {
                    edit();
                });
                $el.on("keyup", ".editor-editable", null, function (e) {
                    if (e.keyCode == 27) {
                        setModelValue(originalValue);
                        view();
                    } else {
                        setModelValue($editor.val());
                        if (e.keyCode == 13) {
                            view();
                        }
                    }
                });
            }
        });
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


