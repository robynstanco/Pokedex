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
    //todo finish api logic tests
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
            _pokedexRepositoryMock.Setup(prm => prm.Search(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(nationalDex);
            _pokedexRepositoryMock.Setup(prm => prm.Search(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pokedex);

            _loggerAppMock = new Mock<ILoggerAdapter<PokedexAppLogic>>();
            _loggerAPIMock = new Mock<ILoggerAdapter<PokedexAPILogic>>();

            _pokedexAppLogic = new PokedexAppLogic(_pokedexRepositoryMock.Object, _loggerAppMock.Object);
            _pokedexAPILogic = new PokedexAPILogic(_pokedexRepositoryMock.Object, _loggerAPIMock.Object);
        }

        [TestMethod]
        public async Task AddPokemonIsSuccessfulAndLogsInformation()
        {
            await _pokedexAppLogic.AddPokemon(new PokemonFormViewModel());

            _pokedexRepositoryMock.Verify(prm => prm.AddPokemon(It.IsAny<tblMyPokedex>()), Times.Once);

            _loggerAppMock.Verify(lm => lm.LogInformation("Mapping Pokémon View Models"), Times.Once);
        }

        [TestMethod]
        public async Task DeletePokemonByIdIsSuccessfulAndLogsInformation()
        {
            await _pokedexAppLogic.DeletePokemonById(DataGenerator.DefaultGuid);

            _pokedexRepositoryMock.Verify(prm => prm.DeletePokemonById(DataGenerator.DefaultGuid), Times.Once);
            _loggerAppMock.Verify(lm => lm.LogInformation("Deleted Pokémon: " + DataGenerator.DefaultGuid), Times.Once);
        }

        [TestMethod]
        public async Task EditPokemonIsSuccessfulAndLogsInformation()
        {
            await _pokedexAppLogic.EditPokemon(new PokemonDetailViewModel()
            {
                MyPokemonId = DataGenerator.DefaultGuid,
                NationalDexPokemonId = 0
            });

            _pokedexRepositoryMock.Verify(prm => prm.EditPokemon(It.Is<tblMyPokedex>(p => p.Id == DataGenerator.DefaultGuid && p.PokemonId == 0)), Times.Once);
            
            _loggerAppMock.Verify(lm => lm.LogInformation("Updated Pokémon: " + DataGenerator.DefaultGuid), Times.Once);
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

            _loggerAppMock.Verify(lm => lm.LogInformation("Mapping 1 Pokémon View Models."), Times.Once);
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

            _loggerAppMock.Verify(lm => lm.LogInformation("Mapping 1 Pokémon View Models."), Times.Once);
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
            _loggerAppMock.Verify(lm => lm.LogInformation("Mapping 5 Pokémon View Models."),Times.Once);
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
            _loggerAppMock.Verify(lm => lm.LogInformation("Mapping 1 Pokémon View Models."), Times.Once);
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
            _loggerAppMock.Verify(lm => lm.LogInformation("Mapping Select List Items."), Times.Once);
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

            _pokedexRepositoryMock.Verify(prm => prm.GetAllAbilities(), Times.Once);
            _pokedexRepositoryMock.Verify(prm => prm.GetAllCategories(), Times.Once);
            _pokedexRepositoryMock.Verify(prm => prm.GetAllPokeballs(), Times.Once);
            _pokedexRepositoryMock.Verify(prm => prm.GetAllTypes(), Times.Once);
            _loggerAppMock.Verify(lm => lm.LogInformation("Mapping Select List Items."), Times.Once);
        }

        [TestMethod]
        public async Task SearchCallsRepository()
        {
            SearchViewModel searchResultsViewModel = await _pokedexAppLogic.Search(new SearchViewModel
            {
                SearchString = "Name0",
                SelectedAbilityId = 0,
                SelectedCategoryId = 0,
                SelectedPokeballId = 0,
                SelectedTypeId = 0
            });

            _pokedexRepositoryMock.Verify(prm => prm.Search("Name0", 0, 0, 0), Times.Once);
            _pokedexRepositoryMock.Verify(prm => prm.Search("Name0", 0, 0, 0, 0), Times.Once);
        }

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
    }
}