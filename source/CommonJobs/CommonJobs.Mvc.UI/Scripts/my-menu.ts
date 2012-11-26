///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path="moment.d.ts" />

 
//#region Underscore personalizations
declare interface UnderscoreVoidIMenuItemListIterator {
    (element : MyMenu.MenuDataItem, index : number, list : MyMenu.MenuDataItem[]) : void;
}

declare interface UnderscoreStatic extends Function {
    each(list: MyMenu.MenuDataItem[], iterator: UnderscoreVoidIMenuItemListIterator, context?: any): void;
}
//#endregion

interface KeyObservableText { 
    Key: string; 
    Text: knockout.koObservableString; 
}

module Utilities {
	//TODO export an interface too
    export function dirtyFlag() : any {
		var observable : any = ko.observable(false);

		observable.register = (anotherObservable : any) => {
			anotherObservable.subscribe(() => {
				observable(true);
			});
		}

		//TODO: observable.deregister?

		observable.reset = () => observable(false);
		return observable;
	}

    export class IdGenerator {
        constructor (public chars = "abcdefghijklmnopqrstuvwxyz1234567890", public length = 8) {
        }
        public generate(prefix = "") {
            var chars = this.chars;
            var charsL = chars.length;
            return prefix + _.map(_.range(this.length), () => chars[_.random(0, charsL)]).join("");
        };
    }

    export var idGenerator = new IdGenerator();

    export class HasCallbacks {
	    constructor() {
		    var _this = this, _constructor = (<any>this).constructor;
		    if (!_constructor.__cb__) {
			    _constructor.__cb__ = {};
			    for (var m in this) {
				    var fn = this[m];
				    if (typeof fn === 'function' && m.indexOf('__cb__') == -1) {
					    _constructor.__cb__[m] = fn;					
				    }
			    }
		    }
		    for (var m in _constructor.__cb__) {
			    (function (m, fn) {
				    _this[m] = function () {
					    return fn.apply(_this, Array.prototype.slice.call(arguments));						
				    };
			    })(m, _constructor.__cb__[m]);
		    }
	    }
    }

    export module ObservableArrays {
        function generateText(baseName: string, collection: knockout.koObservableArrayBase, name?: string): string {
            var texts = _.map(ko.toJS(collection), (item) => item.Text);
            var n = texts.length + 1;

            if (name) {
                if ((<any>name).Text) {
                    name = (<any>name).Text;
                }
                if (_.isString(name)) {
                    if (texts.indexOf(name) == -1) {
                        return name;
                    } else {
                        baseName = name;
                        n = 2;
                    }
                }
            }

            while (true) {
                var name = baseName + n++;
                if (texts.indexOf(name) == -1)
                    return name;
            }
        }

        export function addKeyObservableText(collection: knockout.koObservableArrayBase, baseName: string, idPrefix: string, value?: any): KeyObservableText {
            var item: KeyObservableText;
            if (!value || !value.Key) {
                var text = generateText(baseName, collection, value);
                item = { Key: idGenerator.generate(idPrefix), Text: ko.observable(text) };
            } else {
                item = { Key: value.Key, Text: ko.observable(value.Text) };
            }
            collection.push(item);
            return item;
        }

        export function removeItem(collection: knockout.koObservableArrayBase, item: any, keyField = "Key"): any {
            if (!collection().length)
                return null;

            if (_.isString(item))
                return collection.remove(x => x[keyField] == item);

            var index: number = _.isNumber(item) ? item : collection.indexOf(item);
            return collection.splice(index, 1)[0];
        }
    }
}


module MyMenu {
    
    class CalendarHelper {
        StartDate: () => moment.Moment;
        WeeksQuantity: () => number;
        FirstWeekIdx: () => number; 
        ZeroWeekZeroDay: () => moment.Moment;

        private calculateZeroWeekZeroDay() {
            return this.StartDate().day(0).add('weeks', -1 * this.FirstWeekIdx());
        }

