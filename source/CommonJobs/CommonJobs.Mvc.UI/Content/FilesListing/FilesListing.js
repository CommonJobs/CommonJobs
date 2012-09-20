/// <reference path="../../Scripts/jquery-1.7.2-vsdoc.js" />
/// <reference path="../../Scripts/underscore.js" />
/// <reference path="../../Scripts/url-generator.js" />

$(function () {
    var qs = new QuickSearchPage({
        //pageSize: 3,
        generateRedirectUrl: function (searchParameters) {
            return urlGenerator.action("Index", "Attachments", searchParameters);
        },
        generateSearchUrl: function (searchParameters) {
            return urlGenerator.action("AttachmentsQuickSearch", "Attachments", searchParameters);
        },
        fillOtherSearchParameters: function (searchParameters) {
            if ($("#SearchOnlyInFileName").prop("checked"))
                searchParameters.SearchOnlyInFileName = true;
        }
    });

    $("#SearchOnlyInFileName, #quickSearchSubmit").click(function () {
        //It also catches "enter"s in form inputs
        qs.redirect();
    });

    qs.search();
});