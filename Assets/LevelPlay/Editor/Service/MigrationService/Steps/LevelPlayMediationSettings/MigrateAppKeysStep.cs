namespace Unity.Services.LevelPlay.Editor
{
    sealed class MigrateAppKeysStep : IMigrationStep<LevelPlayMediationSettings, LevelPlaySettingsData>
    {
        public string Name => "Migration_Step_App_Keys";

        public void Apply(LevelPlayMediationSettings source, LevelPlaySettingsData destination)
        {
            if (source != null)
            {
                destination.AndroidAppKey = source.AndroidAppKey;
                destination.IOSAppKey = source.IOSAppKey;
            }
        }
    }
}
