using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Common.Interfaces;
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


        private Mock<IPaginationHelper> _paginationHelperMock;
        private Mock<IPokedexAppLogic> _pokedexAppLogicMock;
        private Mock<ILoggerAdapter<NationalDexController>> _loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            _paginationHelperMock = new Mock<IPaginationHelper>();

            _pokedexAppLogicMock = new Mock<IPokedexAppLogic>();

            _pokedexAppLogicMock.Setup(plm => plm.GetNationalDex())
                .ReturnsAsync(It.IsAny<List<PokemonListingViewModel>>());

            _pokedexAppLogicMock.Setup(plm => plm.GetNationalDexPokemonById(It.IsAny<int>()))
                .ReturnsAsync(It.IsAny<PokemonDetailViewModel>());

            _loggerMock = new Mock<ILoggerAdapter<NationalDexController>>();

            _nationalDexController = new NationalDexController(_pokedexAppLogicMock.Object, 
                _paginationHelperMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task IndexActionIsSuccessfulAndCallsLogic()
        {
            await _nationalDexController.Index(3, 33);

            _pokedexAppLogicMock.Verify(plm => plm.GetNationalDex(), Times.Once);

            _paginationHelperMock.Verify(plm => plm.GetPagedResults<PokemonListingViewModel>(null, 3, 33),
                Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task IndexActionWithLogicExceptionLogsError()
        {
            _pokedexAppLogicMock.Setup(plm => plm.GetNationalDex())
                .ThrowsAsync(new Exception("some exception"));

            await _nationalDexController.Index(2, 22);
            
            _pokedexAppLogicMock.Verify(plm => plm.GetNationalDex(), Times.Once);

            _paginationHelperMock.Verify(plm => plm.GetPagedResults<PokemonListingViewModel>(null, 2, 22),
                Times.Never);

            VerifyLoggerMockLoggedError("some exception");

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task IndexActionWithPaginationExceptionLogsError()
        {
            _paginationHelperMock.Setup(phm => phm.GetPagedResults(
                It.IsAny<IEnumerable<PokemonListingViewModel>>(), 1, 11))
                .Throws(new Exception("some exception"));

            await _nationalDexController.Index(1, 11);

            _pokedexAppLogicMock.Verify(plm => plm.GetNationalDex(), Times.Once);

            _paginationHelperMock.Verify(plm => plm.GetPagedResults<PokemonListingViewModel>(null, 1, 11),
                Times.Once);

            VerifyLoggerMockLoggedError("some exception");

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task DetailActionIsSuccessfullAndCallsLogic()
        {
            await _nationalDexController.Detail(0);

            _pokedexAppLogicMock.Verify(plm => plm.GetNationalDexPokemonById(0), Times.Once);

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task DetailActionWithLogicExceptionLogsError()
        {
            _pokedexAppLogicMock.Setup(plm => plm.GetNationalDexPokemonById(0))
                .ThrowsAsync(new Exception("some exception"));

            await _nationalDexController.Detail(0);

            _pokedexAppLogicMock.Verify(plm => plm.GetNationalDexPokemonById(0), Times.Once);

            VerifyLoggerMockLoggedError("some exception");

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void ErrorActionLogsError()
        {
            Exception error = new Exception("some error");

            IActionResult result = _nationalDexController.Error(error);

            ErrorViewModel errorViewModel = DataGenerator.GetViewModel<ErrorViewModel>(result);

            Assert.AreEqual("some error", errorViewModel.Message);

            VerifyLoggerMockLoggedError("some error");

            _loggerMock.VerifyNoOtherCalls();
        }

        private void VerifyLoggerMockLoggedError(string error)
        {
            _loggerMock.Verify(lm => lm.LogError(It.IsAny<Exception>(), error), Times.Once);
        }
    }
}