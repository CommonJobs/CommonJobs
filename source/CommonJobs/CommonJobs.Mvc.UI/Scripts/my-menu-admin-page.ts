///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='moment-datepicker.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='my-menu.ts' />

declare var urlGenerator: any;
declare var ViewData: any;

module MyMenu {
    export class AdminPage extends MenuDefinition {
		onAjaxCall: knockout.koObservableBool = ko.observable(false);
        constructor () {
            super();
            ko.applyBindings(this);
        }

        load() {
			this.onAjaxCall(true);
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
                    },
					complete: () => this.onAjaxCall(false)
                });
        }

        save() {
			this.onAjaxCall(true);
            var data = this.exportData();
            $.ajax(
                urlGenerator.action("MenuDefinition", "MyMenu"),
                {
                    type: "POST",
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(data),
                    error: (jqXHR) => {
                        alert("Error saving MenuDefinition");
                        $("html").html(jqXHR.responseText);
                    },
					success: () => this.isDirty.reset(),
					complete: () => this.onAjaxCall(false)
                });
        }
    }

    $(document).ready(() => {
		var adminController = new AdminPage();    
		adminController.load()
    });
}


