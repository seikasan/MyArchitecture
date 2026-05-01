using System;
using System.Collections.Generic;

namespace MyArchitecture.Core
{
    public sealed class DisposableBag : IDisposable
    {
        private readonly List<IDisposable> _disposables = new();

        private bool _disposed;

        public bool IsDisposed => _disposed;

        public void Add(IDisposable disposable)
        {
            if (disposable is null)
            {
                throw new ArgumentNullException(nameof(disposable));
            }

            if (_disposed)
            {
                disposable.Dispose();
                return;
            }

            _disposables.Add(disposable);
        }

        public T Track<T>(T disposable)
            where T : IDisposable
        {
            Add(disposable);
            return disposable;
        }

        public void Clear()
        {
            for (int i = _disposables.Count - 1; i >= 0; i--)
            {
                _disposables[i].Dispose();
            }

            _disposables.Clear();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Clear();
        }
    }
}