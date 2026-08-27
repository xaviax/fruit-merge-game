namespace Unity.Services.LevelPlay.Editor
{
    interface IMigrator
    {
        bool ShouldMigrate();
        void Migrate();
    }
}
