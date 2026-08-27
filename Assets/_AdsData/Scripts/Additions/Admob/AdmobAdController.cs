using System.Collections;
using System.Collections.Generic;
#if USE_ADMOB_SIMPLE_BANNER || USE_ADMOB_MREC_BANNER || USE_ADMOB_STATIC_AD || USE_ADMOB_INTERSITITIAL_AD || USE_ADMOB_REWARD_AD ||USE_ADMOB_REWARD_INTERSITITIAL_AD || USE_ADMOB_OPEN_AD_8_5
using GoogleMobileAds.Api;
#endif
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class AdmobAdController : MonoBehaviour
{
    public static AdmobAdController Instance = null;

    [BoxGroup("INFO")] public int lowMemoryLimit = 2048;

#if USE_ADMOB_SIMPLE_BANNER
    [BoxGroup("BANNER")] public bool isAdaptiveBanner = false;
    [BoxGroup("BANNER")] public bool showBannerOnInilization = false;
    [BoxGroup("BANNER")] [ReadOnly] public bool isBannerAdBeingRequested = false;
    [BoxGroup("BANNER")] public AdPosition banner_Position = AdPosition.Top;
    [BoxGroup("BANNER")] private bool IsBannerAdBeingRequested { get => isBannerAdBeingRequested; set => isBannerAdBeingRequested = value; }
    [BoxGroup("BANNER")] public BannerView bannerView = null;
    [BoxGroup("BANNER")] public AdSize Banner_adSize;
#endif
#if USE_ADMOB_MREC_BANNER
    [BoxGroup("MREC")] public BannerView mrecBannerView = null;
    [BoxGroup("MREC")] [ReadOnly] public bool isMrecBannerAdBeingRequested = false;
    [BoxGroup("MREC")] public AdPosition mrec_banner_Position = AdPosition.BottomLeft;
#endif
#if USE_ADMOB_STATIC_AD
    [BoxGroup("STATIC")] public InterstitialAd staticAdmobAd = null;
    [BoxGroup("STATIC")] public bool shouldLoadStaticOnInitilization = true;
    [BoxGroup("STATIC")] [ReadOnly] public bool isLoadingAdmobStaticAd = false;
    [BoxGroup("STATIC")] [ReadOnly] public int staticRetryAttempt = 0;
#endif
#if USE_ADMOB_INTERSITITIAL_AD
    [BoxGroup("INTERSITITIAL")] public InterstitialAd interstitialAdmobAd = null;
    [BoxGroup("INTERSITITIAL")] public bool shouldLoadIntersititialOnInitilization = true;
    [BoxGroup("INTERSITITIAL")] public bool shouldCallStaticOnFail = false;
    [BoxGroup("INTERSITITIAL")] public bool shouldAdDelay = false;
    [BoxGroup("INTERSITITIAL")] [ReadOnly] public bool isLoadingAdmobInterstitialAd = false;
    [BoxGroup("INTERSITITIAL")] [ReadOnly] public int interstitialRetryAttempt = 0;
#endif
#if USE_ADMOB_REWARD_AD

    [BoxGroup("REWARD")] public bool shouldLoadRewardOnInitilization = true;
    [BoxGroup("REWARD")] public bool shouldCallRewardedIntersititialOnFail = false;
    [BoxGroup("REWARD")] [ReadOnly] public int rewardRetryAttempt = 0;
    [BoxGroup("REWARD")] [ReadOnly] bool hasRewardedGaveReward = false;
    [BoxGroup("REWARD")] [ReadOnly] public bool isRewardedAdAdBeingLoaded = false;
    [BoxGroup("REWARD")] public RewardedAd rewardedAd;

#endif

    [BoxGroup("REWARD INTERSITITIAL")] private bool isShowingIntersitialReward = false;
#if USE_ADMOB_REWARD_INTERSITITIAL_AD
   
    [BoxGroup("REWARD INTERSITITIAL")] public bool shouldLoadRewardIntersititialOnInitilization = true;
    [BoxGroup("REWARD INTERSITITIAL")] public bool shouldCallIntersititialOnFail = false;
    [BoxGroup("REWARD INTERSITITIAL")] [ReadOnly] public bool isRewardedInterstitialAdAdBeingLoaded = false;
    [BoxGroup("REWARD INTERSITITIAL")] [ReadOnly] public int rewardedInterstitialRetryAttempt = 1;
    [BoxGroup("REWARD INTERSITITIAL")] private bool hasGaveReward = false;
    
    [BoxGroup("REWARD INTERSITITIAL")] public RewardedInterstitialAd rewardedInterstitialAd;

#endif

    #region CALL FUNCTIONS
    public void ShowBannerAd(AdController.BannerAdTypes type)
    {
        if (!AdConstants.shouldDisplayAds())
        {
            return;
        }

        switch (type)
        {
            case AdController.BannerAdTypes.BANNER_ADMOB:
#if USE_ADMOB_SIMPLE_BANNER
                

                showSimpleBannerAd();
#else
            Debug.LogError(type.ToString()+" NO ENABLED");
#endif
                break;

            case AdController.BannerAdTypes.MREC_ADMOB:
#if USE_ADMOB_MREC_BANNER
                
                showNativeBannerAd();
#endif
                break;
        }
    }
    public void DestroyBannerAd(AdController.BannerAdTypes type)
    {
        switch (type)
        {
            case AdController.BannerAdTypes.BANNER_ADMOB:
#if USE_ADMOB_SIMPLE_BANNER
                DestroyAdmobBannerAd();
#else
            Debug.LogError(type.ToString()+" NO ENABLED");
#endif
                break;

            case AdController.BannerAdTypes.MREC_ADMOB:
#if USE_ADMOB_MREC_BANNER
                DestroyAdmobNativeAd();
#endif
                break;
        }
    }
    public void HideBannerAd(AdController.BannerAdTypes type)
    {

        switch (type)
        {
            case AdController.BannerAdTypes.BANNER_ADMOB:
#if USE_ADMOB_SIMPLE_BANNER
                HideAdmobBannerAd();
#else
            Debug.LogError(type.ToString()+" NO ENABLED");
#endif
                break;

            case AdController.BannerAdTypes.MREC_ADMOB:
#if USE_ADMOB_MREC_BANNER
                HideAdmobNativeAd();
#endif
                break;
        }
    }
    public void showAd(AdController.AdType ad,bool shouldUseAdBreak=true)
    {


        switch (ad)
        {
            case AdController.AdType.STATIC_ADMOB:
                if (!AdConstants.shouldDisplayAds())
                {
                    Debug.Log("USER has remove ads " + ad.ToString() + " cannnot be shown");
                    return;
                }
#if USE_ADMOB_STATIC_AD
                showAdmobStaticAd();
#else
                Debug.Log("Ad not enable");
#endif
                break;
            case AdController.AdType.INTERSTITIAL_ADMOB:
                if (!AdConstants.shouldDisplayAds())
                {
                    Debug.Log("USER has remove ads " + ad.ToString() + " cannnot be shown");
                    return;
                }
#if USE_ADMOB_INTERSITITIAL_AD
                showAdmobInterstitialAd(shouldUseAdBreak);
#else
                Debug.Log("Ad not enable");
#endif
                break;
            case AdController.AdType.REWARD_ADMOB:
#if USE_ADMOB_REWARD_AD
                showRewardedAd();
#else
 AdController.instance.NoAdAvailable();
                Debug.Log("Ad not enable");
#endif
                break;
            case AdController.AdType.OPEN_AD:
                if (!AdConstants.shouldDisplayAds())
                {
                    Debug.Log("USER has remove ads " + ad.ToString() + " cannnot be shown");
                    return;
                }
#if USE_TIER_APP_OPEN
                ShowAppOpenAdIfAvailable();
#else
                Debug.Log("Ad not enable");
#endif
                break;
            case AdController.AdType.REWARDED_INTERSTITIAL_ADMOB:
#if USE_ADMOB_REWARD_INTERSITITIAL_AD
                showRewardedInterstitialAd();
#else
 AdController.instance.NoAdAvailable();
                Debug.Log("Ad not enable");
#endif
                break;
            default:
                break;
        }


    }
    public void loadAd(AdController.AdType ad)
    {


        switch (ad)
        {
            case AdController.AdType.STATIC_ADMOB:
                if (!AdConstants.shouldDisplayAds())
                {
                    Debug.Log("USER has remove ads " + ad.ToString() + " cannnot be shown");
                    return;
                }
#if USE_ADMOB_STATIC_AD
                LoadAdmobStaticAd();
#else
                Debug.Log("Ad not enable");
#endif
                break;
            case AdController.AdType.INTERSTITIAL_ADMOB:
                if (!AdConstants.shouldDisplayAds())
                {
                    Debug.Log("USER has remove ads " + ad.ToString() + " cannnot be shown");
                    return;
                }
#if USE_ADMOB_INTERSITITIAL_AD
                LoadAdmobInterstitialAd();
#else
                Debug.Log("Ad not enable");
#endif
                break;
            case AdController.AdType.REWARD_ADMOB:
#if USE_ADMOB_REWARD_AD
                LoadRewardedAd();
#else
                Debug.Log("Ad not enable");
#endif
                break;
            case AdController.AdType.OPEN_AD:
                if (!AdConstants.shouldDisplayAds())
                {
                    Debug.Log("USER has remove ads " + ad.ToString() + " cannnot be shown");
                    return;
                }
#if USE_TIER_APP_OPEN
                ShowAppOpenAdIfAvailable();
#else
                Debug.Log("Ad not enable");
#endif
                break;
            case AdController.AdType.REWARDED_INTERSTITIAL_ADMOB:
#if USE_ADMOB_REWARD_INTERSITITIAL_AD
                LoadRewardedInterstitiaAd();
#else
                Debug.Log("Ad not enable");
#endif
                break;
            default:
                break;
        }


    }
    #endregion

    #region BANNER AD
    private void showSimpleBannerAd()
    {

#if USE_ADMOB_SIMPLE_BANNER
        if (bannerView != null)
            bannerView.Show();
        else {
            RequestBannerAd();
        }
#endif
    }
    private void RequestBannerAd()
    {

       

#if USE_ADMOB_SIMPLE_BANNER

        string adUnitId = AdsIds.BannerAdUnitIdAdmob();
        if (string.IsNullOrEmpty(adUnitId) || IsBannerAdBeingRequested )
        {

            return;
        }

#if !UNITY_EDITOR
        if (bannerView != null)
        {
            Debug.Log("------ADMOB BANNER AD LOADED -------");
            bannerView.Show();
            return;
        }
#endif

        Debug.Log("------ ADMOB REQUESTING BANNER AD -------");
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
        bannerView = null;

        if (isAdaptiveBanner)
        {

            Banner_adSize = AdSize.GetLandscapeAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        }
        else
        {
            Banner_adSize = AdSize.Banner;
        }

        // Create a banner
        bannerView = new BannerView(adUnitId, Banner_adSize, banner_Position);

        // Add Event Handlers
        bannerView.OnBannerAdLoaded += Callback_BannerAdLoaded;
        bannerView.OnBannerAdLoadFailed += Callback_BannerAdLoadFailed;
        bannerView.OnAdFullScreenContentOpened += Callback_BannerAdShowing;
        // if (SendAdPaidEvents)
        bannerView.OnAdPaid += Callback_BannerAdPaidEvent;

        // Load a banner ad
        IsBannerAdBeingRequested = true;
        bannerView.LoadAd(CreateAdRequest());
#endif
    }
    public bool IsAdmobBannerAvailable()
    {
#if USE_ADMOB_SIMPLE_BANNER
        return bannerView != null ? true : false;
#else
        return false;
#endif
    }
    private void HideAdmobBannerAd()
    {
        Debug.Log("------ ADMOB BANNER HIDE AD  -------");
#if USE_ADMOB_SIMPLE_BANNER
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
        bannerView = null;
#endif
    }
    private void DestroyAdmobBannerAd()
    {
#if USE_ADMOB_SIMPLE_BANNER
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
        bannerView = null;
#endif
    }
#if USE_ADMOB_SIMPLE_BANNER
    private void Callback_BannerAdLoaded()
    {
        if (isAdaptiveBanner)
        {
            FinzAnalysisManager.Instance?.AdAnalysis(AdController.BannerAdTypes.BANNER_ADMOB);
        }
        else
        {
            FinzAnalysisManager.Instance?.AdAnalysis(AdController.BannerAdTypes.BANNER_ADMOB);
        }
        Debug.Log("------ ADMOB BANNER AD LOADED -------");
        IsBannerAdBeingRequested = false;

    }
    private void Callback_BannerAdLoadFailed(LoadAdError error)
    {
        Debug.Log("------ ADMOB LOAD FAILED BANNER AD Reason " + error.GetMessage() + " -------");
        IsBannerAdBeingRequested = false;
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
        bannerView = null;
    }
    private void Callback_BannerAdShowing()
    {

        Debug.Log("------ SHOWING BANNER AD -------");
        IsBannerAdBeingRequested = false;

    }
    private void Callback_BannerAdPaidEvent(AdValue adValue)
    {

#if USE_ADMOB_PAID_EVENT
        if (FinzAnalysisManager.Instance && adValue != null&& bannerView!=null)
        {

            if (isAdaptiveBanner)
            {
                FinzAnalysisManager.Instance.PaidAdAnalytics(AdController.BannerAdTypes.ADAPTIVE_ADMOB.ToString(), bannerView.GetResponseInfo(), adValue);

            }
            else
            {
                FinzAnalysisManager.Instance.PaidAdAnalytics(AdController.BannerAdTypes.BANNER_ADMOB.ToString(), bannerView.GetResponseInfo(), adValue);

            }

        }

#endif
    }
#endif
    #endregion

    #region MREC BANNER
    private void showNativeBannerAd()
    {
#if USE_ADMOB_MREC_BANNER
        if (mrecBannerView != null)
            mrecBannerView.Show();
        else
        {
            RequestNativeBannerAd();
        }
#endif
    }
    private void RequestNativeBannerAd(int width = 200)
    {
#if USE_ADMOB_MREC_BANNER
        string adUnitId = AdsIds.MrecAdUnitIdAdmob();
        if (string.IsNullOrEmpty(adUnitId) || isMrecBannerAdBeingRequested || AdConstants.showNativeBannerAd == false)
        {
            return;
        }
        if (mrecBannerView != null)
        {
            mrecBannerView.Destroy();
        }
        mrecBannerView = null;
 

        Debug.Log("------ REQUESTING ADMOB NATIVE BANNER AD -------");

        mrecBannerView = new BannerView(adUnitId, AdSize.MediumRectangle, mrec_banner_Position);
        // Add Event Handlers
        mrecBannerView.OnBannerAdLoaded += Callback_NativeBannerAdLoaded;
        mrecBannerView.OnBannerAdLoadFailed += Callback_NativeBannerAdLoadFailed;
        mrecBannerView.OnAdFullScreenContentOpened += Callback_NativeBannerAdShowing;
        //if (SendAdPaidEvents)
        mrecBannerView.OnAdPaid += Callback_NativeBannerAdPaidEvent;

        // Load a banner ad
        isMrecBannerAdBeingRequested = true;
        mrecBannerView.LoadAd(CreateAdRequest());
#endif
    }
    public bool IsAdmobNativeBannerAvailable()
    {
#if USE_ADMOB_MREC_BANNER
        return mrecBannerView != null ? true : false;
#else
        return false;
#endif
    }
    private void HideAdmobNativeAd()
    {
#if USE_ADMOB_MREC_BANNER
        if (mrecBannerView != null)
        {
            mrecBannerView.Destroy();
        }
        mrecBannerView = null;
#endif
    }
    private void DestroyAdmobNativeAd()
    {
#if USE_ADMOB_MREC_BANNER
        if (mrecBannerView != null)
        {
            mrecBannerView.Destroy();
        }
        mrecBannerView = null;

#endif
    }

#if USE_ADMOB_MREC_BANNER
    private void Callback_NativeBannerAdLoaded()
    {
        FinzAnalysisManager.Instance?.AdAnalysis(AdController.BannerAdTypes.MREC_ADMOB);
        Debug.Log("------ ADMOB NATIVE BANNER AD LOADED -------");
        isMrecBannerAdBeingRequested = false;
        //  NativeBannerShow();

    }
    private void Callback_NativeBannerAdLoadFailed(LoadAdError error)
    {
        if (mrecBannerView != null)
        {
            mrecBannerView.Destroy();
        }
        mrecBannerView = null;

        Debug.Log("------ LOAD FAILED ADMOB NATIVE BANNER AD Reason " + error.GetMessage() + " -------");
        isMrecBannerAdBeingRequested = false;
        // NativeBannerLoadingFailed();
    }
    private void Callback_NativeBannerAdShowing()
    {
        FinzAnalysisManager.Instance?.AdAnalysis(AdController.BannerAdTypes.Mrec);
        Debug.Log("------ SHOWING ADMOB NATIVE BANNER AD -------");
        // isNativeBannerAdBeingRequested = false;
    }
    private void Callback_NativeBannerAdPaidEvent(AdValue args)
    {
#if USE_ADMOB_PAID_EVENT
        if (FinzAnalysisManager.Instance && args != null&& mrecBannerView!=null)
            FinzAnalysisManager.Instance.PaidAdAnalytics(AdController.BannerAdTypes.MREC_ADMOB.ToString(), mrecBannerView.GetResponseInfo(), args);
#endif
    }
#endif

    #endregion

    #region STATIC
    public bool isStaticAvaiable()
    {
#if USE_ADMOB_STATIC_AD
        return staticAdmobAd == null && staticAdmobAd.CanShowAd();
#endif
        return false;
    }
    private void showAdmobStaticAd()
    {
#if USE_ADMOB_STATIC_AD
        if (isLoadingAdmobStaticAd)
        {
            Debug.Log("Admob static already loading show call cancel");
        }
        if (staticAdmobAd == null)
        {
            Debug.Log("-------static AD IS NOT READY YET ------- ");

            staticAdmobAd = LoadAdmobStaticAd();
            return;
        }

        if (!AdConstants.shouldDisplayAds())
        {
            Debug.Log("Remove ad done static call cancel");
            return;
        }




        AdConstants.IsAdWasShowing = true;

        if (FinzAnalysisManager.instance)
        {
            FinzAnalysisManager.instance.AdAnalysis(AdController.AdType.STATIC_ADMOB);
        }

#if USE_ADMOB_OPEN_AD_7_2_0 || USE_ADMOB_OPEN_AD_8_5
        AdConstants.resumeFromAds = true;
#endif

        staticAdmobAd.Show();
        //isloaded = false;

        Debug.Log("------ Showing admob static AD: -------");// : Static ad loaded : " + isloaded);
#endif
    }
#if USE_ADMOB_STATIC_AD
    private string GetCurrentaAdmobStaticAdID()
    {
#if USE_ADMOB_STATIC_AD
        return AdsIds.StaticAdUnitIdAdmob();
#endif
        return null;
    }
    private InterstitialAd LoadAdmobStaticAd()
    {
        if (SystemInfo.systemMemorySize <= lowMemoryLimit) {
            Debug.Log("---Stoping Admob Static load Low memory");
            return null;
        }

            string AdsIds = GetCurrentaAdmobStaticAdID();
        if (!AdConstants.shouldDisplayAds())
        {
            Debug.Log("User has done remove ads");
            return null;
        }
        // Clean up the old ad before loading a new one.
        if (staticAdmobAd != null)
        {
            staticAdmobAd.Destroy();
            staticAdmobAd = null;
        }

        Debug.Log("Loading the admob static ad.");

        // create our request used to load the ad.
        var adRequest = CreateAdRequest();

        // send the request to load the ad.
        isLoadingAdmobStaticAd = true;
        InterstitialAd.Load(AdsIds, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.Log("admob static ad failed to load an ad " +
                                   "with error : " + error.GetMessage());


                    staticRetryAttempt++;
                    double retryDelay = Math.Pow(2, Math.Min(6, staticRetryAttempt));
                    Invoke(nameof(LoadAdmobStaticAd), (float)retryDelay);
                    isLoadingAdmobStaticAd = false;
                    return;
                }

                Debug.Log("admob static ad loaded with response : "
                          + ad.GetResponseInfo());
                staticRetryAttempt = 0;
                staticAdmobAd = ad;
                isLoadingAdmobStaticAd = false;
                RegisterAdmobStaticEventHandlers(staticAdmobAd);
                RegisterAdmobStatictReloadHandler(staticAdmobAd);
            });
        return staticAdmobAd;

    }
    private void RegisterAdmobStaticEventHandlers(InterstitialAd staticAd)
    {
        //#if USE_ADMOB
        // Raised when the ad is estimated to have earned money.
        if (staticAd == null)
        {
            return;
        }
        staticAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("admob static ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));

#if USE_ADMOB_PAID_EVENT
            if (FinzAnalysisManager.Instance && staticAd != null)
                FinzAnalysisManager.Instance.PaidAdAnalytics(AdController.AdType.STATIC_ADMOB.ToString(), staticAd.GetResponseInfo(), adValue);
#endif
        };

        // Raised when an impression is recorded for an ad.
        staticAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("-------- static ad Imprasseioned --------");
        };
        // Raised when a click is recorded for an ad.
        staticAd.OnAdClicked += () =>
        {
            Debug.Log("a-------- static ad Clicked --------");
        };
        // Raised when an ad opened full screen content.
        staticAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("-------- static ad Opened --------");
        };
        // Raised when the ad closed full screen content.
        staticAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("admob static ad full screen content closed.");
            AdConstants.IsAdWasShowing = true;
            Debug.Log("-------- static ad dismissed --------");
        };
        // Raised when the ad failed to open full screen content.
        staticAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log("-------- static ad  Failed -------- " + error);
        };
    }
    private void RegisterAdmobStatictReloadHandler(InterstitialAd staticAd)
    {
        // Raised when the ad closed full screen content.
        // Raised when the ad closed full screen content.
        if (staticAd == null)
        {
            return;
        }

        staticAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("admob static Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            double retryDelay = Math.Pow(2, Math.Min(6, 1));
            Invoke(nameof(LoadAdmobStaticAd), (float)retryDelay);
        };
        // Raised when the ad failed to open full screen content.
        staticAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log("admob static ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            double retryDelay = Math.Pow(2, Math.Min(6, staticRetryAttempt));
            Invoke(nameof(LoadAdmobStaticAd), (float)retryDelay);
        };
        //#endif
    }
