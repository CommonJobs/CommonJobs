///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='underscore.browser.d.ts' />
var Patches;
(function (Patches) {
    //http://www.knockmeout.net/2012/05/quick-tip-skip-binding.html
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
//From default example

$(document).ready(function () {
    $(".btn-slide").click(function () {
        $("#main").slideToggle("slow");
        $(this).toggleClass("active");
    });
});
