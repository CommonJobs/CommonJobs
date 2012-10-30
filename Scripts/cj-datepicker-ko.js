ko.bindingHandlers.cjdatepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        //initialize datepicker with some optional options
        var options = allBindingsAccessor().datepickerOptions || {};
        $(element).cjdatepicker(options);

        //when a user changes the date, update the view model
        ko.utils.registerEventHandler(element, "changeDate", function (event) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                /*
                console.debug(event.moment);
                console.debug(event.date);
                console.debug(event.dateFormated);
                console.debug(event.dateIso);
                */
                value(event.date);
            }
        });
    },
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        $(element).cjdatepicker('setValue', value);
    }
};