(function () {
    var App = this.App = { };
    App.publicUrlGenerator = new UrlGenerator(ViewData.publicSiteUrl);
    
    App.SharedLink = Backbone.Model.extend({
        defaults: function () {
        },
        initialize: function () {
            this.on("add", this.added, this);
        },
        added: function () {
            if (!this.get('FriendlyName'))
                this.set('FriendlyName', "Link #" + this.collection.length);
        }
    });

    App.TechnicalSkill = Backbone.Model.extend({
        defaults: {
            Name: "",
            Level: 0
        }
    });

    App.TechnicalSkills = Backbone.Collection.extend({
        model: App.TechnicalSkill
    });

    App.JobSearch = Backbone.Model.extend({
        initCollectionField: function (fieldName, fieldType) {
            fieldType = fieldType || Backbone.Collection;
            this.set(fieldName, new fieldType(this.get(fieldName)));
            this.get(fieldName).on("add remove reset change", function () { this.trigger("change"); }, this);
            this.get(fieldName).parentModel = this;
        },
        initialize: function () {
            this.initCollectionField("RequiredTechnicalSkills", App.TechnicalSkills);
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
            PublicCode: {
                controlLink: "Text",
                name: "PublicCode",
                template: _.template('<span class="view-editable" style="display: none;"></span><span class="editor-editable" style="display: none;"><%= App.publicUrlGenerator.bySections() %><input class="editor-editable" type="text" value="" /></span>'),
                readUI: function () {
                    return this.$editor.filter("input").val();
                },
                dataEmpty: function () { return false; },
                valueToContent: function (value) {
                    if (!value) value = "nuevaBusqueda";
                    var url = App.publicUrlGenerator.bySections([value]);
                    return _.template('<span class="view-editable"><a href="<%= url %>"><%= url %></a> <span class="icon-edit">&nbsp;</span></span>', { url: url });
                },
            },
            RequiredTechnicalSkills: {
                controlLink: "Collection",
                item: {
                    controlLink: "Compound",
                    template: _.template('<span class="technical-skill-name" data-cj-suggest="TechnicalSkillName" data-bind="SkillName"></span>: <span class="technical-skill-level" data-bind="SkillLevel"></span>'),
                    items: [
                        { controlLink: "Text", name: "SkillName", field: "Name" },
                        { controlLink: "Options", name: "SkillLevel", field: "Level", options: _.map(ViewData.technicalSkillLevels, function(s, i) { return { value: i, text: s }; }) }
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
            //TODO this does not fully work -- why?
            this.model.get('RequiredTechnicalSkills').on("add remove reset change", this.reloadSuggestedApplicants, this);
            if (this.options.forceReadOnly) {
                this.$el.addClass("edition-force-readonly");
                this.editionReadOnly();
            }
            this.reloadSuggestedApplicants();
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
                success: function (result) {
                    me.editionNormal();
                    me.setModel(new App.JobSearch(result));
                    me.reloadSuggestedApplicants();
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
                    me.reloadSuggestedApplicants();
                }
            });
        },
        reloadSuggestedApplicants: function () {
            var me = this;
            
            var requiredTechnicalSkills = _.map(this.model.get('RequiredTechnicalSkills').models, function (rts) {
                return {
                    Name: rts.get('Name'),
                    Level: rts.get('Level')
                };
            });

            var data = {};
            _.each(requiredTechnicalSkills, function (x, i) {
                if (x.Name) {
                    data["[" + i + "].Name"] = x.Name;
                    if (+x.Level) {
                        data["[" + i + "].Level"] = x.Level;
                    }
                }
            });

            $.ajax({
                url: urlGenerator.action("GetSuggestedApplicants", "JobSearches"),
                type: "GET",
                dataType: "json",
                data: data,
                contentType: "application/json; charset=utf-8",
                success: function (suggestedApplicants) {
                    var suggestedApplicantsTemplate = $("#suggested-applicants-tmpl").html();
                    var requiredSkills = _.map(me.model.get('RequiredTechnicalSkills').toJSON(), function (rts) {
                        return rts.Name;
                    });
                    var templateData = {
                        applicants: suggestedApplicants,
                        //TODO this is a mess -- fix for something more straightforward
                        applicantSkills:
                            _.sortBy(
                                _.uniq(
                                    _.flatten(
                                        _.map(suggestedApplicants, function (app) {
                                            return _.map(app.TechnicalSkills, function (ts) {
                                                return {
                                                    Name: ts.Name,
                                                    IsRequired: _.contains(requiredSkills, ts.Name)
                                                };
                                            });
                                        })
                                    ),
                                    false, // list is partially sorted, so sorted = false
                                    function (item) {
                                        return item.Name;
                                    }
                                ),
                                function (name) { return name; }
                            )
                    };
                    var renderedSuggestedApplicants = _.template(suggestedApplicantsTemplate, templateData);
                    me.$('.suggestedApplicants').html(renderedSuggestedApplicants);
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