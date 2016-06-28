(function ($, ko) {
    ko.bindingHandlers.triState = {
        update: function (element, valueAccessor) {
            var value = valueAccessor()();
            var input = $(element).children("input");
            if ($(input).length == 1 && $(input).prop("type") == "checkbox") {
                switch (value) {
                    case true:
                        $(element).before().removeClass("indeterminate");
                        $(element).before().addClass("checked");
                        $(input).prop("indeterminate", false);
                        $(input).prop("checked", true);
                        break;
                    case false:
                        $(element).before().removeClass("checked");
                        $(element).before().removeClass("indeterminate");
                        $(input).prop("indeterminate", false);
                        $(input).prop("checked", false);
                        break;
                    case null:
                        $(element).before().addClass("indeterminate");
                        $(input).prop("indeterminate", true);
                        $(input).prop("checked", true);
                        break;
                }
            }
        },
    };
})(jQuery, this.ko)
