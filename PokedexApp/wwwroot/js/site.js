$(document).ready(function () {
    var pokemonDetailImageSelector = ".pokemon__item--large .pokemon__item-img";

    //Apply shake animation on hover of the large pokemon images
    $(pokemonDetailImageSelector).mouseover(function () {
        $(pokemonDetailImageSelector).effect("shake");
    });
});