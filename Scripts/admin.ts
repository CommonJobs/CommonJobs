///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='underscore.browser.d.ts' />

interface IMenuModel {
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

class MenuViewModel {
    static defaultModel: IMenuModel = {
        title: "",
        firstWeek: 0,
        firstDay: 0,
        weeks: 4,
        days: [ "Lunes", "Martes", "Miercoles", "Jueves", "Viernes" ],
        options: [ "Común", "Light", "Vegetariano" ],
        startDate: null,
        endDate: null,
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
        return null;
    }

    getFood(weekIndex: number, dayIndex: number, optionIndex: number): knockout.koObservableString {
        return this.foods()[weekIndex][dayIndex][optionIndex];
    }

    addWeek() {
        var weekFoods: knockout.koObservableString[][] = [];
        _.each(this.days(), function(day) {
            var dayFoods: knockout.koObservableString[] = [];
            _.each(this.options(), function(option) {
               dayFoods.push(ko.observable(""));
            }, this);
            weekFoods.push(dayFoods);
        }, this);
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
        _.each(this.foods(), f, this);
    }

    private eachDay(f: (dayFoods: knockout.koObservableString[], weekFoods: knockout.koObservableString[][]) => void ) {
        this.eachWeek(function(weekFoods) {
            _.each(weekFoods, function(dayFoods) {
                f(dayFoods, weekFoods);
            }, this);
       });
    };

    addOption(text?: string) {
        text = _.isString(text) && text || "Menú " + (this.options().length + 1);
        var option = { text: ko.observable(text) };
        
        this.eachDay(function(dayFoods) {
           dayFoods.push(ko.observable(""));
        });
        
        this.options.push(option);
    }

    
    removeOption(option?) {
        if (this.options().length) {
            var index =
                _.isNumber(option) ? option
                : this.options.indexOf(option);
            
            this.eachDay(function(dayFoods) {
               dayFoods.splice(index, 1);
            });

            this.options.splice(index, 1);
        }
    };
        
    
    private addDay(text?: string) {
        text = _.isString(text) && text || "Día " + (this.options().length + 1);
        var day = { text: ko.observable(text) };       
        
        this.eachWeek(function(weekFoods) {
            var dayFoods = [];            
            _.each(this.options(), function(option) {
               dayFoods.push(ko.observable(""));
            }, this);
            weekFoods.push(dayFoods);
        });
        
        this.days.push(day);
    }
}

$(document).ready(function () {
    ko.applyBindings(new MenuViewModel({
        title: "Menú Primaveral"
        , firstWeek: 1 //Empezamos por la segunda semana
        , firstDay: 4 //El 21 de septiembre es viernes
        , weeks: 4
        , options: ["Común", "Light", "Vegetariano"]
        , startDate: new Date("2012-09-21") //inclusive
        , endDate: new Date("2012-12-20") //inclusive
        //, foods: []
    }));
});
