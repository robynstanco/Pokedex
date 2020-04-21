using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Logging.Interfaces;
using PokedexApp.Controllers;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pokedex.Tests.Controllers
{
    [TestClass]
    public class NationalDexMVControllerFixture
    {
        private NationalDexController _nationalDexController;

        private Mock<IPokedexAppLogic> _pokedexAppLogicMock;
        private Mock<ILoggerAdapter<NationalDexController>> _loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            _pokedexAppLogicMock = new Mock<IPokedexAppLogic>();
            _pokedexAppLogicMock.Setup(plm => plm.GetNationalDex()).Returns(It.IsAny<List<PokemonListingViewModel>>());
            _pokedexAppLogicMock.Setup(plm => plm.GetNationalDexPokemonById(It.IsAny<int>())).ReturnsAsync(It.IsAny<PokemonDetailViewModel>());

            _loggerMock = new Mock<ILoggerAdapter<NationalDexController>>();

            _nationalDexController = new NationalDexController(_pokedexAppLogicMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public void IndexActionIsSuccessfulAndCallsLogic()
        {
            _nationalDexController.Index();

            _pokedexAppLogicMock.Verify(plm => plm.GetNationalDex(), Times.Once);
        }

        [TestMethod]
        public async Task DetailActionIsSuccessfullAndCallsLogic()
        {
            await _nationalDexController.Detail(0);

            _pokedexAppLogicMock.Verify(plm => plm.GetNationalDexPokemonById(0), Times.Once);
        }

        [TestMethod]
        public void ErrorActionLogsError()
        {
            Exception error = new Exception("some error");

            _nationalDexController.Error(error);

            _loggerMock.Verify(lm => lm.LogError(error, "some error"), Times.Once);
        }
    }
}