        constructor (startDate: any, weeksQuantity: any, firstWeek: any) {
            var startDateIsFunc = _.isFunction(startDate);
            var weeksQuantityIsFunc = _.isFunction(weeksQuantity);
            var firstWeekIsFunc = _.isFunction(firstWeek);

            if (startDateIsFunc) {
                this.StartDate = () => moment(startDate());
            } else {
                this.StartDate = () => moment(startDate);
            }

            this.WeeksQuantity = weeksQuantityIsFunc ? weeksQuantity : () => weeksQuantity;

            if (firstWeekIsFunc) {
                this.FirstWeekIdx = () => firstWeek() || 0;
            } else {
                firstWeek = firstWeek || 0;
                this.FirstWeekIdx = () => firstWeek;
            }

            if (startDateIsFunc || weeksQuantityIsFunc || firstWeekIsFunc) {
                this.ZeroWeekZeroDay = () => this.calculateZeroWeekZeroDay();
            } else {
                var zeroWeekZeroDay = this.calculateZeroWeekZeroDay();
                this.ZeroWeekZeroDay = () => zeroWeekZeroDay;
            }
        }

        week(date?: any) {
            var weeksQuantity = this.WeeksQuantity();
            var sundayDate = moment(date).day(0);
            var diff = sundayDate.diff(this.ZeroWeekZeroDay(), 'weeks');

            var result = diff < 0 ?
                weeksQuantity + diff % weeksQuantity
                : diff % weeksQuantity;
            return result;
        }

        day(date?: any) {
            var result = moment(date).day();
            return result;
        }

        weekDay(date?: any) : WeekDay {
            return {
                DayIdx: this.day(date),
                WeekIdx: this.week(date)
            }
        }

        match(weekIdx: number, dayIdx: number, date?: any) {
            var dateAsWeekDay = this.weekDay(date);
            return weekIdx == dateAsWeekDay.WeekIdx && dayIdx == dateAsWeekDay.DayIdx;
        }

        near(weekIdx: number, dayIdx: number, date?: any) : moment.Moment {
            var weeksQuantity = this.WeeksQuantity();
            if (weeksQuantity <= weekIdx)
                return null;

            //TODO: optimize
            var mmnt = moment(date);
            if (!mmnt.isValid) {
                return null;
            }
            while (!this.match(weekIdx, dayIdx, mmnt)) {
                mmnt.add('days', 1);
            }
            return mmnt;
        }
    }


    export interface WeekDay {
        WeekIdx: number;
        DayIdx: number;
    }

 
    //Move days selection inside WeekStorage
    export interface Days {
        (): number[];
        getName(day: number): string;
        isValid(day: number): bool;
    }

    export var days = <Days>(function () { 
        var DAYS = [1, 2, 3, 4, 5];
        var def = function () {
            return DAYS;
        }
        def["getName"] = function (day: number) {
            return moment().day(day).format("dddd");
        };
        def["isValid"] = function (day: number) {
            return DAYS.indexOf(day) >= 0;
        };
        return def; 
    })();

    export interface MenuDataItem extends WeekDay {
        OptionKey: string;
        Food: string;
    }

    export interface KeyText {
        Key: string;
        Text: string;
    }

    export interface MenuData {
        Id: string;
        Title?: string;
        FirstWeekIdx?: number;
        WeeksQuantity?: number;
        Options?: KeyText[];
        Places?: KeyText[];
        StartDate?: string;
        EndDate?: string;
        LastSentDate?: string;
        DeadlineTime?: string;
        Foods?: MenuDataItem[]; 
    }

    export interface DayFoods {
        [s: string]: knockout.koObservableString;
    }

    export interface EmployeeMenuDataItem extends WeekDay {
        OptionKey: string;
        PlaceKey?: string;
    }

    export interface EmployeeMenuDataOverrideItem {
        Date: string;
        Cancel?: bool;
        OptionKey?: string; 
        PlaceKey?: string;
        Comment?: string;
    }

    export interface EmployeeMenuData {
        Id: string;
        MenuId: string;
        UserName: string;
        EmployeeName: string;
        DefaultPlaceKey: string;
        WeeklyChoices: EmployeeMenuDataItem[];
        Overrides: EmployeeMenuDataOverrideItem[];
    }

    interface DayData extends WeekDay {
        Value: any;
    }

    class WeekStorage extends Utilities.HasCallbacks {
        WeeksQuantity: { (): number; };
        Weeks: { (): KeyText[]; };
        private items: any[][];
		//TODO: use a type
		isDirty: any = Utilities.dirtyFlag();

        constructor (weeksQuantity?: number) {
            super();
            this.WeeksQuantity = ko.observable(0);
			this.isDirty.register(this.WeeksQuantity);
            this.Weeks = ko.computed(() => _.map(_.range(this.WeeksQuantity()), (x: number): KeyText => {
                return { Key: x.toString(), Text: "Semana " + (x + 1) };
            }));
            this.WeekStorageReset(weeksQuantity);
        }

