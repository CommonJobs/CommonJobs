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

    
}