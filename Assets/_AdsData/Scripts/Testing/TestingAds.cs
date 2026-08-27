using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TestingAds : MonoBehaviour
{
    public Text debugText;
    Vector2 textSize;


    #region Banner
    public void showBannerad() {
        if (AdController.instance) {
            AdController.instance.ShowBannerAd(AdController.BannerAdTypes.BANNER);
        }
    }
    public void hideBannerad()
    {
        if (AdController.instance)
        {
            AdController.instance.HideBannerAd(AdController.BannerAdTypes.BANNER);
        }
    }
    public void showMrecBannerad()
    {
        if (AdController.instance)
        {
            AdController.instance.ShowBannerAd(AdController.BannerAdTypes.Mrec);
        }
    }
    public void hideMrecBannerad()
    {
        if (AdController.instance)
        {
            AdController.instance.HideBannerAd(AdController.BannerAdTypes.Mrec);
        }
    }
    public void showAdmobBannerad()
    {
        if (AdController.instance)
        {
            AdController.instance.ShowBannerAd(AdController.BannerAdTypes.BANNER_ADMOB);
        }
    }
    public void hideAdmobBannerad()
    {
        if (AdController.instance)
        {
            AdController.instance.HideBannerAd(AdController.BannerAdTypes.BANNER_ADMOB);
        }
    }
    public void showMrecAdmobBannerad()
    {
        if (AdController.instance)
        {
            AdController.instance.ShowBannerAd(AdController.BannerAdTypes.MREC_ADMOB);
        }
    }
    public void hideMrecAdmobBannerad()
    {
        if (AdController.instance)
        {
            AdController.instance.HideBannerAd(AdController.BannerAdTypes.MREC_ADMOB);
        }
    }
    #endregion

    #region Ads
    public void showIntersititial()
    {
        if (AdController.instance) {
            AdController.instance.ShowAd(AdController.AdType.INTERSTITIAL);
        }
    }
    public void showRewarded()
    {
        if (AdController.instance)
        {
            AdController.instance.ShowAd(AdController.AdType.REWARDED);
        }
    }
    public void showIntersititialAdmob()
    {
        if (AdController.instance)
        {
            AdController.instance.ShowAd(AdController.AdType.INTERSTITIAL_ADMOB);
        }
    }
    public void showRewardedAdmob()
    {
        if (AdController.instance)
        {
            AdController.instance.ShowAd(AdController.AdType.REWARD_ADMOB);
        }
    }
    public void showStaticAdmob()
    {
        if (AdController.instance)
        {
            AdController.instance.ShowAd(AdController.AdType.STATIC_ADMOB);
        }
    }
    public void showRewardedIntersititialAdmob()
    {
        if (AdController.instance)
        {
            AdController.instance.ShowAd(AdController.AdType.REWARDED_INTERSTITIAL_ADMOB);
        }
    }
    #endregion


    void OnEnable()
    {

        if (debugText) textSize = debugText.gameObject.GetComponent<RectTransform>().sizeDelta;
        Application.logMessageReceived += ShowLogsOnText;

    }


    void OnDisable()
    {
        Application.logMessageReceived -= ShowLogsOnText;
     

    }

//    public void ShowBanner()
//    {
//        if (AdController.instance)
//            AdController.instance.ShowBannerAd(AdController.BannerAdTypes.BANNER);//(AdController.BannerAdTypes.BANNER);

//    }
//    public void HideBanner()
//    {
//        if (AdController.instance)
//            AdController.instance.HideBannerAd(AdController.BannerAdTypes.BANNER);//(AdController.BannerAdTypes.BANNER);
//    }

//    public void DestroyBanner()
//    {
//        if (AdController.instance)
//            AdController.instance.DestroyBannerAd(AdController.BannerAdTypes.BANNER);//(AdController.BannerAdTypes.BANNER);
//    }

//    public void ShowAdaptiveBanner()
//    {
//        if (AdController.instance)
//            AdController.instance.ShowBannerAd(AdController.BannerAdTypes.ADAPTIVE);
//    }

//    public void DestroyAdaptiveBanner()
//    {
//        if (AdController.instance)
//            AdController.instance.DestroyBannerAd(AdController.BannerAdTypes.ADAPTIVE);//(AdController.BannerAdTypes.ADAPTIVE);
//    }

//    public void HideAdaptiveBanner()
//    {
//        if (AdController.instance)
//            AdController.instance.HideBannerAd(AdController.BannerAdTypes.ADAPTIVE);//(AdController.BannerAdTypes.ADAPTIVE);
//    }

//    public void ShowNativeBanner()
//    {
//        if (AdController.instance)
//            AdController.instance.ShowBannerAd(AdController.BannerAdTypes.Mrec);// (AdController.BannerAdTypes.NATIVE);
//    }

//    public void ShowIdleNativeBanner()
//    {
//#if USE_IDLE_MREC
//        if (IdleMrecManager.Instance)
//            IdleMrecManager.Instance.ShowIdleMRec();// (AdController.BannerAdTypes.NATIVE);
//#endif
//    }

//    public void HideNativeBanner()
//    {
//        if (AdController.instance)
//            AdController.instance.HideBannerAd(AdController.BannerAdTypes.Mrec);//(AdController.BannerAdTypes.NATIVE);
//    }
//    public void HideIdleNativeBanner()
//    {
//#if USE_IDLE_MREC
//        if (IdleMrecManager.Instance)
//            IdleMrecManager.Instance.HideIdleMRec();//(AdController.BannerAdTypes.NATIVE);
//#endif
//    }

//    public void DestroyNativeBanner()
//    {
//        if (AdController.instance)
//            AdController.instance.DestroyBannerAd(AdController.BannerAdTypes.Mrec);//(AdController.BannerAdTypes.NATIVE);
//    }

//    public void RequestStaticAd()
//    {
//        if (AdController.instance) 
//            AdController.instance.LoadAd(AdController.AdType.STATIC);

            
//    }

//    public void ShowStaticAd()
//    {
//        if (AdController.instance)
//        {
//            AdController.instance.ShowAd(AdController.AdType.STATIC);
//            //AdConstants.currentState = AdConstants.States.OnPause;
//            //AdController.instance.ChangeState();

//        }

//    }

//    public void RequestInterstitilAd()
//    {
//        if (AdController.instance)
//            AdController.instance.LoadAd(AdController.AdType.INTERSTITIAL);
//    }

//    public void ShowInterstitilAd()
//    {
//        if (AdController.instance)
//        {
//            //AdController.instance.ShowAd(AdController.AdType.STATIC);
//            AdController.instance.ShowAd(AdController.AdType.INTERSTITIAL);
//        }
//    }


//    public void RequestRewardedAd()
//    {
//        if (AdController.instance && !AdController.instance.IsRewardedAdAvailable())
//            AdController.instance.LoadAd(AdController.AdType.REWARDED);
//    }

//    public void ShowRewardedAd()
//    {
//        if (AdController.instance)
//        {
//            AdController.instance.ShowAd(AdController.AdType.REWARDED);

//        }
//    }


//    public void RequestRewardedInterstitialAd()
//    {
       
//    }

//    public void ShowRewardedInterstitialAd()
//    {
        
//    }

//    public void ShowRateUs()
//    {
//        if (AdController.instance)
//        {
//            AdController.instance.PromptRateMenu();
//        }
//    }





    public void ShowLogsOnText(string logString, string stackTrace, LogType type)
    {
        if (debugText)
            debugText.text = "\n======================================\n"+logString;
    }

    public void CheckAndShowRewardedAd()
    {
        if (AdController.instance.IsRewardedAdAvailable())
        {
            //AdController.instance.ShowAd(AdController.AdType.REWARDED);
            Debug.Log("========== Rewarded Ad Available ==========");

        }
        else
        {
            Debug.Log("========== No Rewarded Ad Available ==========");
        }
    }
}
