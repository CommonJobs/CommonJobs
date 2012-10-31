/// <reference path="Libs/underscore.browser.d.ts" />


module CJLogic {
    interface IResultWrapper {
        Successful: bool;
        Message?: string;
        Details?: string;
        Result?: any;
    }

    export function WrapResult(result: any, message?: string): IResultWrapper {
        var result: IResultWrapper = {
            Successful: true,
            Result: result,
        };

        if (!_.isUndefined(message)) {
            result.Message = message;
        }

        return result;
    }

    export function WrapError(message: string, details?: any): IResultWrapper {
        var result: IResultWrapper = {
            Successful: false,
            Message: message
        };

        if (details && _.isString(details)) {
            result.Details = details;
        } else if (details) { 
            result.Details = details.description || details.message || details.toString();
        }
            
        return result;
    }
}

