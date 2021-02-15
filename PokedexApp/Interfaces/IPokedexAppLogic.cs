using PokedexApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokedexApp.Interfaces
{
    public interface IPokedexAppLogic
    {
        Task<PokemonFormViewModel> AddPokemon(PokemonFormViewModel pokemonFormViewModel);
        Task<Guid> DeletePokemonById(Guid id);
        Task<PokemonDetailViewModel> EditPokemon(PokemonDetailViewModel pokemonDetailViewModel);
        Task<List<PokemonListingViewModel>> GetMyPokedex();
        Task<PokemonDetailViewModel> GetMyPokemonById(Guid id);
        Task<List<PokemonListingViewModel>> GetNationalDex();
        Task<PokemonDetailViewModel> GetNationalDexPokemonById(int id);
        Task<PokemonFormViewModel> GetNewPokemonForm();
        Task<SearchViewModel> GetSearchForm();
        Task<SearchViewModel> Search(SearchViewModel searchViewModel);
    }
}