using Pokedex.Common;
using Pokedex.Data.Models;
using Pokedex.Logging.Interfaces;
using Pokedex.Repository.Interfaces;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace PokedexApp.Logic
{
    public class PokedexAppLogic : IPokedexAppLogic
    {
        private const string InformationalMessageMappingWithCount = Constants.Mapping + " {0} {1} {2}.";
        private const string ViewModels = "View Models";

        IPokedexRepository _pokedexRepository;
        ILoggerAdapter<PokedexAppLogic> _logger;
        public PokedexAppLogic(IPokedexRepository pokedexRepository, ILoggerAdapter<PokedexAppLogic> logger)
        {
            _pokedexRepository = pokedexRepository;
            _logger = logger;
        }

        public List<PokemonListingViewModel> GetNationalDex()
        {
            return MapNationalDexLookupsToListingViewModels(_pokedexRepository.GetNationalDex());
        }

        public PokemonDetailViewModel GetNationalDexPokemonById(int id)
        {
            return MapNationalDexLookupToDetailViewModel(_pokedexRepository.GetNationalDexPokemonById(id));
        }

        private List<PokemonListingViewModel> MapNationalDexLookupsToListingViewModels(List<tlkpNationalDex> nationalDex)
        {
            _logger.LogInformation(string.Format(InformationalMessageMappingWithCount, nationalDex.Count, Constants.Pokemon, ViewModels));

            return nationalDex.Select(p => new PokemonListingViewModel
            {
                ImageURL = p.ImageURL,
                Name = p.Name,
                NationalDexPokemonId = p.Id
            }).ToList();
        }

        private PokemonDetailViewModel MapNationalDexLookupToDetailViewModel(tlkpNationalDex pokemon)
        {
            _logger.LogInformation(string.Format(InformationalMessageMappingWithCount, 1, Constants.Pokemon, ViewModels));

            return new PokemonDetailViewModel
            {
                Ability = pokemon.AbilityId.HasValue ? _pokedexRepository.GetAbilityById(pokemon.AbilityId.Value).Name : Constants.NotApplicable,
                Category = pokemon.CategoryId.HasValue ? _pokedexRepository.GetCategoryById(pokemon.CategoryId.Value).Name : Constants.NotApplicable,
                Description = pokemon.Description,
                HeightInInches = pokemon.HeightInInches,
                HiddenAbility = pokemon.HiddenAbilityId.HasValue ? _pokedexRepository.GetAbilityById(pokemon.HiddenAbilityId.Value).Name : Constants.NotApplicable,
                NationalDexPokemonId = pokemon.Id,
                ImageURL = pokemon.ImageURL,
                JapaneseName = pokemon.JapaneseName,
                Name = pokemon.Name,
                TypeOne = pokemon.TypeOneId.HasValue ? _pokedexRepository.GetTypeById(pokemon.TypeOneId.Value).Name : Constants.NotApplicable,
                TypeTwo = pokemon.TypeTwoId.HasValue ? _pokedexRepository.GetTypeById(pokemon.TypeTwoId.Value).Name : Constants.NotApplicable,
                WeightInPounds = pokemon.WeightInPounds
            };
        }
    }
}