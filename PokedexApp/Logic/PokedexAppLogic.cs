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
using System.Threading.Tasks;

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

        public async Task<PokemonFormViewModel> AddPokemon(PokemonFormViewModel pokemonFormViewModel)
        {
            tblMyPokedex pokemon = MapFormViewModelToMyPokemon(pokemonFormViewModel);

            await _pokedexRepository.AddPokemon(pokemon);

            return pokemonFormViewModel;
        }

        public void DeletePokemonById(Guid id)
        {
            _pokedexRepository.DeletePokemonById(id);

            _logger.LogInformation(Constants.Deleted + " " + Constants.Pokemon + ": " + id);
        }

        public async Task<PokemonDetailViewModel> EditPokemon(PokemonDetailViewModel pokemonDetailViewModel)
        {
            tblMyPokedex pokemon = MapDetailViewModelToMyPokemon(pokemonDetailViewModel);

            await _pokedexRepository.EditPokemon(pokemon);

            _logger.LogInformation(Constants.Updated + " " + Constants.Pokemon + ": " + pokemonDetailViewModel.MyPokemonId);

            return pokemonDetailViewModel;
        }

        public List<PokemonListingViewModel> GetMyPokedex()
        {
            List<tblMyPokedex> pokedex = _pokedexRepository.GetMyPokedex();

            List<PokemonListingViewModel> pokemonViewModels = MapPokedexToListingViewModels(pokedex);

            return pokemonViewModels;
        }

        public PokemonDetailViewModel GetMyPokemonById(Guid id)
        {
            tblMyPokedex myPokemon = _pokedexRepository.GetMyPokemonById(id);

            PokemonDetailViewModel pokemonDetailViewModel = MapMyPokemonToDetailViewModel(myPokemon);

            return pokemonDetailViewModel;
        }

        public List<PokemonListingViewModel> GetNationalDex()
        {
            List<tlkpNationalDex> nationalDex = _pokedexRepository.GetNationalDex();

            List<PokemonListingViewModel> pokemonListingViewModel = MapNationalDexLookupsToListingViewModels(nationalDex);

            return pokemonListingViewModel;
        }

        public PokemonDetailViewModel GetNationalDexPokemonById(int id)
        {
            tlkpNationalDex pokemon = _pokedexRepository.GetNationalDexPokemonById(id);

            PokemonDetailViewModel pokemonDetailViewModel = MapNationalDexLookupToDetailViewModel(pokemon);

            return pokemonDetailViewModel;
        }

        public PokemonFormViewModel GetNewPokemonForm()
        {
            _logger.LogInformation(Constants.Mapping + SelectListItems);

            List<SelectListItem> nationalDexOptions = GetNationalDexSelectListItems();

            SelectListItem blankOption = GetBlankSelectListItem();

            List<SelectListItem> pokeballOptions = GetPokeballSelectListItems(blankOption)
                .Where(p => !string.IsNullOrWhiteSpace(p.Value)).ToList();

            List<SelectListItem> sexOptions = GetPokemonSexSelectListItems();

            return new PokemonFormViewModel()
            {
                NationalDexOptions = nationalDexOptions,
                PokeballOptions = pokeballOptions,
                SexOptions = sexOptions
            };
        }

        public SearchViewModel GetSearchForm()
        {
            _logger.LogInformation(Constants.Mapping + SelectListItems);

            SelectListItem blankOption = GetBlankSelectListItem();

            List<SelectListItem> abilityOptions = GetAbilitySelectListItems(blankOption);

            List<SelectListItem> categoryOptions = GetCategorySelectListItems(blankOption);

            List<SelectListItem> pokeballOptions = GetPokeballSelectListItems(blankOption);

            List<SelectListItem> typeOptions = GetTypeSelectListItems(blankOption);

            return new SearchViewModel()
            {
                AbilityOptions = abilityOptions,
                CategoryOptions = categoryOptions,
                PokeballOptions = pokeballOptions,
                TypeOptions = typeOptions
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

            List<tlkpNationalDex> nationalDex = _pokedexRepository.Search(searchViewModel.SearchString, selectedAbilityId, selectedCategoryId, selectedTypeId);
            List<PokemonListingViewModel> pokemonListingViewModels = MapNationalDexLookupsToListingViewModels(nationalDex);
            finalSearchViewModel.FilteredPokemon.AddRange(pokemonListingViewModels);

            List<tblMyPokedex> pokedex = _pokedexRepository.Search(searchViewModel.SearchString, selectedAbilityId, selectedCategoryId, selectedTypeId, selectedPokeballId);
            pokemonListingViewModels = MapPokedexToListingViewModels(pokedex);
            finalSearchViewModel.FilteredPokemon.AddRange(pokemonListingViewModels);
            
            return finalSearchViewModel;
        }

        private List<SelectListItem> GetAbilitySelectListItems(SelectListItem prependOption)
        {
            List<tlkpAbility> abilities = _pokedexRepository.GetAllAbilities();

            return abilities.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).Prepend(prependOption).ToList();
        }

        private static SelectListItem GetBlankSelectListItem()
        {
            return new SelectListItem() { Text = None, Value = "" };
        }

        private List<SelectListItem> GetCategorySelectListItems(SelectListItem prependOption)
        {
            List<tlkpCategory> categories = _pokedexRepository.GetAllCategories();

            return categories.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).Prepend(prependOption).ToList();
        }

        private List<SelectListItem> GetNationalDexSelectListItems()
        {
            List<tlkpNationalDex> nationalDex = _pokedexRepository.GetNationalDex();

            return nationalDex.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList();
        }

        private List<SelectListItem> GetPokeballSelectListItems(SelectListItem prependOption)
        {
            List<tlkpPokeball> pokeballs = _pokedexRepository.GetAllPokeballs();

            return pokeballs.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).Prepend(prependOption).ToList();
        }

        private List<SelectListItem> GetPokemonSexSelectListItems()
        {
            return new List<SelectListItem>() { new SelectListItem() { Text = Constants.Female, Value = "0" },
                new SelectListItem() { Text = Constants.Male, Value = "1" } };
        }

        private List<SelectListItem> GetTypeSelectListItems(SelectListItem prependOption)
        {
            return _pokedexRepository.GetAllTypes().Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).Prepend(prependOption).ToList();
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