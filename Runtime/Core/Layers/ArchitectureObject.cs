using System;
using System.Threading;

namespace MyArchitecture.Core
{
    public abstract class ArchitectureObject :
        IArchitectureObject,
        IArchitectureDisposable,
        IHasArchitectureLifetime
    {
        private readonly DisposableBag _disposableBag = new();
        private readonly CancellationTokenSource _disposeCancellationTokenSource = new();

        private bool _initialized;
        private bool _bound;
        private bool _postInitialized;
        private bool _disposed;

        protected CancellationToken DisposeCancellationToken =>
            _disposeCancellationTokenSource.Token;

        CancellationToken IHasArchitectureLifetime.DisposeCancellationToken =>
            DisposeCancellationToken;

        protected bool IsInitialized => _initialized;
        protected bool IsBound => _bound;
        protected bool IsPostInitialized => _postInitialized;

        protected bool IsDisposed => _disposed;

        public virtual int InitializeOrder => 0;

        public void Initialize()
        {
            if (_initialized) return;

            try
            {
                OnInitialize();
                _initialized = true;
            }
            catch
            {
                ClearDisposables();
                throw;
            }
        }

        /// <summary>
        /// 自分の内部状態だけ初期化。他オブジェクトの購読や呼び出しはしない。
        /// </summary>
        protected virtual void OnInitialize()
        {
        }

        public void Bind()
        {
            if (_bound) return;

            if (!_initialized)
            {
                throw new InvalidOperationException(
                    $"{GetType().Name} is not initialized.");
            }

            try
            {
                OnBind();
                _bound = true;
            }
            catch
            {
                ClearDisposables();
                throw;
            }
        }

        /// <summary>
        /// 他オブジェクトの Observable / Event を購読する。
        /// </summary>
        protected virtual void OnBind()
        {
        }

        public void PostInitialize()
        {
            if (_postInitialized) return;

            if (!_initialized )
            {
                throw new InvalidOperationException(
                    $"{GetType().Name} is not initialized.");
            }
            if (!_bound)
            {
                throw new InvalidOperationException(
                    $"{GetType().Name} is not bound.");
            }

            try
            {
                OnPostInitialize();
                _postInitialized = true;
            }
            catch
            {
                ClearDisposables();
                throw;
            }
        }

        /// <summary>
        /// 全員の Bind 後に、初回同期や初回 Command を送る。
        /// </summary>
        protected virtual void OnPostInitialize()
        {
        }

        protected T Track<T>(T disposable)
            where T : IDisposable
            => _disposableBag.Track(disposable);

        T IHasArchitectureLifetime.Track<T>(T disposable)
            => Track(disposable);

        protected void ClearDisposables()
            => _disposableBag.Clear();

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _disposeCancellationTokenSource.Cancel();
                OnDispose();
            }
            finally
            {
                _disposableBag.Dispose();
                _disposeCancellationTokenSource.Dispose();
            }
        }

        protected virtual void OnDispose()
        {
        }
    }
}
