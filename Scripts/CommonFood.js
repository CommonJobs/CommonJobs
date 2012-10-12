var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
var CommonFood;
(function (CommonFood) {
    var HasCallbacks = (function () {
        function HasCallbacks() {
            var _this = this;
            var _constructor = (this).constructor;

            if(!_constructor.__cb__) {
                _constructor.__cb__ = {
                };
                for(var m in this) {
                    var fn = this[m];
                    if(typeof fn === 'function' && m.indexOf('__cb__') == -1) {
                        _constructor.__cb__[m] = fn;
                    }
                }
            }
            for(var m in _constructor.__cb__) {
                (function (m, fn) {
                    _this[m] = function () {
                        return fn.apply(_this, Array.prototype.slice.call(arguments));
                    };
                })(m, _constructor.__cb__[m]);
            }
        }
        return HasCallbacks;
    })();    
    var MenuViewModel = (function (_super) {
        __extends(MenuViewModel, _super);
        function MenuViewModel(model) {
                _super.call(this);
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
        MenuViewModel.prototype.addWeek = function () {
            var _this = this;
            var weekFoods = [];
            _.each(this.days(), function (day) {
                var dayFoods = [];
                _.each(_this.options(), function (option) {
                    return dayFoods.push(ko.observable(""));
                });
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
                return _.each(weekFoods, function (dayFoods) {
                    return f(dayFoods, weekFoods);
                });
            });
        };
        MenuViewModel.prototype.addOption = function (text) {
            text = _.isString(text) && text || "Menú " + (this.options().length + 1);
            var option = {
                text: ko.observable(text)
            };
            this.eachDay(function (dayFoods) {
                return dayFoods.push(ko.observable(""));
            });
            this.options.push(option);
        };
        MenuViewModel.prototype.removeOption = function (option) {
            if(this.options().length) {
                var index = _.isNumber(option) ? option : this.options.indexOf(option);
                this.eachDay(function (dayFoods) {
                    return dayFoods.splice(index, 1);
                });
                this.options.splice(index, 1);
            }
        };
        MenuViewModel.prototype.addDay = function (text) {
            var _this = this;
            text = _.isString(text) && text || "Día " + (this.options().length + 1);
            var day = {
                text: ko.observable(text)
            };
            this.eachWeek(function (weekFoods) {
                var dayFoods = [];
                _.each(_this.options(), function (option) {
                    return dayFoods.push(ko.observable(""));
                });
                weekFoods.push(dayFoods);
            });
            this.days.push(day);
        };
        return MenuViewModel;
    })(HasCallbacks);
    CommonFood.MenuViewModel = MenuViewModel;    
})(CommonFood || (CommonFood = {}));

