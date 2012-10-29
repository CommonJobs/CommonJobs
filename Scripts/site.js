var Patches;
(function (Patches) {
    $.fn.datepicker.defaults = {
        autoclose: true,
        language: 'es',
        format: 'dd/mm/yyyy'
    };
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
    $(".btn-slide").click(function () {
        $("#main").slideToggle("slow");
        $(this).toggleClass("active");
    });
});
