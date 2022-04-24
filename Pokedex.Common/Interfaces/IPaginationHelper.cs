using cloudscribe.Pagination.Models;
using System.Collections.Generic;

namespace Pokedex.Common.Interfaces
{
    /// <summary>
    /// The Pagination Helper Interface.
    /// </summary>
    public interface IPaginationHelper
    {
        /// <summary>
        /// Generic method to create paged result from a given T class collection and page parameters. 
        /// Records are excluded based on pageSize and pageNumber formula.
        /// </summary>
        /// <typeparam name="T">The class of the data.</typeparam>
        /// <param name="collection">The enumerable collection of type T.</param>
        /// <param name="pageNumber">The current page number to take.</param>
        /// <param name="pageSize">The page size to take.</param>
        /// <returns>A PagedResult of class T.</returns>
        PagedResult<T> GetPagedResults<T>(IEnumerable<T> collection, int pageNumber, int pageSize) where T : class;
    }
}