#endif
    #endregion

    #region INTERSTITIAL
    public bool isIntersitialAvaiable()
    {
#if USE_ADMOB_INTERSITITIAL_AD
        return interstitialAdmobAd != null && interstitialAdmobAd.CanShowAd();
#endif
        return false;
    }
    private void showAdmobInterstitialAd(bool shouldUseAdDelayCheck=true)
    {
#if USE_ADMOB_INTERSITITIAL_AD
        if (isLoadingAdmobInterstitialAd)
        {
            Debug.Log("Admob Intersititial already loading show call cancel");
            if (AdConstants.isShowingLPIntersititialReward && AdController.instance)
            {
                showAd(AdController.AdType.REWARD_ADMOB);
                AdConstants.isShowingLPIntersititialReward = false;
            }
            else if (isShowingIntersitialReward)
            {
                isShowingIntersitialReward = false;
                AdController.instance.NoAdAvailable();
            }
            else {
            }
            return;
        }
        //staticAd.Show();
        //return;
        if (interstitialAdmobAd == null)
        {
            Debug.Log("-------admob interstitial AD IS NOT READY YET ------- ");

            interstitialAdmobAd = LoadAdmobInterstitialAd();
            if (AdConstants.isShowingLPIntersititialReward&&AdController.instance)
            {
                showAd(AdController.AdType.REWARD_ADMOB);
                AdConstants.isShowingLPIntersititialReward = false;
            }
            else if (isShowingIntersitialReward) {
                isShowingIntersitialReward = false;
                AdController.instance.NoAdAvailable();
            }
            {

            }
            return;
        }

        if (!AdConstants.isShowingLPIntersititialReward&& !isShowingIntersitialReward &&!AdConstants.shouldDisplayAds())
        {
            Debug.Log("Remove ad done interstitial call cancel");
            return;
        }
        try
        {
            if (shouldUseAdDelayCheck && shouldAdDelay &&
                DateTime.Now <= AdController.instance.currentTime_Interstitial.AddSeconds(AdConstants.adDelay))
            {
                Debug.Log("-------- Admob Interstitial Ad retur due to ad delay --------");
                return;
            }
        }
        catch (Exception e) {

        }



        AdConstants.IsAdWasShowing = true;

        if (FinzAnalysisManager.instance)
        {
            if (AdConstants.isShowingLPIntersititialReward)
            {
                FinzAnalysisManager.instance.AdAnalysis(AdController.AdType.REWARDED_INTERSTITIAL_ADMOB);
            }
            else
            {
                FinzAnalysisManager.instance.AdAnalysis(AdController.AdType.INTERSTITIAL_ADMOB);
            }
        }

#if USE_ADMOB_OPEN_AD_7_2_0 || USE_ADMOB_OPEN_AD_8_5
        AdConstants.resumeFromAds = true;
#endif
        if (interstitialAdmobAd != null)
        {
            interstitialAdmobAd.Show();
        }
        //isloaded = false;

        Debug.Log("------ Showing admob interstitial AD: -------");// : Static ad loaded : " + isloaded);
#endif
    }
