$(document).ready(function () {
    var viewmodel;
    var modalViewModel;
    var layoutViewModel;

    $('#header-fixed').scrollToFixed({
        marginTop: 90,
        fixed: function () {
            $(this).css('display', 'table');
        },
        unfixed: function () {
            $(this).css('display', 'table-header-group');
        }
    });

    var sortCalificationColumns = function (a, b) {
        if (a.Owner == b.Owner)
            return 0;
        if ((a.Owner == 0 && b.Owner > 0) ||
            (a.Owner == 1 && b.Owner == 3) ||
            (a.Owner == 2 && b.Owner == 1) ||
            (a.Owner == 2 && b.Owner == 3))
            return -1;
        return 1;
    }

    var EvaluationViewModel = function (data) {
        this.userView = '';
        this.userLogged = '';
        this.evaluation = new Evaluation();
        this.califications = [];
        this.groups = [];
        this.isEvaluationEditable = ko.observable(false);
        this.isLoading = ko.observable(false);
    }

    EvaluationViewModel.prototype.load = function () {
        viewmodel.isLoading(true);
        $.getJSON("/Evaluations/api/getEvaluation/" + calificationPeriod + "/" + calificationUserName + "/", function (model) {
            viewmodel.fromJs(model);
            ko.applyBindings(viewmodel, document.getElementById('evaluation-view'));
        })
        .fail(function () {
            alert('Fallo interno. Por favor recargue la página.');
        })
        .always(function () {
            viewmodel.isLoading(false);
        });
    }

    EvaluationViewModel.prototype.isDirty = dirtyFlag();

    EvaluationViewModel.prototype.isValid = function () {
        return !_.some(this.groups, function (group) {
            return _.some(group.items, function (item) {
                return _.some(item.values, function (value) {
                    return value.editable && !value.isValid()
                });
            });
        });
    }

    EvaluationViewModel.prototype.hasEmptyValues = function () {
        return _.some(this.groups, function (group) {
            return _.some(group.items, function (item) {
                return _.some(item.values, function (value) {
                    return value.editable && value.value() === ""
                });
            });
        });
    }

    EvaluationViewModel.prototype.onSave = function () {
        var self = this;
        if (this.calificationFinished || this.evaluationFinished || this.isValid()) {
            viewmodel.isLoading(true);
            var dto = this.toDto();
            $.ajax("/Evaluations/api/SaveEvaluationCalifications/", {
                type: "POST",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(dto),
                success: function (response) {
                    self.isDirty(false);
                    if (self.calificationFinished || self.evaluationFinished) {
                        if (self.userView == 0) {
                            window.location = urlGenerator.action("Index", "Home");
                        } else {
                            window.location = urlGenerator.action(calificationPeriod, "Evaluations");
                        }
                    }
                }
            })
            .fail(function () {
                alert('Fallo interno. Por favor recargue la página.');
            })
            .always(function () {
                viewmodel.isLoading(false);
            });
        } else {
            modalViewModel.showInvalidModal();
        }
    }

    EvaluationViewModel.prototype.onFinish = function () {
        if (!this.isValid()) {
            modalViewModel.showInvalidModal();
        } else if (this.hasEmptyValues()) {
            modalViewModel.showConfirmationModal();
        } else {
            modalViewModel.showFinishModal();
        }

    }

    EvaluationViewModel.prototype.isValueEditable = function (calification) {
        var isCompanyView = this.userView == 3;
        var isCompanyColumn = calification.calificationColumn.owner == 3;
        var isAutoEvaluationColumn = calification.calificationColumn.owner == 0;
        var isCalificationOwnerLogged = this.userLogged == calification.calificationColumn.evaluatorEmployee;
        var isCalificationFinished = calification.calificationColumn.finished;

        // Responsible editing Company califications when devolution in progress
        if (!this.evaluation.finished && this.evaluation.devolutionInProgress && isCompanyView && isCompanyColumn) {
            return true;
        }
        // Evaluated Employee editing its AutoEvaluation when devolution in progress
        else if (!this.evaluation.finished && this.evaluation.devolutionInProgress && isCompanyView && isAutoEvaluationColumn) {
            return true;
        }
        // Cafication Owner editing its calification while Company calification not finished
        else if (!this.evaluation.finished && !this.evaluation.devolutionInProgress && !isCalificationFinished && isCalificationOwnerLogged && !this.isCompanyEvaluationDone) {
            return true;
        }
        // Responsible editing Company califications
        else if (!this.evaluation.finished && !this.evaluation.devolutionInProgress && !isCalificationFinished && isCompanyView && isCompanyColumn) {
            return true;
        }
        else {
            return false;
        }
    }

    EvaluationViewModel.prototype.fromJs = function (data) {
        var self = this;
        this.userView = data.UserView;
        this.userLogged = data.UserLogged;
        this.hasAverageColumn = !data.Evaluation.Finished && (this.userView == 1 || this.userView == 3);
        this.columnsAmount = this.hasAverageColumn ? data.Califications.length + 3 : data.Califications.length + 2
        this.numberOfColumns = "table-" + this.columnsAmount + "-columns";
        var calificationsSorted = data.Califications.sort(sortCalificationColumns);
        this.califications = _.map(calificationsSorted, function (calification) {
            var comment = ko.observable(calification.Comments);
            self.isDirty.register(comment);
            if (calification.Owner == 3 && (!calification.Califications || !calification.Califications.length)) {
                self.isCompanyCalificationsEmpty = true;
            }
            var show = self.userView == 0 || (self.userView != 0 && calification.Owner != 0);
            var hasShowIcon = calification.Finished || (calification.EvaluatorEmployee != self.userLogged && calification.Owner != self.userView);
            return {
                id: calification.Id,
                owner: calification.Owner,
                evaluatorEmployee: calification.EvaluatorEmployee,
                comments: comment,
                finished: calification.Finished,
                show: ko.observable(show),
                hasShowIcon: hasShowIcon
            }
        });

        this.isResposibleCalificating = _.some(self.califications, function (califications) {
            return califications.owner == 1 && !califications.finished && califications.evaluatorEmployee == data.UserLogged
        });

        this.evaluation.fromJs(data.Evaluation);

        if (this.evaluation.devolutionInProgress && this.userView == 3) {
            this.hasAverageColumn = false;
            _.chain(self.califications)
            .map(function (calification) {
                if (calification.owner == 1 || calification.owner == 2) {
                    calification.hasShowIcon = true;
                    calification.show(false);
                } else {
                    calification.hasShowIcon = false;
                    calification.show(true);
                }
            });
        }

        this.isEvaluationEditable(!this.evaluation.finished &&
             (this.evaluation.devolutionInProgress && this.userView == 3) ||
             (_.some(this.califications, function (calification) {
                 return calification.owner == self.userView && !calification.finished;
             }) && (!this.evaluation.devolutionInProgress && !this.isCompanyEvaluationDone))
        );

        var userLoggedCalifiction = _.find(self.califications, function (calification) {
            return (calification.owner == 3 && (self.userView == 3 || (self.userView == 0 && self.evaluation.devolutionInProgress)))
                || (self.userView != 3 && !(self.userView == 0 && self.evaluation.devolutionInProgress) && calification.evaluatorEmployee == self.userLogged);
        })

        var userLoggedEvaluated = _.find(self.califications, function (calification) {
            return calification.owner == 0;
        })

        this.evaluatedComment = (userLoggedEvaluated) ? userLoggedEvaluated.comments : ko.observable('');

        this.generalComment = (userLoggedCalifiction) ? userLoggedCalifiction.comments : ko.observable('');

        if (this.evaluation.devolutionInProgress && this.userView == 2) {
            var comments = _.chain(self.califications)
            .filter(function (calification) {
                return calification.owner == 3 && calification.comments() != null;
            })
            .map(function (comment) {
                return comment.comments();
            })
            .value();
            this.generalComment(comments);
        }

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
                    finished: calification.Finished,
                    owner: calification.Owner
                }
            };
            if (calification.Califications) {
                for (var i in calification.Califications) {
                    var cal = calification.Califications[i];
                    valuesByKey[cal.Key] = cal.Value && parseFloat(cal.Value.toFixed(1));
                }
            }

            return valuesByKey;
        });

        var commentsByKeyCollection = _.map(data.Califications, function (calification) {
            var commentsByKey = {
                commentRow: {
                    calificationId: calification.Id,
                    evaluatorEmployee: calification.EvaluatorEmployee,
                    owner: calification.Owner
                }
            };
            if (calification.Califications) {
                for (var calif in calification.Califications) {
                    var cal = calification.Califications[calif];
                    commentsByKey[cal.Key] = cal.Comment;
                }
            }
            return commentsByKey
        });

        if (this.hasAverageColumn) {
            this.averageCalificationId = "average_column";
            var averageCalificationsColumn = {
                id: this.averageCalificationId,
                owner: 4,
                evaluatorEmployee: "promedio",
                comments: ko.observable(''),
                finished: false,
                show: ko.observable(true),
                hasShowIcon: true
            }

            this.califications.splice(1, 0, averageCalificationsColumn);
        }
        var itemNumber = 0;
        this.groups = _.chain(data.Template.Items)
            .groupBy(function (item) {
                return item.GroupKey;
            })
            .map(function (items, key) {
                var result = {
                    groupKey: key,
                    name: groupNames[key],
                    items: _.map(items, function (item) {
                        itemNumber++;
                        var rowSelected = ko.observable(false);
                        var valuesByItem = {
                            key: item.Key,
                            text: itemNumber + " - " + item.Text,
                            description: item.Description,
                            isRowSelected: rowSelected,
                            addSelfComment: function (data, event) {
                                var selfComment = _.find(this.comments, function (comment) {
                                    return comment.evaluatorEmployee == self.userLogged
                                });
                                selfComment.value("");
                                selfComment.IsEditingComment(true);
                            },
                            values: _.map(valuesByKeyCollection, function (valuesByKey) {
                                var valueItem = {
                                    calificationId: valuesByKey.calificationColumn.calificationId,
                                    value: ko.observable(valuesByKey[item.Key] || ""),
                                    editable: self.isValueEditable(valuesByKey),
                                    isSelected: rowSelected,
                                    owner: valuesByKey.calificationColumn.owner,
                                    showValue: _.find(self.califications, function (calification) {
                                        return calification.id == valuesByKey.calificationColumn.calificationId;
                                    }).show,
                                    onBlur: function (data, event) {
                                        data.isSelected(false);
                                    },
                                    onFocus: function (data, event) {
                                        data.isSelected(true);
                                    }
                                };
                                valueItem.isValid = ko.computed(function () {
                                    return valueItem.value() === "" || (valueItem.value() >= 1 && valueItem.value() <= 4);
                                })
                                self.isDirty.register(valueItem.value);
                                return valueItem;
                            }),
                            comments: _.map(commentsByKeyCollection, function (commentsByKey) {
                                var commentItem = GetCommentItem(commentsByKey.commentRow.calificationId, commentsByKey[item.Key], commentsByKey.commentRow.evaluatorEmployee, commentsByKey.commentRow.evaluatorEmployee == self.userLogged)

                                commentItem.IsEditingComment = ko.observable(false);
                                self.isDirty.register(commentItem.value);

                                return commentItem;
                            }),
                            showComments: ko.observable(false),
                        };

                        if (self.hasAverageColumn) {
                            var averageValue = {
                                calificationId: self.averageCalificationId,
                                value: ko.computed(function () {
                                    var count, total;
                                    var values = _.filter(valuesByItem.values, function (value) {
                                        return value.owner == 1 || value.owner == 2;
                                    });
                                    for (var i in values) {
                                        if (i == 0) {
                                            count = 0;
                                            total = 0;
                                        }
                                        var value = parseFloat(values[i].value());
                                        if (value) {
                                            count++;
                                            total += value;
                                        }
                                    }
                                    if (total) {
                                        return parseFloat((total / count).toFixed(1));
                                    }
                                    return 0;
                                }),
                                editable: false,
                                showValue: _.find(self.califications, function (calification) {
                                    return calification.id == self.averageCalificationId;
                                }).show,
                            };
                            averageValue.isValid = true;

                            valuesByItem.values.splice(1, 0, averageValue);

                            if (self.isCompanyCalificationsEmpty) {
                                var companyCalification = _.find(valuesByItem.values, function (calification) {
                                    return calification.owner == 3;
                                });

                                if (companyCalification) {
                                    companyCalification.value(averageValue.value());
                                }
                            }
                        }
                        valuesByItem.hasSelfComments = ko.computed(function () {
                            return _.some(valuesByItem.comments, function (comment) {
                                return comment.HasComment() && comment.isSelfComment;
                            });
                        });
                        return valuesByItem;
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
                            value: (column.total) ? parseFloat((column.total / column.count).toFixed(1)) : 0,
                            show: column.show
                        };
                    });
                });
                return result;
            })
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
                    value: (column.total) ? parseFloat((column.total / column.count).toFixed(1)) : 0,
                    show: column.show
                };
            });
        });
    };

    EvaluationViewModel.prototype.toDto = function (data) {
        var self = this;
        return {
            EvaluationFinished: this.evaluationFinished,
            CalificationFinished: this.calificationFinished,
            EvaluationId: this.evaluation.id,
            Project: this.evaluation.project(),
            ToImprove: this.evaluation.improveComment(),
            Strengths: this.evaluation.strengthsComment(),
            ActionPlan: this.evaluation.actionPlanComment(),
            Califications: _.chain(self.califications)
                .filter(function (calification) {
                    return calification.owner == self.userView || (self.evaluation.devolutionInProgress && calification.owner == 0);
                })
                .map(function (calification) {
                    var calificationItems = _.chain(self.groups)
                        .map(function (group) {
                            var itemsList = _.map(group.items, function (item) {
                                var ownerValue = _.find(item.values, function (element) {
                                    return element.owner == calification.owner;
                                });
                                var ownerComment = _.find(item.comments, function (element) {
                                    return element.evaluatorEmployee == calification.evaluatorEmployee;
                                });

                                if ((ownerValue && ownerValue.value()) || (ownerComment && ownerComment.value())) {
                                    return {
                                        Key: item.key.toString(),
                                        Value: parseFloat(ownerValue && ownerValue.value()),
                                        Comment: ownerComment && ownerComment.value()
                                    }
                                }
                                return;
                            });
                            return _.filter(itemsList, function (item) { return item });
                        })
                        .flatten()
                        .value();
                    return {
                        CalificationId: calification.id,
                        Items: calificationItems,
                        Comments: calification.comments()
                    }
                })
                .value()
        }
    }

    EvaluationViewModel.prototype.toggleVisibilityColumn = function (data, event) {
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
        this.finished = false;
        this.devolutionInProgress = false;
        this.project = ko.observable('');
        this.strengthsComment = ko.observable('');
        this.improveComment = ko.observable('');
        this.actionPlanComment = ko.observable('');
        this.isCompanyEvaluationDone = false;
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
        this.finished = data.Finished;
        this.devolutionInProgress = data.DevolutionInProgress;
        this.project(data.Project);
        this.strengthsComment(data.StrengthsComment);
        this.improveComment(data.ToImproveComment);
        this.actionPlanComment(data.ActionPlanComment);
        this.isCompanyEvaluationDone = data.IsCompanyEvaluationDone;
        this.evaluators = data.Evaluators.join(', ');
        viewmodel.isDirty.register(this.project);
        viewmodel.isDirty.register(this.strengthsComment);
        viewmodel.isDirty.register(this.improveComment);
        viewmodel.isDirty.register(this.actionPlanComment);
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

    var GetCommentItem = function (calificationId, comment, evaluatorEmployee, isSelfComment) {
        var commentItem = {
            calificationId: calificationId,
            value: ko.observable(comment),
            evaluatorEmployee: evaluatorEmployee,
            isSelfComment: isSelfComment,
            endEdition: function (data, event) {
                data.IsEditingComment(false);
                if (data.value() == "") {
                    data.value(null)
                }
            }
        };
        commentItem.IsCalificationShown = ko.computed(function () {
            return viewmodel.califications.find(function (calification) {
                return calification.id == calificationId
            }).show;
        });
        commentItem.HasComment = ko.computed(function () {
            return commentItem.value() != null;
        });
        commentItem.IsEditingComment = ko.observable(false);
        return commentItem;
    };

    var ModalViewModel = function () {
        this.show = ko.observable(false);
        this.title = ko.observable('');
        this.text = ko.observable('');
        this.buttonBackText = ko.observable('');
        this.buttonConfirmText = ko.observable('');
        this.buttonFinalText = ko.observable('');
        this.isConfirmButtonVisible = ko.observable(false);
        this.isFinalButtonVisible = ko.observable(false);
        this.showModal = ko.computed(function () {
            if (this.show()) {
                $('#evaluations-generated-confirm').modal('show');
            } else {
                $('#evaluations-generated-confirm').modal('hide');
            }
        }, this);
    };

    ModalViewModel.prototype.showInvalidModal = function () {
        this.title("Guardar evaluación");
        this.text("No se puede guardar la evaluación porque hay calificaciones INVÁLIDAS");
        this.buttonBackText("Volver");
        this.show(true);
        this.isConfirmButtonVisible(false);
        this.isFinalButtonVisible(false);
    }

    ModalViewModel.prototype.showConfirmationModal = function () {
        this.title("Finalizar evaluación");
        this.text("¿Desea finalizar la evaluación con calificaciones vacías?");
        this.buttonBackText("Cancelar");
        this.buttonConfirmText("Confirmar");
        this.show(true);
        this.isConfirmButtonVisible(true);
        this.isFinalButtonVisible(false);
    }

    ModalViewModel.prototype.showFinishModal = function () {
        this.title("Finalizar evaluación");
        this.text("¿Desea finalizar la evaluación? Recuerde que una vez finalizada tu evaluación no podrás volver a editarla");
        this.buttonBackText("Cancelar");
        this.buttonFinalText("Finalizar");
        this.show(true);
        this.isConfirmButtonVisible(false);
        this.isFinalButtonVisible(true);
    }

    ModalViewModel.prototype.backAction = function () {
        this.show(false);
    }
    ModalViewModel.prototype.confirmAction = function () {
        this.showFinishModal();
    }

    ModalViewModel.prototype.finalAction = function () {
        this.show(false);
        if (viewmodel.evaluation.devolutionInProgress) {
            viewmodel.evaluationFinished = true;
        } else {
            viewmodel.calificationFinished = true;
        }
        viewmodel.onSave(true);
    }

    modalViewModel = new ModalViewModel();
    ko.applyBindings(modalViewModel, document.getElementById('evaluations-generated-confirm'));

    viewmodel = new EvaluationViewModel();
    viewmodel.load();

    var LayoutViewModel = function () {
        this.isEvaluationEditable = viewmodel.isEvaluationEditable;
    }

    LayoutViewModel.prototype.isDirty = viewmodel.isDirty;

    LayoutViewModel.prototype.onSave = function () {
        viewmodel.onSave();
    };

    layoutViewModel = new LayoutViewModel();
    ko.applyBindings(layoutViewModel, document.getElementById('header-evaluation'));

});
