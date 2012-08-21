/// <reference path="../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../Scripts/underscore.js" />
/// <reference path="../../Scripts/backbone.js" />
/// <reference path="../../Scripts/url-generator.js" />

var DragAndDrop = function (config) {
    this._init(config);
};

DragAndDrop._staticInit = function () {
    $(document).bind('drop dragover', function (e) {
        //Para que un drag no controlado por nosotros no haga nada
        e.preventDefault();
    });
    //Para que solo se ejecute una vez
    DragAndDrop._staticInit = function () { };
};

DragAndDrop.prototype = {
    _defaultConfig: {
        hoverClass: "hover",
        singleFileUploads: false //si no, se hace un request por cada archivo
    },
    _init: function (config) {
        //Solo crear una instancia por cada dropzoneSelector
        var self = this;
        DragAndDrop._staticInit();
        self._config = $.extend({}, this._defaultConfig, config);
    },
    prepareFileDropzone: function (el, myConfig) {
        var self = this;
        myConfig = $.extend({}, self._config, myConfig);
        $(el).each(function () {
            var $el = $(this);
            var $input = $el.find("input[type='file']");
            if (!$input || $input.length != 1) {
                return;
            }

            var fileuploadOptions = {
                //TODO: no envía el nombre de archivo en IE
                dataType: 'json',
                singleFileUploads: myConfig.singleFileUploads,
                dropZone: $el,
                dragover: function (e, f) {
                    if (_.any(e.dataTransfer.types, function (x) { return x == "Files" })) {
                        $el.addClass(myConfig.hoverClass);
                        if (!$el.data("dragleaveSet")) {
                            $el.data("dragleaveSet", true);
                            $el.on("dragleave.XXfiledropzoneXX", function () {
                                $el.removeClass(myConfig.hoverClass);
                                $el.off("dragleave.XXfiledropzoneXX");
                                $el.data("dragleaveSet", false);
                            });
                        }
                    }
                },
                drop: function () {
                    $el.removeClass(self._config.hoverClass);
                }
            };

            if (myConfig.done) {
                fileuploadOptions.done = function (e, data) { myConfig.done(e, data, $el) };
            }
            if (myConfig.add) {
                fileuploadOptions.add = function (e, data) { myConfig.add(e, data, $el) };
            }
            if (myConfig.fail) {
                fileuploadOptions.fail = function (e, data) { myConfig.fail(e, data, $el) };
            }

            $input.fileupload(fileuploadOptions);
        });
    }
};

var UploadModal = function ($modal) {
    this._init($modal);
};

UploadModal.prototype = {
    _init: function ($modal) {
        $modal.removeClass("error");
        $modal.off("hide");
        this.$modal = $modal;
        this._files = null;
        this.data = null;
        this.hide(".detail-link");
    },
    $: function (selector, action) {
        var $el = this.$modal.find(selector);
        if (!action) {
            return $el;
        } else {
            action.apply($el);
            return this;
        }
    },
    modal: function (onHide) {
        if (onHide) {
            this.$modal.on("hide", onHide);
        }
        this.$modal.modal();
        return this;
    },
    person: function ($card) {
        return this
            .$("img.cardPicture", function () { this.remove(); })
            .$(".modal-header", function () { this.prepend($card.find("img.cardPicture").clone()) })
            .text(".person-name", $card.find(".name").text())
            .$(".detail-link", function () {
                this.attr("href", $card.find(".clickable-link").attr("href"));
            });
    },
    title: function (title) {
        return this.text(".title", title);
    },
    error: function () {
        this.$modal.addClass("error");
        return this;
    },
    files: function (data) {
        this.data = data;
        if (data.result && data.result.attachments) {
            this._files = _.map(data.result.attachments, function (x) { return { name: x.FileName }; });
        } else {
            this._files = _.map(data.files, function (x) { return { name: x.name }; });
        }
        var files = this._files;
        return this.$(".file-list", function () {
            var html = [];
            for (var i in files) {
                html.push("<pre>");
                html.push(files[i].name);
                html.push("</pre>");
            }
            this.html(html.join(""));
        });
    },
    show: function (selector) {
        return this.$(selector, function () { this.show(); });
    },
    hide: function (selector) {
        return this.$(selector, function () { this.hide(); });
    },
    text: function (selector, value) {
        return this.$(selector, function () { this.text(value); });
    },
    attr: function (selector, attr, value) {
        return this.$(selector, function () { this.attr(attr, value); });
    }
};

