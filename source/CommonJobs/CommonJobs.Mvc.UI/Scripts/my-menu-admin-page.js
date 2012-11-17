var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
var MyMenu;
(function (MyMenu) {
    var AdminPage = (function (_super) {
        __extends(AdminPage, _super);
        function AdminPage() {
                _super.call(this);
            this.$json = $(".persistence .json-field");
            ko.applyBindings(this);
        }
        AdminPage.prototype.load = function () {
            var data = eval("(" + this.$json.val() + ")");
            this.reset(data);
        };
        AdminPage.prototype.save = function () {
            var data = this.exportData();
            this.$json.val(JSON.stringify(data));
        };
        return AdminPage;
    })(MyMenu.MenuDefinition);
    MyMenu.AdminPage = AdminPage;    
    $(document).ready(function () {
        var adminController = new AdminPage();
        $("#pruebadt").datepicker();
    });
})(MyMenu || (MyMenu = {}));

