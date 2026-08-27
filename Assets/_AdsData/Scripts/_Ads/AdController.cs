using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;



#if USE_FIREBASE
using Firebase;
#endif
#if USE_FIREBASE && USE_REMOTE_CONFIG
using Firebase.RemoteConfig;
#endif

#if USE_IS
using Unity.Services.LevelPlay;

#endif
using Sirenix.OdinInspector;
using UnityEngine;

public class AdController : MonoBehaviour
{
    #region Variables
    public enum FinzBannerPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        CenterLeft,
        Center,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
    public enum BannerShowing
    {
        None, Banner, Adaptive, Mrec, Banner_Admmob, Mrec_Admob, Adaptive_Admob
    }
    public enum BannerAdTypes
    {
        BANNER, ADAPTIVE, Mrec, IDLE_NATIVE, BANNER_ADMOB, MREC_ADMOB, ADAPTIVE_ADMOB
    }

    public enum AdType
    {
        STATIC, INTERSTITIAL, REWARDED, LP_INTERSTITIAL_REWARDED, OPEN_AD, SECOND_INTERSTITIAL, INTERSTITIAL_ADMOB, REWARDED_INTERSTITIAL_ADMOB, STATIC_ADMOB, REWARD_ADMOB
    }


    [BoxGroup("INFO")] [HideInInspector] public bool isSdkInitialized = false;
    [BoxGroup("INFO")] public bool callOnlyAdmob = false;
#if USE_IS
    [Tooltip("Banner with variable height and width.")]
    [HideIf("callOnlyAdmob")] [BoxGroup("BANNER")] public bool isAdaptiveBanner = false;

    [HideIf("callOnlyAdmob")] [BoxGroup("BANNER")] public bool shouldBannerIgnoreNotchArea = false;

    [Tooltip("Will give a show call for banner as soon as SDK is initialized")]
    [HideIf("callOnlyAdmob")] [BoxGroup("BANNER")] public bool showBannerOnIntialization = false;

    [Tooltip("Will give a show call for banner every time banner is loaded")]
    [HideIf("callOnlyAdmob")] [BoxGroup("BANNER")] public bool showBannerOnLoadSuccess = false;

    [HideIf("callOnlyAdmob")] [BoxGroup("BANNER")] private bool canDestoryBanner = true;
    [HideIf("callOnlyAdmob")] [BoxGroup("BANNER")] public FinzBannerPosition bannerPosition = FinzBannerPosition.TopCenter;
  //  [HideIf("callOnlyAdmob")] [BoxGroup("BANNER")] public LevelPlayBannerPosition bannerPosition = LevelPlayBannerPosition.TopCenter;
    [HideIf("callOnlyAdmob")] [BoxGroup("BANNER")] public LevelPlayBannerAd bannerAd;
    [HideIf("callOnlyAdmob")] [BoxGroup("BANNER")] private int bannerRetryAttempt;
    [HideIf("callOnlyAdmob")] [BoxGroup("BANNER")] private bool isBannerAdShowing = false;
    [HideIf("callOnlyAdmob")] [BoxGroup("BANNER")] private bool isBannerAdisbeingLoaded;
    [HideIf("callOnlyAdmob")] [BoxGroup("BANNER")] private bool canLoadBanner = true;

#if USE_IS_NATIVE_MREC
    [Tooltip("Will give a hide call to banner every time Mrec is called")]
    [HideIf("callOnlyAdmob")] [BoxGroup("MREC")] public bool shouldHideBannerOnMrec = false;

    [Tooltip("Will Load Mrec on SDK initilization")]
    [HideIf("callOnlyAdmob")] [BoxGroup("MREC")] public bool shouldLoadMrecOnIntialization = true;

    [HideIf("callOnlyAdmob")] [BoxGroup("MREC")] private int mrecBannerRetryAttempt = 0;
    [HideIf("callOnlyAdmob")] [BoxGroup("MREC")] private bool isMrecBannerAdShowing = false;
    [HideIf("callOnlyAdmob")] [BoxGroup("MREC")] private bool isMrecBannerAdisbeingLoaded = false;
    [HideIf("callOnlyAdmob")] [BoxGroup("MREC")] public FinzBannerPosition mrecBannerPosition = FinzBannerPosition.BottomCenter;
//   [HideIf("callOnlyAdmob")] [BoxGroup("MREC")] public LevelPlayBannerPosition mrecBannerPosition = LevelPlayBannerPosition.BottomCenter;
    [HideIf("callOnlyAdmob")] [BoxGroup("MREC")] public LevelPlayBannerAd mrecBannerAd;
    [HideIf("callOnlyAdmob")] [BoxGroup("MREC")] private bool canLoadMrec = true;
#endif

    [HideIf("callOnlyAdmob")] [BoxGroup("INTERSITITIAL")] private LevelPlayInterstitialAd interstitialAd;

    [Tooltip("Will call Admob interstitial if LP ad is not available")]
    [HideIf("callOnlyAdmob")] [BoxGroup("INTERSITITIAL")] public bool callAdmobIntersititialOnFail = false;

    [HideIf("callOnlyAdmob")] [BoxGroup("INTERSITITIAL")] [ReadOnly] public bool isInterstitialAdisbeingLoaded;
    [HideIf("callOnlyAdmob")] [BoxGroup("INTERSITITIAL")] [HideInInspector] public int interstitialRetryAttempt;
#if THREE_TIER_INTER
    [HideIf("callOnlyAdmob")] [BoxGroup("INTERSITITIAL")] [ReadOnly] public int currentInterstitialTier = 0;
#endif

#if USE_IS_SECOND_INTERSITITIAL
    [Tooltip("Will call Admob interstitial if LP second ad is not available")]
    [HideIf("callOnlyAdmob")] [BoxGroup("SECOND INTERSITITIAL")] public bool callAdmobIntersititialOnSecondFail = false;

    [Tooltip("Will load SecondIntersititial on SDK initilization")]
    [HideIf("callOnlyAdmob")] [BoxGroup("SECOND INTERSITITIAL")] public bool shouldLoadSecondIntersititialOnIntialization = true;

    [HideIf("callOnlyAdmob")] [BoxGroup("SECOND INTERSITITIAL")] private LevelPlayInterstitialAd secondInterstitialAd;
    [HideIf("callOnlyAdmob")] [BoxGroup("SECOND INTERSITITIAL")] private bool isSecondInterstitialAdisbeingLoaded;
    [HideIf("callOnlyAdmob")] [BoxGroup("SECOND INTERSITITIAL")] [HideInInspector] public int secondInterstitialRetryAttempt;
#endif

    [Tooltip("Will reset intersititial timmer on reward show")]
    [HideIf("callOnlyAdmob")] [BoxGroup("REWARD")] public bool shouldResetIntersititialTimeOnReward = false;

    [Tooltip("Will call LP interstitial if LP reward ad is not available")]
    [HideIf("@callOnlyAdmob==true|| CallLpIntersititialAndAdmobIntersititial==true || CallAdmobReward==true")] [BoxGroup("REWARD")] public bool CallLpIntersititial = false;

    [Tooltip("Will call LP interstitial and than Admob interstitial if LP reward ad is not available")]
    [HideIf("@callOnlyAdmob==true|| CallLpIntersititial==true || CallLpIntersititial==true")] [BoxGroup("REWARD")] public bool CallLpIntersititialAndAdmobIntersititial = false;

    [Tooltip("Will call Admob reward if LP reward ad is not available")]
    [HideIf("@callOnlyAdmob==true||CallLpIntersititialAndAdmobIntersititial==true||CallLpIntersititial==true")] [BoxGroup("REWARD")] public bool CallAdmobReward = false;

