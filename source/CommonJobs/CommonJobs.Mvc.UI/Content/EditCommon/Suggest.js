function InitializeSuggest() {
    $(".editable-field[data-cj-suggest]").each(function () {
        var $el = $(this);
        var key = $el.attr("data-cj-suggest");
        $el.find(".editor-editable").typeahead({
            source: function (query, process) {
                $.ajax({
                    url: urlGenerator.action(key, "Suggest", { term: query }),
                    success: function (data) {
                        if (data && data.suggestions)
                            process(data.suggestions);
                    }
                });
            }
        , matcher: function () { return true; }
        });
    });

    $(".editable-field[data-cj-suggest-emaildomain]").each(function () {
        var $el = $(this);
        var key = $el.attr("data-cj-suggest-emaildomain");
        $el.find(".editor-editable").typeahead({
            source: function (query, process) {
                if (query && query.indexOf("@") != -1 && query.indexOf("@") < query.length - 1) {
                    $.ajax({
                        url: urlGenerator.action(key, "Suggest", { term: query }),
                        success: function (data) {
                            if (data && data.suggestions)
                                process(data.suggestions);
                        }
                    });
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
        $input.typeahead({
            source: function (query, process) {
                var input = $input[0];
                var term;
                if (query.length == input.selectionStart && query.length == input.selectionEnd && (term = extractor(query))) {
                    $.ajax({
                        url: urlGenerator.action(key, "Suggest", { term: term }),
                        success: function (data) {
                            if (data && data.suggestions)
                                process(data.suggestions);
                        }
                    });
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