///<reference path='jquery.d.ts' />
///<reference path='Knockout.d.ts' />
///<reference path='underscore.browser.d.ts' />

module Patches {
    //http://www.knockmeout.net/2012/05/quick-tip-skip-binding.html
    var miKo = <any>ko;
    miKo.bindingHandlers.stopBinding = {
        init: function () {
            return { controlsDescendantBindings: true };
        }
    };
    miKo.virtualElements.allowedBindings.stopBinding = true;
}

//From default example
$(document).ready(function () {

    $(".btn-slide").click(function () {
        $("#main").slideToggle("slow");
        $(this).toggleClass("active");
    });

});