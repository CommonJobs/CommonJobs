///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='moment-datepicker.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='my-menu.ts' />

declare var urlGenerator: any;
declare var ViewData: any;

module MyMenu.MyMenuPage {
    var $menuJson: JQuery; 
    var $employeeMenuJson: JQuery; 
    var menuDefinition = new MenuDefinition();
    var employeeMenu: EmployeeMenuDefinition = new EmployeeMenuDefinition(menuDefinition);

    export var load = function () {
        $.ajax(
            urlGenerator.action("MenuDefinition", "MyMenu"),
            {
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: (menuData) => {
                    $.ajax(
                        ViewData.menuUrl,
                        {
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8',
                            success: (employeeMenuData) => {
                                employeeMenu.reset(employeeMenuData);
                            }
                        });
                    menuDefinition.reset(menuData);
                }
            });
    }

    export var save = function () {
        var data = employeeMenu.exportData();
        $.ajax(
            ViewData.menuUrl,
            {
                type: "POST",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data),
                success: (employeeMenuData) => {
                    employeeMenu.reset(employeeMenuData);
                }
            });
    }

    $(document).ready(() => {
        $menuJson = $(".persistence .json-field.menu");
        $employeeMenuJson = $(".persistence .json-field.employee-menu");
        ko.applyBindings(employeeMenu);
    });
}


