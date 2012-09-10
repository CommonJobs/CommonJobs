var FileSearchUtilities = {
    _getIconName: function (fileName) {
        if (!fileName) return "unknown";

        var extensionSeparatorLocation = fileName.lastIndexOf('.');
        var extension = fileName.substring(extensionSeparatorLocation + 1);
        var allowedExtensions = ['avi', 'bmp', 'doc', 'docx', 'gif', 'jpeg', 'jpg', 'mov', 'mp3',
            'mpeg', 'mpg', 'pdf', 'png', 'ppt', 'pptx', 'rar', 'txt', 'wmv', 'xls', 'xlsx', 'zip'];

        var iconName = _.find(allowedExtensions, function (aExt) {
            return aExt.toUpperCase() == extension.toUpperCase();
        });

        if (!iconName) iconName = "unknown";

        return iconName;
    },
    fileSmallIconFromExtension: function (fileName) {
        var iconName = this._getIconName(fileName);
        return urlGenerator.content("Images/filetypes/" + iconName + "-128.png");
    },
    fileIconFromExtension: function (fileName) {
        var iconName = this._getIconName(fileName);
        return urlGenerator.content("Images/filetypes/" + iconName + ".png");
    },
    
    urlToRelatedEntity: function (relatedEntityId, relatedEntityType) {
        if (relatedEntityId == null) return null;

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
    },
    normalizeNewLines: function (text) {
        return text.replace(/[\r\n]+|\s\s+/, '\n');
    },
    splitByNewLines: function (text) {
        return this.normalizeNewLines(text).split('\n');
    },
    searchHighlight: function (text, matchString) {
        if (!_.isString(matchString)) return text;

        matchString = matchString
            .replace(/[-[\]{}()+.,\\^$|#\s]/g, "\\$&")
            .replace(/\*/g, "[^\\\s]*")
            .replace(/\?/g, "[^\\\s]");

        if (!matchString) return text;

        var regex = new RegExp('(\\\s|^)(' + matchString + '[^\\\s]*)(\\\s|$)', 'gi');
        return text.replace(regex, '$1<span class="searchHighlighted">$2</span>$3');
    }
};
