$(document).ready(function () {
    var viewmodel;

    var Calification = function (data) {
        var self = this;
        this.evaluation = new Evaluation();
        this.template = new Template();
        if (data) {
            this.fromJs(data);
        }
    }

    Calification.prototype.fromJs = function (data) {
        this.evaluation.fromJs(data.Evaluation);
        this.template.fromJs(data.Template);
    }

    Calification.prototype.toJs = function () {
        return {
            Evaluation: this.evaluation.toJs(),
            Template: this.template.toJs()
        };
    }
    Calification.prototype.load = function () {
        $.getJSON("/Evaluations/api/getEvaluation/", function (model) {
            viewmodel.fromJs(model);
        });
    }

    var Evaluation = function (data) {
        this.id = ko.observable('');
        this.userName = ko.observable('');
        this.responsibleId = ko.observable('');
        this.fullName = ko.observable('');
        this.currentPosition = ko.observable('');
        this.seniority = ko.observable('');
        this.period = ko.observable('');
        this.project = ko.observable('');
        this.updateProject = function () {
            var project = { Project: this.project() };
            $.ajax("/Evaluations/api/UpdateProject/", {
                type: "POST",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(project)
            });
        }
        if (data) {
            this.fromJs(data);
        }
    }

    Evaluation.prototype.fromJs = function (data) {
        this.id(data.Id);
        this.userName(data.UserName);
        this.responsibleId(data.ResponsibleId);
        this.fullName(data.FullName);
        this.currentPosition(data.CurrentPosition);
        this.seniority(data.Seniority);
        this.period(data.Period);
        this.project(data.Project);
    }

    Evaluation.prototype.toJs = function () {
        return {
            UserName: this.userName,
            ResponsibleId: this.responsibleId,
            CurrentPosition: this.currentPosition,
            Seniority: this.seniority,
            FullName: this.fullName,
            Project: this.project,
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
    viewmodel.load();
    ko.applyBindings(viewmodel);

    $('.icon').on('click', function () {
        var i = $(this).data('calificator-col');
        var className = "hide-column-" + (i + 1);
        $('.calification-items').toggleClass(className);
    });
    $('.editable-projects input')
        .on('focusin', function () {
            $('.editable-projects').addClass('edition-enabled');
        })
        .on('blur', function () {
            viewmodel.evaluation.updateProject();
            $('.editable-projects').removeClass('edition-enabled');
        }).on('keyup', function (e) {
            if (e.keyCode === 13) {
                $('.editable-projects input').blur();
            }
        });
});