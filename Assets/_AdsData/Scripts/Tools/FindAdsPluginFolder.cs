using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FindAdsPluginFolder : MonoBehaviour {

	#if UNITY_EDITOR
    public Object[] Folders;

    int selectedFolder = -1;


    [ContextMenu("Next Folder")]
    public void NextFolder()
    {
        selectedFolder++;
        selectedFolder = selectedFolder % Folders.Length;

        Selection.activeObject = Folders[selectedFolder];
    }

    [ContextMenu("Select All")]
    public void SelectAll()
    {
        Selection.objects = Folders;
    }

     [ContextMenu("Delete All")]
    public void DeleteAll()
    {

        // DisplayDialog ("Title here", "Your text", "Ok");

        Selection.objects = Folders;
        foreach (Object ob in Selection.objects)
        {
              //  File.Delete;
           // DestroyImmediate((ob as Object),true);
           Debug.Log(AssetDatabase.GetAssetPath(ob));
           FileUtil.DeleteFileOrDirectory(AssetDatabase.GetAssetPath(ob));
           UnityEditor.AssetDatabase.Refresh();
        }
    }

 //   [MenuItem("Finz/Delete Ads Plugin")]
	//public static void DeletePlugin()
	//{
 //       Debug.Log("deleting stuff");
 //       foreach (Object ob in Selection.objects)
 //       {
 //             //  File.Delete;
 //          // DestroyImmediate((ob as Object),true);
 //          Debug.Log(AssetDatabase.GetAssetPath(ob));
 //          FileUtil.DeleteFileOrDirectory(AssetDatabase.GetAssetPath(ob));
 //          UnityEditor.AssetDatabase.Refresh();
 //       }
	//}

	#endif

}