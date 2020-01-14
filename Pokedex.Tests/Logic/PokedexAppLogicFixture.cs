using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Data.Models;
using Pokedex.Logging.Interfaces;
using Pokedex.Repository.Interfaces;
using PokedexApp.Logic;
using PokedexApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.Tests.Logic
{
    [TestClass]
    public class PokedexAppLogicFixture
    {
        private PokedexAppLogic _pokedexAppLogic;

        private Mock<IPokedexRepository> _pokedexRepositoryMock;

        private Mock<ILoggerAdapter<PokedexAppLogic>> _loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            List<tlkpAbility> abilities = DataGenerator.GenerateAbilities(2);
            List<tlkpType> types = DataGenerator.GenerateTypes(2);

            _pokedexRepositoryMock = new Mock<IPokedexRepository>();
            _pokedexRepositoryMock.Setup(prm => prm.GetAbilityById(0)).Returns(abilities[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetAbilityById(1)).Returns(abilities[1]); //Hidden Ability
            _pokedexRepositoryMock.Setup(prm => prm.GetAllPokeballs()).Returns(DataGenerator.GeneratePokeballs(5));
            _pokedexRepositoryMock.Setup(prm => prm.GetCategoryById(0)).Returns(DataGenerator.GenerateCategories(1)[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetMyPokedex()).Returns(DataGenerator.GenerateMyPokemon(1));
            _pokedexRepositoryMock.Setup(prm => prm.GetMyPokemonById(DataGenerator.DefaultGuid)).Returns(DataGenerator.GenerateMyPokemon(1)[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetNationalDex()).Returns(DataGenerator.GenerateNationalDexPokemon(5));
            _pokedexRepositoryMock.Setup(prm => prm.GetNationalDexPokemonById(0)).Returns(DataGenerator.GenerateNationalDexPokemon(1)[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetPokeballById(0)).Returns(DataGenerator.GeneratePokeballs(1)[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetTypeById(0)).Returns(types[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetTypeById(1)).Returns(types[1]); //Type Two

            _loggerMock = new Mock<ILoggerAdapter<PokedexAppLogic>>();

            _pokedexAppLogic = new PokedexAppLogic(_pokedexRepositoryMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public void AddPokemonIsSuccessfulAndLogsInformation()
        {
            Assert.Fail("todo");
        }

        [TestMethod]
        public void GetMyPokedexIsSuccessfulAndLogsInformation()
        {
            List<PokemonListingViewModel> pokemonListingViewModels = _pokedexAppLogic.GetMyPokedex();

            Assert.AreEqual(1, pokemonListingViewModels.Count);
            Assert.AreEqual("http://0.com", pokemonListingViewModels[0].ImageURL);
            Assert.AreEqual(DataGenerator.DefaultGuid, pokemonListingViewModels[0].MyPokemonId);
            Assert.AreEqual("Name0", pokemonListingViewModels[0].Name);
            Assert.AreEqual("Nickname0", pokemonListingViewModels[0].Nickname);
            Assert.AreEqual(0, pokemonListingViewModels[0].NationalDexPokemonId);
            
            _pokedexRepositoryMock.Verify(prm => prm.GetMyPokedex(), Times.Once);
            _loggerMock.Verify(lm => lm.LogInformation("Mapping 1 Pokémon View Models."), Times.Once);
        }

        [TestMethod]
        public void GetMyPokemonByIdIsSuccessfulAndLogsInformation()
        {
            PokemonDetailViewModel pokemonDetailViewModel = _pokedexAppLogic.GetMyPokemonById(DataGenerator.DefaultGuid);

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

            _pokedexRepositoryMock.Verify(prm => prm.GetNationalDexPokemonById(0), Times.Once);
            _loggerMock.Verify(lm => lm.LogInformation("Mapping 1 Pokémon View Models."), Times.Once);
        }

        [TestMethod]
        public void GetNationalDexIsSuccessfulAndLogsInformation()
        {
            List<PokemonListingViewModel> pokemonListingViewModels = _pokedexAppLogic.GetNationalDex();

            Assert.AreEqual(5, pokemonListingViewModels.Count);
            Assert.AreEqual("http://0.com", pokemonListingViewModels[0].ImageURL);
            Assert.AreEqual("Name0", pokemonListingViewModels[0].Name);
            Assert.AreEqual(0, pokemonListingViewModels[0].NationalDexPokemonId);

            Assert.IsNull(pokemonListingViewModels[0].MyPokemonId);
            Assert.IsNull(pokemonListingViewModels[0].Nickname);

            _pokedexRepositoryMock.Verify(prm => prm.GetNationalDex(), Times.Once);
            _loggerMock.Verify(lm => lm.LogInformation("Mapping 5 Pokémon View Models."),Times.Once);
        }

        [TestMethod]
        public void GetNationalDexPokemonByIdIsSuccessfulAndLogsInformation()
        {
            PokemonDetailViewModel pokemonDetailViewModel = _pokedexAppLogic.GetNationalDexPokemonById(0);

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
            _loggerMock.Verify(lm => lm.LogInformation("Mapping 1 Pokémon View Models."), Times.Once);
        }

        [TestMethod]
        public void GetNewPokemonFormIsSuccessfulAndLogsInformation()
        {
            PokemonFormViewModel pokemonFormViewModel = _pokedexAppLogic.GetNewPokemonForm();

            List<SelectListItem> nationalDexOptions = pokemonFormViewModel.NationalDexOptions.ToList();
            List<SelectListItem> pokeballOptions = pokemonFormViewModel.PokeballOptions.ToList();
            List<SelectListItem> sexOptions = pokemonFormViewModel.SexOptions.ToList();

            Assert.AreEqual(5, nationalDexOptions.Count);
            Assert.AreEqual("Name0", nationalDexOptions[0].Text);
            Assert.AreEqual("0", nationalDexOptions[0].Value);
            Assert.AreEqual(5, pokeballOptions.Count);
            Assert.AreEqual("Name0", pokeballOptions[0].Text);
            Assert.AreEqual("0", pokeballOptions[0].Value);
            Assert.AreEqual(3, sexOptions.Count);
            Assert.AreEqual("Female", sexOptions[0].Text);
            Assert.AreEqual("0", sexOptions[0].Value);

            _pokedexRepositoryMock.Verify(prm => prm.GetAllPokeballs(), Times.Once);
            _pokedexRepositoryMock.Verify(prm => prm.GetNationalDex(), Times.Once);
            _loggerMock.Verify(lm => lm.LogInformation("Mapping Select List Items."), Times.Once);
        }
    }
}