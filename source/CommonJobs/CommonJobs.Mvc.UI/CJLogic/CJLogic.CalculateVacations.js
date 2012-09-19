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
        TotalTaken: 0,
        //TODO: replace demo data
        TotalPending: 17,
        //TODO: replace demo data
        ByYear: {
            "2010": {
                Antiquity: 0,
                Earned: 3,
                Taken: 0,
                Pending: 3
            },
            "2011": {
                Antiquity: 1,
                Earned: 14,
                Taken: 7,
                Pending: 7
            },
            "2012": {
                Antiquity: 2,
                Earned: 14,
                Taken: 7,
                Pending: 7
            }
        }
    };

    var now = new Date();
    
    _.each(vacationList, function (vacation) {
        var days = h.getDays(vacation.From, vacation.To);
        result.TotalTaken += days;
    });

    return result;
}


