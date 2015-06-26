$(document).ready(function () {
    var evaluationStates = ['En curso', 'Esperando Cal Empleado', 'Esperando Cal Responsable', 'Esperando Cal Empresa', 'Lista para devolución', 'Abierta para devolución', 'Finalizada'];

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
            if (userName) {
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
            }
            self.newCalificator('');
        }
        this.removeCalificator = function () {
            if (this.action() === 0) {
                self.calificators.remove(this);
            } else {
                this.action(1);
            }
        }
        this.close = function () {
            $('.content-evaluation').append($('.content-modal'));
            $('.content-modal').hide();
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
                    getDashboardEvaluations();
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
        this.evaluations = ko.observableArray();
        this.calificatorsManagerModel = new CalificatorsManager();
        if (data) {
            this.fromJS(data);
        }
        this.headers = [
        { title: 'Rol', sortPropertyName: 'idResponsible', asc: true },
        { title: 'Empleado', sortPropertyName: 'fullName', asc: true },
        { title: 'Puesto', sortPropertyName: 'currentPosition', asc: true },
        { title: 'Seniority', sortPropertyName: 'seniority', asc: true },
        { title: 'Calificadores', sortPropertyName: 'evaluators', asc: true },
        { title: 'Estado', sortPropertyName: 'state', asc: true }
        ];
        this.activeSort;
        this.sort = function (header, event) {
            if (self.activeSort === header) {
                header.asc = !header.asc; //toggle the direction of the sort
            } else {
                self.activeSort = header; //first click, remember it
            }
            var prop = self.activeSort.sortPropertyName;
            var ascSort = function (a, b) {
                return a[prop] < b[prop] ? -1 : a[prop] > b[prop] ? 1 : a[prop] == b[prop] ? 0 : 0;
            };
            var descSort = function (a, b) {
                return ascSort(b, a);
            };
            var sortFunc = self.activeSort.asc ? ascSort : descSort;
            self.evaluations.sort(sortFunc);
        };
        this.defaultSort = function () {
            this.sort(this.headers[1]);
        }
    }

    Dashboard.prototype.fromJS = function (data) {
        var self = this;
        this.evaluations(_.map(data.Evaluations, function (e) {
            return new Evaluation(e);
        }));
    }

    Dashboard.prototype.toJs = function () {
        return {
            Evaluations: _.map(this.evaluations(), function (e) {
                return e.toJs();
            })
        }
    }
    Dashboard.prototype.getEvaluationState = function (state) {
        return this.evaluationStates[state];
    }

    var Evaluation = function (data) {
        this.idResponsible = '';
        this.fullName = '';
        this.userName = '';
        this.period = '';
        this.currentPosition = '';
        this.seniority = '';
        this.evaluatorsAmount = '';
        this.evaluatorsString = '';
        this.state = '';
        this.currentState = '';
        this.evaluators = '';
        if (data) {
            this.fromJs(data);
        }
    }

    Evaluation.prototype.fromJs = function (data) {
        this.isResponsible = data.IsResponsible;
        this.fullName = data.FullName;
        this.userName = data.UserName;
        this.period = data.Period;
        this.currentPosition= data.CurrentPosition || '';
        this.seniority = data.Seniority || '';
        this.evaluators = data.Evaluators;
        this.evaluatorsString = ko.computed(function () {
            return this.evaluators.toString().replace(/,/g, ', ');
        }, this);
        this.evaluatorsAmount = ko.computed(function () {
            return this.evaluators.length;
        }, this);
        this.state = data.State;
        this.stateName = evaluationStates[data.State];
        this.showCalificatorsManager = function (data, event) {
            viewmodel.calificatorsManagerModel.fromJs({ evaluation: this, calificators: this.evaluators });
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
            State: this.state,
            UserName: this.userName,
            Period: this.period
        };
    }

    var viewmodel = new Dashboard();

    getDashboardEvaluations();
    ko.applyBindings(viewmodel);
    commonSuggest($('.content-modal .search'), 'UserName');
    

    function getDashboardEvaluations() {
        $.getJSON("/Evaluations/api/getDashboardEvaluations/", function (model) {
            viewmodel.fromJS(model);
            viewmodel.defaultSort();
        });
    }

});