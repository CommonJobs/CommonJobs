var CommonFood;
(function (CommonFood) {
    (function (MyMenu) {
        var $menuJson;
        var $employeeMenuJson;
        var menuDefinition = new CommonFood.MenuDefinition();
        var employeeMenu = new CommonFood.EmployeeMenuDefinition(menuDefinition);
        MyMenu.load = function () {
            var menuData = eval("(" + $menuJson.val() + ")");
            menuDefinition.reset(menuData);
            var employeeMenuData = eval("(" + $employeeMenuJson.val() + ")");
            employeeMenu.reset(employeeMenuData);
        };
        MyMenu.save = function () {
            var data = employeeMenu.exportData();
            $employeeMenuJson.val(JSON.stringify(data));
        };
        $(document).ready(function () {
            $menuJson = $(".persistence .json-field.menu");
            $employeeMenuJson = $(".persistence .json-field.employee-menu");
            ko.applyBindings(employeeMenu);
        });
    })(CommonFood.MyMenu || (CommonFood.MyMenu = {}));
    var MyMenu = CommonFood.MyMenu;

})(CommonFood || (CommonFood = {}));

