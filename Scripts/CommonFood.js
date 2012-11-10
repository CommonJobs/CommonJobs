var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
var CommonFood;
(function (CommonFood) {
    CommonFood.days = (function () {
        var values = [
            1, 
            2, 
            3, 
            4, 
            5
        ];
        var def = function () {
            return values;
        };
        def["getName"] = function (day) {
            return moment().day(day).format("dddd");
        };
        def["isValid"] = function (day) {
            return values.indexOf(day) >= 0;
        };
        return def;
    })();
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
            this.options = ko.observableArray();
            this.places = ko.observableArray();
            this.startDate = ko.observable("");
            this.endDate = ko.observable("");
            this.deadlineTime = ko.observable("");
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
            options: [
                {
                    key: "default_comun",
                    text: "Común"
                }, 
                {
                    key: "default_light",
                    text: "Light"
                }, 
                {
                    key: "default_vegetariano",
                    text: "Vegetariano"
                }
            ],
            places: [
                {
                    key: "default_larioja",
                    text: "La Rioja"
                }, 
                {
                    key: "default_garay",
                    text: "Garay"
                }
            ],
            startDate: "",
            endDate: "",
            deadlineTime: "09:30",
            foods: []
        };
        MenuViewModel.prototype.createKeyTextObservableArray = function (items) {
            return ko.observableArray(_.map(items, function (item) {
                return {
                    key: item.key,
                    text: ko.observable(item.text)
                };
            }));
        };
        MenuViewModel.prototype.reset = function (model) {
            model = $.extend({
            }, MenuViewModel.defaultModel, model);
            var i;
            this.title(model.title);
            this.weeks(0);
            this.options([]);
            this.places([]);
            this.startDate(model.startDate);
            this.endDate(model.endDate);
            this.firstWeek(model.firstWeek);
            this.firstDay(model.firstDay);
            this.deadlineTime(model.deadlineTime);
            this.foods.removeAll();
            for(i in model.options) {
                this.addOption(model.options[i]);
            }
            for(i in model.places) {
                this.addPlace(model.places[i]);
            }
            for(i = 0; i < model.weeks; i++) {
                this.addWeek();
            }
            var weeksLength = this.weeks();
            var opts = {
            };
            _.each(this.options(), function (x) {
                opts[x.key] = true;
            });
            var foods = this.foods();
            if(model.foods) {
                _.each(model.foods, function (item) {
                    if(CommonFood.days.isValid(item.day) && item.week < weeksLength && opts[item.opt]) {
                        foods[item.week][item.day][item.opt](item.food);
                    }
                });
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
            model.places = ko.toJS(this.places);
            model.options = ko.toJS(this.options);
            var foods = model.foods = [];
            this.eachDay(function (dayFoods, weekIndex, dayIndex) {
                for(var opt in dayFoods) {
                    var food = dayFoods[opt]();
                    if(food) {
                        foods.push({
                            week: weekIndex,
                            day: dayIndex,
                            opt: opt,
                            food: food
                        });
                    }
                }
            });
            return model;
        };
        MenuViewModel.prototype.getFood = function (weekIndex, dayIndex, opt) {
            return this.foods()[weekIndex][dayIndex][opt];
        };
        MenuViewModel.prototype.addWeek = function () {
            var weekFoods = [];
            for(var i = 0; i < 7; i++) {
                var dayFoods = {
                };
                _.each(this.options(), function (option) {
                    return dayFoods[option.key] = ko.observable("");
                });
                weekFoods.push(dayFoods);
            }
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
            if(name) {
                if((name).text) {
                    name = (name).text;
                }
                if(_.isString(name)) {
                    if(texts.indexOf(name) == -1) {
                        return name;
                    } else {
                        baseName = name;
                        n = 2;
                    }
                }
            }
            while(true) {
                var name = baseName + n++;
                if(texts.indexOf(name) == -1) {
                    return name;
                }
            }
        };
        MenuViewModel.prototype.addKeyObservableText = function (collection, baseName, value) {
            var item;
            if(!value || !value.key) {
                var text = this.generateText(baseName, collection, value);
                item = {
                    key: text,
                    text: ko.observable(text)
                };
            } else {
                item = {
                    key: value.text,
                    text: ko.observable(value.text)
                };
            }
            collection.push(item);
            return item;
        };
        MenuViewModel.prototype.addOption = function (option) {
            var op = this.addKeyObservableText(this.options, "Menú ", option);
            this.eachDay(function (dayFoods) {
                dayFoods[op.key] = ko.observable("");
            });
        };
        MenuViewModel.prototype.removeOption = function (option) {
            var removed = this.removeItem(this.options, option);
            if(removed) {
                this.eachDay(function (dayFoods) {
                    return delete dayFoods[removed.key];
                });
            }
        };
        MenuViewModel.prototype.addPlace = function (place) {
            this.addKeyObservableText(this.places, "Lugar ", place);
        };
        MenuViewModel.prototype.removeItem = function (collection, item) {
            if(!collection().length) {
                return null;
            }
            if(_.isString(item)) {
                return collection.remove(function (x) {
                    return x.key == item;
                });
            }
            var index = _.isNumber(item) ? item : collection.indexOf(item);
            return collection.splice(index, 1)[0];
        };
        MenuViewModel.prototype.removePlace = function (place) {
            this.removeItem(this.places, place);
        };
        return MenuViewModel;
    })(HasCallbacks);
    CommonFood.MenuViewModel = MenuViewModel;    
})(CommonFood || (CommonFood = {}));

