$(function () {
    var rowTemplate = _.template($("#row-template").text());
    var whileTrue = function (getData, callback, take, skip) {
        skip = skip || 0;
        take = take || ViewData.bsize;
        getData(take, skip, function (data) {
            callback(data, take, skip) && whileTrue(getData, callback, take, skip + take);
        });
    };

    var $table = $('#vacations-table');
    var currentYear = _.first(ViewData.years);
    var yearColumns = ViewData.years.length;
    var years = ViewData.years;

    var getVacationByYear = function (vacations, year) {
        var result = { Earned: 0, Taken: 0 };
        if (vacations && vacations.ByYear && vacations.ByYear[year]) {
            var v = vacations.ByYear[year];
            result.Earned += (+v.Earned || 0);
            result.Taken += (+v.Taken || 0);
        }
        return result;
    };

    var getOldVacations = function (vacations, fromYear) {
        var result = { Earned: 0, Taken: 0 };
        if (vacations && vacations.ByYear) {
            _.each(vacations.ByYear, function (v, k) {
                if (k <= fromYear) {
                    result.Earned += (+v.Earned || 0);
                    result.Taken += (+v.Taken || 0);
                }
            });
        }
        return result;
    };

    var formatVacation = function(earned, taken) {
        return !earned && !taken ? ' - ' : "" + taken + " / " + earned;
    };

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
                        case 'filter': 
                        case 'display': return formatVacation(earned, taken);
                        default: return !earned && !taken ? null : taken;
                    }
                }
            }, moreOptions);
        },
        vacationsByYear: function (year, moreOptions) {
            return DataTablesHelpers.column.vacationCell(
                function (data) { return getVacationByYear(data.vacations, year); },
                moreOptions);
        },
        vacationsOld: function (fromYear, moreOptions) {
            return DataTablesHelpers.column.vacationCell(
                function (data) { return getOldVacations(data.vacations, fromYear) },
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

    columns.push(DataTablesHelpers.column.vacationsOld(currentYear - yearColumns));

    $table.dataTable(
    {
        bPaginate: false,
        bAutoWidth: false,
        aoColumns: columns,
        sDom: 'T<"clear">lfrtip',
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
                	aButtons: [ "csv", "xls", "pdf" ]
                }
		    ]
        },
        fnCreatedRow: function (nRow, aData, iDataIndex) {
            $(nRow).find("td").first().nextAll().addClass("center");
        },
        fnFooterCallback: function (nFoot, aaData, iStart, iEnd, aiDisplay) {
            var $footer = $(nFoot);
            var cells = {
                pending: $footer.find(".pending"),
                taken: $footer.find(".taken"),
                old: $footer.find(".old")
            };

            _.each(years, function (y) {
                cells[y] = $footer.find(".year-" + y);
            });

            var cleanReduce = { pending: 0, taken: 0, old: { Taken: 0, Earned: 0 } };
            _.each(years, function (v) {
                cleanReduce[v] = { Taken: 0, Earned: 0 };
            });

            var totals = _.chain(aiDisplay)
                .map(function (x) { return vacations = aaData[x].vacations; })
                .filter(function (x) { return x; })
                .reduce(
                    function (memo, data) {
                        memo.pending += +data.TotalPending || 0;
                        memo.taken += +data.TotalTaken || 0;

                        var old = getOldVacations(data, currentYear - yearColumns);
                        memo.old.Taken += old.Taken;
                        memo.old.Earned += old.Earned;

                        _.each(years, function (y) {
                            var v = getVacationByYear(data, y);
                            memo[y].Taken += v.Taken;
                            memo[y].Earned += v.Earned;
                        });
                        return memo;
                    },
                    cleanReduce).value();

            
            _.each(years, function(y) {
                totals[y] = formatVacation(totals[y].Earned, totals[y].Taken);
            });
            totals.old = formatVacation(totals.old.Earned, totals.old.Taken);

            _.each(totals, function (v, k) {
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