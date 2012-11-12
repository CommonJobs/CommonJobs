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
            this.prepareMenu(menu);
            this.reset(data);
        }
        EmployeeMenuDefinition.prototype.prepareMenu = function (menu) {
            this.menu = menu;
        };
        EmployeeMenuDefinition.prototype.reset = function (data) {
            this.employeeId = data.employeeId;
            this.name = data.name;
            this.defaultPlace = ko.observable(data.defaultPlace);
            _.each(data.choices, function (choice) {
            });
            _.each(data.overrides, function (override) {
            });
        };
        EmployeeMenuDefinition.prototype.exportData = function () {
            var data = {
                employeeId: this.employeeId,
                name: this.name,
                defaultPlace: this.defaultPlace(),
                choices: [],
                overrides: []
            };
            return data;
        };
        return EmployeeMenuDefinition;
    })(Utilities.HasCallbacks);
    CommonFood.EmployeeMenuDefinition = EmployeeMenuDefinition;    
    var ByDayCollection = (function (_super) {
        __extends(ByDayCollection, _super);
        function ByDayCollection(data) {
                _super.call(this);
            this.weeks = ko.observable(0);
            this.items = ko.observableArray();
            this.reset(data);
        }
        ByDayCollection.prototype.reset = function (data) {
            var i;
            data = _.extend({
                weeks: 0,
                items: []
            }, data);
            this.weeks(0);
            this.items.removeAll();
            for(i = 0; i < data.weeks; i++) {
                this.addWeek();
            }
        };
        ByDayCollection.prototype.addWeek = function () {
            var weekItems = [];
            for(var i = 0; i < 7; i++) {
                var dayItem = this.createNewItem();
                weekItems.push(dayItem);
            }
            this.items.push(weekItems);
            this.weeks(this.weeks() + 1);
        };
        ByDayCollection.prototype.removeWeek = function () {
            var actual = this.weeks();
            if(actual > 0) {
                this.weeks(actual - 1);
                this.items.pop();
            }
        };
        ByDayCollection.prototype.createNewItem = function () {
            return null;
        };
        ByDayCollection.prototype.getItem = function (week, day) {
            return CommonFood.days.isValid(day) && week < this.weeks() ? this.items()[week][day] : null;
        };
        ByDayCollection.prototype.eachWeek = function (f) {
            _.each(this.items(), function (weekFoods, weekIndex) {
                return f(weekFoods, weekIndex);
            });
        };
        ByDayCollection.prototype.eachDay = function (f) {
            this.eachWeek(function (weekFoods, weekIndex) {
                return _.each(weekFoods, function (dayFoods, dayIndex) {
                    return f(dayFoods, weekIndex, dayIndex);
                });
            });
        };
        return ByDayCollection;
    })(Utilities.HasCallbacks);    
    var MenuDefinitionByDayCollection = (function (_super) {
        __extends(MenuDefinitionByDayCollection, _super);
        function MenuDefinitionByDayCollection(menu, data) {
                _super.call(this, data);
            this.menu = menu;
        }
        MenuDefinitionByDayCollection.prototype.reset = function (data) {
            var _this = this;
            _super.prototype.reset.call(this, data);
            if(data && data.foods) {
                _.each(data.foods, function (x) {
                    var option = _this.getFood(x.week, x.day, x.option);
                    if(option) {
                        option(x.food);
                    }
                });
            }
        };
        MenuDefinitionByDayCollection.prototype.createNewItem = function () {
            var item = {
            };
            _.each(this.menu.options(), function (option) {
                return item[option.key] = ko.observable("");
            });
            return item;
        };
        MenuDefinitionByDayCollection.prototype.getItem = function (week, day) {
            return _super.prototype.getItem.call(this, week, day);
        };
        MenuDefinitionByDayCollection.prototype.getFood = function (week, day, option) {
            var dayFoods = this.getItem(week, day);
            return dayFoods && dayFoods[option];
        };
        return MenuDefinitionByDayCollection;
    })(ByDayCollection);    
    var MenuDefinition = (function (_super) {
        __extends(MenuDefinition, _super);
        function MenuDefinition(data) {
                _super.call(this);
            this.title = ko.observable("");
            this.options = ko.observableArray();
            this.places = ko.observableArray();
            this.startDate = ko.observable("");
            this.endDate = ko.observable("");
            this.deadlineTime = ko.observable("");
            this.firstWeek = ko.observable(0);
            this.items = new MenuDefinitionByDayCollection(this);
            this.weeks = this.items.weeks;
            this.foods = this.items.items;
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
            this.startDate(data.startDate);
            this.endDate(data.endDate);
            this.firstWeek(data.firstWeek);
            this.deadlineTime(data.deadlineTime);
            this.places.removeAll();
            for(i in data.places) {
                this.addPlace(data.places[i]);
            }
            this.options.removeAll();
            for(i in data.options) {
                this.addOption(data.options[i]);
            }
            this.items.reset(data);
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
            this.items.eachDay(function (dayFoods, weekIndex, dayIndex) {
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
            return this.items.getFood(weekIndex, dayIndex, opt);
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
            this.items.addWeek();
        };
        MenuDefinition.prototype.removeWeek = function () {
            this.items.removeWeek();
        };
        MenuDefinition.prototype.addOption = function (option) {
            var op = this.addKeyObservableText(this.options, "Menú ", "menu_", option);
            this.items.eachDay(function (dayFoods) {
                dayFoods[op.key] = ko.observable("");
            });
        };
        MenuDefinition.prototype.removeOption = function (option) {
            var removed = this.removeItem(this.options, option);
            if(removed) {
                this.items.eachDay(function (dayFoods) {
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

