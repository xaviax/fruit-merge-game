namespace Unity.Services.LevelPlay.Editor
{
    sealed class LevelPlaySettingsMigrator : ScriptableObjectMigrator<LevelPlayMediationSettings, LevelPlaySettingsData>
    {
        public override string Name => "LevelPlay_Settings_Migration";

        protected override string SourcePath => LevelPlayMediationSettings.s_LevelPlaySettingsAssetPath;

        internal LevelPlaySettingsMigrator(
            int version,
            IStorage<LevelPlaySettingsData> storage,
            IFileService fileService) : base(version, storage, fileService)
        {
            AddStep(new MigrateAutoInitStep());
            AddStep(new MigrateAppKeysStep());
            AddStep(new MigrateAdIdPermissionStep());
            AddStep(new MigrateDebugFlagsStep());
        }
    }
}
