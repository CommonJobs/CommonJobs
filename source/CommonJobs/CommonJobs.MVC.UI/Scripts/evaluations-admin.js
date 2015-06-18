$(document).ready(function () {
    var PeriodCreation = function (data) {
        this.employees = ko.observableArray();
        this.generateButtonEnable = ko.observable(false);
        if (data) {
            this.fromJS(data);
        }
    }

    PeriodCreation.prototype.fromJS = function (data) {
        var self = this;
        this.generateButtonEnable(false);
        this.employees.subscribe(function () {
            ko.utils.arrayForEach(self.employees(), function (item) {
                item.responsible.subscribe(function (i) {
                    self.generateButtonEnable(i || _.some(self.employees(), function (e) { return !!e.responsible() }));
                });
            });
        });
        this.employees(_.map(data.Employees, function (e) {
            return new Employee(e);
        }));
    }
    PeriodCreation.prototype.toJs = function () {
        return {
            Employees: _.map(this.employees(), function (e) {
                return e.toJs();
            })
        }
    }

    var Employee = function (data) {
        this.id = '';
        this.userName = '';
        this.responsible = ko.observable('');
        this.fullName = '';
        this.currentPosition = '';
        this.seniority = '';
        this.period = '';
        if (data) {
            this.fromJs(data);
        }
    }

    Employee.prototype.fromJs = function (data) {
        this.id = data.Id;
        this.userName = data.UserName;
        this.responsible(data.Responsible || '');
        this.fullName = data.FullName;
        this.currentPosition = data.CurrentPosition;
        this.seniority = data.Seniority;
        this.period = data.Period;
    }

    Employee.prototype.toJs = function () {
        return {
            UserName: this.userName,
            Responsible: this.responsible(),
            CurrentPosition: this.currentPosition,
            Seniority: this.seniority,
            FullName: this.fullName,
            Period: this.period
        };
    }

    var viewmodel = new PeriodCreation();

    function getEmployeesToGenerateEvalution() {
        $.getJSON("/Evaluations/api/getEmployeesToGenerateEvalution/" + evaluationPeriod + "/", function (model) {
            viewmodel.fromJS(model);
            $('.evaluation-owner-field').each(function (index, elem) {
                commonSuggest(elem, 'UserName');
            });
        });
    }

    $('#generate-evaluation-button').on('click', function () {
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
                modalContainer.children().find('#count').text(countText);
                modalContainer.modal('show');
            }
        });
    });

    getEmployeesToGenerateEvalution();
    ko.applyBindings(viewmodel);

    var modalContainer = $('#evaluations-generated-confirm');
    modalContainer.modal({ show: false });
    modalContainer.find('.confirm').on('click', function () {
        modalContainer.modal('hide');
        getEmployeesToGenerateEvalution();
    });
});