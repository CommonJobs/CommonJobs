﻿$(document).ready(function () {
    var viewmodel;

    var EvaluationViewModel = function (data) {
        this.userView = '';
        this.userLogged = '';
        this.evaluation = new Evaluation();
        this.califications = [];
        this.groups = [];
        if (data) {
            this.fromJs(data);
        }
    }

    EvaluationViewModel.prototype.load = function () {
        $.getJSON("/Evaluations/api/getEvaluation/" + calificationPeriod + "/" + calificationUserName + "/", function (model) {
            viewmodel.fromJs(model);
            ko.applyBindings(viewmodel);
        });
    }

    EvaluationViewModel.prototype.isDirty = dirtyFlag();

    EvaluationViewModel.prototype.onSave = function () {
    }

    EvaluationViewModel.prototype.onFinish = function () {
    }
    
    EvaluationViewModel.prototype.isValueEditable = function (calification) {
        return this.userLogged == calification.calificationColumn.evaluatorEmployee && !calification.calificationColumn.finished;
    }

    EvaluationViewModel.prototype.fromJs = function (data) {
        var self = this;
        this.userView = data.UserView;
        this.userLogged = data.UserLogged;
        this.evaluation.fromJs(data.Evaluation);

        this.califications = _.map(data.Califications, function (calification) {
            return {
                id: calification.Id,
                owner: calification.Owner,
                evaluatorEmployee: calification.EvaluatorEmployee,
                comments: ko.observable(calification.Comments),
                finished: calification.Finished,
                show: ko.observable(true) //TODO: we need a logic to know if the calification column is visible
            }
        });

        var groupNames = {};
        for (var i in data.Template.Groups) {
            var item = data.Template.Groups[i];
            groupNames[item.Key] = item.Value;
        }

        var valuesByKeyCollection = _.map(data.Califications, function (calification) {
            var valuesByKey = {
                calificationColumn: {
                    calificationId: calification.Id,
                    evaluatorEmployee: calification.EvaluatorEmployee,
                    finished: calification.Finished
                }
            };
            if (calification.Califications) {
                for (var i in calification.Califications) {
                    var cal = calification.Califications[i];
                    valuesByKey[cal.Key] = cal.Value;
                }
            }
            
            return valuesByKey;
        });

        this.groups =_(_(_(data.Template.Items)
            .groupBy(function (item) {
                return item.GroupKey;
            })).map(function (items, key) {
                var result = {
                    groupKey: key,
                    name: groupNames[key],
                    items: _.map(items, function (item) {
                        return {
                            key: item.Key,
                            text: item.Text,
                            description: item.Description,
                            values: _.map(valuesByKeyCollection, function (valuesByKey) {
                                var valueItem = {
                                    calificationId: valuesByKey.calificationColumn.calificationId,
                                    value: ko.observable(valuesByKey[item.Key] || ""),
                                    editable: self.isValueEditable(valuesByKey),
                                    showValue: _.find(self.califications, function (calification) {
                                            return calification.id == valuesByKey.calificationColumn.calificationId;
                                        }).show
                                };
                                self.isDirty.register(valueItem.value);
                                return valueItem;
                            })
                        };
                    })
                }
                result.averages = ko.computed(function () {
                    var averages = []
                    for (var i = 0; i < result.items[0].values.length; i++) {
                        for (var key in result.items) {
                            if (!averages[i]) {
                                averages[i] = {
                                    count: 0,
                                    total: 0,
                                    show: self.califications[i].show
                                };
                            }
                            value = parseFloat(result.items[key].values[i].value());
                            if (value) {
                                averages[i].count++;
                                averages[i].total += value;
                            }
                        }
                    }
                    return _.map(averages, function (column) {
                        return {
                            value: (column.total) ? (column.total / column.count).toFixed(1) : 0,
                            show: column.show
                        };
                    });
                });
                return result;
            }))
            .value();

        this.calificationsAverages = ko.computed(function () {
            var averages = []
            for (var i = 0; i < self.califications.length; i++) {
                for (var key in self.groups) {
                    if (!averages[i]) {
                        averages[i] = {
                            count: 0,
                            total: 0,
                            show: self.califications[i].show
                        };
                    }
                    value = parseFloat(self.groups[key].averages()[i].value);
                    if (value) {
                        averages[i].count++;
                        averages[i].total += value;
                    }
                }
            }
            return _.map(averages, function (column) {
                return {
                    value: (column.total) ? (column.total / column.count).toFixed(1) : 0,
                    show: column.show
                };
            });
        });
    };

    EvaluationViewModel.prototype.toDto = function (data) {
        return {
            EvalutionId: this.evaluation.evaluationId,
            Project: this.evaluation.project(),
            ToImprove: this.evaluation.toImproveComment(),
            Strengths: this.evaluation.StrengthsComment(),
            ActionPlan: this.evaluation.ActionPlanComment(),
            Califications: _.map(this.califications, function(calification) {
                return _(this.groups)
                    .map(function(group) {
                        return _.map(group.items, function(item) {
                            var value = _.findWhere(item.values, { calificationId: calification.calificationId });
                            return {
                                Key: item.key,
                                Value: value.value()
                            }
                        });
                    })
                    .flatten()
                    .value();
            })
        };
    }

    EvaluationViewModel.prototype.toggleVisiblityColumn = function (data, event) {
        data.show(!data.show());
    }

    var Evaluation = function (data) {
        this.id = '';
        this.userName = '';
        this.responsibleId = '';
        this.fullName = '';
        this.currentPosition = '';
        this.seniority = '';
        this.period = '';
        this.evaluators = '';
        this.project = ko.observable('');
        this.strengthsComment = ko.observable('');
        this.improveComment = ko.observable('');
        this.actionPlanComment = ko.observable('');
        if (data) {
            this.fromJs(data);
        }
    }
    
    Evaluation.prototype.fromJs = function (data) {
        this.id = data.Id;
        this.userName = data.UserName;
        this.responsibleId = data.ResponsibleId;
        this.fullName = data.FullName;
        this.currentPosition = data.CurrentPosition;
        this.seniority = data.Seniority;
        this.period = data.Period;
        this.evaluators = data.Evaluators;
        this.project(data.Project);
        this.strengthsComment(data.StrengthsComment);
        this.improveComment(data.ImproveComment);
        this.actionPlanComment(data.ActionPlanComment);
        this.evaluatorsString = (this.evaluators) ? this.evaluators.toString().replace(/,/g, ', ') : '';
    }

    Evaluation.prototype.onFocusInProject = function (data, event) {
        $(event.target).parent().addClass('edition-enabled');
        return true;
    }
    Evaluation.prototype.onBlurProject = function (data, event) {
        $(event.target).parent().removeClass('edition-enabled');
        if (!this.evaluation.project())
            $(event.target).parent().removeAttr('data-tips');
        return true;
    }
    Evaluation.prototype.onKeyUpProject = function (data, event) {
        if (event.keyCode === 13) {
            $(event.target).blur();
        }
    }

    viewmodel = new EvaluationViewModel();
    viewmodel.load();
});