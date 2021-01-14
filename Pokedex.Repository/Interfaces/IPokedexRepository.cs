using Pokedex.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pokedex.Repository.Interfaces
{
    public interface IPokedexRepository
    {
        Task<tblMyPokedex> AddPokemon(tblMyPokedex pokemon);
        Task<tblMyPokedex> DeletePokemonById(Guid pokemonId);
        Task<tblMyPokedex> EditPokemon(tblMyPokedex pokemon);

        Task<List<tlkpAbility>> GetAllAbilities();
        Task<List<tlkpCategory>> GetAllCategories();
        Task<List<tlkpPokeball>> GetAllPokeballs();
        Task<List<tlkpType>> GetAllTypes();
        Task<List<tblMyPokedex>> GetMyPokedex();
        Task<List<tlkpNationalDex>> GetNationalDex();

        Task<tlkpAbility> GetAbilityById(int abilityId);
        Task<tlkpCategory> GetCategoryById(int categoryId);
        Task<tblMyPokedex> GetMyPokemonById(Guid pokemonId);
        Task<tlkpNationalDex> GetNationalDexPokemonById(int pokemonId);
        Task<tlkpPokeball> GetPokeballById(int pokeballId);
        Task<tlkpType> GetTypeById(int typeId);

        Task<List<tlkpNationalDex>> Search(string searchString, int? selectedAbilityId, int? selectedCategoryId, int? selectedTypeId);
        Task<List<tblMyPokedex>> Search(string searchString, int? selectedAbilityId, int? selectedCategoryId, int? selectedTypeId, int? selectedPokeballId);
    }
}