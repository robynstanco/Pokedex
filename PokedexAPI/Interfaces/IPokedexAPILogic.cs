using PokedexAPI.Models.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokedexAPI.Interfaces
{
    public interface IPokedexAPILogic
    {
        Task<List<GenericLookupResult>> GetAllAbilities();
        Task<List<GenericLookupResult>> GetAllCategories();
        Task<List<GenericLookupResult>> GetAllPokeballs();
        Task<List<GenericLookupResult>> GetAllTypes();
        Task<List<GenericPokemonResult>> GetNationalDex();

        Task<GenericLookupResult> GetAbilityById(int id);
        Task<GenericLookupResult> GetCategoryById(int id);
        Task<GenericLookupResult> GetPokeballById(int id);
        Task<GenericLookupResult> GetTypeById(int id);
        Task<GenericPokemonResult> GetNationalDexPokemonById(int id);
    }
}