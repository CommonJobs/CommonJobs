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
    key: string; 
    text: knockout.koObservableString; 
}

module Utilities {
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
            var texts = _.map(ko.toJS(collection), (item) => item.text);
            var n = texts.length + 1;

            if (name) {
                if ((<any>name).text) {
                    name = (<any>name).text;
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
            if (!value || !value.key) {
                var text = generateText(baseName, collection, value);
                item = { key: idGenerator.generate(idPrefix), text: ko.observable(text) };
            } else {
                item = { key: value.key, text: ko.observable(value.text) };
            }
            collection.push(item);
            return item;
        }

        export function removeItem(collection: knockout.koObservableArrayBase, item: any, keyField = "key"): any {
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

    export interface MenuDataItem {
        week: number;
        day: number;
        option: string;
        food: string;
    }

    export interface KeyText {
        key: string;
        text: string;
    }

    export interface MenuData {
        Id: string;
        title?: string;
        firstWeek?: number;
        weeksQuantity?: number;
        options?: KeyText[];
        places?: KeyText[];
        startDate?: string;
        endDate?: string;
        deadlineTime?: string;
        foods?: MenuDataItem[]; 
    }

    export interface DayFoods {
        [s: string]: knockout.koObservableString;
    }

    export interface EmployeeMenuDataItem {
        week: number;
        day: number;
        option: string;
        place?: string;
    }

    export interface EmployeeMenuDataOverrideItem {
        date: string;
        cancel?: bool;
        option?: string; 
        place?: string;
        comment?: string;
    }

    export interface EmployeeMenuData {
        Id: string;
        menuId: string;
        userName: string;
        name: string;
        defaultPlace: string;
        choices: EmployeeMenuDataItem[];
        overrides: EmployeeMenuDataOverrideItem[];
    }

    interface DayData {
        week: number;
        day: number;
        value: any;
    }

    class WeekStorage extends Utilities.HasCallbacks {
        weeksQuantity: { (): number; };
        weeks: { (): KeyText[]; };
        private items: any[][];

        constructor (weeksQuantity?: number) {
            super();
            this.weeksQuantity = ko.observable(0);
            this.weeks = ko.computed(() => _.map(_.range(this.weeksQuantity()), (x: number): KeyText => {
                return { key: x.toString(), text: "Semana " + (x + 1) };
            }));
            this.WeekStorageReset(weeksQuantity);
            //(<knockout.koComputed>this.weeks).subscribe((x) => console.log(x));
        }

        private setWeeksQuantity(n: number) {
            //console.log(n);
            (<knockout.koObservableNumber>this.weeksQuantity)(n);
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
            this.setWeeksQuantity(this.weeksQuantity() + 1);
        }

        removeWeek() {
            var actual = this.weeksQuantity();
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
            return days.isValid(day) && week < this.weeksQuantity()
                ? this.items[week][day] 
                : null;
        }

        eachWeek(f: (weekItems: any[], weekIndex: number) => void ) {
            _.each(this.items, (weekItems, weekIndex) => f(weekItems, weekIndex));
        }

        eachDay(f: (dayItem: any, dayIndex: number, weekIndex: number) => void ) {
            this.eachWeek((weekItems, weekIndex) => 
                _.each(weekItems, (dayItem, dayIndex) => 
                    f(dayItem, weekIndex, dayIndex)));
        };
    }

    class DayChoice {
        option: knockout.koObservableString = ko.observable("");
        place: knockout.koObservableString = ko.observable("");
    }

    class Override extends DayChoice {
        date: knockout.koObservableString = ko.observable("");
        cancel: knockout.koObservableBool = ko.observable(false);
        comment: knockout.koObservableString = ko.observable("");

        constructor (data?: EmployeeMenuDataOverrideItem) {
            super();
            if (data) {
                this.option(data.option);
                this.place(data.place);
                this.date(data.date);
                this.cancel(data.cancel);
                this.comment(data.comment);
            }
        }
    }

    export class EmployeeMenuDefinition extends WeekStorage {
        menuId: knockout.koObservableString = ko.observable("");
        name: string;
        Id: string;
        userName: string;
        defaultPlace: knockout.koObservableString = ko.observable("");
        overrides: knockout.koObservableArrayBase = ko.observableArray([]);

        constructor (public menu: MenuDefinition, data?: EmployeeMenuData) {
            super(menu.weeksQuantity());
            this.EmployeeMenuDefinitionReset(data);
        }

        reset(data?: EmployeeMenuData) {
            super.reset(this.menu.weeksQuantity());
            this.EmployeeMenuDefinitionReset(data);
        }

        createNewItem() {
            return new DayChoice();
        }

        eachWeek(f: (weekItems: DayChoice[], weekIndex: number) => void ) {
            super.eachWeek(f);
        }

        eachDay(f: (dayItem: DayChoice, dayIndex: number, weekIndex: number) => void ) {
            super.eachDay(f);
        };

        
        getItem(week: number, day: number): DayChoice {
            return super.getItem(week, day);
        }

        getMenuChoice(week: number, day: number): knockout.koObservableString {
            //return ko.observable("choice " + week + " " + day);
            return this.getItem(week, day).option;
        }

        getPlaceChoice(week: number, day: number): knockout.koObservableString {
            //return ko.observable("place " + week + " " + day);
            return this.getItem(week, day).place;
        }

        getDefaultPlaceLabel() {
            var key = this.defaultPlace();
            var places = this.menu.places();
            var option: KeyObservableText  = _.find(places, (x: KeyObservableText) => x.key == key);
            if (option) {
                return "Lugar por defecto (" + option.text() + ")";
            } else {
                return "Seleccione un lugar (no se definió el lugar por defecto)";
            }
        }

        EmployeeMenuDefinitionReset(data?: EmployeeMenuData) {
            data = _.extend({ userName: "", Id: "", name: "", defaultPlace: "" }, data);
            this.menuId(data.menuId);
            this.Id = data.Id;
            this.userName = data.userName;
            this.name = data.name;
            this.defaultPlace(data.defaultPlace);
            
            if (data.choices) {
                _.each(data.choices, (x: EmployeeMenuDataItem) => {
                    var choice = this.getItem(x.week, x.day);
                    if (choice /*&& TODO: comprobar validez*/) {
                        choice.option(x.option);
                        choice.place(x.place);
                    }
                });
            }

            this.overrides.removeAll();
            if (data.overrides) {
                _.each(data.overrides, x => this.overrides.push(new Override(x)));
            }
        }

        exportData(): EmployeeMenuData {
            var data: EmployeeMenuData = {
                menuId: this.menuId(),
                Id: this.Id,
                userName: this.userName,
                name: this.name,
                defaultPlace: this.defaultPlace(),
                choices: [],
                overrides: []
            };

            var choices = data.choices;
            this.eachDay((dayChoices, weekIndex, dayIndex) => {
                var option = dayChoices.option();
                if (option) {
                    var item: EmployeeMenuDataItem = {
                        week: weekIndex,
                        day: dayIndex,  
                        option: option
                    };
                    var place = dayChoices.place();
                    if (place) {
                        item.place = place;
                    }
                    choices.push(item);
                }
            });

            data.overrides = ko.toJS(this.overrides);

            return data;
        }

        
        addOverride() {
            this.overrides.push(new Override());
        }
        
        removeOverride(override?) {
            Utilities.ObservableArrays.removeItem(this.overrides, override, 'date');
        };
    }
    


    export class MenuDefinition extends WeekStorage  {
        static defaultData: MenuData = {
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

        Id: knockout.koObservableString = ko.observable("");
        title: knockout.koObservableString = ko.observable("");
        weeksQuantity: { (): number; };
        options: knockout.koObservableArrayBase = ko.observableArray();
        places: knockout.koObservableArrayBase = ko.observableArray();
        startDate: knockout.koObservableAny = ko.observable("");
        endDate: knockout.koObservableAny = ko.observable("");
        deadlineTime: knockout.koObservableString = ko.observable("");
        firstWeek: knockout.koObservableNumber = ko.observable(0);
        static idGenerator = new Utilities.IdGenerator();

        //#region Extend WeekStorage

        createNewItem(): DayFoods {
            var item: DayFoods = {};
            if (this.options) {
                _.each(this.options(), option =>
                    item[option.key] = ko.observable(""));
            }
            return item;
        }

        getItem(week: number, day: number): DayFoods {
            return super.getItem(week, day);
        }

        getFood(week: number, day: number, option: string): knockout.koObservableString {
            //Dependencies
            this.weeksQuantity();
            this.options();

            var dayFoods = this.getItem(week, day);
            return dayFoods && dayFoods[option];
        }

        eachWeek(f: (weekItems: DayFoods[], weekIndex: number) => void ) {
            super.eachWeek(f);
        }

        eachDay(f: (dayItem: DayFoods, dayIndex: number, weekIndex: number) => void ) {
            super.eachDay(f);
        };

        //#endregion

        constructor (data?: MenuData) {
            super(data && data.weeksQuantity);
            this.MenuDefinitionReset(data);
        }

        private createKeyTextObservableArray(items: KeyText[]) {
            return ko.observableArray(_.map(items, (item) => { 
                return {
                    key: item.key,
                    text: ko.observable(item.text)
                };
            }));
        }

        private MenuDefinitionReset(data?: MenuData) {
            data = <MenuData>$.extend({}, MenuDefinition.defaultData, data);
            var i: any;
            this.Id(data.Id);
            this.title(data.title);
            this.startDate(data.startDate);
            this.endDate(data.endDate);
            this.firstWeek(data.firstWeek);
            this.deadlineTime(data.deadlineTime);

            this.places.removeAll();
            for (i in data.places) {
                this.addPlace(data.places[i]);
            } 
            
            this.options.removeAll();
            for (i in data.options) {
                this.addOption(data.options[i]);
            }
            
            if (data && data.foods) {
                _.each(data.foods, x => {
                    var option = this.getFood(x.week, x.day, x.option);
                    if (option) {
                        option(x.food)
                    }
                });
            }
        };

        reset(data?: MenuData) {
            super.reset(data && data.weeksQuantity);
            this.MenuDefinitionReset(data);
        }
        
        exportData(): MenuData {
            var data: MenuData = { 
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

            this.eachDay((dayFoods, weekIndex, dayIndex) => {
                var food;
                for (var opt in dayFoods) {
                    if (food = dayFoods[opt]()) {
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
        }

        addOption(option?: any) {
            var op = Utilities.ObservableArrays.addKeyObservableText(this.options, "Menú ", "menu_", option);
            
            this.eachDay(dayFoods => {
                dayFoods[op.key] = ko.observable("")
            })

            //In order to update content observables
            this.options.valueHasMutated();
        }
  
        removeOption(option?) {
            var removed = Utilities.ObservableArrays.removeItem(this.options, option);
            if (removed) { 
                this.eachDay(dayFoods =>  delete dayFoods[removed.key]);
            }
        };

        addPlace(place?: any) {
            Utilities.ObservableArrays.addKeyObservableText(this.places, "Lugar ", "place_", place);
        }
        
        removePlace(place?) {
            Utilities.ObservableArrays.removeItem(this.places, place);
        };
    }
}