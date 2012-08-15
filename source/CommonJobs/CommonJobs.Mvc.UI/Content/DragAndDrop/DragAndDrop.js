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
    $modal.removeClass("error");
    $modal.off("hide");
    var me = this;
    me.$modal = $modal;


    me.$ = function (selector, action) {
        var $el = $modal.find(selector);
        if (!action) {
            return $el;
        } else {
            action.apply($el);
            return me;
        }
    };

    me.modal = function (onHide) {
        if (onHide) {
            $modal.on("hide", onHide);
        }
        $modal.modal();
        return me;
    };

    me.person = function ($card) {
        return me
            .$("img.cardPicture", function () { this.remove(); })
            .$(".modal-header", function () { this.prepend($card.find("img.cardPicture").clone()) })
            .text(".person-name", $card.find(".name").text());
    };

    me.title = function (title) {
        return me.text(".title", title);
    };

    me.error = function () {
        $modal.addClass("error");
        return me;
    };

    me.files = function (data) {
        return me.$("ul.file-list", function () {
            var files = data.files;
            var key = "name";
            if (data.result && data.result.attachments) {
                var key = "FileName";
                files = data.result.attachments;
            }
            var html = [];
            for (var i in files) {
                html.push("<li>");
                html.push(files[i][key]);
                html.push("</li>");
            }
            this.html(html.join(""));
        });
    };

    me.show = function (selector) {
        return me.$(selector, function () { this.show(); });
    };

    me.hide = function (selector) {
        return me.$(selector, function () { this.hide(); });
    };

    me.text = function (selector, value) {
        return me.$(selector, function () { this.text(value); });
    };

    me.attr = function (selector, attr, value) {
        return me.$(selector, function () { this.attr(attr, value); });
    };
};

