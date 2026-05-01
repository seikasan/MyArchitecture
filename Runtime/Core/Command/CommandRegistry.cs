using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MyArchitecture.Core
{
    public sealed class CommandRegistry
    {
        private readonly HashSet<Type> _commandTypes = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Type type) => _commandTypes.Contains(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<TCommand>()
            where TCommand : class
        {
            _commandTypes.Add(typeof(TCommand));
        }
    }
}