#if USE_ADMOB_INTERSITITIAL_AD
    private string GetCurrentaAdmobInterstitialAdID()
    {
#if USE_ADMOB_INTERSITITIAL_AD
        return AdsIds.InterstitialAdUnitIdAdmob();
#endif
        return null;
    }
    private InterstitialAd LoadAdmobInterstitialAd()
    {
        if (SystemInfo.systemMemorySize <= lowMemoryLimit)
        {
            Debug.Log("---Stoping Admob Interstitial load Low memory");
            return null;
        }

        string AdsIds = GetCurrentaAdmobInterstitialAdID();
        if (!AdConstants.shouldDisplayAds())
        {
            Debug.Log("User has done remove ads");
            return null;
        }
        // Clean up the old ad before loading a new one.
        if (interstitialAdmobAd != null)
        {
            interstitialAdmobAd.Destroy();
            interstitialAdmobAd = null;
        }

        Debug.Log("Loading the admob interstitial ad.");

        // create our request used to load the ad.
        var adRequest = CreateAdRequest();

        // send the request to load the ad.
        isLoadingAdmobInterstitialAd = true;
        InterstitialAd.Load(AdsIds, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.Log("admob interstitial ad failed to load an ad " +
                                   "with error : " + error.GetMessage());


                    interstitialRetryAttempt++;
                    double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
                    Invoke(nameof(LoadAdmobInterstitialAd), (float)retryDelay);
                    isLoadingAdmobInterstitialAd = false;
                    return;
                }

                Debug.Log("admob interstitial ad loaded with response : "
                          + ad.GetResponseInfo());
                interstitialRetryAttempt = 0;
                interstitialAdmobAd = ad;
                isLoadingAdmobInterstitialAd = false;
                RegisterAdmobInterstitialEventHandlers(interstitialAdmobAd);
                RegisterAdmobInterstitialReloadHandler(interstitialAdmobAd);
            });
        return interstitialAdmobAd;

    }
    private void RegisterAdmobInterstitialEventHandlers(InterstitialAd interstitialAd)
    {
        //#if USE_ADMOB
        // Raised when the ad is estimated to have earned money.
        if (interstitialAd == null)
        {
            return;
        }
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("admob interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));

#if USE_ADMOB_PAID_EVENT
            if (FinzAnalysisManager.Instance && interstitialAd != null)
                FinzAnalysisManager.Instance.PaidAdAnalytics(AdController.AdType.INTERSTITIAL_ADMOB.ToString(), interstitialAd.GetResponseInfo(), adValue);
#endif
        };

        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("-------- interstitial ad Imprasseioned --------");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("a-------- interstitial ad Clicked --------");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("-------- interstitial ad Opened --------");
        };
        // Raised when the ad closed full screen content.
        // Raised when the ad failed to open full screen content.
       
    }
    private void RegisterAdmobInterstitialReloadHandler(InterstitialAd interstitialAd)
    {
        // Raised when the ad closed full screen content
        // Raised when the ad closed full screen contennt
        // Raised when the ad closed full screen content.
        // Raised when the ad closed full screen content.
        if (interstitialAd == null)
        {
            return;
        }
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("admob interstitial Ad full screen content closed.");
            AdConstants.IsAdWasShowing = true;
            if (shouldAdDelay)
            {
                AdController.instance.ResetInterstitialTime();
            }
            if (AdConstants.isShowingLPIntersititialReward||isShowingIntersitialReward)
            {
                AdConstants.sawRewarded = true;
              AdController.instance.DecideForReward();
                AdConstants.isShowingLPIntersititialReward = false;
                isShowingIntersitialReward = false;
            }
            // Reload the ad so that we can show another as soon as possible.
            double retryDelay = Math.Pow(2, Math.Min(6, 1));
            Invoke(nameof(LoadAdmobInterstitialAd), (float)retryDelay);
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log("admob interstitial ad failed to open full screen content " +
                           "with error : " + error.GetMessage());
            if (isShowingIntersitialReward) {

                AdController.instance.NoAdAvailable();
                isShowingIntersitialReward = false;
            }
            if (AdConstants.isShowingLPIntersititialReward)
            {
                showAd(AdController.AdType.REWARD_ADMOB);
                AdConstants.isShowingLPIntersititialReward = false;
            }
            else if (shouldCallStaticOnFail)
            {
                showAd(AdController.AdType.STATIC_ADMOB);
            }
            else
            {
            }
            // Reload the ad so that we can show another as soon as possible.
            double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
            Invoke(nameof(LoadAdmobInterstitialAd), (float)retryDelay);

        };
        //#endif
    }