    [HideIf("callOnlyAdmob")] [BoxGroup("REWARD")] private LevelPlayRewardedAd rewardedVideoAd;
    [HideIf("callOnlyAdmob")] [BoxGroup("REWARD")] [ReadOnly] public bool isRewardedAdisbeingLoaded;
    [HideIf("callOnlyAdmob")] [BoxGroup("REWARD")] private int rewardedRetryAttempt;
#if THREE_TIER_REWARD
    [HideIf("callOnlyAdmob")] [BoxGroup("REWARD")] [ReadOnly] public int currentRewardedTier = 0;
#endif
#endif

    [BoxGroup("OTHER")] public bool dontDestroyOnLoad = true;

    public static AdController instance;
    // Property to access the instance
    public static AdController Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("AdController instance is null. Make sure the instance is initialized before accessing it.");
            }

            return instance;
        }
    }

    //========================= BANNER AD CALLBACKS =========================//
    // On Banner failed to Load
    public delegate void bannerLoadingFailed();
    public static event bannerLoadingFailed bannerLoadingFailedMethod;
    // On Banner showing
    public delegate void bannerLoadingSuccessful();
    public static event bannerLoadingSuccessful bannerLoadingSuccessfulMethod;

    //========================= REWARDED CALLBACKS =========================//

    public delegate void rewardedVideoWatched();
    public static event rewardedVideoWatched gaveRewardMethod;

    //========================= REVIEW DIALOG CALLBACKS =========================//
    // On Gave rewarded Ad
    public delegate void reviewDialog();
    public static event reviewDialog reviewDialogMethod;
    // On No rewarded Ad
    public delegate void NoRewardedAd();
    public static event NoRewardedAd noRewardedVideoMethod;
    // On rewarded Cancle
    public delegate void cancelRewardedAd();
    public static event cancelRewardedAd cancelRewardedAdMethod;
    // On rewarded Ad Load
    public delegate void rewardedAdLoad();
    public static event rewardedAdLoad rewardedAdLoadMethod;
    // On rewarded Ad Load Failed
    public delegate void rewardedAdLoadFailed();
    public static event rewardedAdLoadFailed rewardedAdLoadFailedMethod;
    // On rewarded Ad Show 
    public delegate void rewardedAdShowing();
    public static event rewardedAdShowing rewardedAdShowingMethod;
    // On rewarded Ad Show Failed
    public delegate void rewardedAdShowingFailed();
    public static event rewardedAdShowingFailed rewardedAdShowingFailedMethod;

    //========================= FIREBASE REMOTE CONFIG CALLBACKS =========================//
#if USE_FIREBASE && USE_REMOTE_CONFIG
    public delegate void FirebaseRemoteConfig(IDictionary<string, ConfigValue> keyValues);
    public static event FirebaseRemoteConfig onFirebaseRemoteConfigSuccess;
#endif

    //========================= GAME SOUND CALLBACKS =========================//
    public delegate void SoundDelegate(bool soundStatus);
    public static event SoundDelegate updateGameSoundMethod;

    //========================= IN APP REVIEW MANAGER =========================//
    [HideInInspector]
    public InappReviewManager iapInstance;


    public DateTime currentTime_Interstitial; // current interstitial ad time for show ads delay
    public DateTime currentTime_SecondInterstitial; // current interstitial ad time for show ads delay
    private DateTime currentTime_Rewarded; // current rewarded ad time for show ads delay

    #endregion



    public void Awake()
    {

        // Ensure that only one instance of the singleton class exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }
        AdConstants.CountGameSession();



#if USE_IS

#if USE_IS_ADQUALITY
        IronSourceAdQuality.Initialize(AdsIds.SDKKey());
#endif
       

        LevelPlay.SetMetaData("do_not_sell", "false");
        LevelPlay.SetMetaData("is_child_directed", "false");
        LevelPlay.SetMetaData("is_deviceid_optout", "false");
        LevelPlay.SetMetaData("is_child_directed", "false");
        LevelPlay.SetMetaData("BidMachine_COPPA", "true");
        LevelPlay.SetMetaData("Yandex_COPPA", "true");
        LevelPlay.SetMetaData("Meta_Mixed_Audience", "true");
        LevelPlay.SetMetaData("UnityAds_coppa", "true");
        LevelPlay.SetMetaData("Pangle_COPPA", "0");
        LevelPlay.SetMetaData("Mintegral_COPPA", "true");
        LevelPlay.SetMetaData("Vungle_coppa", "true");
        LevelPlay.SetMetaData("InMobi_AgeRestricted", "false");
        LevelPlay.SetMetaData("AdMob_TFCD", "false");
        LevelPlay.SetMetaData("DT_IsChild", "false");
        LevelPlay.SetMetaData("DT_COPPA", "true");
        LevelPlay.SetMetaData("Moloco_COPPA", "true");
        LevelPlay.SetConsent(true);
        LevelPlay.SetPauseGame(true);
        isBannerAdShowing = false;
        canLoadBanner = true;
        InitializeInterstitialAds();
        InitializeRewardedAds();
        InitializePaidAdEvent();
#endif
       
        //   SdkInitializationCompletedEvent();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Debug.Log("Start adcontroller");

        iapInstance = GetComponent<InappReviewManager>(); // Gettings in app review manager
        if (iapInstance == null)
            iapInstance = gameObject.AddComponent<InappReviewManager>();

    }
    public void StartAdController()
    {


#if USE_IS


#if UNITY_ANDROID
        string appKey = AdsIds.SDKKey();
#elif UNITY_IPHONE
        string appKey = AdsIds.SDKKey();
#else
        string appKey = "unexpected_platform";
#endif
#if USE_IS
        Debug.Log("unity-script: IronSource.Agent.validateIntegration");

        LevelPlay.ValidateIntegration();

        Debug.Log($"[LevelPlaySample] Unity version {LevelPlay.UnityVersion}");

        Debug.Log("[LevelPlaySample] Register initialization callbacks");
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
        LevelPlay.Init(AdsIds.SDKKey());
#endif

#endif

    }




#if USE_IS

    void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        Debug.Log("------ [LevelPlaySample] Received SdkInitializationFailedEvent with Error:" + error.ErrorMessage);
        Invoke("StartAdController", 10);
    }
    void SdkInitializationCompletedEvent(LevelPlayConfiguration configuration)
    {

        AdConstants.DebugGreen("-------- IS SDK INITILIZATION COMPLETE--------");
        isSdkInitialized = true;
        if (showBannerOnIntialization)
        {
            ShowBanner();
        }
        Invoke(nameof(LoadInterstitial),2f);
        Invoke(nameof(LoadRewardedAd), 3f);
#if USE_IS_NATIVE_MREC
        if (shouldLoadMrecOnIntialization)
        {
            Invoke(nameof(LoadMrecBanner), 10f);
        }
#endif
#if USE_IS_SECOND_INTERSITITIAL
        if (shouldLoadSecondIntersititialOnIntialization)
        {
            Invoke(nameof(LoadSecondInterstitial), 5f);
        }
#endif

    }

   
