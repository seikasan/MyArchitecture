namespace MyArchitecture.Core
{
    public sealed class ArchitectureSettings
    {
        public bool EnableCommandLog { get; init; }
        public bool EnableQueryLog { get; init; }
        public bool EnableEventPublishLog { get; init; }
        public bool EnableEventSubscribeLog { get; init; }
        public bool EnableInitializationLog { get; init; }

        public static ArchitectureSettings Default()
        {
            return new ArchitectureSettings();
        }

        public static ArchitectureSettings Development()
        {
            return new ArchitectureSettings
            {
                EnableCommandLog = true,
                EnableQueryLog = true,
                EnableEventPublishLog = true,
                EnableEventSubscribeLog = true,
                EnableInitializationLog = true
            };
        }

        public ArchitectureSettings WithCommandLog(bool enable = true)
        {
            return new ArchitectureSettings
            {
                EnableCommandLog = enable,
                EnableQueryLog = EnableQueryLog,
                EnableEventPublishLog = EnableEventPublishLog,
                EnableEventSubscribeLog = EnableEventSubscribeLog,
                EnableInitializationLog = EnableInitializationLog
            };
        }

        public ArchitectureSettings WithQueryLog(bool enable = true)
        {
            return new ArchitectureSettings
            {
                EnableCommandLog = EnableCommandLog,
                EnableQueryLog = enable,
                EnableEventPublishLog = EnableEventPublishLog,
                EnableEventSubscribeLog = EnableEventSubscribeLog,
                EnableInitializationLog = EnableInitializationLog
            };
        }

        public ArchitectureSettings WithEventPublishLog(bool enable = true)
        {
            return new ArchitectureSettings
            {
                EnableCommandLog = EnableCommandLog,
                EnableQueryLog = EnableQueryLog,
                EnableEventPublishLog = enable,
                EnableEventSubscribeLog = EnableEventSubscribeLog,
                EnableInitializationLog = EnableInitializationLog
            };
        }

        public ArchitectureSettings WithEventSubscribeLog(bool enable = true)
        {
            return new ArchitectureSettings
            {
                EnableCommandLog = EnableCommandLog,
                EnableQueryLog = EnableQueryLog,
                EnableEventPublishLog = EnableEventPublishLog,
                EnableEventSubscribeLog = enable,
                EnableInitializationLog = EnableInitializationLog
            };
        }

        public ArchitectureSettings WithInitializationLog(bool enable = true)
        {
            return new ArchitectureSettings
            {
                EnableCommandLog = EnableCommandLog,
                EnableQueryLog = EnableQueryLog,
                EnableEventPublishLog = EnableEventPublishLog,
                EnableEventSubscribeLog = EnableEventSubscribeLog,
                EnableInitializationLog = enable
            };
        }
    }
}
