$(function() {
    var qs = new QuickSearchPage({
        //page: 3,
        generateRedirectUrl: function(searchParameters) {
            return urlGenerator.action("Index", "JobSearches", searchParameters);
        },
        generateSearchUrl: function(searchParameters) {
            return urlGenerator.action("List", "JobSearches", searchParameters);
        },
        fillOtherSearchParameters: function(searchParameters) {
            //nothing in here for the moment
        }
    });

    $(".results").on("click", ".clickable", function(e) {
        e.preventDefault();
        window.location = $(this).find(".clickable-link").attr("href");
    });

    qs.search();
});