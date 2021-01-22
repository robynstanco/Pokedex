using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Logging.Interfaces;
using PokedexAPI.Controllers;
using PokedexAPI.Interfaces;
using PokedexAPI.Models.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pokedex.Tests.Controllers
{
    [TestClass]
    public class PokeballsAPIControllerFixture
    {
        private Mock<IPokedexAPILogic> _pokedexAPILogicMock;
        private Mock<ILoggerAdapter<PokeballsController>> _loggerMock;

        private PokeballsController _pokeballsController;

        [TestInitialize]
        public void Intitialize()
        {
            _pokedexAPILogicMock = new Mock<IPokedexAPILogic>();
            _pokedexAPILogicMock.Setup(plm => plm.GetAllPokeballs()).ReturnsAsync(It.IsAny<List<GenericLookupResult>>());
            _pokedexAPILogicMock.Setup(plm => plm.GetPokeballById(1)).ReturnsAsync(new GenericLookupResult { Id = 1 });

            _loggerMock = new Mock<ILoggerAdapter<PokeballsController>>();

            _pokeballsController = new PokeballsController(_pokedexAPILogicMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetPokeballsIsSuccessfulAndCallsRepository()
        {
            await _pokeballsController.GetPokeballs();

            _pokedexAPILogicMock.Verify(plm => plm.GetAllPokeballs(), Times.Once);
        }

        [TestMethod]
        public async Task GetPokeballByIdIsSuccessfulAndCallsRepository()
        {
            await _pokeballsController.GetPokeballById(1);

            _pokedexAPILogicMock.Verify(plm => plm.GetPokeballById(1), Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }


        [TestMethod]
        public async Task GetPokeballByInvalidIdCallsRepositoryAndLogsInformation()
        {
            await _pokeballsController.GetPokeballById(2);

            _pokedexAPILogicMock.Verify(prm => prm.GetPokeballById(2), Times.Once);

            _loggerMock.Verify(lm => lm.LogInformation("Invalid Request for Pokéball with Id: 2"), Times.Once);
        }
    }
}
