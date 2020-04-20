using PokedexApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokedexApp.Interfaces
{
    public interface IPokedexAppLogic
    {
        Task<PokemonFormViewModel> AddPokemon(PokemonFormViewModel pokemonFormViewModel);
        void DeletePokemonById(Guid id);
        Task<PokemonDetailViewModel> EditPokemon(PokemonDetailViewModel pokemonDetailViewModel);
        List<PokemonListingViewModel> GetMyPokedex();
        PokemonDetailViewModel GetMyPokemonById(Guid id);
        List<PokemonListingViewModel> GetNationalDex();
        PokemonDetailViewModel GetNationalDexPokemonById(int id);
        PokemonFormViewModel GetNewPokemonForm();
        SearchViewModel GetSearchForm();
        SearchViewModel Search(SearchViewModel searchViewModel);
    }
}