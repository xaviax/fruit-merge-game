using System.Threading.Tasks;
using UnityEditor;

namespace Unity.Services.LevelPlay.Editor
{
    class InitUsageDetection
    {
        const string k_SessionKey = "LevelPlay_InitUsageDetectionRan";
        static readonly string k_SearchPattern = nameof(LevelPlay) + "." + nameof(LevelPlay.Init);
        const ushort k_TimeoutMs = 5000;

        readonly IApiUsageDetectionService m_DetectionService;
        readonly IEditorAnalyticsService m_AnalyticsService;

        internal InitUsageDetection(IApiUsageDetectionService detectionService, IEditorAnalyticsService analyticsService)
        {
            m_DetectionService = detectionService;
            m_AnalyticsService = analyticsService;
        }

        public async Task Run()
        {
            if (SessionState.GetBool(k_SessionKey, false))
                return;

            SessionState.SetBool(k_SessionKey, true);

            var request = new ApiUsageDetectionRequest(k_SearchPattern, k_TimeoutMs);
            var result = await m_DetectionService.Detect(request);

            switch (result)
            {
                case ApiUsageDetectionResult.Detected:
                    m_AnalyticsService.SendInitUsageDetected();
                    break;
                case ApiUsageDetectionResult.TimedOut:
                    m_AnalyticsService.SendInitUsageDetectionTimeout();
                    break;
                case ApiUsageDetectionResult.NotDetected:
                    m_AnalyticsService.SendInitUsageNotDetected();
                    break;
            }
        }
    }
}
