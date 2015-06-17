$(document).ready(function () {
    var viewmodel;

    var PeriodCreation = function (data) {
        this.employees = ko.observableArray();
        if (data) {
            this.fromJS(data);
        }
    }

    PeriodCreation.prototype.fromJS = function (data) {
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
        this.employeeName = '';
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
        this.employeeName = data.EmployeeName;
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
            EmployeeName: this.employeeName,
            Period: this.period
        };
    }

    $.getJSON("/Evaluations/api/getEmployeesToGenerateEvalution/" + evaluationPeriod + "/", function (model) {
        viewmodel = new PeriodCreation(model);
        ko.applyBindings(viewmodel);
        $('.evaluation-owner-field').each(function (index, elem) {
            commonSuggest(elem, 'UserName');
        });
    });

    $('button.generar-evaluacion').on('click', function () {
        var model = viewmodel.toJs();
        var modelFiltered = { Employees: _.filter(model.Employees, function (e) { return e.Responsible && !e.Period; }) };
        $.ajax("/Evaluations/api/GenerateEvalutions", {
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(modelFiltered)
        });
    });
});