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
        constructor () {
            super(new MenuDefinition(), null, ViewData.now);
            ko.applyBindings(this);
        }

        load() {
            $.ajax(
            ViewData.menuUrl,
            {
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: (employeeMenuDTO) => {
                    this.menu.reset(employeeMenuDTO.MenuDefinition);
                    this.reset(employeeMenuDTO.EmployeeMenu);
                },
                error: (jqXHR) => {
                    alert("Error getting EmployeeMenu");
                    $("html").html(jqXHR.responseText);
                }
            });
        }

        save() {
            var data = this.exportData();
            $.ajax(
                ViewData.menuUrl,
                {
                    type: "POST",
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(data),
                    success: this.load,
                    error: (jqXHR) => {
                        alert("Error saving EmployeeMenu");
                        $("html").html(jqXHR.responseText);
                    }
                });
        }
    }
}
