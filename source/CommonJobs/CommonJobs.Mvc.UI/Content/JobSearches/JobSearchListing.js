$(function() {
    var publicMarkTemplate = _.template($("#public-mark-template").html());
    var markPublicJobSearches = function ($card, item) {
        if (!item.jobSearch.IsPublic) return;

        $el = $(publicMarkTemplate({
            title: "Pública"
        }));

        $card.prepend($el);
        $card.addClass("public");
    };

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
        },
        prepareResultCard: function ($card, item) {
            markPublicJobSearches($card, item);
        }
    });

    $(".results").on("click", ".clickable", function(e) {
        e.preventDefault();
        window.location = $(this).find(".clickable-link").attr("href");
    });

    qs.search();
});