using System;

namespace MyArchitecture.Core
{
    public sealed class NullArchitectureLogger : IArchitectureLogger
    {
        public void Log(string message)
        {
        }

        public void Warning(string message)
        {
        }

        public void Error(string message)
        {
        }

        public void Exception(Exception exception)
        {
        }
    }
}