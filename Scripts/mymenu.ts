///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='moment-datepicker.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='CommonFood.ts' />

module CommonFood.MyMenu {
    var $menuJson = $(".persistence .json-field.menu");
    var $employeeMenuJson = $(".persistence .json-field.employee-menu");
    var employeeMenu: EmployeeMenuDefinition = null;

    export var load = function () {
        //alert("load");
        //TODO: reset from server
        var menuData = eval("(" + $menuJson.val() + ")");
        var menuDefinition = new MenuDefinition(menuData);

        var employeeMenuData = eval("(" + $employeeMenuJson.val() + ")");
        employeeMenu = new EmployeeMenuDefinition(menuDefinition, employeeMenuData);
            
        ko.applyBindings(employeeMenu);
    }

    export var save = function () {
        //alert("save");
        var data = employeeMenu.exportData();
        //TODO: save to server
        $employeeMenuJson.val(JSON.stringify(data))
    }

    $(document).ready(() => {
        load();
    });
}


