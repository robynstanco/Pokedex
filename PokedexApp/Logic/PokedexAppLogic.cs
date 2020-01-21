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
        private const string SelectListItems = " Select List Items.";
        private const string None = "--None--";

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

        public void EditPokemon(PokemonDetailViewModel pokemonDetailViewModel)
        {
            _pokedexRepository.EditPokemon(MapDetailViewModelToMyPokemon(pokemonDetailViewModel));
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

        public SearchViewModel GetSearchForm()
        {
            _logger.LogInformation(Constants.Mapping + SelectListItems);

            return new SearchViewModel()
            {
                AbilityOptions = GetAbilitySelectListItems(),
                CategoryOptions = GetCategorySelectListItems(),
                PokeballOptions = GetPokeballSelectListItems(),
                TypeOptions = GetTypeSelectListItems()
            };
        }

        public SearchViewModel Search(SearchViewModel searchViewModel)
        {
            SearchViewModel finalSearchViewModel = GetSearchForm();

            int? selectedAbilityId = searchViewModel.SelectedAbilityId;
            int? selectedCategoryId = searchViewModel.SelectedCategoryId;
            int? selectedPokeballId = searchViewModel.SelectedPokeballId;
            int? selectedTypeId = searchViewModel.SelectedTypeId;

            finalSearchViewModel.SelectedAbilityId = selectedAbilityId;
            finalSearchViewModel.SelectedCategoryId = selectedCategoryId;
            finalSearchViewModel.SelectedPokeballId = selectedPokeballId;
            finalSearchViewModel.SelectedTypeId = selectedTypeId;

            finalSearchViewModel.FilteredPokemon = new List<PokemonListingViewModel>();

            finalSearchViewModel.FilteredPokemon.AddRange(MapNationalDexLookupsToListingViewModels(
                _pokedexRepository.Search(searchViewModel.SearchString, selectedAbilityId, selectedCategoryId, selectedTypeId)));

            finalSearchViewModel.FilteredPokemon.AddRange(MapPokedexToListingViewModels(
                _pokedexRepository.Search(searchViewModel.SearchString, selectedAbilityId, selectedCategoryId, selectedTypeId, selectedPokeballId)));
            
            return finalSearchViewModel;
        }

        private List<SelectListItem> GetAbilitySelectListItems()
        {
            return _pokedexRepository.GetAllAbilities().Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).Prepend(GetBlankSelectListItem()).ToList();
        }

        private static SelectListItem GetBlankSelectListItem()
        {
            return new SelectListItem() { Text = None, Value = "" };
        }

        private List<SelectListItem> GetCategorySelectListItems()
        {
            return _pokedexRepository.GetAllCategories().Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).Prepend(GetBlankSelectListItem()).ToList();
        }

        private List<SelectListItem> GetNationalDexSelectListItems()
        {
            return _pokedexRepository.GetNationalDex().Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).Prepend(GetBlankSelectListItem()).ToList();
        }

        private List<SelectListItem> GetPokeballSelectListItems()
        {
            return _pokedexRepository.GetAllPokeballs().Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).Prepend(GetBlankSelectListItem()).ToList();
        }

        private List<SelectListItem> GetPokemonSexSelectListItems()
        {
            return new List<SelectListItem>() { new SelectListItem() { Text = Constants.Female, Value = "0" },
                new SelectListItem() { Text = Constants.Male, Value = "1" } };
        }

        private List<SelectListItem> GetTypeSelectListItems()
        {
            return _pokedexRepository.GetAllTypes().Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).Prepend(GetBlankSelectListItem()).ToList();
        }

        private tblMyPokedex MapDetailViewModelToMyPokemon(PokemonDetailViewModel pokemonDetailViewModel)
        {
            _logger.LogInformation(Constants.Mapping + " " + Constants.Pokemon + " " + ViewModels);

            tlkpNationalDex nationalDexLookup = _pokedexRepository.GetNationalDexPokemonById(pokemonDetailViewModel.NationalDexPokemonId.Value);

            tblMyPokedex beforeUpdates = _pokedexRepository.GetMyPokemonById(pokemonDetailViewModel.MyPokemonId.Value);

            return new tblMyPokedex()
            {
                Date = pokemonDetailViewModel.Date,
                Id = pokemonDetailViewModel.MyPokemonId.Value,
                Level = pokemonDetailViewModel.Level,
                Location = pokemonDetailViewModel.Location,
                Nickname = pokemonDetailViewModel.Nickname,
                PokeballId = beforeUpdates.PokeballId,
                Pokeball = beforeUpdates.Pokeball,
                Pokemon = nationalDexLookup,
                PokemonId = nationalDexLookup.Id,
                Sex = pokemonDetailViewModel.Sex
            };
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

            return pokedex.Select(p => new PokemonListingViewModel
            {
                ImageURL = p.Pokemon.ImageURL,
                MyPokemonId = p.Id,
                Name = p.Pokemon.Name,
                Nickname = p.Nickname,
                NationalDexPokemonId = p.Pokemon.Id,
            }).ToList();
        }

        private PokemonDetailViewModel MapMyPokemonToDetailViewModel(tblMyPokedex myPokemon)
        {
            _logger.LogInformation(string.Format(InformationalMessageMappingWithCount, 1, Constants.Pokemon, ViewModels));
            
            return new PokemonDetailViewModel
            {
                Ability = myPokemon.Pokemon.Ability.Name,
                Category = myPokemon.Pokemon.Category.Name,
                Date = myPokemon.Date,
                Description = myPokemon.Pokemon.Description,
                HeightInInches = myPokemon.Pokemon.HeightInInches,
                HiddenAbility = myPokemon.Pokemon.HiddenAbilityId.HasValue ? myPokemon.Pokemon.HiddenAbility.Name : Constants.NotApplicable,
                ImageURL = myPokemon.Pokemon.ImageURL,
                JapaneseName = myPokemon.Pokemon.JapaneseName,
                Level = myPokemon.Level,
                Location = myPokemon.Location,
                MyPokemonId = myPokemon.Id,
                Name = myPokemon.Pokemon.Name,
                NationalDexPokemonId = myPokemon.Pokemon.Id,
                Nickname = myPokemon.Nickname,
                PokeballImageURL = myPokemon.Pokeball.ImageURL,
                Sex = myPokemon.Sex,
                TypeOne = myPokemon.Pokemon.TypeOne.Name,
                TypeTwo = myPokemon.Pokemon.TypeTwoId.HasValue ? myPokemon.Pokemon.TypeTwo.Name : Constants.NotApplicable,
                WeightInPounds = myPokemon.Pokemon.WeightInPounds
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
                Ability = pokemon.Ability.Name,
                Category = pokemon.Category.Name,
                Description = pokemon.Description,
                HeightInInches = pokemon.HeightInInches,
                HiddenAbility = pokemon.HiddenAbilityId.HasValue ? pokemon.HiddenAbility.Name : Constants.NotApplicable,
                NationalDexPokemonId = pokemon.Id,
                ImageURL = pokemon.ImageURL,
                JapaneseName = pokemon.JapaneseName,
                Name = pokemon.Name,
                TypeOne = pokemon.TypeOne.Name,
                TypeTwo = pokemon.TypeTwoId.HasValue ? pokemon.TypeTwo.Name : Constants.NotApplicable,
                WeightInPounds = pokemon.WeightInPounds
            };
        }
    }
}