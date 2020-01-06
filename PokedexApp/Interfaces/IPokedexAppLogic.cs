using PokedexApp.Models;
using System.Collections.Generic;

namespace PokedexApp.Interfaces
{
    public interface IPokedexAppLogic
    {
        List<PokemonViewModel> GetMyPokedex();
        List<PokemonViewModel> GetNationalDex();
    }
}