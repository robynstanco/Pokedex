using PokedexAPI.Models.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokedexAPI.Interfaces
{
    public interface IPokedexAPILogic
    {
        Task<List<LookupResult>> GetAllAbilities(int pageNumber, int pageSize);
        Task<List<LookupResult>> GetAllCategories(int pageNumber, int pageSize);
        Task<List<LookupResult>> GetAllPokeballs();
        Task<List<LookupResult>> GetAllTypes();
        Task<List<GenericPokemonResult>> GetNationalDex();

        Task<LookupResult> GetAbilityById(int id);
        Task<LookupResult> GetCategoryById(int id);
        Task<LookupResult> GetPokeballById(int id);
        Task<LookupResult> GetTypeById(int id);
        Task<GenericPokemonResult> GetNationalDexPokemonById(int id);
    }
}