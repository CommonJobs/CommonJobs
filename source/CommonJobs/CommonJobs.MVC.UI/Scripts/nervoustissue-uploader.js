(function () {
    var root = this;

    // Require Underscore, if we're on the server, and it's not already present.
    var _ = root._;
    if (!_ && (typeof require !== 'undefined')) _ = require('underscore');

    // Require Backbone, if we're on the server, and it's not already present.
    var Backbone = root.Backbone;
    if (!Backbone && (typeof require !== 'undefined')) Backbone = require('backbone');

    // Require Nervoustissue, if we're on the server, and it's not already present.
    var Nervoustissue = root.Nervoustissue;
    if (!Nervoustissue && (typeof require !== 'undefined')) Nervoustissue = require('nervoustissue');

    var $ = Nervoustissue._domLibrary;

    var m = Nervoustissue.Uploader = {};

    var getUniqueId = (function () {
        var id = 0;
        return function () { return id++; };
    })();

    /**
    * Class for uploading files, uploading itself is handled by child classes
    */
    var UploadHandlerAbstract = function (o) {
        this._options = {
            debug: false,
            action: '/upload.php',
            // maximum number of concurrent uploads        
            maxConnections: 999,
            onProgress: function (id, fileName, loaded, total) { },
            onComplete: function (id, fileName, response) { },
            onCancel: function (id, fileName) { }
        };
        _.extend(this._options, o);

        this._queue = [];
        // params for files in queue
        this._params = [];
    };

    UploadHandlerAbstract.prototype = {
        log: function (str) {
            if (this._options.debug && window.console) console.log('[uploader] ' + str);
        },
        /**
        * Adds file or file input to the queue
        * @returns id
        **/
        add: function (file) { },
        /**
        * Sends the file identified by id and additional query params to the server
        */
        upload: function (id, params) {
            var len = this._queue.push(id);

            var copy = {};
            _.extend(copy, params);
            this._params[id] = copy;

            // if too many active uploads, wait...
            if (len <= this._options.maxConnections) {
                this._upload(id, this._params[id]);
            }
        },
        /**
        * Cancels file upload by id
        */
        cancel: function (id) {
            this._cancel(id);
            this._dequeue(id);
        },
        /**
        * Cancells all uploads
        */
        cancelAll: function () {
            for (var i = 0; i < this._queue.length; i++) {
                this._cancel(this._queue[i]);
            }
            this._queue = [];
        },
        /**
        * Returns name of the file identified by id
        */
        getName: function (id) { },
        /**
        * Returns size of the file identified by id
        */
        getSize: function (id) { },
        /**
        * Returns id of files being uploaded or
        * waiting for their turn
        */
        getQueue: function () {
            return this._queue;
        },
        /**
        * Actual upload method
        */
        _upload: function (id) { },
        /**
        * Actual cancel method
        */
        _cancel: function (id) { },
        /**
        * Removes element from queue, starts upload of next
        */
        _dequeue: function (id) {
            var i = _.indexOf(this._queue, id);
            this._queue.splice(i, 1);

            var max = this._options.maxConnections;

            if (this._queue.length >= max && i < max) {
                var nextId = this._queue[max - 1];
                this._upload(nextId, this._params[nextId]);
            }
        }
    };

    /**
    * Class for uploading files using form and iframe
    * @inherits qq.UploadHandlerAbstract
    */
    var UploadHandlerForm = function (o) {
        UploadHandlerAbstract.apply(this, arguments);

        this._inputs = {};
    };
    // @inherits qq.UploadHandlerAbstract
    _.extend(UploadHandlerForm.prototype, UploadHandlerAbstract.prototype);

    _.extend(UploadHandlerForm.prototype, {
        add: function (fileInput) {
            fileInput.setAttribute('name', 'qqfile');
            var id = 'qq-upload-handler-iframe' + getUniqueId();

            this._inputs[id] = fileInput;

            // remove file input from DOM
            if (fileInput.parentNode) {
                $(fileInput).remove();
            }

            return id;
        },
        getName: function (id) {
            // get input value and remove path to normalize
            return this._inputs[id].value.replace(/.*(\/|\\)/, "");
        },
        _cancel: function (id) {
            this._options.onCancel(id, this.getName(id));

            delete this._inputs[id];

            var iframe = $('#' + id);
            if (iframe) {
                // to cancel request set src to something else
                // we use src="javascript:false;" because it doesn't
                // trigger ie6 prompt on https
                iframe.setAttribute('src', 'javascript:false;');

                $(iframe).remove();
            }
        },
        _upload: function (id, params) {
            var input = this._inputs[id];

            if (!input) {
                throw new Error('file with passed id was not added, or already uploaded or cancelled');
            }

            var fileName = this.getName(id);

            var $iframe = this._createIframe(id);
            var $form = this._createForm($iframe, params);
            $form.append(input);
            
            //I think that it is not necessary
            //$form.append($("<input />").attr("name", "fileName").val($(input).val()));

            var self = this;
            this._attachLoadEvent($iframe, function () {
                self.log('iframe loaded');

                var response = self._getIframeContentJSON($iframe);

                self._options.onComplete(id, fileName, response);
                self._dequeue(id);

                delete self._inputs[id];
                // timeout added to fix busy state in FF3.6
                setTimeout(function () {
                    $iframe.remove();
                }, 1);
            });

            $form.submit();
            $form.remove();

            return id;
        },
        _attachLoadEvent: function ($iframe, callback) {
            $iframe.on('load', function () {
                var iframe = $iframe[0];
                // when we remove iframe from dom
                // the request stops, but in IE load
                // event fires
                if (!iframe.parentNode) {
                    return;
                }

                // fixing Opera 10.53
                if (iframe.contentDocument &&
                        iframe.contentDocument.body &&
                        iframe.contentDocument.body.innerHTML == "false") {
                    // In Opera event is fired second time
                    // when body.innerHTML changed from false
                    // to server response approx. after 1 sec
                    // when we upload file with iframe
                    return;
                }

                callback();
            });
        },
        /**
        * Returns json object received by iframe from server.
        */
        _getIframeContentJSON: function ($iframe) {

            var iframe = $iframe[0];
            // iframe.contentWindow.document - for IE<7
            var doc = iframe.contentDocument ? iframe.contentDocument : iframe.contentWindow.document,
                    response;

            this.log("converting iframe's innerHTML to JSON");
            this.log("innerHTML = " + doc.body.innerHTML);

            try {
                response = JSON.parse(doc.body.innerHTML);
            } catch (err) {
                //Ugly patch. I am getting something like that:
                //<html>
                //<head></head>
                //<body>
                //  <pre>{ "success": true, "attachment": { "Original": { "FileName": "System.Web-kzboe3yv.HttpPostedFileWrapper", "OriginalFileName": "System.Web.HttpPostedFileWrapper", "ContentType": "image/jpeg", "ContentLength": 2757, "Description": "Photo" }, "Thumbnail": { "FileName": "thumb_System.Web-kzboe3yv.HttpPostedFileWrapper", "OriginalFileName": "thumb_System.Web.HttpPostedFileWrapper", "ContentType": "image/jpeg", "ContentLength": 0, "Description": "Thumbnail" } } }</pre>
                //</body>
                //</html>
                try {
                    response = JSON.parse($(doc.body.innerHTML).text());
                } catch (err) {
                    response = {};
                }
            }

            return response;
        },
        /**
        * Creates iframe with unique name
        */
        _createIframe: function (id) {
            // We can't use following code as the name attribute
            // won't be properly registered in IE6, and new window
            // on form submit will open
            // var iframe = document.createElement('iframe');
            // iframe.setAttribute('name', id);

            var $iframe =
                $('<iframe src="javascript:false;" name="' + id + '"/>') // src="javascript:false;" removes ie6 prompt on https
                .attr('id', id)
                .hide();

            $('body').append($iframe);
            return $iframe;
        },
        /**
        * Creates form, that will be submitted to iframe
        */
        _createForm: function ($iframe, params) {
            // We can't use the following code in IE6
            // var form = document.createElement('form');
            // form.setAttribute('method', 'post');
            // form.setAttribute('enctype', 'multipart/form-data');
            // Because in this case file won't be attached to request

            var serializedParams = $.param(params);
            var prefix = this._options.action.indexOf('?') < 0 ? '?' : '&';
            var queryString = this._options.action + prefix + serializedParams;

            var $form =
                $('<form method="post" enctype="multipart/form-data"></form>')
                .attr('action', queryString)
                .attr('target', $iframe.attr("name"))
                .hide();

            $('body').append($form);
            return $form;
        }
    });

    /**
    * Class for uploading files using xhr
    * @inherits qq.UploadHandlerAbstract
    */
    var UploadHandlerXhr = function (o) {
        UploadHandlerAbstract.apply(this, arguments);

        this._files = [];
        this._xhrs = [];

        // current loaded size in bytes for each file 
        this._loaded = [];
    };

    // static method
    UploadHandlerXhr.isSupported = function () {
        var input = document.createElement('input');
        input.type = 'file';

        return (
                'multiple' in input &&
                typeof File != "undefined" &&
                typeof (new XMLHttpRequest()).upload != "undefined");
    };

    // @inherits qq.UploadHandlerAbstract
    _.extend(UploadHandlerXhr.prototype, UploadHandlerAbstract.prototype)

    _.extend(UploadHandlerXhr.prototype, {
        /**
        * Adds file to the queue
        * Returns id to use with upload, cancel
        **/
        add: function (file) {
            if (!(file instanceof File)) {
                throw new Error('Passed obj in not a File (in qq.UploadHandlerXhr)');
            }

            return this._files.push(file) - 1;
        },
        getName: function (id) {
            var file = this._files[id];
            // fix missing name in Safari 4
            return file.fileName != null ? file.fileName : file.name;
        },
        getSize: function (id) {
            var file = this._files[id];
            return file.fileSize != null ? file.fileSize : file.size;
        },
        /**
        * Returns uploaded bytes for file identified by id 
        */
        getLoaded: function (id) {
            return this._loaded[id] || 0;
        },
        /**
        * Sends the file identified by id and additional query params to the server
        * @param {Object} params name-value string pairs
        */
        _upload: function (id, params) {
            var file = this._files[id],
                    name = this.getName(id),
                    size = this.getSize(id);

            this._loaded[id] = 0;

            var xhr = this._xhrs[id] = new XMLHttpRequest();
            var self = this;

            xhr.upload.onprogress = function (e) {
                if (e.lengthComputable) {
                    self._loaded[id] = e.loaded;
                    self._options.onProgress(id, name, e.loaded, e.total);
                }
            };

            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4) {
                    self._onComplete(id, xhr);
                }
            };

            // build query string
            params = params || {};
            params['fileName'] = name;
            var serializedParams = $.param(params);
            var prefix = this._options.action.indexOf('?') < 0 ? '?' : '&';
            var queryString = this._options.action + prefix + serializedParams;

            xhr.open("POST", queryString, true);
            xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
            xhr.setRequestHeader("X-File-Name", encodeURIComponent(name));
            xhr.setRequestHeader("Content-Type", "application/octet-stream");
            xhr.send(file);
        },
        _onComplete: function (id, xhr) {
            // the request was aborted/cancelled
            if (!this._files[id]) return;

            var name = this.getName(id);
            var size = this.getSize(id);

            this._options.onProgress(id, name, size, size);

            if (xhr.status == 200) {
                this.log("xhr - server response received");
                this.log("responseText = " + xhr.responseText);

                var response;

                try {
                    response = eval("(" + xhr.responseText + ")");
                } catch (err) {
                    response = {};
                }

                this._options.onComplete(id, name, response);

            } else {
                this._options.onComplete(id, name, {});
            }

            this._files[id] = null;
            this._xhrs[id] = null;
            this._dequeue(id);
        },
        _cancel: function (id) {
            this._options.onCancel(id, this.getName(id));

            this._files[id] = null;

            if (this._xhrs[id]) {
                this._xhrs[id].abort();
                this._xhrs[id] = null;
            }
        }
    });

    /**
    * Creates upload button, validates upload, but doesn't create file list or dd. 
    */
    m.UploaderElement = function (element, o) {
        this._options = {
            // set to true to see the server response
            debug: false,
            action: '/server/upload',
            params: {},
            multiple: true,
            maxConnections: 3,
            // validation        
            allowedExtensions: [],
            sizeLimit: 0,
            minSizeLimit: 0,
            // events
            // return false to cancel submit
            onSubmit: function (id, fileName) { },
            onProgress: function (id, fileName, loaded, total) { },
            onComplete: function (id, fileName, responseJSON) { },
            onCancel: function (id, fileName) { },
            // messages                
            messages: {
                typeError: "{file} has invalid extension. Only {extensions} are allowed.",
                sizeError: "{file} is too large, maximum file size is {sizeLimit}.",
                minSizeError: "{file} is too small, minimum file size is {minSizeLimit}.",
                emptyError: "{file} is empty, please select files again without it.",
                onLeave: "The files are being uploaded, if you leave now the upload will be cancelled."
            },
            showMessage: function (message) {
                alert(message);
            },
            hoverClass: 'qq-upload-button-hover',
            focusClass: 'qq-upload-button-focus'
        };
        _.extend(this._options, o);

        // number of files being uploaded
        this._filesInProgress = 0;
        this._handler = this._createUploadHandler();

        this.el = $(element)[0];
        this.$el = $(element);
        this._prepareElement();
        this.resetButton();

        this._preventLeaveInProgress();
    };

    m.UploaderElement.prototype = {
        disable: function () {
            this.$input.hide();
        },
        enable: function () {
            this.$input.show();
        },
        _prepareElement: function () {
            this.$el.css({
                position: 'relative',
                overflow: 'hidden',
                direction: 'ltr' // Make sure browse button is in the right side in Internet Explorer
            });
            // Solo soporta block o inline-block
            if (this.$el.css("display") == "inline") {
                this.$el.css("display", "inline-block");
            }
        },
        resetButton: function () {
            if (this.$input) {
                this.$input.remove();
                this.input = this.$input = null;
            };
            this.$el.removeClass(this._options.focusClass);
            this._createInput();
        },
        _createInput: function () {
            var me = this;
            var $input = this.$input =
                $("<input />")
                .attr("type", "file")
                .attr("name", "file")
                .css({
                    position: 'absolute',
                    right: 0, // in Opera only 'browse' button is clickable and it is located at the right side of the input
                    top: 0,
                    fontFamily: 'Arial',
                    fontSize: '118px', // 4 persons reported this, the max values that worked for them were 243, 236, 236, 118
                    margin: 0,
                    padding: 0,
                    cursor: 'pointer',
                    opacity: 0
                });
            this.input = $input[0];
            if (this._options.multiple) {
                $input.attr("multiple", "multiple");
            }
            if (this._options.accept) {
                $input.attr("accept", this._options.accept);
            }

            this.$el.append($input);

            $input.on('change', function () {
                me._onInputChange();
            });

            $input.on('mouseover', function () {
                me.$el.addClass(me._options.hoverClass);
            });
            $input.on('mouseout', function () {
                me.$el.removeClass(me._options.hoverClass);
            });
            $input.on('focus', function () {
                me.$el.addClass(me._options.focusClass);
            });
            $input.on('blur', function () {
                me.$el.removeClass(me._options.focusClass);
            });

            if (window.attachEvent) {
                // IE and Opera, unfortunately have 2 tab stops on file input which is unacceptable in our case, disable keyboard access
                $input.attr('tabIndex', "-1");
            }
        },
        setParams: function (params) {
            this._options.params = params;
        },
        getInProgress: function () {
            return this._filesInProgress;
        },
        _createUploadHandler: function () {
            var self = this,
                    handlerClass;

            if (UploadHandlerXhr.isSupported()) {
                handlerClass = UploadHandlerXhr;
            } else {
                handlerClass = UploadHandlerForm;
            }

            var handler = new handlerClass({
                debug: this._options.debug,
                action: this._options.action,
                maxConnections: this._options.maxConnections,
                onProgress: function (id, fileName, loaded, total) {
                    self._onProgress(id, fileName, loaded, total);
                    self._options.onProgress(id, fileName, loaded, total);
                },
                onComplete: function (id, fileName, result) {
                    self._onComplete(id, fileName, result);
                    self._options.onComplete(id, fileName, result);
                },
                onCancel: function (id, fileName) {
                    self._onCancel(id, fileName);
                    self._options.onCancel(id, fileName);
                }
            });

            return handler;
        },
        _preventLeaveInProgress: function () {
            var self = this;

            $(window).on('beforeunload', function (e) {
                if (!self._filesInProgress) { return; }
                // for ie, ff
                e.returnValue = self._options.messages.onLeave;
                // for webkit
                return self._options.messages.onLeave;
            });
        },
        _onSubmit: function (id, fileName) {
            this._filesInProgress++;
        },
        _onProgress: function (id, fileName, loaded, total) {
        },
        _onComplete: function (id, fileName, result) {
            this._filesInProgress--;
            if (result.error) {
                this._options.showMessage(result.error);
            }
        },
        _onCancel: function (id, fileName) {
            this._filesInProgress--;
        },
        _onInputChange: function () {
            if (this._handler instanceof UploadHandlerXhr) {
                this._uploadFileList(this.input.files);
            } else {
                if (this._validateFile(this.input)) {
                    this._uploadFile(this.input);
                }
            }
            this.resetButton();
        },
        _uploadFileList: function (files) {
            for (var i = 0; i < files.length; i++) {
                if (!this._validateFile(files[i])) {
                    return;
                }
            }

            for (var i = 0; i < files.length; i++) {
                this._uploadFile(files[i]);
            }
        },
        _uploadFile: function (fileContainer) {
            var id = this._handler.add(fileContainer);
            var fileName = this._handler.getName(id);

            if (this._options.onSubmit(id, fileName) !== false) {
                this._onSubmit(id, fileName);
                this._handler.upload(id, this._options.params);
            }
        },
        _validateFile: function (file) {
            var name, size;

            if (file.value) {
                // it is a file input            
                // get input value and remove path to normalize
                name = file.value.replace(/.*(\/|\\)/, "");
            } else {
                // fix missing properties in Safari
                name = file.fileName != null ? file.fileName : file.name;
                size = file.fileSize != null ? file.fileSize : file.size;
            }

            if (!this._isAllowedExtension(name)) {
                this._error('typeError', name);
                return false;

            } else if (size === 0) {
                this._error('emptyError', name);
                return false;

            } else if (size && this._options.sizeLimit && size > this._options.sizeLimit) {
                this._error('sizeError', name);
                return false;

            } else if (size && size < this._options.minSizeLimit) {
                this._error('minSizeError', name);
                return false;
            }

            return true;
        },
        _error: function (code, fileName) {
            var message = this._options.messages[code];
            function r(name, replacement) { message = message.replace(name, replacement); }

            r('{file}', this._formatFileName(fileName));
            r('{extensions}', this._options.allowedExtensions.join(', '));
            r('{sizeLimit}', this._formatSize(this._options.sizeLimit));
            r('{minSizeLimit}', this._formatSize(this._options.minSizeLimit));

            this._options.showMessage(message);
        },
        _formatFileName: function (name) {
            if (name.length > 33) {
                name = name.slice(0, 19) + '...' + name.slice(-13);
            }
            return name;
        },
        _isAllowedExtension: function (fileName) {
            var ext = (-1 !== fileName.indexOf('.')) ? fileName.replace(/.*[.]/, '').toLowerCase() : '';
            var allowed = this._options.allowedExtensions;

            if (!allowed.length) { return true; }

            for (var i = 0; i < allowed.length; i++) {
                if (allowed[i].toLowerCase() == ext) { return true; }
            }

            return false;
        },
        _formatSize: function (bytes) {
            var i = -1;
            do {
                bytes = bytes / 1024;
                i++;
            } while (bytes > 99);

            return Math.max(bytes, 0.1).toFixed(1) + ['kB', 'MB', 'GB', 'TB', 'PB', 'EB'][i];
        }
    };


    Nervoustissue.UILinking.Attachment = Nervoustissue.UILinking.BaseModel.extend({
        //TODO: Drag an drop
        template: _.template('<span class="upload-element">'
                           + '    <span class="view-editable-empty">Sin datos</span>'
                           + '</span>'
                           + '<span class="view-attached" style="display: none;">'
                           + '    <span class="view-editable-content"></span>'
                           + '    <button class="view-editable-clear">-</button>'
                           + '</span>'),
        attachedUrl: function (value) { return "#"; },
        valueToContent: function (value) {
            if (!value) { return ""; }
            return $("<a></a>")
                .attr("href", this.attachedUrl(value))
                .text(value.FileName);
        },
        readUI: null,
        update: null,
        allowedExtensions: [],
        accept: null,
        uploadUrl: "/server/upload",
        bindUI: function () {
            var me = this;
            me.$view = this.$(".view-attached");
            me.$(".view-editable-clear").on("click", function () { me.clearData(); });
            me.uploader = new Nervoustissue.Uploader.UploaderElement(me.$(".upload-element"), {
                action: _.isString(me.uploadUrl) ? me.uploadUrl : me.uploadUrl(),
                allowedExtensions: me.allowedExtensions,
                accept: me.accept,
                onComplete: function (id, fileName, responseJSON) {
                    if (responseJSON && responseJSON.success) {
                        me.linkedData.write(responseJSON.attachment);
                    }
                }
            });
            this._setupDragDrop();
        },
        applyMode: function (mode) {
            if (!mode) {
                mode = this.mode;
            }
            var formMode = this.viewDataBinder.editionMode();
            this.showView();
            if (formMode == "readonly") {
                this.uploader.disable();
                this.$clearButton.hide();
            } else {
                this.uploader.enable();
            }
        },
        _setupDragDrop: function () {
            //            var me = this;
            //            var dropArea = me.$(".upload-dropzone")[0];
            //            console.debug(dropArea);
            //            var dz = new Nervoustissue.Uploader.UploadDropZone({
            //                element: dropArea,
            //                onEnter: function (e) { console.debug(1); },
            //                onLeave: function (e) { console.debug(2); },
            //                // is not fired when leaving element by hovering descendants
            //                onLeaveNotDescendants: function (e) { console.debug(3); },
            //                onDrop: function (e) { console.debug(4); }
            //                /*,
            //                onEnter: function (e) {
            //                qq.addClass(dropArea, "dropActive");
            //                e.stopPropagation();
            //                },
            //                onLeave: function (e) {
            //                e.stopPropagation();
            //                },
            //                onLeaveNotDescendants: function (e) {
            //                qq.removeClass(dropArea, "dropActive");
            //                },
            //                onDrop: function (e) {
            //                console.debug(this);
            //                dropArea.style.display = 'none';
            //                qq.removeClass(dropArea, "dropActive");
            //                me.uploader._uploadFileList(e.dataTransfer.files);
            //                }*/
            //            });

            //            dropArea.style.display = 'none';

            //            qq.attach(document, 'dragenter', function (e) {
            //                if (!dz._isValidFileDrag(e)) return;
            //                console.debug("dragenter");
            //                dropArea.style.display = 'block';
            //            });
            //            qq.attach(document, 'dragleave', function (e) {
            //                if (!dz._isValidFileDrag(e)) return;

            //                var relatedTarget = document.elementFromPoint(e.clientX, e.clientY);
            //                // only fire when leaving document out
            //                if (!relatedTarget || relatedTarget.nodeName == "HTML") {
            //                    dropArea.style.display = 'none';
            //                }
            //            });
        }
    });

}).call(this);
