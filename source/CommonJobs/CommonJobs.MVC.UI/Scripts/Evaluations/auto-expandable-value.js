(function (ko) {
    ko.bindingHandlers.autoExpandableValue = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                element.value = value();
            }
            ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor);
        },
        update: function (element, valueAccessor) {
            ko.bindingHandlers.value.update(element, valueAccessor);
            element.style.height = "1px";
            var scrollHeight = element.scrollHeight == 0 ? 28 : element.scrollHeight
            element.style.height = 25 + scrollHeight + "px";
        }
    };
})(this.ko)