#endif

    #region New Plugin Methods

    public void ShowBannerAd(BannerAdTypes type)
    {
        if (AdConstants.shouldDisplayAds() == false)
            return;

        switch (type)
        {
            case BannerAdTypes.BANNER:
                if (callOnlyAdmob)
                {
                    if (AdmobAdController.Instance)
                    {
                        AdmobAdController.Instance.ShowBannerAd(BannerAdTypes.BANNER_ADMOB);
                    }
                }
                else
                {
                    ShowBanner();
                }
                break;
            case BannerAdTypes.BANNER_ADMOB:
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.ShowBannerAd(BannerAdTypes.BANNER_ADMOB);
                }
                break;
            case BannerAdTypes.ADAPTIVE_ADMOB:
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.ShowBannerAd(BannerAdTypes.BANNER_ADMOB);
                }
                break;
            case BannerAdTypes.ADAPTIVE:
                if (callOnlyAdmob)
                {
                    if (AdmobAdController.Instance)
                    {
                        AdmobAdController.Instance.ShowBannerAd(BannerAdTypes.BANNER_ADMOB);
                    }
                }
                else
                {
                    ShowBanner();
                }
                break;
            case BannerAdTypes.Mrec:
                if (callOnlyAdmob)
                {
                    if (AdmobAdController.Instance)
                    {
                        AdmobAdController.Instance.ShowBannerAd(BannerAdTypes.MREC_ADMOB);
                    }
                }
                else
                {
                    ShowMrecBanner();
                }
                break;
            case BannerAdTypes.MREC_ADMOB:
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.ShowBannerAd(BannerAdTypes.MREC_ADMOB);
                }
                break;
        }
    }

    public void DestroyBannerAd(BannerAdTypes type)
    {
        switch (type)
        {
            case BannerAdTypes.BANNER:
                if (callOnlyAdmob)
                {
                    if (AdmobAdController.Instance)
                    {
                        AdmobAdController.Instance.DestroyBannerAd(BannerAdTypes.BANNER_ADMOB);
                    }
                }
                else
                {
                    HideBanner();
                }
                break;
            case BannerAdTypes.BANNER_ADMOB:
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.DestroyBannerAd(BannerAdTypes.BANNER_ADMOB);
                }
                break;
            case BannerAdTypes.ADAPTIVE_ADMOB:
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.DestroyBannerAd(BannerAdTypes.BANNER_ADMOB);
                }
                break;
            case BannerAdTypes.ADAPTIVE:
                if (callOnlyAdmob)
                {
                    if (AdmobAdController.Instance)
                    {
                        AdmobAdController.Instance.DestroyBannerAd(BannerAdTypes.BANNER_ADMOB);
                    }
                }
                else
                {
                    HideBanner();
                }
                break;
            case BannerAdTypes.Mrec:
                if (callOnlyAdmob)
                {
                    if (AdmobAdController.Instance)
                    {
                        AdmobAdController.Instance.DestroyBannerAd(BannerAdTypes.MREC_ADMOB);
                    }
                }
                else
                {
                    HideMrecBanner();
                }
                break;
            case BannerAdTypes.MREC_ADMOB:
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.DestroyBannerAd(BannerAdTypes.MREC_ADMOB);
                }
                break;
            default:
                break;
        }
    }

    public void HideBannerAd(BannerAdTypes type)
    {

        switch (type)
        {
            case BannerAdTypes.BANNER:
                if (callOnlyAdmob)
                {
                    if (AdmobAdController.Instance)
                    {
                        AdmobAdController.Instance.HideBannerAd(BannerAdTypes.BANNER_ADMOB);
                    }
                }
                else
                {
                    HideBanner();
                }
                break;
            case BannerAdTypes.BANNER_ADMOB:
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.HideBannerAd(BannerAdTypes.BANNER_ADMOB);
                }
                break;
            case BannerAdTypes.ADAPTIVE_ADMOB:
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.HideBannerAd(BannerAdTypes.BANNER_ADMOB);
                }
                break;
            case BannerAdTypes.ADAPTIVE:
                if (callOnlyAdmob)
                {
                    if (AdmobAdController.Instance)
                    {
                        AdmobAdController.Instance.HideBannerAd(BannerAdTypes.BANNER_ADMOB);
                    }
                }
                else
                {
                    HideBanner();
                }
                break;

            case BannerAdTypes.Mrec:
                if (callOnlyAdmob)
                {
                    if (AdmobAdController.Instance)
                    {
                        AdmobAdController.Instance.HideBannerAd(BannerAdTypes.MREC_ADMOB);
                    }
                }
                else
                {
                    HideMrecBanner();
                }
                break;
            case BannerAdTypes.MREC_ADMOB:
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.HideBannerAd(BannerAdTypes.MREC_ADMOB);
                }
                break;
            default:
                break;
        }
    }

    public void LoadAd(AdType type)
    {
        switch (type)
        {
            case AdType.STATIC:
                if (AdConstants.shouldDisplayAds() == false)
                    return;
#if USE_ADMOB_STATIC_AD
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.loadAd(AdType.STATIC_ADMOB);
                }
#else
                LoadInterstitial();
#endif
                break;

            case AdType.INTERSTITIAL:
                if (AdConstants.shouldDisplayAds() == false)
                    return;
                if (callOnlyAdmob)
                {
                    if (AdmobAdController.Instance)
                    {
                        AdmobAdController.Instance.loadAd(AdType.INTERSTITIAL_ADMOB);
                    }
                }
                else
                {
                    LoadInterstitial();
                }
                break;
            case AdType.SECOND_INTERSTITIAL:
                if (AdConstants.shouldDisplayAds() == false)
                    return;
                LoadSecondInterstitial();
                break;
            case AdType.INTERSTITIAL_ADMOB:
                if (AdConstants.shouldDisplayAds() == false)
                    return;
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.loadAd(AdType.INTERSTITIAL_ADMOB);
                }
                break;

            case AdType.REWARDED:
                Debug.Log("IS RW loading is AUTO");
                if (callOnlyAdmob && AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.loadAd(AdType.REWARD_ADMOB);
                }
                break;
        }
    }

    public void ShowAd(AdType type)
    {


        switch (type)
        {
            case AdType.STATIC:
                if (AdConstants.shouldDisplayAds() == false)
                    return;
#if USE_ADMOB_STATIC_AD
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.showAd(AdType.STATIC_ADMOB);
                }
#else
                ShowInterstitial();
#endif
                break;
            case AdType.STATIC_ADMOB:
                if (AdConstants.shouldDisplayAds() == false)
                    return;
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.showAd(AdType.STATIC_ADMOB);
                }
                break;

            case AdType.INTERSTITIAL:
                if (AdConstants.shouldDisplayAds() == false)
                    return;
                if (callOnlyAdmob)
                {
                    if (AdmobAdController.Instance)
                    {
                        AdmobAdController.Instance.showAd(AdType.INTERSTITIAL_ADMOB);
                    }
                }
                else
                {
                    ShowInterstitial();
                }
                break;
            case AdType.INTERSTITIAL_ADMOB:
                if (AdConstants.shouldDisplayAds() == false)
                    return;
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.showAd(AdType.INTERSTITIAL_ADMOB);
                }
                break;
            case AdType.SECOND_INTERSTITIAL:
                if (AdConstants.shouldDisplayAds() == false)
                    return;
                ShowSecondInterstitial();
                break;

            case AdType.REWARDED:
                if (callOnlyAdmob)
                {
                    if (AdmobAdController.Instance)
                    {
                        AdmobAdController.Instance.showAd(AdType.REWARD_ADMOB);
                    }
                }
                else
                {
                    ShowRewardedAd();
                }
                break;
            case AdType.REWARD_ADMOB:
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.showAd(AdType.REWARD_ADMOB);
                }
                break;
            case AdType.REWARDED_INTERSTITIAL_ADMOB:
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.showAd(AdType.REWARDED_INTERSTITIAL_ADMOB);
                }
                break;
        }

    }

    public void ShowAd(AdType type, float timeDelay)
    {
        StartCoroutine(AdDelaySetup(type, null, timeDelay));

    }

    public void ShowAd(AdType type, Action actionBeforeAd, float timeDelay)
    {
        StartCoroutine(AdDelaySetup(type, actionBeforeAd, timeDelay));

    }

    IEnumerator AdDelaySetup(AdType type, Action actionBeforeAd = null, float timeDelay = 0)
    {

        if (actionBeforeAd != null)
        {
            actionBeforeAd.Invoke();
        }
        yield return new WaitForSeconds(timeDelay);
        ShowAd(type);

    }

    #endregion

    #region Banner Ad Methods

    private void ShowBanner()
    {
        Debug.Log("-------- IS Banner show called--------");

        if (!AdConstants.shouldDisplayAds())
        {
            Debug.Log("-------- Ads Disabled--------");
            return;
        }

#if USE_IS




        if (bannerAd != null)
        {
            Debug.Log("-------- Showing banner ad --------");
            bannerAd.ShowAd();
            bannerAd.ResumeAutoRefresh();
        }
        else
        {

            LoadBanner();
        }


#endif
    }

    private void LoadBanner()
    {
     
#if USE_IS
        if (isBannerAdisbeingLoaded)
        {
            Debug.Log("-------- loading Banner ad --------");
            return;
        }
        if (!canLoadBanner) {
            return;
        }
        if (bannerAd != null) {
            bannerAd.DestroyAd();
            bannerAd = null;
        }
        bannerAd = null;

        var configBuilder = new LevelPlayBannerAd.Config.Builder();

        configBuilder.SetSize(LevelPlayAdSize.BANNER);
        if (isAdaptiveBanner)
        {
            configBuilder.SetSize(LevelPlayAdSize.CreateAdaptiveAdSize(Screen.width));
        }
        configBuilder.SetPosition(getBannerPosition(bannerPosition));
        configBuilder.SetRespectSafeArea(shouldBannerIgnoreNotchArea);
        configBuilder.SetDisplayOnLoad(showBannerOnIntialization || showBannerOnLoadSuccess);
        var bannerConfig = configBuilder.Build();
        bannerAd = new LevelPlayBannerAd(AdsIds.BannerAdUnitId(), bannerConfig);

        bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
        bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
        bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
        bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
        bannerAd.OnAdClicked += BannerOnAdClickedEvent;
        bannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
        bannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
        bannerAd.OnAdExpanded += BannerOnAdExpandedEvent;

        // Ad load
        bannerAd.LoadAd();
        isBannerAdisbeingLoaded = true;
#endif
    }

    private void HideBanner()
    {
#if USE_IS
        isBannerAdShowing = false;
        Debug.Log("-------- Hiding Banner ad --------");
        if (bannerAd != null)
        {
            bannerAd.HideAd();
            bannerAd.PauseAutoRefresh();
        }
#endif
    }

    private void ToggleBannerVisibility(bool show)
    {

#if USE_IS
        if (isBannerAdShowing)
        {
            if (show)
            {
                ShowBanner();
            }
            else
            {

                HideBanner();
            }
        }
#endif
    }

    public bool IsBannerAdAvailable()
    {
#if USE_IS
        return AdConstants.shouldDisplayAds() && AdConstants.showBannerAd && !isBannerAdisbeingLoaded;
#else
        return false;

#endif
    }

    public bool IsBannerAdReadyToLoad()
    {
#if USE_IS
        return AdConstants.shouldDisplayAds() && AdConstants.showBannerAd;
#else
        return false;

#endif
    }

