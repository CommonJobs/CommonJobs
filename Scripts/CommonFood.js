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
            this.title = ko.observable("");
            this.weeks = ko.observable(0);
            this.days = ko.observableArray();
            this.options = ko.observableArray();
            this.places = ko.observableArray();
            this.startDate = ko.observable("");
            this.endDate = ko.observable("");
            this.firstWeek = ko.observable(0);
            this.firstDay = ko.observable(0);
            this.foods = ko.observableArray();
            this.reset(model);
        }
        MenuViewModel.defaultModel = {
            title: "Nuevo Menú",
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
            places: [
                "La Rioja", 
                "Garay"
            ],
            startDate: "",
            endDate: "",
            foods: []
        };
        MenuViewModel.prototype.reset = function (model) {
            model = $.extend({
            }, MenuViewModel.defaultModel, model);
            this.title(model.title);
            this.weeks(0);
            this.days([]);
            this.options([]);
            this.places([]);
            this.startDate(model.startDate);
            this.endDate(model.endDate);
            this.firstWeek(model.firstWeek);
            this.firstDay(model.firstDay);
            this.foods([]);
            for(var s in model.options) {
                this.addOption(model.options[s]);
            }
            for(var s in model.places) {
                this.addPlace(model.places[s]);
            }
            for(var s in model.days) {
                this.addDay(model.days[s]);
            }
            for(var i = 0; i < model.weeks; i++) {
                this.addWeek();
            }
        };
        MenuViewModel.prototype.exportModel = function () {
            var _this = this;
            var model = {
            };
            var simpleProperties = [
                "title", 
                "firstWeek", 
                "firstDay", 
                "weeks", 
                "startDate", 
                "endDate"
            ];
            _.each(simpleProperties, function (prop) {
                model[prop] = _this[prop]();
            });
            var textArrProperties = [
                "days", 
                "options", 
                "places"
            ];
            _.each(textArrProperties, function (prop) {
                model[prop] = _.map(_this[prop](), function (item) {
                    return item.text();
                });
            });
            this.eachDay(function (dayFoods, weekIndex, dayIndex) {
                _.each(dayFoods, function (option, optionIndex) {
                    console.log("(week: " + weekIndex + ", day: " + dayIndex + ", option: " + optionIndex + "): " + option());
                });
            });
            return model;
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
            _.each(this.foods(), function (weekFoods, weekIndex) {
                return f(weekFoods, weekIndex);
            });
        };
        MenuViewModel.prototype.eachDay = function (f) {
            this.eachWeek(function (weekFoods, weekIndex) {
                return _.each(weekFoods, function (dayFoods, dayIndex) {
                    return f(dayFoods, weekIndex, dayIndex);
                });
            });
        };
        MenuViewModel.prototype.generateText = function (baseName, collection, name) {
            var texts = _.map(ko.toJS(collection), function (item) {
                return item.text;
            });
            var n = texts.length + 1;
            if(_.isString(name) && name) {
                if(texts.indexOf(name) == -1) {
                    return name;
                } else {
                    baseName = name;
                    n = 2;
                }
            }
            while(true) {
                var name = baseName + n++;
                if(texts.indexOf(name) == -1) {
                    return name;
                }
            }
        };
        MenuViewModel.prototype.addOption = function (text) {
            text = this.generateText("Menú ", this.options, text);
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
        MenuViewModel.prototype.addPlace = function (text) {
            text = this.generateText("Lugar ", this.places, text);
            var place = {
                text: ko.observable(text)
            };
            this.places.push(place);
        };
        MenuViewModel.prototype.removePlace = function (place) {
            if(this.places().length) {
                var index = _.isNumber(place) ? place : this.places.indexOf(place);
                this.places.splice(index, 1);
            }
        };
        MenuViewModel.prototype.addDay = function (text) {
            var _this = this;
            text = this.generateText("Día ", this.days, text);
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

