using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Logging.Interfaces;
using PokedexApp.Controllers;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;

namespace Pokedex.Tests.Controllers
{
    [TestClass]
    public class SearchMVControllerFixture
    {
        private SearchController _searchController;

        private Mock<IPokedexAppLogic> _pokedexAppLogicMock;
        private Mock<ILoggerAdapter<SearchController>> _loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            _pokedexAppLogicMock = new Mock<IPokedexAppLogic>();

            _loggerMock = new Mock<ILoggerAdapter<SearchController>>();
            _pokedexAppLogicMock.Setup(plm => plm.GetSearchForm()).Returns(It.IsAny<SearchViewModel>());

            _searchController = new SearchController(_pokedexAppLogicMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public void IndexActionIsSuccessfulAndCallsLogic()
        {
            _searchController.Index();

            _pokedexAppLogicMock.Verify(plm => plm.GetSearchForm(), Times.Once);
        }

        [TestMethod]
        public void IndexActionWithViewModelIsSuccessfulAndCallsLogic()
        {
            _searchController.Index(new SearchViewModel());

            _pokedexAppLogicMock.Verify(plm => plm.Search(It.IsAny<SearchViewModel>()), Times.Once);
        }

        [TestMethod]
        public void ErrorActionLogsError()
        {
            Exception error = new Exception("some error");

            _searchController.Error(error);

            _loggerMock.Verify(lm => lm.LogError(error, "some error"), Times.Once);
        }
    }
}
