$(document).ready(function () {
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
                show: ko.observable(true) //Hacer lógica
            }
        });

        var groupNames = {};
        for (var i in data.Template.Groups) {
            var item = data.Template.Groups[i];
            groupNames[item.Key] = item.Value;
        }

        var valuesByKeyCollection = _.map(data.Califications, function (calification) {
            var valuesByKey = {
                calificationColumn: { //ugly patch
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
                                    calificationId: valuesByKey.calificationColumn.calificationId, //ugly patch
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
        //this.evaluators = data.Evaluators;
        this.project(data.Project);
        this.strengthsComment(data.StrengthsComment);
        this.improveComment(data.ImproveComment);
        this.actionPlanComment(data.ActionPlanComment);
        this.evaluatorsString = this.evaluators.toString().replace(/,/g, ', ');
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


























    //var viewmodel;
    //var positionItem = 1;

    //var CalificationViewModel = function (data) {
    //    var self = this;
    //    this.evaluation = new Evaluation();
    //    this.template = new Template();
    //    this.califications = ko.observableArray();
    //    this.userView = ko.observable('');
    //    this.userLogged = ko.observable('');
    //    this.toggleVisiblityColumn = function (data, event) {
    //        var i = $(event.target).data('calificator-col');
    //        var className = "hide-column-" + (i + 1);
    //        $('.calification-items').toggleClass(className);
    //    }
    //    if (data) {
    //        this.fromJs(data);
    //    }
    //}

    //CalificationViewModel.prototype.fromJs = function (data) {
    //    this.userView(data.UserView);
    //    this.userLogged(data.UserLogged);
    //    this.evaluation.fromJs(data.Evaluation);
    //    this.califications(_.map(data.Califications, function (calification) {
    //        return new Calification(calification);
    //    }));
    //    this.template.fromJs(data.Template);
    //    this.getAllCalificationsByKey = function (key) {
    //        return _.map(this.califications(), function (calification) {
    //            return calification.getCalificationByKey(key);
    //        });
    //    }
    //    this.calificatorsHeaderTable = ko.computed(function () {
    //        return _.map(this.califications(), function (calification) {
    //            var userName = calification.evaluatorEmployee();
    //            if (calification.evaluatorEmployee() == calification.evaluatedEmployee()) {
    //                userName = "Auto Evaluación";
    //            }
    //            return {
    //                headerName: userName
    //            }
    //        });
    //    }, this);
    //}

    //CalificationViewModel.prototype.toJs = function () {
    //    return {
    //        Evaluation: this.evaluation.toJs(),
    //        Template: this.template.toJs()
    //    };
    //}
    //CalificationViewModel.prototype.load = function () {
    //    $.getJSON("/Evaluations/api/getEvaluation/" + calificationPeriod + "/" + calificationUserName + "/", function (model) {
    //        viewmodel.fromJs(model);
    //    });
    //}

    //var Calification = function (data) {
    //    this.id = ko.observable('');
    //    this.owner = ko.observable('');
    //    this.evaluationId = ko.observable('');
    //    this.period = ko.observable('');
    //    this.templateId = ko.observable('');
    //    this.evaluatedEmployee = ko.observable('');
    //    this.evaluatorEmployee = ko.observable('');
    //    this.comments = ko.observable('');
    //    this.califications = ko.observableArray();
    //    this.finished = ko.observable('');
    //    if (data) {
    //        this.fromJs(data);
    //    }
    //}

    //Calification.prototype.fromJs = function (data) {
    //    this.id(data.Id);
    //    this.owner(data.Owner);
    //    this.evaluationId(data.EvaluationId);
    //    this.period(data.Period);
    //    this.templateId(data.TemplateId);
    //    this.evaluatedEmployee(data.EvaluatedEmployee);
    //    this.evaluatorEmployee(data.EvaluatorEmployee);
    //    this.comments(data.Comments);
    //    if (data.Calification) {
    //        this.califications(_.map(data.Califications, function (item) {
    //            return {
    //                key: item[0],
    //                value: item[1]
    //            }
    //        }));
    //    }
    //}

    //Calification.prototype.getCalificationByKey = function (key) {
    //    var value = _.find(this.califications(), function (item) {
    //        return item.key == key;
    //    });
    //    return {
    //        evaluatorEmployee: this.evaluatorEmployee(),
    //        value: value || '',
    //        editable: this.isCalificationEditable()
    //    }
    //}

    //Calification.prototype.isCalificationEditable = function () {
    //    return viewmodel.userLogged() == this.evaluatorEmployee() && !this.finished();
    //};


    ////Calification.prototype.toJs = function (data) {
    ////    return {
    ////        Period: this.period,
    ////        Evaluated: this.evaluated,
    ////        Template: this.template,
    ////        Owner: this.owner,
    ////        EvaluationId: this.evaluationId,
    ////    }
    ////}

    //var Evaluation = function (data) {
    //    this.id = ko.observable('');
    //    this.userName = ko.observable('');
    //    this.responsibleId = ko.observable('');
    //    this.fullName = ko.observable('');
    //    this.currentPosition = ko.observable('');
    //    this.seniority = ko.observable('');
    //    this.period = ko.observable('');
    //    this.evaluators = ko.observable('');
    //    this.project = ko.observable('');
    //    this.strengthsComment = ko.observable('');
    //    this.improveComment = ko.observable('');
    //    this.actionPlanComment = ko.observable('');
    //    this.updateProject = function () {
    //        var project = { Project: this.project() };
    //        $.ajax("/Evaluations/api/UpdateProject/", {
    //            type: "POST",
    //            dataType: 'json',
    //            contentType: 'application/json; charset=utf-8',
    //            data: JSON.stringify(project)
    //        });
    //    }
    //    this.onFocusInProject = function (data, event) {
    //        $(event.target).parent().addClass('edition-enabled');
    //        return true;
    //    }
    //    this.onBlurProject = function (data, event) {
    //        this.evaluation.updateProject();
    //        $(event.target).parent().removeClass('edition-enabled');
    //        if (!this.evaluation.project())
    //            $(event.target).parent().removeAttr('data-tips');
    //        return true;
    //    }
    //    this.onKeyUpProject = function (data, event) {
    //        if (event.keyCode === 13) {
    //            $(event.target).blur();
    //        }
    //    }
    //    this.evaluatorsString = ko.computed(function () {
    //        return this.evaluators().toString().replace(/,/g, ', ');
    //    }, this);
    //    if (data) {
    //        this.fromJs(data);
    //    }
    //}

    //Evaluation.prototype.fromJs = function (data) {
    //    this.id(data.Id);
    //    this.userName(data.UserName);
    //    this.responsibleId(data.ResponsibleId);
    //    this.fullName(data.FullName);
    //    this.currentPosition(data.CurrentPosition);
    //    this.seniority(data.Seniority);
    //    this.period(data.Period);
    //    //this.evaluators(data.Evaluators);
    //    this.project(data.Project);
    //    this.strengthsComment(data.StrengthsComment);
    //    this.improveComment(data.ImproveComment);
    //    this.actionPlanComment(data.ActionPlanComment);
    //}

    //Evaluation.prototype.toJs = function () {
    //    return {
    //        UserName: this.userName,
    //        ResponsibleId: this.responsibleId,
    //        CurrentPosition: this.currentPosition,
    //        Seniority: this.seniority,
    //        FullName: this.fullName,
    //        Project: this.project,
    //        Period: this.period
    //    };
    //}

    //var Template = function (data) {
    //    this.items = ko.observableArray();
    //    this.groups = ko.observableArray();
    //    if (data) {
    //        this.fromJs(data);
    //    }
    //}

    //Template.prototype.getGroupByKey = function (key) {
    //    return _.find(this.groups(), function (e) {
    //        return (e.key == key) ? e.value : '';
    //    });
    //};
    
    //Template.prototype.fromJs = function (data) {
    //    this.groups(_.map(data.Groups, function (e) {
    //        return {
    //            key: e.Key,
    //            value: e.Value
    //        };
    //    }));
    //    this.groupedItems = ko.computed(function () {
    //        var groupedItems = _.groupBy(this.items(), function (item) {
    //            return item.groupKey();
    //        });
    //        var items = [];
    //        for (var key in groupedItems) {
    //            items.push({
    //                group: this.getGroupByKey(key),
    //                items: groupedItems[key]
    //            })
    //        }
    //        return items;
    //    }, this);
    //    this.items(_.map(data.Items, function (e) {
    //        return new TemplateItem(e);
    //    }));
    //}

    ////Template.prototype.toJs = function (data) {
    ////    return {
    ////        Items: _.map(this.items, function (e) {
    ////            return e.toJs();
    ////        })
    ////    }
    ////}

    //var TemplateItem = function(data) {
    //    this.groupKey = ko.observable('');
    //    this.key = ko.observable('');
    //    this.text = ko.observable('');
    //    this.description = ko.observable('');
    //    this.calificationsByItem = ko.observableArray();
    //    if (data) {
    //        this.fromJs(data);
    //    }
    //}

    //TemplateItem.prototype.fromJs = function (data) {
    //    this.groupKey(data.GroupKey);
    //    this.key(data.Key);
    //    this.text(positionItem + '- ' + data.Text);
    //    this.description(data.Description);
    //    this.calificationsByItem(viewmodel.getAllCalificationsByKey(this.key()));
    //    positionItem++;
    //}

    //TemplateItem.prototype.toJs = function (data) {
    //    return {
    //        GroupKey: this.groupKey,
    //        Key: this.key,
    //        Text: this.text,
    //        Description: this.description,
    //    }
    //}

    //viewmodel = new CalificationViewModel();
    //viewmodel.load();
    //ko.applyBindings(viewmodel);
});