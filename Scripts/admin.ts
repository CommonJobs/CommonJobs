///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='cj-datepicker.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='CommonFood.ts' />

module CommonFood {
    export class AdminController extends MenuViewModel {
        $json = $(".persistence .json-field");
        
        constructor () {
            super();
            ko.applyBindings(this);
        }

        load() {
            //TODO: reset from server
            var model = eval("(" + this.$json.val() + ")");
            //support comments or not? var model = JSON.parse(this.$json.text());

            this.reset(model);
        }

        save() {
            var model = this.exportModel();
            
            //TODO: save to server
            this.$json.val(JSON.stringify(model));
        }
    }

    $(document).ready(() => {
        var adminController = new AdminController();    
        $("#pruebadt").cjdatepicker();
    });
}


