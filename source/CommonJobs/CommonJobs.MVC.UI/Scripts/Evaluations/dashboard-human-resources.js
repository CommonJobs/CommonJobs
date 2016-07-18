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
        { title: 'Links', sortable: false, defaultPropertyName: 'fullName' },
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
        this.isEvaluationEditable = ko.observable(true);
        this.isEditable = false;
        this.responsibleId = ko.observable('');
        this.fullName = '';
        this.userName = '';
        this.period = '';
        this.currentPosition = '';
        this.seniority = '';
        this.state = ko.observable('');
        this.currentState = '';
        this.sharedLinks = ko.observableArray();
        if (data) {
            this.fromJs(data);
        }
    }

    EvaluationReport.prototype.fromJs = function (data) {
        var self = data;
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
        this.sharedLinks(_.map(data.SharedLinks, function (sharedLink) {
            return {
                link: urlGenerator.sharedAction(self.Period + "/" + self.UserName, "Evaluations", null, sharedLink.SharedCode),
                period: self.Period,
                userName: self.UserName,
                sharedCode: sharedLink.SharedCode,
                friendlyName: ko.observable(sharedLink.FriendlyName),
                expirationDate: ko.observable(sharedLink.ExpirationDate),
                edittingFriendlyName: ko.observable(false)
            }
        }));
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

    EvaluationReport.prototype.createLink = function (data, event) {
        $.post("/Evaluations/api/CreateEvaluationSharedLink/" + this.period + "/" + this.userName)
        .success(function (sharedLink) {
            var newSharedLink = {
                link: urlGenerator.sharedAction(this.period + "/" + this.userName, "Evaluations", null, sharedLink.SharedCode),
                period: this.period,
                userName: this.userName,
                friendlyName: ko.observable(sharedLink.FriendlyName),
                sharedCode: sharedLink.SharedCode,
                expirationDate: ko.observable(new Date(sharedLink.ExpirationDate).toLocaleDateString()),
                edittingFriendlyName: ko.observable(false)
            };
            data.sharedLinks.push(newSharedLink);
        })
        .fail(function () {
            alert('Fallo interno. Por favor recargue la página.');
        });
    }

    EvaluationReport.prototype.updateSharedLink = function (data, event) {
        viewmodel.isLoading(true);
        var updatedSharedLink = sharedLinkToDto(data)
        $.ajax("/Evaluations/api/UpdateEvaluationSharedLink/", {
            type:"POST",
            dataType: "text",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(updatedSharedLink)
        })
        .success(function () {
            data.edittingFriendlyName(false);
        })
        .fail(function (error, textStatus) {
            alert('Fallo interno. Por favor recargue la página.');
        })
        .always(function () {
            viewmodel.isLoading(false);
        });
    }

    SharedLink.prototype.deleteSharedLink = function (data, event) {
        viewmodel.isLoading(true);
        $.ajax("/Evaluations/api/DeleteEvaluationSharedLink/", {
            type: "POST",
            dataType: "text",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(data.toDto())
        })
        .success(function () {
            var evaluation = _.find(viewmodel.items(),function (eval) {
                return eval.userName == data.userName;
           });
           evaluation.sharedLinks(_.without(evaluation.sharedLinks(), data));
        })
        .fail(function (error, textStatus) {
            alert('Fallo interno. Por favor recargue la página.');
        })
        .always(function () {
            viewmodel.isLoading(false);
        });
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

    function sharedLinkToDto(updatedSharedLink) {
        return {
            period: updatedSharedLink.period,
            userName: updatedSharedLink.userName,
            sharedLink: {
                FriendlyName: updatedSharedLink.friendlyName(),
                SharedCode: updatedSharedLink.sharedCode,
                ExpirationDate: updatedSharedLink.expirationDate()
            }
        }
    }

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
