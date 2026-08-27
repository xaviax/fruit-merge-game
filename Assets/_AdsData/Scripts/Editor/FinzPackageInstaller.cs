using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using Unity.EditorCoroutines.Editor;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class FinzPackageInstaller : EditorWindow
{

    [System.Serializable]
    private class Manifest
    {
        public Dependencies dependencies;
    }

    [System.Serializable]
    private class Dependencies : Dictionary<string, string> { }

    private class PackageInfo
    {
        public string name;
        public string url;

        public PackageInfo(string name, string url)
        {
            this.name = name;
            this.url = url;
        }
    }

    private List<PackageInfo> packages = new List<PackageInfo>
{
    new PackageInfo("com.google.external-dependency-manager", "https://raw.githubusercontent.com/FinzDevSupport/Finz-Packages/main/com.google.external-dependency-manager.tgz"),
    new PackageInfo("com.google.android.appbundle", "https://raw.githubusercontent.com/FinzDevSupport/Finz-Packages/main/com.google.android.appbundle.tgz"),
    new PackageInfo("com.google.play.common", "https://raw.githubusercontent.com/FinzDevSupport/Finz-Packages/main/com.google.play.common.tgz"),
    new PackageInfo("com.google.play.core", "https://raw.githubusercontent.com/FinzDevSupport/Finz-Packages/main/com.google.play.core.tgz"),
    new PackageInfo("com.google.play.review", "https://raw.githubusercontent.com/FinzDevSupport/Finz-Packages/main/com.google.play.review.tgz"),
    new PackageInfo("com.google.firebase.app", "https://raw.githubusercontent.com/FinzDevSupport/Finz-Packages/main/com.google.firebase.app.tgz"),
    new PackageInfo("com.google.firebase.analytics", "https://raw.githubusercontent.com/FinzDevSupport/Finz-Packages/main/com.google.firebase.analytics.tgz"),
    new PackageInfo("com.google.firebase.remote-config", "https://raw.githubusercontent.com/FinzDevSupport/Finz-Packages/main/com.google.firebase.remote-config.tgz"),
    new PackageInfo("com.google.firebase.crashlytics", "https://raw.githubusercontent.com/FinzDevSupport/Finz-Packages/main/com.google.firebase.crashlytics.tgz"),
    new PackageInfo("com.google.firebase.messaging", "https://raw.githubusercontent.com/FinzDevSupport/Finz-Packages/main/com.google.firebase.messaging.tgz")
};
    private HashSet<string> installedPackages = new HashSet<string>();
    private string GetTimestampKey(string packageName) => $"FinzPackage_{packageName}_LastModified";
    private Dictionary<string, bool> hasUpdate = new Dictionary<string, bool>();


    private float downloadProgress = 0f;
    private string status = "";
    private bool isDownloading = false;

    [MenuItem("Finz/Packages Manager")]
    public static void ShowWindow()
    {
        GetWindow<FinzPackageInstaller>("Packages Manager");
    }




    private void OnGUI()
    {
        GUILayout.Label("Packages", EditorStyles.boldLabel);
        GUILayout.Space(5);

        GUI.enabled = !isDownloading; // 🔒 Lock UI while downloading

        foreach (var pkg in packages)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(pkg.name, GUILayout.Width(position.width * 0.6f));

            bool isInstalled = installedPackages.Contains(pkg.name);
            bool updateAvailable = hasUpdate.ContainsKey(pkg.name) && hasUpdate[pkg.name];

          //  bool updateAvailable = hasUpdate.ContainsKey(pkg.name) && hasUpdate[pkg.name];

            if (isDownloading)
            {
                GUILayout.Button("...", GUILayout.Width(position.width * 0.3f)); // disabled
            }
            else if (!isInstalled)
            {
                if (GUILayout.Button("Add", GUILayout.Width(position.width * 0.3f)))
                {
                    EditorCoroutineUtility.StartCoroutine(DownloadAndInstallPackage(pkg.name, pkg.url), this);
                }
            }
            else if (updateAvailable)
            {
                Debug.Log($"[{pkg.name}] Installed: {isInstalled}, Update: {updateAvailable}");
                if (GUILayout.Button("Update", GUILayout.Width(position.width * 0.3f)))
                {
                    EditorCoroutineUtility.StartCoroutine(DownloadAndInstallPackage(pkg.name, pkg.url), this);
                }
            }
            else
            {
                if (GUILayout.Button("Remove", GUILayout.Width(position.width * 0.3f)))
                {
                    RemovePackage(pkg.name);
                }
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
        }


        GUI.enabled = true; // 🔓 Re-enable for remaining UI

        if (isDownloading)
        {
            GUILayout.Space(10);
            GUILayout.Label($"Status: {status}");
            EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), Mathf.Clamp01(downloadProgress), $"{(downloadProgress * 100f):F0}%");
        }
    }

    private IEnumerator CheckForUpdateByHash(PackageInfo pkg, Action<bool> callback)
    {
        string localPath = Path.Combine(Application.dataPath, $"../Packages/Custom/{pkg.name}.tgz");

        if (!File.Exists(localPath))
        {
            callback(false); // not installed, no update check needed
            yield break;
        }

        string localHash = ComputeFileHash(localPath);

        UnityWebRequest request = UnityWebRequest.Get(pkg.url);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"[UpdateCheck] Failed to download {pkg.name}: {request.error}");
            callback(false);
            yield break;
        }

        string remoteHash = ComputeBytesHash(request.downloadHandler.data);

        bool needsUpdate = !string.Equals(localHash, remoteHash, StringComparison.OrdinalIgnoreCase);
        callback(needsUpdate);
    }



    private void RemovePackage(string packageName)
    {
        string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
        if (!File.Exists(manifestPath))
        {
            Debug.LogError("[Installer] manifest.json not found.");
            return;
        }

        string manifestJson = File.ReadAllText(manifestPath);

        // Find the dependencies block
        int depStart = manifestJson.IndexOf("\"dependencies\"");
        int openBrace = manifestJson.IndexOf('{', depStart);
        int closeBrace = manifestJson.IndexOf('}', openBrace);

        string beforeDeps = manifestJson.Substring(0, openBrace + 1);
        string afterDeps = manifestJson.Substring(closeBrace);

        string depsBlock = manifestJson.Substring(openBrace + 1, closeBrace - openBrace - 1);
        var lines = depsBlock.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        string valueToDelete = null;
        var cleanedLines = new List<string>();

        foreach (var line in lines)
        {
            string trimmed = line.Trim().TrimEnd(',');

            if (trimmed.StartsWith($"\"{packageName}\""))
            {
                // Capture value for file deletion if needed
                int colon = trimmed.IndexOf(':');
                if (colon > 0)
                {
                    valueToDelete = trimmed.Substring(colon + 1).Trim().Trim('"');
                }
                continue; // Skip this line
            }

            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                cleanedLines.Add(trimmed);
            }
        }

        // Add commas to all but the last
        for (int i = 0; i < cleanedLines.Count; i++)
        {
            if (i < cleanedLines.Count - 1)
                cleanedLines[i] += ",";
        }

        string newDepsBlock = string.Join("\n    ", cleanedLines);
        string rebuiltManifest = $"{beforeDeps}\n    {newDepsBlock}\n{afterDeps}";

        File.WriteAllText(manifestPath, rebuiltManifest);

        // Delete file if it was a local package
        if (!string.IsNullOrEmpty(valueToDelete) && valueToDelete.StartsWith("file:Custom/"))
        {
            string tgzPath = Path.Combine(Application.dataPath, "../Packages/" + valueToDelete.Replace("file:", ""));
            if (File.Exists(tgzPath))
            {
                File.Delete(tgzPath);
                Debug.Log($"[Installer] Deleted local package file: {tgzPath}");
            }
        }

        AssetDatabase.Refresh();
        UnityEditor.PackageManager.Client.Resolve();
        LoadInstalledPackages();

        Debug.Log($"[Installer] Package '{packageName}' removed cleanly from manifest.");
    }


    private IEnumerator DownloadAndInstallPackage(string packageName, string tgzUrl)
    {
        Debug.Log("[Installer] Starting coroutine...");

        string packagesPath = Path.Combine(Application.dataPath, "../Packages/Custom/");
        string tgzFileName = packageName + ".tgz";
        string tgzPath = Path.Combine(packagesPath, tgzFileName);

        if (!Directory.Exists(packagesPath))
        {
            Debug.Log("[Installer] Creating folder: " + packagesPath);
            Directory.CreateDirectory(packagesPath);
        }

        // ✅ PREVENT FILE LOCK: Delete existing file first
        if (File.Exists(tgzPath))
        {
            try
            {
                File.Delete(tgzPath);
                Debug.Log($"[Installer] Deleted old .tgz file before update: {tgzPath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Installer] Failed to delete old package: {ex.Message}");
                yield break; // stop if we can't delete it
            }
        }

        Debug.Log($"[Installer] Downloading {tgzUrl} to {tgzPath}");

        UnityWebRequest www = UnityWebRequest.Get(tgzUrl);
        www.downloadHandler = new DownloadHandlerFile(tgzPath, true);

        isDownloading = true;
        status = "Downloading...";
        var asyncOp = www.SendWebRequest();

        while (!asyncOp.isDone)
        {
            downloadProgress = asyncOp.progress;
            Repaint();
            yield return null;
        }

        isDownloading = false;
        status = "";
        downloadProgress = 0f;
        Repaint();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[Installer] Download failed: " + www.error);
            status = "Download failed.";
            yield break;
        }

        Debug.Log($"[Installer] Download success: {tgzPath}");

        // ✅ Save new timestamp and hash after successful download
        EditorPrefs.SetString(GetTimestampKey(packageName), DateTime.UtcNow.ToString("o"));
        EditorPrefs.SetString(GetHashKey(packageName), ComputeFileHash(tgzPath));

        UpdateManifest(packageName, tgzPath);
    }


    //private IEnumerator DownloadAndInstallPackage(string packageName, string tgzUrl)
    //{
    //    Debug.Log("[Installer] Starting coroutine...");

    //    string packagesPath = Path.Combine(Application.dataPath, "../Packages/Custom/");
    //    string newHash = ComputeFileHash(packagesPath);
    //    EditorPrefs.SetString(GetHashKey(packageName), newHash);
    //    string tgzFileName = packageName + ".tgz";
    //    string tgzPath = Path.Combine(packagesPath, tgzFileName);

    //    if (!Directory.Exists(packagesPath))
    //    {
    //        Debug.Log("[Installer] Creating folder: " + packagesPath);
    //        Directory.CreateDirectory(packagesPath);
    //    }

    //    Debug.Log($"[Installer] Downloading {tgzUrl} to {tgzPath}");

    //    UnityWebRequest www = UnityWebRequest.Get(tgzUrl);
    //    www.downloadHandler = new DownloadHandlerFile(tgzPath, true);

    //    isDownloading = true;
    //    status = "Downloading...";
    //    var asyncOp = www.SendWebRequest();

    //    while (!asyncOp.isDone)
    //    {
    //        downloadProgress = asyncOp.progress;
    //        Repaint();
    //        yield return null;
    //    }

    //    if (www.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.LogError("[Installer] Download failed: " + www.error);
    //        status = "Download failed.";
    //        yield break;
    //    }

    //    downloadProgress = 1f;
    //    status = "Download complete!";
    //    isDownloading = false;
    //    status = "";
    //    downloadProgress = 0f;
    //    Repaint();

    //    Debug.Log($"[Installer] Download success: {tgzPath}");
    //    EditorPrefs.SetString(GetTimestampKey(packageName), DateTime.UtcNow.ToString("o"));
    //    UpdateManifest(packageName, tgzPath);
    //}


    private void UpdateManifest(string packageName, string tgzPath)
    {
        string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
        if (!File.Exists(manifestPath))
        {
            Debug.LogError("[Installer] manifest.json not found.");
            return;
        }

        string[] lines = File.ReadAllLines(manifestPath);
        string newEntry = $"    \"{packageName}\": \"file:Custom/{Path.GetFileName(tgzPath)}\"";

        List<string> updatedLines = new List<string>();
        bool insideDependencies = false;
        int lastDependencyIndex = -1;

        for (int i = 0; i < lines.Length; i++)
        {
            string trimmedLine = lines[i].Trim();

            // Detect dependencies block start
            if (!insideDependencies && trimmedLine.StartsWith("\"dependencies\"") && trimmedLine.EndsWith("{"))
            {
                insideDependencies = true;
                updatedLines.Add(lines[i]);
                continue;
            }

            // Inside dependencies
            if (insideDependencies)
            {
                // Remove old entry if exists
                if (trimmedLine.StartsWith($"\"{packageName}\""))
                    continue;

                // Detect end of block
                if (trimmedLine == "}")
                {
                    // Fix comma on previous dependency (if missing)
                    if (lastDependencyIndex >= 0 && !updatedLines[lastDependencyIndex].TrimEnd().EndsWith(","))
                        updatedLines[lastDependencyIndex] += ",";

                    // Insert new entry after the last dependency line
                    updatedLines.Insert(lastDependencyIndex + 1, newEntry);

                    // Add the closing brace
                    updatedLines.Add(lines[i]);
                    insideDependencies = false;
                    continue;
                }

                // Track last dependency line index
                lastDependencyIndex = updatedLines.Count;
            }

            updatedLines.Add(lines[i]);
        }

        File.WriteAllLines(manifestPath, updatedLines);
        AssetDatabase.Refresh();
        UnityEditor.PackageManager.Client.Resolve();

        Debug.Log($"[Installer] ✅ '{packageName}' inserted after the last existing dependency.");
    }





    private void LoadInstalledPackages()
    {
        installedPackages.Clear();

        string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
        if (!File.Exists(manifestPath)) return;

        string json = File.ReadAllText(manifestPath);
        int depIndex = json.IndexOf("\"dependencies\"");
        if (depIndex == -1) return;

        int openBrace = json.IndexOf('{', depIndex);
        int closeBrace = json.IndexOf('}', openBrace);

        string deps = json.Substring(openBrace + 1, closeBrace - openBrace - 1);
        string[] lines = deps.Split(',');

        foreach (var line in lines)
        {
            int colonIndex = line.IndexOf(':');
            if (colonIndex > 0)
            {
                string key = line.Substring(0, colonIndex).Trim().Trim('"');
                if (!string.IsNullOrEmpty(key))
                    installedPackages.Add(key);
            }
        }
    }

    private string ComputeFileHash(string path)
    {
        using (var sha1 = System.Security.Cryptography.SHA1.Create())
        using (var stream = File.OpenRead(path))
        {
            var hashBytes = sha1.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }

    private string ComputeBytesHash(byte[] data)
    {
        using (var sha1 = System.Security.Cryptography.SHA1.Create())
        {
            var hashBytes = sha1.ComputeHash(data);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }

    private string GetHashKey(string packageName) => $"FinzPackage_{packageName}_SHA1";


    private void OnEnable()
    {
        LoadInstalledPackages();

        foreach (var pkg in packages)
        {
            if (installedPackages.Contains(pkg.name))
            {
                EditorCoroutineUtility.StartCoroutine(CheckForUpdateByHash(pkg, isUpdate =>
                {
                    hasUpdate[pkg.name] = isUpdate;
                    Repaint();
                }), this);
            }
        }
    }



}
