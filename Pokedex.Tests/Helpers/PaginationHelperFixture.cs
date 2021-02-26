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
        private PaginationHelper _paginationHelper;

        private Mock<ILoggerAdapter<PaginationHelper>> _loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            _loggerMock = new Mock<ILoggerAdapter<PaginationHelper>>();

            _paginationHelper = new PaginationHelper(_loggerMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void GetPagedStringResultIsSuccessfulAndLogsInformation()
        {
            IEnumerable<string> stringCollection = new List<string>() { "test" };

            PagedResult<string> pagedResult = _paginationHelper.GetPagedResults(stringCollection, 1, 1);

            AssertOnePagedResult(pagedResult);

            VerifyLoggerMockLogsInformation("Mapping PagedResult<System.String>.");
        }

        [TestMethod]
        public void GetPagedViewModelResultIsSuccessfulAndLogsInformation()
        {
            IEnumerable<PokemonListingViewModel> pokemonListingViewModels = new List<PokemonListingViewModel>() { new PokemonListingViewModel() };

            PagedResult<PokemonListingViewModel> pagedResult = _paginationHelper.GetPagedResults(pokemonListingViewModels, 1, 1);

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