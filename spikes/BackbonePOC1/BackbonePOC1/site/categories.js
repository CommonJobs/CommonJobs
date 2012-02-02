/// <reference path="../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../Scripts/backbone.js" />
/// <reference path="../Scripts/underscore.js" />

window.ViewTypes = [
    { Code: 0, Description: 'List' },
    { Code: 1, Description: 'Grid' }
];

    $(function () {
        window.Category = Backbone.Model.extend({
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
                var view = new CategoryDetailView({ model: me });
                $("#edit-category").empty().append(view.render().el);
            }
        });

        window.CategoryList = Backbone.Collection.extend({
            model: Category,
            url: "/rest/categories"
        });

        //My Data
        window.Categories = new CategoryList();

        window.CategoryItemView = Backbone.View.extend({
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

        window.CategoryDetailView = Backbone.View.extend({
            template: _.template($('#edit-category-template').html()),
            events: {
                "change .viewType": "updateViewType"
            },
            render: function () {
                $(this.el).html(this.template(this.model.toJSON()));
                return this;
            },
            updateViewType: function () {
                this.model.set({ ViewType: this.$(".viewtype").val() });
                //unidirectional :(
                //I mean, I can do it: 
                //    `Categories.models[3].edit();`
                //    `Categories.models[3].set({ Description: "new description" });`
                //and it will be reflected in the UI instantly,
                //but, it does not:
                //    `Categories.models[3].set({ ViewType: "List" });`
            }
        })

        window.AppView = Backbone.View.extend({
            el: $("#categoriesConfigurationApp"),
            events: {
            },
            initialize: function () {
                Categories.bind('reset', this.resetItems, this);
                Categories.fetch();
            },
            render: function () {
                //console.debug("render all");
            },
            resetItems: function () {
                $("#category-list").empty();
                Categories.each(function (cat) {
                    var view = new CategoryItemView({ model: cat });
                    $("#category-list").append(view.render().el);
                });
            }
        });

        window.App = new AppView();
    });