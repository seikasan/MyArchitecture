using System;

namespace MyArchitecture.Core
{
    public static class DisposableBagExtensions
    {
        public static T AddTo<T>(this T disposable, DisposableBag bag)
            where T : IDisposable
        {
            if (bag == null)
            {
                throw new ArgumentNullException(nameof(bag));
            }

            bag.Add(disposable);
            return disposable;
        }
    }
}
