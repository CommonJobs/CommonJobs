///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path="moment.d.ts" />


//#region Underscore personalizations
declare interface UnderscoreVoidIMenuItemListIterator {
    (element : CommonFood.IMenuItem, index : number, list : CommonFood.IMenuItem[]) : void;
}

declare interface UnderscoreStatic extends Function {
    each(list: CommonFood.IMenuItem[], iterator: UnderscoreVoidIMenuItemListIterator, context?: any): void;
}
//#endregion

module CommonFood {

    export interface Days {
        (): number[];
        getName(day: number): string;
        isValid(day: number): bool;
    }

    export var days = <Days>(function () { 
        var values = [1, 2, 3, 4, 5];
        var def = function () {
            return values;
        }
        def["getName"] = function (day: number) {
            return moment().day(day).format("dddd");
        };
        def["isValid"] = function (day: number) {
            return values.indexOf(day) >= 0;
        };
        return def; 
    })();

    export interface IMenuItem {
        week: number;
        day: number;
        opt: string;
        food: string;
    }

    export interface IKeyText {
        key: string;
        text: string;
    }

    interface IKeyObservabletext { 
        key: string; 
        text: knockout.koObservableString; 
    }

    

    export interface IDayFoods {
        [s: string]: knockout.koObservableString;
    }
    
    export interface IMenu {
        title?: string;
        firstWeek?: number;
        firstDay?: number;
        weeks?: number;
        options?: IKeyText[];
        options?: string[];
        places?: IKeyText[];
        startDate?: string;
        endDate?: string;
        deadlineTime?: string;
        foods?: IMenuItem[]; 
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

    class HasCallbacks {
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

    export class MenuViewModel extends HasCallbacks  {
        static defaultModel: IMenu = {
            title: "Nuevo Menú",
            firstWeek: 0,
            firstDay: 0,
            weeks: 4,
            options: [{ key: "dcomun", text: "Común" }, { key: "dlight", text: "Light" }, { key: "dvegetariano", text: "Vegetariano" }],
            places: [{ key: "dlarioja", text: "La Rioja" }, { key: "dgaray", text: "Garay" }],
            startDate: "",
            endDate: "",
            deadlineTime: "09:30",
            foods: []
        };

        title: knockout.koObservableString = ko.observable("");
        weeks: knockout.koObservableNumber = ko.observable(0);
        options: knockout.koObservableArrayBase = ko.observableArray();
        places: knockout.koObservableArrayBase = ko.observableArray();
        startDate: knockout.koObservableAny = ko.observable("");
        endDate: knockout.koObservableAny = ko.observable("");
        deadlineTime: knockout.koObservableString = ko.observable("");
        firstWeek: knockout.koObservableNumber = ko.observable(0);
        firstDay: knockout.koObservableNumber = ko.observable(0);
        foods: knockout.koObservableArrayBase = ko.observableArray(); //By week, by day, by option
        private idGenerator = new IdGenerator();

        constructor (model?: IMenu) {
            super();
            this.reset(model);
        }

        private createKeyTextObservableArray(items: IKeyText[]) {
            return ko.observableArray(_.map(items, (item) => { 
                return {
                    key: item.key,
                    text: ko.observable(item.text)
                };
            }));
        }

        reset(model?: IMenu) {
            model =  <IMenu>$.extend({}, MenuViewModel.defaultModel, model);
            var i: any;
            this.title(model.title);
            this.weeks(0);
            this.options([]);
            this.places([]);
            this.startDate(model.startDate);
            this.endDate(model.endDate);
            this.firstWeek(model.firstWeek);
            this.firstDay(model.firstDay);
            this.deadlineTime(model.deadlineTime);
            //this.foods([]); //By week / day / option
            this.foods.removeAll();
        
            for (i in model.options) {
                this.addOption(model.options[i]);
            }        

            //this.places = createKeyTextObservableArray(model.places)
            for (i in model.places) {
                this.addPlace(model.places[i]);
            } 
                   
            for (i = 0; i < model.weeks; i++) {
                this.addWeek();
            }

            var weeksLength = this.weeks();
            var opts = {};
            _.each(this.options(), x => { opts[x.key] = true; });

            var foods = <IDayFoods[][]>this.foods();
            if (model.foods) {
                _.each(model.foods, item => {
                    if (days.isValid(item.day) && item.week < weeksLength && opts[item.opt]) {
                        foods[item.week][item.day][item.opt](item.food);
                    }
                });
            }
        }
        
