using PokedexApp.Models;
using System.Collections.Generic;

namespace PokedexApp.Interfaces
{
    public interface IPokedexAppLogic
    {
        List<PokemonListingViewModel> GetNationalDex();
        PokemonDetailViewModel GetNationalDexPokemonById(int id);
    }
}