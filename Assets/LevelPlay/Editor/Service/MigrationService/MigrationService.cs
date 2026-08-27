using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Unity.Services.LevelPlay.Editor
{
    class MigrationService : IMigrationService
    {
        const string k_ProjectSettings = "ProjectSettings";
        const string k_PackageFolder = "Packages";

        readonly List<IMigrator> m_Migrators;

        static readonly string s_PackageSettingsFolder = Path.Combine(k_ProjectSettings, k_PackageFolder, Constants.k_PackageName);
        static readonly string s_SettingsJson = Path.Combine(s_PackageSettingsFolder, "Settings.json");
        static readonly string s_AdMobConfigurationsJson = Path.Combine(s_PackageSettingsFolder, "AdMobConfigurations.json");

        internal MigrationService(IFileService fileService)
        {
#if LEVELPLAY_DEPENDENCIES_INSTALLED
            m_Migrators = new List<IMigrator>
            {
                new LevelPlaySettingsMigrator(1,
                    new JsonSettingsStorage<LevelPlaySettingsData>(s_SettingsJson, new LevelPlayLogger()),
                    fileService),
                new LevelPlayAdMobConfigurationsMigrator(1,
                    new JsonSettingsStorage<LevelPlayAdMobConfigurationsData>(s_AdMobConfigurationsJson, new LevelPlayLogger()),
                    fileService)
            };
#else
            m_Migrators = new List<IMigrator>();
#endif
        }

        public void Migrate()
        {
            AssetDatabase.Refresh();

            foreach (var migrator in m_Migrators)
                if (migrator.ShouldMigrate())
                    migrator.Migrate();
        }
    }
}