#if USE_IS


    /************* Banner AdInfo Delegates *************/
    //Invoked once the banner has loaded
    void BannerOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        canLoadBanner = false;
        Debug.Log("-------- Banner ad Loaded ");
        if (showBannerOnIntialization)
        {
            bannerAd.ShowAd();
            showBannerOnIntialization = false;
        }
        if (isAdaptiveBanner)
        {
            if (FinzAnalysisManager.Instance)
                FinzAnalysisManager.Instance.AdAnalysis(BannerAdTypes.ADAPTIVE);
        }
        else
        {
            if (FinzAnalysisManager.Instance)
                FinzAnalysisManager.Instance.AdAnalysis(BannerAdTypes.BANNER);
        }

        canDestoryBanner = false;
        isBannerAdisbeingLoaded = false;
        isBannerAdShowing = true;
        bannerRetryAttempt = 0;
        BannerAdLoadingSuccessful();
    }
    //Invoked when the banner loading process has failed.
    void BannerOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        Debug.Log("-------- Banner ad failed to load with error code: " + error.ErrorMessage + " --------");
        isBannerAdisbeingLoaded = false;
        isBannerAdShowing = false;
        bannerRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, bannerRetryAttempt));
        if (canDestoryBanner)
        {
            Invoke("LoadBanner", (float)retryDelay);
        }
        showBannerOnIntialization = false;
        BannerLoadingFailed();
    }
    // Invoked when end user clicks on the banner ad
    void BannerOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
    }
    //Notifies the presentation of a full screen content following user click
    void BannerOnAdScreenPresentedEvent(LevelPlayAdInfo adInfo)
    {
    }
    //Invoked when the user leaves the app
    void BannerOnAdLeftApplicationEvent(LevelPlayAdInfo adInfo)
    {
    }

    void BannerOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdDisplayedEvent With AdInfo " + adInfo);
    }

    void BannerOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        Debug.Log($"[LevelPlaySample] Received BannerOnAdDisplayFailedEvent With AdInfo: {adInfo} and Error: {error}");
    }

    void BannerOnAdCollapsedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdCollapsedEvent With AdInfo " + adInfo);
    }

    void BannerOnAdExpandedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdExpandedEvent With AdInfo " + adInfo);
    }

#endif
    #endregion

    #region Mrec Ad Methods

    private void ShowMrecBanner()
    {
        Debug.Log("-------- Mrec Banner show called--------");

        if (!AdConstants.shouldDisplayAds())
        {
            Debug.Log("-------- Mrec Ads Disabled--------");
            return;
        }

#if USE_IS_NATIVE_MREC

        Debug.Log($" -- Mrec nativer used - - {isMrecBannerAdShowing}");
        if (mrecBannerAd != null)
        {
            Debug.Log("-------- Showing MREC banner ad --------");
            if (shouldHideBannerOnMrec)
            {
                HideBannerAd(BannerAdTypes.BANNER);
            }
            mrecBannerAd.ShowAd();
            mrecBannerAd.ResumeAutoRefresh();
        }
        else
        {
            Debug.Log($" -- Mrec null - - {mrecBannerAd == null}");
            if (mrecBannerAd != null)
            {
                if (shouldHideBannerOnMrec)
                {
                    HideBannerAd(BannerAdTypes.BANNER);
                }
                mrecBannerAd.ShowAd();
            }
            else
            {
                //LoadMrecBanner();
            }
        }

        //}
#endif
    }

    private void LoadMrecBanner()
    {
#if USE_IS_NATIVE_MREC
        if (!canLoadMrec || isMrecBannerAdisbeingLoaded)
            return;

        // IMPORTANT: Destroy the old ad object before creating a new one
        if (mrecBannerAd != null)
        {
            mrecBannerAd.DestroyAd();
            mrecBannerAd = null;
        }

        var configBuilder = new LevelPlayBannerAd.Config.Builder();
        configBuilder.SetSize(LevelPlayAdSize.MEDIUM_RECTANGLE);
        configBuilder.SetPosition(getBannerPosition(mrecBannerPosition));
        configBuilder.SetDisplayOnLoad(false); // Keeps it hidden initially

        var bannerConfig = configBuilder.Build();
        mrecBannerAd = new LevelPlayBannerAd(AdsIds.MRECBannerAdUnitId(), bannerConfig);

        // ... (Your event registrations) ...

        mrecBannerAd.LoadAd();
        isMrecBannerAdisbeingLoaded = true;
#endif
    }

    private void HideMrecBanner()
    {
#if USE_IS_NATIVE_MREC
        if (mrecBannerAd != null)
        {
            mrecBannerAd.PauseAutoRefresh(); // Stop it from fetching new ads while hidden
            mrecBannerAd.HideAd();
            isMrecBannerAdShowing = false;
        }
#endif
    }

    //     private void HideMrecBanner()
    //     {
    // #if USE_IS_NATIVE_MREC

    //         // isMrecBannerAdShowing = false;
    //         Debug.Log("--------  mrec Hiding Banner ad --------");
    //         if (mrecBannerAd != null)
    //         {
    //             mrecBannerAd.PauseAutoRefresh();
    //             mrecBannerAd.HideAd();
    //         }

    //         // mrecBannerAd = null;

    // #endif
    //     }

    private void ToggleMrecBannerVisibility(bool show)
    {
        //#if USE_IS_NATIVE_MREC
        //        if (isMrecBannerAdShowing)
        //        {
        //            if (show)
        //            {
        //                ShowMrecBanner();
        //            }
        //            else
        //            {

        //                HideMrecBanner();
        //            }
        //        }
        //#endif
    }

    public bool IsMrecBannerAdAvailable()
    {
#if USE_IS_NATIVE_MREC
        return AdConstants.shouldDisplayAds() && !isMrecBannerAdisbeingLoaded;
#else
        return false;

#endif
    }

    private bool IsMrecBannerAdReadyToLoad()
    {
#if USE_IS_NATIVE_MREC
        return AdConstants.shouldDisplayAds();
#else
        return false;

#endif
    }

