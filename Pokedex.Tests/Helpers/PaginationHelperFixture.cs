using cloudscribe.Pagination.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pokedex.Common.Helpers;
using Pokedex.Logging.Interfaces;
using System.Collections.Generic;

namespace Pokedex.Tests.Helpers
{
    [TestClass]
    public class PaginationHelperFixture
    {
        public PaginationHelper _paginationHelper;

        private Mock<ILoggerAdapter<PaginationHelper>> _loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            _loggerMock = new Mock<ILoggerAdapter<PaginationHelper>>();
            _paginationHelper = new PaginationHelper(_loggerMock.Object);
        }


        [TestMethod]
        public void GetPagedResultIsSuccessfulAndLogsInformation()
        {
            IEnumerable<string> stringCollection = new List<string>() { "test" };

            PagedResult<string> pagedResult = _paginationHelper.GetPagedResults(stringCollection, 1, 1);

            Assert.AreEqual(1, (int)pagedResult.PageNumber);
            Assert.AreEqual(1, pagedResult.PageSize);
            Assert.AreEqual(1, pagedResult.Data.Count);
            Assert.AreEqual(1, (int)pagedResult.TotalItems);

            _loggerMock.Verify(lm => lm.LogInformation("Mapping PagedResult<System.String>."), Times.Once);
        }
    }
}
