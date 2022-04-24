using PokedexApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokedexApp.Interfaces
{
    public interface IPokedexAppLogic
    {
        /// <summary>
        /// Add a Pokémon to the Pokédex.
        /// </summary>
        /// <param name="pokemonFormViewModel">The Pokémon to add.</param>
        /// <returns>The added Pokémon.</returns>
        Task<PokemonFormViewModel> AddPokemon(PokemonFormViewModel pokemonFormViewModel);

        /// <summary>
        /// Delete a Pokémon from the Pokédex by Id.
        /// </summary>
        /// <param name="id">The Pokémon Id.</param>
        /// <returns>The deleted Id.</returns>
        Task<Guid> DeletePokemonById(Guid id);

        /// <summary>
        /// Edit a given Pokémon.
        /// </summary>
        /// <param name="pokemonDetailViewModel">The Pokémon to edit.</param>
        /// <returns>The updated Pokémon.</returns>
        Task<PokemonDetailViewModel> EditPokemon(PokemonDetailViewModel pokemonDetailViewModel);

        /// <summary>
        /// Get the Pokédex.
        /// </summary>
        /// <returns>The Pokédex.</returns>
        Task<List<PokemonListingViewModel>> GetMyPokedex();

        /// <summary>
        /// Get a Pokédex Pokémon by Id.
        /// </summary>
        /// <param name="id">The Pokémon Id.</param>
        /// <returns></returns>
        Task<PokemonDetailViewModel> GetMyPokemonById(Guid id);

        /// <summary>
        /// Get the National Dex.
        /// </summary>
        /// <returns>The National Dex.</returns>
        Task<List<PokemonListingViewModel>> GetNationalDex();

        /// <summary>
        /// Get the National Dex Pokémon by Id.
        /// </summary>
        /// <param name="id">The National Dex Id.</param>
        /// <returns>The Pokémon detail.</returns>
        Task<PokemonDetailViewModel> GetNationalDexPokemonById(int id);

        /// <summary>
        /// Get the new Pokémon form with dropdown select list items.
        /// </summary>
        /// <returns>The new form.</returns>
        Task<PokemonFormViewModel> GetNewPokemonForm();

        /// <summary>
        /// Get the search form with dropdown select list items.
        /// </summary>
        /// <returns>The search form.</returns>
        Task<SearchViewModel> GetSearchForm();

        /// <summary>
        /// Search the personal & National Pokédex given search parameters.
        /// Only search National dex if Pokeball is not selected.
        /// </summary>
        /// <param name="searchViewModel">The search parameters to filter on.</param>
        /// <returns>The filtered search results.</returns>
        Task<SearchViewModel> Search(SearchViewModel searchViewModel);
    }
}