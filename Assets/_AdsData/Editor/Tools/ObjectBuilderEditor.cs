using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ResourcesTextureOptimization))]
public class ObjectBuilderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		ResourcesTextureOptimization myScript = (ResourcesTextureOptimization)target;
		if(GUILayout.Button("Select All Textures"))
		{
			myScript.SelectAllTextures ();
		}

		if(GUILayout.Button("Convert All Textures"))
		{
			myScript.ConvertAllTextures ();
		}
	}
}

[CustomEditor(typeof(FindAdsPluginFolder))]
public class ObjectBuilderEditor2 : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		FindAdsPluginFolder myScript = (FindAdsPluginFolder)target;
		if(GUILayout.Button("Select Folder"))
		{
			myScript.SelectAll ();
		}

		if(GUILayout.Button("Delete Ads Plugin"))
		{
			myScript.DeleteAll();
		}
	}
}