        exportModel(): IMenu {
            var model: IMenu = { };

            var simpleProperties = ["title", "firstWeek", "firstDay", "weeks", "startDate", "endDate"];
            _.each(simpleProperties, (prop) => {
                model[prop] = this[prop]();
            });

            model.places = ko.toJS(this.places);
            model.options = ko.toJS(this.options);

            var foods = model.foods = [];

            this.eachDay((dayFoods, weekIndex, dayIndex) => {
                for (var opt in dayFoods) {
                    var food = dayFoods[opt]();
                    if (food) {
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
        }

        getFood(weekIndex: number, dayIndex: number, opt: string): knockout.koObservableString {
            return this.foods()[weekIndex][dayIndex][opt];
        }

        addWeek() {
            var weekFoods: IDayFoods[] = [];

            for (var i = 0; i < 7; i++) {
                var dayFoods: IDayFoods = {};
                _.each(this.options(), option => 
                    dayFoods[option.key] = ko.observable(""));
                weekFoods.push(dayFoods);
            }
            this.foods.push(weekFoods);
            this.weeks(this.weeks() + 1);
        }

        removeWeek() {
            var actual = this.weeks();
            if (actual > 0) {
                this.weeks(actual - 1);
                this.foods.pop();
            }            
        };
    
        private eachWeek(f: (weekFoods: IDayFoods[], weekIndex: number) => void ) {
            _.each(this.foods(), (weekFoods, weekIndex) => f(weekFoods, weekIndex));
        }

        private eachDay(f: (dayFoods: IDayFoods, dayIndex: number, weekIndex: number) => void ) {
            this.eachWeek((weekFoods, weekIndex) => 
                _.each(weekFoods, (dayFoods, dayIndex) => 
                    f(dayFoods, weekIndex, dayIndex)));
        };

        private generateText(baseName: string, collection: knockout.koObservableArrayBase, name?: string): string {
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

        private addKeyObservableText(collection: knockout.koObservableArrayBase, baseName: string, idPrefix: string, value?: any): IKeyObservabletext {
            var item: IKeyObservabletext;
            if (!value || !value.key) {
                var text = this.generateText(baseName, collection, value);
                item = { key: this.idGenerator.generate(idPrefix), text: ko.observable(text) };
            } else {
                item = { key: value.key, text: ko.observable(value.text) };
            }
            collection.push(item);
            return item;
        }

        addOption(option?: any) {
            var op = this.addKeyObservableText(this.options, "Menú ", "option_", option);
            this.eachDay(dayFoods => {
                dayFoods[op.key] = ko.observable("")
            })
        }
  
        removeOption(option?) {
            var removed = this.removeItem(this.options, option);
            if (removed) { 
                this.eachDay(dayFoods =>  delete dayFoods[removed.key]);
            }
        };

        addPlace(place?: any) {
            this.addKeyObservableText(this.places, "Lugar ", "place_", place);
        }

        private removeItem(collection: knockout.koObservableArrayBase, item: any) : IKeyObservabletext {
            if (!collection().length)
                return null;

            if (_.isString(item)) 
                return collection.remove(x => x.key == item);
            
            var index: number = _.isNumber(item) ? item : collection.indexOf(item); 
            return collection.splice(index, 1)[0];
        }
        
        removePlace(place?) {
            this.removeItem(this.places, place);
        };
    }
}