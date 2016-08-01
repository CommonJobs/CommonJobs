$(document).ready(function () {
    var evaluationStates = ['En curso', 'Esperando Eval Empleado', 'Esperando Eval Responsable', 'Esperando Eval Empresa', 'Lista para devolución', 'Abierta para devolución', 'Finalizada'];

    var HistoryDashboard = function (data) {
        this.items = ko.observableArray();
        this.fullName = ko.observable("")
        if (data) {
            this.fromJS(data);
        }

        this.headers = [
            { title: "Período", sortable: true, sortPropertyName: "period", defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
            { title: "Evaluadores", sortable: false },
            { title: "Responsable", sortable: true, sortPropertyName: "responsibleId", defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
            { title: "Estado", sortable: true, sortPropertyName: "state", defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
            { title: "Promedio total", sortable: false }
        ];
        this.sort = commonSort.bind(this);
        this.defaultSort = function () {
            this.headers[0].activeSort(false);
            this.sort(this.headers[0]);
        };
        this.isLoading = ko.observable(false);
    };

    HistoryDashboard.prototype.fromJS = function (data) {
        this.items(_.map(data, function (e) {
            return new EvaluationItem(e);
        }));
        this.fullName(data[0].FullName)
    };

    var EvaluationItem = function (data) {
        this.period = "";
        this.responsibleId = "";
        this.evaluators = "";
        this.state = "";
        this.averageCalification = "";
        if (data) {
            this.fromJS(data)
        }
    }

    EvaluationItem.prototype.fromJS = function (data) {
        var self = data;
        this.period = data.Period;
        this.responsibleId = data.ResponsibleId;
        this.evaluators = data.Evaluators;
        this.state = evaluationStates[data.State];
        this.averageCalification = data.AverageCalification != null ? data.AverageCalification : "-";
    }

    var viewmodel = new HistoryDashboard();
    getHistoryDashboard(window.ViewData.userName);
    ko.applyBindings(viewmodel);

    function getHistoryDashboard(userName) {
        viewmodel.isLoading(true);
        var ajax = $.get("/Evaluations/api/GetEmployeeEvaluationHistory",
            {
                username: userName
            },
            function (model) {
                viewmodel.fromJS(model);
                viewmodel.defaultSort();
            },
            "json");
    }
});
