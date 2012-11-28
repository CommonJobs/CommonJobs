///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='moment-datepicker.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='my-menu.ts' />

declare var urlGenerator: any;
declare var ViewData: any;

module MyMenu {
    export interface LastRequestData {
        Date: string;
        Option: string;
        Place: string;
        Comment: string;
        Food: string;
        WeekIdx: number;
        DayIdx: number;
    }

    
    $(document).ready(() => {
        var myMenuPage = new MyMenuPage();
        myMenuPage.load()
    });

    export class MyMenuPage extends EmployeeMenuDefinition {
		onAjaxCall: knockout.koObservableBool = ko.observable(false);
		LastRequest: knockout.koObservableAny = ko.observable();
        
        constructor () {
            super(new MenuDefinition(), null, ViewData.now);
            ko.applyBindings(this);
        }

        todayRequest() : LastRequestData {
            var lastRequest = <LastRequestData>this.LastRequest();
            var now = this.now();
            return lastRequest && Utilities.daysDiff(now, lastRequest.Date) === 0 ? lastRequest : null;
        }

        previousRequest() : LastRequestData {
            var lastRequest = <LastRequestData>this.LastRequest();
            var now = this.now();
            return lastRequest && Utilities.daysDiff(now, lastRequest.Date) > 0 ? lastRequest : null;
        }

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
                    this.LastRequest(employeeMenuDTO.LastRequest);
                },
                error: (jqXHR) => {
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
