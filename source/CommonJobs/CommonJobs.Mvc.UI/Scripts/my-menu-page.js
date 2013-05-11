var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var MyMenu;
(function (MyMenu) {
    $(document).ready(function () {
        var myMenuPage = new MyMenuPage();
        myMenuPage.load();
    });
    var MyMenuPage = (function (_super) {
        __extends(MyMenuPage, _super);
        function MyMenuPage() {
                _super.call(this, new MyMenu.MenuDefinition(), null, ViewData.now);
            this.onAjaxCall = ko.observable(false);
            this.LastOrder = ko.observable();
            ko.applyBindings(this);
        }
        MyMenuPage.prototype.todayOrder = function () {
            var lastOrder = this.LastOrder();
            var now = this.now();
            if(lastOrder && Utilities.daysDiff(now, lastOrder.Date) === 0) {
                return lastOrder;
            } else {
                return this.getChoicesByDate(now);
            }
        };
        MyMenuPage.prototype.load = function () {
            var _this = this;
            this.onAjaxCall(true);
            $.ajax(ViewData.menuUrl, {
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (employeeMenuDTO) {
                    _this.menu.reset(employeeMenuDTO.MenuDefinition);
                    _this.reset(employeeMenuDTO.EmployeeMenu);
                    _this.LastOrder(employeeMenuDTO.LastOrder);
                },
                error: function (jqXHR) {
                    window.location.href = "/MyMenu/Order";
                    alert("Error getting EmployeeMenu");
                    $("html").html(jqXHR.responseText);
                },
                complete: function () {
                    return _this.onAjaxCall(false);
                }
            });
        };
        MyMenuPage.prototype.save = function () {
            var _this = this;
            this.onAjaxCall(true);
            var data = this.exportData();
            $.ajax(ViewData.menuUrl, {
                type: "POST",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data),
                error: function (jqXHR) {
                    alert("Error saving EmployeeMenu");
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
        return MyMenuPage;
    })(MyMenu.EmployeeMenuDefinition);
    MyMenu.MyMenuPage = MyMenuPage;    
})(MyMenu || (MyMenu = {}));
