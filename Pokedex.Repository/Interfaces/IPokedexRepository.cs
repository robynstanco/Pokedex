using Pokedex.Data.Models;
using System.Collections.Generic;

namespace Pokedex.Repository.Interfaces
{
    public interface IPokedexRepository
    {
        List<tlkpAbility> GetAllAbilities();
        List<tlkpCategory> GetAllCategories();
        List<tlkpPokeball> GetAllPokeballs();
        List<tlkpType> GetAllTypes();
        List<tblMyPokedex> GetMyPokedex();
        List<tlkpNationalDex> GetNationalDex();

        tlkpAbility GetAbilityById(int abilityId);
        tlkpCategory GetCategoryById(int categoryId);
        tblMyPokedex GetMyPokemonById(int pokemonId);
        tlkpNationalDex GetNationalDexPokemonById(int pokemonId);
        tlkpPokeball GetPokeballById(int pokeballId);
        tlkpType GetTypeById(int typeId);
        
        void AddPokemon(tblMyPokedex pokemon);
        void EditPokemon(tblMyPokedex pokemon);

        tblMyPokedex DeletePokemonById(int pokemonId);

        List<tlkpNationalDex> Search(string searchString, int? selectedAbilityId, int? selectedCategoryId, int? selectedTypeId);
        List<tblMyPokedex> Search(string searchString, int? selectedAbilityId, int? selectedCategoryId, int? selectedTypeId, int? selectedPokeballId);
    }
}