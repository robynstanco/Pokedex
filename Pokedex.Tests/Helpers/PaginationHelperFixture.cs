using cloudscribe.Pagination.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Common.Helpers;
using Pokedex.Logging.Interfaces;
using PokedexApp.Models;
using System.Collections.Generic;

namespace Pokedex.Tests.Helpers
{
    [TestClass]
    public class PaginationHelperFixture
    {
        private Mock<ILoggerAdapter<PaginationHelper>> _loggerMock;

        private PaginationHelper _paginationHelper;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            _loggerMock = new Mock<ILoggerAdapter<PaginationHelper>>();

            _paginationHelper = new PaginationHelper(_loggerMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            //Assert
            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public void GetPagedStringResultIsSuccessfulAndLogsInformation()
        {
            //Arrange
            IEnumerable<string> stringCollection = new List<string>() { "test" };

            //Act
            PagedResult<string> pagedResult = _paginationHelper.GetPagedResults(stringCollection, 1, 1);

            //Assert
            AssertOnePagedResult(pagedResult);

            VerifyLoggerMockLogsInformation("Mapping PagedResult<System.String>.");
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public void GetPagedViewModelResultIsSuccessfulAndLogsInformation()
        {
            //Arrange
            IEnumerable<PokemonListingViewModel> pokemonListingViewModels = new List<PokemonListingViewModel>() { new PokemonListingViewModel() };

            //Act
            PagedResult<PokemonListingViewModel> pagedResult = _paginationHelper.GetPagedResults(pokemonListingViewModels, 1, 1);

            //Assert
            AssertOnePagedResult(pagedResult);

            VerifyLoggerMockLogsInformation("Mapping PagedResult<PokedexApp.Models.PokemonListingViewModel>.");
        }

        private void VerifyLoggerMockLogsInformation(string info)
        {
            _loggerMock.Verify(lm => lm.LogInformation(info), Times.Once);
        }

        private void AssertOnePagedResult<T>(PagedResult<T> pagedResult) where T : class
        {
            Assert.AreEqual(1, (int)pagedResult.PageNumber);
            Assert.AreEqual(1, pagedResult.PageSize);
            Assert.AreEqual(1, pagedResult.Data.Count);
            Assert.AreEqual(1, (int)pagedResult.TotalItems);
        }
    }
}