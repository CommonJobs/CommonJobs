///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='moment-datepicker.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='my-menu.ts' />

declare var urlGenerator: any;
declare var ViewData: any;

module MyMenu {
    export class AdminPage extends MenuDefinition {
        $json = $(".persistence .json-field");
        
        constructor () {
            super();
            ko.applyBindings(this);
        }

        load() {
            $.ajax(
                urlGenerator.action("MenuDefinition", "MyMenu", ViewData.menuId),
                {
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: (data) => {
                        this.reset(data);
                    }
                });
        }

        save() {
            var data = this.exportData();
            $.ajax(
                urlGenerator.action("MenuDefinition", "MyMenu"),
                {
                    type: "POST",
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(data),
                    success: (data) => {
                        this.load();
                    }
                });
        }
    }

    $(document).ready(() => {
        var adminController = new AdminPage();    
        adminController.load()
    });
}


