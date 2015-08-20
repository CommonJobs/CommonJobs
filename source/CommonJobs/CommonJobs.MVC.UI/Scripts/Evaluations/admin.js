$(document).ready(function () {
    var PeriodCreation = function (data) {
        var self = this;
        this.items = ko.observableArray();
        this.generateButtonEnable = ko.observable(false);
        if (data) {
            this.fromJS(data);
        }
        this.headers = [
        { title: 'Empleado', sortable: true, sortPropertyName: 'fullName', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
        { title: 'Puesto', sortable: true, sortPropertyName: 'currentPosition', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
        { title: 'Seniority', sortable: true, sortPropertyName: 'seniority', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
        { title: 'Responsable', sortable: true, sortPropertyName: 'responsible', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) }
        ];
        this.sort = commonSort.bind(this);
        this.defaultSort = function () {
            this.sort(this.headers[0]);
        }
    }

    PeriodCreation.prototype.fromJS = function (data) {
        var self = this;
        this.generateButtonEnable(false);
        this.items.subscribe(function () {
            ko.utils.arrayForEach(self.items(), function (item) {
                item.responsible.subscribe(function (i) {
                    self.generateButtonEnable(i || _.some(self.items(), function (e) { return !!e.responsible() }));
                });
            });
        });
        this.items(_.map(data.Employees, function (e) {
            return new Employee(e);
        }));
    }
    PeriodCreation.prototype.toJs = function () {
        return {
            Employees: _.map(this.items(), function (e) {
                return e.toJs();
            })
        }
    }

    PeriodCreation.prototype.isValid = function () {
        return _.every(this.items(), function (e) { return e.isValid(); });
    }

    var Employee = function (data) {
        this.id = '';
        this.userName = '';
        this.responsible = ko.observable('');
        this.fullName = '';
        this.currentPosition = '';
        this.seniority = '';
        this.period = '';
        this.isValid = ko.observable('');
        if (data) {
            this.fromJs(data);
        }
    }

    Employee.prototype.fromJs = function (data) {
        this.id = data.Id;
        this.userName = data.UserName;
        this.responsible(data.ResponsibleId || '');
        this.fullName = data.FullName;
        this.currentPosition = data.CurrentPosition;
        this.seniority = data.Seniority || '';
        this.period = data.Period;
        this.isValid = function ()  {
            return this.responsible() != this.userName;
        }
    }

    Employee.prototype.toJs = function () {
        return {
            UserName: this.userName,
            ResponsibleId: this.responsible(),
            CurrentPosition: this.currentPosition,
            Seniority: this.seniority,
            FullName: this.fullName,
            Period: this.period,
            IsValid: this.isValid()
        };
    }
    var viewmodel = new PeriodCreation();

    $('#generate-evaluation-button').on('click', function () {
        if (viewmodel.isValid()) {
            postEvaluationsForGeneration({
                onSuccess: function (result) {
                    showEvaluationGenerationResult(true, result.amountGenerated);
                },
                onError: function () {
                    alert('Fallo interno. Por favor recargue la página.');
                }
            });
        } else {
            showEvaluationGenerationResult(false);
        }
    });

    function postEvaluationsForGeneration(callbacks) {
        var model = viewmodel.toJs();
        var modelFiltered = { Employees: _.filter(model.Employees, function (e) { return e.ResponsibleId && !e.Period; }) };
        $.ajax("/Evaluations/api/GenerateEvalutions/" + evaluationPeriod + "/", {
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(modelFiltered),
            complete: function (response) {
                var result = { amountGenerated: parseInt(response.responseText) };
                if (typeof callbacks.onSuccess === 'function') { callbacks.onSuccess(result); }
            },
            error: callbacks.onError
        });
    };

    function showEvaluationGenerationResult(wereGenerated, amountGenerated) {
        var modalContainer = $('.modal');
        modalContainer.find('.modal-title').text(wereGenerated ? "Evaluaciones generadas" : "Evaluaciones no generadas");
        var text = wereGenerated ? amountGenerated == '1' 
            ? "Se ha generado 1 evaluación correctamente" : "Se han generado " + amountGenerated + " evaluaciones correctamente"
            : "No se pudieron generar las evaluaciones, datos incorrectos!";
        modalContainer.find('.modal-text').text(text);
        modalContainer.find('.back').toggle(!wereGenerated); 
        modalContainer.find('.confirm').toggle(wereGenerated);
        modalContainer.modal('show');
    }

    function getEmployeesToGenerateEvalution() {
        $.getJSON("/Evaluations/api/getEmployeesToGenerateEvalution/" + evaluationPeriod + "/", function (model) {
            viewmodel.fromJS(model);
            viewmodel.defaultSort();
            $('.evaluation-owner-field').each(function (index, elem) {
                commonSuggest(elem, 'UserName');
            });
        });
    }

    getEmployeesToGenerateEvalution();
    ko.applyBindings(viewmodel);
    var modalContainer = $('#evaluations-generated-confirm');
    modalContainer.modal({ show: false });
    modalContainer.find('.confirm').on('click', function () {
        modalContainer.modal('hide');
        getEmployeesToGenerateEvalution();
    });
    modalContainer.find('.back').on('click', function () {
        modalContainer.modal('hide');
    });
});