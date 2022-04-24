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

        /// <summary>
        /// Add a Pokémon to the Pokédex.
        /// </summary>
        /// <param name="pokemonFormViewModel">The Pokémon to add.</param>
        /// <returns>The added Pokémon.</returns>
        public async Task<PokemonFormViewModel> AddPokemon(PokemonFormViewModel pokemonFormViewModel)
        {
            _logger.LogInformation($"{Constants.Mapping} {Constants.Pokemon} {ViewModels}");

            tblMyPokedex pokemon = _mapper.Map<tblMyPokedex>(pokemonFormViewModel);
            pokemon.Id = Guid.NewGuid();

            await _pokedexRepository.AddPokemon(pokemon);

            return pokemonFormViewModel;
        }

        /// <summary>
        /// Delete a Pokémon from the Pokédex by Id.
        /// </summary>
        /// <param name="id">The Pokémon Id.</param>
        /// <returns>The deleted Id.</returns>
        public async Task<Guid> DeletePokemonById(Guid id)
        {
            await _pokedexRepository.DeletePokemonById(id);

            _logger.LogInformation($"{Constants.Deleted} {Constants.Pokemon}: {id}");

            return id;
        }

        /// <summary>
        /// Edit a given Pokémon.
        /// </summary>
        /// <param name="pokemonDetailViewModel">The Pokémon to edit.</param>
        /// <returns>The updated Pokémon.</returns>
        public async Task<PokemonDetailViewModel> EditPokemon(PokemonDetailViewModel pokemonDetailViewModel)
        {
            tblMyPokedex pokemon = await MapDetailViewModelToMyPokemon(pokemonDetailViewModel);

            await _pokedexRepository.EditPokemon(pokemon);

            _logger.LogInformation($"{Constants.Updated} {Constants.Pokemon}: {pokemonDetailViewModel.MyPokemonId}");

            return pokemonDetailViewModel;
        }

        /// <summary>
        /// Get the Pokédex.
        /// </summary>
        /// <returns>The Pokédex.</returns>
        public async Task<List<PokemonListingViewModel>> GetMyPokedex()
        {
            List<tblMyPokedex> pokedex = await _pokedexRepository.GetMyPokedex();

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, pokedex.Count, Constants.Pokemon, ViewModels));

            List<PokemonListingViewModel> pokemonViewModels = _mapper.Map<List<PokemonListingViewModel>>(pokedex);

            return pokemonViewModels;
        }

        /// <summary>
        /// Get a Pokédex Pokémon by Id.
        /// </summary>
        /// <param name="id">The Pokémon Id.</param>
        /// <returns></returns>
        public async Task<PokemonDetailViewModel> GetMyPokemonById(Guid id)
        {
            tblMyPokedex myPokemon = await _pokedexRepository.GetMyPokemonById(id);

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, 1, Constants.Pokemon, ViewModels));

            PokemonDetailViewModel pokemonDetailViewModel = _mapper.Map<PokemonDetailViewModel>(myPokemon);

            return pokemonDetailViewModel;
        }

        /// <summary>
        /// Get the National Dex.
        /// </summary>
        /// <returns>The National Dex.</returns>
        public async Task<List<PokemonListingViewModel>> GetNationalDex()
        {
            List<tlkpNationalDex> nationalDex = await _pokedexRepository.GetNationalDex();

            List<PokemonListingViewModel> pokemonListingViewModel = MapNationalDexLookupsToListingViewModels(nationalDex);

            return pokemonListingViewModel;
        }

        /// <summary>
        /// Get the National Dex Pokémon by Id.
        /// </summary>
        /// <param name="id">The National Dex Id.</param>
        /// <returns>The Pokémon detail.</returns>
        public async Task<PokemonDetailViewModel> GetNationalDexPokemonById(int id)
        {
            tlkpNationalDex pokemon = await _pokedexRepository.GetNationalDexPokemonById(id);

            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, 1, Constants.Pokemon, ViewModels));

            PokemonDetailViewModel pokemonDetailViewModel = _mapper.Map<PokemonDetailViewModel>(pokemon);

            return pokemonDetailViewModel;
        }

        /// <summary>
        /// Get the new Pokémon form with dropdown select list items.
        /// </summary>
        /// <returns>The new form.</returns>
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

        /// <summary>
        /// Get the search form with dropdown select list items.
        /// </summary>
        /// <returns>The search form.</returns>
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
        /// Search the personal & National Pokédex given search parameters.
        /// Only search National dex if Pokéball is not selected.
        /// </summary>
        /// <param name="searchViewModel">The search parameters to filter on.</param>
        /// <returns>The filtered search results.</returns>
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

        /// <summary>
        /// Get the ability select list items.
        /// </summary>
        /// <param name="prependOption">The prepend option.</param>
        /// <returns>The select list items.</returns>
        private async Task<List<SelectListItem>> GetAbilitySelectListItems(SelectListItem prependOption)
        {
            List<tlkpAbility> abilities = await _pokedexRepository.GetAllAbilities();

            List<SelectListItem> selectListItems = _mapper.Map<List<SelectListItem>>(abilities);

            return selectListItems.Prepend(prependOption).ToList();
        }

        /// <summary>
        /// Get the initial blank select list item.
        /// </summary>
        /// <returns>The blank select list item.</returns>
        private static SelectListItem GetBlankSelectListItem()
        {
            return new SelectListItem() { Text = None, Value = string.Empty };
        }

        /// <summary>
        /// Get the category select list items.
        /// </summary>
        /// <param name="prependOption">The prepend option.</param>
        /// <returns>The select list items.</returns>
        private async Task<List<SelectListItem>> GetCategorySelectListItems(SelectListItem prependOption)
        {
            List<tlkpCategory> categories =  await _pokedexRepository.GetAllCategories();

            List<SelectListItem> selectListItems = _mapper.Map<List<SelectListItem>>(categories);

            return selectListItems.Prepend(prependOption).ToList();
        }

        /// <summary>
        /// Get the National Dex select list options.
        /// </summary>
        /// <returns>The select list items.</returns>
        private async Task<List<SelectListItem>> GetNationalDexSelectListItems()
        {
            List<tlkpNationalDex> nationalDex = await _pokedexRepository.GetNationalDex();

            List<SelectListItem> selectListItems = _mapper.Map<List<SelectListItem>>(nationalDex);

            return selectListItems;
        }

        /// <summary>
        /// Get the Pokéball select list options.
        /// </summary>
        /// <param name="prependOption">The prepend option.</param>
        /// <returns>The select list items.</returns>
        private async Task<List<SelectListItem>> GetPokeballSelectListItems(SelectListItem prependOption)
        {
            List<tlkpPokeball> pokeballs = await _pokedexRepository.GetAllPokeballs();

            List<SelectListItem> selectListItems = _mapper.Map<List<SelectListItem>>(pokeballs);

            return selectListItems.Prepend(prependOption).ToList();
        }

        /// <summary>
        /// Get the male and female select list options.
        /// </summary>
        /// <returns>The list of options.</returns>
        private List<SelectListItem> GetPokemonSexSelectListItems()
        {
            return new List<SelectListItem>() 
            { 
                new SelectListItem() { Text = "Female", Value = "0" }, 
                new SelectListItem() { Text = "Male", Value = "1" } 
            };
        }

        /// <summary>
        /// Get the list of select items with a given prepended item.
        /// </summary>
        /// <param name="prependOption">The select item (option) to prepend.</param>
        /// <returns>The list of select items.</returns>
        private async Task<List<SelectListItem>> GetTypeSelectListItems(SelectListItem prependOption)
        {
            List<tlkpType> types = await _pokedexRepository.GetAllTypes();

            List<SelectListItem> selectListItems = _mapper.Map<List<SelectListItem>>(types);

            return selectListItems.Prepend(prependOption).ToList();
        }

        /// <summary>
        /// Map the detail view model to Pokédex entities.
        /// </summary>
        /// <param name="pokemonDetailViewModel">The view model to map.</param>
        /// <returns>The mapped entity.</returns>
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
        /// <param name="nationalDex">The entities to map.</param>
        /// <returns>The mapped listings.</returns>
        private List<PokemonListingViewModel> MapNationalDexLookupsToListingViewModels(List<tlkpNationalDex> nationalDex)
        {
            _logger.LogInformation(string.Format(Constants.InformationalMessageMappingWithCount, nationalDex.Count, Constants.Pokemon, ViewModels));

            List<PokemonListingViewModel> mappedListing = _mapper.Map<List<PokemonListingViewModel>>(nationalDex);

            return mappedListing;
        }
    }
}