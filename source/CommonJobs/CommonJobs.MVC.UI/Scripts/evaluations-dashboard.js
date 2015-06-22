$(document).ready(function () {
    var evaluationStates = ['En curso', 'Esperando Cal Empleado', 'Esperando Cal Responsable', 'Esperando Cal Empresa', 'Lista para devolución', 'Abierta para devolución', 'Finalizada'];
    var Dashboard = function (data) {
        this.evaluations = ko.observableArray();
        if (data) {
            this.fromJS(data);
        }
        showCalificatorsManager = function(){
            $('.content-modal').modal('show');
        }
        cancelCalificatorsManager = function () {
            $('.content-modal').modal('hide');
        }
        saveCalificatorsManager = function () {
            $('.content-modal').modal('hide');
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
        debugger;
        return this.evaluationStates[state];
    }

    var Evaluation = function (data) {
        this.idResponsible = '';
        this.fullName = '';
        this.currentPosition = '';
        this.seniority = '';
        this.evaluatorsAmount = '';
        this.state = '';
        this.currentState = '';
        this.calificators = ko.observableArray();
        if (data) {
            this.fromJs(data);
        }
    }

    Evaluation.prototype.fromJs = function (data) {
        this.isResponsible = data.IsResponsible;
        this.fullName = data.FullName;
        this.currentPosition= data.CurrentPosition;
        this.seniority = data.Seniority;
        this.calificators(data.Calificators);
        this.evaluatorsAmount = ko.computed(function () {
            return this.calificators().length;
        });
        this.state = data.State;
        this.stateName = evaluationStates[data.State];
    }

    Evaluation.prototype.toJs = function () {
        return {
            IsResponsible: this.isResponsible,
            FullName: this.fullName,
            CurrentPosition: this.currentPosition,
            Seniority: this.seniority,
            EvaluatorsAmount: this.evaluatorsAmount(),
            Calificators: this.calificators,
            State: this.state
        };
    }

    var viewmodel = new Dashboard();

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

    getDashboardEvaluations();
    ko.applyBindings(viewmodel);

    //var modalContainer = $('#evaluations-generated-confirm');
    //modalContainer.modal({ show: false });
    //modalContainer.find('.confirm').on('click', function () {
    //    modalContainer.modal('hide');
    //    getEmployeesToGenerateEvalution();
    //});
});