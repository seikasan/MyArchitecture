using System.Runtime.CompilerServices;

namespace MyArchitecture.Core
{
    public static class ArchitectureLoggerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogIf(
            this IArchitectureLogger logger,
            bool condition,
            string message)
        {
            if (condition) logger.Log(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WarningIf(
            this IArchitectureLogger logger,
            bool condition,
            string message)
        {
            if (condition) logger.Warning(message);
        }
    }
}
