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
    var years = _.range(moment(ViewData.now).year(), moment(ViewData.now).year() - 9, -1);

    $.extend(DataTablesHelpers.column, {
        vacationCell: function (getVal, moreOptions) {
            return jQuery.extend({
                "sType": "nulls-below-string",
                "mData": function (data, type, val) {
                    if (type === 'set') return; //TODO
                    var val = getVal(data);
                    var earned = val.Earned || 0;
                    var taken = val.Taken || 0;
                    switch (type) {
                        case 'filter': return !earned && !taken ? ' - ' : "" + taken + " / " + earned;
                        case 'display': return !earned && !taken ? ' - ' : "" + taken + " / " + earned;
                        default: return !earned && !taken ? null : taken;
                    }
                }
            }, moreOptions);
        },
        vacationsByYear: function (year, moreOptions) {
            return DataTablesHelpers.column.vacationCell(
                function (data) {
                    return !data || !data.vacations || !data.vacations.ByYear || !data.vacations.ByYear[year]
                        ? {}
                        : data.vacations.ByYear[year];
                },
                moreOptions);
        },
        vacationsOld: function (fromYear, moreOptions) {
            return DataTablesHelpers.column.vacationCell(
                function (data) {
                    var result = { Earned: 0, Taken: 0 };
                    if (data && data.vacations && data.vacations.ByYear) {
                        _.each(data.vacations.ByYear, function (v, k) {
                            if (k <= fromYear) {
                                result.Earned += (+v.Earned || 0);
                                result.Taken += (+v.Taken || 0);
                            }
                        });
                    }
                    return result;
                },
                moreOptions);
        }
    });
    
    var columns = [
            DataTablesHelpers.column.link(
                DataTablesHelpers.column.fullName(
                    function (data) { return data.employee.LastName; },
                    function (data) { return data.employee.FirstName; }),
                function (data) { return urlGenerator.action("Edit", "Employees", data.employee.Id); }),
            DataTablesHelpers.column.date(function (data) { return data.employee.HiringDate; }),
            DataTablesHelpers.column.number(function (data) { return data.vacations.TotalPending; }),
            DataTablesHelpers.column.number(function (data) { return data.vacations.TotalTaken; })
    ];

    _.each(years, function(y) {
        columns.push(DataTablesHelpers.column.vacationsByYear(y));
    });

    columns.push(DataTablesHelpers.column.vacationsOld(moment(ViewData.now).year() - 9));

    $table.dataTable(
    {
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": true,
        //"bSort": false,
        "bInfo": false,
        "bAutoWidth": false,
        "aoColumns": columns,
        "sDom": 'T<"clear">lfrtip'
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
        , "fnFooterCallback": function (nFoot, aaData, iStart, iEnd, aiDisplay) {
            var $footer = $(nFoot);
            var cells = {
                pending: $footer.find(".pending"),
                taken: $footer.find(".taken")
            };
            _.chain(aiDisplay)
                .map(function (x) { return vacations = aaData[x].vacations; })
                .filter(function (x) { return x; })
                .reduce(
                    function (memo, x) {
                        return {
                            pending: x.TotalPending ? x.TotalPending + memo.pending : memo.pending,
                            taken: x.TotalTaken ? x.TotalTaken + memo.taken : memo.taken
                        };
                    },
                    {
                        pending: 0, taken: 0
                    })
                .each(function (v, k) {
                    cells[k].text(v);
                });
        }
    });

    whileTrue(
        function (take, skip, callback) {
            jQuery.getJSON(urlGenerator.action("VacationBunch", "Vacations"), { Skip: skip, Take: take }, function (data, textStatus, jqXHR) {
                callback(data);
            });
        },
        function (data, take, skip) {
            $table.dataTable().fnAddData(
                _.map(data.Items, function (employee) {
                    return { employee: employee, vacations: $.extend({}, CJLogic.CalculateVacations(employee.HiringDate, employee.Vacations, ViewData.now)) };
                }));

            var thereAreMore = skip + take < data.TotalResults;

            if (thereAreMore) {
                return true;
            } else {
                return false;
            }
        });
});