using cloudscribe.Pagination.Models;
using System.Collections.Generic;

namespace Pokedex.Common.Interfaces
{
    public interface IPaginationHelper
    {
        /// <summary>
        /// Generic method to create paged result from a given T class collection and page parameters. 
        /// Records are excluded based on pageSize & pageNumber formula.
        /// </summary>
        /// <typeparam name="T">the class of the data</typeparam>
        /// <param name="collection">the enumerable collection of type T</param>
        /// <param name="pageNumber">the current page number to take</param>
        /// <param name="pageSize">the page size to take</param>
        /// <returns>a PagedResult of class T</returns>
        PagedResult<T> GetPagedResults<T>(IEnumerable<T> collection, int pageNumber, int pageSize) where T : class;
    }
}