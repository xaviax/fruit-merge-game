using System.Diagnostics;

namespace Unity.Services.LevelPlay.Editor
{
    class StopwatchService : IStopwatchService
    {
        Stopwatch m_Stopwatch;

        public void StartNew()
        {
            m_Stopwatch = Stopwatch.StartNew();
        }

        public long ElapsedMilliseconds => m_Stopwatch?.ElapsedMilliseconds ?? 0;

        public void Stop()
        {
            m_Stopwatch?.Stop();
        }
    }
}
