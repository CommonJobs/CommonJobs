/// <reference path="underscore.js" />
/// <reference path="jquery-1.7.1-vsdoc.js" />

var UrlGenerator = function (baseUrl) {
    if (!_.isString(baseUrl))
        baseUrl = "";
    else if (baseUrl.slice(-1) == "/")
        baseUrl = baseUrl.slice(0, -1);

    var sharedAction = function (action, controller, id, sharedCode, parameters, fragment) {
        var sections = [controller, action, "shared", sharedCode];
        if (id) {
            sections = _.union(sections, id);
        }
        return bySections(sections, parameters, fragment);
    }

    var action = function (action, controller, id, parameters, fragment) {
        /// <summary>
        ///     Return action URL assuming this route format: "{controller}/{action}/{id}"
        ///     <para>    1 - action(action, controller, id, parameters, fragment) </para>
        ///     <para>    2 - action(action, controller, id, parameters) </para>
        ///     <para>    3 - action(action, controller, id, fragment) </para>
        ///     <para>    4 - action(action, controller, id) </para>
        ///     <para>    5 - action(action, controller, parameters, fragment) </para>
        ///     <para>    6 - action(action, controller, parameters) </para>
        ///     <para>    7 - action(action, controller) </para>
        /// </summary>
        /// <param name="action" type="String">
        ///     The name of the action
        /// </param>
        /// <param name="controller" type="String">
        ///     The name of the controler 
        /// </param>
        /// <param name="id" type="String">
        ///     Id of the entity or resource
        /// </param>
        /// <param name="parameters" type="Object">
        ///     parameters to send in the query string 
        /// </param>
        /// <param name="fragment" type="String">
        ///     After # fragment in the URL 
        /// </param>
        /// <returns type="string" />
        var sections = [controller, action];
        if (_.isObject(id)) {
            fragment = parameters;
            parameters = id;
        } else if (!_.isUndefined(id) && !_.isNull(id)) {
            sections = _.union(sections, id);
        }
        return bySections(sections, parameters, fragment);
    }

    var content = function (content) {
        return bySections(["Content", content]);
    }

    var bySections = function (sections, parameters, fragment) {
        /// <summary>
        ///     Return site URL ({baseUrl}/{section-1}/{...}/{section-n}?{parameters}#{fragment})
        ///     <para>    1 - bySections(sections, parameters, fragment) </para>
        ///     <para>    2 - bySections(sections, parameters) </para>
        ///     <para>    3 - bySections(sections, fragment) </para>
        ///     <para>    4 - bySections(sections) </para>
        /// </summary>
        /// <param name="sections" type="Array">
        ///     Url sections
        /// </param>
        /// <param name="parameters" type="Object">
        ///     parameters to send in the query string 
        /// </param>
        /// <param name="fragment" type="String">
        ///     After # fragment in the URL 
        /// </param>
        /// <returns type="string" />
        if (baseUrl.slice(-1) == "/")
            baseUrl.slice(0, -1);
        sections = _.union(baseUrl, sections);
        return byUrl(sections.join("/"), parameters, fragment);
    }

    var byUrl = function (url, parameters, fragment) {
        /// <summary>
        ///     Return site URL ({url}?{parameters}#{fragment})
        ///     <para>    1 - byUrl(url, parameters, fragment) </para>
        ///     <para>    2 - byUrl(url, parameters) </para>
        ///     <para>    3 - byUrl(url, fragment) </para>
        ///     <para>    4 - byUrl(url) </para>
        /// </summary>
        /// <param name="url" type="Array">
        ///     Base URL
        /// </param>
        /// <param name="parameters" type="Object">
        ///     parameters to send in the query string 
        /// </param>
        /// <param name="fragment" type="String">
        ///     After # fragment in the URL 
        /// </param>
        /// <returns type="string" />        
        if (!_.isObject(parameters)) {
            fragment = parameters;
            parameters = null;
        }
        if (url.indexOf("#") != -1) {
            url = url.split("#")[0];
        }
        //TODO: evaluate this possibility in case of current url reuse:
        // if (url.indexOf("?") != -1) {
        //     url = url.split("?")[0];
        // }

        if (!_.isEmpty(parameters)) {
            var hadQueryString = url.indexOf("?") != -1;
            var paramSeparator = hadQueryString ? "&" : "?";
            var params = $.param(parameters, true);
            url = url + paramSeparator + params;
        }
        if (!_.isUndefined(fragment) && !_.isNull(fragment))
            url = url + "#" + fragment;
        return url;
    }

    _.extend(this, {
        sharedAction: sharedAction,
        action: action,
        content: content,
        bySections: bySections,
        byUrl: byUrl
    });
};

window.urlGenerator = new UrlGenerator(window.baseUrl);

//Utilities
UrlGenerator.randomString = function (length, chars) {
    chars = chars || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";
    length = length || 12;
    var randomstring = '';
    for (var i = 0; i < length; i++) {
        var rnum = Math.floor(Math.random() * chars.length);
        randomstring += chars.substring(rnum, rnum + 1);
    }
    return randomstring;
}