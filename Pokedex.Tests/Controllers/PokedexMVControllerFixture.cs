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
        private PokedexController _pokedexController;

        private Mock<ILoggerAdapter<PokedexController>> _loggerMock;
        private Mock<IPaginationHelper> _paginationHelperMock;
        private Mock<IPokedexAppLogic> _pokedexAppLogicMock;

        [TestInitialize]
        public void Initialize()
        {
            _loggerMock = new Mock<ILoggerAdapter<PokedexController>>();

            _paginationHelperMock = new Mock<IPaginationHelper>();

            _pokedexAppLogicMock = new Mock<IPokedexAppLogic>();

            _pokedexController = new PokedexController(_loggerMock.Object, _paginationHelperMock.Object, _pokedexAppLogicMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _loggerMock.VerifyNoOtherCalls();
            _paginationHelperMock.VerifyNoOtherCalls();
            _pokedexAppLogicMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task IndexActionIsSuccessfulAndCallsLogic()
        {
            await _pokedexController.Index(1, 11);

            _pokedexAppLogicMock.Verify(plm => plm.GetMyPokedex(), Times.Once);

            _paginationHelperMock.Verify(plm => plm.GetPagedResults<PokemonListingViewModel>(null, 1, 11), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task IndexActionWithLogicExceptionLogsError()
        {
            _pokedexAppLogicMock.Setup(plm => plm.GetMyPokedex())
                .ThrowsAsync(new Exception("some get exception"));

            await _pokedexController.Index(2, 22);

            _pokedexAppLogicMock.Verify(plm => plm.GetMyPokedex(), Times.Once);

            _paginationHelperMock.Verify(plm => plm.GetPagedResults<PokemonListingViewModel>(null, 2, 22), Times.Never);

            VerifyLoggerMockLoggedError("some get exception");
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task IndexActionWithPaginationExceptionLogsError()
        {
            _paginationHelperMock.Setup(plm => plm.GetPagedResults<PokemonListingViewModel>(null, 3, 33))
                .Throws(new Exception("some pagination exception"));

            await _pokedexController.Index(3, 33);

            _pokedexAppLogicMock.Verify(plm => plm.GetMyPokedex(), Times.Once);

            _paginationHelperMock.Verify(plm => plm.GetPagedResults<PokemonListingViewModel>(null, 3, 33), Times.Once);

            VerifyLoggerMockLoggedError("some pagination exception");
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task DetailActionIsSuccessfulAndCallsLogic()
        {
            await _pokedexController.Detail(DataGenerator.DefaultGuid);

            _pokedexAppLogicMock.Verify(plm => plm.GetMyPokemonById(DataGenerator.DefaultGuid), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task DetailActionWithLogicExceptionLogsError()
        {
            _pokedexAppLogicMock.Setup(plm => plm.GetMyPokemonById(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("some detail exception"));

            await _pokedexController.Detail(DataGenerator.DefaultGuid);

            _pokedexAppLogicMock.Verify(plm => plm.GetMyPokemonById(DataGenerator.DefaultGuid), Times.Once);

            VerifyLoggerMockLoggedError("some detail exception");
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task DeleteActionIsSuccessfulAndCallsLogic()
        {
            IActionResult result = await _pokedexController.Delete(DataGenerator.DefaultGuid);

            SuccessViewModel successViewModel = DataGenerator.GetViewModel<SuccessViewModel>(result);

            Assert.AreEqual("release", successViewModel.ActionName);

            _pokedexAppLogicMock.Verify(plm => plm.DeletePokemonById(DataGenerator.DefaultGuid), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task DeleteActionWithLogicExceptionLogsError()
        {
            _pokedexAppLogicMock.Setup(plm => plm.DeletePokemonById(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("some delete exception"));

            await _pokedexController.Delete(DataGenerator.DefaultGuid);

            _pokedexAppLogicMock.Verify(plm => plm.DeletePokemonById(DataGenerator.DefaultGuid), Times.Once);

            VerifyLoggerMockLoggedError("some delete exception");
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task EditIsSuccessfulAndCallsLogic()
        {
            IActionResult result = await _pokedexController.Edit(new PokemonDetailViewModel());

            SuccessViewModel successViewModel = DataGenerator.GetViewModel<SuccessViewModel>(result);

            Assert.AreEqual("edit", successViewModel.ActionName);

            _pokedexAppLogicMock.Verify(plm => plm.EditPokemon(It.IsAny<PokemonDetailViewModel>()), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task EditWithLogicExceptionLogsError()
        {
            _pokedexAppLogicMock.Setup(plm => plm.EditPokemon(It.IsAny<PokemonDetailViewModel>()))
                .ThrowsAsync(new Exception("some edit exception"));

            await _pokedexController.Edit(new PokemonDetailViewModel());

            _pokedexAppLogicMock.Verify(plm => plm.EditPokemon(It.IsAny<PokemonDetailViewModel>()), Times.Once);

            VerifyLoggerMockLoggedError("some edit exception");
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task EditWithGuidIsSuccessfulandCallsLogic()
        {
            _pokedexAppLogicMock.Setup(plm => plm.GetMyPokemonById(DataGenerator.DefaultGuid))
                .ReturnsAsync(new PokemonDetailViewModel() { IsEditMode = false });

            IActionResult result = await _pokedexController.Edit(DataGenerator.DefaultGuid);

            PokemonDetailViewModel viewModel = DataGenerator.GetViewModel<PokemonDetailViewModel>(result);

            Assert.IsTrue(viewModel.IsEditMode); 

            _pokedexAppLogicMock.Verify(plm => plm.GetMyPokemonById(DataGenerator.DefaultGuid), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task EditWithInvalidGuidThrowsLogicExceptionAndLogsError()
        {
            _pokedexAppLogicMock.Setup(plm => plm.GetMyPokemonById(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("some get exception"));

            await _pokedexController.Edit(Guid.NewGuid());

            _pokedexAppLogicMock.Verify(plm => plm.GetMyPokemonById(It.IsAny<Guid>()), Times.Once);

            VerifyLoggerMockLoggedError("some get exception");
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public void ErrorActionLogsError()
        {
            Exception error = new Exception("some error");

            IActionResult result = _pokedexController.Error(error);

            ErrorViewModel errorViewModel = DataGenerator.GetViewModel<ErrorViewModel>(result);

            Assert.AreEqual("some error", errorViewModel.Message);

            VerifyLoggerMockLoggedError("some error");
        }

        private void VerifyLoggerMockLoggedError(string error)
        {
            _loggerMock.Verify(lm => lm.LogError(It.IsAny<Exception>(), error), Times.Once);
        }
    }
}