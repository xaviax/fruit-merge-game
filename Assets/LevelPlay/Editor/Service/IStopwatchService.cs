namespace Unity.Services.LevelPlay.Editor
{
    interface IStopwatchService
    {
        void StartNew();
        long ElapsedMilliseconds { get; }
        void Stop();
    }
}
