var CommonFood;
(function (CommonFood) {
    (function (MyMenu) {
        var $menuJson;
        var $employeeMenuJson;
        var employeeMenu = null;
        MyMenu.load = function () {
            var menuData = eval("(" + $menuJson.val() + ")");
            var menuDefinition = new CommonFood.MenuDefinition(menuData);
            var employeeMenuData = eval("(" + $employeeMenuJson.val() + ")");
            employeeMenu = new CommonFood.EmployeeMenuDefinition(menuDefinition, employeeMenuData);
            ko.applyBindings(employeeMenu);
        };
        MyMenu.save = function () {
            var data = employeeMenu.exportData();
            $employeeMenuJson.val(JSON.stringify(data));
        };
        $(document).ready(function () {
            $menuJson = $(".persistence .json-field.menu");
            $employeeMenuJson = $(".persistence .json-field.employee-menu");
            MyMenu.load();
        });
    })(CommonFood.MyMenu || (CommonFood.MyMenu = {}));
    var MyMenu = CommonFood.MyMenu;

})(CommonFood || (CommonFood = {}));

