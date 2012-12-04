function cjSuggest(el, key) {
    var $el = $(el);
    el = $el[0];
    var _previousXHR = null;
    $el.typeahead({
        autoselect: false,
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
    $el.blur(function () {
        if (_previousXHR)
            _previousXHR.abort();
    });
}

function cjSuggestEmailDomain(el, key) {
    var $el = $(el);
    el = $el[0];
    var _previousXHR = null;
    $el.typeahead({
        autoselect: false,
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
    $el.blur(function () {
        if (_previousXHR)
            _previousXHR.abort();
    });
}

function cjSuggestMultiple_extractor(query) {
    var result = /([^,]+)$/.exec(query);
    if (result && result[1])
        return result[1].trim();
    return '';
}

function cjSuggestMultiple(el, key) {
    var $el = $(el);
    el = $el[0];
    var _previousXHR = null;
    $el.typeahead({
        autoselect: false,
        source: function (query, process) {
            if (_previousXHR)
                _previousXHR.abort();
            var term;
            if (query.length == el.selectionStart && query.length == el.selectionEnd && (term = cjSuggestMultiple_extractor(query))) {
                _previousXHR = $
                    .ajax({ url: urlGenerator.action(key, "Suggest", { term: term }) })
                    .done(function (data) {
                        if (data && data.suggestions)
                            process(data.suggestions);
                    });
            } else {
                return [];
            }
        },
        matcher: function () { return true; },
        updater: function (item) {
            return this.$element.val().replace(/[^,\s]*$/, '') + item + ', ';
        },
        highlighter: function (item) {
            var query = cjSuggestMultiple_extractor(this.query).replace(/[\-\[\]{}()*+?.,\\\^$|#\s]/g, '\\$&')
            return item.replace(new RegExp('(' + query + ')', 'ig'), function ($1, match) {
                return '<strong>' + match + '</strong>'
            })
        }
    });
    $el.blur(function () {
        if (_previousXHR)
            _previousXHR.abort();
    });
}
