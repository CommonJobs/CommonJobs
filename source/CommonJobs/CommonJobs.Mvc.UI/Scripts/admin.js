var CommonFood;
(function (CommonFood) {
    var AdminController = (function (_super) {
        __extends(AdminController, _super);
        function AdminController() {
            _super.prototype();
            this.$json = $(".persistence .json-field");
            ko.applyBindings(this);
        }
        AdminController.prototype.load = function () {
            var data = eval("(" + this.$json.val() + ")");
            this.reset(data);
        };
        AdminController.prototype.save = function () {
            var data = this.exportData();
            this.$json.val(JSON.stringify(data));
        };
        return AdminController;
    })(MenuDefinition);
    CommonFood.AdminController = AdminController;    
    $(document).ready(function () {
        var adminController = new AdminController();
        $("#pruebadt").datepicker();
    });
})(CommonFood || (CommonFood = {}));

