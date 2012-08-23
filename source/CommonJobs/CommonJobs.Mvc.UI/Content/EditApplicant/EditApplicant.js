/// <reference path="../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../Scripts/underscore.js" />
/// <reference path="../../Scripts/backbone.js" />
/// <reference path="../../Scripts/moment.js" />

(function () {
    var App = this.App = {};

    App.Note = Backbone.Model.extend({
        defaults: function () {
            return {
                RealDate: new Date().toJSON(),
                //TODO: move RegisterDate to a better place
                RegisterDate: new Date().toJSON(),
                Note: "",
                NoteType: 0,
                Attachment: null
            }
        }
    });

    App.Notes = Backbone.Collection.extend({
        model: App.Note
    });
    
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

    App.Applicant = Backbone.Model.extend({
        defaults: function () {
            return { };
        },
        initCollectionField: function (fieldName, fieldType) {
            fieldType = fieldType || Backbone.Collection;
            this.set(fieldName, new fieldType(this.get(fieldName)));
            this.get(fieldName).on("add remove reset change", function () { this.trigger("change"); }, this);
            this.get(fieldName).parentModel = this;
        },
        initialize: function () {
            this.initCollectionField("Notes", App.Notes);
            this.initCollectionField("SharedLinks", App.SharedLinks);
            this.initCollectionField("CompanyHistory");
        }
    });

    var formatLongDateWithYears = function (value) {
        // date format: yyyy-mm-dd
        var year = value.substring(0, 4);
        var month = value.substring(5, 7);
        var day = value.substring(8, 10);

        var date = new Date(year, month - 1, day, 0, 0, 0, 0);
        var age = (new Date() - date) / 365.25 / 24 / 60 / 60 / 1000;
        var ageInt = parseInt(age);
        var casi = "";
        if (age - ageInt > 0.7) {
            casi = "casi ";
            ageInt++;
        }
        var tiempo = ageInt < 1
                        ? "menos de un año"
                        : ageInt == 1
                            ? casi + "un año"
                            : casi + ageInt + " años";
        return Globalize.format(date, "d' de 'MMMM' de 'yyyy") + " (" + tiempo + ")";
    };

    var formatSalary = function (value) {
        return "$ " + value;
    };

    //TODO, a model with templates? Is this correct?
    Nervoustissue.UILinking.CjApplicantPicture = Nervoustissue.UILinking.Attachment.extend({
        //TODO: generalize it
        allowedExtensions: ["jpg", "jpeg", "gif", "png"],
        accept: "image/*",
        uploadUrl: function () { return urlGenerator.action("SavePhoto", "Applicants", this.model.get('Id')); },
        cropUrl: function (value, x, y, width, height) { return urlGenerator.action("CropImageAttachment", "Attachments", value.Original.Id, { x: parseInt(x), y: parseInt(y), width: parseInt(width), height: parseInt(height) }); },
        attachedUrl: function (value) { return urlGenerator.action("Get", "Attachments", value.Original.Id, { returnName: false }) },
        template: _.template('<div class="upload-element">'
                           + '    <img class="view-editable-empty" width="100" height="100" alt="No Photo" src="' + urlGenerator.content("Images/NoPicture.png") + '" title="No Photo" style="display:none"/>'
                           + '</div>'
                           + '<span class="view-attached" style="display: none;">'
                           + '    <div class="view-editable-content"></div>'
                           + '    <button class="view-editable-clear">&#x2717;</button>'
                           + '</span>'
                           + '<div class="cropDialog">'
                           + '    <div class="originalImage"></div>'
                           + '    <div class="croppedImage"></div>'
                           + '    <input type="hidden" class="cropX" />'
                           + '    <input type="hidden" class="cropY" />'
                           + '    <input type="hidden" class="cropWidth" />'
                           + '    <input type="hidden" class="cropHeight" />'
                           + '</div>'
                           ),
        valueToContent: function (value) {
            if (!value) { return ""; }
            return $("<a />")
                .attr("href", this.attachedUrl(value))
                .attr("target", "_blank")
                .addClass("photoLink")
                .append($("<img />").attr("src", urlGenerator.action("Get", "Attachments", value.Thumbnail.Id, { returnName: false }))
                .attr("width", "100").attr("height", "100"));
        },
        cropImage: function (attachment, x, y, w, h, callback) {
            var me = this;
            var cropUrl = me.cropUrl(attachment, x, y, w, h);
            $.ajax({
                url: cropUrl,
                success: function (data) { callback(data); }
            });
        },
        uploadFinished: function (attachment) {
            var me = this;
            var $cropDialog = this.$('.cropDialog');
            var url = this.attachedUrl(attachment);
            $cropDialog.find('.originalImage').append($('<img />').attr('src', url)).css({
                float: "left"
            });
            var $cropImage = $cropDialog.find('.croppedImage');
            $cropImage.append($('<img />').attr('src', url)).css({
                float: "left",
                width: 100,
                height: 100,
                overflow: 'hidden',
                'margin-left': 5
            });
            $cropDialog.dialog({
                buttons: {
                    "Ok": function () {
                        var x = $cropDialog.find('input.cropX').val();
                        var y = $cropDialog.find('input.cropY').val();
                        var w = $cropDialog.find('input.cropWidth').val();
                        var h = $cropDialog.find('input.cropHeight').val();
                        me.cropImage(attachment, x, y, w, h, function (newImage) {
                            $cropDialog.dialog("close");
                            me.linkedData.write(newImage);
                            me.refreshView();
                        });
                    },
                    "No cortar": function () {
                        $(this).dialog("close");
                    }
                },
                draggable: true,
                maxHeight: 500,
                modal: true,
                resizable: false,
                title: "Cortar imagen",
                width: '80%'
            });

            var $originalImage = $cropDialog.find('.originalImage img');
            var $preview = $cropDialog.find('.croppedImage img');

            $originalImage.Jcrop({
                aspectRatio: 1,
                onChange: function (coords) {
                    if (parseInt(coords.w) > 0) {
                        $cropDialog.find('input.cropX').val(coords.x);
                        $cropDialog.find('input.cropY').val(coords.y);
                        $cropDialog.find('input.cropWidth').val(coords.w);
                        $cropDialog.find('input.cropHeight').val(coords.h);

                        var rx = 100 / coords.w;
                        var ry = 100 / coords.h;

                        $preview.css({
                            width: Math.round(rx * parseInt($originalImage.css("width"))),
                            height: Math.round(ry * parseInt($originalImage.css("height"))),
                            marginLeft: '-' + Math.round(rx * coords.x) + 'px',
                            marginTop: '-' + Math.round(ry * coords.y) + 'px'
                        });
                    }
                }
            });
        }
    });

    Nervoustissue.UILinking.CjApplicantAttachment = Nervoustissue.UILinking.Attachment.extend({
        template: _.template('<span class="upload-element">'
                                   + '    <span class="view-editable-empty">Sin archivo adjunto</span>'
                                   + '</span>'
                                   + '<span class="view-attached" style="display: none;">'
                                   + '    Adjunto: <span class="view-editable-content"></span>'
                                   + '<button class="view-editable-clear">-</button>'
                                   + '</span>'),
        uploadUrl: function () { return urlGenerator.action("Post", "Attachments", /* TODO */this.model.collection.parentModel.get('Id')); },
        attachedUrl: function (value) { return urlGenerator.action("Get", "Attachments", value.Id); }
    });

    App.EditApplicantAppViewDataBinder = Nervoustissue.FormBinder.extend({
        dataBindings:
        {
            fullName:
            {
                controlLink: "Text",
                dataLink: "FullName",
                lastNameField: "LastName",
                firstNameField: "FirstName"
            },
            Photo: { controlLink: "CjApplicantPicture" },
            IsHighlighted:
            {
                controlLink: "Toggle",
                onTemplate: _.template('<img border="0" class="on" src="/Content/Images/Highlighted.png" alt="Resaltado" title="Resaltado">'),
                offTemplate: _.template('<img border="0" class="on" src="/Content/Images/Unhighlighted.png" alt="Resaltado" title="Resaltado">')
            },
            BirthDate: { controlLink: "Date", valueToContent: formatLongDateWithYears },
            MaritalStatus: { controlLink: "Options", options: [{ value: 0, text: "Soltero" }, { value: 1, text: "Casado" }, { value: 2, text: "Divorciado"}] },
            IsGraduated: { controlLink: "Options", options: [{ value: false, text: "No recibido" }, { value: true, text: "Recibido"}] },
            CompanyHistory:
            {
                controlLink: "Collection",
                item:
                {
                    controlLink: "Compound",
                    template: _.template('De <span data-bind="StartDate"></span> a <span data-bind="EndDate"></span>: <span data-bind="CompanyName"></span> (<span data-bind="IsCurrent"></span>)'),
                    items:
                    [
                        { controlLink: "Date", name: "StartDate", field: "StartDate" },
                        { controlLink: "Date", name: "EndDate", field: "EndDate" },
                        { controlLink: "Text", name: "CompanyName", field: "CompanyName" },
                        { controlLink: "Options", name: "IsCurrent", field: "IsCurrent", options: [{ value: false, text: "Anterior" }, { value: true, text: "Actual"}] }
                    ]
                }
            },
            Notes:
            {
                controlLink: "Collection",
                item:
                {
                    controlLink: "Compound",
                    template: _.template('<span data-bind="date"></span> (<span data-bind="NoteType"></span>) | <span data-bind="attachment"></span> <div data-bind="text"></div> '),
                    items:
                    [
                        { controlLink: "Date", name: "date", field: "RealDate" },
                        { controlLink: "CjApplicantAttachment", name: "attachment", field: "Attachment" },
                        { controlLink: "Markdown", name: "text", field: "Note" },
                        { controlLink: "Options", name: "NoteType", field: "NoteType", options: [{ value: 0, text: "Nota Genérica" }, { value: 1, text: "Nota de entrevista" }, { value: 2, text: "Nota de entrevista técnica"}] }
                    ]
                }
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
                                return _.template('<span class="view-editable"><a href="<%= urlGenerator.sharedAction("Edit", "Applicants", null, url) %>"><%= text %></a> <span class="icon-edit">&nbsp;</span></span>', value);
                            },
                        },
                        { controlLink: "Date", name: "ExpirationDate", field: "ExpirationDate", uiDateFormat: "d/m/y" }
                    ]
                }
            },
            LinkedInLink: {
                controlLink: "Text",
                name: "LinkedInLink",
                valueToContent: function (value) {
                    if (!value) return value;

                    return value + " <a href='" + value + "'>(visitar)</a>";
                }
            }
        }
    });


    App.EditApplicantAppView = Backbone.View.extend({
        setModel: function (model) {
            this.model = model;
            this.dataBinder.setModel(model);
        },
        initialize: function () {
            this.dataBinder = new App.EditApplicantAppViewDataBinder({ el: this.el, model: this.model });
            this.model.on("change:IsHighlighted", this.refreshHighlightedView, this);
            this.refreshHighlightedView();
            if (this.options.forceReadOnly) {
                this.$el.addClass("edition-force-readonly");
                this.editionReadonly();
            }   
        },
        events: {
            "click .saveApplicant": "saveApplicant",
            "click .reloadApplicant": "reloadApplicant",
            "click .editionNormal": "editionNormal",
            "click .editionReadonly": "editionReadonly",
            "click .editionFullEdit": "editionFullEdit",
            "click .deleteApplicant": "deleteApplicant"
        },
        saveApplicant: function () {
            var me = this;
            $.ajax({
                url: urlGenerator.action("Post", "Applicants"),
                type: 'POST',
                dataType: 'json',
                data: JSON.stringify(App.appView.model.toJSON()),
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    me.editionNormal();
                    me.setModel(new App.Applicant(result));
                }
            });
        },
        reloadApplicant: function () {
            var me = this;
            $.ajax({
                url: urlGenerator.action("Get", "Applicants"),
                type: 'GET',
                dataType: 'json',
                data: { id: ViewData.applicant.Id },
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    me.editionNormal();
                    me.setModel(new App.Applicant(result));
                }
            });
        },
        deleteApplicant: function () {
            if (confirm("¿Está seguro de que desea eliminar este postulante?")) {
                window.location = urlGenerator.action("Delete", "Applicants", this.model.get('Id'));
            }
        },
        editionNormal: function () {
            if (this.options.forceReadOnly) {
                this.editionReadonly();
            } else {
                this.dataBinder.editionMode("normal");
                this.$el.removeClass("edition-readonly edition-full-edit");
                this.$el.addClass("edition-normal");
            }
        },
        editionReadonly: function () {
            this.dataBinder.editionMode("readonly");
            this.$el.removeClass("edition-normal edition-full-edit");
            this.$el.addClass("edition-readonly");
        },
        editionFullEdit: function () {
            if (this.options.forceReadOnly) {
                this.editionReadonly();
            } else {
                this.dataBinder.editionMode("full-edit");
                this.$el.removeClass("edition-readonly edition-normal");
                this.$el.addClass("edition-full-edit");
            }
        },
        refreshHighlightedView: function () {
            //TODO how to avoid making references out of the view?
            var $highlight = $('body').add('.editing-bar', this.$el);
            if (this.model.get('IsHighlighted'))
                $highlight.addClass('highlighted');
            else
                $highlight.removeClass('highlighted');
        }
    });

}).call(this);


$(function () {
    App.appView = new App.EditApplicantAppView({
        el: $("#EditApp"),
        forceReadOnly: ViewData.forceReadOnly,
        model: new App.Applicant(ViewData.applicant)
    });
    
    $('#linkedinLink').attr('href', $('.editable-field[data-bind="LinkedinLink"] input').val());
    $('.editable-field[data-bind="LinkedinLink"] input').blur(function(){
       $('#linkedinLink').attr('href', $('.editable-field[data-bind="LinkedinLink"] input').val());
    });
});