#if USE_IS_NATIVE_MREC

    /************* Banner AdInfo Delegates *************/
    //Invoked once the banner has loaded
    void mrecBannerOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        canLoadMrec = false;
        Debug.Log("-------- Mrec Banner ad Loaded ");

        if (FinzAnalysisManager.Instance)
            FinzAnalysisManager.Instance.AdAnalysis(BannerAdTypes.Mrec);

        isMrecBannerAdisbeingLoaded = false;
        isMrecBannerAdShowing = true;
        mrecBannerRetryAttempt = 0;
        mrecBannerAd.PauseAutoRefresh();
    }

    //Invoked when the banner loading process has failed.
    void mrecBannerOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        mrecBannerAd = null;
        Debug.Log(
            "-------- Mrec Banner ad failed to load with error code: "
                + error.ErrorMessage
                + " --------"
        );
        isMrecBannerAdisbeingLoaded = false;
        isMrecBannerAdShowing = false;
        mrecBannerRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, mrecBannerRetryAttempt));

        // Invoke("LoadMrecBanner", (float)retryDelay);
    }

    // Invoked when end user clicks on the banner ad
    void mrecBannerOnAdClickedEvent(LevelPlayAdInfo adInfo) { }

    //Notifies the presentation of a full screen content following user click
    void mrecBannerOnAdScreenPresentedEvent(LevelPlayAdInfo adInfo) { }

    //Invoked when the user leaves the app
    void mrecBannerOnAdLeftApplicationEvent(LevelPlayAdInfo adInfo) { }

    void mrecBannerOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdDisplayedEvent With AdInfo " + adInfo);
    }

    void mrecBannerOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        Debug.Log(
            $"[LevelPlaySample] Received BannerOnAdDisplayFailedEvent With AdInfo: {adInfo} and Error: {error}"
        );
    }

    void mrecBannerOnAdCollapsedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdCollapsedEvent With AdInfo " + adInfo);
    }

    void mrecBannerOnAdExpandedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdExpandedEvent With AdInfo " + adInfo);
    }

#endif
    #endregion
    #region Interstitial Ad Methods
#if USE_IS

    private void InitializeInterstitialAds()
    {
#if THREE_TIER_INTER
        currentInterstitialTier = 1;
#endif
        ResetInterstitialTime();
        isInterstitialAdisbeingLoaded = false;
    }

#endif
    private void updateInterstitialTierCount()
    {
#if THREE_TIER_INTER && USE_IS
        if (currentInterstitialTier == 0)
        {
            currentInterstitialTier = 1;
        }
        else if (currentInterstitialTier == 1)
        {
            currentInterstitialTier = 2;
        }
        else if (currentInterstitialTier == 2)
        {
            currentInterstitialTier = 3;
        }
        else if (currentInterstitialTier == 3)
        {
            currentInterstitialTier = 1;
        }
        else
        {
            currentInterstitialTier = 3;
        }
#endif
    }
    void LoadInterstitial()
    {
        if (AdConstants.shouldDisplayAds() == false)
            return;
#if USE_IS
        if (!isInterstitialAdisbeingLoaded)
        {
            isInterstitialAdisbeingLoaded = true;

#if THREE_TIER_INTER
            if (currentInterstitialTier == 1 || currentInterstitialTier == 0)
            {
                interstitialAd = new LevelPlayInterstitialAd(AdsIds.InterstitialAdUnitIdTier1());
            }
            else if (currentInterstitialTier == 2)
            {
                interstitialAd = new LevelPlayInterstitialAd(AdsIds.InterstitialAdUnitIdTier2());
            }
            else if (currentInterstitialTier == 3)
            {
                interstitialAd = new LevelPlayInterstitialAd(AdsIds.InterstitialAdUnitIdTier3());
            }
            else
            {
                currentInterstitialTier = 3;
                interstitialAd = new LevelPlayInterstitialAd(AdsIds.InterstitialAdUnitIdTier3());
            }
#else
            interstitialAd = new LevelPlayInterstitialAd(AdsIds.InterstitialAdUnitId());
#endif

            // Register to events
            interstitialAd.OnAdLoaded += InterstitialOnAdReadyEvent;
            interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailed;
            interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
            interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
            interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
            interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
            interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;


            interstitialAd.LoadAd();
        }
#endif
    }

    void ShowInterstitial(bool shouldUseAdDelay = true)
    {


        if (AdConstants.isShowingLPIntersititialReward == false &&
            AdConstants.shouldDisplayAds() == false)
            return;


#if USE_IS

        if (interstitialAd != null && interstitialAd.IsAdReady())
        {

            if (AdConstants.isShowingLPIntersititialReward)
            {

                if (FinzAnalysisManager.Instance)
                    FinzAnalysisManager.Instance.AdAnalysis(AdType.LP_INTERSTITIAL_REWARDED);
            }
            else
            {

                if (shouldUseAdDelay && DateTime.Now <= currentTime_Interstitial.AddSeconds(AdConstants.adDelay))
                {
                    Debug.Log("-------- Interstitial Ad retur due to ad delay --------");
                    return;
                }
                if (FinzAnalysisManager.Instance)
                    FinzAnalysisManager.Instance.AdAnalysis(AdType.INTERSTITIAL);
            }

      
            AdConstants.resumeFromAds = true;

            interstitialAd.ShowAd();

        }
        else
        {
            if (AdConstants.isShowingLPIntersititialReward)
            {
                if (CallLpIntersititialAndAdmobIntersititial && AdmobAdController.Instance)
                {
#if USE_ADMOB_INTERSITITIAL_AD
                    AdmobAdController.Instance.showAd(AdType.INTERSTITIAL_ADMOB,false);
#else
                    NoAdAvailable();
                    AdConstants.isShowingLPIntersititialReward = false;
#endif
                }
                else
                {
                    NoAdAvailable();
                    AdConstants.isShowingLPIntersititialReward = false;
                }
            }
            else if (AdmobAdController.Instance && callAdmobIntersititialOnFail)
            {
                AdmobAdController.Instance.showAd(AdType.INTERSTITIAL_ADMOB);
            }
            else
            {

            }

            Debug.Log("-------- Interstitial Ad is not ready yet --------");
            LoadInterstitial();

        }
#endif
    }
#if USE_IS
    /************* Interstitial AdInfo Delegates *************/
    // Invoked when the interstitial ad was loaded succesfully.
    void InterstitialOnAdReadyEvent(LevelPlayAdInfo adInfo)
    {
#if THREE_TIER_INTER
        Debug.Log("-------- Interstitial loaded Tier " + currentInterstitialTier + " --------");
#else
        Debug.Log("-------- Interstitial loaded --------");
#endif
        isInterstitialAdisbeingLoaded = false;
        // Reset retry attempt
        interstitialRetryAttempt = 0;

    }
    // Invoked when the initialization process has failed.
    void InterstitialOnAdLoadFailed(LevelPlayAdError error)
    {
        interstitialRetryAttempt++;
        updateInterstitialTierCount();
        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));

        //interstitialStatusText.text = "Load failed: " + errorInfo.Code + "\nRetrying in " + retryDelay + "s...";
