//TODO uncomment
/*
$(function () {
    var qs = new QuickSearchPage({
        //pageSize: 3,
        generateRedirectUrl: function (searchParameters) {
            return urlGenerator.action("Index", "Files", searchParameters);
        },
        generateSearchUrl: function (searchParameters) {
            return urlGenerator.action("List", "Files", searchParameters);
        },
        fillOtherSearchParameters: function (searchParameters) {
            //nothing to do here, the function is left for future addition of options
        }
    });

    $("#quickSearchSubmit").click(function () {
        //It also catches "enter"s in form inputs
        qs.redirect();
    });

    $(".results").on("click", ".clickable", function (e) {
        e.preventDefault();
        window.location = $(this).find(".clickable-link").attr("href");
    });

    qs.search();
});

*/