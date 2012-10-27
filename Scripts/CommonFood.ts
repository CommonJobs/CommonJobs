///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='underscore.browser.d.ts' />

module CommonFood {
    export interface IMenuModel {
        title?: string;
        firstWeek?: number;
        firstDay?: number;
        weeks?: number;
        days?: string[];
        options?: string[];
        startDate?: Date;
        endDate?: Date;
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
            startDate: new Date(),
            endDate: new Date(),
            foods: []
        };

        title: knockout.koObservableString;
        weeks: knockout.koObservableNumber;
        days: knockout.koObservableArrayBase;
        options: knockout.koObservableArrayBase;
        startDate: knockout.koObservableString;
        endDate: knockout.koObservableString;
        firstWeek: knockout.koObservableNumber;
        firstDay: knockout.koObservableNumber;
        foods: knockout.koObservableArrayBase; //By week, by day, by option
    
        constructor (model?: IMenuModel) {
            super();
            this.reset(model);
        }

        reset(model?: IMenuModel) {
            model =  <IMenuModel>$.extend({}, MenuViewModel.defaultModel, model);
            this.title = ko.observable(model.title);
            this.weeks = ko.observable(0);
            this.days = ko.observableArray([]);
            this.options = ko.observableArray();
            this.startDate = ko.observable(model.startDate);
            this.endDate = ko.observable(model.endDate);
            this.firstWeek = ko.observable(model.firstWeek);
            this.firstDay = ko.observable(model.firstDay);
            this.foods = ko.observableArray(); //By week, by day, by option
        
            for (var s in model.options) {
                this.addOption(model.options[s]);
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

        addOption(text?: string) {
            text = _.isString(text) && text || "Menú " + (this.options().length + 1);
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
        
    
        private addDay(text?: string) {
            text = _.isString(text) && text || "Día " + (this.options().length + 1);
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