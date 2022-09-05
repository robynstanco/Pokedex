using AutoMapper;
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
    //todo finish for api, automapper, comments
    public class PokedexAPILogic : IPokedexAPILogic
    {
        private const string Results = nameof(Results);

        IPokedexRepository _pokedexRepository;
        ILoggerAdapter<PokedexAPILogic> _logger;
        IMapper _mapper;
        public PokedexAPILogic(IPokedexRepository pokedexRepository, ILoggerAdapter<PokedexAPILogic> logger, IMapper mapper)
        {
            _pokedexRepository = pokedexRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<LookupResult>> GetAllAbilities(int pageNumber, int pageSize)
        {
            List<tlkpAbility> abilities = await _pokedexRepository.GetAllAbilities(pageNumber, pageSize);

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, abilities.Count, Constants.Ability, Results));

            var abilityLookupResults = _mapper.Map<List<LookupResult>>(abilities);

            return abilityLookupResults;
        }

        public async Task<List<LookupResult>> GetAllCategories(int pageNumber, int pageSize)
        {
            List<tlkpCategory> categories = await _pokedexRepository.GetAllCategories(pageNumber, pageSize);

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, categories.Count, Constants.Category, Results));

            var categoryLookupResults = _mapper.Map<List<LookupResult>>(categories);

            return categoryLookupResults;
        }

        public async Task<List<LookupResult>> GetAllPokeballs()
        {
            List<tlkpPokeball> pokeballs = await _pokedexRepository.GetAllPokeballs();

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, pokeballs.Count, Constants.Pokeball, Results));

            var pokeballLookupResults = _mapper.Map<List<LookupResult>>(pokeballs);

            return pokeballLookupResults;
        }

        public async Task<List<LookupResult>> GetAllTypes()
        {
            List<tlkpType> types = await _pokedexRepository.GetAllTypes();

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, types.Count, Constants.Types, Results));

            var typeLookupResults = _mapper.Map<List<LookupResult>>(types);

            return typeLookupResults;
        }

        public async Task<List<GenericPokemonResult>> GetNationalDex()
        {
            List<tlkpNationalDex> nationalDex = await _pokedexRepository.GetNationalDex();

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, nationalDex.Count, Constants.NationalDex, Results));

            return nationalDex.Select(a => new GenericPokemonResult 
            { 
                Ability = a.Ability.Name,
                Category = a.Category.Name,
                Description = a.Description,
                HeightInInches = a.HeightInInches,
                HiddenAbility = a.HiddenAbilityId.HasValue ? a.HiddenAbility.Name : Constants.NotApplicable,
                ImageURL = a.ImageURL,
                JapaneseName = a.JapaneseName,
                Name = a.Name,
                NationalDexPokemonId = a.Id,
                TypeOne = a.TypeOne.Name,
                TypeTwo = a.TypeTwoId.HasValue ? a.TypeTwo.Name : Constants.NotApplicable,
                WeightInPounds = a.WeightInPounds
            }).ToList();
        }

        public async Task<LookupResult> GetAbilityById(int id)
        {
            tlkpAbility ability = await _pokedexRepository.GetAbilityById(id);

            _logger.LogInformation(Constants.Mapping + " " + Constants.Ability + " " + Results + ".");

            var abilityLookupResult = _mapper.Map<LookupResult>(ability);

            return abilityLookupResult;
        }

        public async Task<LookupResult> GetCategoryById(int id)
        {
            tlkpCategory category = await _pokedexRepository.GetCategoryById(id);

            _logger.LogInformation(Constants.Mapping + " " + Constants.Category + " " + Results + ".");

            var categoryLookupResult = _mapper.Map<LookupResult>(category);

            return categoryLookupResult;
        }

        public async Task<LookupResult> GetPokeballById(int id)
        {
            tlkpPokeball pokeball = await _pokedexRepository.GetPokeballById(id);

            _logger.LogInformation(Constants.Mapping + " " + Constants.Pokeball + " " + Results + ".");

            var pokeballLookupResult = _mapper.Map<LookupResult>(pokeball);

            return pokeballLookupResult;
        }

        public async Task<LookupResult> GetTypeById(int id)
        {
            tlkpType type = await _pokedexRepository.GetTypeById(id);

            _logger.LogInformation(Constants.Mapping + " " + Constants.Type + " " + Results + ".");

            var typeLookupResult = _mapper.Map<LookupResult>(type);

            return typeLookupResult;
        }

        public async Task<GenericPokemonResult> GetNationalDexPokemonById(int id)
        {
            tlkpNationalDex nationalDex = await _pokedexRepository.GetNationalDexPokemonById(id);

            _logger.LogInformation(Constants.Mapping + " " + Constants.NationalDex + " " + Results + ".");

            return nationalDex == null ? null : new GenericPokemonResult
            {
                Ability = nationalDex.Ability.Name,
                Category = nationalDex.Category.Name,
                Description = nationalDex.Description,
                HeightInInches = nationalDex.HeightInInches,
                HiddenAbility = nationalDex.HiddenAbilityId.HasValue ? nationalDex.HiddenAbility.Name : Constants.NotApplicable,
                ImageURL = nationalDex.ImageURL,
                JapaneseName = nationalDex.JapaneseName,
                Name = nationalDex.Name,
                NationalDexPokemonId = nationalDex.Id,
                TypeOne = nationalDex.TypeOne.Name,
                TypeTwo = nationalDex.TypeTwoId.HasValue ? nationalDex.TypeTwo.Name : Constants.NotApplicable,
                WeightInPounds = nationalDex.WeightInPounds
            };
        }
    }
}