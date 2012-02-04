/// <reference path="../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../Scripts/backbone.js" />
/// <reference path="../Scripts/underscore.js" />
$(function () {
    App.Category = Backbone.Model.extend({
        defaults: function () {
            return {
                Code: '',
                ResultsViewType: 'List',
                Facets: [],
                Editing: false
            };
        },
        edit: function () {
            var me = this;
            this.collection.each(function (cat) {
                cat.set({ Editing: cat == me });
            });
            App.CategoryDetailView.instance.changeModel(me);
        }
    });

    App.Categories = Backbone.Collection.extend({
        model: App.Category
        //Commented because I am bootstraping now
        //, url: "/categories/list" 
    });

    App.Categories.instance = new App.Categories();

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
            if (changed.hasOwnProperty("Editing")) {
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
        el: $("#edit-category"),
        template: _.template($('#edit-category-template').html()),
        initialize: function () {
        },
        events: {
            "change .resultsviewtype": "updateResultsViewType"
        },
        render: function () {
            this.model 
                && $(this.el).html(this.template(this.model.toJSON())) 
                || $(this.el).empty();
            return this;
        },
        changeModel: function (model) {
            this.model = model;
            return this.render();
        },
        updateResultsViewType: function () {
            this.model.set({ ResultsViewType: this.$(".resultsviewtype").val() });
            //unidirectional :(
            //I mean, I can do it: 
            //    `App.Categories.instance.models[3].edit();`
            //and it will be reflected in the UI instantly,
            //but, it does not:
            //    `App.Categories.instance.models[3].set({ ResultsViewType: "List" });`
        }
    })

    App.CategoryDetailView.instance = new App.CategoryDetailView();

    App.AppView = Backbone.View.extend({
        el: $("#categoriesConfigurationApp"),
        events: {
        },
        initialize: function () {
            App.Categories.instance.bind('reset', this.resetItems, this);
            //Commented because I am bootstraping now
            //App.Categories.instance.fetch();
            App.Categories.instance.reset(App._Categories);
        },
        render: function () {
        },
        resetItems: function () {
            var $categorylist = this.$("#category-list")
            $categorylist.empty();
            App.Categories.instance.each(function (cat) {
                var view = new App.CategoryItemView({ model: cat });
                $categorylist.append(view.render().el);
            });
        }
    });

    App.AppView.instance = new App.AppView();
});