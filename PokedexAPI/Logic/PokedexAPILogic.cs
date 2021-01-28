using Pokedex.Common;
using Pokedex.Data.Models;
using Pokedex.Logging.Interfaces;
using Pokedex.Repository.Interfaces;
using PokedexAPI.Interfaces;
using PokedexAPI.Models.Output;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokedexAPI.Logic
{
    //todo finish for api
    public class PokedexAPILogic : IPokedexAPILogic
    {
        private const string Results = nameof(Results);

        IPokedexRepository _pokedexRepository;
        ILoggerAdapter<PokedexAPILogic> _logger;
        public PokedexAPILogic(IPokedexRepository pokedexRepository, ILoggerAdapter<PokedexAPILogic> logger)
        {
            _pokedexRepository = pokedexRepository;
            _logger = logger;
        }

        public async Task<List<GenericLookupResult>> GetAllAbilities()
        {
            List<tlkpAbility> abilities = await _pokedexRepository.GetAllAbilities();

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, abilities.Count, Constants.Ability, Results));

            return abilities.Select(a => new GenericLookupResult
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();
        }

        public async Task<List<GenericLookupResult>> GetAllCategories()
        {
            List<tlkpCategory> categories = await _pokedexRepository.GetAllCategories();

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, categories.Count, Constants.Category, Results));

            return categories.Select(a => new GenericLookupResult
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();
        }

        public async Task<List<GenericLookupResult>> GetAllPokeballs()
        {
            List<tlkpPokeball> pokeballs = await _pokedexRepository.GetAllPokeballs();

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, pokeballs.Count, Constants.Pokeball, Results));

            return pokeballs.Select(a => new GenericLookupResult
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();
        }

        public async Task<List<GenericLookupResult>> GetAllTypes()
        {
            List<tlkpType> types = await _pokedexRepository.GetAllTypes();

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, types.Count, Constants.Types, Results));

            return types.Select(a => new GenericLookupResult
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();
        }

        public async Task<GenericLookupResult> GetAbilityById(int id)
        {
            tlkpAbility ability = await _pokedexRepository.GetAbilityById(id);

            _logger.LogInformation(Constants.Mapping + " " + Constants.Ability + " " + Results + ".");

            return ability == null ? null : new GenericLookupResult()
            {
                Id = ability.Id,
                Name = ability.Name
            };
        }

        public async Task<GenericLookupResult> GetCategoryById(int id)
        {
            tlkpCategory category = await _pokedexRepository.GetCategoryById(id);

            _logger.LogInformation(Constants.Mapping + " " + Constants.Category + " " + Results + ".");

            return category == null ? null : new GenericLookupResult()
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<GenericLookupResult> GetPokeballById(int id)
        {
            tlkpPokeball pokeball = await _pokedexRepository.GetPokeballById(id);

            _logger.LogInformation(Constants.Mapping + " " + Constants.Pokeball + " " + Results + ".");

            return pokeball == null ? null : new GenericLookupResult()
            {
                Id = pokeball.Id,
                Name = pokeball.Name
            };
        }

        public async Task<GenericLookupResult> GetTypeById(int id)
        {
            tlkpType type = await _pokedexRepository.GetTypeById(id);

            _logger.LogInformation(Constants.Mapping + " " + Constants.Type + " " + Results + ".");

            return type == null ? null : new GenericLookupResult()
            {
                Id = type.Id,
                Name = type.Name
            };
        }
    }
}