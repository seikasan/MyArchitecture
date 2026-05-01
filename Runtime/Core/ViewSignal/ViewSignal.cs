using System;
using R3;

namespace MyArchitecture.Core
{
    public sealed class ViewSignal : IDisposable
    {
        private readonly Subject<Unit> _subject = new();

        public Observable<Unit> AsObservable() => _subject;
        public void Publish() => _subject.OnNext(Unit.Default);
        public void Dispose() => _subject.Dispose();
    }

    public sealed class ViewSignal<T> : IDisposable
    {
        private readonly Subject<T> _subject = new();

        public Observable<T> AsObservable() => _subject;
        public void Publish(T value) => _subject.OnNext(value);
        public void Dispose() => _subject.Dispose();
    }
}