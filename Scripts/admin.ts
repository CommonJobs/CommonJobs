///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='underscore.browser.d.ts' />

$(document).ready(function () {
var ViewModel = function (model) {
    var self = this;
    var defaultModel = {
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
    
    var initialize = function(model) {
        model = $.extend({}, defaultModel, model);
        self.title = ko.observable(model.title);
        self.weeks = ko.observable(0);
        self.days = ko.observableArray();
        self.options = ko.observableArray();
        self.startDate = ko.observable(model.startDate);
        self.endDate = ko.observable(model.endDate);
        self.firstWeek = ko.observable(model.firstWeek);
        self.firstDay = ko.observable(model.firstDay);
        self.foods = ko.observableArray(); //By week, by day, by option
        
        
        for (var s in model.options) {
            self.addOption(model.options[s]);
        }        
        for (var s in model.days) {
            addDay(model.days[s]);
        }
        for (var i = 0; i < model.weeks; i++) {
            self.addWeek();
        }

        //TODO: importar las comidas
    }
        
    self.exportModel = function() {
        //TODO: generar el modelo
        return null;
    }

    self.getFood = function(weekIndex, dayIndex, optionIndex) {
        return self.foods()[weekIndex][dayIndex][optionIndex];
    }
    
    var createDayFood = function(week, option, day) {
        //var defaultText = option.text() + " " + day.text() + " " + week.text();
        var defaultText = null;
        return {
            day: day,
            text: ko.observable(defaultText)
        };
    };

    self.addWeek = function() {
        var weekFoods = [];                    
        _.each(self.days(), function(day) {
            var dayFoods = [];
            _.each(self.options(), function(option) {
               dayFoods.push(ko.observable(""));
            });
            weekFoods.push(dayFoods);
        });
        self.foods.push(weekFoods);
        self.weeks(self.weeks() + 1);
    };
    
    self.removeWeek = function() {
        var actual = self.weeks();
        if (actual > 0) {
            self.weeks(actual - 1);
            self.foods.pop();
        }            
    };

    var eachWeek = function(f) {
        _.each(self.foods(), f);
    }

    var eachDay = function(f) {
        eachWeek(function(weekFoods) {
            _.each(weekFoods, function(dayFoods) {
                f(dayFoods, weekFoods);
            });
       });
    };
    
    self.addOption = function(text) {
        text = _.isString(text) && text || "Menú " + (self.options().length + 1);
        var option = { text: ko.observable(text) };
        
        eachDay(function(dayFoods) {
           dayFoods.push(ko.observable(""));
        });
        
        self.options.push(option);
    };
    
    self.removeOption = function(option) {
        if (self.options().length) {
            var index =
                _.isNumber(option) ? option
                : self.options.indexOf(option);
            
            eachDay(function(dayFoods) {
               dayFoods.splice(index, 1);
            });

            self.options.splice(index, 1);
        }
    };
        
    var addDay = function(text) {
        text = _.isString(text) && text || "Día " + (self.option().length + 1);
        var day = { text: ko.observable(text) };       
        
        eachWeek(function(weekFoods) {
            var dayFoods = [];            
            _.each(self.options(), function(option) {
               dayFoods.push(ko.observable(""));
            });
            weekFoods.push(dayFoods);
        });
        
        self.days.push(day);
    };
        
    initialize(model);
};

ko.applyBindings(new ViewModel({
    title: "Menú Primaveral"
    , firstWeek: 1 //Empezamos por la segunda semana
    , firstDay: 4 //El 21 de septiembre es viernes
    , weeks: 4 
    , options: [ "Común", "Light", "Vegetariano" ]
    , startDate: "2012-09-21" //inclusive
    , endDate: "2012-12-20" //inclusive
    //TODO: , foods: []
}));

});