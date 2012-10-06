function InitializeSuggest() {
    //TODO: refactorize

    $(".editable-field[data-cj-suggest]").each(function () {
        var $el = $(this);
        var key = $el.attr("data-cj-suggest");
        var _previousXHR = null;
        $el.find(".editor-editable").typeahead({
            source: function (query, process) {
                if (_previousXHR)
                    _previousXHR.abort();
                _previousXHR = $
                    .ajax({ url: urlGenerator.action(key, "Suggest", { term: query }) })
                    .done(function (data) {
                        if (data && data.suggestions)
                            process(data.suggestions);
                    });
            }
        , matcher: function () { return true; }
        });
    });

    $(".editable-field[data-cj-suggest-emaildomain]").each(function () {
        var $el = $(this);
        var key = $el.attr("data-cj-suggest-emaildomain");
        var _previousXHR = null;
        $el.find(".editor-editable").typeahead({
            source: function (query, process) {
                if (_previousXHR)
                    _previousXHR.abort();
                if (query && query.indexOf("@") != -1 && query.indexOf("@") < query.length - 1) {
                    _previousXHR = $
                        .ajax({ url: urlGenerator.action(key, "Suggest", { term: query }) })
                        .done(function (data) {
                            if (data && data.suggestions)
                                process(data.suggestions);
                        });
                } else {
                    return [];
                }
            }
        , matcher: function () { return true; }
        });
    });

    function extractor(query) {
        var result = /([^,]+)$/.exec(query);
        if (result && result[1])
            return result[1].trim();
        return '';
    }

    $(".editable-field[data-cj-suggest-multiple]").each(function () {
        var $el = $(this);
        var key = $el.attr("data-cj-suggest-multiple");
        var $input = $el.find(".editor-editable");
        var _previousXHR = null;
        $input.typeahead({
            source: function (query, process) {
                if (_previousXHR)
                    _previousXHR.abort();
                var input = $input[0];
                var term;
                if (query.length == input.selectionStart && query.length == input.selectionEnd && (term = extractor(query))) {
                    _previousXHR = $
                        .ajax({ url: urlGenerator.action(key, "Suggest", { term: term }) })
                        .done(function (data) {
                            if (data && data.suggestions)
                                process(data.suggestions);
                        });
                } else {
                    return [];
                }
            }
            , matcher: function () { return true; }
            , updater: function (item) {
                return this.$element.val().replace(/[^,\s]*$/, '') + item + ', ';
            }
            , highlighter: function (item) {
                var query = extractor(this.query).replace(/[\-\[\]{}()*+?.,\\\^$|#\s]/g, '\\$&')
                return item.replace(new RegExp('(' + query + ')', 'ig'), function ($1, match) {
                    return '<strong>' + match + '</strong>'
                })
            }
        });
    });
}