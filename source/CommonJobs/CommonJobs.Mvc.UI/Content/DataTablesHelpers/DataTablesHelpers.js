$(function () {
    var registerNullsBelow = function (baseSort) {
        var oldpre = jQuery.fn.dataTableExt.oSort[baseSort + "-pre"];
        var oldasc = jQuery.fn.dataTableExt.oSort[baseSort + "-asc"];
        var olddesc = jQuery.fn.dataTableExt.oSort[baseSort + "-desc"];

        jQuery.fn.dataTableExt.oSort["nulls-below-" + baseSort + "-pre"] = function (a) {
            return a === null ? null : oldpre(a);
        };

        jQuery.fn.dataTableExt.oSort["nulls-below-" + baseSort + "-asc"] = function (a, b) {
            if (a === null) {
                return 1;
            } else if (b === null) {
                return -1;
            } else {
                return oldasc(a, b);
            }
        }

        jQuery.fn.dataTableExt.oSort["nulls-below-" + baseSort + "-desc"] = function (a, b) {
            if (a === null) {
                return 1;
            } else if (b === null) {
                return -1;
            } else {
                return olddesc(a, b);
            }
        }
    };
    registerNullsBelow("numeric");
    registerNullsBelow("date");
    registerNullsBelow("string");

    window.DataTablesHelpers = {
        column: {
            date: function (getVal, moreOptions) {
                return jQuery.extend({
                    "sType": "nulls-below-string",
                    "mData": function (data, type, val) {
                        if (type === 'set') return; //TODO
                        var val = getVal(data);
                        switch (type) {
                            case 'filter': return val ? moment(val).format("MMM DD-MM-YYYY-MM-DD") : "Sin fecha";
                            case 'display': return val ? moment(val).format("MMM YYYY") : "Sin fecha";
                            default: return val ? moment(val).format("YYYY-MM-DD") : null;
                        }
                    }
                }, moreOptions);
            },
            number: function (getVal, moreOptions) {
                return jQuery.extend({
                    "sType": "nulls-below-numeric",
                    "mData": function (data, type, val) {
                        if (type === 'set') return; //TODO
                        var val = getVal(data);
                        switch (type) {
                            case 'filter':
                            case 'display':
                                return _.isUndefined(val) ? "Sin datos" : val;
                            default:
                                return _.isUndefined(val) ? null : val;
                        }
                    }
                }, moreOptions);
            },
            fullName: function (getLastName, getFirstName, moreOptions) {
                return jQuery.extend({
                    "sType": "nulls-below-string",
                    "mData": function (data, type, val) {
                        if (type === 'set') return; //TODO

                        var lastName = getLastName(data);
                        var firstName = getFirstName(data);

                        var showLastName = lastName || "Sin apellido";
                        var showFirstName = firstName || "Sin nombre";

                        switch (type) {
                            case 'filter':
                                return showFirstName + " " + showLastName + ", " + showFirstName;
                            case 'display':
                                return showLastName + ", " + showFirstName;
                            default:
                                if (!lastName && !firstName) {
                                    return null;
                                } else {
                                    return (lastName || "zzzzzzz") + ", " + (firstName || "zzzzzzz");
                                }
                        }
                    }
                }, moreOptions);
            },
            link: function (columnDefinition, generateUrl, moreOptions) {
                return jQuery.extend(
                    {},
                    columnDefinition,
                    {
                        "mData": function (data, type, val) {
                            if (type === 'display') {
                                return "<a href='" + generateUrl(data) + "'>" + columnDefinition.mData(data, type, val) + "</a>";
                            } else {
                                return columnDefinition.mData(data, type, val);
                            }
                        }
                    },
                moreOptions);
            }
        }
    };
});