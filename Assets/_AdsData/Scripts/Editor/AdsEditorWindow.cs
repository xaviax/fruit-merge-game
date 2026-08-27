using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.AccessControl;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Reflection;
using UnityEditor.Build.Reporting;
using System.Xml;

[Serializable]
public class UpdateData
{
    public string version;
    public string changelog;
    public string downloadUrl;
}
public class AdsEditorWindow : EditorWindow
{

   
    public class AdKeyData
    {
        public string androidKey = "", iOSKey = "";
        string propertyName, functionName;
        Func<bool> showCondition;
        public AdKeyData(string propertyName, string functionName, Func<bool> showCondition)
        {
            this.propertyName = propertyName;
            this.functionName = functionName;
            this.showCondition = showCondition;
        }
        public void Display()
        {
            if (showCondition())
            {
                if (IsAndroid)
                    androidKey = ShowTextFieldWithRemoveSpace(propertyName, androidKey);
                else
                    iOSKey = ShowTextFieldWithRemoveSpace(propertyName, iOSKey);
            }
        }
        string ShowTextFieldWithRemoveSpace(string fieldName, string text)
        {
            GUIStyle style = new(EditorStyles.textArea)
            {
                wordWrap = true
            };
            EditorGUILayout.BeginHorizontal();
            string fieldText = text;
            GUILayout.Label(fieldName, GUILayout.Width(250));
            fieldText = EditorGUILayout.TextArea(fieldText, style);
            if (!string.IsNullOrEmpty(fieldText) && fieldText.Contains(" "))
            {
                if (GUILayout.Button("FIX", GUILayout.Width(30)))
                {
                    fieldText = fieldText.Replace(" ", "");
                }
            }
            EditorGUILayout.EndHorizontal();
            return fieldText;
        }
        public void GetValuesFromFile(string filePath)
        {
            androidKey = GetKey(functionName, true, filePath);
            iOSKey = GetKey(functionName, false, filePath);
        }
        public bool GetIsChanged(string filePath)
        {
            return androidKey != GetKey(functionName, true, filePath) || iOSKey != GetKey(functionName, false, filePath);
        }
        bool IsAndroid
        {
            get
            {
#if UNITY_ANDROID
                return true;
#elif UNITY_IOS
                return false;
#else
                return true;
#endif
            }
        }
        public enum Platform
        {
            Android,
            iOS,
            Other
        }
        private const string Pattern = @"public static string {0}\(\)\s*{{\s*#if UNITY_ANDROID\s*return ""(.*?)"";\s*#elif UNITY_IOS\s*return ""(.*?)"";\s*#else\s*return """";\s*#endif\s*}}";
        public string GetKey(string functionName, bool isAndroid, string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"File not found: {filePath}");
                return "";
            }
            string fileContent = File.ReadAllText(filePath);
            string formattedPattern = string.Format(Pattern, functionName);
            Match match = Regex.Match(fileContent, formattedPattern, RegexOptions.Singleline);
            if (match.Success)
            {
                Platform platform = isAndroid ? Platform.Android : Platform.iOS;
                int groupIndex = (platform == Platform.Android) ? 1 : (platform == Platform.iOS) ? 2 : -1;
                if (groupIndex != -1)
                {
                    return match.Groups[groupIndex].Value;
                }
            }
            return "";
        }
    }

    static bool IsThreeTierIntersititial()
    {
#if THREE_TIER_INTER
        return true;
#else
        return false;
#endif
    }

    static bool IsThreeTierReward()
    {
#if THREE_TIER_REWARD
        return true;
#else
        return false;
#endif
    }

    static bool IsThreeTierBanner()
    {
#if THREE_TIER_BANNER
        return true;
#else
        return false;
#endif
    }

    static bool IsIS()
    {
#if USE_IS
        return true;
#else
        return false;
#endif
    }
    static bool IsIdleMREC_MAX()
    {
#if USE_IDLE_MREC
        return true;
#else
        return false;
#endif
    }
    static bool IsMAX_OpenAds()
    {
#if USE_MAX_OPENADS
        return true;
#else
        return false;
#endif
    }
    static bool IsAdMob_SimpleBanner()
    {
#if  USE_ADMOB_SIMPLE_BANNER
        return true;
#else
        return false;
#endif
    }
    static bool IsAdMob_Intersititial()
    {
#if  USE_ADMOB_INTERSITITIAL_AD
        return true;
#else
        return false;
#endif
    }
    static bool IsAdMob_Reward()
    {
#if  USE_ADMOB_REWARD_AD
        return true;
#else
        return false;
#endif
    }
    static bool IsAdMob_RewardIntersititial()
    {
#if  USE_ADMOB_REWARD_INTERSITITIAL_AD
        return true;
#else
        return false;
#endif
    }
    static bool IsAdMob_Static()
    {
#if  USE_ADMOB_STATIC_AD
        return true;
#else
        return false;
#endif
    }
    static bool IsAdMob_MrecBanner()
    {
#if  USE_ADMOB_MREC_BANNER
        return true;
#else
        return false;
#endif
    }
    static bool IsAdMob_OpenAds()
    {
#if  USE_ADMOB_OPEN_AD_8_5
        return true;
#else
        return false;
#endif
    }
    static bool IsAdMob_RewardedInterstitial()
    {
#if USE_ADMOB_REWARDED_INTERSITIAL
        return true;
#else
        return false;
#endif
    }
    static bool IsMRECIS()
    {
#if USE_IS_NATIVE_MREC
        return true;
#else
        return false;
#endif
    }
    static bool IsSecondIS()
    {
#if USE_IS_SECOND_INTERSITITIAL
        return true;
#else
        return false;
#endif
    }
    static bool IsMAX_LowInterstitial()
    {
#if USE_MAX_LOW_INTERSITITAL
        return true;
#else
        return false;
#endif
    }
    public enum IDType
    {
        SDKKey,
        InterstitialAdUnitId,
        RewardedAdUnitId,
        BannerAdUnitId,
        MRECBannerAdUnitId,
        IdleMRECBannerAdUnitId,
        AppOpenAdUnitId_MAX,
        AppOpenAdTier1,
        AppOpenAdTier2,
        AppOpenAdTier3,
        LowLPInterstitialAdUnitId,
        AdmobBanner,
        AdmobMrecBanner,
        AdmobIntersititial,
        AdmobStatic,
        AdmobReward,
        AdmobRewardIntersititial,
        InterstitialAdUnitIdTier1,
        InterstitialAdUnitIdTier2,
        InterstitialAdUnitIdTier3,
        RewardAdUnitIdTier1,
        RewardAdUnitIdTier2,
        RewardAdUnitIdTier3,
        BannerAdUnitIdTier1,
        BannerAdUnitIdTier2,
        BannerAdUnitIdTier3
    }
    Dictionary<IDType, AdKeyData> adsKeyData = new()
    {
        { IDType.SDKKey, new("IS SDK Key", "SDKKey", IsIS) },
        { IDType.InterstitialAdUnitId, new("IS Interstitial Ad Id", "InterstitialAdUnitId", () => IsIS() && !IsThreeTierIntersititial()) },
        { IDType.InterstitialAdUnitIdTier1, new("IS Interstitial Tier 1 Ad Id", "InterstitialAdUnitIdTier1", () => IsIS() && IsThreeTierIntersititial()) },
        { IDType.InterstitialAdUnitIdTier2, new("IS Interstitial Tier 2 Ad Id", "InterstitialAdUnitIdTier2", () => IsIS() && IsThreeTierIntersititial()) },
        { IDType.InterstitialAdUnitIdTier3, new("IS Interstitial Tier 3 Ad Id", "InterstitialAdUnitIdTier3", () => IsIS() && IsThreeTierIntersititial()) },
        { IDType.LowLPInterstitialAdUnitId, new("IS Second Interstitial Ad Id", "SecondInterstitialAdUnitId", () => IsIS() && IsSecondIS()) },
        { IDType.RewardedAdUnitId, new("IS Rewarded Ad Id", "RewardedAdUnitId", () => IsIS() && !IsThreeTierReward()) },
        { IDType.RewardAdUnitIdTier1, new("IS Reward Tier 1 Ad Id", "RewardedAdUnitIdTier1", () => IsIS() && IsThreeTierReward()) },
        { IDType.RewardAdUnitIdTier2, new("IS Reward Tier 2 Ad Id", "RewardedAdUnitIdTier2", () => IsIS() && IsThreeTierReward()) },
        { IDType.RewardAdUnitIdTier3, new("IS Reward Tier 3 Ad Id", "RewardedAdUnitIdTier3", () => IsIS() && IsThreeTierReward()) },
        { IDType.BannerAdUnitId, new("IS Banner Ad Id", "BannerAdUnitId", () => IsIS() && !IsThreeTierBanner()) },
        { IDType.BannerAdUnitIdTier1, new("IS Banner Tier 1 Ad Id", "BannerAdUnitIdTier1", () => IsIS() && IsThreeTierBanner()) },
        { IDType.BannerAdUnitIdTier2, new("IS Banner Tier 2 Ad Id", "BannerAdUnitIdTier2", () => IsIS() && IsThreeTierBanner()) },
        { IDType.BannerAdUnitIdTier3, new("IS Banner Tier 3 Ad Id", "BannerAdUnitIdTier3", () => IsIS() && IsThreeTierBanner()) },
        { IDType.MRECBannerAdUnitId, new("IS MREC Id", "MRECBannerAdUnitId", () => IsIS() && IsMRECIS()) },
        { IDType.AdmobBanner, new("Admob Banner Id", "BannerAdUnitIdAdmob", IsAdMob_SimpleBanner) },
        { IDType.AdmobMrecBanner, new("Admob MREC Id", "MrecAdUnitIdAdmob", IsAdMob_MrecBanner) },
        { IDType.AdmobStatic, new("Admob Static Id", "StaticAdUnitIdAdmob", IsAdMob_Static) },
        { IDType.AdmobIntersititial, new("Admob Interstitial Id", "InterstitialAdUnitIdAdmob", IsAdMob_Intersititial) },
        { IDType.AdmobReward, new("Admob Reward Id", "RewardAdUnitIdAdmob", IsAdMob_Reward) },
        { IDType.AdmobRewardIntersititial, new("Admob Reward Interstitial Id", "RewardIntersititialAdUnitIdAdmob", IsAdMob_RewardIntersititial) },
        { IDType.AppOpenAdTier1, new("App OpenAd Tier 1", "AppOpenAdTier1", IsAdMob_OpenAds) },
        { IDType.AppOpenAdTier2, new("App OpenAd Tier 2", "AppOpenAdTier2", IsAdMob_OpenAds) },
        { IDType.AppOpenAdTier3, new("App OpenAd Tier 3", "AppOpenAdTier3", IsAdMob_OpenAds) },

    };
    static string filePathAfterAsset = "_AdsData/Scripts/_Ads/AdsIds.cs";
    [MenuItem("Finz/AdsWindow")]
    private static void ShowWindow()
    {
        //CheckRemoteUpdate();
        var window = GetWindow<AdsEditorWindow>();
        window.titleContent = new GUIContent("AdsWindow");
        window.Show();
    }
    private void OnEnable()
    {
        UpdateInitialKeys();
        // CheckRemoteUpdate();
    }
    private void OnLostFocus()
    {
        string filePath = Path.Combine(Application.dataPath, filePathAfterAsset);
        foreach (var item in adsKeyData)
        {
            if (item.Value.GetIsChanged(filePath))
            {
                UpdateScript(filePath);
                break;
            }
        }
    }
    private Vector2 scrollPos;

    void ShowFeatures()
    {
        GUILayout.Label("Symbols : ", EditorStyles.boldLabel);
        AddRemoveSymbol("IS SDK", "USE_IS");
        AddRemoveSymbol("IS MREC", "USE_IS_NATIVE_MREC");
        AddRemoveSymbol("IS THREE TIER INTERSITITIAL", "THREE_TIER_INTER");
     //   AddRemoveSymbol("IS THREE TIER REWARD", "THREE_TIER_REWARD");
        AddRemoveSymbol("IS SECOND INTERSITITIAL", "USE_IS_SECOND_INTERSITITIAL");
        AddRemoveSymbol("IS AD QUALITY", "USE_IS_ADQUALITY");
        AddRemoveSymbol("ADMOB BANNER", "USE_ADMOB_SIMPLE_BANNER");
        AddRemoveSymbol("ADMOB MREC BANNER", "USE_ADMOB_MREC_BANNER");
        AddRemoveSymbol("ADMOB STATIC", "USE_ADMOB_STATIC_AD");
        AddRemoveSymbol("ADMOB INTERSITITIAL", "USE_ADMOB_INTERSITITIAL_AD");
        AddRemoveSymbol("ADMOB REWARD", "USE_ADMOB_REWARD_AD");
        AddRemoveSymbol("ADMOB REWARD INTERSITITIAL", "USE_ADMOB_REWARD_INTERSITITIAL_AD");
        AddRemoveSymbol("ADMOB PAID EVENT", "USE_ADMOB_PAID_EVENT");
        AddRemoveSymbol("ADMOB OPEN AD 7.8", "USE_ADMOB_OPEN_AD_8_5");
        AddRemoveSymbol("FIREBASE", "USE_FIREBASE");
        AddRemoveSymbol("REMOTE CONFIG", "USE_REMOTE_CONFIG");
        AddRemoveSymbol("CRASHLYTICS", "USE_CRASHLYTICS");
        AddRemoveSymbol("NATIVE RATE", "SHOW_NATIVE_RATE");
        AddRemoveSymbol("DELEGATES", "USE_DELEGATES");
        AddRemoveSymbol("PATCH", "USE_PATCH");
        AddRemoveSymbol("APPFLYER 6.15+", "USE_APPSFLYER_6_15");
        AddRemoveSymbol("FACEBOOK SDK", "USE_FACEBOOK");
        AddRemoveSymbol("BYTEBREW SDK", "USE_BYTEBREW");
        AddRemoveSymbol("ODEE SDK", "USE_ODEE");
        AddRemoveSymbol("EDITOR TOOLS", "USE_EDITOR_TOOLS");

    }
    private void AddRemoveSymbol(string symbolName, string symbol)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(symbolName + " :", GUILayout.Width(250));
        if (!AddDefineSymbols.CheckSymbol(symbol))
        {
            if (GUILayout.Button("Add"))
            {
                AddDefineSymbols.Add(symbol);
            }
        }
        else
        {
            if (GUILayout.Button("Remove"))
            {
                AddDefineSymbols.Clear(symbol);
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    void UpdateInitialKeys()
    {
        string filePath = Path.Combine(Application.dataPath, filePathAfterAsset);
        foreach (var item in adsKeyData)
        {
            item.Value.GetValuesFromFile(filePath);
        }
    }
    private void AdsKeysEditor()
    {
        GUILayout.Label("Ads keys : ", EditorStyles.boldLabel);
        string filePath = Path.Combine(Application.dataPath, filePathAfterAsset); ;
        /*if (GUILayout.Button("Update Initial Keys"))
        {
            UpdateInitialKeys();
        }*/
        foreach (var item in adsKeyData)
        {
            item.Value.Display();
        }
        /*if (GUILayout.Button("OverrideKeys"))
        {
            UpdateScript(filePath);
        }*/
    }
    void UpdateScript(string fileName)
    {
        string newString = $@"
using Sirenix.OdinInspector;
using UnityEngine;
public static class AdsIds
{{
    #if USE_IS
    public static string SDKKey()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.SDKKey].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.SDKKey].iOSKey}"";
        #else
            return """";
        #endif
    }}
      public static string InterstitialAdUnitId()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.InterstitialAdUnitId].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.InterstitialAdUnitId].iOSKey}"";
        #else
            return """";
        #endif
    }}
    public static string RewardedAdUnitId()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.RewardedAdUnitId].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.RewardedAdUnitId].iOSKey}"";
        #else
            return """";
        #endif
    }}
    public static string BannerAdUnitId()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.BannerAdUnitId].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.BannerAdUnitId].iOSKey}"";
        #else
            return """";
        #endif
    }}
    #if USE_IS_NATIVE_MREC
    public static string MRECBannerAdUnitId()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.MRECBannerAdUnitId].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.MRECBannerAdUnitId].iOSKey}"";
        #else
            return """";
        #endif
    }}
    #endif
