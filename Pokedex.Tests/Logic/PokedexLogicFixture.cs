using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Data.Models;
using Pokedex.Logging.Interfaces;
using Pokedex.Repository.Interfaces;
using PokedexAPI.Logic;
using PokedexAPI.Models.Output;
using PokedexApp.Logic;
using PokedexApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.Tests.Logic
{
    //todo finish api logic tests for NationalDex, MyPokemon, Search also the controllers for those.
    [TestClass]
    public class PokedexLogicFixture
    {
        private PokedexAppLogic _pokedexAppLogic;
        private PokedexAPILogic _pokedexAPILogic;

        private Mock<IPokedexRepository> _pokedexRepositoryMock;
        private Mock<ILoggerAdapter<PokedexAppLogic>> _loggerAppMock;
        private Mock<ILoggerAdapter<PokedexAPILogic>> _loggerAPIMock;

        [TestInitialize]
        public void Initialize()
        {
            List<tblMyPokedex> pokedex = DataGenerator.GenerateMyPokemon(1);

            List<tlkpAbility> abilities = DataGenerator.GenerateAbilities(5);
            List<tlkpCategory> categories = DataGenerator.GenerateCategories(5);
            List<tlkpNationalDex> nationalDex = DataGenerator.GenerateNationalDexPokemon(5);
            List<tlkpPokeball> pokeballs = DataGenerator.GeneratePokeballs(5);
            List<tlkpType> types = DataGenerator.GenerateTypes(5);

            _pokedexRepositoryMock = new Mock<IPokedexRepository>();
            _pokedexRepositoryMock.Setup(prm => prm.GetAbilityById(0)).ReturnsAsync(abilities[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetAbilityById(1)).ReturnsAsync(abilities[1]); //Hidden Ability
            _pokedexRepositoryMock.Setup(prm => prm.GetAllAbilities()).ReturnsAsync(abilities);
            _pokedexRepositoryMock.Setup(prm => prm.GetAllCategories()).ReturnsAsync(categories);
            _pokedexRepositoryMock.Setup(prm => prm.GetAllPokeballs()).ReturnsAsync(pokeballs);
            _pokedexRepositoryMock.Setup(prm => prm.GetAllTypes()).ReturnsAsync(types);
            _pokedexRepositoryMock.Setup(prm => prm.GetCategoryById(0)).ReturnsAsync(categories[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetMyPokedex()).ReturnsAsync(pokedex);
            _pokedexRepositoryMock.Setup(prm => prm.GetMyPokemonById(DataGenerator.DefaultGuid)).ReturnsAsync(pokedex[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetNationalDex()).ReturnsAsync(nationalDex);
            _pokedexRepositoryMock.Setup(prm => prm.GetNationalDexPokemonById(0)).ReturnsAsync(nationalDex[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetPokeballById(0)).ReturnsAsync(pokeballs[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetTypeById(0)).ReturnsAsync(types[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetTypeById(1)).ReturnsAsync(types[1]); //Type Two

            _pokedexRepositoryMock.Setup(prm => prm.Search(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(nationalDex);

            _pokedexRepositoryMock.Setup(prm => prm.Search(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(pokedex);

            _loggerAppMock = new Mock<ILoggerAdapter<PokedexAppLogic>>();
            _loggerAPIMock = new Mock<ILoggerAdapter<PokedexAPILogic>>();

            _pokedexAppLogic = new PokedexAppLogic(_pokedexRepositoryMock.Object, _loggerAppMock.Object);
            _pokedexAPILogic = new PokedexAPILogic(_pokedexRepositoryMock.Object, _loggerAPIMock.Object);
        }

        #region App Logic Tests

        [TestMethod]
        public async Task AddPokemonIsSuccessfulAndLogsInformation()
        {
            PokemonFormViewModel pokemonFormViewModel = await _pokedexAppLogic.AddPokemon(new PokemonFormViewModel() { Level = 333 });

            Assert.AreEqual(333, pokemonFormViewModel.Level);

            _pokedexRepositoryMock.Verify(prm => prm.AddPokemon(It.Is<tblMyPokedex>(p => p.Level == 333)), Times.Once);

            VerifyLoggerAppMockLogsInformation("Mapping Pokémon View Models");

            _loggerAppMock.VerifyNoOtherCalls();

            _pokedexRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task DeletePokemonByIdIsSuccessfulAndLogsInformation()
        {
            Guid deleted = await _pokedexAppLogic.DeletePokemonById(DataGenerator.DefaultGuid);

            Assert.AreEqual(DataGenerator.DefaultGuid, deleted);

            _pokedexRepositoryMock.Verify(prm => prm.DeletePokemonById(DataGenerator.DefaultGuid), Times.Once);

            VerifyLoggerAppMockLogsInformation("Deleted Pokémon: " + DataGenerator.DefaultGuid);

            _loggerAppMock.VerifyNoOtherCalls();

            _pokedexRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task EditPokemonIsSuccessfulAndLogsInformation()
        {
            await _pokedexAppLogic.EditPokemon(new PokemonDetailViewModel()
            {
                MyPokemonId = DataGenerator.DefaultGuid,
                NationalDexPokemonId = 0
            });

            _pokedexRepositoryMock.Verify(prm => prm.GetNationalDexPokemonById(0), Times.Once);

            _pokedexRepositoryMock.Verify(prm => prm.GetMyPokemonById(DataGenerator.DefaultGuid), Times.Once);

            _pokedexRepositoryMock.Verify(prm => prm.EditPokemon(It.Is<tblMyPokedex>(
                p => p.Id == DataGenerator.DefaultGuid && p.PokemonId == 0)), Times.Once);

            VerifyLoggerAppMockLogsInformation("Mapping Pokémon View Models");
            VerifyLoggerAppMockLogsInformation("Updated Pokémon: " + DataGenerator.DefaultGuid);

            _loggerAppMock.VerifyNoOtherCalls();

            _pokedexRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetMyPokedexIsSuccessfulAndLogsInformation()
        {
            List<PokemonListingViewModel> pokemonListingViewModels = await _pokedexAppLogic.GetMyPokedex();

            Assert.AreEqual(1, pokemonListingViewModels.Count);
            Assert.AreEqual("http://0.com", pokemonListingViewModels[0].ImageURL);
            Assert.AreEqual(DataGenerator.DefaultGuid, pokemonListingViewModels[0].MyPokemonId);
            Assert.AreEqual("Name0", pokemonListingViewModels[0].Name);
            Assert.AreEqual("Nickname0", pokemonListingViewModels[0].Nickname);
            Assert.AreEqual(0, pokemonListingViewModels[0].NationalDexPokemonId);
            
            _pokedexRepositoryMock.Verify(prm => prm.GetMyPokedex(), Times.Once);

            VerifyLoggerAppMockLogsInformation("Mapping 1 Pokémon View Models.");

            _loggerAppMock.VerifyNoOtherCalls();

            _pokedexRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetMyPokemonByIdIsSuccessfulAndLogsInformation()
        {
            PokemonDetailViewModel pokemonDetailViewModel = await _pokedexAppLogic.GetMyPokemonById(DataGenerator.DefaultGuid);

            Assert.AreEqual("Name0", pokemonDetailViewModel.Ability);
            Assert.AreEqual("Name0", pokemonDetailViewModel.Category);
            Assert.AreEqual(DateTime.Today, pokemonDetailViewModel.Date);
            Assert.AreEqual("Desc0", pokemonDetailViewModel.Description);
            Assert.AreEqual(1, pokemonDetailViewModel.HeightInInches);
            Assert.AreEqual("Name1", pokemonDetailViewModel.HiddenAbility);
            Assert.AreEqual("http://0.com", pokemonDetailViewModel.ImageURL);
            Assert.AreEqual("JapaneseName0", pokemonDetailViewModel.JapaneseName);
            Assert.AreEqual(1, pokemonDetailViewModel.Level);
            Assert.AreEqual("0 Main Street", pokemonDetailViewModel.Location);
            Assert.AreEqual(DataGenerator.DefaultGuid, pokemonDetailViewModel.MyPokemonId);
            Assert.AreEqual("Name0", pokemonDetailViewModel.Name);
            Assert.AreEqual(0, pokemonDetailViewModel.NationalDexPokemonId);
            Assert.AreEqual("Nickname0", pokemonDetailViewModel.Nickname);
            Assert.AreEqual("http://0.com", pokemonDetailViewModel.PokeballImageURL);
            Assert.AreEqual(true, pokemonDetailViewModel.Sex);
            Assert.AreEqual("Name0", pokemonDetailViewModel.TypeOne);
            Assert.AreEqual("Name1", pokemonDetailViewModel.TypeTwo);
            Assert.AreEqual(1, pokemonDetailViewModel.WeightInPounds);

            _pokedexRepositoryMock.Verify(prm => prm.GetMyPokemonById(DataGenerator.DefaultGuid), Times.Once);

            VerifyLoggerAppMockLogsInformation("Mapping 1 Pokémon View Models.");

            _loggerAppMock.VerifyNoOtherCalls();

            _pokedexRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetNationalDexIsSuccessfulAndLogsInformation()
        {
            List<PokemonListingViewModel> pokemonListingViewModels = await _pokedexAppLogic.GetNationalDex();

            Assert.AreEqual(5, pokemonListingViewModels.Count);
            Assert.AreEqual("http://0.com", pokemonListingViewModels[0].ImageURL);
            Assert.AreEqual("Name0", pokemonListingViewModels[0].Name);
            Assert.AreEqual(0, pokemonListingViewModels[0].NationalDexPokemonId);

            Assert.IsNull(pokemonListingViewModels[0].MyPokemonId);
            Assert.IsNull(pokemonListingViewModels[0].Nickname);

            _pokedexRepositoryMock.Verify(prm => prm.GetNationalDex(), Times.Once);

            VerifyLoggerAppMockLogsInformation("Mapping 5 Pokémon View Models.");

            _loggerAppMock.VerifyNoOtherCalls();

            _pokedexRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetNationalDexPokemonByIdIsSuccessfulAndLogsInformation()
        {
            PokemonDetailViewModel pokemonDetailViewModel = await _pokedexAppLogic.GetNationalDexPokemonById(0);

            Assert.AreEqual("Name0", pokemonDetailViewModel.Ability);
            Assert.AreEqual("Name0", pokemonDetailViewModel.Category);
            Assert.AreEqual("Desc0", pokemonDetailViewModel.Description);
            Assert.AreEqual(1, pokemonDetailViewModel.HeightInInches);
            Assert.AreEqual("Name1", pokemonDetailViewModel.HiddenAbility);
            Assert.AreEqual("http://0.com", pokemonDetailViewModel.ImageURL);
            Assert.AreEqual("JapaneseName0", pokemonDetailViewModel.JapaneseName);
            Assert.AreEqual("Name0", pokemonDetailViewModel.Name);
            Assert.AreEqual(0, pokemonDetailViewModel.NationalDexPokemonId);
            Assert.AreEqual("Name0", pokemonDetailViewModel.TypeOne);
            Assert.AreEqual("Name1", pokemonDetailViewModel.TypeTwo);
            Assert.AreEqual(1, pokemonDetailViewModel.WeightInPounds);

            Assert.IsNull(pokemonDetailViewModel.Date);
            Assert.IsNull(pokemonDetailViewModel.Level);
            Assert.IsNull(pokemonDetailViewModel.Location);
            Assert.IsNull(pokemonDetailViewModel.MyPokemonId);
            Assert.IsNull(pokemonDetailViewModel.Nickname);
            Assert.IsNull(pokemonDetailViewModel.PokeballImageURL);
            Assert.IsNull(pokemonDetailViewModel.Sex);

            _pokedexRepositoryMock.Verify(prm => prm.GetNationalDexPokemonById(0), Times.Once);

            VerifyLoggerAppMockLogsInformation("Mapping 1 Pokémon View Models.");

            _loggerAppMock.VerifyNoOtherCalls();

            _pokedexRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetNewPokemonFormIsSuccessfulAndLogsInformation()
        {
            PokemonFormViewModel pokemonFormViewModel = await _pokedexAppLogic.GetNewPokemonForm();

            List<SelectListItem> nationalDexOptions = pokemonFormViewModel.NationalDexOptions.ToList();
            List<SelectListItem> pokeballOptions = pokemonFormViewModel.PokeballOptions.ToList();
            List<SelectListItem> sexOptions = pokemonFormViewModel.SexOptions.ToList();

            Assert.AreEqual(5, nationalDexOptions.Count);
            Assert.AreEqual("Name0", nationalDexOptions[0].Text);
            Assert.AreEqual("0", nationalDexOptions[0].Value);
            Assert.AreEqual(5, pokeballOptions.Count);
            Assert.AreEqual("Name0", pokeballOptions[0].Text);
            Assert.AreEqual("0", pokeballOptions[0].Value);
            Assert.AreEqual(2, sexOptions.Count);
            Assert.AreEqual("Female", sexOptions[0].Text);
            Assert.AreEqual("0", sexOptions[0].Value);

            _pokedexRepositoryMock.Verify(prm => prm.GetAllPokeballs(), Times.Once);

            _pokedexRepositoryMock.Verify(prm => prm.GetNationalDex(), Times.Once);

            VerifyLoggerAppMockLogsInformation("Mapping Select List Items.");

            _loggerAppMock.VerifyNoOtherCalls();

            _pokedexRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetSearchFormIsSuccessfulAndLogsInformation()
        {
            SearchViewModel searchViewModel = await _pokedexAppLogic.GetSearchForm();

            List<SelectListItem> abilityOptions = searchViewModel.AbilityOptions.ToList();
            List<SelectListItem> categoryOptions = searchViewModel.CategoryOptions.ToList();
            List<SelectListItem> pokeballOptions = searchViewModel.PokeballOptions.ToList();
            List<SelectListItem> typeOptions = searchViewModel.TypeOptions.ToList();

            Assert.AreEqual(6, pokeballOptions.Count);
            Assert.AreEqual("Name0", abilityOptions[1].Text);
            Assert.AreEqual("0", abilityOptions[1].Value);
            Assert.AreEqual("Name0", categoryOptions[1].Text);
            Assert.AreEqual("0", categoryOptions[1].Value);
            Assert.AreEqual("Name0", pokeballOptions[1].Text);
            Assert.AreEqual("0", pokeballOptions[1].Value);
            Assert.AreEqual("Name0", typeOptions[1].Text);
            Assert.AreEqual("0", typeOptions[1].Value);

            VerifyRepositoryMockGetsLookups();

            VerifyLoggerAppMockLogsInformation("Mapping Select List Items.");

            _loggerAppMock.VerifyNoOtherCalls();

            _pokedexRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task SearchWithPokeballCallsRepositoryAndLogsInformation()
        {
            SearchViewModel searchResultsViewModel = await _pokedexAppLogic.Search(new SearchViewModel
            {
                SearchString = "Name0",
                SelectedAbilityId = 0,
                SelectedCategoryId = 0,
                SelectedPokeballId = 0,
                SelectedTypeId = 0
            });

            VerifyRepositoryMockGetsLookups();

            _pokedexRepositoryMock.Verify(prm => prm.Search("Name0", 0, 0, 0), Times.Never); // national dex search not called, pokeball is set

            _pokedexRepositoryMock.Verify(prm => prm.Search("Name0", 0, 0, 0, 0), Times.Once);

            VerifyLoggerAppMockLogsInformation("Mapping Select List Items.");
            VerifyLoggerAppMockLogsInformation("Mapping 1 Pokémon View Models.");

            _loggerAppMock.VerifyNoOtherCalls();

            _pokedexRepositoryMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task SearchWithoutPokeballCallsRepositoryAndLogsInformation()
        {
            SearchViewModel searchResultsViewModel = await _pokedexAppLogic.Search(new SearchViewModel
            {
                SearchString = "Name0",
                SelectedAbilityId = 0,
                SelectedCategoryId = 0,
                SelectedTypeId = 0
            });

            VerifyRepositoryMockGetsLookups();

            _pokedexRepositoryMock.Verify(prm => prm.Search("Name0", 0, 0, 0), Times.Once);

            _pokedexRepositoryMock.Verify(prm => prm.Search("Name0", 0, 0, 0, null), Times.Once);

            VerifyLoggerAppMockLogsInformation("Mapping Select List Items.");
            VerifyLoggerAppMockLogsInformation("Mapping 1 Pokémon View Models.");
            VerifyLoggerAppMockLogsInformation("Mapping 5 Pokémon View Models.");

            _loggerAppMock.VerifyNoOtherCalls();

            _pokedexRepositoryMock.VerifyNoOtherCalls();
        }

        #endregion

        #region API Logic Tests

        [TestMethod]
        public async Task GetAbilityByIdIsSuccessfulAndLogsInformation()
        {
            GenericLookupResult ability = await _pokedexAPILogic.GetAbilityById(0);

            Assert.AreEqual(0, ability.Id);
            Assert.AreEqual("Name0", ability.Name);

            _pokedexRepositoryMock.Verify(prm => prm.GetAbilityById(0), Times.Once);

            _loggerAPIMock.Verify(lm => lm.LogInformation("Mapping Ability Results."), Times.Once);
        }

        [TestMethod]
        public async Task GetAllAbilitiesIsSuccessfulAndLogsInformation()
        {
            List<GenericLookupResult> abilities = await _pokedexAPILogic.GetAllAbilities();

            Assert.AreEqual(5, abilities.Count);
            Assert.AreEqual(0, abilities[0].Id);
            Assert.AreEqual("Name0", abilities[0].Name);

            _pokedexRepositoryMock.Verify(prm => prm.GetAllAbilities(), Times.Once);

            _loggerAPIMock.Verify(lm => lm.LogInformation("Mapping 5 Ability Results."), Times.Once);
        }

        [TestMethod]
        public async Task GetCategoryByIdIsSuccessfulAndLogsInformation()
        {
            GenericLookupResult category = await _pokedexAPILogic.GetCategoryById(0);

            Assert.AreEqual(0, category.Id);
            Assert.AreEqual("Name0", category.Name);

            _pokedexRepositoryMock.Verify(prm => prm.GetCategoryById(0), Times.Once);

            _loggerAPIMock.Verify(lm => lm.LogInformation("Mapping Category Results."), Times.Once);
        }

        [TestMethod]
        public async Task GetAllCategoriesIsSuccessfulAndLogsInformation()
        {
            List<GenericLookupResult> categories = await _pokedexAPILogic.GetAllCategories();

            Assert.AreEqual(5, categories.Count);
            Assert.AreEqual(0, categories[0].Id);
            Assert.AreEqual("Name0", categories[0].Name);

            _pokedexRepositoryMock.Verify(prm => prm.GetAllCategories(), Times.Once);

            _loggerAPIMock.Verify(lm => lm.LogInformation("Mapping 5 Category Results."), Times.Once);
        }

        [TestMethod]
        public async Task GetPokeballByIdIsSuccessfulAndLogsInformation()
        {
            GenericLookupResult pokeball = await _pokedexAPILogic.GetPokeballById(0);

            Assert.AreEqual(0, pokeball.Id);
            Assert.AreEqual("Name0", pokeball.Name);

            _pokedexRepositoryMock.Verify(prm => prm.GetPokeballById(0), Times.Once);

            _loggerAPIMock.Verify(lm => lm.LogInformation("Mapping Pokéball Results."), Times.Once);
        }

        [TestMethod]
        public async Task GetAllPokeballsIsSuccessfulAndLogsInformation()
        {
            List<GenericLookupResult> pokeballs = await _pokedexAPILogic.GetAllPokeballs();

            Assert.AreEqual(5, pokeballs.Count);
            Assert.AreEqual(0, pokeballs[0].Id);
            Assert.AreEqual("Name0", pokeballs[0].Name);

            _pokedexRepositoryMock.Verify(prm => prm.GetAllPokeballs(), Times.Once);

            _loggerAPIMock.Verify(lm => lm.LogInformation("Mapping 5 Pokéball Results."), Times.Once);
        }

        [TestMethod]
        public async Task GetTypeByIdIsSuccessfulAndLogsInformation()
        {
            GenericLookupResult type = await _pokedexAPILogic.GetTypeById(0);

            Assert.AreEqual(0, type.Id);
            Assert.AreEqual("Name0", type.Name);

            _pokedexRepositoryMock.Verify(prm => prm.GetTypeById(0), Times.Once);

            _loggerAPIMock.Verify(lm => lm.LogInformation("Mapping Type Results."), Times.Once);
        }

        [TestMethod]
        public async Task GetAllTypesIsSuccessfulAndLogsInformation()
        {
            List<GenericLookupResult> types = await _pokedexAPILogic.GetAllTypes();

            Assert.AreEqual(5, types.Count);
            Assert.AreEqual(0, types[0].Id);
            Assert.AreEqual("Name0", types[0].Name);

            _pokedexRepositoryMock.Verify(prm => prm.GetAllTypes(), Times.Once);

            _loggerAPIMock.Verify(lm => lm.LogInformation("Mapping 5 Types Results."), Times.Once);
        }

        #endregion

        private void VerifyLoggerAppMockLogsInformation(string info)
        {
            _loggerAppMock.Verify(lm => lm.LogInformation(info), Times.Once);
        }

        private void VerifyRepositoryMockGetsLookups()
        {
            _pokedexRepositoryMock.Verify(prm => prm.GetAllAbilities(), Times.Once);
            _pokedexRepositoryMock.Verify(prm => prm.GetAllCategories(), Times.Once);
            _pokedexRepositoryMock.Verify(prm => prm.GetAllPokeballs(), Times.Once);
            _pokedexRepositoryMock.Verify(prm => prm.GetAllTypes(), Times.Once);
        }
    }
}