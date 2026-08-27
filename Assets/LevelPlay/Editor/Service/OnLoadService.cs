using UnityEditor;
using System.Threading.Tasks;
using Unity.Services.LevelPlay.Editor.Analytics;
using UnityEngine;

namespace Unity.Services.LevelPlay.Editor
{
    class OnLoadService : IOnLoadService
    {
        [InitializeOnLoadMethod]
        static async Task InitializeOnLoad()
        {
            var onLoadService = EditorServices.Instance.OnLoadService;
            await onLoadService.OnLoad();
        }

        readonly IMigrationService m_MigrationService;
        readonly ILevelPlaySdkInstaller m_LevelPlaySdkInstaller;
        readonly IApiUsageDetectionService m_ApiUsageDetectionService;
        readonly IEditorAnalyticsService m_EditorAnalyticsService;

        internal OnLoadService(
            ILevelPlaySdkInstaller levelPlaySdkInstaller,
            IMigrationService migrationService,
            IApiUsageDetectionService apiUsageDetectionService,
            IEditorAnalyticsService editorAnalyticsService)
        {
            m_MigrationService = migrationService;
            m_LevelPlaySdkInstaller = levelPlaySdkInstaller;
            m_ApiUsageDetectionService = apiUsageDetectionService;
            m_EditorAnalyticsService = editorAnalyticsService;
        }

        public async Task OnLoad()
        {
            ServicesCoreDependencyInstaller.InstallServicesCoreIfNotFound();
            MobileDependencyResolverInstaller.InstallPlayServicesResolverIfNeeded();
            EnvironmentVariables.BuildManifestPath();

            await m_LevelPlaySdkInstaller.OnLoad();

            m_MigrationService.Migrate();

            EditorSessionTracker.NewSession();
            var initUsageDetection = new InitUsageDetection(m_ApiUsageDetectionService, m_EditorAnalyticsService);
            _ = initUsageDetection.Run();
        }
    }
}
