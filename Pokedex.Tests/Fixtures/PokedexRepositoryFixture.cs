using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Data.Models;
using Pokedex.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.Tests.Fixtures
{
    [TestClass]
    public class PokedexRepositoryFixture
    {
        private PokedexRepository _pokedexRepository;

        private Mock<POKEDEXDBContext> _pokedexDBContextMock;
        private Mock<ILogger<PokedexRepository>> _loggerMock;

        private Mock<DbSet<tblMyPokedex>> _myPokedexMockSet;
        private Mock<DbSet<tlkpAbility>> _abilitiesMockSet;
        private Mock<DbSet<tlkpCategory>> _categoriesMockSet;
        private Mock<DbSet<tlkpPokeball>> _pokeballsMockSet;

        private IQueryable<tblMyPokedex> _queryablePokedex;
        private IQueryable<tlkpAbility> _queryableAbilities;
        private IQueryable<tlkpCategory> _queryableCategories;
        private IQueryable<tlkpPokeball> _queryablePokeballs;

        [TestInitialize]
        public void Initialize()
        {
            _queryablePokedex = DataGenerator.GeneratePokemon(5).AsQueryable();
            _queryableAbilities = DataGenerator.GenerateAbilities(5).AsQueryable();
            _queryableCategories = DataGenerator.GenerateCategories(5).AsQueryable();
            _queryablePokeballs = DataGenerator.GeneratePokeballs(5).AsQueryable();

            _myPokedexMockSet = InitializeMockSet(_queryablePokedex);
            _abilitiesMockSet = InitializeMockSet(_queryableAbilities);
            _categoriesMockSet = InitializeMockSet(_queryableCategories);
            _pokeballsMockSet = InitializeMockSet(_queryablePokeballs);

            _pokedexDBContextMock = new Mock<POKEDEXDBContext>();
            _pokedexDBContextMock.Setup(dbcm => dbcm.tblMyPokedex).Returns(_myPokedexMockSet.Object);
            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpAbility).Returns(_abilitiesMockSet.Object);
            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpCategory).Returns(_categoriesMockSet.Object);
            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpPokeball).Returns(_pokeballsMockSet.Object);

            _loggerMock = new Mock<ILogger<PokedexRepository>>();

            _pokedexRepository = new PokedexRepository(_pokedexDBContextMock.Object, _loggerMock.Object);
        }

        /// <summary>
        /// Generic method to create mock sets from a given T class and a set of queryable data
        /// </summary>
        /// <typeparam name="T">class</typeparam>
        /// <param name="queryableEntities">data</param>
        /// <returns>mock set of class T</returns>
        private Mock<DbSet<T>> InitializeMockSet<T>(IQueryable<T> queryableEntities) where T : class
        {
            Mock<DbSet<T>> mockEntitySet = new Mock<DbSet<T>>();

            mockEntitySet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableEntities.Provider);
            mockEntitySet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableEntities.Expression);
            mockEntitySet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableEntities.ElementType);
            mockEntitySet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableEntities.GetEnumerator());

            return mockEntitySet;
        }

        [TestMethod]
        public void AddPokemonIsSuccessfulAndLogsInformation()
        {
            tblMyPokedex generatedPokemon = DataGenerator.GeneratePokemon(1)[0];

            _pokedexRepository.AddPokemon(generatedPokemon);

            _pokedexDBContextMock.Verify(m => m.Add(generatedPokemon), Times.Once);
            _pokedexDBContextMock.Verify(m => m.SaveChanges(), Times.Once);

            _loggerMock.Verify(lm => lm.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(),
                It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Exactly(1));
        }

        [TestMethod]
        public void DeletePokemonByIdIsSuccessfulandLogsInformation()
        {
            tblMyPokedex deletedPokemon = _pokedexRepository.DeletePokemonById(1);
        
            Assert.AreEqual(DateTime.Today, deletedPokemon.Date);
            Assert.AreEqual(2, deletedPokemon.Level);
            Assert.AreEqual("1 Main Street", deletedPokemon.Location);
            Assert.AreEqual("Nickname1", deletedPokemon.Nickname);
            Assert.AreEqual(1, deletedPokemon.PokeballId);
            Assert.AreEqual(1, deletedPokemon.PokemonId);
            Assert.IsFalse(deletedPokemon.Sex.Value); //ie, Sex is bit 0.

            _pokedexDBContextMock.Verify(m => m.tblMyPokedex, Times.Once);
            _pokedexDBContextMock.Verify(m => m.Remove(It.IsAny<tblMyPokedex>()), Times.Once);
            _pokedexDBContextMock.Verify(m => m.SaveChanges(), Times.Once);

            _loggerMock.Verify(lm => lm.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(),
                It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Exactly(2));
        }

        [TestMethod]
        public void EditPokemonIsSuccessfulAndLogsInformation()
        {
            tblMyPokedex generatedPokemon = DataGenerator.GeneratePokemon(1)[0];

            _pokedexRepository.EditPokemon(generatedPokemon);

            _pokedexDBContextMock.Verify(m => m.Update(generatedPokemon), Times.Once);
            _pokedexDBContextMock.Verify(m => m.SaveChanges(), Times.Once);

            _loggerMock.Verify(lm => lm.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(),
                It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Exactly(1));
        }

        [TestMethod]
        public void GetAbilityByIdIsSuccessfulandLogsInformation()
        {
            tlkpAbility ability = _pokedexRepository.GetAbilityById(1);

            Assert.AreEqual(1, ability.Id);
            Assert.AreEqual("Name1", ability.Name);

            _pokedexDBContextMock.Verify(m => m.tlkpAbility, Times.Once);

            _loggerMock.Verify(lm => lm.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(),
                It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Exactly(1));
        }

        [TestMethod]
        public void GetAllAbilitiesIsSuccessfulandLogsInformation()
        {
            List<tlkpAbility> abilities = _pokedexRepository.GetAllAbilities();

            Assert.AreEqual(5, abilities.Count);
            Assert.AreEqual(0, abilities[0].Id);
            Assert.AreEqual("Name0", abilities[0].Name);

            _pokedexDBContextMock.Verify(m => m.tlkpAbility, Times.Once);

            _loggerMock.Verify(lm => lm.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(),
                It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Exactly(1));
        }

        [TestMethod]
        public void GetAllCategoriesIsSuccessfulandLogsInformation()
        {
            List<tlkpCategory> categories = _pokedexRepository.GetAllCategories();

            Assert.AreEqual(5, categories.Count);
            Assert.AreEqual(0, categories[0].Id);
            Assert.AreEqual("Name0", categories[0].Name);

            _pokedexDBContextMock.Verify(m => m.tlkpCategory, Times.Once);

            _loggerMock.Verify(lm => lm.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(),
                It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Exactly(1));
        }

        [TestMethod]
        public void GetAllCPokeballsIsSuccessfulandLogsInformation()
        {
            List<tlkpPokeball> pokeballs = _pokedexRepository.GetAllPokeballs();

            Assert.AreEqual(5, pokeballs.Count);
            Assert.AreEqual(0, pokeballs[0].Id);
            Assert.AreEqual("http://0.com", pokeballs[0].ImageURL);
            Assert.AreEqual("Name0", pokeballs[0].Name);

            _pokedexDBContextMock.Verify(m => m.tlkpPokeball, Times.Once);

            _loggerMock.Verify(lm => lm.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(),
                It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Exactly(1));
        }
    }
}
