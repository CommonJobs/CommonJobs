///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path="moment.d.ts" />

module CommonFood {
    export interface IMenuModel {
        title?: string;
        firstWeek?: number;
        firstDay?: number;
        weeks?: number;
        days?: string[];
        options?: string[];
        places?: string[];
        startDate?: string;
        endDate?: string;
        foods?: string[][][];
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
        static defaultModel: IMenuModel = {
            title: "Nuevo Menú",
            firstWeek: 0,
            firstDay: 0,
            weeks: 4,
            days: [ "Lunes", "Martes", "Miercoles", "Jueves", "Viernes" ],
            options: [ "Común", "Light", "Vegetariano" ],
            places: [ "La Rioja", "Garay" ],
            startDate: "",
            endDate: "",
            foods: []
        };

        title: knockout.koObservableString = ko.observable("");
        weeks: knockout.koObservableNumber = ko.observable(0);
        days: knockout.koObservableArrayBase = ko.observableArray();
        options: knockout.koObservableArrayBase = ko.observableArray();
        places: knockout.koObservableArrayBase = ko.observableArray();
        startDate: knockout.koObservableAny = ko.observable("");
        endDate: knockout.koObservableAny = ko.observable("");
        firstWeek: knockout.koObservableNumber = ko.observable(0);
        firstDay: knockout.koObservableNumber = ko.observable(0);
        foods: knockout.koObservableArrayBase = ko.observableArray(); //By week, by day, by option
    
        constructor (model?: IMenuModel) {
            super();
            this.reset(model);
        }

        reset(model?: IMenuModel) {
            model =  <IMenuModel>$.extend({}, MenuViewModel.defaultModel, model);
            this.title(model.title);
            this.weeks(0);
            this.days([]);
            this.options([]);
            this.places([]);
            this.startDate(model.startDate);
            this.endDate(model.endDate);
            this.firstWeek(model.firstWeek);
            this.firstDay(model.firstDay);
            this.foods([]); //By week, by day, by option
        
            for (var s in model.options) {
                this.addOption(model.options[s]);
            }        
            for (var s in model.places) {
                this.addPlace(model.places[s]);
            }        
            for (var s in model.days) {
                this.addDay(model.days[s]);
            }
            for (var i = 0; i < model.weeks; i++) {
                this.addWeek();
            }

            //TODO: importar las comidas
        }

        exportModel(): IMenuModel {
            //TODO: generar el modelo
            return "//TODO";
        }

        getFood(weekIndex: number, dayIndex: number, optionIndex: number): knockout.koObservableString {
            return this.foods()[weekIndex][dayIndex][optionIndex];
        }

        addWeek() {
            var weekFoods: knockout.koObservableString[][] = [];
            _.each(this.days(), day => {
                var dayFoods: knockout.koObservableString[] = [];
                _.each(this.options(), option => 
                    dayFoods.push(ko.observable("")));
                weekFoods.push(dayFoods);
            });
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
    
        private eachWeek(f: (weekFoods: knockout.koObservableString[][]) => void ) {
            _.each(this.foods(), f);
        }

        private eachDay(f: (dayFoods: knockout.koObservableString[], weekFoods: knockout.koObservableString[][]) => void ) {
            this.eachWeek(weekFoods => 
                _.each(weekFoods, dayFoods => 
                    f(dayFoods, weekFoods)));
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
    }
}