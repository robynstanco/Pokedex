using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Common.Interfaces;
using Pokedex.Logging.Interfaces;
using PokedexApp.Controllers;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;
using System.Threading.Tasks;

namespace Pokedex.Tests.Controllers
{
    [TestClass]
    public class PokedexMVControllerFixture
    {
        private Mock<ILoggerAdapter<PokedexController>> _loggerMock;
        private Mock<IPaginationHelper> _paginationHelperMock;
        private Mock<IPokedexAppLogic> _pokedexAppLogicMock;

        private PokedexController _pokedexController;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            _loggerMock = new Mock<ILoggerAdapter<PokedexController>>();

            _paginationHelperMock = new Mock<IPaginationHelper>();

            _pokedexAppLogicMock = new Mock<IPokedexAppLogic>();

            _pokedexController = new PokedexController(_loggerMock.Object, _paginationHelperMock.Object, _pokedexAppLogicMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            //Assert
            _loggerMock.VerifyNoOtherCalls();
            _paginationHelperMock.VerifyNoOtherCalls();
            _pokedexAppLogicMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task IndexActionIsSuccessfulAndCallsLogic()
        {
            //Act
            await _pokedexController.Index(1, 11);

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.GetMyPokedex(), Times.Once);

            _paginationHelperMock.Verify(plm => plm.GetPagedResults<PokemonListingViewModel>(null, 1, 11), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task IndexActionWithLogicExceptionLogsError()
        {
            //Arrange
            _pokedexAppLogicMock.Setup(plm => plm.GetMyPokedex())
                .ThrowsAsync(new Exception("some get exception"));

            //Act
            await _pokedexController.Index(2, 22);

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.GetMyPokedex(), Times.Once);

            _paginationHelperMock.Verify(plm => plm.GetPagedResults<PokemonListingViewModel>(null, 2, 22), Times.Never);

            VerifyLoggerMockLoggedError("some get exception");
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task IndexActionWithPaginationExceptionLogsError()
        {
            //Arrange
            _paginationHelperMock.Setup(plm => plm.GetPagedResults<PokemonListingViewModel>(null, 3, 33))
                .Throws(new Exception("some pagination exception"));
            
            //Act
            await _pokedexController.Index(3, 33);

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.GetMyPokedex(), Times.Once);

            _paginationHelperMock.Verify(plm => plm.GetPagedResults<PokemonListingViewModel>(null, 3, 33), Times.Once);

            VerifyLoggerMockLoggedError("some pagination exception");
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task DetailActionIsSuccessfulAndCallsLogic()
        {
            //Act
            await _pokedexController.Detail(DataGenerator.DefaultGuid);

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.GetMyPokemonById(DataGenerator.DefaultGuid), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task DetailActionWithLogicExceptionLogsError()
        {
            //Arrange
            _pokedexAppLogicMock.Setup(plm => plm.GetMyPokemonById(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("some detail exception"));

            //Act
            await _pokedexController.Detail(DataGenerator.DefaultGuid);

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.GetMyPokemonById(DataGenerator.DefaultGuid), Times.Once);

            VerifyLoggerMockLoggedError("some detail exception");
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task DeleteActionIsSuccessfulAndCallsLogic()
        {
            //Act
            IActionResult result = await _pokedexController.Delete(DataGenerator.DefaultGuid);

            var successViewModel = DataGenerator.GetViewModel<SuccessViewModel>(result);

            //Assert
            Assert.AreEqual("release", successViewModel.ActionName);

            _pokedexAppLogicMock.Verify(plm => plm.DeletePokemonById(DataGenerator.DefaultGuid), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task DeleteActionWithLogicExceptionLogsError()
        {
            //Arrange
            _pokedexAppLogicMock.Setup(plm => plm.DeletePokemonById(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("some delete exception"));

            //Act
            await _pokedexController.Delete(DataGenerator.DefaultGuid);

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.DeletePokemonById(DataGenerator.DefaultGuid), Times.Once);

            VerifyLoggerMockLoggedError("some delete exception");
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task EditIsSuccessfulAndCallsLogic()
        {
            //Act
            IActionResult result = await _pokedexController.Edit(new PokemonDetailViewModel());

            var successViewModel = DataGenerator.GetViewModel<SuccessViewModel>(result);

            //Assert
            Assert.AreEqual("edit", successViewModel.ActionName);

            _pokedexAppLogicMock.Verify(plm => plm.EditPokemon(It.IsAny<PokemonDetailViewModel>()), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task EditWithLogicExceptionLogsError()
        {
            //Arrange
            _pokedexAppLogicMock.Setup(plm => plm.EditPokemon(It.IsAny<PokemonDetailViewModel>()))
                .ThrowsAsync(new Exception("some edit exception"));

            //Act
            await _pokedexController.Edit(new PokemonDetailViewModel());

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.EditPokemon(It.IsAny<PokemonDetailViewModel>()), Times.Once);

            VerifyLoggerMockLoggedError("some edit exception");
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task EditWithGuidIsSuccessfulandCallsLogic()
        {
            //Arrange
            _pokedexAppLogicMock.Setup(plm => plm.GetMyPokemonById(DataGenerator.DefaultGuid))
                .ReturnsAsync(new PokemonDetailViewModel() { IsEditMode = false });

            //Act
            IActionResult result = await _pokedexController.Edit(DataGenerator.DefaultGuid);

            var viewModel = DataGenerator.GetViewModel<PokemonDetailViewModel>(result);

            //Assert
            Assert.IsTrue(viewModel.IsEditMode); 

            _pokedexAppLogicMock.Verify(plm => plm.GetMyPokemonById(DataGenerator.DefaultGuid), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task EditWithInvalidGuidThrowsLogicExceptionAndLogsError()
        {
            //Arrange
            _pokedexAppLogicMock.Setup(plm => plm.GetMyPokemonById(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("some get exception"));

            //Act
            await _pokedexController.Edit(Guid.NewGuid());

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.GetMyPokemonById(It.IsAny<Guid>()), Times.Once);

            VerifyLoggerMockLoggedError("some get exception");
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public void ErrorActionLogsError()
        {
            //Arrange
            var error = new Exception("some error");

            //Act
            IActionResult result = _pokedexController.Error(error);

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