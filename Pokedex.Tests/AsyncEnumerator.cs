using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pokedex.Tests
{
    public class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public AsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }
        public T Current
        {
            get { return _inner.Current; }
        }

        ValueTask<bool> IAsyncEnumerator<T>.MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            _inner.Dispose();

            return new ValueTask();
        }
    }
}