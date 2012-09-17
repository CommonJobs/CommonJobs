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
    var h = CJLogic.CalculateVacationsHelpers;
    var debug = "";
    var totalTaked = 0;
    var takedByPeriod = {};

    if (hiringDate) {
        debug += " " + hiringDate + " " + typeof (hiringDate) + "\n";
    } else {
        debug += " NO HIRING DATE \n";
    }
    
    var now = new Date();
    debug += now.toString() + " " + typeof (now) + "\n";


    
    _.each(vacationList, function (vacation) {
        if (!takedByPeriod[vacation.Period]) {
            takedByPeriod[vacation.Period] = 0;
        }
        var days = h.getDays(vacation.From, vacation.To);
        takedByPeriod[vacation.Period] += days;
        totalTaked += days;
    });
    
    debug += JSON.stringify(takedByPeriod);

    var calculatedVacations = {
        Debug: debug,
        TotalDays: totalTaked
    };

    return calculatedVacations;
};

