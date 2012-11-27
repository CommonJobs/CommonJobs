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
            this.onAjaxCall = ko.observable(false);
            ko.applyBindings(this);
        }
        AdminPage.prototype.load = function () {
            var _this = this;
            this.onAjaxCall(true);
            $.ajax(urlGenerator.action("MenuDefinition", "MyMenu", ViewData.menuId), {
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    _this.reset(data);
                },
                error: function (jqXHR) {
                    alert("Error getting MenuDefinition");
                    $("html").html(jqXHR.responseText);
                },
                complete: function () {
                    return _this.onAjaxCall(false);
                }
            });
        };
        AdminPage.prototype.save = function () {
            var _this = this;
            this.onAjaxCall(true);
            var data = this.exportData();
            $.ajax(urlGenerator.action("MenuDefinition", "MyMenu"), {
                type: "POST",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data),
                error: function (jqXHR) {
                    alert("Error saving MenuDefinition");
                    $("html").html(jqXHR.responseText);
                },
                success: function () {
                    return _this.isDirty.reset();
                },
                complete: function () {
                    return _this.onAjaxCall(false);
                }
            });
        };
        return AdminPage;
    })(MyMenu.MenuDefinition);
    MyMenu.AdminPage = AdminPage;    
    $(document).ready(function () {
        var adminController = new AdminPage();
        adminController.load();
    });
})(MyMenu || (MyMenu = {}));

