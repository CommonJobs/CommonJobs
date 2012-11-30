///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='moment-datepicker.d.ts' />
///<reference path='underscore.browser.d.ts' />
///<reference path='my-menu.ts' />

declare var urlGenerator: any;
declare var ViewData: any;

module MyMenu {

    $(document).ready(() => {
        var orderPage = new OrderPage(window["ViewData"]);
        orderPage.bind()
    });

    export class OrderPage extends Utilities.HasCallbacks {
        orderDate: moment.Moment = moment();
        isOrdered: bool = false;

        //TODO: create interfaces
        placeSummaries: { placeKey: string; placeName: string; optionSummaries: { optionKey: string; optionName: string; quantity: number; }[]; }[];
        detail: { userName: string; url: string; employeeName: string; placekey: string; placeName: string; optionKey: string; optionName: string; food: string; comment: string; }[];

        constructor (viewData) {
            super();
            //TODO: create interface
            var order = viewData.order;
            this.isOrdered = order.IsOrdered;
            this.orderDate = moment(order.Date);
            this.placeSummaries = _.sortBy(_.map(order.QuantityByOptionByPlace, (placeQuantityByOption: any, placeKey: string) => ({ 
                placeKey: placeKey, 
                placeName: order.PlacesByKey[placeKey], 
                optionSummaries: _.map(placeQuantityByOption, (quantity: number, optionKey: string) => ({
                    optionKey: optionKey, 
                    optionName: order.OptionsByKey[optionKey], 
                    quantity: quantity
                }))
            })), "placeKey");

            this.detail = _.sortBy(_.sortBy(_.map(order.DetailByUserName, (element: any, key: string) => ({
                userName: key,
                url: viewData.baseLink + key,
                employeeName: element.EmployeeName,
                placekey: element.PlaceKey,
                placeName: element.PlaceKey && order.PlacesByKey[element.PlaceKey] || " - ",
                optionKey: element.OptionKey,
                optionName: element.OptionKey && order.OptionsByKey[element.OptionKey] || " - ",
                food: element.PlaceKey && element.OptionKey && order.FoodsByOption[element.OptionKey] || "No come aquí",
                comment: element.Comment || " - "
            })), "employeeName"), "placeName");
        }

        bind() {
            ko.applyBindings(this);
        }

        refresh() {
            location.reload();
        }
    }
}