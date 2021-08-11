using Pokedex.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pokedex.Repository.Interfaces
{
    public interface IPokedexRepository
    {
        /// <summary>
        /// Add the given Pokémon to Pokédex context & save changes.
        /// </summary>
        /// <param name="pokemon">entity to add</param>
        /// <returns>the added entity</returns>
        Task<tblMyPokedex> AddPokemon(tblMyPokedex pokemon);

        /// <summary>
        /// Delete the given Pokémon from Pokédex context & save changes.
        /// </summary>
        /// <param name="myPokemonId">guid to find & delete</param>
        /// <returns>deleted entity</returns>
        Task<tblMyPokedex> DeletePokemonById(Guid pokemonId);

        /// <summary>
        /// Delete & re-add updated entity to Pokédex context.
        /// </summary>
        /// <param name="pokemon">entity to update</param>
        /// <returns>updated entity</returns>
        Task<tblMyPokedex> EditPokemon(tblMyPokedex pokemon);


        /// <summary>
        /// Get all the ability entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>all ability entities</returns>
        Task<List<tlkpAbility>> GetAllAbilities();

        /// <summary>
        /// Get all the category entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>all category entities</returns>
        Task<List<tlkpCategory>> GetAllCategories();

        /// <summary>
        /// Get all Pokéball entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>all Pokéball entities</returns>
        Task<List<tlkpPokeball>> GetAllPokeballs();

        /// <summary>
        /// Get all type entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>all type entities</returns>
        Task<List<tlkpType>> GetAllTypes();

        /// <summary>
        /// Get all the Pokédex entities with lookup information from context. Results ordered by PokemonId ascending.
        /// </summary>
        /// <returns>all Pokédex entities</returns>
        Task<List<tblMyPokedex>> GetMyPokedex();

        /// <summary>
        /// Get all national dex entities and subsequent lookup information from context. Results ordered Id ascending.
        /// </summary>
        /// <returns>all national dex Pokémon with related lookup information</returns>
        Task<List<tlkpNationalDex>> GetNationalDex();


        /// <summary>
        /// Get the ability entity from a given abilityId.
        /// </summary>
        /// <param name="abilityId">abilityId to find</param>
        /// <returns>the found ability entity</returns>
        Task<tlkpAbility> GetAbilityById(int abilityId);

        /// <summary>
        /// Get the category entity from a given categoryId.
        /// </summary>
        /// <param name="categoryId">categoryId to find</param>
        /// <returns>the found category entity</returns>
        Task<tlkpCategory> GetCategoryById(int categoryId);

        /// <summary>
        /// Get the Pokédex entity from a given myPokemonId with lookup information from context.
        /// </summary>
        /// <param name="myPokemonId">the myPokemonId to find</param>
        /// <returns>the found Pokédex entity</returns>
        Task<tblMyPokedex> GetMyPokemonById(Guid pokemonId);

        /// <summary>
        /// Get the national dex entity from a given pokemonId.
        /// </summary>
        /// <param name="pokemonId">the pokemonId to find</param>
        /// <returns>the found national dex Pokémon</returns>
        Task<tlkpNationalDex> GetNationalDexPokemonById(int pokemonId);

        /// <summary>
        /// Get the Pokéball entity from a given pokeballId.
        /// </summary>
        /// <param name="pokeballId">the pokeballId to find</param>
        /// <returns>the found Pokéball entity</returns>
        Task<tlkpPokeball> GetPokeballById(int pokeballId);

        /// <summary>
        /// Get the type entity from a given typeId.
        /// </summary>
        /// <param name="typeId">the typeId to find</param>
        /// <returns>the found type entity</returns>
        Task<tlkpType> GetTypeById(int typeId);


        /// <summary>
        /// Search the National Pokédex based on a searchString and lookup parameters.
        /// Results filtered by occurence of string in name, Japanese name, & description.
        /// Additional filtering applied based on optional Pokémon characteristics. Results ordered by Id ascending.
        /// </summary>
        /// <param name="searchString">the string to filter on</param>
        /// <param name="abilityId">optional ability characteristic</param>
        /// <param name="categoryId">otpinal category characteristic</param>
        /// <param name="typeId">optional type characteristic</param>
        /// <returns>filtered list of Pokémon from the National Pokédex</returns>
        Task<List<tlkpNationalDex>> Search(string searchString, int? abilityId, int? categoryId, int? typeId);

        /// <summary>
        /// Search the personal Pokédex based on a searchString and lookup parameters.
        /// Results filtered by occurence of string in name, Japanese name, description, & nickname.
        /// Additional filtering applied based on optional Pokémon characteristics.
        /// Results ordered by PokemonId ascending.
        /// </summary>
        /// <param name="searchString">the string to filter on</param>
        /// <param name="abilityId">optional ability characteristic</param>
        /// <param name="categoryId">otpinal category characteristic</param>
        /// <param name="typeId">optional type characteristic</param>
        /// <param name="pokeballId">optional Pokéball characteristic</param>
        /// <returns>a filtered list of Pokémon from the personal Pokédex</returns>
        Task<List<tblMyPokedex>> Search(string searchString, int? abilityId, int? categoryId, int? typeId, int? pokeballId);
    }
}