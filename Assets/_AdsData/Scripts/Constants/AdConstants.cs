using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public static class AdConstants
{


	public static bool canShowIOSFistAd = false;
	public static bool shouldShowRateMenu = false;
	public static bool isfirstTime;
	public static bool isLowEndDevice = false;
	public static bool limitAds = false;
	public static bool showBannerAd = true, showAdaptiveBannerAd = true, showNativeBannerAd = true, showStaticAd = true, showInterstitialAd = true, showRewardedAd = true, showRewardedInterstitialAd = true, showApplovinAppOpen = true, showAdmobAppOpen = true;
	public static int adDelay = 10;
	public static int AdDelayReward=1;
	public static bool isShowingLPIntersititialReward = false;
	public static bool sawRewarded = false;
	public static bool resumeFromAds = false;
	public static bool IsAdWasShowing = false;
	public static bool isUiOverNgui = false;
	public static bool showAppOpenAfterAd;
	public static int firstAppOpenSession = 2;
	public static bool GetInternetStatus()
	{
#if UNITY_EDITOR
		return true;
#else
		return isInternetReachable();
#endif
	}


	public static void DebugGreen(string message)
	{
		Debug.Log($"<color=green>{message}</color>");
	}

	public static bool CanShowFirstAppOpen()
	{
		int currentSession = PlayerPrefs.GetInt("GameSession", 0);
		return currentSession % firstAppOpenSession == 0;
	}

	public static int getGameSession()
	{
		int currentSession = PlayerPrefs.GetInt("GameSession", 0);
		return currentSession;
	}

	public static void CountGameSession()
	{
		int currentSession = PlayerPrefs.GetInt("GameSession", 0);
		currentSession++;
		PlayerPrefs.SetInt("GameSession", currentSession);
	}


	public static bool isInternetReachable()
	{
		return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;

	}

    public static bool shouldDisplayAds()
	{

		return PlayerPrefs.GetInt("ADS_KEY", 1) == 1;

	}

	public static void disableAds()
	{
		PlayerPrefs.SetInt("ADS_KEY", 0);

		PlayerPrefs.Save();

		if (AdController.instance)
			AdController.instance.DestroyBannerAd(AdController.BannerAdTypes.BANNER);

	}

	public static void EnableAds()
	{
		PlayerPrefs.SetInt("ADS_KEY", 1);

		PlayerPrefs.Save();

	}

	public static bool shouldDisplayRateMenu()
	{

		return PlayerPrefs.GetInt("RATE_KEY", 1) == 1;

	}

	public static void userHasRatedApp()
	{
		//PluginCallBacksManager.Instance.AdsInApp ();
		PlayerPrefs.SetInt("RATE_KEY", 0);

		PlayerPrefs.Save();

		if (AdController.instance)
			AdController.instance.DestroyBannerAd(AdController.BannerAdTypes.BANNER);

	}



	public static bool checkPackageAppIsPresent(string package)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
	    AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
	    AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
	    AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

	    //take the list of all packages on the device
	    AndroidJavaObject appList = packageManager.Call<AndroidJavaObject>("getInstalledPackages", 0);
	    int num = appList.Call<int>("size");
	    for (int i = 0; i < num; i++)
	    {
		    AndroidJavaObject appInfo = appList.Call<AndroidJavaObject>("get", i);
		    string packageNew = appInfo.Get<string>("packageName");
		    if (packageNew.CompareTo(package) == 0)
		    {
			    return true;
		    }
	    }
#elif UNITY_IOS
		if (shouldDisplayCPMenu())
			return false;
        else return true;
#elif UNITY_EDITOR
		return false;
#endif
		return false;
	}

	public static bool shouldDisplayCPMenu()
	{

		return PlayerPrefs.GetInt("CP_Menu", 1) == 1;

	}


	static bool checkPackageAppIsPresentForTesting(string package)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

		//take the list of all packages on the device
		AndroidJavaObject appList = packageManager.Call<AndroidJavaObject>("getInstalledPackages", 0);
		int num = appList.Call<int>("size");
		for (int i = 0; i < num; i++)
		{
			AndroidJavaObject appInfo = appList.Call<AndroidJavaObject>("get", i);
			string packageNew = appInfo.Get<string>("packageName");
			if (packageNew.CompareTo(package) == 0)
			{
				return true;
			}
		}
#elif UNITY_IOS && !UNITY_EDITOR
			return false;
#elif UNITY_EDITOR
		return true;
#endif
		return false;
	}

	public static bool isTestingLicenseAvailable()
	{
		if (checkPackageAppIsPresentForTesting("com.finz.license.apk"))
		{
			return true;
		}
		else return false;
	}

	public static bool IsCurrentPackageAvailable(string packageName)
	{
		if (checkPackageAppIsPresentForTesting(packageName))
		{
			return true;
		}
		else return false;
	}

	public static string Colorize(string text, string color, bool bold)
	{
		return
		"<color=" + color + ">" +
		(bold ? "<b>" : "") +
		text +
		(bold ? "</b>" : "") +
		"</color>";
	}


	#region GC Collection
	public static void EnableGC()
	{
		GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
		// Trigger a collection to free memory.
		GC.Collect();
	}


	#endregion

}
