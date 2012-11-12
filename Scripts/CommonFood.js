var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
var Utilities;
(function (Utilities) {
    var IdGenerator = (function () {
        function IdGenerator(chars, length) {
            if (typeof chars === "undefined") { chars = "abcdefghijklmnopqrstuvwxyz1234567890"; }
            if (typeof length === "undefined") { length = 8; }
            this.chars = chars;
            this.length = length;
        }
        IdGenerator.prototype.generate = function (prefix) {
            if (typeof prefix === "undefined") { prefix = ""; }
            var chars = this.chars;
            var charsL = chars.length;
            return prefix + _.map(_.range(this.length), function () {
                return chars[_.random(0, charsL)];
            }).join("");
        };
        return IdGenerator;
    })();
    Utilities.IdGenerator = IdGenerator;    
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
    Utilities.HasCallbacks = HasCallbacks;    
})(Utilities || (Utilities = {}));

var CommonFood;
(function (CommonFood) {
    CommonFood.days = (function () {
        var DAYS = [
            1, 
            2, 
            3, 
            4, 
            5
        ];
        var def = function () {
            return DAYS;
        };
        def["getName"] = function (day) {
            return moment().day(day).format("dddd");
        };
        def["isValid"] = function (day) {
            return DAYS.indexOf(day) >= 0;
        };
        return def;
    })();
    var EmployeeMenuDefinition = (function (_super) {
        __extends(EmployeeMenuDefinition, _super);
        function EmployeeMenuDefinition(menu, data) {
                _super.call(this);
            this.menu = menu;
            this.reset(data);
        }
        EmployeeMenuDefinition.prototype.reset = function (data) {
        };
        EmployeeMenuDefinition.prototype.exportData = function () {
            return null;
        };
        return EmployeeMenuDefinition;
    })(Utilities.HasCallbacks);
    CommonFood.EmployeeMenuDefinition = EmployeeMenuDefinition;    
    var MenuDefinition = (function (_super) {
        __extends(MenuDefinition, _super);
        function MenuDefinition(data) {
                _super.call(this);
            this.title = ko.observable("");
            this.weeks = ko.observable(0);
            this.options = ko.observableArray();
            this.places = ko.observableArray();
            this.startDate = ko.observable("");
            this.endDate = ko.observable("");
            this.deadlineTime = ko.observable("");
            this.firstWeek = ko.observable(0);
            this.foods = ko.observableArray();
            this.reset(data);
        }
        MenuDefinition.defaultData = {
            title: "Nuevo Menú",
            firstWeek: 0,
            weeks: 0,
            options: [],
            places: [],
            startDate: "2000-01-01",
            endDate: "2100-01-01",
            deadlineTime: "09:30",
            foods: []
        };
        MenuDefinition.idGenerator = new Utilities.IdGenerator();
        MenuDefinition.prototype.createKeyTextObservableArray = function (items) {
            return ko.observableArray(_.map(items, function (item) {
                return {
                    key: item.key,
                    text: ko.observable(item.text)
                };
            }));
        };
        MenuDefinition.prototype.reset = function (data) {
            data = $.extend({
            }, MenuDefinition.defaultData, data);
            var i;
            this.title(data.title);
            this.weeks(0);
            this.startDate(data.startDate);
            this.endDate(data.endDate);
            this.firstWeek(data.firstWeek);
            this.deadlineTime(data.deadlineTime);
            this.places.removeAll();
            for(i in data.places) {
                this.addPlace(data.places[i]);
            }
            this.options.removeAll();
            this.foods.removeAll();
            for(i in data.options) {
                this.addOption(data.options[i]);
            }
            for(i = 0; i < data.weeks; i++) {
                this.addWeek();
            }
            var weeksLength = this.weeks();
            var opts = {
            };
            _.each(this.options(), function (x) {
                opts[x.key] = true;
            });
            var foods = this.foods();
            if(data.foods) {
                _.each(data.foods, function (item) {
                    if(CommonFood.days.isValid(item.day) && item.week < weeksLength && opts[item.option]) {
                        foods[item.week][item.day][item.option](item.food);
                    }
                });
            }
        };
        MenuDefinition.prototype.exportData = function () {
            var data = {
                title: this.title(),
                firstWeek: this.firstWeek(),
                weeks: this.weeks(),
                startDate: this.startDate(),
                endDate: this.endDate(),
                places: ko.toJS(this.places),
                options: ko.toJS(this.options),
                foods: []
            };
            this.eachDay(function (dayFoods, weekIndex, dayIndex) {
                var food;
                for(var opt in dayFoods) {
                    if(food = dayFoods[opt]()) {
                        data.foods.push({
                            week: weekIndex,
                            day: dayIndex,
                            option: opt,
                            food: food
                        });
                    }
                }
            });
            return data;
        };
        MenuDefinition.prototype.getFood = function (weekIndex, dayIndex, opt) {
            return this.foods()[weekIndex][dayIndex][opt];
        };
        MenuDefinition.prototype.eachWeek = function (f) {
            _.each(this.foods(), function (weekFoods, weekIndex) {
                return f(weekFoods, weekIndex);
            });
        };
        MenuDefinition.prototype.eachDay = function (f) {
            this.eachWeek(function (weekFoods, weekIndex) {
                return _.each(weekFoods, function (dayFoods, dayIndex) {
                    return f(dayFoods, weekIndex, dayIndex);
                });
            });
        };
        MenuDefinition.prototype.generateText = function (baseName, collection, name) {
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
        MenuDefinition.prototype.addKeyObservableText = function (collection, baseName, idPrefix, value) {
            var item;
            if(!value || !value.key) {
                var text = this.generateText(baseName, collection, value);
                item = {
                    key: MenuDefinition.idGenerator.generate(idPrefix),
                    text: ko.observable(text)
                };
            } else {
                item = {
                    key: value.key,
                    text: ko.observable(value.text)
                };
            }
            collection.push(item);
            return item;
        };
        MenuDefinition.prototype.removeItem = function (collection, item) {
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
        MenuDefinition.prototype.addWeek = function () {
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
        MenuDefinition.prototype.removeWeek = function () {
            var actual = this.weeks();
            if(actual > 0) {
                this.weeks(actual - 1);
                this.foods.pop();
            }
        };
        MenuDefinition.prototype.addOption = function (option) {
            var op = this.addKeyObservableText(this.options, "Menú ", "menu_", option);
            this.eachDay(function (dayFoods) {
                dayFoods[op.key] = ko.observable("");
            });
        };
        MenuDefinition.prototype.removeOption = function (option) {
            var removed = this.removeItem(this.options, option);
            if(removed) {
                this.eachDay(function (dayFoods) {
                    return delete dayFoods[removed.key];
                });
            }
        };
        MenuDefinition.prototype.addPlace = function (place) {
            this.addKeyObservableText(this.places, "Lugar ", "place_", place);
        };
        MenuDefinition.prototype.removePlace = function (place) {
            this.removeItem(this.places, place);
        };
        return MenuDefinition;
    })(Utilities.HasCallbacks);
    CommonFood.MenuDefinition = MenuDefinition;    
})(CommonFood || (CommonFood = {}));

