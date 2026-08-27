using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;


[InitializeOnLoad]
public static class FinzStartUpRoutine
{


    private const string UpdateCheckSessionKey = "FinzPluginUpdateCheckedThisSession";
    private static EditorApplication.CallbackFunction updateCallback;

    static FinzStartUpRoutine()
    {
        // Check if we've already done the update check in this session
        if (SessionState.GetBool(UpdateCheckSessionKey, false))
            return;




        // Mark as checked so it only runs once this session
        SessionState.SetBool(UpdateCheckSessionKey, true);

        Debug.Log("🚀 Project just opened – checking for Finz plugin updates...");

        string updateUrl = "https://drive.google.com/uc?export=download&id=10yFHkY8ki1N0pvkGMQdrZAg8bJN4gTK6";
        UnityWebRequest request = UnityWebRequest.Get(updateUrl);
        request.SendWebRequest();

        updateCallback = () =>
        {
            if (!request.isDone) return;

            EditorApplication.update -= updateCallback;

            if (string.IsNullOrEmpty(request.error))
            {
                try
                {
                    UpdateData data = JsonUtility.FromJson<UpdateData>(request.downloadHandler.text);
                    string currentVersion = "8.1.0"; // Replace with your version string

                    if (data != null && data.version != currentVersion)
                    {
                        bool open = EditorUtility.DisplayDialog(
                            $"Finz Plugin {data.version} Available",
                            $"New Version: {data.version}\n\nChangelog:\n{data.changelog}",
                            "Download",
                            "Later"
                        );

                        if (open)
                        {
                            Application.OpenURL(data.downloadUrl);
                        }
                    }
                    else
                    {
                        Debug.Log("✅ Finz Plugin is up-to-date.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Update check failed: " + ex.Message);
                }
            }
            else
            {
                Debug.LogWarning("Update check failed: " + request.error);
            }
        };

        EditorApplication.update += updateCallback;
    }

    [Serializable]
    public class UpdateData
    {
        public string version;
        public string changelog;
        public string downloadUrl;
    }




}