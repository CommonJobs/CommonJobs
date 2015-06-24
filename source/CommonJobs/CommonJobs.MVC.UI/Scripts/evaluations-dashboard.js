$(document).ready(function () {
    var evaluationStates = ['En curso', 'Esperando Cal Empleado', 'Esperando Cal Responsable', 'Esperando Cal Empresa', 'Lista para devolución', 'Abierta para devolución', 'Finalizada'];

    var CalificatorsManager = function (data) {
        var self = this;
        this.evaluation = '';
        this.calificators = ko.observableArray();
        this.newCalificator = ko.observable();
        this.activeCalificators = ko.computed(function () {
            return _.filter(this.calificators(), function (e) {
                return e.action() !== 1;
            });
        }, this);
        if (data) {
            this.fromJs(data);
        }
        this.addCalificator = ko.computed(function () {
            var pepe = this.newCalificator();
        }, this);
        this.removeCalificator = function () {
            if (this.action() === 0) {
                self.calificators.remove(this);
            } else {
                this.action(1);
            }
        }
        this.cancel = function () {
            $('.content-modal').hide();
        }
        this.save = function () {
            var self = this;
            var updateCalificators = self.calificatorsManagerModel.toJs();
            $.ajax("/Evaluations/api/UpdateCalificators/", {
                type: "POST",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(updateCalificators),
                complete: function (response) {
                    debugger;
                }
            });
        }
    }

    CalificatorsManager.prototype.fromJs = function (data) {
        var self = this;
        this.evaluation = data.evaluation;
        this.calificators(_.map(data.calificators, function (e) {
            return new Calificator(e);
        }));
    }

    CalificatorsManager.prototype.toJs = function () {
        var calificators = _.map(this.calificators(), function (e) {
            return e.toJs();
        });
        var calificatorsFiltered = _.filter(calificators, function (e) {
            return e.Action !== '';
        });

        return {
            Evaluation: this.evaluation.toJs(),
            Calificators: calificatorsFiltered
        };
    }

    var Calificator = function (data) {
        this.userName = '';
        this.action = ko.observable();
        if (data) {
            this.fromJs(data);
        }
    }

    Calificator.prototype.fromJs = function (data) {
        this.userName = data;
        this.action('');
    }

    Calificator.prototype.add = function (data) {
        this.userName = data;
        this.action(0);
    }

    Calificator.prototype.remove = function () {
        this.action(1);
    }

    Calificator.prototype.toJs = function () {
        return {
            UserName: this.userName,
            Action: this.action()
        };
    }

    var Dashboard = function (data) {
        this.evaluations = ko.observableArray();
        this.calificatorsManagerModel = new CalificatorsManager();
        if (data) {
            this.fromJS(data);
        }
    }

    Dashboard.prototype.fromJS = function (data) {
        var self = this;
        this.evaluations(_.map(data.Evaluations, function (e) {
            return new Evaluation(e);
        }));
    }

    Dashboard.prototype.toJs = function () {
        return {
            Evaluations: _.map(this.evaluations(), function (e) {
                return e.toJs();
            })
        }
    }
    Dashboard.prototype.getEvaluationState = function (state) {
        return this.evaluationStates[state];
    }

    var Evaluation = function (data) {
        this.idResponsible = '';
        this.fullName = '';
        this.userName = '';
        this.period = '';
        this.currentPosition = '';
        this.seniority = '';
        this.evaluatorsAmount = '';
        this.evaluatorsString = '';
        this.state = '';
        this.currentState = '';
        this.evaluators = '';
        if (data) {
            this.fromJs(data);
        }
    }

    Evaluation.prototype.fromJs = function (data) {
        this.isResponsible = data.IsResponsible;
        this.fullName = data.FullName;
        this.userName = data.UserName;
        this.period = data.Period;
        this.currentPosition= data.CurrentPosition;
        this.seniority = data.Seniority;
        this.evaluators = data.Evaluators;
        this.evaluatorsString = ko.computed(function () {
            return this.evaluators.toString().replace(',', ', ');
        }, this);
        this.evaluatorsAmount = ko.computed(function () {
            return this.evaluators.length;
        }, this);
        this.state = data.State;
        this.stateName = evaluationStates[data.State];
        this.showCalificatorsManager = function () {
            viewmodel.calificatorsManagerModel.fromJs({ evaluation: this, calificators: this.evaluators });
            $('.content-modal').show();
        }
    }

    Evaluation.prototype.toJs = function () {
        return {
            IsResponsible: this.isResponsible,
            FullName: this.fullName,
            CurrentPosition: this.currentPosition,
            Seniority: this.seniority,
            Evaluators: this.evaluators,
            State: this.state,
            UserName: this.userName,
            Period: this.period
        };
    }

    var viewmodel = new Dashboard();

    getDashboardEvaluations();
    ko.applyBindings(viewmodel);
    commonSuggest($('.content-modal .search'), 'UserName');
    

    function getDashboardEvaluations() {
        $.getJSON("/Evaluations/api/getDashboardEvaluations/", function (model) {
            viewmodel.fromJS(model);
        });
    }

    /*$('#generate-evaluation-button').on('click', function () {
        var model = viewmodel.toJs();
        var modelFiltered = { Employees: _.filter(model.Employees, function (e) { return e.Responsible && !e.Period; }) };
        $.ajax("/Evaluations/api/GenerateEvalutions/" + evaluationPeriod + "/", {
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(modelFiltered),
            complete: function (response) {
                var modalContainer = $('#evaluations-generated-confirm');
                var countText = (response.responseText == '1') ? "Se ha generado 1 evaluación correctamente" : "Se han generado " + response.responseText + " evaluaciones correctamente";
                modalContainer.find('#textCount').text(countText);
                modalContainer.modal('show');
            }
        });
    });*/

    //var modalContainer = $('#evaluations-generated-confirm');
    //modalContainer.modal({ show: false });
    //modalContainer.find('.confirm').on('click', function () {
    //    modalContainer.modal('hide');
    //    getEmployeesToGenerateEvalution();
    //});
});