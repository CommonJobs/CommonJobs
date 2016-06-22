(function ($, ko) {
    ko.bindingHandlers.heightUpdater = {
        update: function (element) {
            $(element).height(1);
            $(element).height(25 + element.scrollHeight);
        },
    };
})(jQuery, this.ko)
