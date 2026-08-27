using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public static class AssemblyManager
{
    // AdMob
    public const string GoogleMobileAdsNameSpace = "GoogleMobileAds";
    /// <summary>
    /// Returns the first class found with the specified class name and (optional) namespace and assembly name.
    /// Returns null if no class found.
    /// </summary>
    /// <returns>The class.</returns>
    /// <param name="className">Class name.</param>
    /// <param name="nameSpace">Optional namespace of the class to find.</param>
    /// <param name="assemblyName">Optional simple name of the assembly.</param>
    public static bool NamespaceExists(string nameSpace, string assemblyName = null)
    {
        Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly asm in assemblies)
        {
            // The assembly must match the given one if any.
            if (!string.IsNullOrEmpty(assemblyName) && !asm.GetName().Name.Equals(assemblyName))
            {
                continue;
            }

            try
            {
                System.Type[] types = asm.GetTypes();
                foreach (System.Type t in types)
                {
                    // The namespace must match the given one if any. Note that the type may not have a namespace at all.
                    // Must be a class and of course class name must match the given one.
                    if (!string.IsNullOrEmpty(t.Namespace) && t.Namespace.Equals(nameSpace))
                    {
                        return true;
                    }
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                foreach (var le in e.LoaderExceptions)
                    Debug.LogException(le);
            }
        }

        return false;
    }


    [InitializeOnLoadMethod]
    public static void CheckForAdmob()
    {

        //if (NamespaceExists(GoogleMobileAdsNameSpace) && !AddDefineSymbols.CheckSymbol("USE_ADMOB"))
        //{
        //    AddDefineSymbols.Add("USE_ADMOB");

        //}
        //else if (!NamespaceExists(GoogleMobileAdsNameSpace) && AddDefineSymbols.CheckSymbol("USE_ADMOB"))
        //{
        //    AddDefineSymbols.Clear("USE_ADMOB");
        //}

    }
}
