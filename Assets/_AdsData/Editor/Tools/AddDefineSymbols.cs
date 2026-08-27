using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Adds the given define symbols to PlayerSettings define symbols.
/// Just add your own define symbols to the Symbols property at the below.
/// </summary>
public static class AddDefineSymbols
{
    /// <summary>
    /// Symbols that will be added to the editor
    /// </summary>
    private static List<string> Symbols = new List<string>();

    /// <summary>
    /// Add define symbols as soon as Unity gets done compiling.
    /// </summary>
    public static void Add(string define)
    {
        Symbols.Add(define);
        string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> allDefines = definesString.Split(';').ToList();
        allDefines.AddRange(Symbols.Except(allDefines));
        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup,
            string.Join(";", allDefines.ToArray()));

        EditorUtility.DisplayDialog(define, "Tag added \nVerify all the tags from project settings as well", "OK");
    }

    /// <summary>
    /// Clear all symbols.
    /// </summary>
    private static List<string> tempDefines = new List<string>();
    public static void Clear(string define)
    {
        tempDefines.Clear();
        string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> allDefines = definesString.Split(';').ToList();

        foreach (var item in allDefines)
        {
            //            Debug.Log(item.ToString());
            if (!item.Equals(define))
            {
                tempDefines.Add(item.ToString());
            }
        }

        allDefines.Clear();
        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup,
            string.Join(";", tempDefines.ToArray()));


        EditorUtility.DisplayDialog(define, "Tag removed \nDon't forget to remove its SDK from Assets/Packages folder", "OK");
    }

    public static bool CheckSymbol(string define)
    {
        bool isAvailable = false;
        string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> allDefines = definesString.Split(';').ToList();

        foreach (var item in allDefines)
        {
            if (item.Equals(define))
            {
                isAvailable = true;
            }
        }

        return isAvailable;
    }

}