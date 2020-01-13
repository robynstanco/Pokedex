using System;

namespace PokedexApp.Models
{
    public class PokemonListingViewModel
    {
        public string ImageURL { get; set; }
        public string ImageSource {
            get
            {
                return ImageURL != null ? string.Format("data:image/png;base64,{0}", ImageURL) : null;
            }
        }
        public Guid? MyPokemonId { get; set; }
        public string Name { get; set; }
        public int? NationalDexPokemonId { get; set; }
        public string Nickname { get; set; }
    }
}