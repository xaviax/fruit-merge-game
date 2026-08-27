using UnityEngine;
#if USE_ADMOB_SIMPLE_BANNER || USE_ADMOB_MREC_BANNER || USE_ADMOB_STATIC_AD || USE_ADMOB_INTERSITITIAL_AD || USE_ADMOB_REWARD_AD ||USE_ADMOB_REWARD_INTERSITITIAL_AD || USE_ADMOB_OPEN_AD_8_5
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
#endif
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Unity.VisualScripting;

namespace GpAppTrackingg
{
    [AddComponentMenu("Finz Max Plugin/GpAppTrackingg/GoogleAppTracking")]
    public class GoogleAppTracking : MonoBehaviour
    {
        public enum CmpShiftType
        {
            SCENE,PREFAB
        }

#if USE_ADMOB_SIMPLE_BANNER || USE_ADMOB_MREC_BANNER || USE_ADMOB_STATIC_AD || USE_ADMOB_INTERSITITIAL_AD || USE_ADMOB_REWARD_AD || USE_ADMOB_REWARD_INTERSITITIAL_AD || USE_ADMOB_OPEN_AD_8_5
#if UNITY_ANDROID && USE_ADMOB_OPEN_AD_8_5
        [BoxGroup("INFO")] [SerializeField] private bool shouldShowAOAOnCMP = false;
        [HideIf("@shouldShowAOAOnCMP==false")] [BoxGroup("INFO")] public bool shouldSkipOnFirstSession = false;
#endif

        [BoxGroup("INFO")] [SerializeField] [ReadOnly] private bool isInitialized = false;
        [BoxGroup("INFO")] [SerializeField] [ReadOnly] private ConsentForm _consentForm;
        [BoxGroup("INFO")] public bool CanRequestAds => ConsentInformation.CanRequestAds();
        // The Google Mobile Ads Unity plugin needs to be run only once.
        private static bool? _isInitialized;
        [BoxGroup("APP TRACKING")] [SerializeField] private DebugGeography debugGeography = DebugGeography.Disabled;
        [BoxGroup("APP TRACKING")] [SerializeField] private bool tagForUnderAgeOfConsent = false;

        [BoxGroup("EVENT")] [SerializeField] private UnityEvent OnConsentSucess = null;
        [BoxGroup("EVENT")] [SerializeField] private UnityEvent OnConsentFail = null;

        [BoxGroup("AUTO SHIFT")] [SerializeField] public bool shouldAutoShift = false;
        [ShowIf("shouldAutoShift")] [BoxGroup("AUTO SHIFT")] [SerializeField] public float waitForSeconds = 20f;

        [BoxGroup("FLOW")] [SerializeField] public CmpShiftType flowType = CmpShiftType.SCENE;
        [ShowIf("@flowType == CmpShiftType.PREFAB")] [SerializeField] public GameObject flowPrefab = null;

        [BoxGroup("TESTING")]
        [SerializeField]
        private readonly List<string> TEST_DEVICE_IDS = new List<string>
        {
            AdRequest.TestDeviceSimulator,
            // Add your test device IDs (replace with your own device IDs).
#if UNITY_IPHONE
                "96e23e80653bb28980d3f40beb58915c",
#elif UNITY_ANDROID
                "75EF8D155528C04DACBBA6F36F433035"
#endif
        };


        private void Start()
        {
            // On Android, Unity is paused when displaying interstitial or rewarded video.
            // This setting makes iOS behave consistently with Android.
            MobileAds.SetiOSAppPauseOnBackground(true);

            // When true all events raised by GoogleMobileAds will be raised
            // on the Unity main thread. The default value is false.
            // https://developers.google.com/admob/unity/quick-start#raise_ad_events_on_the_unity_main_thread
            MobileAds.RaiseAdEventsOnUnityMainThread = true;

            // Configure your RequestConfiguration with Child Directed Treatment
            // and the Test Device Ids.
            MobileAds.SetRequestConfiguration(new RequestConfiguration
            {
                TestDeviceIds = TEST_DEVICE_IDS
            });

            // If we can request ads, we should initialize the Google Mobile Ads Unity plugin.
            if (CanRequestAds)
            {
                InitializeGoogleMobileAds();
            }

            // Ensures that privacy and consent information is up to date.
            InitializeGoogleMobileAdsConsent();

            if (shouldAutoShift)
            {
                Invoke(nameof(switchToPluginScene), waitForSeconds);
            }
        }

