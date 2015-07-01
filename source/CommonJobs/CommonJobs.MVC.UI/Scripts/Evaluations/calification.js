$(document).ready(function () {
    var viewmodel;

    var CalificationViewModel = function (data) {
        var self = this;
        this.evaluation = new Evaluation();
        this.template = new Template();
        this.califications = ko.observableArray();
        this.userView = ko.observable('');
        this.userLogged = ko.observable('');
        this.toggleVisiblityColumn = function (data, event) {
            var i = $(event.target).data('calificator-col');
            var className = "hide-column-" + (i + 1);
            $('.calification-items').toggleClass(className);
        }
        if (data) {
            this.fromJs(data);
        }
        this.getAllCalificationsByKey = function (key) {
            return _.map(this.califications(), function (calification) {
                return calification.getCalificationByKey(key);
            });
        }
        this.calificatorsHeaderTable = ko.computed(function () {
            return _.map(this.califications(), function (calification) {
                var userName = calification.evaluatorEmployee();
                if (calification.evaluatorEmployee() == calification.evaluatedEmployee()) {
                    userName = "Auto Evaluación";
                }
                return {
                    headerName: userName
                }
            });
        }, this);
    }

    CalificationViewModel.prototype.fromJs = function (data) {
        this.userView(data.UserView);
        this.userLogged(data.UserLogged);
        this.evaluation.fromJs(data.Evaluation);
        this.califications(_.map(data.Califications, function (calification) {
            return new Calification(calification);
        }));
        this.template.fromJs(data.Template);
    }

    CalificationViewModel.prototype.toJs = function () {
        return {
            Evaluation: this.evaluation.toJs(),
            Template: this.template.toJs()
        };
    }
    CalificationViewModel.prototype.load = function () {
        $.getJSON("/Evaluations/api/getEvaluation/" + calificationPeriod + "/" + calificationUserName + "/", function (model) {
            viewmodel.fromJs(model);
        });
    }

    var Calification = function (data) {
        this.id = ko.observable('');
        this.owner = ko.observable('');
        this.evaluationId = ko.observable('');
        this.period = ko.observable('');
        this.templateId = ko.observable('');
        this.evaluatedEmployee = ko.observable('');
        this.evaluatorEmployee = ko.observable('');
        this.comments = ko.observable('');
        this.califications = ko.observableArray();
        this.finished = ko.observable('');
        if (data) {
            this.fromJs(data);
        }
    }

    Calification.prototype.fromJs = function (data) {
        this.id(data.Id);
        this.owner(data.Owner);
        this.evaluationId(data.EvaluationId);
        this.period(data.Period);
        this.templateId(data.TemplateId);
        this.evaluatedEmployee(data.EvaluatedEmployee);
        this.evaluatorEmployee(data.EvaluatorEmployee);
        this.comments(data.Comments);
        if (data.Calification) {
            this.califications(_.map(data.Califications, function (item) {
                return {
                    key: item[0],
                    value: item[1]
                }
            }));
        }
    }

    Calification.prototype.getCalificationByKey = function (key) {
        var value = _.find(this.califications(), function (item) {
            return item.key == key;
        });
        return {
            evaluatorEmployee: this.evaluatorEmployee(),
            value: value || ''
        }
    }


    //Calification.prototype.toJs = function (data) {
    //    return {
    //        Period: this.period,
    //        Evaluated: this.evaluated,
    //        Template: this.template,
    //        Owner: this.owner,
    //        EvaluationId: this.evaluationId,
    //    }
    //}

    var Evaluation = function (data) {
        this.id = ko.observable('');
        this.userName = ko.observable('');
        this.responsibleId = ko.observable('');
        this.fullName = ko.observable('');
        this.currentPosition = ko.observable('');
        this.seniority = ko.observable('');
        this.period = ko.observable('');
        this.evaluators = ko.observable('');
        this.project = ko.observable('');
        this.strengthsComment = ko.observable('');
        this.improveComment = ko.observable('');
        this.actionPlanComment = ko.observable('');
        this.updateProject = function () {
            var project = { Project: this.project() };
            $.ajax("/Evaluations/api/UpdateProject/", {
                type: "POST",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(project)
            });
        }
        this.onFocusInProject = function (data, event) {
            $(event.target).parent().addClass('edition-enabled');
            return true;
        }
        this.onBlurProject = function (data, event) {
            this.evaluation.updateProject();
            $(event.target).parent().removeClass('edition-enabled');
            if (!this.evaluation.project())
                $(event.target).parent().removeAttr('data-tips');
            return true;
        }
        this.onKeyUpProject = function (data, event) {
            if (event.keyCode === 13) {
                $(event.target).blur();
            }
        }
        this.evaluatorsString = ko.computed(function () {
            return this.evaluators().toString().replace(/,/g, ', ');
        }, this);
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
        //this.evaluators(data.Evaluators);
        this.project(data.Project);
        this.strengthsComment(data.StrengthsComment);
        this.improveComment(data.ImproveComment);
        this.actionPlanComment(data.ActionPlanComment);
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
        //this.groupedItems = ko.computed(function () {
        //    return _.groupBy(this.items(), function (e) {
        //        return e.groupKey();
        //    })
        //}, this);
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
        this.calificationsByItem = ko.observableArray();
        if (data) {
            this.fromJs(data);
        }
    }

    TemplateItem.prototype.fromJs = function (data) {
        this.groupKey(data.GroupKey);
        this.key(data.Key);
        this.text(data.Text);
        this.description(data.Description);
        this.calificationsByItem(viewmodel.getAllCalificationsByKey(this.key()));
    }

    TemplateItem.prototype.toJs = function (data) {
        return {
            GroupKey: this.groupKey,
            Key: this.key,
            Text: this.text,
            Description: this.description,
        }
    }

    viewmodel = new CalificationViewModel();
    viewmodel.load();
    ko.applyBindings(viewmodel);
});