///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='moment-datepicker.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='my-menu.ts' />

module MyMenu.MyMenuPage {
    var $menuJson: JQuery; 
    var $employeeMenuJson: JQuery; 
    var menuDefinition = new MenuDefinition();
    var employeeMenu: EmployeeMenuDefinition = new EmployeeMenuDefinition(menuDefinition);

    export var load = function () {
        //alert("load");
        //TODO: reset from server
        var menuData = eval("(" + $menuJson.val() + ")");
        menuDefinition.reset(menuData);

        var employeeMenuData = eval("(" + $employeeMenuJson.val() + ")");
        employeeMenu.reset(employeeMenuData);
    }

    export var save = function () {
        //alert("save");
        var data = employeeMenu.exportData();
        //TODO: save to server
        $employeeMenuJson.val(JSON.stringify(data))
    }

    $(document).ready(() => {
        $menuJson = $(".persistence .json-field.menu");
        $employeeMenuJson = $(".persistence .json-field.employee-menu");
        ko.applyBindings(employeeMenu);
    });
}


