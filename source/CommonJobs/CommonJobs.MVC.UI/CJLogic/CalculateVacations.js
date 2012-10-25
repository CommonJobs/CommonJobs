var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
/// <reference path="CJLogic.ts" />
/// <reference path="Libs/moment.d.ts" />
/// <reference path="Libs/underscore.browser.d.ts" />
/// <reference path="Libs/twix.d.ts" />
//CJ patch: remove follow var declaration//
var CJLogic;
(function (CJLogic) {
    var Vacation = (function () {
        function Vacation(cloneFrom) {
            _.extend(this, cloneFrom);
        }
        Vacation.prototype.countDays = function () {
            return new Twix(this.From, this.To, true).countDays();
        };
        Vacation.prototype.comparator = function () {
            return moment(this.From).unix();
        };
        return Vacation;
    })();    
    var VacationsPeriodReport = (function () {
        function VacationsPeriodReport() {
            this.Earned = 0;
            this.Taken = 0;
            this.Detail = [];
        }
        VacationsPeriodReport.prototype.addVacationBase = function (vacation) {
            var pos = _.sortedIndex(this.Detail, vacation, function (v) {
                return v.comparator();
            });
            this.Detail.splice(pos, 0, vacation);
        };
        VacationsPeriodReport.prototype.addVacation = function (vacation) {
            var vacation = new Vacation(vacation);
            this.Taken += vacation.countDays();
            this.addVacationBase(vacation);
        };
        VacationsPeriodReport.prototype.addPeriod = function (period) {
            var _this = this;
            this.Earned += period.Earned;
            this.Taken += period.Taken;
            _.each(period.Detail, function (vacation) {
                return _this.addVacationBase(vacation);
            });
        };
        return VacationsPeriodReport;
    })();    
    var YearVacations = (function (_super) {
        __extends(YearVacations, _super);
        function YearVacations(year, hiringYear, hiringMonth) {
                _super.call(this);
            this.year = year;
            var antiquity = year - hiringYear;
            if(hiringMonth < 7) {
                antiquity++;
            }
            if(antiquity == 0) {
                this.Earned = 13 - hiringMonth;
            } else {
                if(antiquity <= 5) {
                    this.Earned = 14;
                } else {
                    if(antiquity <= 10) {
                        this.Earned = 21;
                    } else {
                        if(antiquity <= 20) {
                            this.Earned = 28;
                        } else {
                            this.Earned = 35;
                        }
                    }
                }
            }
        }
        return YearVacations;
    })(VacationsPeriodReport);    
    var ReportMaker = (function () {
        function ReportMaker(currentYear, hiringDate, vacations) {
            this.currentYear = currentYear;
            this.yearVacations = [];
            this.inAdvance = new VacationsPeriodReport();
            var hiringDate = moment(hiringDate);
            this.hiringYear = hiringDate.year();
            this.hiringMonth = hiringDate.month() + 1;
            this.generateEmptyYearVacations();
            this.fillPeriodVacations(vacations);
        }
        //#region prepare data
                ReportMaker.prototype.yearIndex = //#region yearVacations Helpers
        function (year) {
            return year <= this.currentYear && year >= this.hiringYear ? year - this.hiringYear : null;
        };
        ReportMaker.prototype.indexToYear = function (index) {
            return this.hiringYear + index;
        };
        ReportMaker.prototype.addYearVacations = function (yearVacations) {
            var index = this.yearIndex(yearVacations.year);
            if(index !== null) {
                this.yearVacations[index] = yearVacations;
            }
        };
        ReportMaker.prototype.getYearVacations = function (year) {
            var index = this.yearIndex(year);
            return index !== null ? this.yearVacations[index] : null;
        }//#endregion
        ;
        ReportMaker.prototype.generateEmptyYearVacations = function () {
            var year;
            for(year = this.hiringYear; year <= this.currentYear; year++) {
                this.addYearVacations(new YearVacations(year, this.hiringYear, this.hiringMonth));
            }
        };
        ReportMaker.prototype.fillPeriodVacations = function (vacations) {
            var _this = this;
            _.each(vacations, function (vacation) {
                if(vacation.Period < _this.hiringYear) {
                    return;
                }
                var periodVacations = vacation.Period > _this.currentYear ? _this.inAdvance : _this.getYearVacations(vacation.Period);
                periodVacations.addVacation(vacation);
            });
        }//#endregion
        ;
        ReportMaker.prototype.Execute = function (detailedYearsQuantity) {
            var older = new VacationsPeriodReport();
            var result = {
                TotalEarned: 0,
                TotalTaken: this.inAdvance.Taken || 0,
                TotalPending: 0,
                Older: older,
                InAdvance: this.inAdvance,
                ByYear: {
                }
            };
            var firstYear = this.currentYear - detailedYearsQuantity;
            _.each(this.yearVacations, function (yearVacations) {
                if(yearVacations.year > firstYear) {
                    result.ByYear[yearVacations.year] = yearVacations;
                    result.TotalEarned += yearVacations.Earned;
                    result.TotalTaken += yearVacations.Taken;
                } else {
                    older.addPeriod(yearVacations);
                }
            });
            result.TotalEarned += older.Earned;
            result.TotalTaken += older.Taken;
            result.TotalPending = result.TotalEarned - result.TotalTaken;
            return result;
        };
        return ReportMaker;
    })();    
    ; ;
    function CalculateVacations(data, config) {
        try  {
            if(!data.HiringDate) {
                return CJLogic.WrapError("HiringDate is required.");
            } else {
                var maker = new ReportMaker(config.CurrentYear, data.HiringDate, data.Vacations);
                var result = maker.Execute(config.DetailedYearsQuantity);
                return CJLogic.WrapResult(result);
            }
        } catch (ex) {
            return CJLogic.WrapError("Unexpected exception", ex);
        }
    }
    CJLogic.CalculateVacations = CalculateVacations;
})(CJLogic || (CJLogic = {}));

