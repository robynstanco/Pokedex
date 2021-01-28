using cloudscribe.Pagination.Models;
using Pokedex.Common.Interfaces;
using Pokedex.Logging.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.Common.Helpers
{
    public class PaginationHelper : IPaginationHelper
    {
        private ILoggerAdapter<PaginationHelper> _logger;
        public PaginationHelper(ILoggerAdapter<PaginationHelper> logger)
        {
            _logger = logger;
        }

        public PagedResult<T> GetPagedResults<T>(IEnumerable<T> collection, int pageNumber, int pageSize) where T : class
        {
            int totalCount = collection.ToList().Count;

            int excludeRecords = (pageNumber * pageSize) - pageSize;

            collection = collection.Skip(excludeRecords).Take(pageSize);

            _logger.LogInformation(Constants.Mapping + " " + nameof(PagedResult<T>) + "<" + typeof(T) + ">.");

            return new PagedResult<T>
            {
                Data = collection.ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalCount
            };
        }
    }
}