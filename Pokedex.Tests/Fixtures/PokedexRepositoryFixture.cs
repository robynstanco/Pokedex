using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Data.Models;
using Pokedex.Logging.Interfaces;
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
        private Mock<ILoggerAdapter<PokedexRepository>> _loggerMock;

        private Mock<DbSet<tblMyPokedex>> _myPokedexMockSet;
        private Mock<DbSet<tlkpAbility>> _abilitiesMockSet;
        private Mock<DbSet<tlkpCategory>> _categoriesMockSet;
        private Mock<DbSet<tlkpNationalDex>> _nationalDexMockSet;
        private Mock<DbSet<tlkpPokeball>> _pokeballsMockSet;
        private Mock<DbSet<tlkpType>> _typesMockSet;

        [TestInitialize]
        public void Initialize()
        {
            _myPokedexMockSet = InitializeMockSet(DataGenerator.GenerateMyPokemon(5).AsQueryable());
            _abilitiesMockSet = InitializeMockSet(DataGenerator.GenerateAbilities(5).AsQueryable());
            _categoriesMockSet = InitializeMockSet(DataGenerator.GenerateCategories(5).AsQueryable());
            _nationalDexMockSet = InitializeMockSet(DataGenerator.GenerateNationalDexPokemon(5).AsQueryable());
            _pokeballsMockSet = InitializeMockSet(DataGenerator.GeneratePokeballs(5).AsQueryable());
            _typesMockSet = InitializeMockSet(DataGenerator.GenerateTypes(5).AsQueryable());

            _pokedexDBContextMock = new Mock<POKEDEXDBContext>();
            _pokedexDBContextMock.Setup(dbcm => dbcm.tblMyPokedex).Returns(_myPokedexMockSet.Object);
            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpAbility).Returns(_abilitiesMockSet.Object);
            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpCategory).Returns(_categoriesMockSet.Object);
            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpNationalDex).Returns(_nationalDexMockSet.Object);
            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpPokeball).Returns(_pokeballsMockSet.Object);
            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpType).Returns(_typesMockSet.Object);

            _loggerMock = new Mock<ILoggerAdapter<PokedexRepository>>();

            _pokedexRepository = new PokedexRepository(_pokedexDBContextMock.Object, _loggerMock.Object);
        }

        /// <summary>
        /// Generic method to create mock sets from a given T class and a set of queryable data
        /// </summary>
        /// <typeparam name="T">the class of the data</typeparam>
        /// <param name="queryableEntities">the data</param>
        /// <returns>mock db set of class T</returns>
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
            tblMyPokedex generatedPokemon = DataGenerator.GenerateMyPokemon(1)[0];

            _pokedexRepository.AddPokemon(generatedPokemon);

            _pokedexDBContextMock.Verify(m => m.Add(generatedPokemon), Times.Once);
            _pokedexDBContextMock.Verify(m => m.SaveChanges(), Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Added Pokemon to DBContext with Id: 0"), Times.Once);
        }

        [TestMethod]
        public void DeletePokemonByIdIsSuccessfulAndLogsInformation()
        {
            _pokedexRepository.DeletePokemonById(1);
        
            _pokedexDBContextMock.Verify(m => m.tblMyPokedex, Times.Once);
            _pokedexDBContextMock.Verify(m => m.Remove(It.IsAny<tblMyPokedex>()), Times.Once);
            _pokedexDBContextMock.Verify(m => m.SaveChanges(), Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved Pokemon from DBContext with Id: 1"), Times.Once);
            _loggerMock.Verify(lm => lm.LogInformation("Deleted Pokemon from DBContext with Id: 1"), Times.Once);
        }

        [TestMethod]
        public void EditPokemonIsSuccessfulAndLogsInformation()
        {
            tblMyPokedex generatedPokemon = DataGenerator.GenerateMyPokemon(1)[0];

            _pokedexRepository.EditPokemon(generatedPokemon);

            _pokedexDBContextMock.Verify(m => m.Update(generatedPokemon), Times.Once);
            _pokedexDBContextMock.Verify(m => m.SaveChanges(), Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Updated Pokemon in DBContext with Id: 0"), Times.Once);
        }

        [TestMethod]
        public void GetAbilityByIdIsSuccessfulAndLogsInformation()
        {
            tlkpAbility ability = _pokedexRepository.GetAbilityById(1);

            Assert.AreEqual(1, ability.Id);
            Assert.AreEqual("Name1", ability.Name);

            _pokedexDBContextMock.Verify(m => m.tlkpAbility, Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved Ability from DBContext with Id: 1"), Times.Once);
        }

        [TestMethod]
        public void GetAllAbilitiesIsSuccessfulAndLogsInformation()
        {
            List<tlkpAbility> abilities = _pokedexRepository.GetAllAbilities();

            Assert.AreEqual(5, abilities.Count);
            Assert.AreEqual(0, abilities[0].Id);
            Assert.AreEqual("Name0", abilities[0].Name);

            _pokedexDBContextMock.Verify(m => m.tlkpAbility, Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved 5 Abilities from DBContext."), Times.Once);
        }

        [TestMethod]
        public void GetAllCategoriesIsSuccessfulAndLogsInformation()
        {
            List<tlkpCategory> categories = _pokedexRepository.GetAllCategories();

            Assert.AreEqual(5, categories.Count);
            Assert.AreEqual(0, categories[0].Id);
            Assert.AreEqual("Name0", categories[0].Name);

            _pokedexDBContextMock.Verify(m => m.tlkpCategory, Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved 5 Categories from DBContext."), Times.Once);
        }

        [TestMethod]
        public void GetAllPokeballsIsSuccessfulAndLogsInformation()
        {
            List<tlkpPokeball> pokeballs = _pokedexRepository.GetAllPokeballs();

            Assert.AreEqual(5, pokeballs.Count);
            Assert.AreEqual(0, pokeballs[0].Id);
            Assert.AreEqual("http://0.com", pokeballs[0].ImageURL);
            Assert.AreEqual("Name0", pokeballs[0].Name);

            _pokedexDBContextMock.Verify(m => m.tlkpPokeball, Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved 5 Pokeballs from DBContext."), Times.Once);
        }

        [TestMethod]
        public void GetAllTypesIsSuccessfulAndLogsInformation()
        {
            List<tlkpType> types = _pokedexRepository.GetAllTypes();

            Assert.AreEqual(5, types.Count);
            Assert.AreEqual(0, types[0].Id);
            Assert.AreEqual("Name0", types[0].Name);

            _pokedexDBContextMock.Verify(m => m.tlkpType, Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved 5 Types from DBContext."), Times.Once);
        }

        [TestMethod]
        public void GetCategoryByIdIsSuccessfulAndLogsInformation()
        {
            tlkpCategory category = _pokedexRepository.GetCategoryById(3);

            Assert.AreEqual(3, category.Id);
            Assert.AreEqual("Name3", category.Name);

            _pokedexDBContextMock.Verify(m => m.tlkpCategory, Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved Category from DBContext with Id: 3"), Times.Once);
        }

        [TestMethod]
        public void GetMyPokedexIsSuccessfulAndLogsInformation()
        {
            List<tblMyPokedex> pokedex = _pokedexRepository.GetMyPokedex();

            Assert.AreEqual(5, pokedex.Count);
            Assert.AreEqual(0, pokedex[0].Id);
            Assert.AreEqual(DateTime.Today, pokedex[0].Date);
            Assert.AreEqual(1, pokedex[0].Level);
            Assert.AreEqual("0 Main Street", pokedex[0].Location);
            Assert.AreEqual("Nickname0", pokedex[0].Nickname);
            Assert.AreEqual(0, pokedex[0].PokeballId);
            Assert.AreEqual(0, pokedex[0].PokemonId);
            Assert.IsTrue(pokedex[0].Sex.Value); //ie, Sex == bit 1 in SQL.

            _pokedexDBContextMock.Verify(m => m.tblMyPokedex, Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved 5 Pokemon from DBContext."), Times.Once);
        }

        [TestMethod]
        public void GetMyPokemonByIdIsSuccessfulAndLogsInformation()
        {
            tblMyPokedex pokemon = _pokedexRepository.GetMyPokemonById(3);

            Assert.AreEqual(DateTime.Today, pokemon.Date);
            Assert.AreEqual(4, pokemon.Level);
            Assert.AreEqual("3 Main Street", pokemon.Location);
            Assert.AreEqual("Nickname3", pokemon.Nickname);
            Assert.AreEqual(3, pokemon.PokeballId);
            Assert.AreEqual(3, pokemon.PokemonId);
            Assert.IsFalse(pokemon.Sex.Value); //ie, Sex == bit 0 in SQL.

            _pokedexDBContextMock.Verify(m => m.tblMyPokedex, Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved Pokemon from DBContext with Id: 3"), Times.Once);
        }

        [TestMethod]
        public void GetNationalDexIsSuccessfulAndLogsInformation()
        {
            List<tlkpNationalDex> nationalDex = _pokedexRepository.GetNationalDex();

            Assert.AreEqual(5, nationalDex.Count);
            Assert.AreEqual(0, nationalDex[0].AbilityId);
            Assert.AreEqual(0, nationalDex[0].CategoryId);
            Assert.AreEqual("Desc0", nationalDex[0].Description);
            Assert.AreEqual(1, nationalDex[0].HeightInInches);
            Assert.AreEqual(1, nationalDex[0].HiddenAbilityId);
            Assert.AreEqual(0, nationalDex[0].Id);
            Assert.AreEqual("http://0.com", nationalDex[0].ImageURL);
            Assert.AreEqual("JapaneseName0", nationalDex[0].JapaneseName);
            Assert.AreEqual("Name0", nationalDex[0].Name);
            Assert.AreEqual(0, nationalDex[0].TypeOneId);
            Assert.AreEqual(1, nationalDex[0].TypeTwoId);
            Assert.AreEqual(1, nationalDex[0].WeightInPounds);

            _pokedexDBContextMock.Verify(m => m.tlkpNationalDex, Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved 5 Pokemon from DBContext."), Times.Once);
        }

        [TestMethod]
        public void GetNationalDexPokemonByIdIsSuccessfulAndLogsInformation()
        {
            tlkpNationalDex pokemon = _pokedexRepository.GetNationalDexPokemonById(4);

            Assert.AreEqual(4, pokemon.AbilityId);
            Assert.AreEqual(4, pokemon.CategoryId);
            Assert.AreEqual("Desc4", pokemon.Description);
            Assert.AreEqual(5, pokemon.HeightInInches);
            Assert.AreEqual(5, pokemon.HiddenAbilityId);
            Assert.AreEqual(4, pokemon.Id);
            Assert.AreEqual("http://4.com", pokemon.ImageURL);
            Assert.AreEqual("JapaneseName4", pokemon.JapaneseName);
            Assert.AreEqual("Name4", pokemon.Name);
            Assert.AreEqual(4, pokemon.TypeOneId);
            Assert.AreEqual(5, pokemon.TypeTwoId);
            Assert.AreEqual(5, pokemon.WeightInPounds);

            _pokedexDBContextMock.Verify(m => m.tlkpNationalDex, Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved Pokemon from DBContext with Id: 4"), Times.Once);
        }

        [TestMethod]
        public void GetPokeballByIdIsSuccessfulAndLogsInformation()
        {
            tlkpPokeball pokeball = _pokedexRepository.GetPokeballById(1);
            
            Assert.AreEqual(1, pokeball.Id);
            Assert.AreEqual("http://1.com", pokeball.ImageURL);
            Assert.AreEqual("Name1", pokeball.Name);

            _pokedexDBContextMock.Verify(m => m.tlkpPokeball, Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved Pokeball from DBContext with Id: 1"), Times.Once);
        }

        [TestMethod]
        public void GetTypeByIdIsSuccessfulAndLogsInformation()
        {
            tlkpType type = _pokedexRepository.GetTypeById(1);

            Assert.AreEqual(1, type.Id);
            Assert.AreEqual("Name1", type.Name);

            _pokedexDBContextMock.Verify(m => m.tlkpType, Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved Type from DBContext with Id: 1"), Times.Once);
        }

        [TestMethod]
        public void SearchNationalDexIsSuccessfulAndLogsInformation()
        {
            List<tlkpNationalDex> searchResults = _pokedexRepository.Search("Name3", 3, 3, 3);

            Assert.AreEqual(1, searchResults.Count);
            Assert.AreEqual(3, searchResults[0].AbilityId);
            Assert.AreEqual(3, searchResults[0].CategoryId);
            Assert.AreEqual("Desc3", searchResults[0].Description);
            Assert.AreEqual(4, searchResults[0].HeightInInches);
            Assert.AreEqual(4, searchResults[0].HiddenAbilityId);
            Assert.AreEqual(3, searchResults[0].Id);
            Assert.AreEqual("http://3.com", searchResults[0].ImageURL);
            Assert.AreEqual("JapaneseName3", searchResults[0].JapaneseName);
            Assert.AreEqual("Name3", searchResults[0].Name);
            Assert.AreEqual(3, searchResults[0].TypeOneId);
            Assert.AreEqual(4, searchResults[0].TypeTwoId);
            Assert.AreEqual(4, searchResults[0].WeightInPounds);

            _pokedexDBContextMock.Verify(m => m.tlkpNationalDex, Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved 5 Pokemon from DBContext."), Times.Once);
            _loggerMock.Verify(lm => lm.LogInformation("Retrieved 1 Pokemon from DBContext matching search string: Name3"), Times.Once);
        }

        [TestMethod]
        public void SearchPokedexIsSuccessfulAndLogsInformation()
        {
            List<tblMyPokedex> searchResults = _pokedexRepository.Search("Nickname3", null, null, null, null);

            Assert.AreEqual(DateTime.Today, searchResults[0].Date);
            Assert.AreEqual(4, searchResults[0].Level);
            Assert.AreEqual("3 Main Street", searchResults[0].Location);
            Assert.AreEqual("Nickname3", searchResults[0].Nickname);
            Assert.AreEqual(3, searchResults[0].PokeballId);
            Assert.AreEqual(3, searchResults[0].Id);
            Assert.IsFalse(searchResults[0].Sex.Value); //ie, Sex == bit 0 in SQL.

            _loggerMock.Verify(lm => lm.LogInformation("Retrieved 5 Pokemon from DBContext."), Times.Once);
            _loggerMock.Verify(lm => lm.LogInformation("Retrieved 1 Pokemon from DBContext matching search string: Nickname3"), Times.Once);
        }
    }
}