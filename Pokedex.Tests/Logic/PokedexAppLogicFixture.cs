using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Data.Models;
using Pokedex.Logging.Interfaces;
using Pokedex.Repository.Interfaces;
using PokedexApp.Logic;
using PokedexApp.Models;
using System.Collections.Generic;

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
            _pokedexRepositoryMock.Setup(prm => prm.GetCategoryById(0)).Returns(DataGenerator.GenerateCategories(1)[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetNationalDex()).Returns(DataGenerator.GenerateNationalDexPokemon(5));
            _pokedexRepositoryMock.Setup(prm => prm.GetNationalDexPokemonById(0)).Returns(DataGenerator.GenerateNationalDexPokemon(1)[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetTypeById(0)).Returns(types[0]);
            _pokedexRepositoryMock.Setup(prm => prm.GetTypeById(1)).Returns(types[1]); //Type Two

            _loggerMock = new Mock<ILoggerAdapter<PokedexAppLogic>>();

            _pokedexAppLogic = new PokedexAppLogic(_pokedexRepositoryMock.Object, _loggerMock.Object);
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
            _loggerMock.Verify(lm => lm.LogInformation("Mapping 5 Pokémon View Models."));
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
            _loggerMock.Verify(lm => lm.LogInformation("Mapping 1 Pokémon View Models."));
        }
    }
}
