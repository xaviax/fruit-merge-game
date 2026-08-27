public static class AdConfig
{
    public static string AppKey => GetAppKey();
    public static string BannerAdUnitId => GetBannerAdUnitId();
    public static string InterstitalAdUnitId => GetInterstitialAdUnitId();
    public static string RewardedVideoAdUnitId => GetRewardedVideoAdUnitId();

    static string GetAppKey()
    {
        #if UNITY_ANDROID
            return "25b63cf85";
        #elif UNITY_IPHONE
            return "25c43a4a5";
        #else
            return "unexpected_platform";
        #endif
    }

    static string GetBannerAdUnitId()
    {
        #if UNITY_ANDROID
            return "4fpetq4lhe5lsw3e";
        #elif UNITY_IPHONE
            return "xc2bsuntn9ea734t";
        #else
            return "unexpected_platform";
        #endif
    }
    static string GetInterstitialAdUnitId()
    {
        #if UNITY_ANDROID
            return "h3xw38h9214adgxo";
        #elif UNITY_IPHONE
            return "obg6ohwts3y690ks";
        #else
            return "unexpected_platform";
        #endif
    }

    static string GetRewardedVideoAdUnitId()
    {
        #if UNITY_ANDROID
            return "syz3d8ekts22q0or";
        #elif UNITY_IPHONE
            return "l1quzz1xmmdhw5er";
        #else
            return "unexpected_platform";
        #endif
    }
}