        private void InitializeGoogleMobileAdsConsent()
        {
            Debug.Log("Google Mobile Ads gathering consent.");

            GatherConsent((string error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Failed to gather consent with error: " +
                        error);
                }
                else
                {
                    Debug.Log("Google Mobile Ads consent updated: "
                        + ConsentInformation.ConsentStatus);

                }

                if (CanRequestAds)
                {
                    if (OnConsentSucess != null)
                    {
                        OnConsentSucess.Invoke();
                    }
                    InitializeGoogleMobileAds();

                }
                else
                {
                    if (OnConsentFail != null)
                    {
                        OnConsentFail.Invoke();
                    }
                    switchToPluginScene();
                }

            });
        }

        /// <summary>
        /// Initializes the Google Mobile Ads Unity plugin.
        /// </summary>
        private void InitializeGoogleMobileAds()
        {
            // The Google Mobile Ads Unity plugin needs to be run only once and before loading any ads.
            if (_isInitialized.HasValue)
            {
                switchToPluginScene();
                return;
            }

            _isInitialized = false;

            // Initialize the Google Mobile Ads Unity plugin.
            Debug.Log("Google Mobile Ads Initializing.");
            MobileAds.Initialize((InitializationStatus initstatus) =>
            {
                if (initstatus == null)
                {
                    Debug.LogError("Google Mobile Ads initialization failed.");
                    _isInitialized = null;
                    switchToPluginScene();
                    return;
                }

                // If you use mediation, you can check the status of each adapter.
                var adapterStatusMap = initstatus.getAdapterStatusMap();
                if (adapterStatusMap != null)
                {
                    foreach (var item in adapterStatusMap)
                    {
                        Debug.Log(string.Format("Adapter {0} is {1}",
                            item.Key,
                            item.Value.InitializationState));
                    }
                }

                Debug.Log("Google Mobile Ads initialization complete.");
                _isInitialized = true;
                switchToPluginScene();
            });
        }

        public void GatherConsent(Action<string> onComplete)
        {
            Debug.Log("Gathering consent.");

            var requestParameters = new ConsentRequestParameters
            {
                // False means users are not under age.
                TagForUnderAgeOfConsent = false,
                ConsentDebugSettings = new ConsentDebugSettings
                {
                    // For debugging consent settings by geography.
                    DebugGeography = debugGeography,
                    // https://developers.google.com/admob/unity/test-ads
                    TestDeviceHashedIds = TEST_DEVICE_IDS,
                }
            };

            // Combine the callback with an error popup handler.


            // The Google Mobile Ads SDK provides the User Messaging Platform (Google's
            // IAB Certified consent management platform) as one solution to capture
            // consent for users in GDPR impacted countries. This is an example and
            // you can choose another consent management platform to capture consent.
            ConsentInformation.Update(requestParameters, (FormError updateError) =>
            {
                // Enable the change privacy settings button.
                //UpdatePrivacyButton();

                if (updateError != null)
                {
                    onComplete(updateError.Message);
                    return;
                }

                // Determine the consent-related action to take based on the ConsentStatus.
                if (CanRequestAds)
                {
                    // Consent has already been gathered or not required.
                    // Return control back to the user.
                    onComplete(null);
                    return;
                }

                // Consent not obtained and is required.
                // Load the initial consent request form for the user.
                ConsentForm.LoadAndShowConsentFormIfRequired((FormError showError) =>
                {
                    // UpdatePrivacyButton();
                    if (showError != null)
                    {
                        // Form showing failed.
                        if (onComplete != null)
                        {
                            onComplete(showError.Message);
                        }
                    }
                    // Form showing succeeded.
                    else if (onComplete != null)
                    {
                        onComplete(null);
                    }
                });
            });
        }
        private bool hasCalledSceneShift = false;
        private void switchToPluginScene()
        {
            if (!hasCalledSceneShift)
            {
                hasCalledSceneShift = true;
            }
            else
            {
                return;
            }
#if UNITY_ANDROID && USE_ADMOB_OPEN_AD_8_5


            if (shouldShowAOAOnCMP && AppOpenAdManager.Instance != null)
            {
                AppOpenAdManager.Instance.showShowOpenAdOnOpen = shouldShowAOAOnCMP;
                if (shouldSkipOnFirstSession &&
               AdConstants.getGameSession() == 1)
                {
                    Debug.Log("------- App open canceled due to first session");
                }
                else
                {
                    AppOpenAdManager.Instance.LoadAOA();
                }

            }
#endif
#if USE_IS
            Unity.Services.LevelPlay.LevelPlay.SetConsent(true);
            Unity.Services.LevelPlay.LevelPlay.SetMetaData("do_not_sell", "false");
#endif

            if (flowType == CmpShiftType.SCENE)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else {
                if (flowPrefab != null) {
                    Instantiate(flowPrefab);
                }

            }

        }

#endif
    }
}
