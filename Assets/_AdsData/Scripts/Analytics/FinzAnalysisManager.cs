using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
#if USE_FIREBASE
using Firebase.Analytics;
using Firebase;
using System.Threading.Tasks;
using Firebase.Extensions;
#if USE_FIREBASE && USE_REMOTE_CONFIG
using Firebase.RemoteConfig;
#endif
#endif
#if USE_FIREBASE && USE_CRASHLYTICS
using Firebase.Crashlytics;

 
#endif
#if USE_ADMOB_PAID_EVENT
using GoogleMobileAds.Api;
#endif

#if USE_APPSFLYER || USE_APPSFLYER_6_15
using AppsFlyerSDK;
#endif

#if USE_FACEBOOK
using Facebook.Unity;
#endif

using UnityEngine.Analytics;

public class FinzAnalysisManager : MonoBehaviour
{
    // Variables
    #region Variables

    public static FinzAnalysisManager Instance;
    public static string RemoveAdsPriceString = "GET";

    public static string PremiumPriceString = "GET";

    // Property to access the instance
    public static FinzAnalysisManager instance
    {
        get
        {
            if (Instance == null)
            {
                Debug.LogError("FinzAnalysisManager instance is null. Make sure the instance is initialized before accessing it.");
            }

            return Instance;
        }
    }
    [Serializable]
    public class RemoteEvent : UnityEvent<FirebaseRemoteData>
    {
        // todo
    }
    [Serializable]
    public class FirebaseRemoteData
    {
        public enum DataType
        {
            NUMBER, STRING, BOOLEAN, JSON
        }
        public string name;
        public DataType type;
        [ShowIf("type", DataType.NUMBER)]
        public int DefaultValue_Number;
        [ShowIf("type", DataType.STRING)]
        public string DefaultValue_String;
        [ShowIf("type", DataType.BOOLEAN)]
        public bool DefaultValue_Boolean;
        [ShowIf("type", DataType.JSON)]
        public string DefaultValue_Json;
        public RemoteEvent onFetched=null;
    }
    public enum PaidAdEvent
    {
        ad_impression,
        new_ad_impression,
        both
    }
    public PaidAdEvent impressionType = PaidAdEvent.ad_impression;
    [Title("Firebase Remote Config")] // Remote config
    public List<FirebaseRemoteData> remote_Data = new List<FirebaseRemoteData>(); // array containing all the data of remote config
    private FirebaseRemoteData remote_adsSettings, remote_LowEndDevices;
    [Title("Firebase On Complete Processing Callback")] // Remote config
    public UnityEvent OnFirebaseInitialized; // Callback after firebase is Initialized
    [Title("Remote Config On Complete Processing Callback")] // Remote config
    public UnityEvent OnRemoteConfigInitialized; // Callback after firebase is Initialized
    [HideInInspector] public bool sendAnalytics = false;
    [Title("This make sure to keep one instance of ads Initializer keep this true")]

    #endregion

