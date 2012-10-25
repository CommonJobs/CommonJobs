/// <reference path="Libs/underscore.browser.d.ts" />
var CJLogic;
(function (CJLogic) {
    function WrapResult(result, message) {
        var result = {
            Successful: true,
            Result: result
        };
        if(!_.isUndefined(message)) {
            result.Message = message;
        }
        return result;
    }
    CJLogic.WrapResult = WrapResult;
    function WrapError(message, details) {
        var result = {
            Successful: false,
            Message: message
        };
        if(details && _.isString(details)) {
            result.Details = details;
        } else {
            if(details) {
                result.Details = details.description || details.message || details.toString();
            }
        }
        return result;
    }
    CJLogic.WrapError = WrapError;
})(CJLogic || (CJLogic = {}));

