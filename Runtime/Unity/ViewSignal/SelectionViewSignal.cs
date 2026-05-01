using System;
using MyArchitecture.Core;
using R3;

namespace MyArchitecture.Unity
{
    public sealed class SelectionViewSignal<T> : IDisposable
    {
        private readonly ViewSignal<T> _selected = new();

        public ViewSignal<T> Selected => _selected;

        public Observable<T> AsObservable()
            => _selected.AsObservable();

        public void Select(T value)
            => _selected.Publish(value);

        public void Dispose()
            => _selected.Dispose();
    }
}
