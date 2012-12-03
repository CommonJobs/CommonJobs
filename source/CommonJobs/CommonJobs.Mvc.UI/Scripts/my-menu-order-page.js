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
    ; ;
    ; ;
    var OrderPage = (function (_super) {
        __extends(OrderPage, _super);
        function OrderPage(viewData) {
                _super.call(this);
            this.orderDate = moment();
            this.isOrdered = false;
            this.isProcessButtonVisible = ko.observable(false);
            var order = viewData.order;
            this.isOrdered = order.IsOrdered;
            this.orderDate = moment(order.Date);
            this.placeSummaries = _.sortBy(_.map(order.PlacesByKey, function (placeName, placeKey) {
                return ({
                    placeKey: placeKey,
                    placeName: placeName,
                    optionSummaries: _.map(order.OptionsByKey, function (optionName, optionKey) {
                        return ({
                            optionKey: optionKey,
                            optionName: optionName,
                            food: order.FoodsByOption[optionKey],
                            quantity: order.QuantityByOptionByPlace[placeKey] && order.QuantityByOptionByPlace[placeKey][optionKey] || 0
                        });
                    })
                });
            }), "placeKey");
            this.detail = _.sortBy(_.map(order.DetailByUserName, function (element, key) {
                return ({
                    userName: key,
                    url: viewData.baseLink + key,
                    employeeName: element.EmployeeName,
                    placeKey: element.PlaceKey,
                    placeName: element.PlaceKey && order.PlacesByKey[element.PlaceKey] || " - ",
                    optionKey: element.OptionKey,
                    optionName: element.OptionKey && order.OptionsByKey[element.OptionKey] || " - ",
                    food: element.PlaceKey && element.OptionKey && order.FoodsByOption[element.OptionKey] || "No come aquí",
                    comment: element.Comment || " - "
                });
            }), "employeeName");
        }
        OrderPage.prototype.toggleProcessButton = function () {
            this.isProcessButtonVisible(!this.isProcessButtonVisible());
        };
        OrderPage.prototype.bind = function () {
            ko.applyBindings(this);
            var tables = $('table.table-detail');
            tables.dataTable({
                bPaginate: false,
                sDom: 'T<"clear">lfrtip',
                bInfo: false,
                oTableTools: {
                    aButtons: [
                        {
                            sExtends: "copy",
                            sButtonText: "Copiar"
                        }, 
                        {
                            sExtends: "print",
                            sButtonText: "Imprimir"
                        }, 
                        {
                            sExtends: "collection",
                            sButtonText: "Exportar",
                            aButtons: [
                                "csv", 
                                "xls", 
                                "pdf"
                            ]
                        }
                    ]
                }
            });
        };
        OrderPage.prototype.refresh = function () {
            location.reload();
        };
        return OrderPage;
    })(Utilities.HasCallbacks);
    MyMenu.OrderPage = OrderPage;    
})(MyMenu || (MyMenu = {}));

