(function ($, ko) {
    ko.bindingHandlers.triState = {
        update: function (element, valueAccessor) {
            var value = valueAccessor()();
            var checkboxContainer = $(element);
            var checkbox = $(checkboxContainer.children("input"));
            if (checkbox.length == 1 && checkbox.prop("type") == "checkbox") {
                switch (value) {
                    case true:
                        checkboxContainer.before()
                            .removeClass("indeterminate")
                            .addClass("checked");
                        checkbox
                            .prop("indeterminate", false)
                            .prop("checked", true);
                        break;
                    case false:
                        checkboxContainer.before()
                            .removeClass("checked")
                            .removeClass("indeterminate")
                        checkbox
                            .prop("indeterminate", false)
                            .prop("checked", false);
                        break;
                    case null:
                        checkboxContainer.before()
                            .addClass("indeterminate")
                        checkbox
                            .prop("indeterminate", true)
                            .prop("checked", true);
                        break;
                }
            }
        },
    };
})(jQuery, this.ko)
