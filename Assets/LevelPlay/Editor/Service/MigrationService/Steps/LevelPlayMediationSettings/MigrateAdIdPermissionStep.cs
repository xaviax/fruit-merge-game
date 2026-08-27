namespace Unity.Services.LevelPlay.Editor
{
    sealed class MigrateAdIdPermissionStep : IMigrationStep<LevelPlayMediationSettings, LevelPlaySettingsData>
    {
        public string Name => "Migration_Step_Ad_Id_Permission";

        public void Apply(LevelPlayMediationSettings source, LevelPlaySettingsData destination)
        {
            if (source != null) destination.DeclareAdIdPermission = source.DeclareAD_IDPermission;
        }
    }
}
