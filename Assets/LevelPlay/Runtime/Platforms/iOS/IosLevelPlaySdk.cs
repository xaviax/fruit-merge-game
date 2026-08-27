#if UNITY_IOS
using System;
using System.Collections.Generic;

namespace Unity.Services.LevelPlay
{
    sealed class IosLevelPlaySdk : ILevelPlaySdk
    {
        internal static readonly IosLevelPlaySdk Instance = new();
        readonly IIosNativeBridge m_NativeBridge;

        public event Action<LevelPlayConfiguration> OnInitSuccess;
        public event Action<LevelPlayInitError> OnInitFailed;
        public event Action<LevelPlayImpressionData> OnImpressionDataReady;

        IosLevelPlaySdk()
        {
            m_NativeBridge = new IosNativeBridge();
        }

        internal IosLevelPlaySdk(IIosNativeBridge bridge)
        {
            m_NativeBridge = bridge;
        }

        public void Initialize(string appKey, string userId)
        {
            m_NativeBridge.SetPluginData("Unity", LevelPlay.PluginVersion, LevelPlay.UnityVersion);
            m_NativeBridge.LPMInitialize(appKey, userId, OnInitializationSuccess, OnInitializationFailed, OnImpressionSuccess);
        }

        public void SetPauseGame(bool pause)
        {
            m_NativeBridge.LPMSetPauseGame(pause);
        }

        public bool SetDynamicUserId(string dynamicUserId)
        {
            return m_NativeBridge.LPMSetDynamicUserId(dynamicUserId);
        }

        public void ValidateIntegration()
        {
            m_NativeBridge.LPMValidateIntegration();
        }

        public void LaunchTestSuite()
        {
            m_NativeBridge.LPMLaunchTestSuite();
        }

        public void SetAdaptersDebug(bool enabled)
        {
            m_NativeBridge.LPMSetAdaptersDebug(enabled);
        }

        public void SetNetworkData(string networkKey, string networkData)
        {
            m_NativeBridge.LPMSetNetworkData(networkKey, networkData);
        }

        public void SetMetaData(string key, string value)
        {
            m_NativeBridge.LPMSetMetaData(key, value);
        }

        public void SetMetaData(string key, params string[] values)
        {
            m_NativeBridge.LPMSetMetaDataWithValues(key, values);
        }

        public void SetConsent(bool consent)
        {
            m_NativeBridge.LPMSetConsent(consent);
        }

        public void SetSegment(LevelPlaySegment segment)
        {
            var dict = segment.GetSegmentAsDictionary();
            var json = LevelPlayJson.Serialize(dict);
            m_NativeBridge.LPMSetSegment(json);
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
                m_NativeBridge.LPMSetGDPRConsents(json);
            }
            catch (Exception e)
            {
                LevelPlayLogger.LogError($"Failed to set GDPR consents. Exception: {e.Message}");
            }
        }

        public void SetCCPA(bool value)
        {
            m_NativeBridge.LPMSetCCPA(value);
        }

        public void SetCOPPA(bool value)
        {
            m_NativeBridge.LPMSetCOPPA(value);
        }

        delegate void InitSuccessCallback(string configuration);
        delegate void InitFailureCallback(string error);
        delegate void ImpressionSuccessCallback(string impressionData);

        [AOT.MonoPInvokeCallback(typeof(InitSuccessCallback))]
        static void OnInitializationSuccess(string configuration)
        {
            Instance?.OnInitSuccess?.Invoke(new LevelPlayConfiguration(configuration));
        }

        [AOT.MonoPInvokeCallback(typeof(InitFailureCallback))]
        static void OnInitializationFailed(string error)
        {
            Instance?.OnInitFailed?.Invoke(new LevelPlayInitError(error));
        }

        [AOT.MonoPInvokeCallback(typeof(ImpressionSuccessCallback))]
        static void OnImpressionSuccess(string impressionData)
        {
            Instance?.OnImpressionDataReady?.Invoke(new LevelPlayImpressionData(impressionData));
        }
    }
}
#endif
