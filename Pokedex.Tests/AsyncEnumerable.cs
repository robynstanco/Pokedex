using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Pokedex.Tests
{
    public class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public AsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public AsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            return new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new AsyncQueryProvider<T>(this); }
        }

        public IAsyncEnumerator<T> GetEnumerator()
        {
            return GetAsyncEnumerator(new CancellationToken());
        }
    }
}