        private setWeeksQuantity(n: number) {
            (<knockout.koObservableNumber>this.WeeksQuantity)(n);
        }

        reset(weeksQuantity?: number) {
            this.WeekStorageReset(weeksQuantity);
        }

        private WeekStorageReset(weeksQuantity?: number) {
            var i: any;
            this.setWeeksQuantity(0);
            this.items = [];

            if (weeksQuantity) {
                for (i = 0; i < weeksQuantity; i++) {
                    this.addWeek();
                }
            }
        }

        addWeek() {
            var weekItems = [];

            for (var i = 0; i < 7; i++) {
                var dayItem = this.createNewItem();
                weekItems.push(dayItem);
            }
            this.items.push(weekItems);
            this.setWeeksQuantity(this.WeeksQuantity() + 1);
        }

        removeWeek() {
            var actual = this.WeeksQuantity();
            if (actual > 0) {
                this.setWeeksQuantity(actual - 1);
                this.items.pop();
            }    
        }

        ///To override
        createNewItem() {
            return null;
        }

        getItem(week: number, day: number): any {
            return days.isValid(day) && week < this.WeeksQuantity()
                ? this.items[week][day] 
                : null;
        }

        eachWeek(f: (weekItems: any[], weekIdx: number) => void ) {
            _.each(this.items, (weekItems, weekIdx) => f(weekItems, weekIdx));
        }

        eachDay(f: (dayItem: any, dayIdx: number, weekIdx: number) => void ) {
            this.eachWeek((weekItems, weekIdx) => 
                _.each(weekItems, (dayItem, dayIdx) => 
                    f(dayItem, weekIdx, dayIdx)));
        };
    }

    class DayChoice {
        OptionKey: knockout.koObservableString = ko.observable("");
        PlaceKey: knockout.koObservableString = ko.observable("");
    }

    class Override extends DayChoice {
        Date: knockout.koObservableString = ko.observable("");
        Cancel: knockout.koObservableBool = ko.observable(false);
        Comment: knockout.koObservableString = ko.observable("");

        constructor (data?: EmployeeMenuDataOverrideItem) {
            super();
            if (data) {
                this.OptionKey(data.OptionKey);
                this.PlaceKey(data.PlaceKey);
                this.Date(data.Date);
                this.Cancel(data.Cancel);
                this.Comment(data.Comment);
            }
        }
    }

    export class EmployeeMenuDefinition extends WeekStorage {
        MenuId: knockout.koObservableString = ko.observable("");
        EmployeeName: string;
        Id: string;
        UserName: string;
        DefaultPlaceKey: knockout.koObservableString = ko.observable("");
        Overrides: knockout.koObservableArrayBase = ko.observableArray([]);
        calendarHelper: CalendarHelper;
        now: knockout.koObservableAny = ko.observable();

        constructor (public menu: MenuDefinition, data?: EmployeeMenuData, now?: any) {
            super(menu.WeeksQuantity());
			this.isDirty.register(this.MenuId);
			this.isDirty.register(this.DefaultPlaceKey);
			this.isDirty.register(this.Overrides);

            this.EmployeeMenuDefinitionReset(data);
            this.calendarHelper = new CalendarHelper(menu.StartDate, menu.WeeksQuantity, menu.FirstWeekIdx);
            this.now(now);
        }

        reset(data?: EmployeeMenuData) {
            super.reset(this.menu.WeeksQuantity());
            this.EmployeeMenuDefinitionReset(data);
			this.isDirty(false);
        }

        nearFormated(weekIdx: any, dayIdx: number) {
            //TODO: remove
            var now = moment(this.now());
            var date = this.calendarHelper.near(+weekIdx, dayIdx, now);

            if (date == null)
                return null;

            var days = date.diff(now, 'days', true);

            var str = date.format("D [de] MMMM [de] YYYY");

            return days == 0 ? "Hoy (" + str + ")"
                : days == 1 ? "Mañana (" + str + ")"
                : days == 2 ? "Pasado mañana (" + str + ")"
                : days < 7 ? "En " + days + " días (" + str + ")"
                : str;
        }
        
