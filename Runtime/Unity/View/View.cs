using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyArchitecture.Core;
using UnityEngine;

namespace MyArchitecture.Unity
{
    public abstract class View :
        MonoBehaviour,
        IView
    {
        private readonly DisposableBag _disposableBag = new();

        protected CancellationToken DestroyCancellationToken =>
            this.GetCancellationTokenOnDestroy();

        protected virtual void Awake() => OnAwake();
        protected virtual void Start() => OnStart();

        protected virtual void OnDestroy()
        {
            OnViewDestroy();
            _disposableBag.Dispose();
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnViewDestroy()
        {
        }

        protected T Track<T>(T disposable)
            where T : IDisposable
            => _disposableBag.Track(disposable);

        protected void ClearDisposables()
            => _disposableBag.Clear();
    }
}