#if THREE_TIER_INTER
        Debug.Log("-------- Interstitial Tier " + currentInterstitialTier + " failed to load with error code:" + error.ErrorMessage + " --------");
#else
        Debug.Log("-------- Interstitial failed to load with error code: " + error.ErrorMessage + " --------");
#endif
        isInterstitialAdisbeingLoaded = false;
        Invoke("LoadInterstitial", (float)retryDelay);
    }
    // Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
    void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {

    }
    // Invoked when end user clicked on the interstitial ad
    void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdClickedEvent With AdInfo " + adInfo);
    }
    // Invoked when the ad failed to show.
    void InterstitialOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        Debug.Log($"[LevelPlaySample] Received InterstitialOnAdDisplayFailedEvent With AdInfo: {adInfo} and Error: {error}");
        updateInterstitialTierCount();
        AdConstants.resumeFromAds = false;
        // Interstitial ad failed to display. We recommend loading the next ad
#if THREE_TIER_INTER
        Debug.Log("-------- Interstitial Tier " + currentInterstitialTier + " failed to display with error code :" + error.ErrorMessage + " --------");
#else
        Debug.Log("-------- Interstitial failed to display with error code: " + error.ErrorMessage + " --------");
#endif

        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
        Invoke("LoadInterstitial", (float)retryDelay);
        if (AdConstants.isShowingLPIntersititialReward)
        {
            if (CallLpIntersititialAndAdmobIntersititial && AdmobAdController.Instance)
            {
#if USE_ADMOB_INTERSITITIAL_AD
                AdmobAdController.Instance.showAd(AdType.INTERSTITIAL_ADMOB,false);
#else
                NoAdAvailable();
                AdConstants.isShowingLPIntersititialReward = false;
#endif
            }
            else
            {
                NoAdAvailable();
                AdConstants.isShowingLPIntersititialReward = false;
            }
        }
        else if (callAdmobIntersititialOnFail)
        {
            if (AdmobAdController.Instance)
            {
                AdmobAdController.Instance.showAd(AdType.INTERSTITIAL_ADMOB);
            }
        }
    }
    void InterstitialOnAdShowFailedEvent(LevelPlayAdError infoError)
    {
       


    }
    // Invoked when the interstitial ad closed and the user went back to the application screen.
    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {

#if THREE_TIER_INTER
        currentInterstitialTier = 1;
#endif
#if USE_ADMOB_OPEN_AD_8_5
        if (FinzAdmobOpenAds.AppOpenAdLauncher.Instance && FinzAdmobOpenAds.AppOpenAdLauncher.Instance.shouldShowAoaAfterInter)
        {
            AppOpenAdManager.Instance.ShowAdIfAvailable();
        }
#endif
        AdConstants.resumeFromAds = true;
        AdConstants.IsAdWasShowing = true;
        // Interstitial ad is hidden. Pre-load the next ad
#if THREE_TIER_INTER
        Debug.Log("-------- Interstitial Tier " + currentInterstitialTier + " dismissed" + " --------");
#else
        Debug.Log("-------- Interstitial dismissed --------");
#endif
        ResetInterstitialTime(); // resetting time

        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
        Invoke("LoadInterstitial", (float)retryDelay);
        if (AdConstants.isShowingLPIntersititialReward)
        {
            AdConstants.sawRewarded = true;
            DecideForReward();
            AdConstants.isShowingLPIntersititialReward = false;
        }
    }

    void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdInfoChangedEvent With AdInfo " + adInfo);
    }
#endif

    #endregion

    #region Second Interstitial Ad Methods
#if USE_IS

    private void SecondInitializeInterstitialAds()
    {
#if USE_IS_SECOND_INTERSITITIAL
        ResetSecondInterstitialTime();
        isSecondInterstitialAdisbeingLoaded = false;
#endif
    }

#endif

    void LoadSecondInterstitial()
    {
        if (AdConstants.shouldDisplayAds() == false)
            return;
#if USE_IS_SECOND_INTERSITITIAL
        if (!isSecondInterstitialAdisbeingLoaded)
        {
            secondInterstitialAd = new LevelPlayInterstitialAd(AdsIds.SecondInterstitialAdUnitId());

            // Register to events
            secondInterstitialAd.OnAdLoaded += SecondInterstitialOnAdReadyEvent;
            secondInterstitialAd.OnAdLoadFailed += SecondInterstitialOnAdLoadFailed;
            secondInterstitialAd.OnAdDisplayed += SecondInterstitialOnAdDisplayedEvent;
            secondInterstitialAd.OnAdDisplayFailed += SecondInterstitialOnAdShowFailedEvent;
            secondInterstitialAd.OnAdClicked += SecondInterstitialOnAdClickedEvent;
            secondInterstitialAd.OnAdClosed += SecondInterstitialOnAdClosedEvent;
            secondInterstitialAd.OnAdInfoChanged += SecondInterstitialOnAdInfoChangedEvent;

            isSecondInterstitialAdisbeingLoaded = true;
            secondInterstitialAd.LoadAd();
        }
#endif
    }

    void ShowSecondInterstitial()
    {


        if (AdConstants.shouldDisplayAds() == false)
            return;


#if USE_IS_SECOND_INTERSITITIAL

        if (secondInterstitialAd != null && secondInterstitialAd.IsAdReady() && DateTime.Now >= currentTime_SecondInterstitial.AddSeconds(AdConstants.adDelay))
        {

            if (FinzAnalysisManager.Instance)
                FinzAnalysisManager.Instance.AdAnalysis(AdType.SECOND_INTERSTITIAL);
  
            AdConstants.resumeFromAds = true;
            secondInterstitialAd.ShowAd();

        }
        else
        {

            if (callAdmobIntersititialOnSecondFail)
            {
                if (AdmobAdController.Instance)
                {
                    AdmobAdController.Instance.showAd(AdType.INTERSTITIAL_ADMOB);
                }
            }
            Debug.Log("-------- Second Interstitial Ad is not ready yet --------");
            LoadSecondInterstitial();

        }
#endif
    }





#if USE_IS && USE_IS_SECOND_INTERSITITIAL
    /************* Interstitial AdInfo Delegates *************/
    // Invoked when the interstitial ad was loaded succesfully.
    void SecondInterstitialOnAdReadyEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("-------- Second Interstitial loaded --------");
        isSecondInterstitialAdisbeingLoaded = false;
        // Reset retry attempt
        secondInterstitialRetryAttempt = 0;

    }
    // Invoked when the initialization process has failed.
    void SecondInterstitialOnAdLoadFailed(LevelPlayAdError error)
    {
        secondInterstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, secondInterstitialRetryAttempt));

        //interstitialStatusText.text = "Load failed: " + errorInfo.Code + "\nRetrying in " + retryDelay + "s...";
        Debug.Log("-------- Second Interstitial failed to load with error code: " + error.ErrorMessage + " --------");
        isSecondInterstitialAdisbeingLoaded = false;
        Invoke("LoadSecondInterstitial", (float)retryDelay);
    }
    // Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
    void SecondInterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {

    }
    // Invoked when end user clicked on the interstitial ad
    void SecondInterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got Second InterstitialOnAdClickedEvent With AdInfo " + adInfo);
    }


    // Invoked when the ad failed to show.
    void SecondInterstitialOnAdShowFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        AdConstants.resumeFromAds = false;
        // Interstitial ad failed to display. We recommend loading the next ad
        Debug.Log("-------- Second Interstitial failed to display with error code: " + error.ErrorMessage + " --------");
   
        double retryDelay = Math.Pow(2, Math.Min(6, secondInterstitialRetryAttempt));
        Invoke("LoadSecondInterstitial", (float)retryDelay);
        if (callAdmobIntersititialOnSecondFail)
        {
            if (AdmobAdController.Instance)
            {
                AdmobAdController.Instance.showAd(AdType.INTERSTITIAL_ADMOB);
            }
        }


    }
    // Invoked when the interstitial ad closed and the user went back to the application screen.
    void SecondInterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        AdConstants.resumeFromAds = true;
        AdConstants.IsAdWasShowing = true;
        // Interstitial ad is hidden. Pre-load the next ad
        Debug.Log("-------- Interstitial dismissed --------");
        ResetSecondInterstitialTime(); // resetting time

        double retryDelay = Math.Pow(2, Math.Min(6, secondInterstitialRetryAttempt));
        Invoke("LoadSecondInterstitial", (float)retryDelay);
    }

    void SecondInterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got SecondInterstitialOnAdInfoChangedEvent With AdInfo " + adInfo);
    }
