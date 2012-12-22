﻿/// <reference path="/Scripts/AjaxHelper.js" />

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
            }
        ),
        {
            sClass: "cell-data",
            sType: "nulls-below-numeric",
            mData: "TotalFull"
        },
        {
            sClass: "cell-data",
            sType: "nulls-below-numeric",
            mData: "TotalPartial"
        },
        {
            sClass: "cell-data",
            sType: "nulls-below-numeric",
            mData: "TotalRemoteWork"
        }
    ];

    $("tr.months").each(function () {
        var current = moment([year]);
        $(this).find("th.month").each(function () {
            $(this).text(current.format("MMMM YYYY"));
            current.add('months', 1);
        });
    });

    
    var current = moment([year]);
    var end = moment([year]).endOf("year").startOf('day').valueOf();
    while (current.valueOf() <= end) {
        createDayColumn(current.clone());
        current.add('days', 1);
    }

    function createDayColumn(date) {
        var weekday = date.day();
        var sClass = "cell-day" + (weekday == 0 || weekday == 6 ? " weekend" : "");
        var daysDataIndex = +date.format("DDD");
        columns.push({
            bSortable: false,
            sClass: sClass,
            mData: null,
            fnCreatedCell: function (nTd, sData, oData, iRow, iCol) {
                var $td = $(nTd);
                $td.data("date", date);
                
                var dayData = oData.daysData[daysDataIndex];
                
                var classes = dayData ? ["absence", dayData.AbsenceType, dayData.ReasonSlug] : []
                if ((oData.HiringDateNo && daysDataIndex < oData.HiringDateNo) || (oData.TerminationDateNo && daysDataIndex > oData.TerminationDateNo)) {
                    classes.push("not-active");
                }

                if (classes.length) {
                    $td.addClass(classes.join(" "));
                }
            }
        });
    };

    var table = $table.dataTable(
    {
        bPaginate: false,
        bAutoWidth: false,
        aoColumns: columns,
        sDom: "<'row-fluid'<'span6'T><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
        oTableTools: {
            aButtons: [
            /*
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
            */
            ]
        },
        fnCreatedRow: function (nRow, aData, iDataIndex) {
            $(nRow).data("employee", aData);
        }
    });

    function getTitle() {
        var $this = $(this);
        var date = $this.data("date");
        var oData = $this.parent("tr").data("employee");
        var sData = oData.daysData[+date.format("DDD")];
        var name = "" + oData.LastName + ", " + oData.FirstName;
        if (sData)
            name = "<a href='" + urlGenerator.action("Edit", "Employees", oData.Id) + "'>" + name + "</a>";
        return name + "<br />" + date.format("dddd D [de] MMMM YYYY");
    }

    function getContent() {
        //TODO: mejorar este método, que como otras cosas está horrible
        var $this = $(this);
        var date = $this.data("date");
        var oData = $this.parent("tr").data("employee");
        var sData = oData.daysData[+date.format("DDD")];

        var period = "";
        var from = moment(sData.RealDate);
        var to = moment(sData.To || sData.RealDate);

        var formatTo = year == from.year() && year == to.year()
            ? "D MMMM"
            : "D MMMM YYYY"

        if (to.year() == from.year() && to.month() == from.month() && to.day() == from.day()) {
            period = to.format(formatTo);
        } else {
            var formatFrom = from.year() && to.year()
                ? (from.month() == to.month() ? "D" : "D MMMM")
                : "D MMMM YYYY"

            period = from.format(formatFrom) + " - " + to.format(formatTo);
        }

        var absenceType =
            sData.AbsenceType == "Partial" ? "Parte del día"
            : sData.AbsenceType == "RemoteWork" ? "Trabajo remoto"
            : "Todo el día";

        var attachment =
            sData.Attachment ? "<a href='" + urlGenerator.action("Get", "Attachments", sData.Attachment.Id) + "'>" + sData.Attachment.FileName + "</a>"
            : "<em>No tiene</em>";

        var md = new MarkdownDeep.Markdown();
        md.ExtraMode = true;
        var note = sData.Note ? md.Transform(sData.Note) : "<em>No tiene</em>";

        return "<dl class='dl-horizontal'>" +
            "<dt>Razón:</dt><dd>" + sData.Reason + "</dd>" +
            "<dt>Tipo:</dt><dd>" + absenceType + "</dd>" + 
            "<dt>Fecha:</dt><dd>" + period + "</dd>" + 
            "<dt>Certificado:</dt><dd>" + (sData.HasCertificate ? "Si" : "No") + "</dd>" +
            "<dt>Adjunto:</dt><dd>" + attachment + "</dd>" +
            "<dt>Nota:</dt><dd class='markdown-content'>" + note + "</dd>" + 
            "</dl>";
    }
    
    var processor = new AjaxHelper.BunchProcessor(
        function (take, skip, callback) {
            jQuery.getJSON(urlGenerator.action("AbsenceBunch", "Absences"), { year: year, Skip: skip, Take: take/*, Term: "mos" */}, function (data, textStatus, jqXHR) {
                callback(data);
            });
        },
        function (data, take, skip) {
            for (var i in data.Items) {
                var item = data.Items[i];
                var daysData = item.daysData = [];

                if (item.HiringDate) {
                    var aux = moment(item.HiringDate);
                    if (aux.year() == year) {
                        item.HiringDateNo = +aux.format("DDD");
                    }
                }
                if (item.TerminationDate) {
                    var aux = moment(item.TerminationDate);
                    if (aux.year() == year) {
                        item.TerminationDateNo = +aux.format("DDD");
                    }
                }

                for (var j in item.Absences) {
                    var absence = item.Absences[j];
                    absence.toString = toStringEmpty; //No puedo usar mRender porque me modifica el sData de fnCreatedCell, otra opción sería borrar el contenido del TD en fnCreatedCell
                    var from = moment(absence.RealDate).startOf('day');
                    var to = moment(absence.To || absence.RealDate).startOf('day');
                    if (to.valueOf() < from.valueOf)
                        to = from;

                    if (to.year() >= ViewData.year && from.year() <= ViewData.year) {
                        var end = to.valueOf();
                        var date = from;
                        while (date.valueOf() <= end) {
                            daysData[date.format("DDD")] = absence;
                            date.add('days', 1);
                        }
                    }
                }

                item["TotalFull"] = 0;
                item["TotalPartial"] = 0;
                item["TotalRemoteWork"] = 0;

                for (var j in daysData) {
                    var absence = daysData[j];
                    item["Total" + absence.AbsenceType]++;
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
            $("#absences-table")
                .tooltip({
                    selector: "td.cell-day:not(.absence)",
                    html: true,
                    title: getTitle,
                    placement: "bottom"
                    //, delay: 500
                });

            //TODO: mejorar esto, está horrible
            $("#absences-table").find("td.cell-day.absence")
                .popover({
                    content: getContent,
                    title: getTitle,
                    trigger: 'manual',
                    animate: false,
                    html: true,
                    placement: "bottom",
                    template: '<div class="popover" onmouseover="$(this).mouseleave(function() {$(this).hide(); });"><div class="arrow"></div><div class="popover-inner"><button type="button" class="close" onclick="$(this).parent().parent().hide();">&times;</button><h3 class="popover-title"></h3><div class="popover-content"><p></p></div></div></div>'
                }).mouseenter(function (e) {
                    $(this).popover('show');
                    $(this).mouseleave(function (e) {
                        if (!$(e.relatedTarget).parent(".popover").length)
                            $(this).popover('hide');
                    })
                });
        }
    );
    processor.run(ViewData.bsize);
});