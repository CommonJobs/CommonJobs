///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='CommonFood.ts' />

module CommonFood {
    export class AdminController {
        json = ko.observable("");
        menuViewModel = new CommonFood.MenuViewModel();

        constructor (adminElement: HTMLElement, menuElement?: HTMLElement) {
            
            //#region Quick patch, please rewrite it
            this.json($(adminElement).find(".json-field").text());
            //#endregion
            
            ko.applyBindings(this.menuViewModel, menuElement);
            ko.applyBindings(this, adminElement);
        }

        loadFromJSON() {
            //TODO: change it by a jsonbinding
            var model = eval("(" + this.json() + ")");
            //support comments or not? var model = JSON.parse(this.json());

            this.menuViewModel.reset(model);
        }

        saveToJSON() {
            var model = this.menuViewModel.exportModel();
            //TODO: change it by a jsonbinding
            this.json(JSON.stringify(model));
        }
    }
}

$(document).ready(() => {
    $.fn.datepicker.defaults = {
        autoclose: true,
        language: 'es',
        format: 'dd/mm/yyyy'
    };

    var adminController = new CommonFood.AdminController($(".adminController")["0"]);  
});
