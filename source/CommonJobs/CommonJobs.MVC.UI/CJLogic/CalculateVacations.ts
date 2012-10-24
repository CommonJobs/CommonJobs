/// <reference path="CJLogic.ts" />
/// <reference path="Libs/moment.d.ts" />
/// <reference path="Libs/underscore.browser.d.ts" />
/// <reference path="Libs/twix.d.ts" />

//CJ patch: remove follow var declaration//
module CJLogic {
    export interface IVacationsReportConfiguration
    {
        CurrentYear: number;
        DetailedYearsQuantity: number;
    }

    export interface IVacationsReportData
    {
        HiringDate?: string;
		Vacations: IVacation[];
    }

    export interface IVacationsReport {
        TotalEarned: number;
        TotalTaken: number;
        TotalPending: number;
        ByYear: any; 
        Older: IVacationsPeriodReport;
        InAdvance: IVacationsPeriodReport;
    }

	export interface IVacationsPeriodReport {
        Earned: number;
        Taken: number;
        Detail: IVacation[];
    }

    export interface IVacation {
        Period: number;
        From: string;
        To: string;
    }

    class Vacation implements IVacation {
        Period: number;
        From: string;
        To: string;
        constructor (cloneFrom: IVacation) {
            _.extend(this, cloneFrom);
        }

        public countDays(): number {
            return new Twix(this.From, this.To, true).countDays();
        }

        public comparator(): number {
            return moment(this.From).unix();
        }
    }

    class VacationsPeriodReport implements IVacationsPeriodReport {
        Earned: number = 0;
        Taken: number = 0;
        Detail: Vacation[] = [];

        private addVacationBase(vacation: Vacation) {
            var pos = _.sortedIndex(this.Detail, vacation, v => v.comparator());
            this.Detail.splice(pos, 0, vacation);
        }

        public addVacation(vacation: IVacation) : void {
            var vacation = new Vacation(vacation);
            this.Taken += vacation.countDays();
            this.addVacationBase(vacation);
        }

        public addPeriod(period: VacationsPeriodReport) {
            this.Earned += period.Earned;
            this.Taken += period.Taken;
            _.each(period.Detail, vacation => this.addVacationBase(vacation));
        }
    }

    class YearVacations implements IVacationsPeriodReport extends VacationsPeriodReport {
        constructor (public year: number, hiringYear: number, hiringMonth: number) {
            super();
            var antiquity = year - hiringYear;
            if (hiringMonth < 7)
                antiquity++;

            if (antiquity == 0) {
                this.Earned = 13 - hiringMonth;
            } else if (antiquity <= 5) {
                this.Earned = 14;
            } else if (antiquity <= 10) {
                this.Earned = 21;
            } else if (antiquity <= 20) {
                this.Earned = 28;
            } else {
                this.Earned = 35;
            }
        }
    }

    class ReportMaker {
        private yearVacations: YearVacations[] = [];
        private hiringYear: number;
        private hiringMonth: number;
        private inAdvance = new VacationsPeriodReport();
        
        //#region yearVacations Helpers

        private yearIndex(year: number): number {
            return year <= this.currentYear && year >= this.hiringYear
                ? year - this.hiringYear
                : null;
        }

        private indexToYear(index: number): number {
            return this.hiringYear + index;
        }
       
        private addYearVacations(yearVacations: YearVacations): void {
            var index = this.yearIndex(yearVacations.year)
            if (index !== null)
                this.yearVacations[index] = yearVacations;
        }

        private getYearVacations(year: number): YearVacations {
            var index = this.yearIndex(year);
            return index !== null
                ? this.yearVacations[index]
                : null;
        }

        //#endregion


        constructor (private currentYear: number, hiringDate: any, vacations: IVacation[]) {
            var hiringDate = moment(hiringDate);
            this.hiringYear = hiringDate.year();
            this.hiringMonth = hiringDate.month() + 1;
            
            this.generateEmptyYearVacations();
            this.fillPeriodVacations(vacations);
        }

        //#region prepare data
        private generateEmptyYearVacations(): void {
            var year: number;
            for (year = this.hiringYear; year <= this.currentYear; year++) {
                this.addYearVacations(new YearVacations(year, this.hiringYear, this.hiringMonth));
            }
        }

        private fillPeriodVacations(vacations: IVacation[]): void {
            _.each(vacations, (vacation: IVacation) => {

                if (vacation.Period < this.hiringYear)
                    return;
                
                var periodVacations = vacation.Period > this.currentYear
                    ? this.inAdvance
                    : this.getYearVacations(vacation.Period);
                
                periodVacations.addVacation(vacation);
            });
        }

        //#endregion 

        public Execute(detailedYearsQuantity: number): IVacationsReport {
            var older = new VacationsPeriodReport();
            var result: IVacationsReport = {
                TotalEarned: 0,
                TotalTaken: this.inAdvance.Taken || 0,
                TotalPending: 0,
                Older: older,
                InAdvance: this.inAdvance,
                ByYear: {}
            }

            var firstYear = this.currentYear - detailedYearsQuantity;

            _.each(this.yearVacations, (yearVacations: YearVacations) => {
                if (yearVacations.year > firstYear) {
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
        }
    };


    export function CalculateVacations(data: IVacationsReportData, config: IVacationsReportConfiguration): IResultWrapper {
        try {
            if (!data.HiringDate) {
                return WrapError("HiringDate is required.");
            } else {
                var maker = new ReportMaker(
                    config.CurrentYear,
                    data.HiringDate,
                    data.Vacations);
                var result = maker.Execute(config.DetailedYearsQuantity);
                return WrapResult(result);
            }
        } catch (ex) {
            return WrapError("Unexpected exception", ex);
        }
    }
}
