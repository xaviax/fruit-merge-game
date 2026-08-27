using System.Threading.Tasks;

namespace Unity.Services.LevelPlay.Editor
{
    internal enum ApiUsageDetectionResult
    {
        Detected,
        NotDetected,
        TimedOut
    }

    readonly struct ApiUsageDetectionRequest
    {
        public string SearchPattern { get; }
        public ushort TimeOutMs { get; }

        public ApiUsageDetectionRequest(string searchPattern, ushort timeOut)
        {
            SearchPattern = searchPattern;
            TimeOutMs = timeOut;
        }
    }

    interface IApiUsageDetectionService
    {
        Task<ApiUsageDetectionResult> Detect(ApiUsageDetectionRequest request);
    }
}
