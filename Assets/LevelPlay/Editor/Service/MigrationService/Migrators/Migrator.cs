using System.Collections.Generic;

namespace Unity.Services.LevelPlay.Editor
{
    abstract class Migrator<TSource, TDestination> : IMigrator
        where TDestination : IVersionable
    {
        public int Version { get; }
        public abstract string Name { get; }

        protected abstract TSource LoadSource();
        protected abstract TDestination LoadDestination();
        protected abstract void SaveDestination(TDestination destination);

        readonly List<IMigrationStep<TSource, TDestination>> m_Steps = new();

        internal void AddStep(IMigrationStep<TSource, TDestination> step) => m_Steps.Add(step);

        protected Migrator(int version) { Version = version; }

        public virtual bool ShouldMigrate()
        {
            var destination = LoadDestination();
            return Version > destination.Version && m_Steps.Count > 0;
        }

        public virtual void Migrate()
        {
            var source = LoadSource();
            var destination = LoadDestination();

            foreach (var step in m_Steps)
                step.Apply(source, destination);

            destination.Version = Version;

            SaveDestination(destination);
        }
    }
}
