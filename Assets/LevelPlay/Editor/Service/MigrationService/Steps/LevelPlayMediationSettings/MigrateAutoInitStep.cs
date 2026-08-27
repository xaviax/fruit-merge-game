namespace Unity.Services.LevelPlay.Editor
{
    sealed class MigrateAutoInitStep : IMigrationStep<LevelPlayMediationSettings, LevelPlaySettingsData>
    {
        public string Name => "Migration_Step_Auto_Init";

        public void Apply(LevelPlayMediationSettings source, LevelPlaySettingsData destination)
        {
            if (source != null) destination.EnableAutoInit = source.EnableIronsourceSDKInitAPI;
        }
    }
}
