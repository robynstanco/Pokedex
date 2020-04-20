"use strict";

$(document).ready(function () {
    //Selectors
    var pokemonDetailImageSelector = ".pokemon__item--large .pokemon__item-img";

    //Event Handlers
    $(pokemonDetailImageSelector).mouseover(function () {
        $(pokemonDetailImageSelector).effect("shake");
    });
});

