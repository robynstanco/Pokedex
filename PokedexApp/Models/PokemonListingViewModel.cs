using System;

namespace PokedexApp.Models
{
    public class PokemonListingViewModel
    {
        public string ImageURL { get; set; }

        public Guid? MyPokemonId { get; set; }

        public string Name { get; set; }

        public int? NationalDexPokemonId { get; set; }

        public string Nickname { get; set; }
    }
}