        createNewItem() {
			var dayChoice = new DayChoice();
			this.isDirty.register(dayChoice.OptionKey); //TODO: potential memory leak
			this.isDirty.register(dayChoice.PlaceKey); //TODO: potential memory leak
            return dayChoice;
        }

        eachWeek(f: (weekItems: DayChoice[], weekIdx: number) => void ) {
            super.eachWeek(f);
        }

        eachDay(f: (dayItem: DayChoice, dayIdx: number, weekIdx: number) => void ) {
            super.eachDay(f);
        };

        
        getItem(week: number, day: number): DayChoice {
            return super.getItem(week, day);
        }

        getMenuChoice(week: number, day: number): knockout.koObservableString {
            //return ko.observable("choice " + week + " " + day);
            return this.getItem(week, day).OptionKey;
        }

        getPlaceChoice(week: number, day: number): knockout.koObservableString {
            //return ko.observable("place " + week + " " + day);
            return this.getItem(week, day).PlaceKey;
        }

        getDefaultPlaceLabel() {
            var key = this.DefaultPlaceKey();
            var places = this.menu.Places();
            var option: KeyObservableText  = _.find(places, (x: KeyObservableText) => x.Key == key);
            if (option) {
                return "Lugar por defecto (" + option.Text() + ")";
            } else {
                return "Seleccione un lugar (no se definió el lugar por defecto)";
            }
        }

        EmployeeMenuDefinitionReset(data?: EmployeeMenuData) {
            data = _.extend({ UserName: "", Id: "", EmployeeName: "", DefaultPlaceKey: "" }, data);
            this.MenuId(data.MenuId);
            this.Id = data.Id;
            this.UserName = data.UserName;
            this.EmployeeName = data.EmployeeName;
            this.DefaultPlaceKey(data.DefaultPlaceKey);
            
            if (data.WeeklyChoices) {
                _.each(data.WeeklyChoices, (x: EmployeeMenuDataItem) => {
                    var choice = this.getItem(x.WeekIdx, x.DayIdx);
                    if (choice /*&& TODO: comprobar validez*/) {
                        choice.OptionKey(x.OptionKey);
                        choice.PlaceKey(x.PlaceKey);
                    }
                });
            }

            this.Overrides.removeAll();
            if (data.Overrides) {
                _.each(data.Overrides, x => { 
					var ov = new Override(x);
					this.Overrides.push(ov);
					this.isDirty.register(ov.OptionKey);
					this.isDirty.register(ov.PlaceKey);
					this.isDirty.register(ov.Date);
					this.isDirty.register(ov.Cancel);
					this.isDirty.register(ov.Comment);
				});
            }
        }

        exportData(): EmployeeMenuData {
            var data: EmployeeMenuData = {
                MenuId: this.MenuId(),
                Id: this.Id,
                UserName: this.UserName,
                EmployeeName: this.EmployeeName,
                DefaultPlaceKey: this.DefaultPlaceKey(),
                WeeklyChoices: [],
                Overrides: []
            };

            var choices = data.WeeklyChoices;
            this.eachDay((dayChoices, weekIdx, dayIdx) => {
                var optionKey = dayChoices.OptionKey();
                if (optionKey) {
                    var item: EmployeeMenuDataItem = {
                        WeekIdx: weekIdx,
                        DayIdx: dayIdx,  
                        OptionKey: optionKey
                    };
                    var place = dayChoices.PlaceKey();
                    if (place) {
                        item.PlaceKey = place;
                    }
                    choices.push(item);
                }
            });

            data.Overrides = ko.toJS(this.Overrides);

            return data;
        }

        
        addOverride() {
			var override = new Override();
            this.Overrides.push(override);
			this.isDirty.register(override.OptionKey); //TODO: potential memory leak
			this.isDirty.register(override.PlaceKey); //TODO: potential memory leak
			this.isDirty.register(override.Date); //TODO: potential memory leak
			this.isDirty.register(override.Cancel); //TODO: potential memory leak
			this.isDirty.register(override.Comment); //TODO: potential memory leak
        }
        
        removeOverride(override?) {
            Utilities.ObservableArrays.removeItem(this.Overrides, override, 'date');
        };
    }
    


