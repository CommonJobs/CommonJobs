(function () {
    var App = this.App = { };

    App.publicUrlGenerator = new UrlGenerator(ViewData.publicSiteUrl);

    App.SharedLink = Backbone.Model.extend({
        defaults: function () {
            return {
                SharedCode: UrlGenerator.randomString(),
                ExpirationDate: moment().add('days', 3).format("YYYY-MM-DD")
            }
        },
        initialize: function () {
            this.on("add", this.added, this);
        },
        added: function () {
            if (!this.get('FriendlyName'))
                this.set('FriendlyName', "Link #" + this.collection.length);
        }
    });

    App.SharedLinks = Backbone.Collection.extend({
        model: App.SharedLink
    });

    App.JobSearch = Backbone.Model.extend({
        initCollectionField: function (fieldName, fieldType) {
            fieldType = fieldType || Backbone.Collection;
            this.set(fieldName, new fieldType(this.get(fieldName)));
            this.get(fieldName).on("add remove reset change", function () { this.trigger("change"); }, this);
            this.get(fieldName).parentModel = this;
        },
        initialize: function () {
            this.initCollectionField("SharedLinks", App.SharedLinks);
        }
    });

    App.EditJobSearchAppViewDataBinder = Nervoustissue.FormBinder.extend({
       dataBindings: {
           Title: { controlLink: "Text" },
           PublicNotes: { controlLink: "Markdown" },
           PrivateNotes: { controlLink: "Markdown" },
           IsPublic: {
               controlLink: "Toggle",
               onTemplate: _.template('<span class="checkmark checked" title="Público">&#x2713;</span>'),
               offTemplate: _.template('<span class="checkmark nonChecked" title="No público">&#x2713;</span>')
           },
           SharedLinks:
            {
                controlLink: "Collection",
                item: {
                    controlLink: "Compound",
                    template: _.template('<span data-bind="Link"></span> (<span data-bind="ExpirationDate"></span>)'),
                    items:
                    [
                        { 
                            controlLink: "LinkEditableText", name: "Link", dataLink: "UrlLink", textField: "FriendlyName", urlField: "SharedCode",
                            valueToContent: function (value) {
                                return _.template('<span class="view-editable"><a href="<%= App.publicUrlGenerator.bySections([ "new", url ]) %>"><%= text %></a> <span class="icon-edit">&nbsp;</span></span>', value);
                            },
                        },
                        { controlLink: "Date", name: "ExpirationDate", field: "ExpirationDate", uiDateFormat: "d/m/y" }
                    ]
                }
            }
        } 
    });

    App.EditJobSearchAppView = Backbone.View.extend({
        setModel: function (model) {
            this.model = model;
            this.dataBinder.setModel(model);
        },
        initialize: function () {
            this.dataBinder = new App.EditJobSearchAppViewDataBinder({ el: this.el, model: this.model });
            if (this.options.forceReadOnly) {
                this.$el.addClass("edition-force-readonly");
                this.editionReadOnly();
            }
        },
        events: {
            "click .saveJobSearch": "saveJobSearch",
            "click .reloadJobSearch": "reloadJobSearch",
            "click .editionNormal": "editionNormal",
            "click .editionReadOnly": "editionReadOnly",
            "click .editionFullEdit": "editionFullEdit",
            "click .deleteJobSearch": "deleteJobSearch"
        },
        saveJobSearch: function () {
            var me = this;
            $.ajax({
                url: urlGenerator.action("Post", "JobSearches"),
                type: "POST",
                dataType: "json",
                data: JSON.stringify(App.appView.model.toJSON()),
                contentType: "application/json; charset=utf-8",
                success: function(result) {
                    me.editionNormal();
                    me.setModel(new App.JobSearch(result));
                }
            });
        },
        reloadJobSearch: function () {
            var me = this;
            $.ajax({
                url: urlGenerator.action("Get", "JobSearches"),
                type: "GET",
                dataType: "json",
                data: { id: ViewData.jobSearch.Id },
                contentType: "application/json; charset=utf-8",
                success: function(result) {
                    me.editionNormal();
                    me.setModel(new App.JobSearch(result));
                }
            });
        },
        deleteJobSearch: function () {
            if (confirm("¿Está seguro de que desea eliminar esta búsqueda?")) {
                window.location = urlGenerator.action("Delete", "JobSearches", this.model.get("Id"));
            }
        },
        editionNormal: function() {
            if (this.options.forceReadOnly) {
                this.editionReadOnly();
            } else {
                this.dataBinder.editionMode("normal");
                this.$el.removeClass("edition-readonly edition-full-edit");
                this.$el.addClass("edition-normal");
            }
        },
        editionReadOnly: function () {
            this.dataBinder.editionMode("readonly");
            this.$el.removeClass("edition-normal edition-full-edit");
            this.$el.addClass("edition-readonly");
        },
        editionFullEdit: function () {
            if (this.options.forceReadOnly) {
                this.editionReadOnly();
            } else {
                this.dataBinder.editionMode("full-edit");
                this.$el.removeClass("edition-readonly edition-normal");
                this.$el.addClass("edition-full-edit");
            }
        },
    });
}).call(this);

$(function() {
    App.appView = new App.EditJobSearchAppView({
        el: $("#EditApp"),
        forceReadOnly: ViewData.forceReadOnly,
        model: new App.JobSearch(ViewData.jobSearch)
    });
});