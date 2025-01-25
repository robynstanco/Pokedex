using cloudscribe.Pagination.Models;
using Pokedex.Common.Interfaces;
using Pokedex.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.Common.Helpers
{
    /// <summary>
    /// The Pagination Helper.
    /// </summary>
    public class PaginationHelper : IPaginationHelper
    {
        private readonly ILoggerAdapter<PaginationHelper> _logger;
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
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>A PagedResult of class T.</returns>
        public PagedResult<T> GetPagedResults<T>(IEnumerable<T> collection, int pageNumber, int pageSize) where T : class
        {
            collection = collection.ToList(); 

            var totalCount = collection.Count();

            var excludeRecords = (pageNumber * pageSize) - pageSize;

            collection = collection.Skip(excludeRecords).Take(pageSize);

            _logger.LogInformation($"{Constants.Mapping} {nameof(PagedResult<T>)}<{typeof(T)}>.");

            var result = new PagedResult<T>
            {
                Data = collection.ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalCount
            };

            return result;
        }
    }
}