#endif
    #endregion

    #region REWARD
#if USE_ADMOB_REWARD_AD
    private string GetCurrentRewardedAdID()
    {
        return AdsIds.RewardAdUnitIdAdmob();
    }
    private void LoadRewardedAd()
    {
        if (SystemInfo.systemMemorySize <= lowMemoryLimit)
        {
            Debug.Log("---Stoping Admob Rewarded load Low memory");
            return;
        }
        if (isRewardedAdAdBeingLoaded)
        {
            return;
        }

        string AdsIds = GetCurrentRewardedAdID();
        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading rewarded interstitial ad.");
        isRewardedAdAdBeingLoaded = true;
        // Create our request used to load the ad.
        var adRequest = CreateAdRequest();

        RewardedAd.Load(AdsIds, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.Log("Rewarded ad failed to load an ad with error : " + error.GetMessage());
                Callback_RewardedAdLoadFailed();
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.Log("Unexpected error: Rewarded load event fired with null ad and null error.");
                Callback_RewardedAdLoadFailed();
                return;
            }

            // The operation completed successfully.
            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
            rewardedAd = ad;
            Callback_RewardedAdLoaded();

            // Register to ad events to extend functionality.
            RegisterRewardedAdEventHandlers(ad);

        });

    }
    private void Callback_RewardedAdLoadFailed()
    {
        Debug.Log("(------ LOAD FAILED REWARDED AD ------");
        isRewardedAdAdBeingLoaded = false;

        rewardRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardRetryAttempt));
        Invoke(nameof(LoadRewardedAd), (float)retryDelay);
    }
    private void Callback_RewardedAdLoaded()
    {
        Debug.Log("(------ REWARDED AD LOADED ------");
        isRewardedAdAdBeingLoaded = false;
        rewardRetryAttempt = 0;
    }
    private void RegisterRewardedAdEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += Callback_RewardedAdPaidEvent;
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when the ad opened full screen content.
        ad.OnAdFullScreenContentOpened += Callback_RewardedAdShowing;
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += Callback_RewardedAdClosed;
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += Callback_RewardedAdShowingFailed;
    }
    private void Callback_RewardedAdPaidEvent(AdValue args)
    {

#if USE_ADMOB_PAID_EVENT
        if (FinzAnalysisManager.Instance && rewardedAd != null)
            FinzAnalysisManager.Instance.PaidAdAnalytics(AdController.AdType.REWARD_ADMOB.ToString(), rewardedAd.GetResponseInfo(), args);
#endif
    }
    private void Callback_RewardedAdShowing()
    {
        // FinzAnalysisManager.Instance?.AdAnalysis(AdController.AdType.REWARDED);
        Debug.Log("(------ SHOWING REWARDED AD ------");
        isRewardedAdAdBeingLoaded = false;
    }
    private void Callback_RewardedAdClosed()
    {
        Debug.Log("(------ CLOSED REWARDED AD ------");
        isRewardedAdAdBeingLoaded = false;
        rewardRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardRetryAttempt));
        Invoke(nameof(LoadRewardedAd), (float)retryDelay);
    }
    private void Callback_RewardedAdEarnedReward()
    {
        AdConstants.sawRewarded = true;
        hasRewardedGaveReward = true;
        AdController.instance.DecideForReward();
        Debug.Log("Admob Rewarded  gave rewarded : ");
    }
    private void Callback_RewardedAdShowingFailed(AdError error)
    {
        Debug.Log("(------ SHOWING FAILED REWARDED AD ------");
        isRewardedAdAdBeingLoaded = false;
        hasRewardedGaveReward = false;
        rewardRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardRetryAttempt));
        Invoke(nameof(LoadRewardedAd), (float)retryDelay);
        if (shouldCallRewardedIntersititialOnFail)
        {
            showAd(AdController.AdType.REWARDED_INTERSTITIAL_ADMOB);
        }
        else
        {
            AdController.instance.NoAdAvailable();
        }
    }
