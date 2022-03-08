using cloudscribe.Pagination.Models;
using Pokedex.Common.Interfaces;
using Pokedex.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.Common.Helpers
{
    public class PaginationHelper : IPaginationHelper
    {
        private ILoggerAdapter<PaginationHelper> _logger;
        public PaginationHelper(ILoggerAdapter<PaginationHelper> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Generic method to create paged result from a given T class collection and page parameters. 
        /// Records are excluded based on pageSize and pageNumber formula.
        /// </summary>
        /// <typeparam name="T">The class of the data.</typeparam>
        /// <param name="collection">The enumerable collection of type T.</param>
        /// <param name="pageNumber">The current page number to take.</param>
        /// <param name="pageSize">The page size to take.</param>
        /// <returns>A PagedResult of class T.</returns>
        public PagedResult<T> GetPagedResults<T>(IEnumerable<T> collection, int pageNumber, int pageSize) where T : class
        {
            int totalCount = collection.Count();

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