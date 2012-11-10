var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
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
            var model = eval("(" + this.$json.val() + ")");
            this.reset(model);
        };
        AdminController.prototype.save = function () {
            var model = this.exportModel();
            this.$json.val(JSON.stringify(model));
        };
        return AdminController;
    })(CommonFood.MenuDefinition);
    CommonFood.AdminController = AdminController;    
    $(document).ready(function () {
        var adminController = new AdminController();
        $("#pruebadt").datepicker();
    });
})(CommonFood || (CommonFood = {}));

