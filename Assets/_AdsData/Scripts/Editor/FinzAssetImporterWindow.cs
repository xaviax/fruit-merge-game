using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Unity.EditorCoroutines.Editor;
using System.Linq;

public class FinzAssetImporterWindow : EditorWindow
{
    private class AssetInfo
    {
        public string name;
        public string url;
        public string fileName;
        public string versionUrl;
        public string remoteVersion = "…";
        public string category;

        public AssetInfo(string url, string fileName, string versionUrl, string category)
        {
            this.url = url;
            this.fileName = fileName;
            this.versionUrl = versionUrl;
            this.category = category;
        }
    }


    [Serializable]
    private class VersionData
    {
        public string name;
        public string version;
    }


    private List<AssetInfo> assets = new List<AssetInfo>
{
    new AssetInfo(
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/UnityLevelPlay.unitypackage",
        "LevelPlay.unitypackage",
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/UnityLevelPlay.json",
        "LP"
    ),
      new AssetInfo(
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/Adquality.unitypackage",
        "Adquality.unitypackage",
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/Adquality.json",
        "LP"
    ),
    new AssetInfo(
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/GoogleMobileAds.unitypackage",
        "GoogleMobileAds.unitypackage",
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/GoogleMobileAds.json",
        "ADMOB"
    ),
    new AssetInfo(
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/AppLovin.unitypackage",
        "AppLovin.unitypackage",
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/AppLovin.json",
        "MAX"
    ),
     new AssetInfo(
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/Appsflyer.unitypackage",
        "Appsflyer.unitypackage",
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/Appsflyer.json",
        "MMP"
    ),
      new AssetInfo(
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/Facebook.unitypackage",
        "Facebook.unitypackage",
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/Facebook.json",
        "MMP"
    ),
     new AssetInfo(
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/ByteBrewSDK.unitypackage",
        "ByteBrewSDK.unitypackage",
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/ByteBrewSDK.json",
        "ANALYTICS"
    ),
     new AssetInfo(
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/GadsmeSDK.unitypackage",
        "GadsmeSDK.unitypackage",
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/GadsmeSDK.json",
        "CUSTOM"
    ),
    new AssetInfo(
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/Odeeo.unitypackage",
        "Odeeo.unitypackage",
        "https://raw.githubusercontent.com/FinzDevSupport/FinzSdk/main/Odeeo.json",
        "CUSTOM"
    )
};

    private bool isDownloading = false;
    private float downloadProgress = 0f;
    private string status = "";

    [MenuItem("Finz/SDKs")]
    public static void ShowWindow()
    {
        GetWindow<FinzAssetImporterWindow>("Finz Asset Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Asset Importer", EditorStyles.boldLabel);
        GUILayout.Space(5);

        GUI.enabled = !isDownloading;

        var grouped = assets.GroupBy(a => a.category);

        foreach (var group in grouped)
        {
            GUILayout.Space(10);
            GUILayout.Label(group.Key.ToUpper(), EditorStyles.boldLabel);

            foreach (var asset in group)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label($"{asset.name} (v{asset.remoteVersion})", GUILayout.Width(position.width * 0.6f));

                GUI.enabled = !isDownloading;
                if (GUILayout.Button("Download & Import", GUILayout.Width(position.width * 0.3f)))
                {
                    string path = Path.Combine(Application.dataPath, "../Temp/" + asset.fileName);
                    EditorCoroutineUtility.StartCoroutine(DownloadAndImportAsset(asset.url, path), this);
                }
                GUI.enabled = true;

                EditorGUILayout.EndHorizontal();
            }
        }


        GUI.enabled = true;

        if (isDownloading)
        {
            GUILayout.Space(10);
            GUILayout.Label($"Status: {status}");
            EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), Mathf.Clamp01(downloadProgress), $"{(downloadProgress * 100f):F0}%");
        }
    }

    private IEnumerator FetchVersion(AssetInfo asset)
    {
        UnityWebRequest request = UnityWebRequest.Get(asset.versionUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            asset.name = "Unknown";
            asset.remoteVersion = "N/A";
            yield break;
        }

        try
        {
            var data = JsonUtility.FromJson<VersionData>(request.downloadHandler.text);
            asset.name = data.name;
            asset.remoteVersion = data.version;
        }
        catch
        {
            asset.name = "Invalid JSON";
            asset.remoteVersion = "Invalid";
        }

        Repaint();
    }


    private IEnumerator DownloadAndImportAsset(string url, string savePath)
    {
        isDownloading = true;
        status = "Downloading asset...";
        Repaint();

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.downloadHandler = new DownloadHandlerFile(savePath, true);
        var op = www.SendWebRequest();

        while (!op.isDone)
        {
            downloadProgress = op.progress;
            Repaint();
            yield return null;
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[AssetImporter] Download failed: {www.error}");
            isDownloading = false;
            status = "Download failed.";
            Repaint();
            yield break;
        }

        Debug.Log($"[AssetImporter] Downloaded to: {savePath}");

        // Open Unity's import window
        AssetDatabase.ImportPackage(savePath, true);

        // Schedule deletion if canceled
        EditorCoroutineUtility.StartCoroutine(DeleteIfUnused(savePath, 15f), this);

        isDownloading = false;
        status = "";
        downloadProgress = 0f;
        Repaint();
    }

    private IEnumerator DeleteIfUnused(string path, float delay)
    {
        yield return new EditorWaitForSeconds(delay);

        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
                Debug.Log($"[AssetImporter] Cleaned up canceled asset: {path}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AssetImporter] Could not delete file: {ex.Message}");
            }
        }
    }

    private void OnEnable()
    {
        foreach (var asset in assets)
        {
            EditorCoroutineUtility.StartCoroutine(FetchVersion(asset), this);
        }
    }

}
