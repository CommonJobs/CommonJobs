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
    Utilities.idGenerator = new IdGenerator();
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
    (function (ObservableArrays) {
        function generateText(baseName, collection, name) {
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
        }
        function addKeyObservableText(collection, baseName, idPrefix, value) {
            var item;
            if(!value || !value.key) {
                var text = generateText(baseName, collection, value);
                item = {
                    key: Utilities.idGenerator.generate(idPrefix),
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
        }
        ObservableArrays.addKeyObservableText = addKeyObservableText;
        function removeItem(collection, item, keyField) {
            if (typeof keyField === "undefined") { keyField = "key"; }
            if(!collection().length) {
                return null;
            }
            if(_.isString(item)) {
                return collection.remove(function (x) {
                    return x[keyField] == item;
                });
            }
            var index = _.isNumber(item) ? item : collection.indexOf(item);
            return collection.splice(index, 1)[0];
        }
        ObservableArrays.removeItem = removeItem;
    })(Utilities.ObservableArrays || (Utilities.ObservableArrays = {}));
    var ObservableArrays = Utilities.ObservableArrays;

})(Utilities || (Utilities = {}));

var MyMenu;
(function (MyMenu) {
    MyMenu.days = (function () {
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
            return MyMenu.days.isValid(day) && week < this.weeksQuantity() ? this.items[week][day] : null;
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
        function DayChoice() {
            this.option = ko.observable("");
            this.place = ko.observable("");
        }
        return DayChoice;
    })();    
    var Override = (function (_super) {
        __extends(Override, _super);
        function Override(data) {
                _super.call(this);
            this.date = ko.observable("");
            this.cancel = ko.observable(false);
            this.comment = ko.observable("");
            if(data) {
                this.option(data.option);
                this.place(data.place);
                this.date(data.date);
                this.cancel(data.cancel);
                this.comment(data.comment);
            }
        }
        return Override;
    })(DayChoice);    
    var EmployeeMenuDefinition = (function (_super) {
        __extends(EmployeeMenuDefinition, _super);
        function EmployeeMenuDefinition(menu, data) {
                _super.call(this, menu.weeksQuantity());
            this.menu = menu;
            this.menuId = ko.observable("");
            this.defaultPlace = ko.observable("");
            this.overrides = ko.observableArray([]);
            this.EmployeeMenuDefinitionReset(data);
        }
        EmployeeMenuDefinition.prototype.reset = function (data) {
            _super.prototype.reset.call(this, this.menu.weeksQuantity());
            this.EmployeeMenuDefinitionReset(data);
        };
        EmployeeMenuDefinition.prototype.createNewItem = function () {
            return new DayChoice();
        };
        EmployeeMenuDefinition.prototype.eachWeek = function (f) {
            _super.prototype.eachWeek.call(this, f);
        };
        EmployeeMenuDefinition.prototype.eachDay = function (f) {
            _super.prototype.eachDay.call(this, f);
        };
        EmployeeMenuDefinition.prototype.getItem = function (week, day) {
            return _super.prototype.getItem.call(this, week, day);
        };
        EmployeeMenuDefinition.prototype.getMenuChoice = function (week, day) {
            return this.getItem(week, day).option;
        };
        EmployeeMenuDefinition.prototype.getPlaceChoice = function (week, day) {
            return this.getItem(week, day).place;
        };
        EmployeeMenuDefinition.prototype.getDefaultPlaceLabel = function () {
            var key = this.defaultPlace();
            var places = this.menu.places();
            var option = _.find(places, function (x) {
                return x.key == key;
            });
            if(option) {
                return "Lugar por defecto (" + option.text() + ")";
            } else {
                return "Seleccione un lugar (no se definió el lugar por defecto)";
            }
        };
        EmployeeMenuDefinition.prototype.EmployeeMenuDefinitionReset = function (data) {
            var _this = this;
            data = _.extend({
                userName: "",
                Id: "",
                name: "",
                defaultPlace: ""
            }, data);
            this.menuId(data.menuId);
            this.Id = data.Id;
            this.userName = data.userName;
            this.name = data.name;
            this.defaultPlace(data.defaultPlace);
            if(data.choices) {
                _.each(data.choices, function (x) {
                    var choice = _this.getItem(x.week, x.day);
                    if(choice) {
                        choice.option(x.option);
                        choice.place(x.place);
                    }
                });
            }
            this.overrides.removeAll();
            if(data.overrides) {
                _.each(data.overrides, function (x) {
                    return _this.overrides.push(new Override(x));
                });
            }
        };
        EmployeeMenuDefinition.prototype.exportData = function () {
            var data = {
                menuId: this.menuId(),
                Id: this.Id,
                userName: this.userName,
                name: this.name,
                defaultPlace: this.defaultPlace(),
                choices: [],
                overrides: []
            };
            var choices = data.choices;
            this.eachDay(function (dayChoices, weekIndex, dayIndex) {
                var option = dayChoices.option();
                if(option) {
                    var item = {
                        week: weekIndex,
                        day: dayIndex,
                        option: option
                    };
                    var place = dayChoices.place();
                    if(place) {
                        item.place = place;
                    }
                    choices.push(item);
                }
            });
            data.overrides = ko.toJS(this.overrides);
            return data;
        };
        EmployeeMenuDefinition.prototype.addOverride = function () {
            this.overrides.push(new Override());
        };
        EmployeeMenuDefinition.prototype.removeOverride = function (override) {
            Utilities.ObservableArrays.removeItem(this.overrides, override, 'date');
        };
        return EmployeeMenuDefinition;
    })(WeekStorage);
    MyMenu.EmployeeMenuDefinition = EmployeeMenuDefinition;    
    var MenuDefinition = (function (_super) {
        __extends(MenuDefinition, _super);
        function MenuDefinition(data) {
                _super.call(this, data && data.weeksQuantity);
            this.Id = ko.observable("");
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
            Id: "Menu/DefaultMenu",
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
            this.Id(data.Id);
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
                Id: this.Id(),
                deadlineTime: this.deadlineTime(),
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
        MenuDefinition.prototype.addOption = function (option) {
            var op = Utilities.ObservableArrays.addKeyObservableText(this.options, "Menú ", "menu_", option);
            this.eachDay(function (dayFoods) {
                dayFoods[op.key] = ko.observable("");
            });
            this.options.valueHasMutated();
        };
        MenuDefinition.prototype.removeOption = function (option) {
            var removed = Utilities.ObservableArrays.removeItem(this.options, option);
            if(removed) {
                this.eachDay(function (dayFoods) {
                    return delete dayFoods[removed.key];
                });
            }
        };
        MenuDefinition.prototype.addPlace = function (place) {
            Utilities.ObservableArrays.addKeyObservableText(this.places, "Lugar ", "place_", place);
        };
        MenuDefinition.prototype.removePlace = function (place) {
            Utilities.ObservableArrays.removeItem(this.places, place);
        };
        return MenuDefinition;
    })(WeekStorage);
    MyMenu.MenuDefinition = MenuDefinition;    
})(MyMenu || (MyMenu = {}));
