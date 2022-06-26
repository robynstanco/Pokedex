using Pokedex.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pokedex.Repository.Interfaces
{
    /// <summary>
    /// The Pokédex Repository.
    /// </summary>
    public interface IPokedexRepository
    {
        /// <summary>
        /// Add the given Pokémon to Pokédex context and save changes.
        /// </summary>
        /// <param name="pokemon">The entity to add.</param>
        /// <returns>The added entity.</returns>
        Task<tblMyPokedex> AddPokemon(tblMyPokedex pokemon);

        /// <summary>
        /// Delete the given Pokémon from Pokédex context and save changes.
        /// </summary>
        /// <param name="myPokemonId">The guid to find and delete.</param>
        /// <returns>The deleted entity.</returns>
        Task<tblMyPokedex> DeletePokemonById(Guid pokemonId);

        /// <summary>
        /// Delete and re-add updated entity to Pokédex context.
        /// </summary>
        /// <param name="pokemon">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        Task<tblMyPokedex> EditPokemon(tblMyPokedex pokemon);

        /// <summary>
        /// Get all the ability entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>All ability entities.</returns>
        Task<List<tlkpAbility>> GetAllAbilities();

        /// <summary>
        /// Get all the ability entities from context. Pagination applied.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>All paginated ability entities.</returns>
        Task<List<tlkpAbility>> GetAllAbilities(int pageNumber, int pageSize);

        /// <summary>
        /// Get all the category entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>All category entities.</returns>
        Task<List<tlkpCategory>> GetAllCategories();

        /// <summary>
        /// Get all Pokéball entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>All Pokéball entities.</returns>
        Task<List<tlkpPokeball>> GetAllPokeballs();

        /// <summary>
        /// Get all type entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>All type entities.</returns>
        Task<List<tlkpType>> GetAllTypes();

        /// <summary>
        /// Get all the Pokédex entities with lookup information from context. Results ordered by PokemonId ascending.
        /// </summary>
        /// <returns>All Pokédex entities.</returns>
        Task<List<tblMyPokedex>> GetMyPokedex();

        /// <summary>
        /// Get all national dex entities and subsequent lookup information from context. Results ordered Id ascending.
        /// </summary>
        /// <returns>All national dex Pokémon with related lookup information.</returns>
        Task<List<tlkpNationalDex>> GetNationalDex();

        /// <summary>
        /// Get the ability entity from a given abilityId.
        /// </summary>
        /// <param name="abilityId">The abilityId to find.</param>
        /// <returns>The found ability entity.</returns>
        Task<tlkpAbility> GetAbilityById(int abilityId);

        /// <summary>
        /// Get the category entity from a given categoryId.
        /// </summary>
        /// <param name="categoryId">The categoryId to find.</param>
        /// <returns>The found category entity.</returns>
        Task<tlkpCategory> GetCategoryById(int categoryId);

        /// <summary>
        /// Get the Pokédex entity from a given myPokemonId with lookup information from context.
        /// </summary>
        /// <param name="myPokemonId">The myPokemonId to find.</param>
        /// <returns>The found Pokédex entity.</returns>
        Task<tblMyPokedex> GetMyPokemonById(Guid pokemonId);

        /// <summary>
        /// Get the national dex entity from a given pokemonId.
        /// </summary>
        /// <param name="pokemonId">The pokemonId to find.</param>
        /// <returns>The found national dex Pokémon.</returns>
        Task<tlkpNationalDex> GetNationalDexPokemonById(int pokemonId);

        /// <summary>
        /// Get the Pokéball entity from a given pokeballId.
        /// </summary>
        /// <param name="pokeballId">The pokeballId to find.</param>
        /// <returns>The found Pokéball entity.</returns>
        Task<tlkpPokeball> GetPokeballById(int pokeballId);

        /// <summary>
        /// Get the type entity from a given typeId.
        /// </summary>
        /// <param name="typeId">The typeId to find.</param>
        /// <returns>The found type entity.</returns>
        Task<tlkpType> GetTypeById(int typeId);

        /// <summary>
        /// Search the National Pokédex based on a searchString and lookup parameters.
        /// Results filtered by occurence of string in name, Japanese name, and description.
        /// Additional filtering applied based on optional Pokémon characteristics. Results ordered by Id ascending.
        /// </summary>
        /// <param name="searchString">The string to filter on.</param>
        /// <param name="abilityId">The optional ability characteristic.</param>
        /// <param name="categoryId">The optional category characteristic.</param>
        /// <param name="typeId">The optional type characteristic.</param>
        /// <returns>The filtered list of Pokémon from the National Pokédex.</returns>
        Task<List<tlkpNationalDex>> Search(string searchString, int? abilityId, int? categoryId, int? typeId);

        /// <summary>
        /// Search the personal Pokédex based on a searchString and lookup parameters.
        /// Results filtered by occurence of string in name, Japanese name, description, and nickname.
        /// Additional filtering applied based on optional Pokémon characteristics.
        /// Results ordered by PokemonId ascending.
        /// </summary>
        /// <param name="searchString">The string to filter on.</param>
        /// <param name="abilityId">The optional ability characteristic.</param>
        /// <param name="categoryId">The optional category characteristic.</param>
        /// <param name="typeId">The optional type characteristic.</param>
        /// <param name="pokeballId">The optional Pokéball characteristic.</param>
        /// <returns>The filtered list of Pokémon from the personal Pokédex.</returns>
        Task<List<tblMyPokedex>> Search(string searchString, int? abilityId, int? categoryId, int? typeId, int? pokeballId);
    }
}