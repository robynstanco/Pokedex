using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace PokedexApp.Models
{
    /// <summary>
    /// The Search View Model.
    /// </summary>
    public class SearchViewModel
    {
        /// <summary>
        /// The ability options.
        /// </summary>
        public IEnumerable<SelectListItem> AbilityOptions { get; set; }

        /// <summary>
        /// The selected ability Id.
        /// </summary>
        public int? SelectedAbilityId { get; set; }

        /// <summary>
        /// The category options.
        /// </summary>
        public IEnumerable<SelectListItem> CategoryOptions { get; set; }

        /// <summary>
        /// The selected category Id.
        /// </summary>
        public int? SelectedCategoryId { get; set; }

        /// <summary>
        /// The filtered results.
        /// </summary>
        public List<PokemonListingViewModel> FilteredPokemon { get; set; }

        /// <summary>
        /// The Pokéball options.
        /// </summary>
        public IEnumerable<SelectListItem> PokeballOptions { get; set; }

        /// <summary>
        /// The selected Pokéball Id.
        /// </summary>
        public int? SelectedPokeballId { get; set; }
        
        /// <summary>
        /// The search string.
        /// </summary>
        public string SearchString { get; set; }

        /// <summary>
        /// The type options.
        /// </summary>
        public IEnumerable<SelectListItem> TypeOptions { get; set; }

        /// <summary>
        /// The selected type Id.
        /// </summary>
        public int? SelectedTypeId { get; set; }
    }
}
