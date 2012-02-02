/// <reference path="../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../Scripts/backbone.js" />
/// <reference path="../Scripts/underscore.js" />
    $(function () {
        window.App = {};
        App.ViewTypes = [
            { Code: 0, Description: 'List' },
            { Code: 1, Description: 'Grid' }
        ];

        App.Category = Backbone.Model.extend({
            //detailView: null,
            defaults: function () {
                return {
                    Code: '',
                    Description: '',
                    ViewType: 'List',
                    Facets: [],
                    Editing: false
                };
            },
            edit: function () {
                var me = this;
                this.collection.each(function (cat) {
                    cat.set({ Editing: cat == me });
                });
                //I am generating a lot of views...
                var v = new App.CategoryDetailView({ model: me });
                v.render();
            }
        });

        App.Categories = Backbone.Collection.extend({
            model: App.Category,
            url: "/rest/categories"
        });

        //My Data
        App.Categories.Instance = new App.Categories();

        App.CategoryItemView = Backbone.View.extend({
            tagName: "li",
            template: _.template($('#item-template').html()),
            events: {
                "click": "edit"
            },
            initialize: function () {
                this.model.bind('change', this.onChange, this);
            },
            onChange: function (e) {
                var changed = e.changedAttributes();
                //it could be useful because I do not want to renderize in all cases
                if (changed.hasOwnProperty("Description") || changed.hasOwnProperty("Editing")) {
                    this.render();
                }
            },
            render: function () {
                $(this.el).html(this.template(this.model.toJSON()));
                return this;
            },
            edit: function () {
                this.model.edit();
            }
        });

        App.CategoryDetailView = Backbone.View.extend({
            container: $("#edit-category"),
            //el: $("#edit-category"), //it does not work because the container should shared by different views
            template: _.template($('#edit-category-template').html()),
            initialize: function () {
            },
            events: {
                "change .viewtype": "updateViewType"
            },
            render: function () {
                $(this.el).html(this.template(this.model.toJSON()));
                this.container.html(this.el); //it is working but.... is it fine?
                return this;
            },
            updateViewType: function () {
                console.debug(this);
                this.model.set({ ViewType: this.$(".viewtype").val() });
                //unidirectional :(
                //I mean, I can do it: 
                //    `App.Categories.Instance.models[3].edit();`
                //    `App.Categories.Instance.models[3].set({ Description: "new description" });`
                //and it will be reflected in the UI instantly,
                //but, it does not:
                //    `App.Categories.Instance.models[3].set({ ViewType: "List" });`
            }
        })

        //App.CategoryDetailView.Instance = new App.CategoryDetailView();

        App.AppView = Backbone.View.extend({
            el: $("#categoriesConfigurationApp"),
            events: {
            },
            initialize: function () {
                App.Categories.Instance.bind('reset', this.resetItems, this);
                App.Categories.Instance.fetch();
                this.$categoryList = $("#category-list");
            },
            render: function () {
                //console.debug("render all");
            },
            resetItems: function () {
                var me = this;
                me.$categoryList.empty();
                App.Categories.Instance.each(function (cat) {
                    var view = new App.CategoryItemView({ model: cat });
                    me.$categoryList.append(view.render().el);
                });
                //this.$("#edit-category").empty().append(App.CategoryDetailView.Instance.render().el);
            }
        });

        App.AppView.Instance = new App.AppView();
    });