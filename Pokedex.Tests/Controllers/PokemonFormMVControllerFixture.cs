using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Logging.Interfaces;
using PokedexApp.Controllers;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;
using System.Threading.Tasks;

namespace Pokedex.Tests.Controllers
{
    [TestClass]
    public class PokemonFormMVControllerFixture
    {
        private PokemonFormController _pokemonFormController;

        private Mock<IPokedexAppLogic> _pokedexAppLogicMock;
        private Mock<ILoggerAdapter<PokemonFormController>> _loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            _pokedexAppLogicMock = new Mock<IPokedexAppLogic>();

            _pokedexAppLogicMock.Setup(plm => plm.GetNewPokemonForm()).ReturnsAsync(It.IsAny<PokemonFormViewModel>());

            _loggerMock = new Mock<ILoggerAdapter<PokemonFormController>>();

            _pokemonFormController = new PokemonFormController(_pokedexAppLogicMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task IndexActionIsSuccessfulAndCallsLogic()
        {
            await _pokemonFormController.Index();

            _pokedexAppLogicMock.Verify(plm => plm.GetNewPokemonForm(), Times.Once);

            _loggerMock.VerifyNoOtherCalls();

            _pokedexAppLogicMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task IndexActionWithLogicExceptionLogsError()
        {
            _pokedexAppLogicMock.Setup(plm => plm.GetNewPokemonForm()).ThrowsAsync(new Exception("logic exception"));

            await _pokemonFormController.Index();

            _pokedexAppLogicMock.Verify(plm => plm.GetNewPokemonForm(), Times.Once);

            VerifyLoggerMockLoggedError("logic exception");

            _loggerMock.VerifyNoOtherCalls();

            _pokedexAppLogicMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task IndexWithVMActionIsSuccessfulAndCallsLogic()
        {
            SuccessViewModel successViewModel = DataGenerator.GetViewModel<SuccessViewModel>
                (await _pokemonFormController.Index(new PokemonFormViewModel()));

            Assert.AreEqual("addition", successViewModel.ActionName);

            _pokedexAppLogicMock.Verify(plm => plm.AddPokemon(It.IsAny<PokemonFormViewModel>()), Times.Once);

            _loggerMock.VerifyNoOtherCalls();

            _pokedexAppLogicMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task IndexWithVMActionWithLogicExceptionLogsError()
        {
            _pokedexAppLogicMock.Setup(plm => plm.AddPokemon(It.IsAny< PokemonFormViewModel>()))
                .ThrowsAsync(new Exception("logic exception"));

            ErrorViewModel errorViewModel = DataGenerator.GetViewModel<ErrorViewModel>
                (await _pokemonFormController.Index(new PokemonFormViewModel()));

            _pokedexAppLogicMock.Verify(plm => plm.AddPokemon(It.IsAny<PokemonFormViewModel>()), Times.Once);

            VerifyLoggerMockLoggedError("logic exception");

            _loggerMock.VerifyNoOtherCalls();

            _pokedexAppLogicMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void ErrorActionLogsError()
        {
            Exception error = new Exception("some error");

            IActionResult result = _pokemonFormController.Error(error);

            ErrorViewModel errorViewModel = DataGenerator.GetViewModel<ErrorViewModel>(result);

            Assert.AreEqual("some error", errorViewModel.Message);

            VerifyLoggerMockLoggedError("some error");

            _loggerMock.VerifyNoOtherCalls();

            _pokedexAppLogicMock.VerifyNoOtherCalls();
        }

        private void VerifyLoggerMockLoggedError(string error)
        {
            _loggerMock.Verify(lm => lm.LogError(It.IsAny<Exception>(), error), Times.Once);
        }
    }
}