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
        private Mock<ILoggerAdapter<SearchController>> _loggerMock;
        private Mock<IPokedexAppLogic> _pokedexAppLogicMock;

        private SearchController _searchController;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            _loggerMock = new Mock<ILoggerAdapter<SearchController>>();

            _pokedexAppLogicMock = new Mock<IPokedexAppLogic>();

            _pokedexAppLogicMock.Setup(plm => plm.GetSearchForm())
                .ReturnsAsync(It.IsAny<SearchViewModel>());

            _searchController = new SearchController(_loggerMock.Object, _pokedexAppLogicMock.Object);
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
            await _searchController.Index();

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.GetSearchForm(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task IndexActionWithLogicExceptionLogsError()
        {
            //Arrange
            _pokedexAppLogicMock.Setup(plm => plm.GetSearchForm())
                .ThrowsAsync(new Exception("logic error"));

            //Act
            await _searchController.Index();

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.GetSearchForm(), Times.Once);

            VerifyLoggerMockLoggedError("logic error");
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task IndexActionWithViewModelIsSuccessfulAndCallsLogic()
        {
            //Act
            await _searchController.Index(new SearchViewModel());

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.Search(It.IsAny<SearchViewModel>()), Times.Once);
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task IndexActionWithViewModelAndLogicExceptionLogsError()
        {
            //Arrange
            _pokedexAppLogicMock.Setup(plm => plm.Search(It.IsAny<SearchViewModel>()))
                .ThrowsAsync(new Exception("search error"));

            //Act
            await _searchController.Index(new SearchViewModel());

            //Assert
            _pokedexAppLogicMock.Verify(plm => plm.Search(It.IsAny<SearchViewModel>()), Times.Once);

            VerifyLoggerMockLoggedError("search error");
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public void ErrorActionLogsError()
        {
            //Arrange
            Exception error = new Exception("some error");

            //Act
            IActionResult result = _searchController.Error(error);

            ErrorViewModel errorViewModel = DataGenerator.GetViewModel<ErrorViewModel>(result);

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