using cloudscribe.Pagination.Models;
using System.Collections.Generic;

namespace Pokedex.Common.Interfaces
{
    public interface IPaginationHelper
    {
        PagedResult<T> GetPagedResults<T>(IEnumerable<T> collection, int pageNumber, int pageSize) where T : class;
    }
}