    // Initializations for analtytics
    #region Initialization

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
    }
    void Start()
    {
        firebaseAnalysis(); // Firebase Initialization calling




    }

    #region Firebase

    async void firebaseAnalysis()
    {

        try
        {
#if USE_FIREBASE

            IDictionary<Firebase.Analytics.ConsentType, Firebase.Analytics.ConsentStatus> consentValues = new Dictionary<Firebase.Analytics.ConsentType, Firebase.Analytics.ConsentStatus>
                            {
                                { Firebase.Analytics.ConsentType.AdUserData, Firebase.Analytics.ConsentStatus.Granted },
                                { Firebase.Analytics.ConsentType.AnalyticsStorage, Firebase.Analytics.ConsentStatus.Granted },
                                { Firebase.Analytics.ConsentType.AdPersonalization, Firebase.Analytics.ConsentStatus.Granted },
                                { Firebase.Analytics.ConsentType.AdStorage, Firebase.Analytics.ConsentStatus.Granted }
                                // Add more entries as needed
                            };

            Firebase.Analytics.FirebaseAnalytics.SetConsent(consentValues);
            Debug.Log("Checking Dependencies");
            DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
#if USE_CRASHLYTICS
            Crashlytics.IsCrashlyticsCollectionEnabled = true;
#endif
            if (dependencyStatus == DependencyStatus.Available)
            {
                var app = FirebaseApp.DefaultInstance;
                sendAnalytics = true;
                AdConstants.DebugGreen("=================> Firebase Initialized Successfully =================>");

            }
            else
            {
                Debug.Log("=================> Firebase Initialization Failed =================>");
                Invoke("firebaseAnalysis", 10f);
            }

#endif
        }
        catch (Exception e)
        {

        }
        if (OnFirebaseInitialized != null)
            OnFirebaseInitialized.Invoke();

    }

    #endregion



    #endregion

    // public Analytics Methods
    #region Analytics Methods

    #region Ads Analysis
    public void AdAnalysis(AdController.BannerAdTypes adType)
    {

        if (sendAnalytics == false)  // if adcontroller is not intialized or analytics are not enabled from user
            return;

        switch (adType)
        {
            case AdController.BannerAdTypes.BANNER:
                AdAnalysis("ad_banner");
                break;
            case AdController.BannerAdTypes.ADAPTIVE:
                AdAnalysis("ad_adaptive_banner");
                break;
            case AdController.BannerAdTypes.Mrec:
                AdAnalysis("ad_mrec");
                break;
            case AdController.BannerAdTypes.BANNER_ADMOB:
                AdAnalysis("ad_admob_banner");
                break;
            case AdController.BannerAdTypes.MREC_ADMOB:
                AdAnalysis("ad_admob_mrec");
                break;
            case AdController.BannerAdTypes.ADAPTIVE_ADMOB:
                AdAnalysis("ad_admob_adaptive_banner");
                break;
        }
    }


    public void AdAnalysis(AdController.AdType adType)
    {

        if (sendAnalytics == false)  // if adcontroller is not intialized or analytics are not enabled from user
            return;

        switch (adType)
        {
            case AdController.AdType.STATIC:
                AdAnalysis("ad_static");
                break;
            case AdController.AdType.STATIC_ADMOB:
                AdAnalysis("ad_admob_static");
                break;
            case AdController.AdType.INTERSTITIAL:
#if THREE_TIER_INTER && USE_IS
                AdAnalysis("ad_interstitial_tier_"+AdController.instance.currentInterstitialTier);
#else
                AdAnalysis("ad_interstitial");
#endif
                break;
            case AdController.AdType.INTERSTITIAL_ADMOB:
                AdAnalysis("ad_admob_interstitial");
                break;
            case AdController.AdType.REWARDED:
#if THREE_TIER_REWARD && USE_IS
                AdAnalysis("ad_rewarded_tier_" + AdController.instance.currentRewardedTier);
#else
                AdAnalysis("ad_rewarded");
#endif
                break;
            case AdController.AdType.REWARD_ADMOB:
                AdAnalysis("ad_admob_rewarded");
                break;
            case AdController.AdType.REWARDED_INTERSTITIAL_ADMOB:
                AdAnalysis("ad_admob_interstitial_rewarded");
                break;
            case AdController.AdType.LP_INTERSTITIAL_REWARDED:
                AdAnalysis("ad_interstitial_rewarded");
                break;
            case AdController.AdType.SECOND_INTERSTITIAL:
                AdAnalysis("ad_second_interstitial");
                break;
            case AdController.AdType.OPEN_AD:
                AdAnalysis("ad_app_open");
                break;
        }
    }

    private void AdAnalysis(string adType)
    {
        if (sendAnalytics == false)  // if adcontroller is not intialized or analytics are not enabled from user
            return;

#if USE_FIREBASE

        try
        {
            //FirebaseAnalytics.LogEvent("Ads_Analysis", // Sending Analytics to firebase
            // new Parameter("Ads", adType) // sending ads type send from Adcontroller
            // );

            FirebaseAnalytics.LogEvent(adType); // sending ads type send from Adcontroller

        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

#endif
    }

#endregion
//#if USE_IS
//    void OnApplicationPause(bool isPaused)
//    {
//        IronSource.Agent.onApplicationPause(isPaused);
//    }
   

//#endif
#region Paid Events
#if USE_IS
    public void PaidAdAnalytics(Unity.Services.LevelPlay.LevelPlayImpressionData impressionData)
    {
        if (impressionData != null)
        {
            AdConstants.DebugGreen("-------- IS Paid Ad Evemt " + impressionData.AdFormat + " " + impressionData.InstanceName);
        }
        try
        {

            // if adcontroller is not intialized or analytics are not enabled from user or any args or parameter is send with null
            if (sendAnalytics == false)
                return;
#if USE_FIREBASE
            Double revenue = (double)impressionData.Revenue;
            if (impressionData != null)
            {
                Firebase.Analytics.Parameter[] AdParameters = {
             new Firebase.Analytics.Parameter("ad_platform", "ironSource"),
              new Firebase.Analytics.Parameter("ad_source", impressionData.AdNetwork),
              new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdFormat),
            new Firebase.Analytics.Parameter("ad_format", impressionData.InstanceName),
              new Firebase.Analytics.Parameter("currency","USD"),
            new Firebase.Analytics.Parameter("value", revenue)
        };

                if (impressionType == PaidAdEvent.ad_impression || impressionType == PaidAdEvent.both)
                {
                    FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
                }
                if (impressionType == PaidAdEvent.new_ad_impression || impressionType == PaidAdEvent.both)
                {
                    FirebaseAnalytics.LogEvent("new_ad_impression", AdParameters);
                }
                CheckTheRevenue(revenue);
            }

#if USE_APPSFLYER_6_15
            Dictionary<string, string> additionalParams = new Dictionary<string, string>();

            additionalParams.Add( AdRevenueScheme.AD_UNIT, impressionData.AdFormat);
            additionalParams.Add(AdRevenueScheme.AD_TYPE, impressionData.InstanceName);
            var logRevenue = new AFAdRevenueData(impressionData.AdNetwork, MediationNetwork.IronSource, "USD", (double)impressionData.Revenue);
            AppsFlyer.logAdRevenue(logRevenue, additionalParams);
#endif

#if USE_FACEBOOK
            Dictionary<string, object> additionalParamsFb = new Dictionary<string, object>();
            additionalParamsFb.Add(AppEventParameterName.Currency, "USD");
            FB.LogAppEvent("AdImpression", (float)impressionData.revenue, additionalParamsFb);
#endif

#endif

#if USE_BYTEBREW

            if (FinzByteBrewAnalyticsManager.instance!=null
                &&impressionData != null
                && impressionData.MediationAdUnitName != null)
            {
                if (impressionData.MediationAdUnitName.ToLower().Contains("reward"))
                {
                    FinzByteBrewAnalyticsManager.instance.RewardPaidAdImpression("ironSource", impressionData.AdNetwork, revenue);
                }
                else if (impressionData.MediationAdUnitName.ToLower().Contains("inter"))
                {
                    FinzByteBrewAnalyticsManager.instance.InterstitialPaidAdImpression("ironSource", impressionData.AdNetwork, revenue);
                }
                else {

                }
            }
#endif



            }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }

