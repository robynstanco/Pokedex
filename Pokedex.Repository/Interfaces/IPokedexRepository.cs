using Pokedex.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pokedex.Repository.Interfaces
{
    public interface IPokedexRepository
    {
        List<tlkpAbility> GetAllAbilities();
        List<tlkpCategory> GetAllCategories();
        List<tlkpPokeball> GetAllPokeballs();
        List<tlkpType> GetAllTypes();
        Task<List<tblMyPokedex>> GetMyPokedex();
        List<tlkpNationalDex> GetNationalDex();

        Task<tlkpAbility> GetAbilityById(int abilityId);
        tlkpCategory GetCategoryById(int categoryId);
        Task<tblMyPokedex> GetMyPokemonById(Guid pokemonId);
        Task<tlkpNationalDex> GetNationalDexPokemonById(int pokemonId);
        tlkpPokeball GetPokeballById(int pokeballId);
        tlkpType GetTypeById(int typeId);
        
        Task<tblMyPokedex> AddPokemon(tblMyPokedex pokemon);
        Task<tblMyPokedex> EditPokemon(tblMyPokedex pokemon);
        Task<tblMyPokedex> DeletePokemonById(Guid pokemonId);

        List<tlkpNationalDex> Search(string searchString, int? selectedAbilityId, int? selectedCategoryId, int? selectedTypeId);
        Task<List<tblMyPokedex>> Search(string searchString, int? selectedAbilityId, int? selectedCategoryId, int? selectedTypeId, int? selectedPokeballId);
    }
}