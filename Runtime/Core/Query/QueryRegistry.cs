using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MyArchitecture.Core
{
    public sealed class QueryRegistry
    {
        private readonly HashSet<Type> _queryTypes = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Type type) => _queryTypes.Contains(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<TQuery>()
            where TQuery : class
        {
            _queryTypes.Add(typeof(TQuery));
        }
    }
}