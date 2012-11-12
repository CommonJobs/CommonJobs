///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='moment-datepicker.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='CommonFood.ts' />

module CommonFood {
    export class AdminController extends MenuDefinition {
        $json = $(".persistence .json-field");
        
        constructor () {
            super();
            ko.applyBindings(this);
        }

        load() {
            //TODO: reset from server
            var data = eval("(" + this.$json.val() + ")");
            //support comments or not? var model = JSON.parse(this.$json.text());

            this.reset(data);
        }

        save() {
            var data = this.exportData();
            
            //TODO: save to server
            this.$json.val(JSON.stringify(data));
        }
    }

    $(document).ready(() => {
        var adminController = new AdminController();    
        $("#pruebadt").datepicker();
    });
}


