using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Runtime.InteropServices;


#if USE_ODEE
using Odeeo;
using Odeeo.Data;
#endif

public class OdeeManager : MonoBehaviour
{

#if USE_ODEE
    [BoxGroup("KEY")]public string APP_KEY = "0270d75d-58bc-4978-bb21-f96f3034d444";

    [BoxGroup("ICON")] public string iconId = "327811616";
    [BoxGroup("ICON")] public OdeeoSdk.IconPosition iconPositoon = OdeeoSdk.IconPosition.BottomLeft;
    [BoxGroup("ICON")] public int xOffset = 0;
    [BoxGroup("ICON")] public int yOffset = 0;
    [BoxGroup("ICON")] public int adSize = 70;
#endif
    private bool _isInitializationInProgress = false;

    public static OdeeManager Instance;

    public void Awake()
    {

        // Ensure that only one instance of the singleton class exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        Invoke("InitializeOdeeSDK", 2f);
    }

    

    public void InitializeOdeeSDK()
    {
#if USE_ODEE
        if (!OdeeoSdk.IsInitialized() && !_isInitializationInProgress)
        {
            OdeeoSdk.OnInitializationSuccess += OnInitializationFinished;
            OdeeoSdk.OnInitializationFailed += OnInitializationFailed;

            _isInitializationInProgress = true;
        }

        OdeeoSdk.SetLogLevel(OdeeoSdk.LogLevel.Debug);
        OdeeoSdk.Initialize(APP_KEY);
#endif
    }

    public void ShowIconAd()
    {
#if USE_ODEE
        OdeeoAdManager.ShowAd(iconId);
#endif
    }

    public void RemoveAd()
    {
#if USE_ODEE
        if (!OdeeoAdManager.IsAnyAdPlaying())
            return;

        OdeeoAdManager.RemoveAd(iconId);
#endif
    }

    private void OnInitializationFinished()
    {
#if USE_ODEE
        _isInitializationInProgress = false;

      
        OdeeoSdk.OnInitializationSuccess -= OnInitializationFinished;
        OdeeoSdk.OnInitializationFailed -= OnInitializationFailed;

        // Create Icon Ad
        OdeeoAdManager.CreateAudioIconAd(iconId);
        OdeeoAdManager.SetIconPosition(iconId, iconPositoon, xOffset, yOffset);
        OdeeoAdManager.SetIconSize(iconId, adSize);
        SubscribePlacement(iconId);
#endif
    }

    private void OnInitializationFailed(int errorParam, string error)
    {
#if USE_ODEE
        _isInitializationInProgress = false;
#endif
    }

    private void SubscribePlacement(string placementId)
    {
#if USE_ODEE
        if (!OdeeoAdManager.IsPlacementExist(placementId))
            return;

        //Common callbacks
        OdeeoAdManager.AdUnitCallbacks(placementId).OnAvailabilityChanged += AdOnAvailabilityChanged;
        OdeeoAdManager.AdUnitCallbacks(placementId).OnShow += AdOnShow;
        OdeeoAdManager.AdUnitCallbacks(placementId).OnShowFailed += AdOnShowFailed;

        //If rewarded ad type, rewarded callback
        OdeeoAdManager.AdUnitCallbacks(placementId).OnReward += AdOnReward;
#endif
    }


    private void OnApplicationPause(bool pauseStatus)
    {
#if USE_ODEE
        OdeeoSdk.onApplicationPause(pauseStatus);
#endif
    }

    #region Events

#if USE_ODEE
    private void AdOnAvailabilityChanged(bool flag, OdeeoAdData data)
    {
       
    }

    private void AdOnShow()
    {
       
    }

    private void AdOnShowFailed(string placementId, OdeeoAdUnit.ErrorShowReason reason, string description)
    {
    
    }

    private void AdOnReward(float amount)
    {
      
    }


    private void UnsubscribePlacement(string placementId)
    {
        if (!OdeeoAdManager.IsPlacementExist(placementId))
            return;

        //Common callbacks
        OdeeoAdManager.AdUnitCallbacks(placementId).OnAvailabilityChanged -= AdOnAvailabilityChanged;
        OdeeoAdManager.AdUnitCallbacks(placementId).OnShow -= AdOnShow;
        OdeeoAdManager.AdUnitCallbacks(placementId).OnShowFailed -= AdOnShowFailed;

        //If rewarded ad type, rewarded callback
        OdeeoAdManager.AdUnitCallbacks(placementId).OnReward -= AdOnReward;
    }


    private void OnDestroy()
    {

        UnsubscribePlacement(iconId);
        
    }
#endif
#endregion
}
