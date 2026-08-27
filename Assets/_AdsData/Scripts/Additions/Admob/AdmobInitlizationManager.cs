
#if USE_ADMOB_SIMPLE_BANNER || USE_ADMOB_MREC_BANNER || USE_ADMOB_STATIC_AD || USE_ADMOB_INTERSITITIAL_AD ||USE_ADMOB_REWARD_AD ||USE_ADMOB_OPEN_AD_8_5 ||USE_ADMOB_REWARD_INTERSITITIAL_AD
using System.Collections;
using GoogleMobileAds.Api;
using Sirenix.OdinInspector;
#endif

using UnityEngine;
using System;
public class AdmobInitlizationManager : GenericSingletonClass<AdmobInitlizationManager>
{


    public delegate void admobSDKInitializationAddition();
    public static event admobSDKInitializationAddition admobSdkInitializationAddition;
#if USE_ADMOB_SIMPLE_BANNER || USE_ADMOB_MREC_BANNER || USE_ADMOB_STATIC_AD || USE_ADMOB_INTERSITITIAL_AD ||USE_ADMOB_REWARD_AD ||USE_ADMOB_OPEN_AD_8_5 ||USE_ADMOB_REWARD_INTERSITITIAL_AD
    private IEnumerator Start()
    {
        if (Time.timeScale != 0)
        {
            yield return new WaitForSeconds(1);
        }
        else {
            yield return new WaitForSecondsRealtime(2);
        }
        try
        {
            MobileAds.SetiOSAppPauseOnBackground(true);
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            InitializeGoogleMobileAds();
        }
        catch (Exception e)
        {
            Debug.Log("---Admob Intilization Failed Error "+e.Message);
        }

    }

    private void InitializeGoogleMobileAds()
    {

        MobileAds.Initialize(status => {
            MobileAds.SetiOSAppPauseOnBackground(true);
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            if (admobSdkInitializationAddition != null) {
                admobSdkInitializationAddition.Invoke();
            }
        });
    }
#endif
}
