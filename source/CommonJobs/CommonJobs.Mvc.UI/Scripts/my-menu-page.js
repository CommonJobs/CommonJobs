var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
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
            this.LastRequest = ko.observable();
            ko.applyBindings(this);
        }
        MyMenuPage.prototype.todayRequest = function () {
            var lastRequest = this.LastRequest();
            var now = this.now();
            return lastRequest && Utilities.daysDiff(now, lastRequest.Date) === 0 ? lastRequest : null;
        };
        MyMenuPage.prototype.previousRequest = function () {
            var lastRequest = this.LastRequest();
            var now = this.now();
            return lastRequest && Utilities.daysDiff(now, lastRequest.Date) > 0 ? lastRequest : null;
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
                    _this.LastRequest(employeeMenuDTO.LastRequest);
                },
                error: function (jqXHR) {
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

