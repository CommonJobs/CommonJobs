/// <reference path="../../Scripts/jquery-1.7.1-vsdoc.js" />
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
            //nothing to do here, the function is left for future addition of options
        }
    });

    $("#quickSearchSubmit").click(function () {
        //It also catches "enter"s in form inputs
        qs.redirect();
    });

    $(".results").on("click", ".fileResult", function (e) {
        e.preventDefault();
        window.location = $(this).find(".fileTitle a").first().attr("href");
    });

    qs.search();
});

var FileSearchUtilities = {
    fileIconFromExtension: function(fileName) {
        if (fileName == null) return urlGenerator.content("Images/filetypes/unknown.png");

        var extensionSeparatorLocation = fileName.lastIndexOf('.');
        var extension = fileName.substring(extensionSeparatorLocation + 1);
        var allowedExtensions = ['avi', 'bmp', 'doc', 'docx', 'gif', 'jpeg', 'jpg', 'mov', 'mp3',
            'mpeg', 'mpg', 'pdf', 'png', 'ppt', 'pptx', 'rar', 'txt', 'wmv', 'xls', 'xlsx', 'zip'];

        var iconName = _.find(allowedExtensions, function (aExt) {
            return aExt.toUpperCase() == extension.toUpperCase();
        });

        if (iconName == null) iconName = "unknown";

        return urlGenerator.content("Images/filetypes/" + iconName + ".png");
    },
    urlToRelatedEntity: function (relatedEntityId, relatedEntityType) {
        if (relatedEntityType.indexOf("Applicant") != -1) {
            return urlGenerator.action("Edit", "Applicants", relatedEntityId);
        }

        if (relatedEntityType.indexOf("Employee") != -1) {
            return urlGenerator.action("Edit", "Employees", relatedEntityId);
        }

        return null;
    },
    urlToFile: function (attachmentId) {
        return urlGenerator.action("Get", "Attachments", attachmentId);
    }
};
