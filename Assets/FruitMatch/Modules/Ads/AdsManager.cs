using System;
using System.Collections;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    
    public static AdsManager Instance { get; private set; }
    
    
    private bool interstitialRequestInProgress = false;
    private bool interstitialFinished = false;
    
    
    private Action pendingRewardAction = null;
    private Action pendingUnavailableAction = null;
    private Action pendingCancelledAction = null;
    
    
    private bool rewardRequestInProgress = false;
    private bool isPrimaryInstance = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        isPrimaryInstance = true;
        
        //This AdsObject should survive scene changes (even tho there isn't any)
        
        DontDestroyOnLoad(gameObject);
        
    }

    private void OnEnable()
    {

        if (!isPrimaryInstance)
        {
            return;
        }
        
        //subscribing methods to events provided 
        //by the Finz AdController plugin... 

        AdController.gaveRewardMethod += HandleRewardGranted;
        AdController.noRewardedVideoMethod += HandleRewardUnavailable;
        AdController.cancelRewardedAdMethod += HandleRewardCancelled;
        
        
    }

    private void OnDisable()
    {
        if (!isPrimaryInstance)
        {
            return;
        }
        
        
        // unsubscribing methods to events provided 
        //by the Finz AdController plugin on disable
        // to prevent duplicate callbacks and references to destroyed objects

        AdController.gaveRewardMethod -= HandleRewardGranted;
        AdController.noRewardedVideoMethod -= HandleRewardUnavailable;
        AdController.cancelRewardedAdMethod -= HandleRewardCancelled;
        
    }


    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    // Function for Interstitial Ads
    public bool ShowInterstitial()
    {

        if (!TryGetAdController(out AdController controller))
        {
            return false;
        }
        
        controller.ShowAd(
            AdController.AdType.INTERSTITIAL);

        return true;
    }

    // For Rewarded Advertisements 

    public bool ShowRewarded(Action onRewardGranted, Action onUnavailable = null, Action onCancelled = null)
    {

        if (rewardRequestInProgress)
        {
            Debug.LogWarning(
                "A rewarded advertisement is already in progress.",
                this);

            return false;
            
        }

        if (!TryGetAdController(out AdController controller))
        {
            onUnavailable?.Invoke();
            return false;
        }

        if (!controller.IsRewardedAdAvailable())
        {
            Debug.Log("No rewarded advertisement available.");
            onUnavailable?.Invoke();
            return false;
        }
        
        
        // below, we are saving methods that should run 
        // when the plugin later reports the advertisements result...

        pendingRewardAction = onRewardGranted;
        pendingUnavailableAction = onUnavailable;
        pendingCancelledAction = onCancelled;

        rewardRequestInProgress = true;
        
        controller.ShowAd(
            AdController.AdType.REWARDED);

        return true;

    }

    public bool IsRewardedAvailable()
    {
        if (!TryGetAdController(out AdController controller))
        {
            return false;
        }
        
        return controller.IsRewardedAdAvailable();
        
        
        
    }
    
    // Banner advertisements 

    public bool ShowBanner()
    {
        if (!TryGetAdController(out AdController controller))
        {
            return false;
        }
        
        
        controller.ShowBannerAd(AdController.BannerAdTypes.BANNER);
        
        return true;
        
    }

    public bool HideBanner()
    {
        if (!TryGetAdController(out AdController controller))
        {
            return false;
        }
        
        controller.HideBannerAd(
            AdController.BannerAdTypes.BANNER);
        
        return true;
    }

    public bool DestroyBanner()
    {
        if (!TryGetAdController(out AdController controller))
        {
            return false;
        }
        
        controller.DestroyBannerAd(
            AdController.BannerAdTypes.BANNER);
        
        return true;
    }
    
    
    
    // AdController Callbacks

    private void HandleRewardGranted()
    {
        
        if (!rewardRequestInProgress)
        {
            
            Debug.LogWarning("Reward callback received, but no reward was pending.");
            return;
        }
        
        // store the callback locally before clearing
        // the current request 
        
        Action rewardAction = pendingRewardAction;
        
        ClearRewardRequest();
        
        Debug.Log("Reward ad complete.");
        
        rewardAction?.Invoke();
        
        
        
        
        
    }

    private void HandleRewardUnavailable()
    {
        if (!rewardRequestInProgress)
        {
            return;
        }
        
        Action unavailableAction = pendingUnavailableAction;
        
        ClearRewardRequest();
        
        Debug.Log("Reward ad unavailable.");
        
        unavailableAction?.Invoke();
    }


    private void HandleRewardCancelled()
    {
        if (!rewardRequestInProgress)
        {
            return;
        }
        
        Action cancelledAction = pendingCancelledAction;
        
        ClearRewardRequest();
        
        Debug.Log("Reward ad cancelled.");
        
        cancelledAction?.Invoke();
    }

    private void ClearRewardRequest()
    {
        rewardRequestInProgress = false;
        
        pendingRewardAction = null;
        pendingUnavailableAction = null;
        pendingCancelledAction = null;
    }
    
    
    
    
    // Plugin Availability 

    private bool TryGetAdController(out AdController controller)
    {
        controller = AdController.Instance;

        if (controller != null)
        {
            return true;
        }
        
        Debug.LogWarning(
            "AdController is not Initialized.", this);
        
        return false;
    }
    
    // Coroutine For showing Interstitial 

    public IEnumerator ShowInterstitialAndWait()
    {
        if (interstitialRequestInProgress)
        {
            Debug.LogWarning(
                "An interstitial ad is already in progress.",  this);
            
            yield break;
        }
        

        if (!TryGetAdController(out AdController controller))
        {
            Debug.LogWarning(
                "Ad skipped because Adcontroller is unavailable");
            
            yield break;
        }
        
        interstitialRequestInProgress = true;
        interstitialFinished = false;
        
        Debug.Log("Requesting Interstitial Ad");
        
        
        
    }
    
    
    
    
    
    
    
    
}