#endif
    public bool isAdmobRWAdAvailable()
    {
#if USE_ADMOB_REWARD_AD
        return rewardedAd != null && rewardedAd.CanShowAd() ? true : false;
#else
        return false;
#endif

    }
    private void showRewardedAd()
    {

#if USE_ADMOB_REWARD_AD
        if (isAdmobRWAdAvailable())
        {
            AdConstants.sawRewarded = false;
            isRewardedAdAdBeingLoaded = false;
            hasRewardedGaveReward = false;
#if USE_ADMOB_OPEN_AD_7_2_0 || USE_ADMOB_OPEN_AD_8_5
            AdConstants.resumeFromAds = true;
#endif
            rewardedAd.Show((Reward reward) =>
            {
                Callback_RewardedAdEarnedReward();
            });
        }
        else
        {
            if (isRewardedAdAdBeingLoaded)
            {
                Debug.Log("Add being loaded");
            }
            else
            {
                hasRewardedGaveReward = false;
                LoadRewardedAd();
            }

            if (shouldCallRewardedIntersititialOnFail) {
                showAd(AdController.AdType.REWARDED_INTERSTITIAL_ADMOB);
            }
            else {
                AdController.instance.NoAdAvailable();
            }
        }
#else
        AdController.instance.NoAdAvailable();
#endif


    }
    #endregion

    #region REWARD INTERSITITIAL
