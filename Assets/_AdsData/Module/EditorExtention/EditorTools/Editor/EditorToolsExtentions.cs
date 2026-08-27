using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Sirenix.Utilities;
using System.Linq;

namespace FinzEditorTools
{
#if USE_EDITOR_TOOLS
    public static class SceneHelper
    {
        static string sceneToOpen;
        static bool Play_Scene;
        public static void StartScene(string sceneName, bool Play_Scene_ = false)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
            Play_Scene = Play_Scene_;
            sceneToOpen = sceneName;
            EditorApplication.update += OnUpdate;
        }
        static void OnUpdate()
        {
            if (sceneToOpen == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
            EditorApplication.update -= OnUpdate;
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                if (Play_Scene)
                {
                    EditorSceneManager.sceneOpened += Just_Play_Scene;
                }
                EditorSceneManager.OpenScene(sceneToOpen);
            }
            sceneToOpen = null;
        }
        private static void Just_Play_Scene(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
            EditorApplication.isPlaying = true;
            EditorSceneManager.sceneOpened -= Just_Play_Scene;
        }
    }
    class TransformClipboard
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public string name;
        public TransformClipboard(Transform transform, bool world = true, bool withScale = false)
        {
            position = world ? transform.position : transform.localPosition;
            rotation = world ? transform.rotation : transform.localRotation;
            name = transform.name;
            if (withScale)
            {
                scale = transform.localScale;
            }
        }
        public void Paste(Transform transform, bool world = true, bool withScale = false)
        {
            if (!world)
            {
                transform.localPosition = position;
                transform.localRotation = rotation;
            }
            else
            {
                transform.position = position;
                transform.rotation = rotation;
            }
            if (withScale)
            {
                transform.localScale = scale;
            }
        }
    }
    public class EditorToolsExtentions : EditorWindow
    {
        private EditorMenuStates CurrentMenustate;
        [MenuItem("Window/General/ToolsExtentions")]
        private static void ShowWindow()
        {
            var window = GetWindow<EditorToolsExtentions>();
            window.titleContent = new GUIContent("ToolsExtentions");
            window.Show();
        }
        enum EditorMenuStates
        {
            ProjectMacros,
            SceneManager,
            Tools
        }
        void OnGUI()
        {
            GUILayout.BeginHorizontal(new GUIStyle() { padding = new RectOffset(20, 20, 10, 10) });
            if (GUILayout.Button("Scene Manager", GUILayout.Width(100)))
            {
                CurrentMenustate = EditorMenuStates.SceneManager;
            }
            GUILayout.EndHorizontal();
            switch (CurrentMenustate)
            {
                case EditorMenuStates.SceneManager:
                    ShowSceneMananger();
                    break;

            }
        }



        void ShowSceneMananger()
        {
            GUILayout.Label("Scene Manager : ", EditorStyles.boldLabel);
            GUILayout.Label($"Current Scene : {EditorSceneManager.GetActiveScene().name}", EditorStyles.helpBox);
            for (int i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
            {
                /*if (i % 2 == 0)
                {
                    GUILayout.BeginHorizontal();
                }*/
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(
                    EditorBuildSettings.scenes[i].path.Split('/')[EditorBuildSettings.scenes[i].path.Split('/').Length - 1], GUILayout.Width(170)))
                {
                    SceneHelper.StartScene(EditorBuildSettings.scenes[i].path);
                }
                if (GUILayout.Button("∆", GUILayout.Width(20)))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(EditorBuildSettings.scenes[i].path));
                }
                if (GUILayout.Button(">>", GUILayout.Width(30)))
                {
                    SceneHelper.StartScene(EditorBuildSettings.scenes[i].path, true);
                }
                GUILayout.EndHorizontal();
                /*if (i % 2 == 1)
                {
                    GUILayout.EndHorizontal();
                }*/
            }
        }
        static class ToolbarStyles
        {
            public static readonly GUIStyle commandButtonStyle;
            static ToolbarStyles()
            {
                commandButtonStyle = new GUIStyle("Command")
                {
                    fontSize = 16,
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Bold
                };
            }
        }
        [InitializeOnLoad]
        public class SceneSwitchLeftButton
        {
            static int adder = 1;
            static SceneSwitchLeftButton()
            {
                ToolbarExtenderFinz.LeftToolbarGUI.Add(OnToolbarGUI);
            }
            static void OnToolbarGUI()
            {
                for (int i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
                {
                    if (i < EditorBuildSettings.scenes.Length)
                    {
                        string tooltip = EditorBuildSettings.scenes[i].path.Split('/')[EditorBuildSettings.scenes[i].path.Split('/').Length - 1];
                        if (GUILayout.Button(new GUIContent(tooltip[0].ToString(), tooltip), ToolbarStyles.commandButtonStyle))
                        {
                            SceneHelper.StartScene(EditorBuildSettings.scenes[i].path);
                        }
                    }
                }
                if (GUILayout.Button(new GUIContent("CP", "Clear Prefs"), ToolbarStyles.commandButtonStyle))
                {
                    PlayerPrefs.DeleteAll();
                }
            }
        }
    }
#endif
}