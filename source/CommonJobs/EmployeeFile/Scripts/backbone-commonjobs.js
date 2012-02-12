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
    bindNotesField_defaults: {
        template: _.template('<ul class="list-editable"></ul><span class="add-button">+</span>'),
        subtemplate: _.template('<li><span class="remove-button">-</span><span class="editable-field" data-bind="Text"><span></li>'),
        getElement: function (view, fieldName) {
            return view.$('[data-bind="' + fieldName + '"]');
        },
        getCollection: function (view, fieldName) {
            if (view.model) {
                return view.model.get(fieldName);
            } else {
                return view.model;
            }
        },
        bindAddEvent: function (view, collection, callback, context) {
            return view.on(collection, "add", callback, view);
        },
        bindDestroyEvent: function (view, submodel, callback, context) {
            return view.on(submodel, "destroy", callback, view);
        }
    },
    bindNotesField: function (fieldName, options) {
        var me = this;
        options = _.extend({}, me.bindNotesField_defaults, options);
        var collection = options.getCollection(me, fieldName);
        var $els = options.getElement(me, fieldName);
        $els.each(function () {
            var $el = $(this);
            $el.off().empty();
            if (collection) {
                var bindAddEvent = function (callback) { return options.bindAddEvent(me, collection, callback, me); };
                var bindDestroyEvent = function (submodel, callback) { return options.bindDestroyEvent(me, submodel, callback, me); };
                $el.html(options.template());
                var $ul = $el.find(".list-editable");
                var add = function () { collection.add(); };
                var remove = function (submodel) {
                    collection.remove(submodel);
                    submodel.destroy();
                };
                var addEl = function (submodel) {
                    var $li = $(options.subtemplate());
                    $ul.append($li);
                    me.bindTextField(
                        "Text",
                        {
                            getModel: function (view) { return submodel; },
                            getElement: function (view, fieldName) { return $li.find('[data-bind="' + fieldName + '"]'); }
                        }
                    );
                    $li.on("click", ".remove-button", null, function () { remove(submodel); });
                    bindDestroyEvent(submodel, function () {
                        $li.remove();
                    });
                };
                $el.on("click", ".add-button", null, add);
                bindAddEvent(function (e) { addEl(e); });
                collection.each(addEl);
            }
        });
    },
    bindTextField_defaults: {
        template: _.template('<span class="view-editable-empty">Sin datos</span><span class="view-editable" style="display: none;"></span><input class="editor-editable" type="text" value="" style="display: none;"/>'),
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
                $el.html(options.template());
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