#if USE_ADMOB_REWARD_INTERSITITIAL_AD
    private string GetCurrentRewardedInterstitialAdID()
    {
        return AdsIds.RewardIntersititialAdUnitIdAdmob();
    }
    private void LoadRewardedInterstitiaAd()
    {
        try
        {
            if (SystemInfo.systemMemorySize <= lowMemoryLimit)
            {
                Debug.Log("---Stoping Admob Rewarded Interstitia; load Low memory");
                return ;
            }
            if (isRewardedInterstitialAdAdBeingLoaded)
            {
                return;
            }

            string AdsIds = GetCurrentRewardedInterstitialAdID();
            // Clean up the old ad before loading a new one.
            if (rewardedInterstitialAd != null)
            {
                rewardedInterstitialAd.Destroy();
                rewardedInterstitialAd = null;
            }

            Debug.Log("Loading rewarded interstitial ad.");
            isRewardedInterstitialAdAdBeingLoaded = true;
            // Create our request used to load the ad.
            var adRequest = CreateAdRequest();

            // Send the request to load the ad.
            RewardedInterstitialAd.Load(AdsIds, adRequest,
                (RewardedInterstitialAd ad, LoadAdError error) =>
                {
                    // If the operation failed with a reason.
                    if (error != null)
                    {
                        isRewardedInterstitialAdAdBeingLoaded = false;
                        Debug.Log("Rewarded interstitial ad failed to load an ad with error : "
                                     + error.GetMessage());

                        rewardedInterstitialRetryAttempt++;
                        double retryDelay = Math.Pow(2, Math.Min(6, rewardedInterstitialRetryAttempt));
                        Invoke(nameof(LoadRewardedInterstitiaAd), (float)retryDelay);

                        return;
                    }
                    // If the operation failed for unknown reasons.
                    // This is an unexpexted error, please report this bug if it happens.
                    if (ad == null)
                    {
                        isRewardedInterstitialAdAdBeingLoaded = false;
                        Debug.Log("Unexpected error: Rewarded interstitial Tier load event fired with null ad and null error.");
                        rewardedInterstitialRetryAttempt++;
                        double retryDelay = Math.Pow(2, Math.Min(6, rewardedInterstitialRetryAttempt));
                        Invoke(nameof(LoadRewardedInterstitiaAd), (float)retryDelay);
                        return;
                    }

                    // The operation completed successfully.
                    Debug.Log("Rewarded interstitial ad  loaded with response : "
                        + ad.GetResponseInfo());
                    rewardedInterstitialAd = ad;
                    rewardedInterstitialRetryAttempt = 0;
                    // Register to ad events to extend functionality.
                    RegisterRewardedInterstitiaEventHandlers(ad);
                    isRewardedInterstitialAdAdBeingLoaded = false;

                });
        }
        catch
        {

        }
    }
    private void RegisterRewardedInterstitiaEventHandlers(RewardedInterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {


#if USE_ADMOB_PAID_EVENT
            if (FinzAnalysisManager.Instance && ad != null)
                FinzAnalysisManager.Instance.PaidAdAnalytics(AdController.AdType.REWARDED_INTERSTITIAL_ADMOB.ToString(), ad.GetResponseInfo(), adValue);
#endif

            Debug.Log(String.Format("Rewarded interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {

            Debug.Log("Rewarded interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            AdConstants.resumeFromAds = false;
            if (!hasGaveReward)
            {
                hasGaveReward = true;
                AdController.instance.DecideForReward();
            }
            Debug.Log("Rewarded interstitial ad full screen content closed.");
            rewardedInterstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, rewardedInterstitialRetryAttempt));
            Invoke(nameof(LoadRewardedInterstitiaAd), (float)retryDelay);
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            AdConstants.resumeFromAds = false;
            Debug.Log("Rewarded interstitial ad failed to open full screen content" +
                           " with error : " + error.GetMessage());
            rewardedInterstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, rewardedInterstitialRetryAttempt));
            Invoke(nameof(LoadRewardedInterstitiaAd), (float)retryDelay);
            if (shouldCallIntersititialOnFail)
            {
                isShowingIntersitialReward = true;
                showAdmobInterstitialAd();
            }
            else
            {
                AdController.instance.NoAdAvailable();
            }
        };
    }
