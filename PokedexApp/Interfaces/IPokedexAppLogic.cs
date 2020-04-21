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
        Task<List<PokemonListingViewModel>> GetMyPokedex();
        Task<PokemonDetailViewModel> GetMyPokemonById(Guid id);
        List<PokemonListingViewModel> GetNationalDex();
        Task<PokemonDetailViewModel> GetNationalDexPokemonById(int id);
        PokemonFormViewModel GetNewPokemonForm();
        SearchViewModel GetSearchForm();
        Task<SearchViewModel> Search(SearchViewModel searchViewModel);
    }
}