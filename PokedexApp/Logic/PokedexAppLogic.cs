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

        public List<PokemonViewModel> GetMyPokedex()
        {
            return MapPokedexTableToViewModels(_pokedexRepository.GetMyPokedex());
        }

        public List<PokemonViewModel> GetNationalDex()
        {
            return MapNationalDexLookupsToViewModels(_pokedexRepository.GetNationalDex());
        }
        
        private List<PokemonViewModel> MapPokedexTableToViewModels(List<tblMyPokedex> pokedex)
        {
            _logger.LogInformation(string.Format(InformationalMessageMappingWithCount, pokedex.Count, Constants.Pokemon, ViewModels));

            List<PokemonViewModel> pokemonViewModels = new List<PokemonViewModel>();

            foreach(tblMyPokedex pokemon in pokedex)
            {
                tlkpNationalDex nationalDexLookup = _pokedexRepository.GetNationalDexPokemonById(pokemon.PokemonId);

                pokemonViewModels.Add(new PokemonViewModel
                {
                    Ability = nationalDexLookup.AbilityId.HasValue ? _pokedexRepository.GetAbilityById(nationalDexLookup.AbilityId.Value).Name : Constants.NotApplicable,
                    Category = nationalDexLookup.CategoryId.HasValue ? _pokedexRepository.GetCategoryById(nationalDexLookup.CategoryId.Value).Name : Constants.NotApplicable,
                    Date = pokemon.Date,
                    Description = nationalDexLookup.Description,
                    HeightInInches = nationalDexLookup.HeightInInches,
                    HiddenAbility = nationalDexLookup.HiddenAbilityId.HasValue ? _pokedexRepository.GetAbilityById(nationalDexLookup.HiddenAbilityId.Value).Name : Constants.NotApplicable,
                    Id = pokemon.PokemonId,
                    ImageURL = nationalDexLookup.ImageURL,
                    JapaneseName = nationalDexLookup.JapaneseName,
                    Level = pokemon.Level,
                    Location = pokemon.Location,
                    Name = nationalDexLookup.Name,
                    Nickname = pokemon.Nickname,
                    Pokeball = pokemon.PokeballId.HasValue ? _pokedexRepository.GetPokeballById(pokemon.PokeballId.Value).Name : Constants.NotApplicable,
                    Sex = pokemon.Sex,
                    TypeOne = nationalDexLookup.TypeOneId.HasValue ? _pokedexRepository.GetTypeById(nationalDexLookup.TypeOneId.Value).Name : Constants.NotApplicable,
                    TypeTwo = nationalDexLookup.TypeTwoId.HasValue ? _pokedexRepository.GetTypeById(nationalDexLookup.TypeTwoId.Value).Name : Constants.NotApplicable,
                    WeightInPounds = nationalDexLookup.WeightInPounds
                });
            }

            return pokemonViewModels;
        }

        private List<PokemonViewModel> MapNationalDexLookupsToViewModels(List<tlkpNationalDex> nationalDex)
        {
            _logger.LogInformation(string.Format(InformationalMessageMappingWithCount, nationalDex.Count, Constants.Pokemon, ViewModels));

            return nationalDex.Select(p => new PokemonViewModel
            {
                Ability = p.AbilityId.HasValue ? _pokedexRepository.GetAbilityById(p.AbilityId.Value).Name : Constants.NotApplicable,
                Category = p.CategoryId.HasValue ? _pokedexRepository.GetCategoryById(p.CategoryId.Value).Name : Constants.NotApplicable,
                Description = p.Description,
                HeightInInches = p.HeightInInches,
                HiddenAbility = p.HiddenAbilityId.HasValue ? _pokedexRepository.GetAbilityById(p.HiddenAbilityId.Value).Name : Constants.NotApplicable,
                Id = p.Id,
                ImageURL = p.ImageURL,
                JapaneseName = p.JapaneseName,
                Name = p.Name,
                TypeOne = p.TypeOneId.HasValue ? _pokedexRepository.GetTypeById(p.TypeOneId.Value).Name : Constants.NotApplicable,
                TypeTwo = p.TypeTwoId.HasValue ? _pokedexRepository.GetTypeById(p.TypeTwoId.Value).Name : Constants.NotApplicable,
                WeightInPounds = p.WeightInPounds
            }).ToList();
        }
    }
}