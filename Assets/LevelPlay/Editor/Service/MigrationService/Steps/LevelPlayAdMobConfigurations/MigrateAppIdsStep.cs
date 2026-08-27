namespace Unity.Services.LevelPlay.Editor
{
    sealed class MigrateAppIdsStep : IMigrationStep<LevelPlayMediationNetworkSettings, LevelPlayAdMobConfigurationsData>
    {
        public string Name => "Migration_Step_App_IDs";

        public void Apply(LevelPlayMediationNetworkSettings source, LevelPlayAdMobConfigurationsData destination)
        {
            if (source != null)
            {
                destination.AndroidAdMobAppId = source.AdmobAndroidAppId;
                destination.IOSAdMobAppId = source.AdmobIOSAppId;
            }
        }
    }
}
