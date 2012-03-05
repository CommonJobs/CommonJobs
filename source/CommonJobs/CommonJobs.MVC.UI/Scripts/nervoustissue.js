//     Nervoustissue.js 0.0.2

//     (c) 2012 Andrés Moschini
//     Nervoustissue may be freely distributed under the MIT license.

// TODO: Leer la configuración de los elementos desde el mismo elemento, por ejemplo permitir que en lugar de hacer lo siguiente:
//
//        <h1 class="editable-field" data-bind="fullName"></h1>
//        App.EditEmployeeAppViewDataBinder = Nervoustissue.FormBinder.extend({
//            dataBindings: {
//                fullName: { controlLink: "Text", dataLink: "FullName", lastNameField: "LastName", firstNameField: "FirstName" },
//            [...]
//
// hacer
//
//        <h1 class="editable-field" data-bind="{'controlLink':'Text','dataLink':'FullName','lastNameField':'LastName','firstNameField':'FirstName'}"></h1>
//
// o bien
//
//        <h1 class="editable-field" data-bind-controlLink="Text" data-bind-dataLink="FullName" data-bind-lastNameField="LastName" data-bind-firstNameField="FirstName"></h1>
//
// Luego se pueden hacer helpers razor para generar el html correspondiente a partir de las anotaciones en el modelo. También
// se podría renderizar el texto inicial para facilitar la indexación por buscadores y al menos mostrar el contenido si la página no soporta javascript.
// Por ejemplo:
//
//        @Html.NervousFullNameFieldFor("h1", x => x.LastName, x => x.FirstName, new { @class = "editable-field" })
//
// generaría:
//
//        <h1 class="editable-field" data-bind="{'controlLink':'Text','dataLink':'FullName','lastNameField':'LastName','firstNameField':'FirstName'}">Perez, Juan</h1>
// O
//
//        @Html.NervousFieldFor("span", x => x.MaritalStatus, new { @class = "editable-field" })
//
// generaría:
//
//        <span class="editable-field" data-bind="{'controlLink':'Options','options':[{'value':0,'text':'Soltero'},{'value':1,'text':'Casado'},{'value':2,'text':'Divorciado'}]}">
//
// Ya que el modelo tendría las anotaciones correspondientes o automáticamente se detectaría que MaritalStatus es de tipo enum con los valores Soltero, Casado y Divorciado.


