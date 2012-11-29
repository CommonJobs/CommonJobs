var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
var Utilities;
(function (Utilities) {
    function daysDiff(date1, date2) {
        var mmnt1 = moment(date1);
        var mmnt2 = moment(date2);
        if(!date1 || !date2 || !mmnt1 || !mmnt2) {
            return NaN;
        }
        mmnt1.hours(0).minutes(0).seconds(0).milliseconds(0);
        mmnt2.hours(0).minutes(0).seconds(0).milliseconds(0);
        return mmnt1.diff(mmnt2, 'days');
    }
    Utilities.daysDiff = daysDiff;
    function dirtyFlag() {
        var observable = ko.observable(false);
        observable.register = function (anotherObservable) {
            anotherObservable.subscribe(function () {
                observable(true);
            });
        };
        observable.reset = function () {
            return observable(false);
        };
        return observable;
    }
    Utilities.dirtyFlag = dirtyFlag;
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
                return item.Text;
            });
            var n = texts.length + 1;
            if(name) {
                if((name).Text) {
                    name = (name).Text;
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
            if(!value || !value.Key) {
                var text = generateText(baseName, collection, value);
                item = {
                    Key: Utilities.idGenerator.generate(idPrefix),
                    Text: ko.observable(text)
                };
            } else {
                item = {
                    Key: value.Key,
                    Text: ko.observable(value.Text)
                };
            }
            collection.push(item);
            return item;
        }
        ObservableArrays.addKeyObservableText = addKeyObservableText;
        function removeItem(collection, item, keyField) {
            if (typeof keyField === "undefined") { keyField = "Key"; }
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
    var CalendarHelper = (function () {
        function CalendarHelper(startDate, weeksQuantity, firstWeek) {
            var _this = this;
            var startDateIsFunc = _.isFunction(startDate);
            var weeksQuantityIsFunc = _.isFunction(weeksQuantity);
            var firstWeekIsFunc = _.isFunction(firstWeek);
            if(startDateIsFunc) {
                this.StartDate = function () {
                    return moment(startDate());
                };
            } else {
                this.StartDate = function () {
                    return moment(startDate);
                };
            }
            this.WeeksQuantity = weeksQuantityIsFunc ? weeksQuantity : function () {
                return weeksQuantity;
            };
            if(firstWeekIsFunc) {
                this.FirstWeekIdx = function () {
                    return firstWeek() || 0;
                };
            } else {
                firstWeek = firstWeek || 0;
                this.FirstWeekIdx = function () {
                    return firstWeek;
                };
            }
            if(startDateIsFunc || weeksQuantityIsFunc || firstWeekIsFunc) {
                this.ZeroWeekZeroDay = function () {
                    return _this.calculateZeroWeekZeroDay();
                };
            } else {
                var zeroWeekZeroDay = this.calculateZeroWeekZeroDay();
                this.ZeroWeekZeroDay = function () {
                    return zeroWeekZeroDay;
                };
            }
        }
        CalendarHelper.prototype.calculateZeroWeekZeroDay = function () {
            return this.StartDate().day(0).add('weeks', -1 * this.FirstWeekIdx());
        };
        CalendarHelper.prototype.week = function (date) {
            var weeksQuantity = this.WeeksQuantity();
            var sundayDate = moment(date).day(0);
            var diff = sundayDate.diff(this.ZeroWeekZeroDay(), 'weeks');
            var result = diff < 0 ? weeksQuantity + diff % weeksQuantity : diff % weeksQuantity;
            return result;
        };
        CalendarHelper.prototype.day = function (date) {
            var result = moment(date).day();
            return result;
        };
        CalendarHelper.prototype.match = function (weekIdx, dayIdx, date) {
            return weekIdx == this.week(date) && dayIdx == this.day(date);
        };
        CalendarHelper.prototype.near = function (weekIdx, dayIdx, date) {
            var weeksQuantity = this.WeeksQuantity();
            if(weeksQuantity <= weekIdx) {
                return null;
            }
            var mmnt = moment(date);
            if(!mmnt.isValid) {
                return null;
            }
            while(!this.match(weekIdx, dayIdx, mmnt)) {
                mmnt.add('days', 1);
            }
            return mmnt;
        };
        return CalendarHelper;
    })();    
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
            this.isDirty = Utilities.dirtyFlag();
            this.WeeksQuantity = ko.observable(0);
            this.isDirty.register(this.WeeksQuantity);
            this.Weeks = ko.computed(function () {
                return _.map(_.range(_this.WeeksQuantity()), function (x) {
                    return {
                        Key: x.toString(),
                        Text: "Semana " + (x + 1)
                    };
                });
            });
            this.WeekStorageReset(weeksQuantity);
        }
        WeekStorage.prototype.setWeeksQuantity = function (n) {
            (this.WeeksQuantity)(n);
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
            this.setWeeksQuantity(this.WeeksQuantity() + 1);
        };
        WeekStorage.prototype.removeWeek = function () {
            var actual = this.WeeksQuantity();
            if(actual > 0) {
                this.setWeeksQuantity(actual - 1);
                this.items.pop();
            }
        };
        WeekStorage.prototype.createNewItem = function () {
            return null;
        };
        WeekStorage.prototype.getItem = function (week, day) {
            return MyMenu.days.isValid(day) && week < this.WeeksQuantity() ? this.items[week][day] : null;
        };
        WeekStorage.prototype.eachWeek = function (f) {
            _.each(this.items, function (weekItems, weekIdx) {
                return f(weekItems, weekIdx);
            });
        };
        WeekStorage.prototype.eachDay = function (f) {
            this.eachWeek(function (weekItems, weekIdx) {
                return _.each(weekItems, function (dayItem, dayIdx) {
                    return f(dayItem, weekIdx, dayIdx);
                });
            });
        };
        return WeekStorage;
    })(Utilities.HasCallbacks);    
    var DayChoice = (function () {
        function DayChoice() {
            this.OptionKey = ko.observable("");
            this.PlaceKey = ko.observable("");
        }
        return DayChoice;
    })();    
    var Override = (function (_super) {
        __extends(Override, _super);
        function Override(data) {
                _super.call(this);
            this.Date = ko.observable("");
            this.Cancel = ko.observable(false);
            this.Comment = ko.observable("");
            if(data) {
                this.OptionKey(data.OptionKey);
                this.PlaceKey(data.PlaceKey);
                this.Date(data.Date);
                this.Cancel(data.Cancel);
                this.Comment(data.Comment);
            }
        }
        return Override;
    })(DayChoice);    
    var EmployeeMenuDefinition = (function (_super) {
        __extends(EmployeeMenuDefinition, _super);
        function EmployeeMenuDefinition(menu, data, now) {
                _super.call(this, menu.WeeksQuantity());
            this.menu = menu;
            this.MenuId = ko.observable("");
            this.DefaultPlaceKey = ko.observable("");
            this.Overrides = ko.observableArray([]);
            this.now = ko.observable();
            this.isDirty.register(this.MenuId);
            this.isDirty.register(this.DefaultPlaceKey);
            this.isDirty.register(this.Overrides);
            this.EmployeeMenuDefinitionReset(data);
            this.calendarHelper = new CalendarHelper(menu.StartDate, menu.WeeksQuantity, menu.FirstWeekIdx);
            this.now(now);
        }
        EmployeeMenuDefinition.prototype.reset = function (data) {
            _super.prototype.reset.call(this, this.menu.WeeksQuantity());
            this.EmployeeMenuDefinitionReset(data);
            this.isDirty(false);
        };
        EmployeeMenuDefinition.prototype.nearFormated = function (weekIdx, dayIdx) {
            var now = moment(this.now());
            var date = this.calendarHelper.near(+weekIdx, dayIdx, now);
            if(date == null) {
                return null;
            }
            var days = date.diff(now, 'days', true);
            var str = date.format("D [de] MMMM [de] YYYY");
            return days == 0 ? "Hoy (" + str + ")" : days == 1 ? "Mañana (" + str + ")" : days == 2 ? "Pasado mañana (" + str + ")" : days < 7 ? "En " + days + " días (" + str + ")" : str;
        };
        EmployeeMenuDefinition.prototype.createNewItem = function () {
            var dayChoice = new DayChoice();
            this.isDirty.register(dayChoice.OptionKey);
            this.isDirty.register(dayChoice.PlaceKey);
            return dayChoice;
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
            var item = this.getItem(week, day);
            return item && item.OptionKey;
        };
        EmployeeMenuDefinition.prototype.getPlaceChoice = function (week, day) {
            var item = this.getItem(week, day);
            return item && item.PlaceKey;
        };
        EmployeeMenuDefinition.prototype.getChoicesByDate = function (date) {
            var weekIdx = this.calendarHelper.week(date);
            var dayIdx = this.calendarHelper.day(date);
            var placeKey = this.DefaultPlaceKey();
            var optionKey = null;
            var comment = null;
            var food = null;
            var option = null;
            var place = null;
            var item = this.getItem(weekIdx, dayIdx);
            if(item) {
                placeKey = item.PlaceKey() || placeKey;
                optionKey = item.OptionKey() || optionKey;
            }
            var override = _.last(_.filter(this.Overrides(), function (x) {
                return Utilities.daysDiff(x.Date(), date) === 0;
            }));
            if(override) {
                comment = override.Comment();
                if(override.Cancel()) {
                    placeKey = null;
                    optionKey = null;
                } else {
                    placeKey = override.PlaceKey() || placeKey;
                    optionKey = override.OptionKey() || optionKey;
                }
            }
            food = optionKey && this.menu.getFood(weekIdx, dayIdx, optionKey)();
            var auxPlace = placeKey && _.find(this.menu.Places(), function (x) {
                return x.Key == placeKey;
            });
            place = auxPlace ? auxPlace.Text() : null;
            var auxOption = optionKey && _.find(this.menu.Options(), function (x) {
                return x.Key == optionKey;
            });
            option = auxOption ? auxOption.Text() : null;
            if(!food || !placeKey) {
                place = null;
                option = null;
                food = null;
            }
            return {
                Date: date,
                Place: place,
                Option: option,
                Food: food,
                Comment: comment,
                WeekIdx: weekIdx,
                DayIdx: dayIdx
            };
        };
        EmployeeMenuDefinition.prototype.getDefaultPlaceLabel = function () {
            var key = this.DefaultPlaceKey();
            var places = this.menu.Places();
            var option = _.find(places, function (x) {
                return x.Key == key;
            });
            if(option) {
                return "Lugar por defecto (" + option.Text() + ")";
            } else {
                return "Seleccione un lugar (no se definió el lugar por defecto)";
            }
        };
        EmployeeMenuDefinition.prototype.getDayWeekLabel = function (date) {
            var mmnt = moment(ko.utils.unwrapObservable(date));
            if(!mmnt || !mmnt.isValid) {
                return "Seleccione un día válido";
            }
            return mmnt.format("dddd") + ' de ' + 'Semana ' + (this.calendarHelper.week(mmnt) + 1);
        };
        EmployeeMenuDefinition.prototype.getDayWeekOptionOverrideInfo = function (date, option) {
            var mmnt = moment(ko.utils.unwrapObservable(date));
            if(!mmnt || !mmnt.isValid) {
                return "Atención! Seleccione un día válido";
            }
            var week = this.calendarHelper.week(mmnt);
            var day = this.calendarHelper.day(mmnt);
            var optionKey = ko.utils.unwrapObservable(option) || ko.utils.unwrapObservable(this.getMenuChoice(week, day));
            var food = ko.utils.unwrapObservable(this.menu.getFood(week, day, optionKey));
            return food || "Atención! no hay comida seleccionada para este día";
        };
        EmployeeMenuDefinition.prototype.getDayWeekPlaceOverrideInfo = function (date, place) {
            var mmnt = moment(ko.utils.unwrapObservable(date));
            if(!mmnt || !mmnt.isValid) {
                return "Atención! Seleccione un día válido";
            }
            var week = this.calendarHelper.week(mmnt);
            var day = this.calendarHelper.day(mmnt);
            var placeKey = ko.utils.unwrapObservable(place) || ko.utils.unwrapObservable(this.getPlaceChoice(week, day)) || ko.utils.unwrapObservable(this.DefaultPlaceKey);
            var places = this.menu.Places();
            var finded = _.find(places, function (x) {
                return x.Key == placeKey;
            });
            var text = finded && finded.Text;
            return text || "Atención! no hay lugar seleccionado para este día";
        };
        EmployeeMenuDefinition.prototype.EmployeeMenuDefinitionReset = function (data) {
            var _this = this;
            data = _.extend({
                UserName: "",
                Id: "",
                EmployeeName: "",
                DefaultPlaceKey: ""
            }, data);
            this.MenuId(data.MenuId);
            this.Id = data.Id;
            this.UserName = data.UserName;
            this.EmployeeName = data.EmployeeName;
            this.DefaultPlaceKey(data.DefaultPlaceKey);
            if(data.WeeklyChoices) {
                _.each(data.WeeklyChoices, function (x) {
                    var choice = _this.getItem(x.WeekIdx, x.DayIdx);
                    if(choice) {
                        choice.OptionKey(x.OptionKey);
                        choice.PlaceKey(x.PlaceKey);
                    }
                });
            }
            this.Overrides.removeAll();
            if(data.Overrides) {
                _.each(data.Overrides, function (x) {
                    var ov = new Override(x);
                    _this.Overrides.push(ov);
                    _this.isDirty.register(ov.OptionKey);
                    _this.isDirty.register(ov.PlaceKey);
                    _this.isDirty.register(ov.Date);
                    _this.isDirty.register(ov.Cancel);
                    _this.isDirty.register(ov.Comment);
                });
            }
        };
        EmployeeMenuDefinition.prototype.exportData = function () {
            var data = {
                MenuId: this.MenuId(),
                Id: this.Id,
                UserName: this.UserName,
                EmployeeName: this.EmployeeName,
                DefaultPlaceKey: this.DefaultPlaceKey(),
                WeeklyChoices: [],
                Overrides: []
            };
            var choices = data.WeeklyChoices;
            this.eachDay(function (dayChoices, weekIdx, dayIdx) {
                var optionKey = dayChoices.OptionKey();
                if(optionKey) {
                    var item = {
                        WeekIdx: weekIdx,
                        DayIdx: dayIdx,
                        OptionKey: optionKey
                    };
                    var place = dayChoices.PlaceKey();
                    if(place) {
                        item.PlaceKey = place;
                    }
                    choices.push(item);
                }
            });
            data.Overrides = ko.toJS(this.Overrides);
            return data;
        };
        EmployeeMenuDefinition.prototype.addOverride = function () {
            var override = new Override();
            this.Overrides.push(override);
            this.isDirty.register(override.OptionKey);
            this.isDirty.register(override.PlaceKey);
            this.isDirty.register(override.Date);
            this.isDirty.register(override.Cancel);
            this.isDirty.register(override.Comment);
        };
        EmployeeMenuDefinition.prototype.removeOverride = function (override) {
            Utilities.ObservableArrays.removeItem(this.Overrides, override, 'date');
        };
        return EmployeeMenuDefinition;
    })(WeekStorage);
    MyMenu.EmployeeMenuDefinition = EmployeeMenuDefinition;    
    var MenuDefinition = (function (_super) {
        __extends(MenuDefinition, _super);
        function MenuDefinition(data) {
                _super.call(this, data && data.WeeksQuantity);
            this.Id = ko.observable("");
            this.Title = ko.observable("");
            this.Options = ko.observableArray();
            this.Places = ko.observableArray();
            this.StartDate = ko.observable("");
            this.EndDate = ko.observable("");
            this.DeadlineTime = ko.observable("");
            this.LastSentDate = ko.observable("");
            this.FirstWeekIdx = ko.observable(0);
            this.isDirty.register(this.Id);
            this.isDirty.register(this.Title);
            this.isDirty.register(this.Options);
            this.isDirty.register(this.Places);
            this.isDirty.register(this.StartDate);
            this.isDirty.register(this.EndDate);
            this.isDirty.register(this.DeadlineTime);
            this.isDirty.register(this.FirstWeekIdx);
            this.isDirty.register(this.LastSentDate);
            this.MenuDefinitionReset(data);
        }
        MenuDefinition.defaultData = {
            Id: "Menu/DefaultMenu",
            Title: "Nuevo Menú",
            FirstWeekIdx: 0,
            WeeksQuantity: 0,
            Options: [],
            Places: [],
            StartDate: "2000-01-01",
            EndDate: "2100-01-01",
            LastSentDate: "2000-01-01",
            DeadlineTime: "09:30",
            Foods: []
        };
        MenuDefinition.idGenerator = new Utilities.IdGenerator();
        MenuDefinition.prototype.createNewItem = function () {
            var _this = this;
            var item = {
            };
            if(this.Options) {
                _.each(this.Options(), function (option) {
                    var obs = ko.observable("");
                    item[option.Key] = obs;
                    _this.isDirty.register(obs);
                });
            }
            return item;
        };
        MenuDefinition.prototype.getItem = function (week, day) {
            return _super.prototype.getItem.call(this, week, day);
        };
        MenuDefinition.prototype.getFood = function (week, day, option) {
            this.WeeksQuantity();
            this.Options();
            var dayFoods = this.getItem(week, day);
            return dayFoods && dayFoods[option];
        };
        MenuDefinition.prototype.eachWeek = function (f) {
            _super.prototype.eachWeek.call(this, f);
        };
        MenuDefinition.prototype.eachDay = function (f) {
            _super.prototype.eachDay.call(this, f);
        };
        MenuDefinition.prototype.MenuDefinitionReset = function (data) {
            var _this = this;
            data = $.extend({
            }, MenuDefinition.defaultData, data);
            var i;
            this.Id(data.Id);
            this.Title(data.Title);
            this.StartDate(data.StartDate);
            this.EndDate(data.EndDate);
            this.FirstWeekIdx(data.FirstWeekIdx);
            this.DeadlineTime(data.DeadlineTime);
            this.LastSentDate(data.LastSentDate);
            this.Places.removeAll();
            for(i in data.Places) {
                this.addPlace(data.Places[i]);
            }
            this.Options.removeAll();
            for(i in data.Options) {
                this.addOption(data.Options[i]);
            }
            if(data && data.Foods) {
                _.each(data.Foods, function (x) {
                    var option = _this.getFood(x.WeekIdx, x.DayIdx, x.OptionKey);
                    if(option) {
                        option(x.Food);
                    }
                });
            }
            this.isDirty.reset();
        };
        MenuDefinition.prototype.reset = function (data) {
            _super.prototype.reset.call(this, data && data.WeeksQuantity);
            this.MenuDefinitionReset(data);
        };
        MenuDefinition.prototype.exportData = function () {
            var data = {
                Id: this.Id(),
                DeadlineTime: this.DeadlineTime(),
                LastSentDate: this.LastSentDate(),
                Title: this.Title(),
                FirstWeekIdx: this.FirstWeekIdx(),
                WeeksQuantity: this.WeeksQuantity(),
                StartDate: this.StartDate(),
                EndDate: this.EndDate(),
                Places: ko.toJS(this.Places),
                Options: ko.toJS(this.Options),
                Foods: []
            };
            this.eachDay(function (dayFoods, weekIdx, dayIdx) {
                var food;
                for(var opt in dayFoods) {
                    if(food = dayFoods[opt]()) {
                        data.Foods.push({
                            WeekIdx: weekIdx,
                            DayIdx: dayIdx,
                            OptionKey: opt,
                            Food: food
                        });
                    }
                }
            });
            return data;
        };
        MenuDefinition.prototype.addOption = function (option) {
            var _this = this;
            var op = Utilities.ObservableArrays.addKeyObservableText(this.Options, "Menú ", "menu_", option);
            this.isDirty.register(op.Text);
            this.eachDay(function (dayFoods) {
                var dayOpt = ko.observable("");
                dayFoods[op.Key] = dayOpt;
                _this.isDirty.register(dayOpt);
            });
            this.Options.valueHasMutated();
        };
        MenuDefinition.prototype.removeOption = function (option) {
            var removed = Utilities.ObservableArrays.removeItem(this.Options, option);
            if(removed) {
                this.eachDay(function (dayFoods) {
                    return delete dayFoods[removed.Key];
                });
            }
        };
        MenuDefinition.prototype.addPlace = function (place) {
            var place = Utilities.ObservableArrays.addKeyObservableText(this.Places, "Lugar ", "place_", place);
            this.isDirty.register(place.Text);
        };
        MenuDefinition.prototype.removePlace = function (place) {
            Utilities.ObservableArrays.removeItem(this.Places, place);
        };
        return MenuDefinition;
    })(WeekStorage);
    MyMenu.MenuDefinition = MenuDefinition;    
})(MyMenu || (MyMenu = {}));

