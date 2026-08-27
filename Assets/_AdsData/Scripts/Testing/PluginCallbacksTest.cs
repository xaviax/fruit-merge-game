using System;
using System.Collections;
using System.Collections.Generic;
#if USE_FIREBASE && USE_REMOTE_CONFIG
using Firebase.RemoteConfig;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class PluginCallbacksTest : MonoBehaviour
{
    public static PluginCallbacksTest instance;
    // Property to access the instance
    public static PluginCallbacksTest Instance
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
    public string dummyRemoteKey;
    public int dummyDefaultRemoteValue;
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
            DontDestroyOnLoad(this);
        }

    }

    private void OnEnable()
    {
        AdController.gaveRewardMethod += EarnedReward;
        //AdController.loadNextScene += LoadScene;

        AdController.noRewardedVideoMethod += NoRewardedVideo; //AdController.noVideoDialogMethod += noVideoDialog;
        AdController.cancelRewardedAdMethod += CancelRewardedAd;

        AdController.reviewDialogMethod += promptReviewDialog;

#if USE_FIREBASE && USE_REMOTE_CONFIG
        AdController.onFirebaseRemoteConfigSuccess += OnRemoteDataFetched;
#endif
        //AdController.crossPromotiongMethod += ShowCrossPromotions;
    }

    private void OnDisable()
    {
        AdController.gaveRewardMethod -= EarnedReward;

        AdController.noRewardedVideoMethod -= NoRewardedVideo;  //    AdController.noVideoDialogMethod -= noVideoDialog;
        AdController.cancelRewardedAdMethod -= CancelRewardedAd;

        AdController.reviewDialogMethod -= promptReviewDialog;

#if USE_FIREBASE && USE_REMOTE_CONFIG

        AdController.onFirebaseRemoteConfigSuccess -= OnRemoteDataFetched;
#endif
    }

    public void EarnedReward()
    {
        Debug.Log("========== Earned Reward =================");
    }


    public void CancelRewardedAd()
    {
        Debug.Log("========== Cancel Reward =================");
    }

    public void NoRewardedVideo()
    {
        // will ad gave reward logic here.
        Debug.Log("=========================================================== No Rewards===========================================================");


    }

    public void promptReviewDialog()
    {
        if (AdConstants.shouldDisplayRateMenu())
        {
            Debug.Log("Rate event called");
            if (Screen.orientation == ScreenOrientation.Portrait)
                Instantiate(Resources.Load("PotraitNativeAndroidRateDialogMenu"));
            else
                Instantiate(Resources.Load("NativeAndroidRateDialogMenu"));

            if (AdController.instance)
                AdController.instance.DestroyBannerAd(AdController.BannerAdTypes.Mrec);
        }

    }

    private void OnRemoteDataFetched(
#if USE_FIREBASE && USE_REMOTE_CONFIG
        IDictionary<string, ConfigValue> fetchValue
#endif
        )

    {

        Debug.Log("==============> Firebase Data Fetched => User Side <==============");
        try
        {
#if USE_FIREBASE && USE_REMOTE_CONFIG
            fetchValue = FirebaseRemoteConfig.DefaultInstance.AllValues;

            foreach (var item in fetchValue)
            {
                if (dummyRemoteKey.Contains(item.Key))
                {
                    dummyDefaultRemoteValue = (int)item.Value.DoubleValue;
                }
            }
#endif
        }
        catch (Exception e)
        {
            Debug.LogError("Error Occur: "+e.Message);
        }
    }
}
