$(document).ready(function () {
    var viewmodel;

    var Calification = function (data) {
        var self = this;
        this.employee = new Employee();
        this.template = new Template();
        if (data) {
            this.fromJs(data);
        }
    }

    Calification.prototype.fromJs = function (data) {
        this.employee.fromJs(data.Employee);
        this.template.fromJs(data.Template);
    }

    Calification.prototype.toJs = function () {
        return {
            Employee: this.employee.toJs(),
            Template: this.template.toJs()
        };
    }
    Calification.prototype.load = function () {
        $.getJSON("/Evaluations/api/getEvaluation/", function (model) {
            viewmodel.fromJs(model);
        });
    }

    var Employee = function (data) {
        this.id = ko.observable('');
        this.userName = ko.observable('');
        this.responsibleId = ko.observable('');
        this.fullName = ko.observable('');
        this.currentPosition = ko.observable('');
        this.seniority = ko.observable('');
        this.period = ko.observable('');
        if (data) {
            this.fromJs(data);
        }
    }

    Employee.prototype.fromJs = function (data) {
        this.id(data.Id);
        this.userName(data.UserName);
        this.responsibleId(data.ResponsibleId);
        this.fullName(data.FullName);
        this.currentPosition(data.CurrentPosition);
        this.seniority(data.Seniority);
        this.period(data.Period);
    }

    Employee.prototype.toJs = function () {
        return {
            UserName: this.userName,
            ResponsibleId: this.responsibleId,
            CurrentPosition: this.currentPosition,
            Seniority: this.seniority,
            FullName: this.fullName,
            Period: this.period
        };
    }

    var Template = function (data) {
        this.items = ko.observableArray();
        if (data) {
            this.fromJs(data);
        }
    }

    Template.prototype.fromJs = function (data) {
        this.items( _.map(data.Items, function (e) {
            return new TemplateItem(e);
        }));
    }

    Template.prototype.toJs = function (data) {
        return {
            Items: _.map(this.items, function (e) {
                return e.toJs();
            })
        }
    }

    var TemplateItem = function(data) {
        this.groupKey = ko.observable('');
        this.key = ko.observable('');
        this.text = ko.observable('');
        this.description = ko.observable('');
        if (data) {
            this.fromJs(data);
        }
    }

    TemplateItem.prototype.fromJs = function (data) {
        this.groupKey(data.GroupKey);
        this.key(data.Key);
        this.text(data.Text);
        this.description(data.Description);
    }

    TemplateItem.prototype.toJs = function (data) {
        return {
            GroupKey: this.groupKey,
            Key: this.key,
            Text: this.text,
            Description: this.description,
        }
    }

    viewmodel = new Calification();
    ko.applyBindings(viewmodel);
    viewmodel.load();
});