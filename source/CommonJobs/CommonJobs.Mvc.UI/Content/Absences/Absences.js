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
        //Necesito evaluar current ahora, no cuando se invocan las funciones
        (function (current) {
            var weekday = current.day();
            columns.push({
                bSortable: false,
                sClass: "cell-day" + (weekday == 0 || weekday == 6 ? " weekend" : ""),
                mData: function (source, type, val) {
                    return source.daysData["m" + current.month() + "d" + current.date()] || null;
                },
                fnCreatedCell: function (nTd, sData, oData, iRow, iCol) {
                    var $td = $(nTd);
                    $td.data("current", current);
                    //No puedo usar mRender porque me modifica el sData de fnCreatedCell
                    if (sData) {
                        $td
                            .addClass("absence " + sData.AbsenceType + " " + sData.ReasonSlug)
                            .data("absenceData", sData);
                    }
                },
            });
        })(current.clone());
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
            $row.find("td.cell-day:not(.absence)").tooltip({
                html: true,
                title: getTitle,
                placement: "bottom"
                //, delay: 500
            });
            $row.find("td.cell-day.absence").popover({
                html: true,
                title: getTitle,
                placement: "bottom",
                content: getContent
            });
        }
    });

    function getTitle() {
        var $this = $(this);
        var current = $this.data("current");
        var oData = $this.parent("tr").data("absenceData");
        return "" + oData.LastName + ", " + oData.FirstName + "<br />" + current.format("dddd D [de] MMMM YYYY");
    }

    function getContent() {
        var $this = $(this);
        var current = $this.data("current");
        var sData = $this.data("absenceData");
        var oData = $this.parent("tr").data("absenceData");
        //TODO: complete it
        return "<dl class='dl-horizontal'>" +
            "<dt>" + "Razón:" + "</dt>" + "<dd>" + sData.Reason + "</dd>" +
            "<dt>" + "Tipo:" + "</dt>" + "<dd>" + sData.AbsenceType + "</dd>" + //TODO: traducir
            "<dt>" + "Desde:" + "</dt>" + "<dd>" + sData.RealDate + "</dd>" + //TODO: formatear y cambiar "Desde" por "Fecha"
            "<dt>" + "Hasta:" + "</dt>" + "<dd>" + sData.To + "</dd>" + //TODO: formatear u ocultar o mostrar desde/hasta en una linea
            "<dt>" + "Certificado:" + "</dt>" + "<dd>" + (sData.HasCertificate ? "Si" : "No") + "</dd>" + 
            "<dt>" + "Adjunto:" + "</dt>" + "<dd>" + sData.Attachment + "</dd>" + //TODO: mostrar link? se me va el popover, como hago? con click?
            "<dt>" + "Nota:" + "</dt>" + "<dd>" + sData.Note + "</dd>" + //TODO: markdown
            "</dl>";
    }
    
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