(function () {

    // Initial Setup
    // -------------

    // Save a reference to the global object (`window` in the browser, `global`
    // on the server).
    var root = this;

    // Save the previous value of the `Nervoustissue` variable, so that it can be
    // restored later on, if `noConflict` is used.
    var previousNervoustissue = root.Nervoustissue;

    // The top-level namespace. All public Backbone classes and modules will
    // be attached to this. Exported for both CommonJS and the browser.
    // (Supongo que está bien...)
    var Nervoustissue;
    if (typeof exports !== 'undefined') {
        Nervoustissue = exports;
    } else {
        Nervoustissue = root.Nervoustissue = {};
    }

    // Require Underscore, if we're on the server, and it's not already present.
    var _ = root._;
    if (!_ && (typeof require !== 'undefined')) _ = require('underscore');

    // Require Backbone, if we're on the server, and it's not already present.
    var Backbone = root.Backbone;
    if (!Backbone && (typeof require !== 'undefined')) Backbone = require('backbone');


    // For Nervoustissue's purposes, jQuery, Zepto, or Ender owns the `$` variable.
    // (los otros no los conozco pero supongo que está bien)
    var $ = Nervoustissue._domLibrary = root.jQuery || root.Zepto || root.ender;

    // Set the JavaScript library that will be used for DOM manipulation and
    // Ajax calls (a.k.a. the `$` variable). By default Nervoustissue will use: jQuery,
    // Zepto, or Ender; but the `setDomLibrary()` method lets you inject an
    // alternate JavaScript library (or a mock library for testing your views
    // outside of a browser).
    Nervoustissue.setDomLibrary = function (lib) {
        $ = Nervoustissue._domLibrary = lib;
    };

    // Runs Nervoustissue.js in *noConflict* mode, returning the `Nervoustissue` variable
    // to its previous owner. Returns a reference to this Nervoustissue object.
    Nervoustissue.noConflict = function () {
        root.Nervoustissue = previousNervoustissue;
        return this;
    };


    (function () {
        // Nervoustissue.DataLinking
        // -------------------------

        // Abstracciones para acceder a las propiedades de los objetos de datos y a sus eventos

        var m = Nervoustissue.DataLinking = {};

        // Nervoustissue.DataLinking.Base
        // ------------------------------

        m.Base = function (options) {
            _.extend(this, options);
            this.validModel = !!this.model;
            this.initialize();
        }

        m.Base.extend = Backbone.Model.extend;

        _.extend(m.Base.prototype, {
            initialize: function () { }
        });

        // Nervoustissue.DataLinking.Model
        // -------------------------------

        // Se encarga de leer y escribir en en propiedades de un Model de backbone y 
        // registrar los eventos correspondientes.

        m.Model = m.Base.extend({
            initialize: function () {

            },
            read: function () {
                return this.model.get(this.field);
            },
            write: function (value) {
                this.model.set(this.field, value);
            },
            onChange: function (action, context) {
                this.viewDataBinder.registerModelEvent(this.model, "change:" + this.field, action, context);
            }
        });

        // Nervoustissue.DataLinking.FullName
        // ---------------------------------------------

        // Lee y escribe en dos campos diferentes concatenando y parseandolos

        m.FullName = m.Model.extend({
            read: function () {
                var firstName = this.model.get(this.firstNameField);
                var lastName = this.model.get(this.lastNameField);
                if (firstName) {
                    return lastName + ", " + firstName;
                } else {
                    return lastName;
                }
            },
            write: function (value) {
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
                obj[this.firstNameField] = firstName;
                obj[this.lastNameField] = lastName;
                this.model.set(obj);
            },
            onChange: function (action, context) {
                this.viewDataBinder.registerModelEvent(this.model, "change:" + this.firstNameField, action, context);
                this.viewDataBinder.registerModelEvent(this.model, "change:" + this.lastNameField, action, context);
            }
        });

        // Nervoustissue.DataLinking.Collection
        // ------------------------------------------

        // Se encarga de leer y escribir en los elementos de un Collection de backbone y 
        // registrar los eventos correspondientes.	
        m.Collection = m.Base.extend({
            initialize: function () {
                this.collection = this.validModel ? this.model.get(this.field) : null;
                this.validModel = !!this.collection;
            },
            add: function () {
                this.collection.add();
            },
            remove: function (item) {
                this.collection.remove(item);
                item.destroy();
            },
            onAdd: function (action, context) {
                this.viewDataBinder.registerModelEvent(this.collection, "add", action, context);
            },
            each: function (action, context) {
                for (var i in this.collection.models) {
                    action.call(context, this.collection.models[i]);
                }
            }
        });
    })();

    (function () {
        // Nervoustissue.UILinking
        // -------------------------

        // Abstracciones para modificar los elementos del DOM y leer sus datos y escuchar sus eventos

        var m = Nervoustissue.UILinking = {};

        m.Base = function (configuration) {
            _.extend(this, configuration);
            this.el = $(this.el)[0];
            this.$el = $(this.el);
            this.$el.off().empty();
            this.linkedData = new this.dataLink(_.extend({ viewDataBinder: this.viewDataBinder, model: this.model }, configuration));
            if (this.linkedData.validModel) {
                this.viewDataBinder.on("change", this.applyFormEditMode, this);
                this.$el.html(this.render());
                this._initialize();
            }
        }

        _.extend(m.Base.prototype, {
            render: function () {
                return this.template(this.getTemplateModel());
            },
            getTemplateModel: function () {
                return null;
            },
            applyFormEditMode: function () {
                this.applyMode("view");
            },
            mode: "view",
            applyMode: function (mode) { }
        });

        m.Base.extend = Backbone.Model.extend;

        m.Collection = m.Base.extend({
            template: _.template('<ul class="list-editable"></ul><button class="add-button">+</button>'),
            subtemplate: _.template('<li><button class="remove-button">-</button><span class="editable-field" data-bind="item"></span></li>'),
            dataLink: Nervoustissue.DataLinking.Collection,
            _initialize: function () {
                var me = this;
                this.$ul = this.$el.find(".list-editable");
                this.addEl = function (item) {
                    var $li = $(this.subtemplate());
                    this.$ul.append($li);
                    this.viewDataBinder.dataBind(
                        $li.find('[data-bind=item]'),
                        item,
                        this.item
                    );
                    $li.on("click", ".remove-button", null, function () { me.linkedData.remove(item); });
                    //TODO: use remove event in place of destroy 
                    this.viewDataBinder.registerModelEvent(item, "destroy", function () { $li.remove(); }, this.viewDataBinder);
                };
                this.$el.on("click", ".add-button", null, function () { me.linkedData.add(); });
                this.linkedData.onAdd(this.addEl, this);
                this.linkedData.each(this.addEl, this);
            },
            applyMode: function (mode) {
                var formMode = this.viewDataBinder.editionMode();
                var buttons = this.$el.find(".add-button,.remove-button");
                if (formMode == "readonly") {
                    buttons.hide();
                } else {
                    buttons.show();
                }
            }
        });


        // Nervoustissue.UILinking.BaseModel
        // ------------------------------------

        m.BaseModel = m.Base.extend({
            _initialize: function () {
                var me = this;
                me.$view = this.$(".view-editable");
                var $viewContent = this.$(".view-editable-content");
                me.$viewContent = $viewContent.length ? $viewContent : me.$view;
                me.$viewEmpty = this.$(".view-editable-empty");
                me.$editor = this.$(".editor-editable");
                me.writing = false;
                me.linkedData.onChange(this.refresh, this);
                me.bindUI();
                me.applyMode("view");
                me.refresh();
            },
            $: function (selector) {
                return this.$el.find(selector);
            },
            applyMode: function (mode) {
                if (!mode) {
                    mode = this.mode;
                }
                var formMode = this.viewDataBinder.editionMode();
                if (formMode == "full-edit") {
                    mode = "edit";
                } else if (formMode == "readonly") {
                    mode = "view";
                }
                this.mode = mode;
                if (mode == "view") {
                    this.showView();
                } else if (mode == "edit") {
                    this.showEdit();
                }
            },
            dataEmpty: function () {
                var value = this.linkedData.read();
                return typeof value == "undefined" || value === null || (_.isString(value) && $.trim(value) === "");
            },
            dataLink: Nervoustissue.DataLinking.Model,
            originalValue: null,
            focusOnEditor: function () {
                if (this.$editor.css("display") != "none") {
                    this.$editor.focus().select();
                }
            },
            refreshView: function (text) {
                this.$viewContent.html(text);
            },
            refreshEdit: function (value) {
                this.$editor.val(value);
            },
            showView: function () {
                this.$editor.hide();
                if (!this.dataEmpty()) {
                    this.$viewEmpty.hide();
                    this.$view.show();
                } else {
                    this.$view.hide();
                    this.$viewEmpty.show();
                }
            },
            showEdit: function () {
                this.originalValue = this.linkedData.read();
                this.$view.hide();
                this.$viewEmpty.hide();
                this.$editor.show();
            },
            clearData: function () {
                this.linkedData.write(null);
            },
            undoEdition: function () {
                this.linkedData.write(this.originalValue);
            },
            update: function () {
                this.writing = true;
                this.linkedData.write(this.readUI());
                this.writing = false;
            },
            refresh: function () {
                var value = this.linkedData.read();
                this.refreshView(this.dataEmpty(value) ? '' : this.valueToContent(value));
                if (!this.writing) {
                    this.refreshEdit(value);
                    this.applyMode();
                }
            },
            valueToContent: function (value) {
                return value;
            }
        });

        m.ReadOnlyText = m.BaseModel.extend({
            template: _.template('<span class="view-editable-empty">Sin datos</span><span class="view-editable" style="display: none;"></span>'),
            showView: function () {
                if (!this.dataEmpty()) {
                    this.$viewEmpty.hide();
                    this.$view.show();
                } else {
                    this.$view.hide();
                    this.$viewEmpty.show();
                }
            },
            showEdit: function () { },
            refreshEdit: function (value) { },
            readUI: function () { },
            bindUI: function () { },
            valueToContent: function (value) {
                return value.toString();
            }
        });

        m.Text = m.BaseModel.extend({
            template: _.template('<span class="view-editable-empty">Sin datos</span><span class="view-editable" style="display: none;"></span><input class="editor-editable" type="text" value="" style="display: none;"/>'),
            readUI: function () {
                return this.$editor.val();
            },
            onEditableClick: function () {
                this.applyMode("edit");
                this.focusOnEditor();
            },
            onKeyUp: function (e) {
                if (e.keyCode == 27) {
                    this.undoEdition();
                    this.applyMode("view");
                } else {
                    this.update();
                    if (!e.altKey && !e.shiftKey && e.keyCode == 13) {
                        this.applyMode("view");
                    }
                }
            },
            bindUI: function () {
                var me = this;
                me.$el.on("click", ".view-editable,.view-editable-empty", null, function () {
                    me.onEditableClick();
                });
                me.$el.on("keyup", ".editor-editable", null, function (e) {
                    me.onKeyUp(e);
                });
            },
            valueToContent: function (value) {
                return value.toString();
            }
        });

        m.MultilineText = m.Text.extend({
            template: _.template('<span class="view-editable-empty">Sin datos</span><div class="view-editable markdown-content" style="display: none;"></div><div class="editor-editable" style="display: none;"><textarea  cols=50 rows=10 class="mdd_editor"></textarea></div>'),
            bindUI: function () {
                var me = this;
                me.$el.on("click", ".view-editable,.view-editable-empty", null, function () {
                    me.onEditableClick();
                });
                me.$el.on("keyup", ".editor-editable textarea", null, function (e) {
                    me.onKeyUp(e);
                });
            },
            onKeyUp: function (e) {
                if (e.keyCode == 27) {
                    this.undoEdition();
                    this.applyMode("view");
                } else {
                    this.update();
                    //I am waiting for ctrl + enter to accept changes
                    if (e.ctrlKey && e.keyCode == 13) {
                        this.applyMode("view");
                    }
                }
            },
            focusOnEditor: function () {
                if (this.$editor.css("display") != "none") {
                    this.$editor.find("textarea").focus().select();
                }
            },
            refreshEdit: function (value) {
                this.$editor.find("textarea").val(value);
            },
            readUI: function () {
                return this.$editor.find("textarea").val();
            },
            valueToContent: function (value) {
                return $("<pre></pre>").text(value);
            }
        });

        m.Markdown = m.Text.extend({
            template: _.template('<span class="view-editable-empty">Sin datos</span><div class="view-editable markdown-content" style="display: none;"></div><div class="editor-editable" style="display: none;"><textarea  cols=50 rows=10 class="mdd_editor"></textarea></div>'),
            bindUI: function () {
                //TODO: separar de alguna forma $editor y .editor-editable ya que están representando tanto la vista de edición como el elemento que tiene el valor.
                var me = this;
                me.$editor.find("textarea").MarkdownDeep({
                    help_location: "/Scripts/mdd_help.htm",
                    disableTabHandling: true,
                    resizebar: false,
                    ExtraMode: true
                });
                me.$editor.find(".mdd_preview").hide(); //Esto es un parche para no mostrar el preview de MarkdownDeep
                me.$el.on("click", ".view-editable,.view-editable-empty", null, function () {
                    me.onEditableClick();
                });
                me.$el.on("keyup", ".editor-editable textarea", null, function (e) {
                    me.onKeyUp(e);
                });
            },
            onKeyUp: function (e) {
                if (e.keyCode == 27) {
                    this.undoEdition();
                    this.applyMode("view");
                } else {
                    this.update();
                    if (e.ctrlKey && e.keyCode == 13) {
                        this.applyMode("view");
                    }
                }
            },
            focusOnEditor: function () {
                //TODO: separar de alguna forma $editor y .editor-editable ya que están representando tanto la vista de edición como el elemento que tiene el valor.
                if (this.$editor.css("display") != "none") {
                    this.$editor.focus().select();
                }
            },
            refreshEdit: function (value) {
                //TODO: separar de alguna forma $editor y .editor-editable ya que están representando tanto la vista de edición como el elemento que tiene el valor.
                this.$editor.find("textarea").val(value);
            },
            readUI: function () {
                //TODO: separar de alguna forma $editor y .editor-editable ya que están representando tanto la vista de edición como el elemento que tiene el valor.
                return this.$editor.find("textarea").val();
            },
            valueToContent: function (value) {
                //return value;
                var md = new MarkdownDeep.Markdown();
                md.ExtraMode = true;
                return md.Transform(value);
            }
        });

        m.Int = m.Text.extend({
            template: _.template('<span class="view-editable-empty">Sin datos</span><span class="view-editable" style="display: none;"></span><input class="editor-editable" type="text" value="" style="display: none;"/>'),
            onKeyUp: function (e) {
                this.$editor.val(this.$editor.val().replace(/[^0-9]/g, ''));
                //TODO: Acá estoy llamando al onKeyUp de Text, tiene que haber una forma mejor 
                this.__proto__.__proto__.onKeyUp.call(this, e);
            }
        });

        m.Date = m.BaseModel.extend({
            template: _.template('<span class="view-editable-empty">Sin datos</span><span class="view-editable" style="display: none;"></span><input class="editor-editable" type="text" value="" style="display: none;"/>'),
            valueToContent: function (value) {
                return Globalize.format(new Date(value), "d");
            },
            refreshEdit: function (value) {
                this.$editor.datepicker("setDate", value ? new Date(value) : null);
            },
            readUI: function () {
                var date = this.$editor.datepicker("getDate");
                return date ? date.toJSON() : null;
            },
            bindUI: function () {
                var me = this;
                me.$editor.datepicker({
                    onClose: function () {
                        me.update();
                        me.focusOnEditor();
                        //which is better? or both?
                        //me.$editor.focus().select();
                        //me.applyMode("view");
                    }
                });
                me.$el.on("click", ".view-editable,.view-editable-empty", null, function () {
                    me.applyMode("edit");
                    me.focusOnEditor();
                });
                me.$el.on("keyup", ".editor-editable", null, function (e) {
                    //TODO: cuando un campo que está bindeado en dos controles diferentes está inicialmente vacío y en uno de los controles escribo el otro continua mostrando "Sin datos" hasta que presiono enter.
                    //Es mas, cuando apreto enter tampoco funciona, tengo que empezar a editar y luego queda correcto
                    if (e.keyCode == 27) {
                        me.undoEdition();
                        me.applyMode("view");
                    } else {
                        me.update();
                        if (e.keyCode == 13) {
                            me.applyMode("view");
                        }
                    }
                });

            }
        });

        m.Compound = m.BaseModel.extend({
            refresh: function () { },
            bindUI: function () {
                var me = this;
                for (var i in this.items) {
                    var itemcfg = this.items[i];
                    this.viewDataBinder.dataBind(
                        this.$el.find('[data-bind=' + (itemcfg.name || itemcfg.field) + ']'),
                        this.model,
                        itemcfg);
                }
            }
        });

        m.Toggle = m.BaseModel.extend({
            template: _.template('<a class="view-editable"></a>'),
            onTemplate: _.template('On'),
            offTemplate: _.template('Off'),
            valueToContent: function (value) {
                return value ? this.onTemplate() : this.offTemplate();
            },
            readUI: null,
            refreshView: function (text) {
                this.$view.html(text);
            },
            applyMode: function (mode) {
                if (!mode) {
                    mode = this.mode;
                }
                var formMode = this.viewDataBinder.editionMode();
                this.showView();
                if (formMode == "readonly") {
                    this.$view.removeAttr("href");
                } else {
                    this.$view.attr("href", "javascript:void(null)");
                }
            },
            dataEmpty: function () { return false; },
            onEditableClick: function () { this.toggle(); },
            toggle: function () {
                var formMode = this.viewDataBinder.editionMode();
                if (formMode != "readonly") {
                    var value = this.linkedData.read();
                    this.linkedData.write(!value);
                }
            },
            onKeyUp: function (e) {
                if (e.keyCode == 32) {
                    this.toggle();
                }
            },
            bindUI: function () {
                var me = this;
                me.$el.on("click", ".view-editable", null, function () {
                    me.onEditableClick();
                });
                me.$el.on("keyup", ".view-editable", null, function (e) {
                    me.onKeyUp(e);
                });
            },
            refreshEdit: function () { }
        });

        m.Options = m.BaseModel.extend({
            getTemplateModel: function () {
                return { Model: this.options };
            },
            template: _.template('<span class="view-editable-empty">Sin datos</span><span class="view-editable" style="display: none;"></span><select class="editor-editable" style="display: none;"><% for (var i in Model) { %><option value="<%= Model[i].value %>"><%= Model[i].text %></option><% } %></select>'),
            refreshEdit: function (value) {
                this.$editor.val(value === null ? '' : value.toString());
            },
            valueToContent: function (value) {
                for (var i in this.options) {
                    if (value.toString() == this.options[i].value.toString()) {
                        return this.options[i].text;
                    }
                }
                return "<i>Invalid Data!</i>";
            },
            readUI: function () {
                return this.$editor.val();
            },
            onEditableClick: function () {
                this.applyMode("edit");
                this.focusOnEditor();
            },
            onKeyUp: function (e) {
                if (e.keyCode == 27) {
                    this.undoEdition();
                    this.applyMode("view");
                } else {
                    this.update();
                    if (e.keyCode == 13) {
                        this.applyMode("view");
                    }
                }
            },
            bindUI: function () {
                var me = this;
                me.$el.on("click", ".view-editable,.view-editable-empty", null, function () {
                    me.onEditableClick();
                });
                me.$el.on("keyup", ".editor-editable", null, function (e) {
                    me.onKeyUp(e);
                });
                me.$el.on("change", ".editor-editable", null, function (e) {
                    me.update();
                });
            }
        });

    })();


    // Nervoustissue.FormBinder
    // ----------------------------

    // Bindea los elementos del DOM correspondientes a un bloque (el) con
    // un Model de Backbone (En un futuro quiero eliminar esa dependencia)

    var FormBinder = Nervoustissue.FormBinder = function (configuration) {
        _.extend(this, configuration);
        this.setElement(this.el);
        this.setModel(this.model);
    };

    Nervoustissue.FormBinder.extend = Backbone.Model.extend;

    _.extend(FormBinder.prototype, Backbone.Events, {
        // Establece el elemento correspondiente al bloque que se bindeará
        setElement: function (element) {
            if (this.el) {
                this.unregisterAllElementEvents();
                this.el = this.$el = null;
            }
            if (element) {
                this.el = $(element)[0];
                this.$el = $(this.el);
            }
            // No estoy seguro de que esto sea necesario, ni como hacerlo. 
            //Probar cuando permita cambiar los Elements:
            //this.autoDataBind();
        },
        // Establece el modelo que se bindeará
        setModel: function (model) {
            if (this.model) {
                this.unregisterAllModelEvents();
                this.model = null;
            }
            if (model) {
                this.model = model;
            }
            this.$el.removeClass("editing");
            this.autoDataBind();
            if (this.model) {
                //TODO: remover la dependencia con Backbone acá
                this.registerModelEvent(this.model, "change", function () { this.$el.addClass("editing"); }, this);
                //TODO: Tal vez acá podría escuchar los eventos de las inner collections para no 
                //tener que hacerlo afuera.
            }
        },
        // Limpia todos los eventos asociados a los elementos
        unregisterAllElementEvents: function () {
            //TODO
        },

        // ### Bindeo eventos de los modelos ###
        // Mantiene una lista de todos los eventos registrados
        modelEventBindings: [],
        // Registra un evento asociado a un Model
        registerModelEvent: function (model, event, callback, context) {
            //TODO: remover dependencia con Backbone
            this.modelEventBindings.push({ model: model, event: event, callback: callback, context: context });
            return model.on(event, callback, context)
        },
        // Libera todos los eventos asociados a un Model
        unregisterAllModelEvents: function () {
            var binding;
            while (binding = this.modelEventBindings.pop()) {
                binding.model.off(binding.event, binding.callback, binding.context);
            }
        },
        // Configuración personalizada de los bindeos
        dataBindings: {},

        //TODO: Rename view to viewDataBinder and view to view 
        autoDataBind: function () {
            var viewDataBinder = this;
            var $els = this.$el.find('[data-bind]');
            $els.each(function () {
                var $el = $(this);
                var bindId = $el.attr("data-bind");
                var dataBindCfg = viewDataBinder.dataBindings[bindId];
                viewDataBinder.dataBind($el, viewDataBinder.model, _.extend({ field: bindId }, dataBindCfg));
            });
        },
        dataBind: function ($els, model, options) {
            var viewDataBinder = this;
            var controlClass = options.controlLink ? options.controlLink : Nervoustissue.UILinking.Text;
            if (_.isString(controlClass)) {
                var aux = Nervoustissue.UILinking[controlClass];
                if (!_.isFunction(aux)) {
                    alert("Nervoustissue.js: " + controlClass + " is not a valid control");
                    return;
                }
                controlClass = aux;
            }

            var myoptions = { viewDataBinder: viewDataBinder, model: model };

            if (options.dataLink) {
                var dataLink = options.dataLink;
                dataLink = _.isString(dataLink) ? Nervoustissue.DataLinking[dataLink] : dataLink;
                myoptions.dataLink = dataLink;
            }

            $els.each(function () {
                new controlClass(_.extend({}, options, myoptions, { el: this }));
            });
        },
        _editionMode: "normal", //normal, readonly, full-edit
        editionMode: function (mode) {
            if (typeof mode === "undefined") {
                return this._editionMode;
            } else {
                var previous = this._editionMode;
                switch (mode) {
                    case "readonly":
                    case "full-edit":
                        this._editionMode = mode; break;
                    default: this._editionMode = "normal"; break;
                }
                if (this._editionMode != previous) {
                    this.trigger("change change:edition-mode change:edition-mode:" + this._editionMode, this._editionMode);
                }
            }
        }
    });

}).call(this);
