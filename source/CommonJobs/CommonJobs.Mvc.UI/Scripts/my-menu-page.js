var MyMenu;
(function (MyMenu) {
    (function (MyMenuPage) {
        var $menuJson;
        var $employeeMenuJson;
        var menuDefinition = new MyMenu.MenuDefinition();
        var employeeMenu = new MyMenu.EmployeeMenuDefinition(menuDefinition, null, ViewData.now);
        MyMenuPage.load = function () {
            $.ajax(ViewData.menuUrl, {
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (employeeMenuData) {
                    $.ajax(urlGenerator.action("MenuDefinition", "MyMenu", employeeMenuData.menuId), {
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        success: function (menuData) {
                            menuDefinition.reset(menuData);
                            employeeMenu.reset(employeeMenuData);
                        }
                    });
                }
            });
        };
        MyMenuPage.save = function () {
            var data = employeeMenu.exportData();
            $.ajax(ViewData.menuUrl, {
                type: "POST",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data),
                success: MyMenuPage.load
            });
        };
        $(document).ready(function () {
            $menuJson = $(".persistence .json-field.menu");
            $employeeMenuJson = $(".persistence .json-field.employee-menu");
            ko.applyBindings(employeeMenu);
            MyMenuPage.load();
        });
    })(MyMenu.MyMenuPage || (MyMenu.MyMenuPage = {}));
    var MyMenuPage = MyMenu.MyMenuPage;

})(MyMenu || (MyMenu = {}));

