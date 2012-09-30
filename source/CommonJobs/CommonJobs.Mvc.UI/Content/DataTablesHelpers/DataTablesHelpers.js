$(function () {
    $.extend($.fn.DataTable.defaults.oLanguage, {
            "sEmptyTable": "No hay datos disponibles",
            "sInfo": "Mostrando desde _START_ hasta _END_ de _TOTAL_ registros",
            "sInfoEmpty": "Mostrando desde 0 hasta 0 de 0 registros",
            "sInfoFiltered": "(filtrado de _MAX_ registros en total)",
            "sInfoPostFix": "",
            "sInfoThousands": ",",
            "sLengthMenu": "Mostrar _MENU_ registros",
            "sLoadingRecords": "Cargando...",
            "sProcessing": "Procesando...",
            "sSearch": "Buscar:",
            "sZeroRecords": "No se encontraron resultados",
            "oPaginate": {
                "sFirst": "Primero",
                "sLast": "Último",
                "sNext": "Siguiente",
                "sPrevious": "Anterior"
            },
            "oAria": {
                "sSortAscending": ": activar para Ordenar Ascendentemente",
                "sSortDescending": ": activar para Ordendar Descendentemente"
            }
    });
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
                            case 'filter': return val ? moment(val).format("MMMM DD-MM-YYYY-MM-DD") : "Sin fecha";
                            case 'display': return val ? moment(val).format("MMMM YYYY") : "<em>Sin fecha</em>";
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
                                return _.isUndefined(val) ? "<em>Sin datos</em>" : val;
                            case 'display':
                                return _.isUndefined(val) ? "<em>Sin datos</em>" : val;
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

                        switch (type) {
                            case 'filter':
                                return (firstName || "Sin nombre") + " " + (lastName || "Sin apellido") + ", " + (firstName || "Sin nombre");
                            case 'display':
                                return (lastName || "<em>Sin apellido</em>") + ", " + (firstName || "<em>Sin nombre</em>");
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
            },
            string: function (getVal, moreOptions) {
                return jQuery.extend({
                    "sType": "nulls-below-string",
                    "mData": function (data, type, val) {
                        if (type === 'set') return; //TODO

                        return getVal(data);
                    }
                },
                moreOptions);
            }
        }
    };
});