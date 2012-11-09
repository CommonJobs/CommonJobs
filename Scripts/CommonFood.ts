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
    export interface IMenuItem {
        week: number;
        day: number;
        option: number;
        food: string;
    }

    export interface IMenu {
        title?: string;
        firstWeek?: number;
        firstDay?: number;
        weeks?: number;
        weekFirstDay?: number;
        weekLastDay?: number;
        options?: string[];
        places?: string[];
        startDate?: string;
        endDate?: string;
        deadlineTime?: string;
        foods?: IMenuItem[]; 
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
            weekFirstDay: 1, //Monday
            weekLastDay: 5, //Friday
            options: [ "Común", "Light", "Vegetariano" ],
            places: [ "La Rioja", "Garay" ],
            startDate: "",
            endDate: "",
            deadlineTime: "09:30",
            foods: []
        };

        title: knockout.koObservableString = ko.observable("");
        weeks: knockout.koObservableNumber = ko.observable(0);
        weekFirstDay: knockout.koObservableNumber = ko.observable(1);
        weekLastDay: knockout.koObservableNumber = ko.observable(5);
        options: knockout.koObservableArrayBase = ko.observableArray();
        places: knockout.koObservableArrayBase = ko.observableArray();
        startDate: knockout.koObservableAny = ko.observable("");
        endDate: knockout.koObservableAny = ko.observable("");
        deadlineTime: knockout.koObservableString = ko.observable("");
        firstWeek: knockout.koObservableNumber = ko.observable(0);
        firstDay: knockout.koObservableNumber = ko.observable(0);
        foods: knockout.koObservableArrayBase = ko.observableArray(); //By week, by day, by option
        days: knockout.koExtend;

        constructor (model?: IMenu) {
            super();
            this.reset(model);
            this.days = ko.computed(() => { 
                var mmnt = moment();
                var result = [];
                for (var i = this.weekFirstDay(); i <= this.weekLastDay(); i++) {
                    result.push({ 
                        day: i,
                        name: mmnt.day(i).format("dddd")});
                }
                return result;
            });
        }
        reset(model?: IMenu) {
            model =  <IMenu>$.extend({}, MenuViewModel.defaultModel, model);
            this.title(model.title);
            this.weeks(0);
            this.weekFirstDay(model.weekFirstDay);
            this.weekLastDay(model.weekLastDay);
            this.options([]);
            this.places([]);
            this.startDate(model.startDate);
            this.endDate(model.endDate);
            this.firstWeek(model.firstWeek);
            this.firstDay(model.firstDay);
            this.deadlineTime(model.deadlineTime);
            this.foods([]); //By week / day / option
        
            for (var s in model.options) {
                this.addOption(model.options[s]);
            }        
            for (var s in model.places) {
                this.addPlace(model.places[s]);
            }        
            for (var i = 0; i < model.weeks; i++) {
                this.addWeek();
            }

            var weekFirstDay = this.weekFirstDay();
            var weekLastDay = this.weekLastDay();
            var weeksLength = this.weeks();
            var optionsLength = this.options().length;
            var foods = this.foods();
            if (model.foods) {
                _.each(model.foods, item => {
                    if (item.day >= weekFirstDay && item.day <= weekLastDay && item.week < weeksLength && item.option < optionsLength) {
                        foods[item.week][item.day][item.option](item.food);
                    }
                });
            }
        }
        
        exportModel(): IMenu {
            var model: IMenu = { };

            var simpleProperties = ["title", "firstWeek", "firstDay", "weeks", "weekFirstDay", "weekLastDay", "startDate", "endDate"];
            _.each(simpleProperties, (prop) => {
                model[prop] = this[prop]();
            });
            var textArrProperties = ["options", "places"];
            _.each(textArrProperties, (prop) => {
                model[prop] = _.map(this[prop](), (item) => item.text());
            });

            var foods = model.foods = [];

            this.eachDay((dayFoods, weekIndex, dayIndex) => {
                _.each(dayFoods, (option, optionIndex) => {
                    var food = option();
                    if (food) {
                        foods.push({
                            week: weekIndex,
                            day: dayIndex,
                            option: optionIndex,
                            food: food
                        });
                    }
                });
            });

            return model;
        }

        getFood(weekIndex: number, dayIndex: number, optionIndex: number): knockout.koObservableString {
            return this.foods()[weekIndex][dayIndex][optionIndex];
        }

        addWeek() {
            var weekFoods: knockout.koObservableString[][] = [];

            for (var i = 0; i < 7; i++) {
                var dayFoods: knockout.koObservableString[] = [];
                _.each(this.options(), option => 
                    dayFoods.push(ko.observable("")));
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
    
        private eachWeek(f: (weekFoods: knockout.koObservableString[][], weekIndex: number) => void ) {
            _.each(this.foods(), (weekFoods, weekIndex) => f(weekFoods, weekIndex));
        }

        private eachDay(f: (dayFoods: knockout.koObservableString[], dayIndex: number, weekIndex: number) => void ) {
            this.eachWeek((weekFoods, weekIndex) => 
                _.each(weekFoods, (dayFoods, dayIndex) => 
                    f(dayFoods, weekIndex, dayIndex)));
        };

        private generateText(baseName: string, collection: knockout.koObservableArrayBase, name?: string): string {
            var texts = _.map(ko.toJS(collection), (item) => item.text);
            var n = texts.length + 1;
            if (_.isString(name) && name) {
                if (texts.indexOf(name) == -1) {
                    return name;
                } else {
                    baseName = name;
                    n = 2;
                }
            } 
            while (true) {
                var name = baseName + n++;
                if (texts.indexOf(name) == -1) 
                    return name;
            }
        }

        addOption(text?: string) {
            text = this.generateText("Menú ", this.options, text);
            var option = { text: ko.observable(text) };
        
            this.eachDay(dayFoods => 
                dayFoods.push(ko.observable("")));

            //No hay generics, de manera que this.options acepta cualquier cosa
            this.options.push(option);
        }
  
        removeOption(option?) {
            if (this.options().length) {
                var index =
                    _.isNumber(option) ? option
                    : this.options.indexOf(option);
            
                this.eachDay(dayFoods => 
                    dayFoods.splice(index, 1));

                this.options.splice(index, 1);
            }
        };

        addPlace(text?: string) {
            text = this.generateText("Lugar ", this.places, text);
            var place = { text: ko.observable(text) };
        
            //No hay generics, de manera que this.options acepta cualquier cosa
            this.places.push(place);
        }
        
        removePlace(place?) {
            if (this.places().length) {
                var index =
                    _.isNumber(place) ? place
                    : this.places.indexOf(place);
            
                this.places.splice(index, 1);
            }
        };
    
        /*
        private addDay(text?: string) {
            text = this.generateText("Día ", this.days, text);
            var day = { text: ko.observable(text) };       
        
            this.eachWeek(weekFoods => {
                var dayFoods = [];            
                _.each(this.options(), option => 
                    dayFoods.push(ko.observable("")));
                weekFoods.push(dayFoods);
            });
        
            //No hay generics, de manera que this.days acepta cualquier cosa
            this.days.push(day);
        }
        */
    }
}