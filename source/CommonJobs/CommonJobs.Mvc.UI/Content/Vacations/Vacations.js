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
            DataTablesHelpers.column.fullName(
                function (data) { return data.employee.LastName; },
                function (data) { return data.employee.FirstName; }),
            DataTablesHelpers.column.date(function (data) { return data.employee.HiringDate; }),
            DataTablesHelpers.column.number(function (data) { return data.vacations.TotalPending; }),
            DataTablesHelpers.column.number(function (data) { return data.vacations.TotalTaken; })
        ]
        ,"sDom": 'T<"clear">lfrtip'
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
});