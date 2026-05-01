using System;
using MyArchitecture.Core;
using UnityEngine;

namespace MyArchitecture.Unity
{
    public sealed class UnityArchitectureLogger : IArchitectureLogger
    {
        private const string Prefix = "[Architecture] ";

        public void Log(string message)
        {
            Debug.Log(Prefix + message);
        }

        public void Warning(string message)
        {
            Debug.LogWarning(Prefix + message);
        }

        public void Error(string message)
        {
            Debug.LogError(Prefix + message);
        }

        public void Exception(Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}