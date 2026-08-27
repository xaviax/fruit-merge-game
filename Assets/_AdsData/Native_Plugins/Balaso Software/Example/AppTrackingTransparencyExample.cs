using System.Collections;
using System.Collections.Generic;
using Balaso;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;


/// <summary>
/// Example MonoBehaviour class requesting iOS Tracking Authorization
/// </summary>
public class AppTrackingTransparencyExample : MonoBehaviour
{
    private void Awake()
    {

#if UNITY_IOS
        AppTrackingTransparency.RegisterAppForAdNetworkAttribution();
        AppTrackingTransparency.UpdateConversionValue(3);
#endif
    }

    void Start()
    {
#if UNITY_IOS
        AppTrackingTransparency.OnAuthorizationRequestDone += OnAuthorizationRequestDone;

        AppTrackingTransparency.AuthorizationStatus currentStatus = AppTrackingTransparency.TrackingAuthorizationStatus;
        Debug.Log(string.Format("Current authorization status: {0}", currentStatus.ToString()));
        if (currentStatus != AppTrackingTransparency.AuthorizationStatus.AUTHORIZED)
        {
            Debug.Log("Requesting authorization...");
            AppTrackingTransparency.RequestTrackingAuthorization();
        }
        if (currentStatus == AppTrackingTransparency.AuthorizationStatus.NOT_DETERMINED)
        {
            Debug.Log("Authorize not determin.");
            AdConstants.canShowIOSFistAd = false;
        }
        else
        {
            Debug.Log("Authorized.");
            AdConstants.canShowIOSFistAd = true;
        }
#endif
    }

#if UNITY_IOS

    /// <summary>
    /// Callback invoked with the user's decision
    /// </summary>
    /// <param name="status"></param>
    private void OnAuthorizationRequestDone(AppTrackingTransparency.AuthorizationStatus status)
    {
        switch (status)
        {
            case AppTrackingTransparency.AuthorizationStatus.NOT_DETERMINED:
                Debug.Log("AuthorizationStatus: NOT_DETERMINED");
#if USE_FACEBOOK
                AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(true);
#endif
                break;
            case AppTrackingTransparency.AuthorizationStatus.RESTRICTED:
                Debug.Log("AuthorizationStatus: RESTRICTED");
#if USE_FACEBOOK
                AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(true);
#endif
                break;
            case AppTrackingTransparency.AuthorizationStatus.DENIED:
                Debug.Log("AuthorizationStatus: DENIED");
#if USE_FACEBOOK
                AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(false);
#endif
                break;
            case AppTrackingTransparency.AuthorizationStatus.AUTHORIZED:
                Debug.Log("AuthorizationStatus: AUTHORIZED");
#if USE_FACEBOOK
                AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(true);
#endif
                break;
        }
        appTracking(status.ToString());
        // Obtain IDFA
        Debug.Log(string.Format("IDFA: {0}", AppTrackingTransparency.IdentifierForAdvertising()));

    }
#endif

                void appTracking(string condition)
    {
        

    }
}   
