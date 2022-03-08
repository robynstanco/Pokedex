using AutoMapper;
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
        IMapper _mapper;
        IPokedexRepository _pokedexRepository;
        public PokedexAppLogic(ILoggerAdapter<PokedexAppLogic> logger, IMapper mapper, IPokedexRepository pokedexRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _pokedexRepository = pokedexRepository ?? throw new ArgumentNullException(nameof(pokedexRepository));
        }

        public async Task<PokemonFormViewModel> AddPokemon(PokemonFormViewModel pokemonFormViewModel)
        {
            _logger.LogInformation($"{Constants.Mapping} {Constants.Pokemon} {ViewModels}");

            tblMyPokedex pokemon = _mapper.Map<tblMyPokedex>(pokemonFormViewModel);
            pokemon.Id = Guid.NewGuid();

            await _pokedexRepository.AddPokemon(pokemon);

            return pokemonFormViewModel;
        }

        public async Task<Guid> DeletePokemonById(Guid id)
        {
            await _pokedexRepository.DeletePokemonById(id);

            _logger.LogInformation($"{Constants.Deleted} {Constants.Pokemon}: {id}");

            return id;
        }

        public async Task<PokemonDetailViewModel> EditPokemon(PokemonDetailViewModel pokemonDetailViewModel)
        {
            tblMyPokedex pokemon = await MapDetailViewModelToMyPokemon(pokemonDetailViewModel);

            await _pokedexRepository.EditPokemon(pokemon);

            _logger.LogInformation($"{Constants.Updated} {Constants.Pokemon}: {pokemonDetailViewModel.MyPokemonId}");

            return pokemonDetailViewModel;
        }

        public async Task<List<PokemonListingViewModel>> GetMyPokedex()
        {
            List<tblMyPokedex> pokedex = await _pokedexRepository.GetMyPokedex();

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, pokedex.Count, Constants.Pokemon, ViewModels));

            List<PokemonListingViewModel> pokemonViewModels = _mapper.Map<List<PokemonListingViewModel>>(pokedex);

            return pokemonViewModels;
        }

        public async Task<PokemonDetailViewModel> GetMyPokemonById(Guid id)
        {
            tblMyPokedex myPokemon = await _pokedexRepository.GetMyPokemonById(id);

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, 1, Constants.Pokemon, ViewModels));

            PokemonDetailViewModel pokemonDetailViewModel = _mapper.Map<PokemonDetailViewModel>(myPokemon);

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

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, 1, Constants.Pokemon, ViewModels));

            PokemonDetailViewModel pokemonDetailViewModel = _mapper.Map<PokemonDetailViewModel>(pokemon);

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

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, pokedex.Count, Constants.Pokemon, ViewModels));

            List<PokemonListingViewModel> pokemonListingViewModels = _mapper.Map<List<PokemonListingViewModel>>(pokedex);

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

            List<SelectListItem> selectListItems = _mapper.Map<List<SelectListItem>>(abilities);

            return selectListItems.Prepend(prependOption).ToList();
        }

        private static SelectListItem GetBlankSelectListItem()
        {
            return new SelectListItem() { Text = None, Value = string.Empty };
        }

        private async Task<List<SelectListItem>> GetCategorySelectListItems(SelectListItem prependOption)
        {
            List<tlkpCategory> categories =  await _pokedexRepository.GetAllCategories();

            List<SelectListItem> selectListItems = _mapper.Map<List<SelectListItem>>(categories);

            return selectListItems.Prepend(prependOption).ToList();
        }

        private async Task<List<SelectListItem>> GetNationalDexSelectListItems()
        {
            List<tlkpNationalDex> nationalDex = await _pokedexRepository.GetNationalDex();

            List<SelectListItem> selectListItems = _mapper.Map<List<SelectListItem>>(nationalDex);

            return selectListItems;
        }

        private async Task<List<SelectListItem>> GetPokeballSelectListItems(SelectListItem prependOption)
        {
            List<tlkpPokeball> pokeballs = await _pokedexRepository.GetAllPokeballs();

            List<SelectListItem> selectListItems = _mapper.Map<List<SelectListItem>>(pokeballs);

            return selectListItems.Prepend(prependOption).ToList();
        }

        private List<SelectListItem> GetPokemonSexSelectListItems()
        {
            return new List<SelectListItem>() 
            { 
                new SelectListItem() { Text = "Female", Value = "0" }, 
                new SelectListItem() { Text = "Male", Value = "1" } 
            };
        }

        private async Task<List<SelectListItem>> GetTypeSelectListItems(SelectListItem prependOption)
        {
            List<tlkpType> types = await _pokedexRepository.GetAllTypes();

            List<SelectListItem> selectListItems = _mapper.Map<List<SelectListItem>>(types);

            return selectListItems.Prepend(prependOption).ToList();
        }

        private async Task<tblMyPokedex> MapDetailViewModelToMyPokemon(PokemonDetailViewModel pokemonDetailViewModel)
        {
            _logger.LogInformation($"{Constants.Mapping} {Constants.Pokemon} {ViewModels}");

            tlkpNationalDex nationalDexLookup = await _pokedexRepository.GetNationalDexPokemonById(pokemonDetailViewModel.NationalDexPokemonId.Value);
            tblMyPokedex beforeUpdates = await _pokedexRepository.GetMyPokemonById(pokemonDetailViewModel.MyPokemonId.Value);

            tblMyPokedex updatedPokemon = _mapper.Map<tblMyPokedex>(pokemonDetailViewModel);
            updatedPokemon.PokeballId = beforeUpdates.PokeballId;
            updatedPokemon.Pokeball = beforeUpdates.Pokeball;
            updatedPokemon.Pokemon = nationalDexLookup;
            updatedPokemon.PokemonId = nationalDexLookup.Id;

            return updatedPokemon;
        }

        /// <summary>
        /// Map the National Dex entities to listing view models.
        /// </summary>
        /// <param name="nationalDex">entities to map</param>
        /// <returns></returns>
        private List<PokemonListingViewModel> MapNationalDexLookupsToListingViewModels(List<tlkpNationalDex> nationalDex)
        {
            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, nationalDex.Count, Constants.Pokemon, ViewModels));

            List<PokemonListingViewModel> mappedListing = _mapper.Map<List<PokemonListingViewModel>>(nationalDex);

            return mappedListing;
        }
    }
}