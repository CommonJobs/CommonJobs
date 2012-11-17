var MyMenu;
(function (MyMenu) {
    (function (MyMenuPage) {
        var $menuJson;
        var $employeeMenuJson;
        var menuDefinition = new MyMenu.MenuDefinition();
        var employeeMenu = new MyMenu.EmployeeMenuDefinition(menuDefinition);
        MyMenuPage.load = function () {
            var menuData = eval("(" + $menuJson.val() + ")");
            menuDefinition.reset(menuData);
            var employeeMenuData = eval("(" + $employeeMenuJson.val() + ")");
            employeeMenu.reset(employeeMenuData);
        };
        MyMenuPage.save = function () {
            var data = employeeMenu.exportData();
            $employeeMenuJson.val(JSON.stringify(data));
        };
        $(document).ready(function () {
            $menuJson = $(".persistence .json-field.menu");
            $employeeMenuJson = $(".persistence .json-field.employee-menu");
            ko.applyBindings(employeeMenu);
        });
    })(MyMenu.MyMenuPage || (MyMenu.MyMenuPage = {}));
    var MyMenuPage = MyMenu.MyMenuPage;

})(MyMenu || (MyMenu = {}));

