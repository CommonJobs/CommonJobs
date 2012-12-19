/// <reference path="/Scripts/AjaxHelper.js" />

$(function () {
    var toStringEmpty = function () { return ""; };
    var rowTemplate = _.template($("#row-template").text());
    var $table = $('#absences-table');
    var currentYear = ViewData.currentYear;
    var year = ViewData.year;

    var columns = [
            DataTablesHelpers.column.link(
                DataTablesHelpers.column.fullName(
                    function (data) { return data.LastName; },
                    function (data) { return data.FirstName; }),
                function (data) { return urlGenerator.action("Edit", "Employees", data.Id); },
                {
                    sClass: "cell-name"
                })
            //TODO: Other columns with related abscence date, like employee summary, or something
    ];
    
    var current = moment([year]);
    var end = moment([year]).endOf("year").startOf('day').valueOf();
    
    while (current.valueOf() <= end) {
        var weekday = current.day();
        var index = "m" + current.month() + "d" + current.date();
        var generateMData = function (index) {
            return function (source, type, val) { return source.daysData[index] || null; };
        };
        columns.push({
            bSortable: false,
            sClass: "cell-day" + (weekday == 0 || weekday == 6 ? " weekend" : ""), 
            mData: generateMData(index),
            fnCreatedCell: function (nTd, sData, oData, iRow, iCol) {
                //No puedo usar mRender porque me modifica el sData de fnCreatedCell
                if (sData) {
                    $(nTd)
                        .addClass("absence " + sData.AbsenceType + " " + sData.ReasonSlug)
                        .data("absenceData", sData);
                }
            }
        });
        current.add('days', 1);
    }

    var table = $table.dataTable(
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
            var $row = $(nRow);
            $row.data("absenceData", aData);
        }
    });
    
    var processor = new AjaxHelper.BunchProcessor(
        function (take, skip, callback) {
            jQuery.getJSON(urlGenerator.action("AbsenceBunch", "Absences"), { year: year, Skip: skip, Take: take, Term: "mos" }, function (data, textStatus, jqXHR) {
                callback(data);
            });
        },
        function (data, take, skip) {
            for (var i in data.Items) {
                var item = data.Items[i];
                var daysData = item.daysData = { };

                for (var j in item.Absences) {
                    var absence = item.Absences[j];
                    absence.toString = toStringEmpty; //No puedo usar mRender porque me modifica el sData de fnCreatedCell, otra opción sería borrar el contenido del TD en fnCreatedCell
                    var from = moment(absence.RealDate).startOf('day');
                    var to = moment(absence.To || absence.RealDate).startOf('day');
                    if (to.valueOf() < from.valueOf)
                        to = from;

                    if (to.year() >= ViewData.year && from.year() <= ViewData.year) {
                        var end = to.valueOf();
                        var current = from;
                        while (current.valueOf() <= end) {
                            daysData["m" + current.month() + "d" + current.date()] = absence;
                            current.add('days', 1);
                        }
                    }
                }
            }
            $table.dataTable().fnAddData(data.Items);
        },
        function (data, take, skip) {
            return data.TotalResults - skip - take;
        },
        function () {
            //console.log("td.absence");
            //console.log($("td.absence"));
        }
    );
    processor.run(ViewData.bsize);
});