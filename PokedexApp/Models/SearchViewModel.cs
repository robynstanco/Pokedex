using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace PokedexApp.Models
{
    public class SearchViewModel
    {
        public IEnumerable<SelectListItem> AbilityOptions { get; set; }
        public int? SelectedAbilityId { get; set; }

        public IEnumerable<SelectListItem> CategoryOptions { get; set; }
        public int? SelectedCategoryId { get; set; }

        public List<PokemonListingViewModel> FilteredPokemon { get; set; }

        public IEnumerable<SelectListItem> PokeballOptions { get; set; }
        public int? SelectedPokeballId { get; set; }
        
        public string SearchString { get; set; }

        public IEnumerable<SelectListItem> TypeOptions { get; set; }
        public int? SelectedTypeId { get; set; }
    }
}
