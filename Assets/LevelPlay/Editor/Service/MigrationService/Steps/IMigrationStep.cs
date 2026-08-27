namespace Unity.Services.LevelPlay.Editor
{
    interface IMigrationStep<in TSource, in TDestination>
    {
        string Name { get; }
        void Apply(TSource source, TDestination destination);
    }
}
