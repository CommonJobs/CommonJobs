/// <reference path="Libs/json2.js" />
/// <reference path="Libs/underscore.js" />
/// <reference path="Libs/moment.js" />
/// <reference path="Libs/twix.js" />

var CJLogic = CJLogic || {};

CJLogic.CalculateVacationsHelpers = {
    getDays: function (from, to) {
        var period = new Twix(from, to, true)
        return period.countDays();
    }
};

CJLogic.CalculateVacations = function (hiringDate, vacationList) {
    if (!hiringDate)
        return null;

    var h = CJLogic.CalculateVacationsHelpers;

    var result = {
        TotalTaked: 0,
        TotalPending: 0,
        ByYear: { }
    };

    var now = new Date();
    var currentYear = moment(now).year();
    var hiringYear = moment(hiringDate).year();

    var takedVacationsByYear = {};
    _.each(vacationList, function (vacation) {
        takedVacationsByYear[vacation.Period] = (takedVacationsByYear[vacation.Period] || 0) + h.getDays(vacation.From, vacation.To);
    });

    var antiquity = 0;
    for (var year = hiringYear; year <= currentYear; year++) {
        var item = result.ByYear[year] = {};
        item.Antiquity = antiquity++; //TODO fix it because antiquity could be calculated different depending on hiring month 
        item.Earned = 14; //TODO fix it because earned could be calculated different depending on hiring month and antiquity
        item.Taked = takedVacationsByYear[year] || 0;
        item.Pending = item.Earned - item.Taked;
        result.TotalTaked += item.Taked;
        result.TotalPending += item.Pending;
    }
    
    return result;
}


