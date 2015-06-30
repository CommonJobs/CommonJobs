function commonSuggest(el, key) {
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

function commonSort(header) {
    if (header && header.sortPropertyName) {
        if (header.activeSort()) {
            header.asc = !header.asc;
        } else {
            $.each(this.headers, function (i, e) {
                e.activeSort(false);
            });
            header.activeSort(true);
        }
        var prop = header.sortPropertyName;
        var ascSort = function (a, b) {
            return a[prop] < b[prop] ? -1 : a[prop] > b[prop] ? 1 : a[prop] == b[prop] ? 0 : 0;
        };
        var descSort = function (a, b) {
            return ascSort(b, a);
        };
        var sortFunc = header.asc ? ascSort : descSort;
        this.items.sort(sortFunc);
    }
};