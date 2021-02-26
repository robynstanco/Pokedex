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
    public class SearchMVControllerFixture
    {
        private SearchController _searchController;

        private Mock<ILoggerAdapter<SearchController>> _loggerMock;
        private Mock<IPokedexAppLogic> _pokedexAppLogicMock;

        [TestInitialize]
        public void Initialize()
        {
            _loggerMock = new Mock<ILoggerAdapter<SearchController>>();

            _pokedexAppLogicMock = new Mock<IPokedexAppLogic>();

            _pokedexAppLogicMock.Setup(plm => plm.GetSearchForm())
                .ReturnsAsync(It.IsAny<SearchViewModel>());

            _searchController = new SearchController(_loggerMock.Object, _pokedexAppLogicMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _loggerMock.VerifyNoOtherCalls();
            _pokedexAppLogicMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task IndexActionIsSuccessfulAndCallsLogic()
        {
            await _searchController.Index();

            _pokedexAppLogicMock.Verify(plm => plm.GetSearchForm(), Times.Once);
        }

        [TestMethod]
        public async Task IndexActionWithLogicExceptionLogsError()
        {
            _pokedexAppLogicMock.Setup(plm => plm.GetSearchForm())
                .ThrowsAsync(new Exception("logic error"));

            await _searchController.Index();

            _pokedexAppLogicMock.Verify(plm => plm.GetSearchForm(), Times.Once);

            VerifyLoggerMockLoggedError("logic error");
        }

        [TestMethod]
        public async Task IndexActionWithViewModelIsSuccessfulAndCallsLogic()
        {
            await _searchController.Index(new SearchViewModel());

            _pokedexAppLogicMock.Verify(plm => plm.Search(It.IsAny<SearchViewModel>()), Times.Once);
        }

        [TestMethod]
        public async Task IndexActionWithViewModelAndLogicExceptionLogsError()
        {
            _pokedexAppLogicMock.Setup(plm => plm.Search(It.IsAny<SearchViewModel>()))
                .ThrowsAsync(new Exception("search error"));

            await _searchController.Index(new SearchViewModel());

            _pokedexAppLogicMock.Verify(plm => plm.Search(It.IsAny<SearchViewModel>()), Times.Once);

            VerifyLoggerMockLoggedError("search error");
        }

        [TestMethod]
        public void ErrorActionLogsError()
        {
            Exception error = new Exception("some error");

            IActionResult result = _searchController.Error(error);

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