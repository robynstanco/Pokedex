using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Data;
using Pokedex.Data.Models;
using Pokedex.Logging.Interfaces;
using Pokedex.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pokedex.Tests.Repositories
{
    [TestClass]
    public class PokedexRepositoryFixture
    {
        private PokedexRepository _pokedexRepository;

        private Mock<POKEDEXDBContext> _pokedexDBContextMock;
        private Mock<ILoggerAdapter<PokedexRepository>> _loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            IQueryable<tblMyPokedex> myPokedex = DataGenerator.GenerateMyPokemon(5).AsQueryable();
            IQueryable<tlkpAbility> abilities = DataGenerator.GenerateAbilities(6).AsQueryable();
            IQueryable<tlkpCategory> categories = DataGenerator.GenerateCategories(5).AsQueryable();
            IQueryable<tlkpNationalDex> nationalDex = DataGenerator.GenerateNationalDexPokemon(5).AsQueryable();
            IQueryable<tlkpPokeball> pokeballs = DataGenerator.GeneratePokeballs(5).AsQueryable();
            IQueryable<tlkpType> types = DataGenerator.GenerateTypes(6).AsQueryable();

            _pokedexDBContextMock = new Mock<POKEDEXDBContext>();

            _pokedexDBContextMock.Setup(dbcm => dbcm.tblMyPokedex).Returns(InitializeMockSet(myPokedex).Object);

            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpAbility).Returns(InitializeMockSet(abilities).Object);

            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpCategory).Returns(InitializeMockSet(categories).Object);

            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpNationalDex).Returns(InitializeMockSet(nationalDex).Object);

            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpPokeball).Returns(InitializeMockSet(pokeballs).Object);

            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpType).Returns(InitializeMockSet(types).Object);

            _pokedexDBContextMock.Setup(dbcm => dbcm.tblMyPokedex.FindAsync(DataGenerator.DefaultGuid))
                .ReturnsAsync((object[] ids) =>
                {
                    return myPokedex.FirstOrDefault(p => p.Id == (Guid)ids[0]);
                });

            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpAbility.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((object[] ids) =>
                {
                    return abilities.FirstOrDefault(p => p.Id == (int)ids[0]);
                });

            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpCategory.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((object[] ids) =>
                {
                    return categories.FirstOrDefault(p => p.Id == (int)ids[0]);
                });

            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpNationalDex.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((object[] ids) =>
                {
                    return nationalDex.FirstOrDefault(p => p.Id == (int)ids[0]);
                });

            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpPokeball.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((object[] ids) =>
                {
                    return pokeballs.FirstOrDefault(p => p.Id == (int)ids[0]);
                });

            _pokedexDBContextMock.Setup(dbcm => dbcm.tlkpType.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((object[] ids) =>
                {
                    return types.FirstOrDefault(p => p.Id == (int)ids[0]);
                });

            _loggerMock = new Mock<ILoggerAdapter<PokedexRepository>>();

            _pokedexRepository = new PokedexRepository(_pokedexDBContextMock.Object, _loggerMock.Object);
        }

        /// <summary>
        /// Generic method to create mock sets from a given T class and a set of queryable data.
        /// </summary>
        /// <typeparam name="T">the class of the data</typeparam>
        /// <param name="queryableEntities">the queryable dataset</param>
        /// <returns>mock db set of class T</returns>
        private Mock<DbSet<T>> InitializeMockSet<T>(IQueryable<T> queryableEntities) where T : class
        {
            Mock<DbSet<T>> mockEntitySet = new Mock<DbSet<T>>();

            mockEntitySet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableEntities.Provider);

            mockEntitySet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableEntities.Expression);

            mockEntitySet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableEntities.ElementType);

            mockEntitySet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableEntities.GetEnumerator());

            mockEntitySet.As<IAsyncEnumerable<T>>().Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new AsyncEnumerator<T>(queryableEntities.GetEnumerator()));

            return mockEntitySet;
        }

        [TestMethod]
        public async Task AddPokemonIsSuccessfulAndLogsInformation()
        {
            tblMyPokedex generatedPokemon = DataGenerator.GenerateMyPokemon(1)[0];

            await _pokedexRepository.AddPokemon(generatedPokemon);

            VerifyDBContextMockAddsAsync(generatedPokemon);

            VerifyDBContextMockSavesChanges(1);

            VerifyLoggerMockLogsInformation("Added Pokémon to DBContext with Id: " + DataGenerator.DefaultGuid);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task DeletePokemonByIdIsSuccessfulAndLogsInformation()
        {
            await _pokedexRepository.DeletePokemonById(DataGenerator.DefaultGuid);

            _pokedexDBContextMock.Verify(m => m.tblMyPokedex.FindAsync(new object[] { DataGenerator.DefaultGuid }), Times.Once);

            VerifyLoggerMockLogsLookupInformationWithId(0);

            VerifyLoggerMockLogsPokeballAndNationalDexWithId(0);

            VerifyLoggerMockLogsDefaultPokemonRetrieval();

            VerifyDBContextMockRemovesPokemon(1);

            VerifyDBContextMockSavesChanges(1);

            VerifyLoggerMockLogsInformation("Deleted Pokémon from DBContext with Id: " + DataGenerator.DefaultGuid);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task DeleteNonExistentPokemonByIdDoesNotLogOrDelete()
        {
            tblMyPokedex tblMyPokedex = await _pokedexRepository.DeletePokemonById(Guid.NewGuid());

            Assert.IsNull(tblMyPokedex);

            _pokedexDBContextMock.Verify(m => m.tblMyPokedex.FindAsync(new object[] { It.IsAny<Guid>() }), Times.Once);

            VerifyDBContextMockRemovesPokemon(0);

            VerifyDBContextMockSavesChanges(0);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task EditPokemonIsSuccessfulAndLogsInformation()
        {
            tblMyPokedex generatedPokemon = DataGenerator.GenerateMyPokemon(1)[0];

            await _pokedexRepository.EditPokemon(generatedPokemon);

            _pokedexDBContextMock.Verify(m => m.tblMyPokedex.FindAsync(new object[] { DataGenerator.DefaultGuid }), Times.Once);

            VerifyDBContextMockRemovesPokemon(1);

            VerifyDBContextMockAddsAsync(generatedPokemon);

            VerifyDBContextMockSavesChanges(2);

            VerifyLoggerMockLogsLookupInformationWithId(0);

            VerifyLoggerMockLogsPokeballAndNationalDexWithId(0);

            VerifyLoggerMockLogsDefaultPokemonRetrieval();

            VerifyLoggerMockLogsInformation("Deleted Pokémon from DBContext with Id: " + DataGenerator.DefaultGuid);

            VerifyLoggerMockLogsInformation("Added Pokémon to DBContext with Id: " + DataGenerator.DefaultGuid);

            VerifyLoggerMockLogsInformation("Updated Pokémon in DBContext with Id: " + DataGenerator.DefaultGuid);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task EditNonExistentPokemonDoesNotUpdateOrLog()
        {
            await _pokedexRepository.EditPokemon(new tblMyPokedex() { Id = Guid.NewGuid() });

            _pokedexDBContextMock.Verify(m => m.tblMyPokedex.FindAsync(new object[] { It.IsAny<Guid>() }), Times.Once);

            VerifyDBContextMockRemovesPokemon(0);

            VerifyDBContextMockSavesChanges(0);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetAbilityByIdIsSuccessfulAndLogsInformation()
        {
            tlkpAbility ability = await _pokedexRepository.GetAbilityById(0);

            Assert.AreEqual(0, ability.Id);
            Assert.AreEqual("Name0", ability.Name);

            _pokedexDBContextMock.Verify(m => m.tlkpAbility, Times.Once);

            VerifyLoggerMockLogsInformation("Retrieved Ability from DBContext with Id: 0");

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetNonExistentAbilityByIdDoesNotLog()
        {
            tlkpAbility ability = await _pokedexRepository.GetAbilityById(-33);

            Assert.IsNull(ability);

            _pokedexDBContextMock.Verify(m => m.tlkpAbility, Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetAllAbilitiesIsSuccessfulAndLogsInformation()
        {
            List<tlkpAbility> abilities = await _pokedexRepository.GetAllAbilities();

            Assert.AreEqual(6, abilities.Count);
            Assert.AreEqual(0, abilities[0].Id);
            Assert.AreEqual("Name0", abilities[0].Name);

            _pokedexDBContextMock.Verify(m => m.tlkpAbility, Times.Once);

            VerifyLoggerMockLogsInformation("Retrieved 6 Abilities from DBContext.");

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetAllCategoriesIsSuccessfulAndLogsInformation()
        {
            List<tlkpCategory> categories = await _pokedexRepository.GetAllCategories();

            Assert.AreEqual(5, categories.Count);
            Assert.AreEqual(0, categories[0].Id);
            Assert.AreEqual("Name0", categories[0].Name);

            _pokedexDBContextMock.Verify(m => m.tlkpCategory, Times.Once);

            VerifyLoggerMockLogsInformation("Retrieved 5 Categories from DBContext.");

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetAllPokeballsIsSuccessfulAndLogsInformation()
        {
            List<tlkpPokeball> pokeballs = await _pokedexRepository.GetAllPokeballs();

            Assert.AreEqual(5, pokeballs.Count);
            Assert.AreEqual(0, pokeballs[0].Id);
            Assert.AreEqual("http://0.com", pokeballs[0].ImageURL);
            Assert.AreEqual("Name0", pokeballs[0].Name);

            _pokedexDBContextMock.Verify(m => m.tlkpPokeball, Times.Once);

            VerifyLoggerMockLogsInformation("Retrieved 5 Pokéballs from DBContext.");

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetAllTypesIsSuccessfulAndLogsInformation()
        {
            List<tlkpType> types = await _pokedexRepository.GetAllTypes();

            Assert.AreEqual(6, types.Count);
            Assert.AreEqual(0, types[0].Id);
            Assert.AreEqual("Name0", types[0].Name);

            _pokedexDBContextMock.Verify(m => m.tlkpType, Times.Once);

            VerifyLoggerMockLogsInformation("Retrieved 6 Types from DBContext.");

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetCategoryByIdIsSuccessfulAndLogsInformation()
        {
            tlkpCategory category = await _pokedexRepository.GetCategoryById(3);

            Assert.AreEqual(3, category.Id);
            Assert.AreEqual("Name3", category.Name);

            _pokedexDBContextMock.Verify(m => m.tlkpCategory, Times.Once);

            VerifyLoggerMockLogsInformation("Retrieved Category from DBContext with Id: 3");

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetNonExistentCategoryByIdIsDoesNotLog()
        {
            tlkpCategory category = await _pokedexRepository.GetCategoryById(-33);

            Assert.IsNull(category);

            _pokedexDBContextMock.Verify(m => m.tlkpCategory, Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetMyPokedexIsSuccessfulAndLogsInformation()
        {
            List<tblMyPokedex> pokedex = await _pokedexRepository.GetMyPokedex();

            Assert.AreEqual(5, pokedex.Count);
            Assert.AreEqual(DataGenerator.DefaultGuid, pokedex[0].Id);
            Assert.AreEqual(DateTime.Today, pokedex[0].Date);
            Assert.AreEqual(1, pokedex[0].Level);
            Assert.AreEqual("0 Main Street", pokedex[0].Location);
            Assert.AreEqual("Nickname0", pokedex[0].Nickname);
            Assert.AreEqual(0, pokedex[0].PokeballId);
            Assert.AreEqual(0, pokedex[0].PokemonId);
            Assert.IsTrue(pokedex[0].Sex.Value); //ie, Sex == bit 1 in SQL.

            _pokedexDBContextMock.Verify(m => m.tblMyPokedex, Times.Once);

            VerifyLoggerMockLogsInformation("Retrieved 5 Pokémon from DBContext.");

            VerifyLoggerMockLogsLookupAndPokemonInformationIdsZeroToFour();

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetMyPokemonByIdIsSuccessfulAndLogsInformation()
        {
            tblMyPokedex pokemon = await _pokedexRepository.GetMyPokemonById(DataGenerator.DefaultGuid);

            Assert.AreEqual(DateTime.Today, pokemon.Date);
            Assert.AreEqual(1, pokemon.Level);
            Assert.AreEqual("0 Main Street", pokemon.Location);
            Assert.AreEqual("Nickname0", pokemon.Nickname);
            Assert.AreEqual(0, pokemon.PokeballId);
            Assert.AreEqual(0, pokemon.PokemonId);
            Assert.IsTrue(pokemon.Sex.Value); //ie, Sex == bit 0 in SQL.

            _pokedexDBContextMock.Verify(m => m.tblMyPokedex, Times.Once);

            VerifyLoggerMockLogsDefaultPokemonRetrieval();

            VerifyLoggerMockLogsLookupInformationWithId(0);
            VerifyLoggerMockLogsPokeballAndNationalDexWithId(0);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetMyNonExistentPokemonByIdDoesNotLog()
        {
            tblMyPokedex pokemon = await _pokedexRepository.GetMyPokemonById(Guid.NewGuid());

            Assert.IsNull(pokemon);

            _pokedexDBContextMock.Verify(m => m.tblMyPokedex, Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetNationalDexIsSuccessfulAndLogsInformation()
        {
            List<tlkpNationalDex> nationalDex = await _pokedexRepository.GetNationalDex();

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

            VerifyLoggerMockLogsLookupInformationWithIdOneToFour();

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetNationalDexPokemonByIdIsSuccessfulAndLogsInformation()
        {
            tlkpNationalDex pokemon = await _pokedexRepository.GetNationalDexPokemonById(0);

            Assert.AreEqual(0, pokemon.AbilityId);
            Assert.AreEqual("Name0", pokemon.Ability.Name);
            Assert.AreEqual(0, pokemon.CategoryId);
            Assert.AreEqual("Name0", pokemon.Category.Name);
            Assert.AreEqual("Desc0", pokemon.Description);
            Assert.AreEqual(1, pokemon.HeightInInches);
            Assert.AreEqual("Name1", pokemon.HiddenAbility.Name);
            Assert.AreEqual(1, pokemon.HiddenAbilityId);
            Assert.AreEqual(0, pokemon.Id);
            Assert.AreEqual("http://0.com", pokemon.ImageURL);
            Assert.AreEqual("JapaneseName0", pokemon.JapaneseName);
            Assert.AreEqual("Name0", pokemon.Name);
            Assert.AreEqual(0, pokemon.TypeOneId);
            Assert.AreEqual(1, pokemon.TypeTwoId);
            Assert.AreEqual(1, pokemon.WeightInPounds);

            _pokedexDBContextMock.Verify(m => m.tlkpNationalDex, Times.Once);

            VerifyLoggerMockLogsInformation("Retrieved Pokémon from DBContext with Id: 0");

            VerifyLoggerMockLogsLookupInformationWithId(0);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetNonExistentNationalDexPokemonByIdDoesNotLog()
        {
            tlkpNationalDex pokemon = await _pokedexRepository.GetNationalDexPokemonById(-333);

            Assert.IsNull(pokemon);

            _pokedexDBContextMock.Verify(m => m.tlkpNationalDex, Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetPokeballByIdIsSuccessfulAndLogsInformation()
        {
            tlkpPokeball pokeball = await _pokedexRepository.GetPokeballById(1);

            Assert.AreEqual(1, pokeball.Id);
            Assert.AreEqual("http://1.com", pokeball.ImageURL);
            Assert.AreEqual("Name1", pokeball.Name);

            _pokedexDBContextMock.Verify(m => m.tlkpPokeball, Times.Once);

            VerifyLoggerMockLogsInformation("Retrieved Pokéball from DBContext with Id: 1");

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetNonExistentPokeballByIdDoesNotLog()
        {
            tlkpPokeball pokeball = await _pokedexRepository.GetPokeballById(-333);

            Assert.IsNull(pokeball);

            _pokedexDBContextMock.Verify(m => m.tlkpPokeball, Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetTypeByIdIsSuccessfulAndLogsInformation()
        {
            tlkpType type = await _pokedexRepository.GetTypeById(1);

            Assert.AreEqual(1, type.Id);
            Assert.AreEqual("Name1", type.Name);

            _pokedexDBContextMock.Verify(m => m.tlkpType, Times.Once);

            VerifyLoggerMockLogsInformation("Retrieved Type from DBContext with Id: 1");

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetNonExistentTypeByIdDoesNotLog()
        {
            tlkpType type = await _pokedexRepository.GetTypeById(-333);

            Assert.IsNull(type);

            _pokedexDBContextMock.Verify(m => m.tlkpType, Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task SearchNationalDexIsSuccessfulAndLogsInformation()
        {
            List<tlkpNationalDex> searchResults = await _pokedexRepository.Search("Name3", 3, 3, 3);

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
            
            VerifyLoggerMockLogsLookupInformationWithIdOneToFour();

            VerifyLoggerMockLogsInformation("Retrieved 1 Pokémon from DBContext matching search string: Name3");

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task SearchPokedexIsSuccessfulAndLogsInformation()
        {
            List<tblMyPokedex> searchResults = await _pokedexRepository.Search("Nickname3", null, null, null, null);

            Assert.AreEqual(DateTime.Today, searchResults[0].Date);
            Assert.AreEqual(4, searchResults[0].Level);
            Assert.AreEqual("3 Main Street", searchResults[0].Location);
            Assert.AreEqual("Nickname3", searchResults[0].Nickname);
            Assert.AreEqual(3, searchResults[0].PokeballId);
            Assert.AreEqual(DataGenerator.DefaultGuid, searchResults[0].Id);
            Assert.IsFalse(searchResults[0].Sex.Value); //ie, Sex == bit 0 in SQL.

            VerifyLoggerMockLogsInformation("Retrieved 5 Pokémon from DBContext.");

            VerifyLoggerMockLogsInformation("Retrieved 1 Pokémon from DBContext matching search string: Nickname3");

            VerifyLoggerMockLogsLookupAndPokemonInformationIdsZeroToFour();

            _loggerMock.VerifyNoOtherCalls();
        }

        private void VerifyLoggerMockLogsLookupAndPokemonInformationIdsZeroToFour()
        {
            VerifyLoggerMockLogsLookupInformationWithId(0);
            VerifyLoggerMockLogsPokeballAndNationalDexWithId(0);

            VerifyLoggerMockLogsLookupInformationWithId(1);
            VerifyLoggerMockLogsPokeballAndNationalDexWithId(1);

            VerifyLoggerMockLogsLookupInformationWithId(2);
            VerifyLoggerMockLogsPokeballAndNationalDexWithId(2);

            VerifyLoggerMockLogsLookupInformationWithId(3);
            VerifyLoggerMockLogsPokeballAndNationalDexWithId(3);

            VerifyLoggerMockLogsLookupInformationWithId(4);
            VerifyLoggerMockLogsPokeballAndNationalDexWithId(4);
        }

        private void VerifyLoggerMockLogsLookupInformationWithIdOneToFour()
        {
            VerifyLoggerMockLogsInformation("Retrieved 5 Pokémon from DBContext.");

            VerifyLoggerMockLogsLookupInformationWithId(0);

            VerifyLoggerMockLogsLookupInformationWithId(1);

            VerifyLoggerMockLogsLookupInformationWithId(2);

            VerifyLoggerMockLogsLookupInformationWithId(3);

            VerifyLoggerMockLogsLookupInformationWithId(4);
        }

        private void VerifyDBContextMockAddsAsync(tblMyPokedex pokemon)
        {
            _pokedexDBContextMock.Verify(m => m.AddAsync(pokemon, It.IsAny<CancellationToken>()), Times.Once);
        }

        private void VerifyDBContextMockSavesChanges(int t)
        {
            _pokedexDBContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(t));
        }

        private void VerifyDBContextMockRemovesPokemon(int t)
        {
            _pokedexDBContextMock.Verify(m => m.Remove(It.IsAny<tblMyPokedex>()), Times.Exactly(t));
        }

        private void VerifyLoggerMockLogsInformation(string info)
        {
            _loggerMock.Verify(lm => lm.LogInformation(info), Times.AtLeastOnce);
        }

        private void VerifyLoggerMockLogsDefaultPokemonRetrieval()
        {
            VerifyLoggerMockLogsInformation("Retrieved Pokémon from DBContext with Id: " + DataGenerator.DefaultGuid);
        }

        private void VerifyLoggerMockLogsLookupInformationWithId(int id)
        {
            VerifyLoggerMockLogsInformation("Retrieved Ability from DBContext with Id: " + id);
            VerifyLoggerMockLogsInformation("Retrieved Category from DBContext with Id: " + id);
            VerifyLoggerMockLogsInformation("Retrieved Ability from DBContext with Id: " + (id + 1));
            VerifyLoggerMockLogsInformation("Retrieved Type from DBContext with Id: " + id);
            VerifyLoggerMockLogsInformation("Retrieved Type from DBContext with Id: " + (id + 1));
        }

        private void VerifyLoggerMockLogsPokeballAndNationalDexWithId(int id)
        {
            VerifyLoggerMockLogsInformation("Retrieved Pokémon from DBContext with Id: " + id);
            VerifyLoggerMockLogsInformation("Retrieved Pokéball from DBContext with Id: " + id);
        }
    }
}