using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Data.Models;
using Pokedex.Repository.Repositories;
using System;
using System.Linq;

namespace Pokedex.Tests.Fixtures
{
    [TestClass]
    public class PokedexRepositoryFixture
    {
        private PokedexRepository _pokedexRepository;

        private Mock<POKEDEXDBContext> _pokedexContextMock;
        private Mock<ILogger<PokedexRepository>> _loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            _pokedexContextMock = new Mock<POKEDEXDBContext>();
            _loggerMock = new Mock<ILogger<PokedexRepository>>();

            //todo setup here

            _pokedexRepository = new PokedexRepository(_pokedexContextMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public void AddPokemonSavesChangesAndLogsInformation()
        {
            tblMyPokedex generatedPokemon = DataGenerator.GeneratePokemon(1).ToList()[0];

            _pokedexRepository.AddPokemon(generatedPokemon);

            _pokedexContextMock.Verify(m => m.Add(generatedPokemon), Times.Once);
            _pokedexContextMock.Verify(m => m.SaveChanges(), Times.Once);

            _loggerMock.Verify(lm => lm.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(),
                It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Exactly(1));
        }
    }
}
