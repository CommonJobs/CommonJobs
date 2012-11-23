/// <reference path="../../Scripts/jquery-1.7.2-vsdoc.js" />

//TODO convert into a jQuery plugin, set options
$(function () {
    $("body").on("click", ".collapsable-section-title", function (event) {
        var $clickedLink = $(event.target);
        var $dataContainer = $clickedLink.parent().next(".collapsable-section-data");
        var isVisible = $dataContainer.is(":visible");

        $dataContainer.toggleClass("hidden", isVisible);
        $clickedLink.text(isVisible ? "Mostrar" : "Ocultar");
    });
});