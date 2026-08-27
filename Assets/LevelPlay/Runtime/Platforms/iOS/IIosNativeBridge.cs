#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Unity.Services.LevelPlay
{
    internal interface IIosNativeBridge
    {
        void LPMInitialize(string appKey, string userId,
            Action<string> initSuccessCallback,
            Action<string> initFailureCallback,
            Action<string> impressionSuccessCallback);
        void SetPluginData(string pluginType, string pluginVersion, string pluginFrameworkVersion);
        void LPMSetPauseGame(bool pause);
        bool LPMSetDynamicUserId(string dynamicUserId);
        void LPMValidateIntegration();
        void LPMLaunchTestSuite();
        void LPMSetAdaptersDebug(bool enabled);
        void LPMSetNetworkData(string networkKey, string networkData);
        void LPMSetMetaData(string key, string value);
        void LPMSetMetaDataWithValues(string key, params string[] values);
        void LPMSetConsent(bool consent);
        void LPMSetSegment(string json);
        void LPMSetGDPRConsents(string json);
        void LPMSetCCPA(bool value);
        void LPMSetCOPPA(bool value);
    }

    internal class IosNativeBridge : IIosNativeBridge
    {
        public void LPMInitialize(string appKey, string userId,
            Action<string> initSuccessCallback,
            Action<string> initFailureCallback,
            Action<string> impressionSuccessCallback)
            => LPMInitializeNative(appKey, userId, initSuccessCallback, initFailureCallback, impressionSuccessCallback);

        public void SetPluginData(string pluginType, string pluginVersion, string pluginFrameworkVersion)
            => setPluginDataNative(pluginType, pluginVersion, pluginFrameworkVersion);

        public void LPMSetPauseGame(bool pause)
            => LPMSetPauseGameNative(pause);

        public bool LPMSetDynamicUserId(string dynamicUserId)
            => LPMSetDynamicUserIdNative(dynamicUserId);

        public void LPMValidateIntegration()
            => LPMValidateIntegrationNative();

        public void LPMLaunchTestSuite()
            => LPMLaunchTestSuiteNative();

        public void LPMSetAdaptersDebug(bool enabled)
            => LPMSetAdaptersDebugNative(enabled);

        public void LPMSetNetworkData(string networkKey, string networkData)
            => LPMSetNetworkDataNative(networkKey, networkData);

        public void LPMSetMetaData(string key, string value)
            => LPMSetMetaDataNative(key, value);

        public void LPMSetMetaDataWithValues(string key, params string[] values)
            => LPMSetMetaDataWithValuesNative(key, values);

        public void LPMSetConsent(bool consent)
            => LPMSetConsentNative(consent);

        public void LPMSetSegment(string json)
            => LPMSetSegmentNative(json);

        public void LPMSetGDPRConsents(string json)
            => LPMSetGDPRConsentsNative(json);

        public void LPMSetCCPA(bool value)
            => LPMSetCCPANative(value);

        public void LPMSetCOPPA(bool value)
            => LPMSetCOPPANative(value);

        [DllImport("__Internal", EntryPoint = "LPMInitialize")]
        private static extern void LPMInitializeNative(string appKey, string userId,
            Action<string> initSuccessCallback,
            Action<string> initFailureCallback,
            Action<string> impressionSuccessCallback);

        [DllImport("__Internal", EntryPoint = "setPluginData")]
        private static extern void setPluginDataNative(string pluginType, string pluginVersion, string pluginFrameworkVersion);

        [DllImport("__Internal", EntryPoint = "LPMSetPauseGame")]
        private static extern void LPMSetPauseGameNative(bool pause);

        [DllImport("__Internal", EntryPoint = "LPMSetDynamicUserId")]
        private static extern bool LPMSetDynamicUserIdNative(string dynamicUserId);

        [DllImport("__Internal", EntryPoint = "LPMValidateIntegration")]
        private static extern void LPMValidateIntegrationNative();

        [DllImport("__Internal", EntryPoint = "LPMLaunchTestSuite")]
        private static extern void LPMLaunchTestSuiteNative();

        [DllImport("__Internal", EntryPoint = "LPMSetAdaptersDebug")]
        private static extern void LPMSetAdaptersDebugNative(bool enabled);

        [DllImport("__Internal", EntryPoint = "LPMSetNetworkData")]
        private static extern void LPMSetNetworkDataNative(string networkKey, string networkData);

        [DllImport("__Internal", EntryPoint = "LPMSetMetaData")]
        private static extern void LPMSetMetaDataNative(string key, string value);

        [DllImport("__Internal", EntryPoint = "LPMSetMetaDataWithValues")]
        private static extern void LPMSetMetaDataWithValuesNative(string key, params string[] values);

        [DllImport("__Internal", EntryPoint = "LPMSetConsent")]
        private static extern void LPMSetConsentNative(bool consent);

        [DllImport("__Internal", EntryPoint = "LPMSetSegment")]
        private static extern void LPMSetSegmentNative(string json);

        [DllImport("__Internal", EntryPoint = "LPMSetGDPRConsents")]
        private static extern void LPMSetGDPRConsentsNative(string json);

        [DllImport("__Internal", EntryPoint = "LPMSetCCPA")]
        private static extern void LPMSetCCPANative(bool value);

        [DllImport("__Internal", EntryPoint = "LPMSetCOPPA")]
        private static extern void LPMSetCOPPANative(bool value);
    }
}
#endif
