var MyMenu;
(function (MyMenu) {
    (function (MyMenuPage) {
        var $menuJson;
        var $employeeMenuJson;
        var menuDefinition = new MyMenu.MenuDefinition();
        var employeeMenu = new MyMenu.EmployeeMenuDefinition(menuDefinition);
        MyMenuPage.load = function () {
            $.ajax(urlGenerator.action("MenuDefinition", "MyMenu"), {
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (menuData) {
                    $.ajax(ViewData.menuUrl, {
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        success: function (employeeMenuData) {
                            employeeMenu.reset(employeeMenuData);
                        }
                    });
                    menuDefinition.reset(menuData);
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
                success: function (employeeMenuData) {
                    employeeMenu.reset(employeeMenuData);
                }
            });
        };
        $(document).ready(function () {
            $menuJson = $(".persistence .json-field.menu");
            $employeeMenuJson = $(".persistence .json-field.employee-menu");
            ko.applyBindings(employeeMenu);
        });
    })(MyMenu.MyMenuPage || (MyMenu.MyMenuPage = {}));
    var MyMenuPage = MyMenu.MyMenuPage;

})(MyMenu || (MyMenu = {}));

