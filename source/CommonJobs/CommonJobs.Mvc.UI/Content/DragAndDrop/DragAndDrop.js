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
                fileuploadOptions.done = myConfig.done;
            }
            if (myConfig.add) {
                fileuploadOptions.add = myConfig.add;
            }

            $input.fileupload(fileuploadOptions);
        });
    }
};


