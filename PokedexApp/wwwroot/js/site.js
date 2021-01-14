$(document).ready(function () {
    var pokemonDetailImageSelector = ".pokemon__item--large .pokemon__item-img";

    $(pokemonDetailImageSelector).mouseover(function () {
        $(pokemonDetailImageSelector).effect("shake");
    });
});