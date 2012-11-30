var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
var MyMenu;
(function (MyMenu) {
    $(document).ready(function () {
        var orderPage = new OrderPage(window["ViewData"]);
        orderPage.bind();
    });
    var OrderPage = (function (_super) {
        __extends(OrderPage, _super);
        function OrderPage(viewData) {
                _super.call(this);
            this.orderDate = moment();
            this.isOrdered = false;
            var order = viewData.order;
            this.isOrdered = order.IsOrdered;
            this.orderDate = moment(order.Date);
            this.placeSummaries = _.sortBy(_.map(order.QuantityByOptionByPlace, function (placeQuantityByOption, placeKey) {
                return ({
                    placeKey: placeKey,
                    placeName: order.PlacesByKey[placeKey],
                    optionSummaries: _.map(placeQuantityByOption, function (quantity, optionKey) {
                        return ({
                            optionKey: optionKey,
                            optionName: order.OptionsByKey[optionKey],
                            quantity: quantity
                        });
                    })
                });
            }), "placeKey");
            this.detail = _.sortBy(_.sortBy(_.map(order.DetailByUserName, function (element, key) {
                return ({
                    userName: key,
                    url: viewData.baseLink + key,
                    employeeName: element.EmployeeName,
                    placekey: element.PlaceKey,
                    placeName: element.PlaceKey && order.PlacesByKey[element.PlaceKey] || " - ",
                    optionKey: element.OptionKey,
                    optionName: element.OptionKey && order.OptionsByKey[element.OptionKey] || " - ",
                    food: element.PlaceKey && element.OptionKey && order.FoodsByOption[element.OptionKey] || "No come aquí",
                    comment: element.Comment || " - "
                });
            }), "employeeName"), "placeName");
        }
        OrderPage.prototype.bind = function () {
            ko.applyBindings(this);
        };
        OrderPage.prototype.refresh = function () {
            location.reload();
        };
        return OrderPage;
    })(Utilities.HasCallbacks);
    MyMenu.OrderPage = OrderPage;    
})(MyMenu || (MyMenu = {}));

