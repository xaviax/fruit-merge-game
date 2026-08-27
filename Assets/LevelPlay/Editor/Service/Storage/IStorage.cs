namespace Unity.Services.LevelPlay.Editor
{
    interface IStorage<T>
    {
        T Load();
        void Save(T data);
    }
}
