using Microsoft.AspNetCore.Mvc;
using PokedexAPI.Models.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokedexAPI.Interfaces
{
    public interface IPokedexAPILogic
    {
        Task<ActionResult<List<GenericLookupResult>>> GetAllAbilities();
        Task<GenericLookupResult> GetAbilityById(int id);
    }
}
