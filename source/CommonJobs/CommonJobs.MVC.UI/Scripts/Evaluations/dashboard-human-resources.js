$(document).ready(function () {
    var evaluationStates = ['En curso', 'Esperando Eval Empleado', 'Esperando Eval Responsable', 'Esperando Eval Empresa', 'Lista para devolución', 'Abierta para devolución', 'Finalizada'];

    var ReportDashboard = function (data) {
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
        { title: 'Estado', sortable: true, sortPropertyName: 'state', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
        { title: '', sortable: false }
        ];
        this.sort = commonSort.bind(this);
        this.defaultSort = function () {
            this.headers[0].activeSort(false);
            this.sort(this.headers[0]);
        }
        this.responsibleManagerModel = new ResponsibleManager();
        this.isLoading = ko.observable(false);
    }

    ReportDashboard.prototype.fromJS = function (data) {
        this.items(_.map(data.Evaluations, function (e) {
            return new EvaluationReport(e);
        }));
    }

    ReportDashboard.prototype.getEvaluationState = function (state) {
        return this.evaluationStates[state];
    }

    var EvaluationReport = function (data) {
        this.id = '';
        this.isResponsible = false;
        this.isEvaluationManager = false;
        this.isEditable = false;
        this.responsibleId = ko.observable('');
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

    EvaluationReport.prototype.fromJs = function (data) {
        this.id = data.Id;
        this.isResponsible = data.IsResponsible;
        this.isEvaluationManager = window.ViewData.isEvaluationManager;
        this.isEditable = data.IsEditable;
        this.responsibleId(data.ResponsibleId);
        this.fullName = data.FullName;
        this.userName = data.UserName;
        this.period = data.Period;
        this.currentPosition = data.CurrentPosition || '';
        this.seniority = data.Seniority || '';
        this.state(data.State);
        this.stateName = evaluationStates[this.state()];
        this.stateClasses = "state-doc state-" + this.state();
        this.calificationUrl = urlGenerator.action(this.period + "/" + this.userName + "/", "Evaluations");
        this.showResponsibleManager = function (data, event) {
            viewmodel.responsibleManagerModel.fromJs({ evaluation: this });
            var popupContainer = $(event.target).parents('.responsible-column');
            popupContainer.append($('.content-modal'));
            $('.content-modal').show();
            return true;
        }
    }

    EvaluationReport.prototype.toJs = function () {
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

    var ResponsibleManager = function (data) {
        var self = this;
        this.evaluation = '';
        this.responsibles = ko.observableArray();
        this.newResponsible = ko.observable('');
        this.saveButtonEnable = ko.observable(false);
        if (data) {
            this.fromJs(data);
        };
        this.close = function () {
            $('.content-evaluation').append($('.content-modal'));
            $('.content-modal').hide();
            self.saveButtonEnable(false);
        };
        this.save = function () {
            this.responsibleManagerModel.close();
            this.responsibleManagerModel.saveButtonEnable(false);
            viewmodel.isLoading(true);
            $.post("/Evaluations/api/ChangeResponsible/", {
                period: self.evaluation.period,
                username: self.evaluation.userName,
                newResponsibleName: self.newResponsible()
            })
            .success(function () {
                self.evaluation.responsibleId(self.newResponsible());
            })
            .fail(function () {
                alert('Fallo interno. Por favor recargue la página.');
            })
            .always(function () {
                viewmodel.isLoading(false);
                self.newResponsible('');
            });
        };
    }

    ResponsibleManager.prototype.fromJs = function (data) {
        var self = this;
        this.evaluation = data.evaluation;
        this.responsibleId = data.responsibleId
    }

    var viewmodel = new ReportDashboard();

    getReportDashboard(window.ViewData.period);
    ko.applyBindings(viewmodel);
    commonSuggest($('.content-modal .search'), 'UserName');
    $("#selectedPeriod").change(function () {
        window.location = this.value;
    })

    function getReportDashboard(period) {
        viewmodel.isLoading(true);
        var ajax = $.getJSON("/Evaluations/api/GetDashboardEvaluationsForEmployeeManagers/" + period + "/", function (model) {
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