#endif

    #endregion

    #region Rewarded Ad Methods
#if USE_IS
    private void InitializeRewardedAds()
    {
#if THREE_TIER_REWARD
        currentRewardedTier = 1;
#endif
        ResetRewardedTime();
        isRewardedAdisbeingLoaded = false;
    }
#endif
    private void updateRewardTierCount()
    {
#if USE_IS
#if THREE_TIER_REWARD
        if (currentRewardedTier == 0)
        {
            currentRewardedTier = 1;
        }
        else if (currentRewardedTier == 1)
        {
            currentRewardedTier = 2;
        }
        else if (currentRewardedTier == 2)
        {
            currentRewardedTier = 3;
        }
        else if (currentRewardedTier == 3)
        {
            currentRewardedTier = 1;
        }
        else
        {
            currentRewardedTier = 3;
        }
#endif
#endif
    }
    public void resetRewardedAdisbeingLoaded()
    {
#if USE_IS
        isRewardedAdisbeingLoaded = false;
#endif
    }
    private void LoadRewardedAd()
    {

        if (AdConstants.shouldDisplayAds() == false)
            return;
#if USE_IS

        if (!isRewardedAdisbeingLoaded)
        {
            isRewardedAdisbeingLoaded = true;

#if THREE_TIER_REWARD
            if (currentRewardedTier == 1 || currentRewardedTier == 0)
            {
                rewardedVideoAd = new LevelPlayRewardedAd(AdsIds.RewardedAdUnitIdTier1());
            }
            else if (currentRewardedTier == 2)
            {
                rewardedVideoAd = new LevelPlayRewardedAd(AdsIds.RewardedAdUnitIdTier2());
            }
            else if (currentRewardedTier == 3)
            {
                rewardedVideoAd = new LevelPlayRewardedAd(AdsIds.RewardedAdUnitIdTier3());
            }
            else
            {
                currentRewardedTier = 3;
                rewardedVideoAd = new LevelPlayRewardedAd(AdsIds.RewardedAdUnitId());
            }
#else
            rewardedVideoAd = new LevelPlayRewardedAd(AdsIds.RewardedAdUnitId());
#endif



            Debug.Log("-------- Rewarded Ad loading --------");
            // Register to Rewarded Video events
            rewardedVideoAd.OnAdLoaded += RewardedVideoOnLoadedEvent;
            rewardedVideoAd.OnAdLoadFailed += RewardedVideoOnAdLoadFailedEvent;
            rewardedVideoAd.OnAdDisplayed += RewardedVideoOnAdDisplayedEvent;
            rewardedVideoAd.OnAdDisplayFailed += RewardedVideoOnAdDisplayedFailedEvent;

            rewardedVideoAd.OnAdRewarded += RewardedVideoOnAdRewardedEvent;
            rewardedVideoAd.OnAdClicked += RewardedVideoOnAdClickedEvent;
            rewardedVideoAd.OnAdClosed += RewardedVideoOnAdClosedEvent;
            rewardedVideoAd.OnAdInfoChanged += RewardedVideoOnAdInfoChangedEvent;
            rewardedVideoAd.LoadAd();
        }
        else
        {
            Debug.Log("-------- Rewarded Ad already loading --------");
        }
#endif
    }
    private void ShowRewardedAd()
    {
        AdConstants.sawRewarded = false;
#if USE_IS

        AdConstants.isShowingLPIntersititialReward = false;
        if (IsRewardedAdAvailable())
        {

            if (FinzAnalysisManager.Instance)
                FinzAnalysisManager.Instance.AdAnalysis(AdType.REWARDED);
      
            AdConstants.resumeFromAds = true;
            rewardedVideoAd.ShowAd();
        }
        else
        {
            Debug.Log("-------- Rewarded Ad is not ready yet --------");
            if (CallLpIntersititial || CallLpIntersititialAndAdmobIntersititial)
            {
                AdConstants.isShowingLPIntersititialReward = true;
                ShowInterstitial(false);
            }
            else
            {
                if (CallAdmobReward)
                {
#if USE_ADMOB_REWARD_AD
                    if (AdmobAdController.Instance)
                    {
                        AdmobAdController.Instance.showAd(AdType.REWARD_ADMOB);
                    }
#else
                    NoAdAvailable();
#endif
                }
                else
                {
                    NoAdAvailable();
                }
            }

            LoadRewardedAd();
        }
#endif
    }
    public bool IsRewardedAdAvailable()
    {
#if USE_IS
        return rewardedVideoAd!=null&& rewardedVideoAd.IsAdReady() && AdConstants.showRewardedAd && DateTime.Now >= currentTime_Rewarded.AddSeconds(AdConstants.AdDelayReward);
#else
        return false;

#endif
    }
    public bool IsRewardedAdReadyToLoad()
    {
#if USE_IS
        return AdConstants.showRewardedAd && !isRewardedAdisbeingLoaded;
#else
        return false;

#endif
    }

#if USE_IS

    void RewardedVideoOnLoadedEvent(LevelPlayAdInfo adInfo)
    {


#if THREE_TIER_REWARD
        Debug.Log("-------- Rewarded loaded Tier " + currentRewardedTier + " --------");
#else
        Debug.Log("-------- Rewarded ad loaded --------");
#endif

        Debug.Log("-------- Rewarded ad loaded --------");
        isRewardedAdisbeingLoaded = false;
        RewardedAdLoaded();
        rewardedRetryAttempt = 0;
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnLoadedEvent With AdInfo: {adInfo}");
    }

    void RewardedVideoOnAdLoadFailedEvent(LevelPlayAdError error)
    {
       
        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));

#if THREE_TIER_REWARD
        Debug.Log("-------- Reward Tier " + currentTime_Rewarded + " failed to load with error code:" + error.ErrorMessage + " --------");
#else
        Debug.Log("-------- Reward failed to load with error code: " + error.ErrorMessage + " --------");
#endif
        isRewardedAdisbeingLoaded = false;
        Invoke("LoadRewardedAd", (float)retryDelay);
        updateRewardTierCount();
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdLoadFailedEvent With Error: {error}");
    }

    void RewardedVideoOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdDisplayedEvent With AdInfo: {adInfo}");
    }
    void RewardedVideoOnAdDisplayedFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));
        Invoke("LoadRewardedAd", (float)retryDelay);


        AdConstants.resumeFromAds = false;
        // Rewarded ad failed to display. We recommend loading the next ad
#if THREE_TIER_REWARD
        Debug.Log("-------- Rewarded ad Tier "+currentRewardedTier+" failed to display"+ error+" --------");
#else
        Debug.Log("-------- Rewarded ad failed to display --------");
#endif
        updateRewardTierCount();


        if (CallLpIntersititial || CallLpIntersititialAndAdmobIntersititial)
        {
            AdConstants.isShowingLPIntersititialReward = true;
            ShowInterstitial(false);
        }
        else if (CallAdmobReward)
        {
#if USE_ADMOB_REWARD_AD
            if (AdmobAdController.Instance)
            {
                AdmobAdController.Instance.showAd(AdType.REWARD_ADMOB);
            }
#else
            NoAdAvailable();
#endif
        }
        else
        {
            NoAdAvailable();
        }
        RewardedAdShowingFailed();
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdDisplayedFailedEvent With AdInfo: {adInfo} and Error: {error}");
    }
