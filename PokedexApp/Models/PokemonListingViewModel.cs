using System;

namespace PokedexApp.Models
{
    /// <summary>
    /// The Pokémon Listing View Model.
    /// </summary>
    public class PokemonListingViewModel
    {
        /// <summary>
        /// The Image Url.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// The Id.
        /// </summary>
        public Guid? MyPokemonId { get; set; }

        /// <summary>
        /// The Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The National Dex Id.
        /// </summary>
        public int? NationalDexPokemonId { get; set; }

        /// <summary>
        /// The Nickname.
        /// </summary>
        public string Nickname { get; set; }
    }
}