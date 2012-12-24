/// <reference path="/Scripts/AjaxHelper.js" />
$(function () {
    var rowTemplate = _.template($("#row-template").text());

    var $table = $('#vacations-table');
    var currentYear = ViewData.currentYear;
    var yearColumns = ViewData.years.length;
    var years = ViewData.years;

    var createElementWithDetail = function (content, val) {
        if (val.Detail && val.Detail.length) {
            var contentArr = [];
            contentArr.push('<ul>');
            _.each(val.Detail, function (vacation) {
                contentArr.push('<li>');
                
                var from = moment(vacation.From);
                var to = moment(vacation.To);

                var formatTo = vacation.Period == from.year() && vacation.Period == to.year()
                    ? "D MMM"
                    : "D MMM YYYY"

                if (to.year() == from.year() && to.month() == from.month() && to.day() == from.day()) {
                    contentArr.push("Periodo " + vacation.Period + ": " + to.format(formatTo));
                } else {
                    var formatFrom = from.year() && to.year()
                        ? (from.month() == to.month() ? "D" : "D MMM")
                        : "D MMM YYYY"

                    contentArr.push("Periodo " + vacation.Period + ": " + from.format(formatFrom) + " - " + to.format(formatTo));
                }
                contentArr.push('</li>');
            });
            contentArr.push('</ul>');
            var span = "<span class='vacation-list' data-content='" +
                jQuery('<div />').text(contentArr.join("\n")).html().replace(/"/g, "&quot;").replace(/'/g, "&apos;")
                + "'>" + content + "</span>";
            return span;
        } else {
            return content;
        }
    };

    var formatVacation = function (val) {
        if (!val) {
            return null;
        } else  if (!val.Earned && !val.Taken) {
            return " - ";
        } else {
            var content = "" + val.Taken + " / " + val.Earned;
            return createElementWithDetail(content, val);
        }
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
                        case 'display':
                            return formatVacation(val);
                        default:
                            if (!val || !val.Earned)
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
                    return data.vacations && (data.vacations.TotalEarned || data.vacations.TotalTaken)
                        ? data.vacations.ByYear[year] || { Earned: 0 }
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
        vacationsInAdvance: function (getVal, moreOptions) {
            return jQuery.extend({
                "sType": "nulls-below-numeric",
                "mData": function (data, type, val) {
                    if (type === 'set') return; //TODO
                    var val = getVal(data);
                    switch (type) {
                        case 'filter':
                            return !val ? "" : val.Taken;
                        case 'display':
                            return !val ? ""
                                : !val.Taken ? " - "
                                : createElementWithDetail("<span class='alert-error'>" + val.Taken + "</span>", val);
                        default:
                            return val && val.Taken ? val.Taken : null;
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
            DataTablesHelpers.column.vacationsInAdvance(function (data) { return data.vacations.InAdvance; })
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
        sDom: "<'row-fluid'<'span6'T><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
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
        },
        fnCreatedRow: function (nRow, aData, iDataIndex) {
            $(nRow).find("td").first().nextAll().addClass("center");
            $(nRow).find(".vacation-list").popover({
                title: 'Detalle',
                placement: "top"
            });
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
                        
                        if (data.InAdvance && data.InAdvance.Taken)
                            memo.inAdvance += data.InAdvance.Taken;

                        var old = data.Older || { Taken: 0, Earned: 0 };
                        memo.old.Taken += old.Taken;
                        memo.old.Earned += old.Earned;

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


    var processor = new AjaxHelper.BunchProcessor(
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
                                ByYear: {}
                            },
                            report.Result)
                    };
                }));
        },
        function (data, take, skip) {
            return data.TotalResults - skip - take;
        });
    processor.run(ViewData.bsize);
});