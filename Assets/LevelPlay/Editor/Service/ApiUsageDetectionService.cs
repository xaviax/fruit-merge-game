using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unity.Services.LevelPlay.Editor
{
    class ApiUsageDetectionService : IApiUsageDetectionService
    {
        readonly IStopwatchService m_StopwatchService;
        readonly IFileService m_FileService;
        readonly IAssemblyService m_AssemblyService;

        internal ApiUsageDetectionService(IStopwatchService stopwatchService, IFileService fileService, IAssemblyService assemblyService)
        {
            m_StopwatchService = stopwatchService;
            m_FileService = fileService;
            m_AssemblyService = assemblyService;
        }

        public async Task<ApiUsageDetectionResult> Detect(ApiUsageDetectionRequest request)
        {
            var sourceFiles = m_AssemblyService.CollectSourceFiles();

            var result = await Task.Run(() => ScanFiles(sourceFiles, request.SearchPattern, request.TimeOutMs));

            return result;
        }

        ApiUsageDetectionResult ScanFiles(List<string> sourceFiles, string searchPattern, ushort timeoutMs)
        {
            m_StopwatchService.StartNew();

            foreach (var filePath in sourceFiles)
            {
                if (m_StopwatchService.ElapsedMilliseconds > timeoutMs)
                {
                    m_StopwatchService.Stop();
                    return ApiUsageDetectionResult.TimedOut;
                }

                try
                {
                    if (m_FileService.FileContainsText(filePath, searchPattern))
                    {
                        m_StopwatchService.Stop();
                        return ApiUsageDetectionResult.Detected;
                    }
                }
                catch
                {
                    m_StopwatchService.Stop();
                }
            }

            m_StopwatchService.Stop();
            return ApiUsageDetectionResult.NotDetected;
        }
    }
}
