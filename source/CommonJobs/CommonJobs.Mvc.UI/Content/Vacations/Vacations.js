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
    var currentYear = ViewData.currentYear;
    var yearColumns = ViewData.years.length;
    var years = ViewData.years;

    var formatVacation = function (val) {
        return !val || (!val.Earned && !val.Taken)
            ? ' - '
            : "" + val.Taken + " / " + val.Earned;
    };

    $.extend(DataTablesHelpers.column, {
        vacationCell: function (getVal, moreOptions) {
            return jQuery.extend({
                "sType": "nulls-below-string",
                "mData": function (data, type, val) {
                    if (type === 'set') return; //TODO
                    var val = getVal(data);
                    switch (type) {
                        case 'filter':
                        case 'display': return formatVacation(val);
                        default:
                            if (!val || _.isUndefined(val.Earned))
                                return null;
                            else
                                return val.Earned
                                    ? val.Taken + 1 / val.Earned
                                    : val.Taken;
                    }
                }
            }, moreOptions);
        },
        vacationsByYear: function (year, moreOptions) {
            return DataTablesHelpers.column.vacationCell(
                function (data) {
                    return data.vacations && data.vacations.ByYear
                        ? data.vacations.ByYear[year]
                        : null;
                },
                moreOptions);
        },
        vacationsOld: function (fromYear, moreOptions) {
            return DataTablesHelpers.column.vacationCell(
                function (data) { return data.vacations.Older },
                moreOptions);
        },
        numberNegativeInRed: function (getVal, moreOptions) {
            return jQuery.extend({
                "sType": "nulls-below-numeric",
                "mData": function (data, type, val) {
                    if (type === 'set') return; //TODO
                    var val = getVal(data);
                    switch (type) {
                        case 'filter':
                            return _.isUndefined(val) ? "<em>Sin datos</em>" : val;
                        case 'display':
                            return _.isUndefined(val) ? "<em>Sin datos</em>"
                                : val < 0 ? "<span class='alert-error'>" + val + "</span>"
                                : val;
                        default:
                            return _.isUndefined(val) ? null : val;
                    }
                }
            }, moreOptions);
        },
        numberRedHideZeros: function (getVal, moreOptions) {
            return jQuery.extend({
                "sType": "nulls-below-numeric",
                "mData": function (data, type, val) {
                    if (type === 'set') return; //TODO
                    var val = getVal(data);
                    switch (type) {
                        case 'filter':
                            return _.isUndefined(val) ? "" : val;
                        case 'display':
                            return _.isUndefined(val) ? ""
                                : !val ? " - "
                                : "<span class='alert-error'>" + val + "</span>";
                        default:
                            return _.isUndefined(val) ? null : val;
                    }
                }
            }, moreOptions);
        }
    });

    var columns = [
            DataTablesHelpers.column.link(
                DataTablesHelpers.column.fullName(
                    function (data) { return data.employee.LastName; },
                    function (data) { return data.employee.FirstName; }),
                function (data) { return urlGenerator.action("Edit", "Employees", data.employee.Id); }),
            DataTablesHelpers.column.month(function (data) { return data.employee.HiringDate; }),
            DataTablesHelpers.column.numberNegativeInRed(function (data) { return data.vacations.TotalPending; }),
            DataTablesHelpers.column.number(function (data) { return data.vacations.TotalTaken; }),
            DataTablesHelpers.column.numberRedHideZeros(function (data) { return data.vacations.InAdvance.Taken; })
    ];

    _.each(years, function (y) {
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
                    aButtons: ["csv", "xls", "pdf"]
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
                old: $footer.find(".old"),
                inAdvance: $footer.find(".inadvance")
            };

            _.each(years, function (y) {
                cells[y] = $footer.find(".year-" + y);
            });

            var cleanReduce = { pending: 0, taken: 0, old: { Taken: 0, Earned: 0 }, inAdvance: 0 };
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

                        memo.inAdvance += data.InAdvance.Taken || 0;
                        var old = data.Older;
                        memo.old.Taken += old.Taken || 0;
                        memo.old.Earned += old.Earned || 0;

                        _.each(years, function (y) {
                            var v = data.ByYear[y];
                            if (v) {
                                memo[y].Taken += v.Taken || 0;
                                memo[y].Earned += v.Earned || 0;
                            }
                        });
                        return memo;
                    },
                    cleanReduce).value();


            _.each(years, function (y) {
                totals[y] = formatVacation(totals[y]);
            });
            totals.old = formatVacation(totals.old);

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
                    var data = { HiringDate: employee.HiringDate, Vacations: employee.Vacations };
                    var configuration = { CurrentYear: currentYear, DetailedYearsQuantity: yearColumns };
                    var report = CJLogic.CalculateVacations(data, configuration);
                    return {
                        employee: employee,
                        vacations: $.extend(
                            {
                                TotalEarned: null,
                                TotalTaken: null,
                                TotalPending: null,
                                Older: {},
                                ByYear: {},
                                InAdvance: {}
                            },
                            report.Result)
                    };
                }));

            var thereAreMore = skip + take < data.TotalResults;

            return thereAreMore;
        });
});