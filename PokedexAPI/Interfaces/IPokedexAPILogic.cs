using PokedexAPI.Models.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokedexAPI.Interfaces
{
    public interface IPokedexAPILogic
    {
        Task<List<GenericLookupResult>> GetAllAbilities();
        Task<List<GenericLookupResult>> GetAllCategories();

        Task<GenericLookupResult> GetAbilityById(int id);
        Task<GenericLookupResult> GetCategoryById(int id);
    }
}