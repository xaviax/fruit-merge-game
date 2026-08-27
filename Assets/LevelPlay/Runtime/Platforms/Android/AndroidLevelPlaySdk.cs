#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Services.LevelPlay
{
    sealed class AndroidLevelPlaySdk : ILevelPlaySdk, IUnityLevelPlayInitListener, IUnityLevelPlayImpressionDataListener
    {
        internal static readonly AndroidLevelPlaySdk Instance = new();

        IAndroidNativeBridge m_LevelPlayBridge;
        const string k_LevelPlayBridge = "com.ironsource.unity.androidbridge.LevelPlayBridge";
        readonly IUnityLevelPlayInitListener m_InitListener;
        readonly IUnityLevelPlayImpressionDataListener m_ImpressionListener;

        public event Action<LevelPlayConfiguration> OnInitSuccess;
        public event Action<LevelPlayInitError> OnInitFailed;
        public event Action<LevelPlayImpressionData> OnImpressionDataReady;

        private AndroidLevelPlaySdk()
        {
            m_InitListener = new UnityLevelPlayInitListener(this);
            m_ImpressionListener = new UnityLevelPlayImpressionDataListener(this);
        }

        internal AndroidLevelPlaySdk(
            IUnityLevelPlayInitListener initListener,
            IUnityLevelPlayImpressionDataListener impressionListener,
            IAndroidNativeBridge bridge = null)
        {
            m_InitListener = initListener;
            m_ImpressionListener = impressionListener;
            m_LevelPlayBridge = bridge;
        }

        public void onInitSuccess(string configuration)
        {
            OnInitSuccess?.Invoke(new LevelPlayConfiguration(configuration));
        }

        public void onInitFailed(string error)
        {
            OnInitFailed?.Invoke(new LevelPlayInitError(error));
        }

        public void onImpressionSuccess(string impressionData)
        {
            OnImpressionDataReady?.Invoke(new LevelPlayImpressionData(impressionData));
        }

        IAndroidNativeBridge GetBridge()
        {
            if (m_LevelPlayBridge == null)
            {
                using var pluginClass = new AndroidJavaClass(k_LevelPlayBridge);
                var javaObject = pluginClass.CallStatic<AndroidJavaObject>("getInstance");
                m_LevelPlayBridge = new AndroidNativeBridge(javaObject);
            }
            return m_LevelPlayBridge;
        }

        public void Initialize(string appKey, string userId)
        {
            var bridge = GetBridge();
            bridge.Call("setPluginData", "Unity", LevelPlay.PluginVersion, LevelPlay.UnityVersion);
            bridge.Call("setUnityImpressionDataListener", m_ImpressionListener);
            bridge.Call("initialize", appKey, userId, m_InitListener);
        }

        public bool SetDynamicUserId(string dynamicUserId)
        {
            return GetBridge().Call<bool>("setDynamicUserId", dynamicUserId);
        }

        public void LaunchTestSuite()
        {
            GetBridge().Call("launchTestSuite");
        }

        public void SetAdaptersDebug(bool enabled)
        {
            GetBridge().Call("setAdaptersDebug", enabled);
        }

        public void ValidateIntegration()
        {
            GetBridge().Call("validateIntegration");
        }

        public void SetNetworkData(string networkKey, string networkData)
        {
            GetBridge().Call("setNetworkData", networkKey, networkData);
        }

        public void SetMetaData(string key, string value)
        {
            GetBridge().Call("setMetaData", key, value);
        }

        public void SetMetaData(string key, params string[] values)
        {
            GetBridge().Call("setMetaData", key, values);
        }

        public void SetConsent(bool consent)
        {
            GetBridge().Call("setConsent", consent);
        }

        public void SetSegment(LevelPlaySegment segment)
        {
            var dict = segment.GetSegmentAsDictionary();
            var json = LevelPlayJson.Serialize(dict);
            GetBridge().Call("setSegment", json);
        }

        public void SetPauseGame(bool pause)
        {
            LevelPlayLogger.LogWarning("Unexpected call to SetPauseGame on Android. This method is not implemented for Android.");
        }

        public void SetGDPRConsents(Dictionary<string, bool> networkConsents)
        {
            if (networkConsents == null)
            {
                LevelPlayLogger.LogWarning("SetGDPRConsents called with null networkConsents");
                return;
            }

            try
            {
                var json = LevelPlayJson.Serialize(networkConsents);
                GetBridge().Call("setGDPRConsents", json);
            }
            catch (Exception e)
            {
                LevelPlayLogger.LogError($"Failed to set GDPR consents. Exception: {e.Message}");
            }
        }

        public void SetCCPA(bool value)
        {
            GetBridge().Call("setCCPA", value);
        }

        public void SetCOPPA(bool value)
        {
            GetBridge().Call("setCOPPA", value);
        }
    }
}
#endif
