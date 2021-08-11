"use strict";

$(document).ready(function () {
    var pokemonDetailImageSelector = ".pokemon__item--large .pokemon__item-img";

    //Apply shake animation on hover of the large Pokémon images
    $(pokemonDetailImageSelector).mouseover(function () {
        $(pokemonDetailImageSelector).effect("shake");
    });
});
