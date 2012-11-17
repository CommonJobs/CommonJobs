var Patches;
(function (Patches) {
    var miKo = ko;
    miKo.bindingHandlers.stopBinding = {
        init: function () {
            return {
                controlsDescendantBindings: true
            };
        }
    };
    miKo.virtualElements.allowedBindings.stopBinding = true;
})(Patches || (Patches = {}));

$(document).ready(function () {
    moment.lang('es');
    $(".btn-slide").click(function () {
        $("#main").slideToggle("slow");
        $(this).toggleClass("active");
    });
});
