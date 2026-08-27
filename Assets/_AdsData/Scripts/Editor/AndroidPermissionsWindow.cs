using UnityEditor;
using UnityEngine;
using System.IO;
using System.Xml;

public class AndroidPermissionsWindow : EditorWindow
{
    private static readonly string ManifestPath = "Assets/Plugins/Android/AndroidManifest.xml";

    private readonly string[] requiredPermissions = new string[]
    {
        "android.permission.INTERNET",
        "android.permission.ACCESS_NETWORK_STATE",
        "com.google.android.gms.permission.AD_ID",
        "android.permission.READ_PHONE_STATE",
        "android.permission.ACCESS_WIFI_STATE",
        "android.permission.VIBRATE",
        "android.permission.POST_NOTIFICATIONS"
    };

    [MenuItem("Finz/Android Permissions")]
    public static void ShowWindow()
    {
        AndroidPermissionsWindow window = GetWindow<AndroidPermissionsWindow>("Permissions");
        window.minSize = new Vector2(500, 250);
        window.maxSize = new Vector2(500, 250);
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("🔒 Android Permissions Validator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (!File.Exists(ManifestPath))
        {
            if (GUILayout.Button("Create AndroidManifest.xml"))
            {
                CreateDefaultManifest();
                AssetDatabase.Refresh();
            }

            EditorGUILayout.HelpBox("AndroidManifest.xml not found. Click above to create a new one.", MessageType.Warning);
            return;
        }

        XmlDocument manifestDoc = new XmlDocument();
        manifestDoc.Load(ManifestPath);
        XmlNode manifestNode = manifestDoc.SelectSingleNode("/manifest");

        EditorGUILayout.BeginVertical(GUI.skin.box);
        foreach (string permission in requiredPermissions)
        {
            bool hasPermission = manifestDoc.OuterXml.Contains(permission);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("• " + permission, GUILayout.Width(370));

            if (hasPermission)
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Remove", GUILayout.Width(90)))
                {
                    RemovePermission(manifestDoc, manifestNode, permission);
                }
            }
            else
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Add", GUILayout.Width(90)))
                {
                    AddPermission(manifestDoc, manifestNode, permission);
                }
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    private void CreateDefaultManifest()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ManifestPath));

        string defaultManifest = @"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android""
    package=""com.finzgame.placeholder"">
    <application />
</manifest>";

        File.WriteAllText(ManifestPath, defaultManifest);
        Debug.Log("✅ AndroidManifest.xml created at: " + ManifestPath);
    }

    private void AddPermission(XmlDocument manifestDoc, XmlNode manifestNode, string permission)
    {
        XmlElement permissionElement = manifestDoc.CreateElement("uses-permission");
        permissionElement.SetAttribute("name", "http://schemas.android.com/apk/res/android", permission);
        manifestNode.AppendChild(permissionElement);
        SaveManifest(manifestDoc);
    }

    private void RemovePermission(XmlDocument manifestDoc, XmlNode manifestNode, string permission)
    {
        XmlNodeList usesPermissions = manifestNode.SelectNodes("uses-permission");

        foreach (XmlNode node in usesPermissions)
        {
            XmlAttribute nameAttr = node.Attributes["android:name"];
            if (nameAttr != null && nameAttr.Value == permission)
            {
                manifestNode.RemoveChild(node);
                break;
            }
        }

        SaveManifest(manifestDoc);
    }

    private void SaveManifest(XmlDocument manifestDoc)
    {
        manifestDoc.Save(ManifestPath);
        AssetDatabase.Refresh();
        Debug.Log("✅ AndroidManifest.xml updated.");
    }
}
