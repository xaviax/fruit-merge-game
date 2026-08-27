namespace Unity.Services.LevelPlay.Editor
{
    sealed class MigrateAdMobEnablementStep : IMigrationStep<LevelPlayMediationNetworkSettings, LevelPlayAdMobConfigurationsData>
    {
        public string Name => "Migration_Step_AdMob_Enablement";

        public void Apply(LevelPlayMediationNetworkSettings source, LevelPlayAdMobConfigurationsData destination)
        {
            if (source != null) destination.EnableAdMob = source.EnableAdmob;
        }
    }
}
