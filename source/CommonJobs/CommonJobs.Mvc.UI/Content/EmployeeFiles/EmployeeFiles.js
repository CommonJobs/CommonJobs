$(function () {
    var rowTemplate = _.template($("#row-template").text());
    var whileTrue = function (getData, callback, take, skip) {
        skip = skip || 0;
        take = take || ViewData.batchSize;
        getData(take, skip, function (data) {
            callback(data, take, skip) && whileTrue(getData, callback, take, skip + take);
        });
    };

    var $table = $('#employee-files-table');

    var columns = [
            DataTablesHelpers.column.string(function (data) { return data.employee.FileId; }),
            DataTablesHelpers.column.link(
                DataTablesHelpers.column.fullName(
                    function (data) { return data.employee.LastName; },
                    function (data) { return data.employee.FirstName; }
                ),
                function (data) { return urlGenerator.action("Edit", "Employees", data.employee.Id); }
            ),
            DataTablesHelpers.column.string(function (data) { return data.employee.Cuil; }),
            DataTablesHelpers.column.date(function (data) { return data.employee.HiringDate; }),
            DataTablesHelpers.column.string(function (data) { return data.employee.BankName; }),
            DataTablesHelpers.column.string(function (data) { return data.employee.BankBranch; }),
            DataTablesHelpers.column.string(function (data) { return data.employee.BankAccount; }),
            DataTablesHelpers.column.string(function (data) { return data.employee.UniqueBankCode; }),
            DataTablesHelpers.column.string(function (data) { return data.employee.HealthInsurance; }),
            DataTablesHelpers.column.string(function (data) { return data.employee.Agreement; }),
            DataTablesHelpers.column.string(function (data) { return data.employee.CurrentProject; }),
            DataTablesHelpers.column.date(function (data) { return data.employee.BirthDate; })
    ];

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
        }
        //TODO: format cells
        //,
        //fnCreatedRow: function (nRow, aData, iDataIndex) {
        //    $(nRow).find("td").first().nextAll().addClass("center");
        //}
    });

    whileTrue(
        function (take, skip, callback) {
            jQuery.getJSON(urlGenerator.action("EmployeeFileBatch", "EmployeeFiles"), { Skip: skip, Take: take }, function (data, textStatus, jqXHR) {
                callback(data);
            });
        },
        function (data, take, skip) {
            $table.dataTable().fnAddData(
                _.map(data.Items, function (employee) {
                    return { employee: employee };
                }));

            var thereAreMore = skip + take < data.TotalResults;

            return thereAreMore;
        });
});