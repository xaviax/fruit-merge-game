using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.LevelPlay.Editor
{
    abstract class ScriptableObjectMigrator<TSource, TDestination> : Migrator<TSource, TDestination>
        where TSource : ScriptableObject
        where TDestination : class, IVersionable, new()
    {
        readonly IStorage<TDestination> m_Storage;
        readonly IFileService m_FileService;

        protected abstract string SourcePath { get; }

        protected ScriptableObjectMigrator(
            int version,
            IStorage<TDestination> storage,
            IFileService fileService) : base(version)
        {
            m_Storage = storage;
            m_FileService = fileService;
        }

        public override bool ShouldMigrate() => base.ShouldMigrate() && m_FileService.Exists(SourcePath);

        protected override TSource LoadSource() => AssetDatabase.LoadAssetAtPath<TSource>(SourcePath);
        protected override TDestination LoadDestination() => m_Storage.Load();
        protected override void SaveDestination(TDestination destination) => m_Storage.Save(destination);
    }
}
