///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='CommonFood.ts' />
var CommonFood;
(function (CommonFood) {
    var AdminController = (function () {
        function AdminController(adminElement, menuElement) {
            this.json = ko.observable("");
            this.menuViewModel = new CommonFood.MenuViewModel();
            //#region Quick patch, please rewrite it
            this.json($(adminElement).find(".json-field").text());
            //#endregion
            ko.applyBindings(this.menuViewModel, menuElement);
            ko.applyBindings(this, adminElement);
        }
        AdminController.prototype.loadFromJSON = function () {
            //TODO: change it by a jsonbinding
            var model = eval("(" + this.json() + ")");
            //support comments or not? var model = JSON.parse(this.json());
            this.menuViewModel.reset(model);
        };
        AdminController.prototype.saveToJSON = function () {
            var model = this.menuViewModel.exportModel();
            //TODO: change it by a jsonbinding
            this.json(JSON.stringify(model));
        };
        return AdminController;
    })();
    CommonFood.AdminController = AdminController;    
})(CommonFood || (CommonFood = {}));

$(document).ready(function () {
    $.fn.datepicker.defaults = {
        autoclose: true,
        language: 'es',
        format: 'dd/mm/yyyy'
    };
    var adminController = new CommonFood.AdminController($(".adminController")["0"]);
});
