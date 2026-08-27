
using Sirenix.OdinInspector;
using UnityEngine;
public static class AdsIds
{
    #if USE_IS
    public static string SDKKey()
    {
        #if UNITY_ANDROID
            return "2362640d5";
        #elif UNITY_IOS
            return "10b5b4f65";
        #else
            return "";
        #endif
    }
      public static string InterstitialAdUnitId()
    {
        #if UNITY_ANDROID
            return "s9dyk2tiyt7mxtt1";
        #elif UNITY_IOS
            return "gle6noxwuwrjzisr";
        #else
            return "";
        #endif
    }
    public static string RewardedAdUnitId()
    {
        #if UNITY_ANDROID
            return "zr5skb2ls3r2dj0y";
        #elif UNITY_IOS
            return "jhut4u78k77ca8w1";
        #else
            return "";
        #endif
    }
    public static string BannerAdUnitId()
    {
        #if UNITY_ANDROID
            return "ackkioa6s6nabwzd";
        #elif UNITY_IOS
            return "4xu1san636ndet9n";
        #else
            return "";
        #endif
    }
    #if USE_IS_NATIVE_MREC
    public static string MRECBannerAdUnitId()
    {
        #if UNITY_ANDROID
            return "3no9u4ma26idue2i";
        #elif UNITY_IOS
            return "ec6805c550a1dd8d";
        #else
            return "";
        #endif
    }
    #endif
#if USE_IS_SECOND_INTERSITITIAL
    public static string SecondInterstitialAdUnitId()
    {
        #if UNITY_ANDROID
            return "po2clkvtv3t6uwpk";
        #elif UNITY_IOS
            return "";
        #else
            return "";
        #endif
    }
    #endif

#if THREE_TIER_INTER
    public static string InterstitialAdUnitIdTier1()
    {
        #if UNITY_ANDROID
            return "tng5ya05q0aa5csr";
        #elif UNITY_IOS
            return "";
        #else
            return "";
        #endif
    }
    public static string InterstitialAdUnitIdTier2()
    {
        #if UNITY_ANDROID
            return "nvb3smg8oh8tjhxb";
        #elif UNITY_IOS
            return "";
        #else
            return "";
        #endif
    }
    public static string InterstitialAdUnitIdTier3()
    {
        #if UNITY_ANDROID
            return "hs9xwhmeo8j21l9h";
        #elif UNITY_IOS
            return "";
        #else
            return "";
        #endif
    }
    #endif


#if THREE_TIER_REWARD
    public static string RewardedAdUnitIdTier1()
    {
        #if UNITY_ANDROID
            return "ajjkmduj9vv8s68s";
        #elif UNITY_IOS
            return "";
        #else
            return "";
        #endif
    }
    public static string RewardedAdUnitIdTier2()
    {
        #if UNITY_ANDROID
            return "8yzjg0pie7haoiox";
        #elif UNITY_IOS
            return "";
        #else
            return "";
        #endif
    }
    public static string RewardedAdUnitIdTier3()
    {
        #if UNITY_ANDROID
            return "km1mrjcv775j0ytl";
        #elif UNITY_IOS
            return "";
        #else
            return "";
        #endif
    }
    #endif

#if THREE_TIER_BANNER
    public static string BannerAdUnitIdTier1()
    {
        #if UNITY_ANDROID
            return "lm8q76zl04hc8r7i";
        #elif UNITY_IOS
            return "";
        #else
            return "";
        #endif
    }
    public static string BannerAdUnitIdTier2()
    {
        #if UNITY_ANDROID
            return "4rck7mmhw4hsgd7b";
        #elif UNITY_IOS
            return "";
        #else
            return "";
        #endif
    }
    public static string BannerAdUnitIdTier3()
    {
        #if UNITY_ANDROID
            return "zgqv3gv13vf10b0s";
        #elif UNITY_IOS
            return "";
        #else
            return "";
        #endif
    }
    #endif


    #endif


   #if  USE_ADMOB_SIMPLE_BANNER
    public static string BannerAdUnitIdAdmob()
    {
        #if UNITY_ANDROID
            return "ca-app-pub-4921234158243313/4645820075";
        #elif UNITY_IOS
            return "ec6805c550a1dd8d";
        #else
            return "";
        #endif
    }
    #endif

       #if  USE_ADMOB_MREC_BANNER
    public static string MrecAdUnitIdAdmob()
    {
        #if UNITY_ANDROID
            return "ca-app-pub-4921234158243313/3045979124";
        #elif UNITY_IOS
            return "ec6805c550a1dd8d";
        #else
            return "";
        #endif
    }
    #endif

           #if  USE_ADMOB_STATIC_AD
    public static string StaticAdUnitIdAdmob()
    {
        #if UNITY_ANDROID
            return "ca-app-pub-4921234158243313/6793652443";
        #elif UNITY_IOS
            return "ec6805c550a1dd8d";
        #else
            return "";
        #endif
    }
    #endif

       #if  USE_ADMOB_INTERSITITIAL_AD
    public static string InterstitialAdUnitIdAdmob()
    {
        #if UNITY_ANDROID
            return "ca-app-pub-4921234158243313/7082229383";
        #elif UNITY_IOS
            return "ec6805c550a1dd8d";
        #else
            return "";
        #endif
    }
    #endif

       #if  USE_ADMOB_REWARD_AD
    public static string RewardAdUnitIdAdmob()
    {
        #if UNITY_ANDROID
            return "ca-app-pub-4921234158243313/9736211255";
        #elif UNITY_IOS
            return "ec6805c550a1dd8d";
        #else
            return "";
        #endif
    }
    #endif

       #if  USE_ADMOB_REWARD_INTERSITITIAL_AD
    public static string RewardIntersititialAdUnitIdAdmob()
    {
        #if UNITY_ANDROID
            return "ca-app-pub-4921234158243313/8336828665";
        #elif UNITY_IOS
            return "ec6805c550a1dd8d";
        #else
            return "";
        #endif
    }
    #endif

    #if  USE_ADMOB_OPEN_AD_8_5
    public static string AppOpenAdTier1()
    {
        #if UNITY_ANDROID
            return "ca-app-pub-4921234158243313/19646804481";
        #elif UNITY_IOS
            return "ca-app-pub-3747940928920534/1218772772";
        #else
            return "";
        #endif
    }
    public static string AppOpenAdTier2()
    {
        #if UNITY_ANDROID
            return "ca-app-pub-4921234158243313/5637198114";
        #elif UNITY_IOS
            return "ca-app-pub-3747940928920534/8905691109";
        #else
            return "";
        #endif
    }
    public static string AppOpenAdTier3()
    {
        #if UNITY_ANDROID
            return "ca-app-pub-4921234158243313/4590843785";
        #elif UNITY_IOS
            return "ca-app-pub-3747940928920534/8122274232";
        #else
            return "";
        #endif
    }
    #endif


    


}