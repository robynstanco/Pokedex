using Pokedex.Data.Models;
using System.Collections.Generic;

namespace Pokedex.Repository.Interfaces
{
    public interface IPokedexRepository
    {
        IEnumerable<tlkpAbility> GetAllAbilities();
        IEnumerable<tlkpCategory> GetAllCategories();
        IEnumerable<tlkpPokeball> GetAllPokeballs();
        IEnumerable<tlkpType> GetAllTypes();
        IEnumerable<tblMyPokedex> GetMyPokedex();
        IEnumerable<tlkpNationalDex> GetNationalDex();

        tlkpAbility GetAbilityById(int abilityId);
        tlkpCategory GetCategoryById(int categoryId);
        tblMyPokedex GetMyPokemonById(int pokemonId);
        tlkpNationalDex GetNationalDexPokemonById(int pokemonId);
        tlkpPokeball GetPokeballById(int pokeballId);
        tlkpType GetTypeById(int typeId);
        
        void AddPokemon(tblMyPokedex pokemon);
        void EditPokemon(tblMyPokedex pokemon);
        void DeletePokemonById(int pokemonId);

        IEnumerable<tlkpNationalDex> Search(string searchString, int? selectedAbilityId, int? selectedCategoryId, int? selectedTypeId);
        IEnumerable<tblMyPokedex> Search(string searchString, int? selectedAbilityId, int? selectedCategoryId, int? selectedTypeId, int? selectedPokeballId);
    }
}