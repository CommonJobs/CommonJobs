$(document).ready(function () {
    var evaluationStates = ['En curso', 'Esperando Eval Empleado', 'Esperando Eval Responsable', 'Esperando Eval Empresa', 'Lista para devolución', 'Abierta para devolución', 'Finalizada'];

    var CalificatorsManager = function (data) {
        var self = this;
        this.evaluation = '';
        this.calificators = ko.observableArray();
        this.newCalificator = ko.observable('');
        this.activeCalificators = ko.computed(function () {
            return _.filter(this.calificators(), function (e) {
                return e.action() !== 1;
            });
        }, this);
        this.saveButtonEnable = ko.observable(false);
        this.title = ko.computed(function () {
            return this.activeCalificators().length > 0 ? 'Editar Evaluadores' : 'Agregar Evaluadores';
        }, this);
        if (data) {
            this.fromJs(data);
        }
        this.onEnter = function (d, e) {
            if (e.keyCode === 13) {
                self.addCalificator();
            }
            return true;
        };
        this.addCalificator = function () {
            var userName = self.newCalificator();
            if (userName && userName != self.evaluation.userName) {
                var calificator = _.find(self.calificators(), function (e) {
                    return e.userName == userName;
                }, this);
                if (calificator) {
                    calificator.action(0);
                } else {
                    var calificator = new Calificator();
                    calificator.add(userName);
                    self.calificators.push(calificator);
                }
                self.saveButtonEnable(true);
            }
            self.newCalificator('');
        }
        this.removeCalificator = function () {
            if (this.action() === 0) {
                self.calificators.remove(this);
            } else {
                this.action(1);
            }
            self.saveButtonEnable(true);
        }
        this.close = function () {
            $('.content-evaluation').append($('.content-modal'));
            $('.content-modal').hide();
            self.saveButtonEnable(false);
        }
        this.save = function () {
            var updateCalificators = this.calificatorsManagerModel.toJs();
            $.ajax("/Evaluations/api/UpdateCalificators/", {
                type: "POST",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(updateCalificators),
                complete: function (response) {
                    self.close();
                    self.saveButtonEnable(false);
                },
                error: function () {
                    alert('Fallo');
                }
            });
        }
    }

    CalificatorsManager.prototype.fromJs = function (data) {
        var self = this;
        this.evaluation = data.evaluation;
        this.calificators(_.map(data.calificators, function (e) {
            return new Calificator(e);
        }));
    }

    CalificatorsManager.prototype.toJs = function () {
        var calificators = _.map(this.calificators(), function (e) {
            return e.toJs();
        });
        var calificatorsFiltered = _.filter(calificators, function (e) {
            return e.Action !== '';
        });

        return {
            Evaluation: this.evaluation.toJs(),
            Calificators: calificatorsFiltered
        };
    }

    var Calificator = function (data) {
        this.userName = '';
        this.action = ko.observable();
        if (data) {
            this.fromJs(data);
        }
    }

    Calificator.prototype.fromJs = function (data) {
        this.userName = data;
        this.action('');
    }

    Calificator.prototype.add = function (data) {
        this.userName = data;
        this.action(0);
    }

    Calificator.prototype.remove = function () {
        this.action(1);
    }

    Calificator.prototype.toJs = function () {
        return {
            UserName: this.userName,
            Action: this.action()
        };
    }

    var Dashboard = function (data) {
        var self = this;
        this.items = ko.observableArray();
        this.calificatorsManagerModel = new CalificatorsManager();
        if (data) {
            this.fromJS(data);
        }
        this.headers = [
        { title: 'Rol', sortable: true, sortPropertyName: 'isResponsible', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
        { title: 'Empleado', sortable: true, sortPropertyName: 'fullName', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
        { title: 'Puesto', sortable: true, sortPropertyName: 'currentPosition', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
        { title: 'Seniority', sortable: true, sortPropertyName: 'seniority', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false) },
        { title: 'Evaluadores', sortable: false},
        { title: 'Estado', sortable: true, sortPropertyName: 'state', defaultPropertyName: 'fullName', asc: true, activeSort: ko.observable(false), observable: true },
        { title: '', sortable: false },
        { title: '', sortable: false }
        ];
        this.sort = commonSort.bind(this);
        this.defaultSort = function () {
            this.headers[0].activeSort(false);
            this.sort(this.headers[0]);
        }
        this.isLoading = ko.observable(false);
    }

    Dashboard.prototype.fromJS = function (data) {
        var self = this;
        this.items(_.map(data.Evaluations, function (e) {
            return new Evaluation(e);
        }));
    }

    Dashboard.prototype.toJs = function () {
        return {
            Evaluations: _.map(this.items(), function (e) {
                return e.toJs();
            })
        }
    }
    Dashboard.prototype.getEvaluationState = function (state) {
        return this.evaluationStates[state];
    }

    var Evaluation = function (data) {
        this.id = '';
        this.isResponsible = false;
        this.isEditable = false;
        this.fullName = '';
        this.userName = '';
        this.period = '';
        this.currentPosition = '';
        this.seniority = '';
        this.evaluatorsAmount = '';
        this.evaluatorsString = '';
        this.state = ko.observable('');
        this.currentState = '';
        this.evaluators = ko.observableArray();
        if (data) {
            this.fromJs(data);
        }
    }

    Evaluation.prototype.fromJs = function (data) {
        this.id = data.Id;
        this.isResponsible = data.IsResponsible;
        this.isEditable = data.IsEditable;
        this.fullName = data.FullName;
        this.userName = data.UserName;
        this.period = data.Period;
        this.currentPosition= data.CurrentPosition || '';
        this.seniority = data.Seniority || '';
        this.evaluators(data.Evaluators);
        this.evaluatorsString = ko.computed(function () {
            return this.evaluators().toString().replace(/,/g, ', ');
        }, this);
        this.evaluatorsAmount = ko.computed(function () {
            return this.evaluators().length;
        }, this);
        this.evaluatorsTextLink = ko.computed(function () {
            return (this.evaluatorsAmount() === 1) ? this.evaluatorsAmount() + " evaluador" : this.evaluatorsAmount() + " evaluadores";
        }, this);
        this.calificationActionTooltip = ko.observable('');
        this.calificationActionText = ko.observable('');
        this.calificationActionClass = ko.observable('');
        this.calificationUrl = urlGenerator.action(this.period + "/" + this.userName + "/", "Evaluations");
        this.startDevolutionUrl = function () {
            $.ajax(urlGenerator.action("api/StartDevolution", "Evaluations"), {
                type: "POST",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ evaluationId: this.id }),
                success: function (response) {
                    getDashboardEvaluations();
                }
            });
        };
        this.state.subscribe(function () {
            if (this.isResponsible && this.state() != 6) {
                if (this.state() == 0 || this.state() == 2) {
                    this.calificationActionTooltip("Evaluar como responsable");
                    this.calificationActionClass("icon user");
                    this.calificationActionText("Evaluar");
                    return;
                } else if (this.state() == 1 || this.state() == 3) {
                    this.calificationActionText("Evaluar");
                    this.calificationActionClass("icon empresa");
                    this.calificationActionTooltip("Evaluar como empresa");
                    return;
                } else if (this.state() == 5) {
                    this.calificationActionText("Devolución");
                    this.calificationActionClass("icon user");
                    this.calificationActionTooltip("Hacer la devolución con el evaluado");
                    return;
                }
            } else if (!this.isResponsible && this.isEditable && this.state() != 6) {
                this.calificationActionTooltip("Evaluar como evaluador");
                this.calificationActionClass("icon user");
                this.calificationActionText("Evaluar");
                return;
            }
            this.calificationActionText("Ver Evaluación");
            this.calificationActionClass("icon view");
            this.calificationActionTooltip("Ver Evaluación");
        }, this);
        this.state(data.State);
        this.stateName = evaluationStates[this.state()];
        this.stateClasses = "state-doc state-" + this.state();
        this.isCalificatorsEditable = ko.computed(function () {
            return this.isResponsible && this.state() != 6;
        }, this);
        this.showCalificatorsManager = function (data, event) {
            viewmodel.calificatorsManagerModel.fromJs({ evaluation: this, calificators: this.evaluators() });
            var popupContainer = $(event.target).parents('.calificators-column');
            popupContainer.append($('.content-modal'));
            $('.content-modal').show();
            return true;
        }
    }

    Evaluation.prototype.toJs = function () {
        return {
            IsResponsible: this.isResponsible,
            FullName: this.fullName,
            CurrentPosition: this.currentPosition,
            Seniority: this.seniority,
            Evaluators: this.evaluators,
            State: this.state(),
            UserName: this.userName,
            Period: this.period
        };
    }

    var viewmodel = new Dashboard();

    getDashboardEvaluations();
    ko.applyBindings(viewmodel);
    commonSuggest($('.content-modal .search'), 'UserName');
    
    function getDashboardEvaluations() {
        viewmodel.isLoading(true);
        var ajax = $.getJSON("/Evaluations/api/getDashboardEvaluations/", function (model) {
            viewmodel.fromJS(model);
            viewmodel.defaultSort();
            
        });
        ajax.always(function () {
            viewmodel.isLoading(false);
        });
        ajax.fail(function () {
            alert('Fallo');
        });
    }
});