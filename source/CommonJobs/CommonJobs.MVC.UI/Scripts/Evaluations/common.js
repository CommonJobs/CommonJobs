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
                if (e.sortable) {
                    e.activeSort(false);
                }
            });
            header.activeSort(true);
        }
        var prop = header.sortPropertyName;
        var defaultProp = header.defaultPropertyName;
        var ascSort = function (a, b) {
            var propertyA = (typeof a[prop] === "function") ? a[prop]() : a[prop];
            var propertyB = (typeof b[prop] === "function") ? b[prop]() : b[prop];
            var propertyDefaultA = (typeof a[defaultProp] === "function") ? a[defaultProp]() : a[defaultProp];
            var propertyDefaultB = (typeof b[defaultProp] === "function") ? b[defaultProp]() : b[defaultProp];
            propertyA = propertyA.toString().trim();
            propertyB = propertyB.toString().trim();
            propertyDefaultA = propertyDefaultA.toString().trim();
            propertyDefaultB = propertyDefaultB.toString().trim();
            var result = propertyA < propertyB ? -1 : propertyA > propertyB ? 1 : propertyDefaultA < propertyDefaultB ? -1 : propertyDefaultA > propertyDefaultB ? 1 : 0;
            return result;
        };
        var descSort = function (a, b) {
            return ascSort(b, a);
        };
        var sortFunc = header.asc ? ascSort : descSort;
        this.items.sort(sortFunc);
    }
};

function dirtyFlag() {
    var observable = ko.observable(false);
    observable.register = function (anotherObservable) {
        anotherObservable.subscribe(function () {
            observable(true);
        });
    };
    observable.reset = function () {
        return observable(false);
    };
    return observable;
}