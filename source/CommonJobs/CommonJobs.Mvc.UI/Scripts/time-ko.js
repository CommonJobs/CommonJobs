ko.bindingHandlers.time = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var $el = $(element);

        ko.utils.registerEventHandler(element, "blur", function (event) {
            var accessor = valueAccessor();
            if (ko.isObservable(accessor)) {
                var value = moment($el.val(), "HH:mm").format("HH:mm");
                accessor(value);
            }
        });
    },
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        $(element).val(moment(value, "HH:mm").format("HH:mm"));
    }
};