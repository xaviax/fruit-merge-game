using System.Collections.Generic;
using System.IO;
using UnityEditor.Compilation;
using CompilationAssembly = UnityEditor.Compilation.Assembly;

namespace Unity.Services.LevelPlay.Editor
{
    class AssemblyService : IAssemblyService
    {
        public List<string> CollectSourceFiles()
        {
            var files = new List<string>();

            foreach (var assembly in CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies))
            {
                if (!ReferencesLevelPlay(assembly))
                    continue;

                files.AddRange(assembly.sourceFiles);
            }

            return files;
        }

        bool ReferencesLevelPlay(CompilationAssembly assembly)
        {
            if (assembly.name == Constants.k_LevelPlayRuntimeAssemblyName || assembly.name == Constants.k_LevelPlayEditorAssemblyName)
                return false;

            foreach (var assemblyReference in assembly.assemblyReferences)
            {
                if (assemblyReference.name == Constants.k_LevelPlayRuntimeAssemblyName)
                    return true;
            }

            foreach (var refPath in assembly.compiledAssemblyReferences)
            {
                if (Path.GetFileNameWithoutExtension(refPath) == Constants.k_LevelPlayRuntimeAssemblyName)
                    return true;
            }

            return false;
        }
    }
}