    export class MenuDefinition extends WeekStorage  {
        static defaultData: MenuData = {
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

        Id: knockout.koObservableString = ko.observable("");
        Title: knockout.koObservableString = ko.observable("");
        //weeksQuantity: { (): number; };
        Options: knockout.koObservableArrayBase = ko.observableArray();
        Places: knockout.koObservableArrayBase = ko.observableArray();
        StartDate: knockout.koObservableAny = ko.observable("");
        EndDate: knockout.koObservableAny = ko.observable("");
        DeadlineTime: knockout.koObservableString = ko.observable("");
        LastSentDate: knockout.koObservableString = ko.observable("");
        FirstWeekIdx: knockout.koObservableNumber = ko.observable(0);
        static idGenerator = new Utilities.IdGenerator();

        //#region Extend WeekStorage

        createNewItem(): DayFoods {
            var item: DayFoods = {};
            if (this.Options) {
                _.each(this.Options(), option => {
					var obs = ko.observable("");
                    item[option.Key] = obs;
					this.isDirty.register(obs); //TODO: potential memory leak 
				});
            }
            return item;
        }

        getItem(week: number, day: number): DayFoods {
            return super.getItem(week, day);
        }

        getFood(week: number, day: number, option: string): knockout.koObservableString {
            //Dependencies
            this.WeeksQuantity();
            this.Options();

            var dayFoods = this.getItem(week, day);
            return dayFoods && dayFoods[option];
        }

        eachWeek(f: (weekItems: DayFoods[], weekIdx: number) => void ) {
            super.eachWeek(f);
        }

        eachDay(f: (dayItem: DayFoods, dayIdx: number, weekIdx: number) => void ) {
            super.eachDay(f);
        };

        //#endregion

        constructor (data?: MenuData) {
            super(data && data.WeeksQuantity);
			this.isDirty.register(this.Id);
			this.isDirty.register(this.Title);
			this.isDirty.register(this.Options);
			this.isDirty.register(this.Places);
			this.isDirty.register(this.StartDate);
			this.isDirty.register(this.EndDate);
			this.isDirty.register(this.DeadlineTime);
			this.isDirty.register(this.FirstWeekIdx);
            this.MenuDefinitionReset(data);
        }
		
        private MenuDefinitionReset(data?: MenuData) {
            data = <MenuData>$.extend({}, MenuDefinition.defaultData, data);
            var i: any;
            this.Id(data.Id);
            this.Title(data.Title);
            this.StartDate(data.StartDate);
            this.EndDate(data.EndDate);
            this.FirstWeekIdx(data.FirstWeekIdx);
            this.DeadlineTime(data.DeadlineTime);
            this.LastSentDate(data.LastSentDate);

            this.Places.removeAll();
            for (i in data.Places) {
                this.addPlace(data.Places[i]);
            } 
            
            this.Options.removeAll();
            for (i in data.Options) {
                this.addOption(data.Options[i]);
            }
            
            if (data && data.Foods) {
                _.each(data.Foods, x => {
                    var option = this.getFood(x.WeekIdx, x.DayIdx, x.OptionKey);
                    if (option) {
                        option(x.Food)
                    }
                });
            }

			this.isDirty.reset();
        };

        reset(data?: MenuData) {
            super.reset(data && data.WeeksQuantity);
            this.MenuDefinitionReset(data);
        }
        
        exportData(): MenuData {
            var data: MenuData = { 
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

            this.eachDay((dayFoods, weekIdx, dayIdx) => {
                var food;
                for (var opt in dayFoods) {
                    if (food = dayFoods[opt]()) {
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
        }

        addOption(option?: any) {
            var op: KeyObservableText = Utilities.ObservableArrays.addKeyObservableText(this.Options, "Menú ", "menu_", option);
			this.isDirty.register(op.Text); //TODO: potential memory leak
            
            this.eachDay(dayFoods => {
				var dayOpt = ko.observable("");
                dayFoods[op.Key] = dayOpt;
				this.isDirty.register(dayOpt); //TODO: potential memory leak
            })

            //In order to update content observables
            this.Options.valueHasMutated();
        }
  
        removeOption(option?) {
            var removed = Utilities.ObservableArrays.removeItem(this.Options, option);
            if (removed) { 
                this.eachDay(dayFoods =>  delete dayFoods[removed.Key]);
            }
        };

        addPlace(place?: any) {
            var place: KeyObservableText = Utilities.ObservableArrays.addKeyObservableText(this.Places, "Lugar ", "place_", place);
			this.isDirty.register(place.Text); //TODO: potential memory leak
        }
        
        removePlace(place?) {
            Utilities.ObservableArrays.removeItem(this.Places, place);
        };
    }
}