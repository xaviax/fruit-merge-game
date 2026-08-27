namespace Unity.Services.LevelPlay.Editor
{
    sealed class MigrateDebugFlagsStep : IMigrationStep<LevelPlayMediationSettings, LevelPlaySettingsData>
    {
        public string Name => "Migration_Step_Debug_Flags";

        public void Apply(LevelPlayMediationSettings source, LevelPlaySettingsData destination)
        {
            if (source != null)
            {
                destination.EnableAdapterDebug = source.EnableAdapterDebug;
                destination.EnableIntegrationHelper = source.EnableIntegrationHelper;
            }
        }
    }
}
