/// <reference path="../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../Scripts/backbone.js" />
/// <reference path="../Scripts/underscore.js" />

$(function () {
    window.ViewTypes = [
        { code: 'List', description: 'List' },
        { code: 'Grid', description: 'Grid' }
    ];

    window.Category = Backbone.Model.extend({
        defaults: function () {
            return {
                code: '',
                description: '',
                viewType: 'List',
                facets: [],
                editing: false
            };
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
            "click .categoryitem": "showDetails"
        },
        initialize: function () {
            this.model.bind('change', this.render, this);
        },
        render: function () {
            $(this.el).html(this.template(this.model.toJSON()));
            return this;
        },
        showDetails: function () {
            var model = this.model;
            Categories.each(function (cat) {
                cat.set({ editing: cat == model });
            });
        }
    });

    window.AppView = Backbone.View.extend({
        el: $("#categoriesConfigurationApp"),
        events: {
        },
        initialize: function () {
            Categories.bind('reset', this.addAll, this);
            Categories.fetch();
        },
        render: function () {
            //console.debug("render all");
        },
        addAll: function () {
            Categories.each(function (cat) {
                var view = new CategoryItemView({ model: cat });
                $("#category-list").append(view.render().el);
            });
        }
    });

    window.App = new AppView();
});