var MenuViewModel = (function () {
    function MenuViewModel(model) {
        model = $.extend({
        }, MenuViewModel.defaultModel, model);
        this.title = ko.observable(model.title);
        this.weeks = ko.observable(0);
        this.days = ko.observableArray([]);
        this.options = ko.observableArray();
        this.startDate = ko.observable(model.startDate);
        this.endDate = ko.observable(model.endDate);
        this.firstWeek = ko.observable(model.firstWeek);
        this.firstDay = ko.observable(model.firstDay);
        this.foods = ko.observableArray();
        for(var s in model.options) {
            this.addOption(model.options[s]);
        }
        for(var s in model.days) {
            this.addDay(model.days[s]);
        }
        for(var i = 0; i < model.weeks; i++) {
            this.addWeek();
        }
    }
    MenuViewModel.defaultModel = {
        title: "",
        firstWeek: 0,
        firstDay: 0,
        weeks: 4,
        days: [
            "Lunes", 
            "Martes", 
            "Miercoles", 
            "Jueves", 
            "Viernes"
        ],
        options: [
            "Común", 
            "Light", 
            "Vegetariano"
        ],
        startDate: null,
        endDate: null,
        foods: []
    };
    MenuViewModel.prototype.exportModel = function () {
        return null;
    };
    MenuViewModel.prototype.getFood = function (weekIndex, dayIndex, optionIndex) {
        return this.foods()[weekIndex][dayIndex][optionIndex];
    };
    MenuViewModel.prototype.eachDayy = function (f) {
        _.each(this.days(), f, this);
    };
    MenuViewModel.prototype.addWeek = function () {
        var weekFoods = [];
        this.eachDayy(function (day) {
            var dayFoods = [];
            _.each(this.options(), function (option) {
                dayFoods.push(ko.observable(""));
            }, this);
            weekFoods.push(dayFoods);
        });
        this.foods.push(weekFoods);
        this.weeks(this.weeks() + 1);
    };
    MenuViewModel.prototype.removeWeek = function () {
        var actual = this.weeks();
        if(actual > 0) {
            this.weeks(actual - 1);
            this.foods.pop();
        }
    };
    MenuViewModel.prototype.eachWeek = function (f) {
        _.each(this.foods(), f);
    };
    MenuViewModel.prototype.eachDay = function (f) {
        this.eachWeek(function (weekFoods) {
            _.each(weekFoods, function (dayFoods) {
                f(dayFoods, weekFoods);
            });
        });
    };
    MenuViewModel.prototype.addOption = function (text) {
        text = _.isString(text) && text || "Menú " + (this.options().length + 1);
        var option = {
            text: ko.observable(text)
        };
        this.eachDay(function (dayFoods) {
            dayFoods.push(ko.observable(""));
        });
        this.options.push(option);
    };
    MenuViewModel.prototype.removeOption = function (option) {
        if(this.options().length) {
            var index = _.isNumber(option) ? option : this.options.indexOf(option);
            this.eachDay(function (dayFoods) {
                dayFoods.splice(index, 1);
            });
            this.options.splice(index, 1);
        }
    };
    MenuViewModel.prototype.addDay = function (text) {
        text = _.isString(text) && text || "Día " + (this.options().length + 1);
        var day = {
            text: ko.observable(text)
        };
        this.eachWeek(function (weekFoods) {
            var dayFoods = [];
            _.each(this.options(), function (option) {
                dayFoods.push(ko.observable(""));
            });
            weekFoods.push(dayFoods);
        });
        this.days.push(day);
    };
    return MenuViewModel;
})();
$(document).ready(function () {
    ko.applyBindings(new MenuViewModel({
        title: "Menú Primaveral",
        firstWeek: 1,
        firstDay: 4,
        weeks: 4,
        options: [
            "Común", 
            "Light", 
            "Vegetariano"
        ],
        startDate: new Date("2012-09-21"),
        endDate: new Date("2012-12-20")
    }));
});
