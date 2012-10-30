var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='CommonFood.ts' />
var CommonFood;
(function (CommonFood) {
    var AdminController = (function (_super) {
        __extends(AdminController, _super);
        function AdminController() {
                _super.call(this);
            this.$json = $(".persistence .json-field");
            ko.applyBindings(this);
        }
        AdminController.prototype.load = function () {
            //TODO: reset from server
            var model = eval("(" + this.$json.val() + ")");
            //support comments or not? var model = JSON.parse(this.$json.text());
            this.reset(model);
        };
        AdminController.prototype.save = function () {
            var model = this.exportModel();
            //TODO: save to server
            this.$json.val(JSON.stringify(model));
        };
        return AdminController;
    })(CommonFood.MenuViewModel);
    CommonFood.AdminController = AdminController;    
    $(document).ready(function () {
        var adminController = new AdminController();
        ($("#pruebadt")).datepicker();
    });
})(CommonFood || (CommonFood = {}));

