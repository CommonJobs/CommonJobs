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

    export interface IPlaceSummary {
        placeKey: string; 
        placeName: string; 
        optionSummaries: IOptionSumary[];
    }   

    export interface IOptionSumary {
        optionKey: string;
        optionName: string;
        food: string;
        quantity: number;
    }

    export interface IDetailItemData {
        placeKey: string; 
        placeName: string; 
        optionKey: string;
        optionName: string;
        food: string;
        userName: string;
        url: string;
        employeeName: string;
        comment: string;
    };

    export interface IOrderData {
        Id: string;
        Date: string;
        MenuId: string;
        WeekIdx: number;
        DayIdx: number;
        PlacesByKey: any; //public Dictionary<string, string>
        OptionsByKey: any; //public Dictionary<string, string>
        FoodsByOption: any; //public Dictionary<string, string>
        QuantityByOptionByPlace: any; //public Dictionary<string, Dictionary<string, int>>
        DetailByUserName: any; //public Dictionary<string, MenuOrderDetailItem>
        IsOrdered: bool;
    };

    export class OrderPage extends Utilities.HasCallbacks {
        orderDate: moment.Moment = moment();
        isOrdered: bool = false;
        isProcessButtonVisible: knockout.koObservableBool = ko.observable(false);

        placeSummaries: IPlaceSummary[];
        detail: IDetailItemData[];
        
        toggleProcessButton() {
            this.isProcessButtonVisible(!this.isProcessButtonVisible());
        }

        constructor (viewData) {
            super();
            //TODO: create interface
            var order: IOrderData = viewData.order;
            this.isOrdered = order.IsOrdered;
            this.orderDate = moment(order.Date);

            this.placeSummaries = _.sortBy(
                _.map(order.PlacesByKey, (placeName: string, placeKey: string) => ({
                        placeKey: placeKey,
                        placeName: placeName,
                        optionSummaries: _.map(order.OptionsByKey, (optionName: string, optionKey: string) => ({
                            optionKey: optionKey,
                            optionName: optionName,
                            food: order.FoodsByOption[optionKey],
                            quantity: order.QuantityByOptionByPlace[placeKey] && order.QuantityByOptionByPlace[placeKey][optionKey] || 0
                        }))
                    })
                ), "placeKey");

            this.detail = _.sortBy(_.map(order.DetailByUserName, (element: any, key: string) => ({
                userName: key,
                url: viewData.baseLink + key,
                employeeName: element.EmployeeName,
                placeKey: element.PlaceKey,
                placeName: element.PlaceKey && order.PlacesByKey[element.PlaceKey] || " - ",
                optionKey: element.OptionKey,
                optionName: element.OptionKey && order.OptionsByKey[element.OptionKey] || " - ",
                food: element.PlaceKey && element.OptionKey && order.FoodsByOption[element.OptionKey] || "No come aquí",
                comment: element.Comment || " - "
            })), "employeeName");
        }

        bind() {
            ko.applyBindings(this);

            var tables = <any>$('table.table-detail');
            tables.dataTable({
                bPaginate: false,
                sDom: "<'row-fluid'<'span6'T><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
                bInfo: false,
                oLanguage: {
                    sEmptyTable: "No hay datos disponibles",
                    sInfo: "Mostrando desde _START_ hasta _END_ de _TOTAL_ registros",
                    sInfoEmpty: "Mostrando desde 0 hasta 0 de 0 registros",
                    sInfoFiltered: "(filtrado de _MAX_ registros en total)",
                    sInfoPostFix: "",
                    sInfoThousands: ",",
                    sLengthMenu: "Mostrar _MENU_ registros",
                    sLoadingRecords: "Cargando...",
                    sProcessing: "Procesando...",
                    sSearch: "Buscar:",
                    sZeroRecords: "No se encontraron resultados",
                    oPaginate: {
                        sFirst: "Primero",
                        sLast: "Último",
                        sNext: "Siguiente",
                        sPrevious: "Anterior"
                    },
                    oAria: {
                        sSortAscending: ": activar para Ordenar Ascendentemente",
                        sSortDescending: ": activar para Ordendar Descendentemente"
                    }
                },
                oTableTools: {
                    aButtons: [
                        {
                            sExtends: "print",
                            sButtonText: "Imprimir"
                        },
                        {
                            sExtends: "copy",
                            sButtonText: "Copiar"
                        },
                        {
                            sExtends: "pdf",
                            sButtonText: "PDF"
                        },
                        {
                            sExtends: "csv",
                            sButtonText: "Excel"
                        }
		            ]
                }
            });
        }

        refresh() {
            location.reload();
        }
    }
}