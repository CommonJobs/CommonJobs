$(function () {
    var rowTemplate = _.template($("#row-template").text());
    var whileTrue = function (getData, callback, take, skip) {
        skip = skip || 0;
        take = take || 10;
        getData(take, skip, function (data) {
            callback(data, take, skip) && whileTrue(getData, callback, take, skip + take);
        });
    };

    var $debugElement = $('#debug-element');
    var $table = $('#vacations-table');

    $table.dataTable(
    {
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        //"bSort": false,
        "bInfo": false,
        "bAutoWidth": false,
        "aoColumns": [
            {
                "mData": function (data, type, val) {
                    if (type === 'set') {
                        //TODO: data.employee.LastName = parseLastName(val);
                        //TODO: data.employee.FirstName = parseFirstName(val);
                        return;
                    } else {
                        return (data.employee.LastName || "Sin apellido") + ", " + (data.employee.LastName || "Sin nombre");
                    }
                }
            },
            {
                "mData": function (data, type, val) {
                    if (type === 'set') {
                        //TODO: set HiringDate
                        return;
                    }
                    else if (type === 'display') {
                        if (data.employee.HiringDate) {
                            return moment(data.employee.HiringDate).format("MMM YYYY");
                        } else {
                            return "Sin fecha";
                        }
                    }
                    else if (type === 'filter') {
                        if (data.employee.HiringDate) {
                            return moment(data.employee.HiringDate).format("MMM DD-MM-YYYY-MM-DD");
                        } else {
                            return "Sin fecha";
                        }
                    }
                    else if (data.employee.HiringDate) {
                        return moment(data.employee.HiringDate).format("YYYY-MM-DD");
                    } else {
                        //TODO: Hacer algo para que estos queden siempre abajo?
                        return "z";
                    }
                }
            },
            {
                "sType": "only-numeric",
                "mData": function (data, type, val) {
                    if (type === 'set') {
                        //TODO: set pending
                        return;
                    }
                    else if (_.isUndefined(data.vacations.TotalPending)) {
                        return "Sin datos";
                    } else {
                        return data.vacations.TotalPending;
                    }
                }
            },
            {
                "sType": "numeric",
                "mData": function (data, type, val) {
                    if (type === 'set') {
                        //TODO: set TotalTaken
                        return;
                    }
                    else if (_.isUndefined(data.vacations.TotalTaken)) {
                        return type === 'sort' ? -1 : "Sin datos";
                    } else {
                        return data.vacations.TotalTaken;
                    }
                }
            }
        ]
        //  , "sDom": "<'row'<'span6'l><'span6'f>r>t<'row'<'span6'i><'span6'p>>"
        , "sDom": 'T<"clear">lfrtip'
        //TODO: Read 
        // * http://datatables.net/release-datatables/extras/TableTools/bootstrap.html
        // * http://datatables.net/extras/tabletools/button_options
        ,"oTableTools": {
            "aButtons": [
                "copy",
                "print",
                {
                	"sExtends":    "collection",
                	"sButtonText": "Save",
                	"aButtons":    [ "csv", "xls", "pdf" ]
                }
		    ]
        }
    });

    whileTrue(
        function (take, skip, callback) {
            jQuery.getJSON(urlGenerator.action("VacationBunch", "Vacations"), { Skip: skip, Take: take }, function (data, textStatus, jqXHR) {
                callback(data);
            });
        },
        function (data, take, skip) {
            //#region Debug
            $debugElement.append("\n\nSkipped: " + data.Skipped + "; TotalResults: " + data.TotalResults);
            //#endregion

            _.each(data.Items, function (employee) {
                var vacations = {};
                if (data.Items.length) {
                    vacations = $.extend(vacations, CJLogic.CalculateVacations(employee.HiringDate, employee.Vacations, ViewData.now));
                }

                //#region Debug
                $debugElement.append("\n" + JSON.stringify(employee));
                if (vacations) {
                    $debugElement.append("\n" + JSON.stringify(vacations));
                }
                $debugElement.append("\n");
                //#endregion

                $table.dataTable().fnAddData({ employee: employee, vacations: vacations });
            });

            var thereAreMore = skip + take < data.TotalResults;

            if (thereAreMore) {
                return true;
            } else {
                return false;
            }
        });

    jQuery.extend(jQuery.fn.dataTableExt.oSort, {
        "only-numeric-asc": function (a, b) {
            var na = parseFloat(a);
            var nb = parseFloat(b);

            if (!isNaN(na) && isNaN(nb)) {
                return -1;
            } else if (isNaN(na) && !isNaN(nb)) {
                return 1;
            } else {
                if (!isNaN(na)) {
                    a = na;
                    b = nb;
                }
                return ((a < b) ? -1 : ((a > b) ? 1 : 0));
            }
        },

        "only-numeric-desc": function (a, b) {
            var na = parseFloat(a);
            var nb = parseFloat(b);
            if (!isNaN(na) && isNaN(nb)) {
                return -1;
            } else if (isNaN(na) && !isNaN(nb)) {
                return 1;
            } else {
                if (!isNaN(na)) {
                    a = na;
                    b = nb;
                }
                return ((a < b) ? 1 : ((a > b) ? -1 : 0));
            }
        }
    });
});