///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='moment-datepicker.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='my-menu.ts' />

declare var urlGenerator: any;
declare var ViewData: any;

module MyMenu {

    $(document).ready(() => {
        var myMenuPage = new MyMenuPage();
        myMenuPage.load()
    });

    export class MyMenuPage extends EmployeeMenuDefinition {
		onAjaxCall: knockout.koObservableBool = ko.observable(false);
		LastOrder: knockout.koObservableAny = ko.observable();
        
        constructor () {
            super(new MenuDefinition(), null, ViewData.now);
            ko.applyBindings(this);
        }

        todayOrder() : EmployeeOrderData {
            var lastOrder = <EmployeeOrderData>this.LastOrder();
            var now = this.now();
            if (lastOrder && Utilities.daysDiff(now, lastOrder.Date) === 0) {
                return lastOrder;
            } else {
                return this.getChoicesByDate(now);
            }
        }

        /*
        TODO, complete it and add to UI

        previousOrder() : EmployeeOrderData {
            var lastOrder = <EmployeeOrderData>this.LastOrder();
            var now = this.now();
            return lastOrder && Utilities.daysDiff(now, lastOrder.Date) > 0 ? lastOrder : null;
        }

        nextOrder() : EmployeeOrderData {
            //TODO: calculate tomorrow (or next monday) order based on menu and current data
        }

        */


        load() {
            this.onAjaxCall(true);
            $.ajax(
            ViewData.menuUrl,
            {
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: (employeeMenuDTO) => {
                    this.menu.reset(employeeMenuDTO.MenuDefinition);
                    this.reset(employeeMenuDTO.EmployeeMenu);
                    this.LastOrder(employeeMenuDTO.LastOrder);
                },
                error: (jqXHR) => {
                    window.location.href = "/MyMenu/Order";
                    alert("Error getting EmployeeMenu");
                    $("html").html(jqXHR.responseText);
                },
				complete: () => this.onAjaxCall(false)
            });
        }

        save() {
            this.onAjaxCall(true);
            var data = this.exportData();
            $.ajax(
                ViewData.menuUrl,
                {
                    type: "POST",
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(data),
                    error: (jqXHR) => {
                        alert("Error saving EmployeeMenu");
                        $("html").html(jqXHR.responseText);
                    },
					success: () => this.isDirty.reset(),
					complete: () => this.onAjaxCall(false)
                });
        }
    }
}
