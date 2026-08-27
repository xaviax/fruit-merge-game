using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class FinzMenuItems
{
	#region Variables

	public static int go_count = 0, components_count = 0, missing_count = 0;

	#endregion



	#region Utils

	[MenuItem("Finz/Utils/Clear PlayerPrefs")]
	private static void ClearPrefs()
	{
		PlayerPrefs.DeleteAll();

		EditorUtility.DisplayDialog("Data Cleared", "PlayerPrefs are cleared now", "OK");
	}

	[MenuItem("Finz/Utils/Multiply W&H by 2 &#2")]
	public static void Multiply_W_H_15()
	{
		MultiplySelected(2f);
	}
	public static void DivideSelected(float by)
	{
		GameObject[] go = Selection.gameObjects;

		for (int i = 0; i < go.Length; i++)
		{
			if (go[i].transform.GetComponent<RectTransform>() == null)
			{
				Debug.Log("This is not a RectTransform", go[i].gameObject);
			}
			else
			{

				RectTransform rect = go[i].GetComponent<RectTransform>();
				UnityEditor.Undo.RecordObject(rect, "Resizing Object");
				rect.sizeDelta = new Vector2(rect.sizeDelta.x / by, rect.sizeDelta.y / by);
			}
		}
	}
	public static void MultiplySelected(float by)
	{
		GameObject[] go = Selection.gameObjects;

		for (int i = 0; i < go.Length; i++)
		{
			if (go[i].transform.GetComponent<RectTransform>() == null)
			{
				Debug.Log("This is not a RectTransform", go[i].gameObject);
			}
			else
			{

				RectTransform rect = go[i].GetComponent<RectTransform>();
				UnityEditor.Undo.RecordObject(rect, "Resizing Object");
				rect.sizeDelta = new Vector2(rect.sizeDelta.x * by, rect.sizeDelta.y * by);
			}
		}

	}
	[MenuItem("Finz/Utils/Find Missing Scripts")]
	private static void FindInSelected()
	{
		GameObject[] go = Selection.gameObjects;
		go_count = 0;
		components_count = 0;
		missing_count = 0;
		foreach (GameObject g in go)
		{
			FindInGO(g);
		}
		Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
	}
	private static void FindInGO(GameObject g)
	{
		go_count++;
		Component[] components = g.GetComponents<Component>();
		for (int i = 0; i < components.Length; i++)
		{
			components_count++;
			if (components[i] == null)
			{
				missing_count++;
				string s = g.name;
				Transform t = g.transform;
				while (t.parent != null)
				{
					s = t.parent.name + "/" + s;
					t = t.parent;
				}
				Debug.Log(s + " has an empty script attached in position: " + i, g);
			}
		}

		foreach (Transform childT in g.transform)
		{
			FindInGO(childT.gameObject);
		}
	}
	[MenuItem("Finz/Utils/Divide Width&Height by 2 &2")]
	public static void Divide_W_H_2()
	{
		DivideSelected(2);
	}

	[MenuItem("Finz/Open 1st Scene &1")]
	public static void openPlugin()
	{
		int i = 0;
		EditorSceneManager.SaveOpenScenes();
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

		EditorSceneManager.OpenScene(EditorBuildSettings.scenes[i].path);
	}


	#endregion

	[MenuItem("Finz/Version")]
	public static void showDIalog()
	{
		EditorUtility.DisplayDialog("Finz Ads Plugin | IRONSOURCE | V8.1.0", "", "OK");
	}

}