#if USE_IS_SECOND_INTERSITITIAL
    public static string SecondInterstitialAdUnitId()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.LowLPInterstitialAdUnitId].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.LowLPInterstitialAdUnitId].iOSKey}"";
        #else
            return """";
        #endif
    }}
    #endif

#if THREE_TIER_INTER
    public static string InterstitialAdUnitIdTier1()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.InterstitialAdUnitIdTier1].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.InterstitialAdUnitIdTier1].iOSKey}"";
        #else
            return """";
        #endif
    }}
    public static string InterstitialAdUnitIdTier2()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.InterstitialAdUnitIdTier2].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.InterstitialAdUnitIdTier2].iOSKey}"";
        #else
            return """";
        #endif
    }}
    public static string InterstitialAdUnitIdTier3()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.InterstitialAdUnitIdTier3].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.InterstitialAdUnitIdTier3].iOSKey}"";
        #else
            return """";
        #endif
    }}
    #endif


#if THREE_TIER_REWARD
    public static string RewardedAdUnitIdTier1()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.RewardAdUnitIdTier1].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.RewardAdUnitIdTier1].iOSKey}"";
        #else
            return """";
        #endif
    }}
    public static string RewardedAdUnitIdTier2()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.RewardAdUnitIdTier2].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.RewardAdUnitIdTier2].iOSKey}"";
        #else
            return """";
        #endif
    }}
    public static string RewardedAdUnitIdTier3()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.RewardAdUnitIdTier3].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.RewardAdUnitIdTier3].iOSKey}"";
        #else
            return """";
        #endif
    }}
    #endif

#if THREE_TIER_BANNER
    public static string BannerAdUnitIdTier1()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.BannerAdUnitIdTier1].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.BannerAdUnitIdTier1].iOSKey}"";
        #else
            return """";
        #endif
    }}
    public static string BannerAdUnitIdTier2()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.BannerAdUnitIdTier2].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.BannerAdUnitIdTier2].iOSKey}"";
        #else
            return """";
        #endif
    }}
    public static string BannerAdUnitIdTier3()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.BannerAdUnitIdTier3].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.BannerAdUnitIdTier3].iOSKey}"";
        #else
            return """";
        #endif
    }}
    #endif


    #endif


   #if  USE_ADMOB_SIMPLE_BANNER
    public static string BannerAdUnitIdAdmob()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.AdmobBanner].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.AdmobBanner].iOSKey}"";
        #else
            return """";
        #endif
    }}
    #endif

       #if  USE_ADMOB_MREC_BANNER
    public static string MrecAdUnitIdAdmob()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.AdmobMrecBanner].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.AdmobMrecBanner].iOSKey}"";
        #else
            return """";
        #endif
    }}
    #endif

           #if  USE_ADMOB_STATIC_AD
    public static string StaticAdUnitIdAdmob()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.AdmobStatic].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.AdmobStatic].iOSKey}"";
        #else
            return """";
        #endif
    }}
    #endif

       #if  USE_ADMOB_INTERSITITIAL_AD
    public static string InterstitialAdUnitIdAdmob()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.AdmobIntersititial].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.AdmobIntersititial].iOSKey}"";
        #else
            return """";
        #endif
    }}
    #endif

       #if  USE_ADMOB_REWARD_AD
    public static string RewardAdUnitIdAdmob()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.AdmobReward].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.AdmobReward].iOSKey}"";
        #else
            return """";
        #endif
    }}
    #endif

       #if  USE_ADMOB_REWARD_INTERSITITIAL_AD
    public static string RewardIntersititialAdUnitIdAdmob()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.AdmobRewardIntersititial].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.AdmobRewardIntersititial].iOSKey}"";
        #else
            return """";
        #endif
    }}
    #endif

    #if  USE_ADMOB_OPEN_AD_8_5
    public static string AppOpenAdTier1()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.AppOpenAdTier1].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.AppOpenAdTier1].iOSKey}"";
        #else
            return """";
        #endif
    }}
    public static string AppOpenAdTier2()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.AppOpenAdTier2].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.AppOpenAdTier2].iOSKey}"";
        #else
            return """";
        #endif
    }}
    public static string AppOpenAdTier3()
    {{
        #if UNITY_ANDROID
            return ""{adsKeyData[IDType.AppOpenAdTier3].androidKey}"";
        #elif UNITY_IOS
            return ""{adsKeyData[IDType.AppOpenAdTier3].iOSKey}"";
        #else
            return """";
        #endif
    }}
    #endif


    


}}";
        try
        {
            File.WriteAllText(fileName, newString);
            AssetDatabase.Refresh();
            Debug.Log($"BILLI : File Write Success");
        }
        catch (Exception error)
        {
            Debug.LogError("Error creating file: " + error.Message);
        }
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        // Left panel
        GUILayout.BeginVertical(GUILayout.Width(200), GUILayout.ExpandHeight(true));

        // Header moved above Quick Links
        GUILayout.Space(5);
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = 12
        };
        GUILayout.Label("Finz Ads Plugin Version 8.1.0", headerStyle);
        GUILayout.Space(5);

        // Quick Links
        DrawQuickLinks();

        GUILayout.EndVertical();

        // Right panel
        EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        AdsKeysEditor();
        GUILayout.Space(10);
        ShowFeatures();
        GUILayout.Space(10);

        GUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }
    private void DrawQuickLinks()
    {
        GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(200));

        // Header
        GUILayout.Label("Quick Links", EditorStyles.boldLabel);

        // Buttons
        if (GUILayout.Button("Dev Talk", GUILayout.Height(30)))
            Application.OpenURL("https://docs.google.com/spreadsheets/d/1B_BR8wsZLXTV3MSw3pFX0_KABjJaTgvJAL9myRb1-cE/edit?gid=0#gid=0");

        if (GUILayout.Button("Dev Drive", GUILayout.Height(30)))
            Application.OpenURL("https://drive.google.com/drive/u/2/folders/1EVlk0wjyl5hUpLcHS0MCET8hAgxJcSIF");

        if (GUILayout.Button("Bug Report", GUILayout.Height(30)))
            Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSdcnaj1URaW7vjR7S7aG7_OxCQEFq0i_cTZ_F6tDemUysDM1g/viewform");

        if (GUILayout.Button("Change Logs", GUILayout.Height(30)))
            Application.OpenURL("https://docs.google.com/spreadsheets/d/1B_BR8wsZLXTV3MSw3pFX0_KABjJaTgvJAL9myRb1-cE/edit?gid=11723955#gid=11723955");

        if (GUILayout.Button("Check For Update", GUILayout.Height(30)))
        {
            CheckRemoteUpdate();
        }

        //if (GUILayout.Button("Validate", GUILayout.Height(30)))
        //{
        //    checkStripEngine();
        //    CheckTargetDevices();
        //    CheckTargetArchitectures();
        //    CheckScriptingBackend();
        //    CheckTargetApiLevel();
        //    CheckMinApiLevel();
        //    CheckAdMobAppId();
        //    CheckIronSource();
        //    CheckIronsourceAdmobID();
        //    CheckMissingAndroidPermissions();
        //}
#if UNITY_ANDROID
        if (GUILayout.Button("Permissions", GUILayout.Height(30)))
        {
            AndroidPermissionsWindow.ShowWindow();
            // Add Android-specific permissions logic here
        }
#endif

        if (GUILayout.Button("Packages", GUILayout.Height(30)))
        {
            FinzPackageInstaller.ShowWindow();
            Debug.Log("📦 Packages button clicked");
            // Future: Show package manager integration
        }
        if (GUILayout.Button("SKDs", GUILayout.Height(30)))
        {
            FinzAssetImporterWindow.ShowWindow();
          
            // Future: Show package manager integration
        }

        GUILayout.Space(10);

        // Logo
        Texture2D logo = Resources.Load<Texture2D>("finz_logo"); // Make sure it's "finz_logo.png" in Resources
        if (logo != null)
        {
            float maxWidth = 120;
            float aspectRatio = (float)logo.width / logo.height;
            float height = maxWidth / aspectRatio;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(logo, GUILayout.Width(maxWidth), GUILayout.Height(height));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.Label("Logo not found", EditorStyles.helpBox);
        }

        GUILayout.EndVertical();
    }

    #region Validator
    public static bool checkStripEngine()
    {
        var level = PlayerSettings.GetManagedStrippingLevel(EditorUserBuildSettings.selectedBuildTargetGroup);

        Debug.Log($"Managed Stripping Level is set to: {level}");

        var playerSettingsAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0];
        SerializedObject serializedObject = new SerializedObject(playerSettingsAsset);
        SerializedProperty stripEngineCodeProp = serializedObject.FindProperty("stripEngineCode");

        if (stripEngineCodeProp != null)
        {
            Debug.Log($"Strip Engine Code is {(stripEngineCodeProp.boolValue ? "ENABLED" : "DISABLED")}.");
        }
        else
        {
            Debug.LogWarning("Could not locate the 'stripEngineCode' property in ProjectSettings.");
        }
        if (stripEngineCodeProp.boolValue)
        {
            EditorUtility.DisplayDialog("Error", "Strip Engine is enabled. Disable it!!", "OK");
            return true;
        }
        else {
            return false;
        }
    }
    public static bool CheckTargetDevices()
    {
        var settingsAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0];
        var serializedObject = new SerializedObject(settingsAsset);
        SerializedProperty property = serializedObject.FindProperty("AndroidTargetDevices");

        if (property != null)
        {
            int intValue = property.intValue;

            string label = intValue switch
            {
                0 => "All Devices",
                1 => "Phones, Tablets, and TV Devices Only",
                2 => "Chrome OS Devices Only",
                _ => $"Unknown (value: {intValue})"
            };

            Debug.Log($"Target Devices setting: {label} (Raw enum value: {intValue})");
            if (intValue != 1)
            {
               
                EditorUtility.DisplayDialog("Error", "Target devices is not set to Phones, Tablets, and TV Devices Only", "OK");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("Could not find 'AndroidTargetDevices' property.");
        }
        return true;
    }
    public static bool CheckTargetArchitectures()
    {
#if UNITY_ANDROID
        var arch = PlayerSettings.Android.targetArchitectures;

        string result = "Target Architectures:\n";
        if ((arch & AndroidArchitecture.ARMv7) == 0)
        {
            EditorUtility.DisplayDialog("Error", "Architectures ARMv7 is diabled", "OK");
            return false;
        }
        if ((arch & AndroidArchitecture.ARM64) == 0)
        {
            EditorUtility.DisplayDialog("Error", "Architectures ARM64 is diabled", "OK");
            return false;
        }
        if ((arch & AndroidArchitecture.X86_64) == 0)
        {
            EditorUtility.DisplayDialog("Error", "Architectures X86_64 is diabled", "OK");
            return false;
        }
#else
        Debug.LogWarning("This check is only valid for Android platform.");
#endif
        return true;
    }
    public static bool CheckScriptingBackend()
    {
        BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

        var backend = PlayerSettings.GetScriptingBackend(targetGroup);

        if (backend != ScriptingImplementation.IL2CPP)
        {
            EditorUtility.DisplayDialog("Error", "Current scripting backend is not IL2CPP ", "OK");
            return false;
        }
        return true;
        Debug.Log($"Current scripting backend for {targetGroup}: {backend}");
    }
    public static bool CheckTargetApiLevel()
    {
#if UNITY_ANDROID
        AndroidSdkVersions apiLevel = PlayerSettings.Android.targetSdkVersion;

        if (apiLevel == AndroidSdkVersions.AndroidApiLevelAuto)
        {

            Debug.Log("Target API Level is set to: Automatic (highest installed)");
        }
        else
        {
            Debug.Log($"Target API Level is set to: {apiLevel} ({(int)apiLevel})");
        }
        if (((int)apiLevel) < 34)
        {
            EditorUtility.DisplayDialog("Error", "Current API target is not 34 ", "OK");
            return false;
        }
#else
        Debug.LogWarning("This check is only valid for Android platform.");
#endif
        return true;
    }
    public static bool CheckMinApiLevel()
    {
#if UNITY_ANDROID
        AndroidSdkVersions minApiLevel = PlayerSettings.Android.minSdkVersion;
        Debug.Log($"Minimum API Level is set to: {minApiLevel} ({(int)minApiLevel})");
        if (((int)minApiLevel) < 24)
        {
            EditorUtility.DisplayDialog("Error", "Current Minimum API Level target is less than 24 ", "OK");
            return false;
        }
#else
        Debug.LogWarning("This check is only valid for Android platform.");
#endif
        return true;
    }
    public static bool CheckAdMobAppId()
    {
        var settings = Resources.Load("GoogleMobileAdsSettings");

        if (settings == null)
        {
            Debug.LogWarning("❌ GoogleMobileAdsSettings asset not found in Resources.");
            return true;
        }

        var type = settings.GetType();
        var androidProp = type.GetProperty("GoogleMobileAdsAndroidAppId");
        var iosProp = type.GetProperty("GoogleMobileAdsIOSAppId");

        var androidAppId = androidProp?.GetValue(settings)?.ToString();
        var iosAppId = iosProp?.GetValue(settings)?.ToString();

        //var androidAppId = androidField?.GetValue(settings)?.ToString();
        //var iosAppId = iosField?.GetValue(settings)?.ToString();

        Debug.Log($"✅ AdMob App IDs Found:\n→ Android: {androidAppId}\n→ iOS: {iosAppId}");

#if UNITY_ANDROID
        if (androidAppId == null ||string.IsNullOrEmpty(androidAppId))
        {
            EditorUtility.DisplayDialog("Error", "Admob App Id is null ", "OK");
            return false;
        }
        else
        {
            string input = androidAppId;

            bool hasLeadingOrTrailingSpaces = input != input.Trim();

            if (hasLeadingOrTrailingSpaces)
            {
                EditorUtility.DisplayDialog("Error", "Admob App Id is has spaces ", "OK");
                return false;
                Debug.Log("The string has leading or trailing spaces.");
            }
            else
            {
                Debug.Log("The string is clean on both ends.");
            }

        }

#else
        if (iosAppId == null ||string.IsNullOrEmpty(iosAppId))
        {
            EditorUtility.DisplayDialog("Error", "Admob App Id is null ", "OK");
            return true;
        }
        else
        {
            string input = iosAppId;

            bool hasLeadingOrTrailingSpaces = input != input.Trim();

            if (hasLeadingOrTrailingSpaces)
            {
                EditorUtility.DisplayDialog("Error", "Admob App Id is has spaces ", "OK");
                return false;
                Debug.Log("The string has leading or trailing spaces.");
            }
            else
            {
                Debug.Log("The string is clean on both ends.");
            }

        }
#endif
        return true;
    }
    public static bool CheckIronSource()
    {
        bool folderExists = AssetDatabase.IsValidFolder("Assets/LevelPlay");
        if (folderExists)
        {
            var settings = Resources.Load("IronSourceMediationSettings");


            if (settings == null)
            {
                Debug.LogWarning("❌ Could not load IronSourceMediationSettings from Resources.");
                return true;
            }

            SerializedObject serialized = new SerializedObject(settings);
            SerializedProperty androidAppIdProp = serialized.FindProperty("AndroidAppKey");
            SerializedProperty iosAppIdProp = serialized.FindProperty("IOSAppKey");
            // string iosId = iosAppIdProp?.stringValue ?? "(missing)";

#if UNITY_ANDROID
            if (androidAppIdProp != null)
            {
                string appId = androidAppIdProp.stringValue;

                if (string.IsNullOrEmpty(appId))
                {
                    Debug.LogError("❌ LevelPlay Android App ID is null or empty!");
                    EditorUtility.DisplayDialog("Error", "LP App Id is has spaces ", "OK");
                    return false;
                }
                else
                {
                    Debug.Log($"✅ LevelPlay Android App ID is set: {appId}");
                }
            }
            else
            {
                Debug.LogWarning("❌ 'AndroidAppKey' property not found in IronSourceMediationSettings. Check if it's serialized.");
            }
#else

            if (iosAppIdProp != null)
            {
                string appId = iosAppIdProp.stringValue;

                if (string.IsNullOrEmpty(appId))
                {
                    Debug.LogError("❌ LevelPlay IOS App ID is null or empty!");
                    EditorUtility.DisplayDialog("Error", "LP App Id is has spaces ", "OK");
                    return false;
                }
                else
                {
                    Debug.Log($"✅ LevelPlay Android App ID is set: {appId}");
                }
            }
            else
            {
                Debug.LogWarning("❌ 'AndroidAppKey' property not found in IronSourceMediationSettings. Check if it's serialized.");
            }
#endif

            return true;
        }
        return true;
    }
    public static bool CheckIronsourceAdmobID()
    {
        bool folderExists = AssetDatabase.IsValidFolder("Assets/LevelPlay");
        if (folderExists)
        {
            // 1. Load via Resources
            var settings = Resources.Load("IronSourceMediatedNetworkSettings");

            if (settings == null)
            {
                Debug.LogError("❌ Could not find 'IronSourceMediatedNetworkSettings' in Resources.");
                return true;
            }

            Debug.Log($"✅ Loaded settings from Resources path. Object type: {settings.GetType().Name}");

            // 2. Log raw field values directly
            var type = settings.GetType();

            var androidField = type.GetField("AdmobAndroidAppId");
            var iosField = type.GetField("AdmobIOSAppId");



            Debug.Log(androidField != null
                ? $"✅ androidField exists. Value: {androidField.GetValue(settings)}"
                : "❌ androidField NOT found");

#if UNITY_ANDROID
            string appId = androidField.GetValue(settings).ToString();
            if (string.IsNullOrEmpty(appId))
            {
                EditorUtility.DisplayDialog("Error", "LP Admob ID is Missing", "OK");
                return false;
            }
#else
            string appIdios = iosField.GetValue(settings).ToString();
            if (string.IsNullOrEmpty(appIdios))
            {
                EditorUtility.DisplayDialog("Error", "LP Admob ID is Missing", "OK");
                return false;
            }
#endif

            Debug.Log(iosField != null
                ? $"✅ iosField exists. Value: {iosField.GetValue(settings)}"
                : "❌ iosField NOT found");

            // 3. Log asset path of the real source
            string[] paths = AssetDatabase.FindAssets("IronSourceMediatedNetworkSettings");
            foreach (var guid in paths)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log($"📦 Found matching asset at: {path}");
            }
            return true;
        }
        return true;
    }
    private static readonly string ManifestPath = "Assets/Plugins/Android/AndroidManifest.xml";
    private static readonly string[] RequiredPermissions = new string[]
    {
        "android.permission.INTERNET",
        "android.permission.ACCESS_NETWORK_STATE",
        "com.google.android.gms.permission.AD_ID",
        "android.permission.READ_PHONE_STATE",
        "android.permission.ACCESS_WIFI_STATE"
    };
    public static bool CheckMissingAndroidPermissions()
    {
#if !UNITY_ANDROID
        return true;
#endif

        if (!File.Exists(ManifestPath))
        {
            Debug.LogError("❌ AndroidManifest.xml is missing at: " + ManifestPath);
         
            EditorUtility.DisplayDialog("Error", "AndroidManifest.xml is missing ", "OK");
            return false;
        }

        XmlDocument manifestDoc = new XmlDocument();
        manifestDoc.Load(ManifestPath);

        string manifestContent = manifestDoc.OuterXml;
        List<string> missingPermissions = new List<string>();

        foreach (string permission in RequiredPermissions)
        {
            if (!manifestContent.Contains(permission))
            {
                missingPermissions.Add(permission);
            }
        }

        if (missingPermissions.Count == 0)
        {
            Debug.Log("✅ All required permissions are present in AndroidManifest.xml.");
            return true;
        }
        else
        {
            Debug.LogWarning("⚠️ Missing permissions in AndroidManifest.xml:");
            foreach (string p in missingPermissions)
            {
                Debug.LogWarning("   → " + p);
            }

            EditorUtility.DisplayDialog("Error", "Permissions are missing in the Android Manifest ", "OK");
            return false;
        }
        return true;
    }


#endregion

#region Update
    static string CurrentVersion = "8.1.0";
    public static void CheckForUpdateFromStartup()
    {
        Debug.Log("?? [UpdateCheck] Running CheckRemoteUpdate from startup...");
        AdsEditorWindow tempWindow = CreateInstance<AdsEditorWindow>();
        tempWindow.CheckRemoteUpdate();
    }

    private EditorApplication.CallbackFunction updateCallback;

    public void CheckRemoteUpdate()
    {
        Debug.Log("🚀 Starting update check...");

        string updateUrl = "https://drive.google.com/uc?export=download&id=10yFHkY8ki1N0pvkGMQdrZAg8bJN4gTK6";
        UnityWebRequest request = UnityWebRequest.Get(updateUrl);
        request.SendWebRequest();

        updateCallback = () =>
        {
            if (request.isDone)
            {
                EditorApplication.update -= updateCallback;

                if (string.IsNullOrEmpty(request.error))
                {
                    try
                    {
                        UpdateData data = JsonUtility.FromJson<UpdateData>(request.downloadHandler.text);

                        if (data != null && data.version != CurrentVersion)
                        {
                            bool open = EditorUtility.DisplayDialog(
                                $"Finz Plugin {data.version} Available",
                                $"New Version: {data.version}\n\nChangelog:\n{data.changelog}",
                                "Download",
                                "Later"
                            );
 
                            if (open)
                            {
                                Application.OpenURL(data.downloadUrl);
                            }
                        }
                        else
                        {
                            EditorUtility.DisplayDialog($"No Update Available",
                                $"Latest Version: {data.version}\n\nChangelog:\n{data.changelog}", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("Update check failed: " + ex.Message);
                    }
                }
                else
                {
                    Debug.LogWarning("Update check failed: " + request.error);
                }
            }
        };

        EditorApplication.update += updateCallback;
    }
#endregion

}




