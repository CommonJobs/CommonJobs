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
    var WeekStorage = (function (_super) {
        __extends(WeekStorage, _super);
        function WeekStorage(weeksQuantity) {
            var _this = this;
                _super.call(this);
            this.weeksQuantity = ko.observable(0);
            this.weeks = ko.computed(function () {
                return _.map(_.range(_this.weeksQuantity()), function (x) {
                    return {
                        key: x.toString(),
                        text: "Semana " + (x + 1)
                    };
                });
            });
            this.WeekStorageReset(weeksQuantity);
        }
        WeekStorage.prototype.setWeeksQuantity = function (n) {
            (this.weeksQuantity)(n);
        };
        WeekStorage.prototype.reset = function (weeksQuantity) {
            this.WeekStorageReset(weeksQuantity);
        };
        WeekStorage.prototype.WeekStorageReset = function (weeksQuantity) {
            var i;
            this.setWeeksQuantity(0);
            this.items = [];
            if(weeksQuantity) {
                for(i = 0; i < weeksQuantity; i++) {
                    this.addWeek();
                }
            }
        };
        WeekStorage.prototype.addWeek = function () {
            var weekItems = [];
            for(var i = 0; i < 7; i++) {
                var dayItem = this.createNewItem();
                weekItems.push(dayItem);
            }
            this.items.push(weekItems);
            this.setWeeksQuantity(this.weeksQuantity() + 1);
        };
        WeekStorage.prototype.removeWeek = function () {
            var actual = this.weeksQuantity();
            if(actual > 0) {
                this.setWeeksQuantity(actual - 1);
                this.items.pop();
            }
        };
        WeekStorage.prototype.createNewItem = function () {
            return null;
        };
        WeekStorage.prototype.getItem = function (week, day) {
            return CommonFood.days.isValid(day) && week < this.weeksQuantity() ? this.items[week][day] : null;
        };
        WeekStorage.prototype.eachWeek = function (f) {
            _.each(this.items, function (weekItems, weekIndex) {
                return f(weekItems, weekIndex);
            });
        };
        WeekStorage.prototype.eachDay = function (f) {
            this.eachWeek(function (weekItems, weekIndex) {
                return _.each(weekItems, function (dayItem, dayIndex) {
                    return f(dayItem, weekIndex, dayIndex);
                });
            });
        };
        return WeekStorage;
    })(Utilities.HasCallbacks);    
    var DayChoice = (function () {
        function DayChoice(option, place) {
            if (typeof option === "undefined") { option = null; }
            if (typeof place === "undefined") { place = null; }
            this.option = ko.observable(option);
            this.place = ko.observable(place);
        }
        return DayChoice;
    })();    
    var EmployeeMenuDefinition = (function (_super) {
        __extends(EmployeeMenuDefinition, _super);
        function EmployeeMenuDefinition(menu, data) {
                _super.call(this, menu.weeksQuantity());
            this.menu = menu;
            this.EmployeeMenuDefinitionReset(data);
        }
        EmployeeMenuDefinition.prototype.reset = function (data) {
            _super.prototype.reset.call(this, this.menu.weeksQuantity());
            this.EmployeeMenuDefinitionReset(data);
        };
        EmployeeMenuDefinition.prototype.createNewItem = function () {
            return new DayChoice();
        };
        EmployeeMenuDefinition.prototype.getItem = function (week, day) {
            return _super.prototype.getItem.call(this, week, day);
        };
        EmployeeMenuDefinition.prototype.EmployeeMenuDefinitionReset = function (data) {
            var _this = this;
            this.employeeId = data.employeeId;
            this.name = data.name;
            this.defaultPlace = ko.observable(data.defaultPlace);
            _.each(data.choices, function (x) {
                var choice = _this.getItem(x.week, x.day);
                if(choice) {
                    choice.option(x.option);
                    choice.place(x.place);
                }
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
    })(WeekStorage);
    CommonFood.EmployeeMenuDefinition = EmployeeMenuDefinition;    
    var MenuDefinition = (function (_super) {
        __extends(MenuDefinition, _super);
        function MenuDefinition(data) {
                _super.call(this, data && data.weeksQuantity);
            this.title = ko.observable("");
            this.options = ko.observableArray();
            this.places = ko.observableArray();
            this.startDate = ko.observable("");
            this.endDate = ko.observable("");
            this.deadlineTime = ko.observable("");
            this.firstWeek = ko.observable(0);
            this.MenuDefinitionReset(data);
        }
        MenuDefinition.defaultData = {
            title: "Nuevo Menú",
            firstWeek: 0,
            weeksQuantity: 0,
            options: [],
            places: [],
            startDate: "2000-01-01",
            endDate: "2100-01-01",
            deadlineTime: "09:30",
            foods: []
        };
        MenuDefinition.idGenerator = new Utilities.IdGenerator();
        MenuDefinition.prototype.createNewItem = function () {
            var item = {
            };
            if(this.options) {
                _.each(this.options(), function (option) {
                    return item[option.key] = ko.observable("");
                });
            }
            return item;
        };
        MenuDefinition.prototype.getItem = function (week, day) {
            return _super.prototype.getItem.call(this, week, day);
        };
        MenuDefinition.prototype.getFood = function (week, day, option) {
            this.weeksQuantity();
            this.options();
            var dayFoods = this.getItem(week, day);
            return dayFoods && dayFoods[option];
        };
        MenuDefinition.prototype.eachWeek = function (f) {
            _super.prototype.eachWeek.call(this, f);
        };
        MenuDefinition.prototype.eachDay = function (f) {
            _super.prototype.eachDay.call(this, f);
        };
        MenuDefinition.prototype.createKeyTextObservableArray = function (items) {
            return ko.observableArray(_.map(items, function (item) {
                return {
                    key: item.key,
                    text: ko.observable(item.text)
                };
            }));
        };
        MenuDefinition.prototype.MenuDefinitionReset = function (data) {
            var _this = this;
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
            if(data && data.foods) {
                _.each(data.foods, function (x) {
                    var option = _this.getFood(x.week, x.day, x.option);
                    if(option) {
                        option(x.food);
                    }
                });
            }
        };
        MenuDefinition.prototype.reset = function (data) {
            _super.prototype.reset.call(this, data && data.weeksQuantity);
            this.MenuDefinitionReset(data);
        };
        MenuDefinition.prototype.exportData = function () {
            var data = {
                title: this.title(),
                firstWeek: this.firstWeek(),
                weeksQuantity: this.weeksQuantity(),
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
        MenuDefinition.prototype.addOption = function (option) {
            var op = this.addKeyObservableText(this.options, "Menú ", "menu_", option);
            this.eachDay(function (dayFoods) {
                dayFoods[op.key] = ko.observable("");
            });
            this.options.valueHasMutated();
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
    })(WeekStorage);
    CommonFood.MenuDefinition = MenuDefinition;    
})(CommonFood || (CommonFood = {}));

