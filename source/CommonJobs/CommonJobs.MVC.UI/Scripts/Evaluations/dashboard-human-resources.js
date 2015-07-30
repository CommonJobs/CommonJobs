$(document).ready(function () {
    var evaluationStates = ['En curso', 'Esperando Eval Empleado', 'Esperando Eval Responsable', 'Esperando Eval Empresa', 'Lista para devolución', 'Abierta para devolución', 'Finalizada'];

    var ReportDashboard = function (data) {
        var self = this;
        this.items = ko.observableArray();
        if (data) {
            this.fromJS(data);
        }
        this.headers = [
        { title: 'Periodo', sortable: true, sortPropertyName: 'period', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
        { title: 'Empleado', sortable: true, sortPropertyName: 'fullName', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
        { title: 'Puesto', sortable: true, sortPropertyName: 'currentPosition', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
        { title: 'Seniority', sortable: true, sortPropertyName: 'seniority', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
        { title: 'Responsable', sortable: true, sortPropertyName: 'responsibleId', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
        { title: 'Estado', sortable: true, sortPropertyName: 'state', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false), observable: true },
        { title: '', sortable: false }
        ];
        this.sort = commonSort.bind(this);
        this.defaultSort = function () {
            this.headers[0].activeSort(false);
            this.sort(this.headers[0]);
        }
        this.isLoading = ko.observable(false);
    }

    ReportDashboard.prototype.fromJS = function (data) {
        var self = this;
        debugger;
        this.items(_.map(data.Evaluations, function (e) {
            return new Evaluation(e);
        }));
    }

    ReportDashboard.prototype.getEvaluationState = function (state) {
        return this.evaluationStates[state];
    }

    var Evaluation = function (data) {
        this.id = '';
        this.isResponsible = false;
        this.isEditable = false;
        this.responsableId = '';
        this.fullName = '';
        this.userName = '';
        this.period = '';
        this.currentPosition = '';
        this.seniority = '';
        this.state = ko.observable('');
        this.currentState = '';
        if (data) {
            this.fromJs(data);
        }
    }

    Evaluation.prototype.fromJs = function (data) {
        this.id = data.Id;
        this.isResponsible = data.IsResponsible;
        this.isEditable = data.IsEditable;
        this.responsibleId = data.ResponsableId;
        this.fullName = data.FullName;
        this.userName = data.UserName;
        this.period = data.Period;
        this.currentPosition= data.CurrentPosition || '';
        this.seniority = data.Seniority || '';
        this.state(data.State);
        this.stateName = evaluationStates[this.state()];
        this.stateClasses = "state-doc state-" + this.state();
        this.calificationUrl = urlGenerator.action(this.period + "/" + this.userName + "/", "Evaluations");
    }

    Evaluation.prototype.toJs = function () {
        return {
            IsResponsible: this.isResponsible,
            ResponsibleId: this.responsibleId,
            FullName: this.fullName,
            CurrentPosition: this.currentPosition,
            Seniority: this.seniority,
            Evaluators: this.evaluators,
            State: this.state(),
            UserName: this.userName,
            Period: this.period
        };
    }

    var viewmodel = new ReportDashboard();

    getDashboardEvaluations();
    ko.applyBindings(viewmodel);

    function getDashboardEvaluations() {
        var ajax = $.getJSON("/Evaluations/api/GetDashboardEvaluationsForEmployeeManagers/" + employeePeriod + "/", function (model) {
            viewmodel.fromJS(model);
            viewmodel.defaultSort();

        });
        ajax.always(function () {
            viewmodel.isLoading(false);
        });
        ajax.fail(function () {
            alert('Fallo interno. Por favor recargue la página.');
        });
    }
});
