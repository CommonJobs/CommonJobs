///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='CommonFood.ts' />

$(document).ready(() => {
    $.fn.datepicker.defaults = {
        autoclose: true,
        language: 'es',
        format: 'dd/mm/yyyy'
    };

    ko.applyBindings(new CommonFood.MenuViewModel({
        title: "Menú Primaveral"
        , firstWeek: 1 //Empezamos por la segunda semana
        , firstDay: 4 //El 21 de septiembre es viernes
        , weeks: 4
        , options: ["Común", "Light", "Vegetariano"]
        , startDate: new Date("2012-09-21") //inclusive
        , endDate: new Date("2012-12-20") //inclusive
        //, foods: []
    }))
});