#endif
    private void showRewardedInterstitialAd()
    {
#if USE_ADMOB_REWARD_INTERSITITIAL_AD
        isShowingIntersitialReward = false;
        Debug.Log("Admob Rewarded interstitial CALL");
        AdConstants.sawRewarded = false;
        if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
        {
            Debug.Log("Admob Rewarded interstitial AVAILABLE");
            AdConstants.sawRewarded = false;
            hasGaveReward = false;
            AdConstants.resumeFromAds = true;
            if (FinzAnalysisManager.instance) {
                FinzAnalysisManager.instance.AdAnalysis(AdController.AdType.REWARDED_INTERSTITIAL_ADMOB);
            }
           
#if USE_ADMOB_OPEN_AD_7_2_0 || USE_ADMOB_OPEN_AD_8_5
            AdConstants.resumeFromAds = true;
#endif
            Debug.Log("Admob Rewarded interstitial SHOW CALL");
            rewardedInterstitialAd.Show((Reward reward) =>
            {
                AdConstants.sawRewarded = true;
                hasGaveReward = true;
                AdController.instance.DecideForReward();
                Debug.Log("Admob Rewarded interstitial gave rewarded : ");
            });
        }
        else
        {
            Debug.Log("Admob Rewarded interstitial NNOT READY : ");
            if (shouldCallIntersititialOnFail) {
                isShowingIntersitialReward = true;
                showAdmobInterstitialAd();
            }
            else {
                AdController.instance.NoAdAvailable();
            }
       
            rewardedInterstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, rewardedInterstitialRetryAttempt));
            Invoke(nameof(LoadRewardedInterstitiaAd), (float)retryDelay);
        }
