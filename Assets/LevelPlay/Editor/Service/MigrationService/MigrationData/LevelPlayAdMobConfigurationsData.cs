using System;

namespace Unity.Services.LevelPlay.Editor
{
    [Serializable]
    internal record LevelPlayAdMobConfigurationsData : IVersionable
    {
        public int Version { get; set; }

        public bool EnableAdMob { get; set; }
        public string AndroidAdMobAppId { get; set; }
        public string IOSAdMobAppId { get; set; }
    }
}
