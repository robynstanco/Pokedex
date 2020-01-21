using PokedexApp.Models;
using System;
using System.Collections.Generic;

namespace PokedexApp.Interfaces
{
    public interface IPokedexAppLogic
    {
        void AddPokemon(PokemonFormViewModel pokemonFormViewModel);
        void DeletePokemonById(Guid id);
        void EditPokemon(PokemonDetailViewModel pokemonDetailViewModel);
        List<PokemonListingViewModel> GetMyPokedex();
        PokemonDetailViewModel GetMyPokemonById(Guid id);
        List<PokemonListingViewModel> GetNationalDex();
        PokemonDetailViewModel GetNationalDexPokemonById(int id);
        PokemonFormViewModel GetNewPokemonForm();
        SearchViewModel GetSearchForm();
        SearchViewModel Search(SearchViewModel searchViewModel);
    }
}