#endif

    }
    public bool isRWIntersititialAvaible()
    {
#if USE_ADMOB_REWARD_INTERSITITIAL_AD
        return rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd();
#else
        return false;
#endif
    }
    #endregion


    private void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        AdmobInitlizationManager.admobSdkInitializationAddition += LoadAd;
    }
    public void LoadAd()
    {
#if USE_ADMOB_SIMPLE_BANNER
        RequestBannerAd();
#endif
#if USE_ADMOB_INTERSITITIAL_AD
        if (shouldLoadIntersititialOnInitilization)
        {
            Invoke(nameof(LoadAdmobInterstitialAd), 2);
        }
#endif
#if USE_ADMOB_STATIC_AD
        if (shouldLoadStaticOnInitilization)
        {
            Invoke(nameof(LoadAdmobStaticAd), 4);
        }
#endif
#if USE_ADMOB_REWARD_AD
        if (shouldLoadRewardOnInitilization)
        {
            Invoke(nameof(LoadRewardedAd), 6);
        }
#endif
#if USE_ADMOB_REWARD_INTERSITITIAL_AD
        if (shouldLoadRewardIntersititialOnInitilization)
        {
            Invoke(nameof(LoadRewardedInterstitiaAd), 10);
        }
#endif
    }
#if USE_ADMOB_SIMPLE_BANNER || USE_ADMOB_MREC_BANNER || USE_ADMOB_STATIC_AD || USE_ADMOB_INTERSITITIAL_AD || USE_ADMOB_REWARD_AD ||USE_ADMOB_REWARD_INTERSITITIAL_AD || USE_ADMOB_OPEN_AD_8_5
    private AdRequest CreateAdRequest()
    {
        AdRequest request = new AdRequest();
        return request;
    }
#endif

} 