#pragma warning disable 0618
    void RewardedVideoOnAdDisplayedFailedEvent(LevelPlayAdError error)
    {
       
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdDisplayedFailedEvent With Error: {error}");
    }
#pragma warning restore 0618
    void RewardedVideoOnAdRewardedEvent(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        AdConstants.sawRewarded = true;
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdRewardedEvent With AdInfo: {adInfo} and Reward: {reward}");
    }

    void RewardedVideoOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdClickedEvent With AdInfo: {adInfo}");
    }

    void RewardedVideoOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {

#if THREE_TIER_REWARD
        Debug.Log("-------- Rewarded ad Tier " + currentRewardedTier + " ad dismissed " + " --------");
#else
     Debug.Log("-------- Rewarded ad dismissed --------");
#endif
        rewardedRetryAttempt=1;
        double retryDelay = Math.Pow(2, Math.Min(2, rewardedRetryAttempt));
        Invoke("LoadRewardedAd", (float)retryDelay);

        AdConstants.resumeFromAds = true;
        // Rewarded ad is hidden. Pre-load the next ad

        Debug.Log("-------- Rewarded ad dismissed --------");
        ResetRewardedTime(); // resetting time
        AdConstants.IsAdWasShowing = true;
        DecideForReward();
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdClosedEvent With AdInfo: {adInfo}");
    }

    void RewardedVideoOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdInfoChangedEvent With AdInfo {adInfo}");
    }

#endif

#endregion

        #region Paid Impression
#if USE_IS
        public void InitializePaidAdEvent()
    {
        LevelPlay.OnImpressionDataReady += ImpressionDataReadyEvent;

    }
    void ImpressionDataReadyEvent(LevelPlayImpressionData impressionData)
    {
        if (FinzAnalysisManager.instance)
        {
            FinzAnalysisManager.instance.PaidAdAnalytics(impressionData);
        }
    }
#endif
    #endregion

    #region RATE US
    IEnumerator showingIOSRate(bool ratePopUp = false)
    {
#if UNITY_IOS

        Ping googPing = new Ping("8.8.8.8");
        // keep in mind that 'Ping' only accepts IP addresses, it doesn't 
        // do DNS lookup. This address may not work for your location-
        // Google owns many servers the world over.


        while (!googPing.isDone)
        {
            yield return new WaitForSecondsRealtime(2);
        }
        // Debug.Log(googPing.time);

        if (googPing.time > 0 && googPing.time < 500)
        {
            yield return new WaitForSecondsRealtime(1);

            if (UnityEngine.iOS.Device.RequestStoreReview())
            {
                if (ratePopUp) // this is for controlling time scale
                    Instantiate(Resources.Load("IOSRateMenuSupport"));

            }
        }


#else
        yield return null;
#endif

    }

    public void PromptRateMenu(bool ratePopUp = false)
    {

#if UNITY_IOS
        // Show Rate us here
        if (AdConstants.GetInternetStatus())
        {
            // Debug.LogWarning("$$Tag: Rate us called " );
            StartCoroutine(showingIOSRate(ratePopUp));

        }
#elif UNITY_ANDROID //&& !UNITY_EDITOR

        if (AdConstants.shouldDisplayRateMenu())
        {
            DestroyBannerAd(BannerAdTypes.Mrec);
#if !SHOW_NATIVE_RATE
            if (reviewDialogMethod != null)
                reviewDialogMethod();
#elif SHOW_NATIVE_RATE
            AdController.instance?.iapInstance.showRateMenu();
#endif


        }

#endif


    }

    #endregion

    #region User Callback


    private void BannerAdLoadingSuccessful()
    {
#if USE_DELEGATES
        if (bannerLoadingSuccessfulMethod != null)
        {
            bannerLoadingSuccessfulMethod.Invoke();
        }
#endif
    }

    private void BannerLoadingFailed()
    {
#if USE_DELEGATES

        if (bannerLoadingFailedMethod != null)
        {
            bannerLoadingFailedMethod.Invoke();
        }
#endif
    }






    public void DecideForReward()
    {

        if (AdConstants.sawRewarded)
        {
            Debug.Log("DecideForReward GaveReward");
            GaveReward();
        }
        else
        {
            Debug.Log("DecideForReward CancelReward");
            CancelReward();
        }



    }

    public void RewardedAdLoaded()
    {
#if USE_DELEGATES

        if (rewardedAdLoadMethod != null)
        {
            rewardedAdLoadMethod.Invoke();
        }
#endif
    }


    private void RewardedAdLoadFailed()
    {
#if USE_DELEGATES

        if (rewardedAdLoadFailedMethod != null)
        {
            rewardedAdLoadFailedMethod.Invoke();
        }
#endif
    }

    private void RewardedAdShowing()
    {
#if USE_DELEGATES

        if (rewardedAdShowingMethod != null)
            rewardedAdShowingMethod.Invoke();
#endif
    }

    private void RewardedAdShowingFailed()
    {
#if USE_DELEGATES

        if (rewardedAdShowingFailedMethod != null)
            rewardedAdShowingFailedMethod.Invoke();
#endif
    }

    private void GaveReward()
    {
#if USE_IS
        if (shouldResetIntersititialTimeOnReward)
        {
            ResetInterstitialTime();
        }
#endif

        Debug.Log("-------- Gave Reward --------");
        if (gaveRewardMethod != null)
            gaveRewardMethod();
    }

    private void CancelReward()
    {
        Debug.Log("-------- Reward Skipped --------");

        if (cancelRewardedAdMethod != null)
            cancelRewardedAdMethod();
    }


    public void NoAdAvailable()
    {
        Debug.Log("-------- No Rewarded Ad Available --------");

        if (noRewardedVideoMethod != null)
            noRewardedVideoMethod();
    }




#if USE_FIREBASE && USE_REMOTE_CONFIG
    public void OnFirebaseCompleteFetching(IDictionary<string, ConfigValue> fetchValue)
    {
        if (onFirebaseRemoteConfigSuccess != null)
        {
            onFirebaseRemoteConfigSuccess(fetchValue);
        }
    }
#endif

    #endregion

    #region Ads Delay Management

    public void ResetInterstitialTime()
    {
        currentTime_Interstitial = DateTime.Now;
    }
    public void ResetSecondInterstitialTime()
    {
        currentTime_SecondInterstitial = DateTime.Now;
    }

    private void ResetRewardedTime()
    {
        currentTime_Rewarded = DateTime.Now;
    }
    #endregion

#if USE_IS
    private LevelPlayBannerPosition getBannerPosition(FinzBannerPosition finzBannerPosition) {

        switch (finzBannerPosition)
        {
            case FinzBannerPosition.TopLeft:
                return LevelPlayBannerPosition.TopLeft;
                break;
            case FinzBannerPosition.TopCenter:
                return LevelPlayBannerPosition.TopCenter;
                break;
            case FinzBannerPosition.TopRight:
                return LevelPlayBannerPosition.TopRight;
                break;
            case FinzBannerPosition.CenterLeft:
                return LevelPlayBannerPosition.CenterLeft;
                break;
            case FinzBannerPosition.Center:
                return LevelPlayBannerPosition.Center;
                break;
            case FinzBannerPosition.CenterRight:
                return LevelPlayBannerPosition.CenterRight;
                break;
            case FinzBannerPosition.BottomLeft:
                return LevelPlayBannerPosition.BottomLeft;
                break;
            case FinzBannerPosition.BottomCenter:
                return LevelPlayBannerPosition.BottomCenter;
                break;
            case FinzBannerPosition.BottomRight:
                return LevelPlayBannerPosition.BottomRight;
                break;
            default:
                break;
        }
        return LevelPlayBannerPosition.BottomCenter;
    }
#endif

}


