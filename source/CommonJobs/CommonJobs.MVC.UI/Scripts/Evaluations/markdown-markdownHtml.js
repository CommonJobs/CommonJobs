(function ($, ko) {

    ko.bindingHandlers.markdownHtml = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var vm = {
                commentHtml: ko.computed(function () {
                    var markdown = new MarkdownDeep.Markdown();
                    if (valueAccessor()()) {
                        return markdown.Transform(valueAccessor()())
                    }
                    else {
                        return null;
                    }
                })
            };
            var newElement = $("<div data-bind='html: commentHtml'></div>")
                .appendTo(element)[0];
            ko.applyBindings(vm, newElement);
            return { controlsDescendantBindings: true };
        },
    };
})(jQuery, this.ko);
