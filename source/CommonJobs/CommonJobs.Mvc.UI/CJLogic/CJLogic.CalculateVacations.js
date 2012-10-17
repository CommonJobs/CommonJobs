/// <reference path="Libs/json2.js" />
/// <reference path="Libs/underscore.js" />
/// <reference path="Libs/moment.js" />
/// <reference path="Libs/twix.js" />

var CJLogic = CJLogic || {};

CJLogic.CalculateVacations = function (hiringDate, vacationList, now) {
    var resultWrapper = {
        Successful: false,
        Message: null,
        Details: null,
        Result: null
    };
    
    try 
    {
        if (!hiringDate) {
            resultWrapper.Message = "HiringDate is required."
        } else {
            now = now || new Date();
            var result = resultWrapper.Result = {
                TotalTaken: 0,
                TotalPending: 0,
                ByYear: {}
            };

            hiringDate = moment(hiringDate);
            var currentYear = moment(now).year();
            var hiringYear = hiringDate.year();
            var hiringMonth = hiringDate.month() + 1;

            var TakenVacationsByYear = {};
            _.each(vacationList, function (vacation) {
                TakenVacationsByYear[vacation.Period] = (TakenVacationsByYear[vacation.Period] || 0) + new Twix(vacation.From, vacation.To, true).countDays();
            });

            var antiquity = 0;
            for (var year = hiringYear; year <= currentYear; year++) {
                var item = result.ByYear[year] = {};

                if (year != hiringYear || hiringMonth < 7) {
                    antiquity++;
                }
                item.Antiquity = antiquity;

                if (antiquity == 0) {
                    item.Earned = 13 - hiringMonth;
                } else if (antiquity <= 5) {
                    item.Earned = 14;
                } else if (antiquity <= 10) {
                    item.Earned = 21;
                } else if (antiquity <= 20) {
                    item.Earned = 28;
                } else {
                    item.Earned = 35;
                }

                item.Taken = TakenVacationsByYear[year] || 0;
                item.Pending = item.Earned - item.Taken;
                result.TotalTaken += item.Taken;
                result.TotalPending += item.Pending;
            }

            for (var year in TakenVacationsByYear) {
                if (year > currentYear) {
                    var item = result.ByYear[year] = {};
                    item.Taken = TakenVacationsByYear[year] || 0;
                    result.TotalTaken += item.Taken;
                    result.TotalPending -= item.Taken;
                }
            }

            resultWrapper.Successful = true;
        }
    } catch (ex) {
        //TODO: add some base utilities to deal with this kind of things
        resultWrapper.Message = "Unexpected exception";
        resultWrapper.Details = ex.description || ex.message || ex;
    }
    return resultWrapper;
}


