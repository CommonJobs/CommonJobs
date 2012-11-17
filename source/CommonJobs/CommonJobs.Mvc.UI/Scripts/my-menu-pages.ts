///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='underscore.browser.d.ts' />

module Patches {
    //http://www.knockmeout.net/2012/05/quick-tip-skip-binding.html
    var miKo = <any>ko;
    miKo.bindingHandlers.stopBinding = {
        init: function () {
            return { controlsDescendantBindings: true };
        }
    };
    miKo.virtualElements.allowedBindings.stopBinding = true;

    miKo.bindingHandlers.time = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var $el = $(element);

            ko.utils.registerEventHandler(element, "blur", function (event) {
                var accessor = valueAccessor();
                if (miKo.isObservable(accessor)) {
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
}

declare var moment: any;

//From default example
$(document).ready(function () {
    moment.lang('es');
});