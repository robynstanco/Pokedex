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
        private const string ViewModels = "View Models";
        private const string SelectListItems = " Select List Items.";
        private const string None = "--None--";

        ILoggerAdapter<PokedexAppLogic> _logger;
        IPokedexRepository _pokedexRepository;
        public PokedexAppLogic(ILoggerAdapter<PokedexAppLogic> logger, IPokedexRepository pokedexRepository)
        {
            _logger = logger;
            _pokedexRepository = pokedexRepository;
        }

        public async Task<PokemonFormViewModel> AddPokemon(PokemonFormViewModel pokemonFormViewModel)
        {
            tblMyPokedex pokemon = MapFormViewModelToMyPokemon(pokemonFormViewModel);

            await _pokedexRepository.AddPokemon(pokemon);

            return pokemonFormViewModel;
        }

        public async Task<Guid> DeletePokemonById(Guid id)
        {
            await _pokedexRepository.DeletePokemonById(id);

            _logger.LogInformation(Constants.Deleted + " " + Constants.Pokemon + ": " + id);

            return id;
        }

        public async Task<PokemonDetailViewModel> EditPokemon(PokemonDetailViewModel pokemonDetailViewModel)
        {
            tblMyPokedex pokemon = await MapDetailViewModelToMyPokemon(pokemonDetailViewModel);

            await _pokedexRepository.EditPokemon(pokemon);

            _logger.LogInformation(Constants.Updated + " " + Constants.Pokemon + ": " + pokemonDetailViewModel.MyPokemonId);

            return pokemonDetailViewModel;
        }

        public async Task<List<PokemonListingViewModel>> GetMyPokedex()
        {
            List<tblMyPokedex> pokedex = await _pokedexRepository.GetMyPokedex();

            List<PokemonListingViewModel> pokemonViewModels = MapPokedexToListingViewModels(pokedex);

            return pokemonViewModels;
        }

        public async Task<PokemonDetailViewModel> GetMyPokemonById(Guid id)
        {
            tblMyPokedex myPokemon = await _pokedexRepository.GetMyPokemonById(id);

            PokemonDetailViewModel pokemonDetailViewModel = MapMyPokemonToDetailViewModel(myPokemon);

            return pokemonDetailViewModel;
        }

        public async Task<List<PokemonListingViewModel>> GetNationalDex()
        {
            List<tlkpNationalDex> nationalDex = await _pokedexRepository.GetNationalDex();

            List<PokemonListingViewModel> pokemonListingViewModel = MapNationalDexLookupsToListingViewModels(nationalDex);

            return pokemonListingViewModel;
        }

        public async Task<PokemonDetailViewModel> GetNationalDexPokemonById(int id)
        {
            tlkpNationalDex pokemon = await _pokedexRepository.GetNationalDexPokemonById(id);

            PokemonDetailViewModel pokemonDetailViewModel = MapNationalDexLookupToDetailViewModel(pokemon);

            return pokemonDetailViewModel;
        }

        public async Task<PokemonFormViewModel> GetNewPokemonForm()
        {
            _logger.LogInformation(Constants.Mapping + SelectListItems);

            List<SelectListItem> nationalDexOptions = await GetNationalDexSelectListItems();

            SelectListItem blankOption = GetBlankSelectListItem();
            List<SelectListItem> pokeballOptions = await GetPokeballSelectListItems(blankOption);
            pokeballOptions = pokeballOptions.Where(p => !string.IsNullOrWhiteSpace(p.Value)).ToList();

            List<SelectListItem> sexOptions = GetPokemonSexSelectListItems();

            return new PokemonFormViewModel()
            {
                NationalDexOptions = nationalDexOptions,
                PokeballOptions = pokeballOptions,
                SexOptions = sexOptions
            };
        }

        public async Task<SearchViewModel> GetSearchForm()
        {
            _logger.LogInformation(Constants.Mapping + SelectListItems);

            SelectListItem blankOption = GetBlankSelectListItem();

            //Grab lookup option for form
            List<SelectListItem> abilityOptions = await GetAbilitySelectListItems(blankOption);
            List<SelectListItem> categoryOptions = await GetCategorySelectListItems(blankOption);
            List<SelectListItem> pokeballOptions = await GetPokeballSelectListItems(blankOption);
            List<SelectListItem> typeOptions = await GetTypeSelectListItems(blankOption);

            return new SearchViewModel()
            {
                AbilityOptions = abilityOptions,
                CategoryOptions = categoryOptions,
                PokeballOptions = pokeballOptions,
                TypeOptions = typeOptions
            };
        }

        /// <summary>
        /// Search the personal & national Pokedex given search parameters.
        /// Only search national dex if Pokeball is not selected.
        /// </summary>
        /// <param name="searchViewModel">search parameters to filter on</param>
        /// <returns>filtered search results</returns>
        public async Task<SearchViewModel> Search(SearchViewModel searchViewModel)
        {
            SearchViewModel finalSearchViewModel = await GetSearchForm();

            int? selectedAbilityId = searchViewModel.SelectedAbilityId;
            int? selectedCategoryId = searchViewModel.SelectedCategoryId;
            int? selectedPokeballId = searchViewModel.SelectedPokeballId;
            int? selectedTypeId = searchViewModel.SelectedTypeId;

            finalSearchViewModel.SelectedAbilityId = selectedAbilityId;
            finalSearchViewModel.SelectedCategoryId = selectedCategoryId;
            finalSearchViewModel.SelectedPokeballId = selectedPokeballId;
            finalSearchViewModel.SelectedTypeId = selectedTypeId;

            finalSearchViewModel.FilteredPokemon = new List<PokemonListingViewModel>();

            List<tblMyPokedex> pokedex = await _pokedexRepository.Search(searchViewModel.SearchString, selectedAbilityId, selectedCategoryId, selectedTypeId, selectedPokeballId);

            List<PokemonListingViewModel> pokemonListingViewModels = MapPokedexToListingViewModels(pokedex);

            finalSearchViewModel.FilteredPokemon.AddRange(pokemonListingViewModels);

            if (!selectedPokeballId.HasValue)
            {
                List<tlkpNationalDex> nationalDex = await _pokedexRepository.Search(searchViewModel.SearchString, selectedAbilityId, selectedCategoryId, selectedTypeId);

                pokemonListingViewModels = MapNationalDexLookupsToListingViewModels(nationalDex);

                finalSearchViewModel.FilteredPokemon.AddRange(pokemonListingViewModels);
            }

            return finalSearchViewModel;
        }

        private async Task<List<SelectListItem>> GetAbilitySelectListItems(SelectListItem prependOption)
        {
            List<tlkpAbility> abilities = await _pokedexRepository.GetAllAbilities();

            return abilities.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).Prepend(prependOption).ToList();
        }

        private static SelectListItem GetBlankSelectListItem()
        {
            return new SelectListItem() { Text = None, Value = string.Empty };
        }

        private async Task<List<SelectListItem>> GetCategorySelectListItems(SelectListItem prependOption)
        {
            List<tlkpCategory> categories =  await _pokedexRepository.GetAllCategories();

            return categories.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).Prepend(prependOption).ToList();
        }

        private async Task<List<SelectListItem>> GetNationalDexSelectListItems()
        {
            List<tlkpNationalDex> nationalDex = await _pokedexRepository.GetNationalDex();

            return nationalDex.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList();
        }

        private async Task<List<SelectListItem>> GetPokeballSelectListItems(SelectListItem prependOption)
        {
            List<tlkpPokeball> pokeballs = await _pokedexRepository.GetAllPokeballs();

            return pokeballs.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).Prepend(prependOption).ToList();
        }

        private List<SelectListItem> GetPokemonSexSelectListItems()
        {
            return new List<SelectListItem>() { new SelectListItem() { Text = "Female", Value = "0" }, new SelectListItem() { Text = "Male", Value = "1" } };
        }

        private async Task<List<SelectListItem>> GetTypeSelectListItems(SelectListItem prependOption)
        {
            List<tlkpType> types = await _pokedexRepository.GetAllTypes();

            return types.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).Prepend(prependOption).ToList();
        }

        private async Task<tblMyPokedex> MapDetailViewModelToMyPokemon(PokemonDetailViewModel pokemonDetailViewModel)
        {
            _logger.LogInformation(Constants.Mapping + " " + Constants.Pokemon + " " + ViewModels);

            tlkpNationalDex nationalDexLookup = await _pokedexRepository.GetNationalDexPokemonById(pokemonDetailViewModel.NationalDexPokemonId.Value);

            tblMyPokedex beforeUpdates = await _pokedexRepository.GetMyPokemonById(pokemonDetailViewModel.MyPokemonId.Value);

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
            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, pokedex.Count, Constants.Pokemon, ViewModels));

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
            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, 1, Constants.Pokemon, ViewModels));
            
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
            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, nationalDex.Count, Constants.Pokemon, ViewModels));

            return nationalDex.Select(p => new PokemonListingViewModel
            {
                ImageURL = p.ImageURL,
                Name = p.Name,
                NationalDexPokemonId = p.Id
            }).ToList();
        }

        private PokemonDetailViewModel MapNationalDexLookupToDetailViewModel(tlkpNationalDex pokemon)
        {
            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, 1, Constants.Pokemon, ViewModels));

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