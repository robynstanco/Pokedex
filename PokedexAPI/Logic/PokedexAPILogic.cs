using Microsoft.AspNetCore.Mvc;
using Pokedex.Data.Models;
using Pokedex.Repository.Interfaces;
using PokedexAPI.Interfaces;
using PokedexAPI.Models.Output;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokedexAPI.Logic
{
    public class PokedexAPILogic : IPokedexAPILogic
    {
        IPokedexRepository _pokedexRepository;
        public PokedexAPILogic(IPokedexRepository pokedexRepository)
        {
            _pokedexRepository = pokedexRepository;
        }

        public async Task<GenericLookupResult> GetAbilityById(int id)
        {
            tlkpAbility ability = await _pokedexRepository.GetAbilityById(id);

            return ability == null ? null : new GenericLookupResult()
            {
                Id = ability.Id,
                Name = ability.Name
            };
        }

        public async Task<ActionResult<List<GenericLookupResult>>> GetAllAbilities()
        {
            List<tlkpAbility> abilities = await _pokedexRepository.GetAllAbilities();

            return abilities.Select(a => new GenericLookupResult
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();
        }
    }
}