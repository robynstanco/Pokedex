using Microsoft.AspNetCore.Mvc.Rendering;
using Pokedex.Common;
using Pokedex.Data.Models;
using Pokedex.Logging.Interfaces;
using Pokedex.Repository.Interfaces;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;
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

        public void AddPokemon(PokemonFormViewModel pokemonFormViewModel)
        {
            _pokedexRepository.AddPokemon(MapFormViewModelToMyPokemon(pokemonFormViewModel));
        }

        public void DeletePokemonById(Guid id)
        {
            _pokedexRepository.DeletePokemonById(id);

            _logger.LogInformation(Constants.Deleted + " " + Constants.Pokemon + ": " + id);
        }

        public List<PokemonListingViewModel> GetMyPokedex()
        {
            return MapPokedexToListingViewModels(_pokedexRepository.GetMyPokedex());
        }

        public PokemonDetailViewModel GetMyPokemonById(Guid id)
        {
            return MapMyPokemonToDetailViewModel(_pokedexRepository.GetMyPokemonById(id));
        }

        public List<PokemonListingViewModel> GetNationalDex()
        {
            return MapNationalDexLookupsToListingViewModels(_pokedexRepository.GetNationalDex());
        }

        public PokemonDetailViewModel GetNationalDexPokemonById(int id)
        {
            return MapNationalDexLookupToDetailViewModel(_pokedexRepository.GetNationalDexPokemonById(id));
        }

        public PokemonFormViewModel GetNewPokemonForm()
        {
            _logger.LogInformation(Constants.Mapping + " Select List Items.");

            return new PokemonFormViewModel()
            {
                NationalDexOptions = GetNationalDexSelectListItems(),
                PokeballOptions = GetPokeballSelectListItems(),
                SexOptions = GetPokemonSexSelectListItems()
            };
        }

        private List<SelectListItem> GetNationalDexSelectListItems()
        {
            List<SelectListItem> nationalDexSelectListItems = new List<SelectListItem>();

            nationalDexSelectListItems.AddRange(_pokedexRepository.GetNationalDex().Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }));

            return nationalDexSelectListItems;
        }

        private List<SelectListItem> GetPokeballSelectListItems()
        {
            List<SelectListItem> pokeballSelectListItems = new List<SelectListItem>();

            pokeballSelectListItems.AddRange(_pokedexRepository.GetAllPokeballs().Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }));

            return pokeballSelectListItems;
        }

        private List<SelectListItem> GetPokemonSexSelectListItems()
        {
            return new List<SelectListItem>() { new SelectListItem() { Text = "Female", Value = "0" },
                new SelectListItem() { Text = "Male", Value = "1" } };
        }

        private tblMyPokedex MapFormViewModelToMyPokemon(PokemonFormViewModel pokemonFormViewModel)
        {
            _logger.LogInformation(Constants.Mapping + " " + Constants.Pokemon + " " + ViewModels);

            return new tblMyPokedex()
            {
                Date = pokemonFormViewModel.Date,
                Id = Guid.NewGuid(),
                Level = pokemonFormViewModel.Level,
                Location = pokemonFormViewModel.Location,
                Nickname = pokemonFormViewModel.Nickname,
                PokeballId = pokemonFormViewModel.SelectedPokeballId,
                PokemonId = pokemonFormViewModel.SelectedNationalDexPokemonId,
                Sex = pokemonFormViewModel.SelectedSexId == 0 ? false : true
            };
        }

        private List<PokemonListingViewModel> MapPokedexToListingViewModels(List<tblMyPokedex> pokedex)
        {
            _logger.LogInformation(string.Format(InformationalMessageMappingWithCount, pokedex.Count, Constants.Pokemon, ViewModels));

            List<PokemonListingViewModel> pokemonListingViewModels = new List<PokemonListingViewModel>();

            foreach (tblMyPokedex pokemon in pokedex)
            {
                tlkpNationalDex nationalDexLookup = _pokedexRepository.GetNationalDexPokemonById(pokemon.PokemonId);

                pokemonListingViewModels.Add(new PokemonListingViewModel
                {
                    ImageURL = nationalDexLookup.ImageURL,
                    MyPokemonId = pokemon.Id,
                    Name = nationalDexLookup.Name,
                    Nickname = pokemon.Nickname,
                    NationalDexPokemonId = nationalDexLookup.Id,
                });
            }

            return pokemonListingViewModels;
        }

        private PokemonDetailViewModel MapMyPokemonToDetailViewModel(tblMyPokedex pokemon)
        {
            _logger.LogInformation(string.Format(InformationalMessageMappingWithCount, 1, Constants.Pokemon, ViewModels));

            tlkpNationalDex nationalDexLookup = _pokedexRepository.GetNationalDexPokemonById(pokemon.PokemonId);

            return new PokemonDetailViewModel
            {
                Ability = nationalDexLookup.AbilityId.HasValue ? _pokedexRepository.GetAbilityById(nationalDexLookup.AbilityId.Value).Name : Constants.NotApplicable,
                Category = nationalDexLookup.CategoryId.HasValue ? _pokedexRepository.GetCategoryById(nationalDexLookup.CategoryId.Value).Name : Constants.NotApplicable,
                Date = pokemon.Date,
                Description = nationalDexLookup.Description,
                HeightInInches = nationalDexLookup.HeightInInches,
                HiddenAbility = nationalDexLookup.HiddenAbilityId.HasValue ? _pokedexRepository.GetAbilityById(nationalDexLookup.HiddenAbilityId.Value).Name : Constants.NotApplicable,
                ImageURL = nationalDexLookup.ImageURL,
                JapaneseName = nationalDexLookup.JapaneseName,
                Level = pokemon.Level,
                Location = pokemon.Location,
                MyPokemonId = pokemon.Id,
                Name = nationalDexLookup.Name,
                NationalDexPokemonId = nationalDexLookup.Id,
                NationalDexSize = _pokedexRepository.GetNationalDex().Count,
                Nickname = pokemon.Nickname,
                PokeballImageURL = pokemon.PokeballId.HasValue ? _pokedexRepository.GetPokeballById(pokemon.PokeballId.Value).ImageURL : Constants.NotApplicable,
                Sex = pokemon.Sex,
                TypeOne = nationalDexLookup.TypeOneId.HasValue ? _pokedexRepository.GetTypeById(nationalDexLookup.TypeOneId.Value).Name : Constants.NotApplicable,
                TypeTwo = nationalDexLookup.TypeTwoId.HasValue ? _pokedexRepository.GetTypeById(nationalDexLookup.TypeTwoId.Value).Name : Constants.NotApplicable,
                WeightInPounds = nationalDexLookup.WeightInPounds
            };
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
                NationalDexSize = _pokedexRepository.GetNationalDex().Count,
                TypeOne = pokemon.TypeOneId.HasValue ? _pokedexRepository.GetTypeById(pokemon.TypeOneId.Value).Name : Constants.NotApplicable,
                TypeTwo = pokemon.TypeTwoId.HasValue ? _pokedexRepository.GetTypeById(pokemon.TypeTwoId.Value).Name : Constants.NotApplicable,
                WeightInPounds = pokemon.WeightInPounds
            };
        }
    }
}