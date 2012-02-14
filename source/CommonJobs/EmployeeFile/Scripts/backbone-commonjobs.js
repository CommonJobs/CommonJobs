(function () {
    //Utilities to avoid memory leaks on set/unset models
    _.extend(Backbone.View.prototype, {
        modelEventBindings: [],
        on: function (model, event, callback, context) {
            this.modelEventBindings.push({ model: model, event: event, callback: callback, context: context });
            return model.on(event, callback, context)
        },
        offAll: function () {
            var binding;
            while (binding = this.modelEventBindings.pop()) {
                binding.model.off(binding.event, binding.callback, binding.context);
            }
        },
        setModel: function (model) {
            if (this.model) {
                this.beforeUnsetModel();
                this.offAll();
                this.model = null;
            }
            if (model) {
                this.model = model;
            }
            this.$el.removeClass("editing");
            this.autoDataBind();
            if (this.model) {
                this.on(this.model, "change", function () { this.$el.addClass("editing"); }, this);
                this.afterSetModel();
            }
        },
        unsetModel: function () {
            this.setModel(null);
        },
        afterSetModel: function () { },
        beforeUnsetModel: function () { }
    });
} ());


(function () {
    //Utilities to data-binding

    var simpleModelBinder = function (view, model, options) {
        this.model = model;
        this.validModel = !!model;
        this.read = function () {
            return model.get(options.field);
        };
        this.write = function (value) {
            model.set(options.field, value);
        };
        this.onChange = function (action) {
            view.on(model, "change:" + options.field, action, view);
        };
    };

    var simpleCollectionBinder = function (view, model, options) {
        this.model = model;
        this.validModel = !!model;
        var collection = this.validModel ? model.get(options.field) : null;
        this.validCollection = !!collection;
        this.add = function () {
            collection.add();
        };
        this.remove = function (item) {
            collection.remove(item);
            item.destroy();
        };
        this.onAdd = function (action) {
            view.on(collection, "add", action, view);
        };
        this.each = function (action) {
            collection.each(action);
        };
    };

    var fullNameModelBinder = function (view, model, options) {
        this.validModel = !!model;
        this.read = function () {
            var firstName = model.get(options.firstNameField);
            var lastName = model.get(options.lastNameField);
            if (firstName) {
                return lastName + ", " + firstName;
            } else {
                return lastName;
            }
        };
        this.write = function (value) {
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
            var obj = {};
            obj[options.firstNameField] = firstName;
            obj[options.lastNameField] = lastName;
            model.set(obj);
        };
        this.onChange = function (action) {
            view.on(model, "change:" + options.firstNameField, action, view);
            view.on(model, "change:" + options.lastNameField, action, view);
        };
    };

    var bindOptionsControl = function (view, $el, modelBinder, options) {
        //TODO: there are details
        $el.off().empty();
        if (modelBinder.validModel) {
            $el.html(options.template({ Model: options.options }));
            var $view = $el.find(".view-editable");
            var $viewEmpty = $el.find(".view-editable-empty");
            var $editor = $el.find(".editor-editable");
            var optionIsValid = function(value) { 
                for (var i in options.options) {
                    if (value == options.options[i].value) {
                        return true; }}
                return false;
            };
            var findText = function(value) {
                for (var i in options.options) {
                    if (value == options.options[i].value) {
                        return options.options[i].text; }}
                return null;
            };
            var show = function () {
                $editor.hide();
                if (optionIsValid(modelBinder.read())) {
                    $viewEmpty.hide();
                    $view.show();
                } else {
                    $view.hide();
                    $viewEmpty.show();
                }
            };
            var edit = function () {
                $view.hide();
                $viewEmpty.hide();
                $editor.show().focus().select();
            };
            var refresh = function () {
                var value = modelBinder.read();
                if (optionIsValid(value)) {
                    $view.text(findText(value));
                    $editor.val(value);
                } else {
                    show();
                }
            };
            modelBinder.onChange(refresh);
            refresh();
            show();
            $el.on("click", ".view-editable,.view-editable-empty", null, function () {
                edit();
            });
            $el.on("change", ".editor-editable", null, function (e) {
                modelBinder.write($editor.val());
            });
            $el.on("keyup", ".editor-editable", null, function (e) {
                if (e.keyCode == 13 || e.keyCode == 27) {
                    show();
                }
            });
        }
    };

    var bindTextControl = function (view, $el, modelBinder, options) {
        $el.off().empty();
        if (modelBinder.validModel) {
            $el.html(options.template());
            var $view = $el.find(".view-editable");
            var $viewEmpty = $el.find(".view-editable-empty");
            var $editor = $el.find(".editor-editable");
            var originalValue;
            var show = function () {
                $editor.hide();
                if (modelBinder.read()) {
                    $viewEmpty.hide();
                    $view.show();
                } else {
                    $view.hide();
                    $viewEmpty.show();
                }
            };
            var edit = function () {
                originalValue = modelBinder.read(); ;
                $view.hide();

                $viewEmpty.hide();
                $editor.show().focus().select();
            };
            var refresh = function () {
                var value = modelBinder.read();
                $view.text(value);
                $editor.val(value);
            };
            modelBinder.onChange(refresh);
            refresh();
            show();
            $el.on("click", ".view-editable,.view-editable-empty", null, function () {
                edit();
            });
            $el.on("keyup", ".editor-editable", null, function (e) {
                //TODO: cuando un campo que está bindeado en dos controles diferentes está inicialmente vacío y en uno de los controles escribo el otro continua mostrando "Sin datos" hasta que presiono enter.
                //Es mas, cuando apreto enter tampoco funciona, tengo que empezar a editar y luego queda correcto
                if (e.keyCode == 27) {
                    modelBinder.write(originalValue);
                    show();
                } else {
                    modelBinder.write($editor.val());
                    if (e.keyCode == 13) {
                        show();
                    }
                }
            });
        }
    };

    var bindCompoundControl = function (view, $el, modelBinder, options) {
        $el.off().empty();
        $el.html(options.template());
        for (var i in options.items) {
            var itemcfg = options.items[i];
            view.dataBind(
                $el.find('[data-bind=' + (itemcfg.name || itemcfg.field) + ']'),
                modelBinder.model,
                itemcfg);
        }
    };

    var bindCollectionControl = function (view, $el, modelBinder, options) {
        $el.off().empty();
        if (modelBinder.validCollection) {
            $el.html(options.template());
            var $ul = $el.find(".list-editable");
            var addEl = function (item) {
                var $li = $(options.subtemplate());
                $ul.append($li);
                view.dataBind(
                    $li.find('[data-bind=item]'),
                    item,
                    options.item
                );
                $li.on("click", ".remove-button", null, function () { modelBinder.remove(item); });
                //TODO: use remove event in place of destroy 
                view.on(item, "destroy", function () { $li.remove(); }, view);
            };
            $el.on("click", ".add-button", null, modelBinder.add);
            modelBinder.onAdd(addEl);
            modelBinder.each(addEl);
        }
    };
    
    _.extend(Backbone.View.prototype, {
        dataBindings: {},
        modelBindings: {
            "simpleModel": simpleModelBinder,
            "simpleCollection": simpleCollectionBinder,
            "fullName": fullNameModelBinder
        },
        controlMappings: {
            "text": bindTextControl,
            //TODO: "date": bindDateControl,
            "date": bindTextControl,
            "compound": bindCompoundControl,
            "collection": bindCollectionControl,
            //TODO: "int": bindIntControl,
            "int": bindTextControl,
            "options": bindOptionsControl
        },
        defaultControlOptions: {
            "text": {
                template: _.template('<span class="view-editable-empty">Sin datos</span><span class="view-editable" style="display: none;"></span><input class="editor-editable" type="text" value="" style="display: none;"/>'),
                modelBinder: "simpleModel"
            },
            //TODO:
            "date": {
                template: _.template('<span class="view-editable-empty">Sin datos</span><span class="view-editable" style="display: none;"></span><input class="editor-editable" type="text" value="" style="display: none;"/>'),
                modelBinder: "simpleModel"
            },
            //TODO:
            "int": {
                template: _.template('<span class="view-editable-empty">Sin datos</span><span class="view-editable" style="display: none;"></span><input class="editor-editable" type="text" value="" style="display: none;"/>'),
                modelBinder: "simpleModel"
            },
            "options": {
                template: _.template('<span class="view-editable-empty">Sin datos</span><span class="view-editable" style="display: none;"></span><select class="editor-editable" style="display: none;"><% for (var i in Model) { %><option value="<%= Model[i].value %>"><%= Model[i].text %></option><% } %></select>'),
                modelBinder: "simpleModel"
            },
            "collection": {
                template: _.template('<ul class="list-editable"></ul><span class="add-button">+</span>'),
                subtemplate: _.template('<li><span class="remove-button">-</span><span class="editable-field" data-bind="item"></span></li>'),
                modelBinder: "simpleCollection"
            }
        },
        autoDataBind: function () {
            var view = this;
            var $els = view.$('[data-bind]');
            $els.each(function () {
                $el = $(this);
                var bindId = $el.attr("data-bind");
                var dataBindCfg = view.dataBindings[bindId];
                view.dataBind($el, view.model, _.extend({ field: bindId }, dataBindCfg));
            });
        },
        dataBind: function ($els, model, options) {
            var view = this;
            var controlName = options.control ? options.control : "text";
            var mappingFunction = view.controlMappings[controlName];
            var mergerdOptions = _.extend({ modelBinder: "simpleModel" }, view.defaultControlOptions[controlName], options);
            var modelBinderConstructor = _.isFunction(mergerdOptions.modelBinder) ? mergerdOptions.modelBinder : this.modelBindings[mergerdOptions.modelBinder];
            var modelBinder = new modelBinderConstructor(view, model, mergerdOptions);
            if (mappingFunction) {
                $els.each(function () {
                    mappingFunction(view, $(this), modelBinder, mergerdOptions);
                });
            }
        }
    });
} ());