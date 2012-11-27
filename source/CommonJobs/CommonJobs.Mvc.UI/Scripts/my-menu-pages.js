var Patches;
(function (Patches) {
    var miKo = ko;
    miKo.bindingHandlers.stopBinding = {
        init: function () {
            return {
                controlsDescendantBindings: true
            };
        }
    };
    miKo.virtualElements.allowedBindings.stopBinding = true;
    miKo.bindingHandlers.time = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var $el = $(element);
            ko.utils.registerEventHandler(element, "blur", function (event) {
                var accessor = valueAccessor();
                if(miKo.isObservable(accessor)) {
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
})(Patches || (Patches = {}));

$(document).ready(function () {
    moment.lang('es');
});
