
#if USE_ADMOB_OPEN_AD_8_5
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using Sirenix.OdinInspector;
#endif
using UnityEngine;
namespace FinzAdmobOpenAds
{
    public enum AppOpenShowCallStyle
    {
        AppState,
        AppPause,
        AppFocus
    }

    public class AppOpenAdLauncher : GenericSingletonClass<AppOpenAdLauncher>
    {
#if USE_ADMOB_OPEN_AD_8_5
        [BoxGroup("INFO")] public bool showShowOpenAdOnOpen = false;
        [HideIf("@showShowOpenAdOnOpen==false")] [BoxGroup("INFO")] public bool shouldSkipOnFirstSession = false;
        [HideIf("@showShowOpenAdOnOpen==false")] [BoxGroup("INFO")] public bool shouldShowAoaAfterInter = false;

        [BoxGroup("INFO")] public AppOpenShowCallStyle callStyle = AppOpenShowCallStyle.AppState;



#endif

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);

        }
        private void Start()
        {
        
#if USE_ADMOB_OPEN_AD_8_5
            AppOpenAdManager.Instance.showShowOpenAdOnOpen = showShowOpenAdOnOpen;
            if (showShowOpenAdOnOpen&&
                shouldSkipOnFirstSession &&
                AdConstants.getGameSession() == 1) {
                AppOpenAdManager.Instance.showShowOpenAdOnOpen = false;
            }
            AdmobInitlizationManager.admobSdkInitializationAddition += LoadAd;
            if (callStyle == AppOpenShowCallStyle.AppState)
            {
                AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
            }
#endif

        }

        private void OnDestroy()
        {
#if !USE_MAX_OPENADS
#if  USE_ADMOB_OPEN_AD_8_5
            if (callStyle == AppOpenShowCallStyle.AppState)
            {
                AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
            }
#endif
#endif
        }

        public void LoadAd() {
#if  USE_ADMOB_OPEN_AD_8_5
            AppOpenAdManager.Instance.LoadAd();
#endif
        }


#if  USE_ADMOB_OPEN_AD_8_5

        private void OnAppStateChanged(AppState state)
        {
            if (callStyle == AppOpenShowCallStyle.AppState)
            {
                Debug.Log("App State changed to : " + state);

                // if the app is Foregrounded and the ad is available, show it.
                if (state == AppState.Foreground)
                {

                    if (AppOpenAdManager.ConfigResumeApp && !AdConstants.resumeFromAds)
                    {

                        AppOpenAdManager.Instance.ShowAdIfAvailable();

                    }
                    AdConstants.resumeFromAds = false;



                }
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                if (callStyle == AppOpenShowCallStyle.AppFocus)
                {

                    if (AppOpenAdManager.ConfigResumeApp && !AdConstants.resumeFromAds)
                    {

                        AppOpenAdManager.Instance.ShowAdIfAvailable();

                    }
                    AdConstants.resumeFromAds = false;
                }


            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                if (callStyle == AppOpenShowCallStyle.AppPause)
                {

                    if (AppOpenAdManager.ConfigResumeApp && !AdConstants.resumeFromAds)
                    {

                        AppOpenAdManager.Instance.ShowAdIfAvailable();

                    }
                    AdConstants.resumeFromAds = false;
                }


            }
        }

#endif

    }
}