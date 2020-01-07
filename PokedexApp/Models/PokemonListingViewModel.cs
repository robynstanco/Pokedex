using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokedexApp.Models
{
    public class PokemonListingViewModel
    {
        public string ImageURL { get; set; }
        public int MyPokemonId { get; set; }
        public string Name { get; set; }
        public int NationalDexPokemonId { get; set; }
        public string Nickname { get; set; }
    }
}
