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
        private Mock<ILoggerAdapter<PokemonFormController>> _loggerMock;
        private Mock<IPokedexAppLogic> _pokedexAppLogicMock;

        private PokemonFormController _pokemonFormController;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            _loggerMock = new Mock<ILoggerAdapter<PokemonFormController>>();
            
            _pokedexAppLogicMock = new Mock<IPokedexAppLogic>();

            _pokedexAppLogicMock.Setup(plm => plm.GetNewPokemonForm())
                .ReturnsAsync(DataGenerator.GeneratePokemonForm());

            _pokemonFormController = new PokemonFormController(_loggerMock.Object, _pokedexAppLogicMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            //Assert
            _loggerMock.VerifyNoOtherCalls();
            _pokedexAppLogicMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task IndexActionIsSuccessfulAndCallsLogic()
        {
            //Act
            await _pokemonFormController.Index();

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.GetNewPokemonForm(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task IndexActionWithLogicExceptionLogsError()
        {
            //Arrange
            _pokedexAppLogicMock.Setup(plm => plm.GetNewPokemonForm())
                .ThrowsAsync(new Exception("logic exception"));

            //Act
            await _pokemonFormController.Index();

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.GetNewPokemonForm(), Times.Once);

            VerifyLoggerMockLoggedError("logic exception");
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task IndexWithVMActionIsSuccessfulAndCallsLogic()
        {
            //Act
            var successViewModel = DataGenerator.GetViewModel<SuccessViewModel>
                (await _pokemonFormController.Index(new PokemonFormViewModel()));

            //Assert
            Assert.AreEqual("addition", successViewModel.ActionName);

            _pokedexAppLogicMock.Verify(plm => plm.AddPokemon(It.IsAny<PokemonFormViewModel>()), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task IndexWithVMActionWithLogicExceptionLogsError()
        {
            //Arrange
            _pokedexAppLogicMock.Setup(plm => plm.AddPokemon(It.IsAny< PokemonFormViewModel>()))
                .ThrowsAsync(new Exception("logic exception"));

            //Act
            var errorViewModel = DataGenerator.GetViewModel<ErrorViewModel>
                (await _pokemonFormController.Index(new PokemonFormViewModel()));

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.AddPokemon(It.IsAny<PokemonFormViewModel>()), Times.Once);

            VerifyLoggerMockLoggedError("logic exception");
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task CaptureActionIsSuccessfulAndCallsLogic()
        {
            //Act
            await _pokemonFormController.Capture(1);

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.GetNewPokemonForm(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task CaptureActionWithLogicExceptionLogsError()
        {
            //Arrange
            _pokedexAppLogicMock.Setup(plm => plm.GetNewPokemonForm())
                .ThrowsAsync(new Exception("logic exception"));

            //Act
            await _pokemonFormController.Capture(1);

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.GetNewPokemonForm(), Times.Once);

            VerifyLoggerMockLoggedError("logic exception");
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public void ErrorActionLogsError()
        {
            //Arrange
            var error = new Exception("some error");

            //Act
            IActionResult result = _pokemonFormController.Error(error);

            var errorViewModel = DataGenerator.GetViewModel<ErrorViewModel>(result);

            //Assert
            Assert.AreEqual("some error", errorViewModel.Message);

            VerifyLoggerMockLoggedError("some error");
        }

        private void VerifyLoggerMockLoggedError(string error)
        {
            _loggerMock.Verify(lm => lm.LogError(It.IsAny<Exception>(), error), Times.Once);
        }
    }
}