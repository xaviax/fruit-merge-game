namespace Unity.Services.LevelPlay.Editor
{
    sealed class LevelPlayAdMobConfigurationsMigrator : ScriptableObjectMigrator<LevelPlayMediationNetworkSettings, LevelPlayAdMobConfigurationsData>
    {
        public override string Name => "LevelPlay_AdMob_Configurations_Migration";

        protected override string SourcePath => LevelPlayMediationNetworkSettings.MEDIATION_SETTINGS_ASSET_PATH;

        internal LevelPlayAdMobConfigurationsMigrator(
            int version,
            IStorage<LevelPlayAdMobConfigurationsData> storage,
            IFileService fileService) : base(version, storage, fileService)
        {
            AddStep(new MigrateAdMobEnablementStep());
            AddStep(new MigrateAppIdsStep());
        }
    }
}
