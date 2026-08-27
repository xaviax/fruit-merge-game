using System;

namespace Unity.Services.LevelPlay.Editor
{
    [Serializable]
    internal record LevelPlaySettingsData : IVersionable
    {
        public int Version { get; set; }

        public bool EnableAutoInit { get; set; }
        public string AndroidAppKey { get; set; }
        public string IOSAppKey { get; set; }

        public bool DeclareAdIdPermission { get; set; }

        public bool EnableAdapterDebug { get; set; }
        public bool EnableIntegrationHelper { get; set; }
    }
}
