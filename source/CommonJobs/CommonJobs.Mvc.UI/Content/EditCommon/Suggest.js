function InitializeSuggest() {
    $(".editable-field[data-suggest]").each(function () {
        var $el = $(this);
        var key = $el.attr("data-suggest");
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
}