#endif


#region ROAS COMPAIN

    private double roasThreshold = 0.10; // Can be 0.05, 0.10, or 0.13 depending on the campaign

    // Enable or disable ROAS experiment logic
    public bool enableRoasExperiment = true;

    public void CheckTheRevenue(double _rev)
    {
#if USE_FIREBASE
        double customRevenue;
        double currentAccumulatedRevenue;

        // Retrieve existing revenue from PlayerPrefs
        if (!double.TryParse(PlayerPrefs.GetString("revenue_add", "0"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out customRevenue))
        {
            customRevenue = 0.0;
        }

        currentAccumulatedRevenue = customRevenue + _rev;

        // Update PlayerPrefs with new accumulated revenue
        PlayerPrefs.SetString("revenue_add", currentAccumulatedRevenue.ToString(System.Globalization.CultureInfo.InvariantCulture));
        PlayerPrefs.Save();

        // Custom ROAS experimental event
        if (enableRoasExperiment && currentAccumulatedRevenue >= roasThreshold)
        {
            string customEventName = "c_ad_impression_10";

            var roasParameters = new[]
            {
                new Parameter(FirebaseAnalytics.ParameterValue, currentAccumulatedRevenue),
                new Parameter(FirebaseAnalytics.ParameterCurrency, "USD")
            };

            FirebaseAnalytics.LogEvent(customEventName, roasParameters);

            // Optional: Reset the revenue after firing the ROAS event (depending on your experiment)
             PlayerPrefs.SetString("revenue_add", "0");
             PlayerPrefs.Save();
        }
#endif
    }


#endregion

    public bool oldAdmobAppsflyerCode = true;

#if USE_ADMOB_PAID_EVENT
    public void PaidAdAnalytics(string adString, ResponseInfo info, GoogleMobileAds.Api.AdValue adValue)
    {

        if (info != null && adValue != null)
        {
            AdConstants.DebugGreen("-------- GP Paid Ad Evemt " + adString);
        }
        // if adcontroller is not intialized or analytics are not enabled from user or any args or parameter is send with null
        if (!sendAnalytics || info == null || adValue == null)
            return;


        try
        {


            decimal currentImpressionRevenue = (decimal)(adValue.Value / Mathf.Pow(10, 6)); // calculation impression revenue with 10^6 decimals
            decimal previousTroasCache = decimal.Parse(getAdValue(adString), System.Globalization.NumberStyles.Float); // previously cached troas
            decimal currentTroasCache = (decimal)(previousTroasCache + currentImpressionRevenue); // summing up previous and current troas to get estimated value

#if USE_FACEBOOK
                Dictionary<string, object> additionalParamsFb = new Dictionary<string, object>();
                additionalParamsFb.Add(AppEventParameterName.Currency, "USD");
                FB.LogAppEvent("AdImpression", (float)currentImpressionRevenue, additionalParamsFb);
#endif

            if (oldAdmobAppsflyerCode) {

#if USE_APPSFLYER_6_15
                Dictionary<string, string> additionalParams = new Dictionary<string, string>();

                additionalParams.Add(AdRevenueScheme.AD_UNIT, info.GetLoadedAdapterResponseInfo().AdSourceInstanceName);
                additionalParams.Add(AdRevenueScheme.AD_TYPE, adString);
                var logRevenue = new AFAdRevenueData(info.GetMediationAdapterClassName().ToString(), MediationNetwork.GoogleAdMob, adValue.CurrencyCode.ToString(), adValue.Value); 
                AppsFlyer.logAdRevenue(logRevenue, additionalParams);
#endif

            }
            else
            {

#if USE_APPSFLYER_6_15
                Dictionary<string, string> additionalParams = new Dictionary<string, string>();

                additionalParams.Add(AdRevenueScheme.AD_UNIT, info.GetLoadedAdapterResponseInfo().AdSourceInstanceName);
                additionalParams.Add(AdRevenueScheme.AD_TYPE, adString);
                var logRevenue = new AFAdRevenueData(info.GetMediationAdapterClassName().ToString(), MediationNetwork.GoogleAdMob, adValue.CurrencyCode.ToString(), (double)currentImpressionRevenue); 
                AppsFlyer.logAdRevenue(logRevenue, additionalParams);
#endif
            }
            CheckTheRevenue((double)currentImpressionRevenue);
            if (currentTroasCache >= (decimal)0.01) // avoiding minor values we do'nt need those
            {

#if USE_FIREBASE

                if (impressionType == PaidAdEvent.ad_impression || impressionType == PaidAdEvent.both)
                {
                    FirebaseAnalytics.LogEvent("ad_impression", // sending Paid events details to Firebase
                   new Parameter("value", (double)currentTroasCache),
                   new Parameter("currency", "" + adValue.CurrencyCode.ToString()),
                   new Parameter("precision", "" + adValue.Precision.ToString()),
                   new Parameter("network", "" + info.GetMediationAdapterClassName().ToString())
                   ); ;
                }

                if (impressionType == PaidAdEvent.new_ad_impression || impressionType == PaidAdEvent.both)
                {
                    FirebaseAnalytics.LogEvent("new_ad_impression", // sending Paid events details to Firebase
                   new Parameter("value", (double)currentTroasCache),
                   new Parameter("currency", "" + adValue.CurrencyCode.ToString()),
                   new Parameter("precision", "" + adValue.Precision.ToString()),
                   new Parameter("network", "" + info.GetMediationAdapterClassName().ToString())
                   ); ;
                }

           
//                CheckTheRevenue((double)currentTroasCache));
                setAdValue(adString, "0");

#endif


            }
            else
            {
                setAdValue(adString, currentTroasCache.ToString()); // else update Troas in cache
            }





#if USE_BYTEBREW

                if (FinzByteBrewAnalyticsManager.instance != null
                    && info != null
                    && adValue != null)
                {
                    if (adString.ToLower().Contains("reward"))
                    {
                        FinzByteBrewAnalyticsManager.instance.RewardPaidAdImpression("Google", "admob", (double)currentImpressionRevenue);
                    }
                    else if (adString.ToLower().Contains("inter"))
                    {
                        FinzByteBrewAnalyticsManager.instance.InterstitialPaidAdImpression("Google", "admob", (double)currentImpressionRevenue);
                    }
                    else
                    {

                    }
                }
#endif
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    } // Paid events Admob

#endif

#region Playerpref

    private void setAdValue(string str, string val)
    {
        PlayerPrefs.SetString("tROAS" + str, val); // Saving Troas values
    }

    private string getAdValue(string str)
    {
        return PlayerPrefs.GetString("tROAS" + str, "0"); // Getting Troas values
    }



#endregion

#endregion

#endregion

    // Firebase Remote Config
#region Remote Configurations


#region Remote Config 



    public void FetchFireBase()
    {
#if USE_REMOTE_CONFIG && USE_FIREBASE
        try
        {
            FetchDataAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

#else
        if (OnRemoteConfigInitialized != null)
            OnRemoteConfigInitialized.Invoke();

#endif
    }
#if USE_REMOTE_CONFIG && USE_FIREBASE
    private Task FetchDataAsync()
    {

        System.Threading.Tasks.Task fetchTask =
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
            TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);

    }
#endif
#if USE_REMOTE_CONFIG && USE_FIREBASE
    private void FetchComplete(Task fetchTask)
    {
        try
        {
            if (fetchTask.IsCanceled)
            {
                Debug.Log("Fetch canceled.");

            }
            else if (fetchTask.IsFaulted)
            {
                Debug.Log("Fetch encountered an error.");

            }
            else if (fetchTask.IsCompleted)
            {
                Debug.Log("Fetch completed successfully!");

            }


            var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus)
            {
                case Firebase.RemoteConfig.LastFetchStatus.Success:
                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                    .ContinueWithOnMainThread(task =>
                    {
                        Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).", info.FetchTime));
                    });
                    OnRemoteDataFetched();
                    if (OnRemoteConfigInitialized != null)
                        OnRemoteConfigInitialized.Invoke();
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Failure:

                    Debug.Log(AdConstants.Colorize("Firebase Local Json Activated", "Red", true));

                    // On failed
                    if (OnRemoteConfigInitialized != null)
                        OnRemoteConfigInitialized.Invoke();
                    switch (info.LastFetchFailureReason)
                    {
                        case Firebase.RemoteConfig.FetchFailureReason.Error:
                            Debug.Log("Fetch failed for unknown reason");
                            break;
                        case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                            Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }
                    Invoke("FetchFireBase",10f);
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Pending:
                    Debug.Log("Latest Fetch call still pending.");
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }
#endif

#endregion

#if USE_REMOTE_CONFIG && USE_FIREBASE

    private void OnRemoteDataFetched()
    {
        try
        {
            IDictionary<string, ConfigValue> fetchValue = FirebaseRemoteConfig.DefaultInstance.AllValues;

            foreach (var item in fetchValue)
            {

                for (int i = 0; i < remote_Data.Count; i++)
                {

                    if (remote_Data[i].name.Equals(item.Key))
                    {
                        switch (remote_Data[i].type)
                        {
                            case FirebaseRemoteData.DataType.NUMBER:

                                //Debug.Log(string.Format("Name: {0} Value: {1}", item.Key, item.Value.DoubleValue));
                                remote_Data[i].DefaultValue_Number = (int)item.Value.DoubleValue;

                                break;

                            case FirebaseRemoteData.DataType.STRING:

                                //Debug.Log(string.Format("Name: {0} Value: {1}", item.Key, item.Value.StringValue));
                                remote_Data[i].DefaultValue_String = item.Value.StringValue;

                                break;

                            case FirebaseRemoteData.DataType.BOOLEAN:

                                //Debug.Log(string.Format("Name: {0} Value: {1}", item.Key, item.Value.BooleanValue));
                                remote_Data[i].DefaultValue_Boolean = item.Value.BooleanValue;

                                break;

                            case FirebaseRemoteData.DataType.JSON:

                                //Debug.Log(string.Format("Name: {0} Value: {1}", item.Key, item.Value.StringValue));
                                remote_Data[i].DefaultValue_Json = item.Value.StringValue;

                                break;
                        }
                        // Calling the Callback with updated data 
                        if (remote_Data[i].onFetched != null)
                            remote_Data[i].onFetched.Invoke(remote_Data[i]);
                    }
                }
            }


            if (AdController.instance)
                AdController.instance.OnFirebaseCompleteFetching(fetchValue); // Calling delegate method for user firebase cusstom remote config data
            Debug.Log(AdConstants.Colorize("Custom Firebase Fetched", "Green", true));
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);

            Debug.Log(AdConstants.Colorize("Firebase Local Json Activated Reason:" + e.Message, "Red", true));



        }

    }
#endif


#endregion

#region Remote Methods

    public void OnFetchAdsSettings(FirebaseRemoteData data)
    {
        Debug.Log(AdConstants.Colorize("Android Ads Settings Fetched Sucessfully Data: " + data.DefaultValue_Json, "Green", true));

    }

    public void OnFetchLowEndDevicesSettings(FirebaseRemoteData data)
    {
        Debug.Log(AdConstants.Colorize("Low End Devices Ads Settings Fetched Sucessfully Data: " + data.DefaultValue_Json, "Green", true));

    }

#endregion


}
