using PokedexAPI.Models.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokedexAPI.Interfaces
{
    public interface IPokedexApiLogic
    {
        /// <summary>
        /// Get the abilities from repository and map to lookup result. Pagination applied.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>The paginated abilities.</returns>
        Task<List<LookupResult>> GetAbilities(int pageNumber, int pageSize);

        /// <summary>
        /// Get the categories from repository and map to lookup result. Pagination applied.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>The paginated categories.</returns>
        Task<List<LookupResult>> GetCategories(int pageNumber, int pageSize);

        Task<List<LookupResult>> GetAllPokeballs();
        Task<List<LookupResult>> GetAllTypes();

        /// <summary>
        /// Get the National Dex from repository and map to Pokemon results. Pagination applied.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The paginated National Dex.</returns>
        Task<List<GenericPokemonResult>> GetNationalDex(int pageNumber, int pageSize);

        Task<LookupResult> GetAbilityById(int id);
        Task<LookupResult> GetCategoryById(int id);
        Task<LookupResult> GetPokeballById(int id);
        Task<LookupResult> GetTypeById(int id);
        Task<GenericPokemonResult> GetNationalDexPokemonById(int id);
    }
}