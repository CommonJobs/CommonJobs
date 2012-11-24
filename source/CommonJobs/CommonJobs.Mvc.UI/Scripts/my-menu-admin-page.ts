///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='moment-datepicker.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='my-menu.ts' />

declare var urlGenerator: any;
declare var ViewData: any;

module MyMenu {
    export class AdminPage extends MenuDefinition {
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
                    },
                    error: (jqXHR) => {
                        alert("Error getting MenuDefinition");
                        $("html").html(jqXHR.responseText);
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
                    },
                    error: (jqXHR) => {
                        alert("Error saving MenuDefinition");
                        $("html").html(jqXHR.responseText);
                    }
                });
        }
    }

    $(document).ready(() => {
        var adminController = new AdminPage();    
        adminController.load()
    });
}


