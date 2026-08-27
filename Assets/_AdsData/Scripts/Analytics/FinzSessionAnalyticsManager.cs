using System;
using System.Collections;
using System.Collections.Generic;
#if USE_FIREBASE
using Firebase.Analytics;

#endif
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
#if USE_APPSFLYER|| USE_APPSFLYER_6_15
using AppsFlyerSDK;
#endif

public class FinzSessionAnalyticsManager : MonoBehaviour
{
    public enum SessionThreshold
    {
        Session_1_Minutes,
        Session_2_Minutes,
        Session_3_Minutes,
        Session_4_Minutes,
        Session_5_Minutes,
        Session_10_Minutes,
        Session_15_Minutes,
        Session_20_Minutes,
        Session_25_Minutes,
        Session_30_Minutes,
        Session_35_Minutes,
        Session_40_Minutes
    }

    [BoxGroup("1 minutes")] [SerializeField] private bool for1minutes = false;
    [BoxGroup("1 minutes")] public delegate void on1minute();
    [BoxGroup("1 minutes")] public static event on1minute on1minuteMethod;

    [BoxGroup("2 minutes")] [SerializeField] private bool for2minutes = false;
    [BoxGroup("2 minutes")] public delegate void on2minute();
    [BoxGroup("2 minutes")] public static event on2minute on2minuteMethod;

    [BoxGroup("3 minutes")] [SerializeField] private bool for3minutes = false;
    [BoxGroup("3 minutes")] public delegate void on3minute();
    [BoxGroup("3 minutes")] public static event on3minute on3minuteMethod;

    [BoxGroup("4 minutes")] [SerializeField] private bool for4minutes = false;
    [BoxGroup("4 minutes")] public delegate void on4minute();
    [BoxGroup("4 minutes")] public static event on4minute on4minuteMethod;

    [BoxGroup("5 minutes")] [SerializeField] private bool for5minutes = false;
    [BoxGroup("5 minutes")] public delegate void on5minute();
    [BoxGroup("5 minutes")] public static event on5minute on5minuteMethod;

    [BoxGroup("10 minutes")] [SerializeField] private bool for10minutes = false;
    [BoxGroup("10 minutes")] public delegate void on10minute();
    [BoxGroup("10 minutes")] public static event on10minute on10minuteMethod;

    [BoxGroup("15 minutes")] [SerializeField] private bool for15minutes = false;
    [BoxGroup("15 minutes")] public delegate void on15minute();
    [BoxGroup("15 minutes")] public static event on15minute on15minuteMethod;

    [BoxGroup("20 minutes")] [SerializeField] private bool for20minutes = false;
    [BoxGroup("20 minutes")] public delegate void on20minute();
    [BoxGroup("20 minutes")] public static event on20minute on20minuteMethod;

    [BoxGroup("25 minutes")] [SerializeField] private bool for25minutes = false;
    [BoxGroup("30 minutes")] public delegate void on25minute();
    [BoxGroup("30 minutes")] public static event on25minute on25minuteMethod;


    [BoxGroup("30 minutes")] [SerializeField] private bool for30minutes = false;
    [BoxGroup("30 minutes")] public delegate void on30minute();
    [BoxGroup("30 minutes")] public static event on30minute on30minuteMethod;

    [BoxGroup("35 minutes")] [SerializeField] private bool for35minutes = false;
    [BoxGroup("35 minutes")] public delegate void on35minute();
    [BoxGroup("35 minutes")] public static event on35minute on35minuteMethod;

    [BoxGroup("40 minutes")] [SerializeField] private bool for40minutes = false;
    [BoxGroup("40 minutes")] public delegate void on40minute();
    [BoxGroup("40 minutes")] public static event on40minute on40minuteMethod;
    [BoxGroup("INFO")] public int currentMinutes;

    public static FinzSessionAnalyticsManager Instance = null;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        currentMinutes = 0;
        if (FinzAnalysisManager.instance)
        {
            for (int i = 0; i < 1201; i++)
            {
                yield return new WaitForSecondsRealtime(1);

                if (i == 60 && for1minutes)
                {
                    SendAnalytics(SessionThreshold.Session_1_Minutes.ToString());
                    currentMinutes = 1;
                    if (on1minuteMethod != null)
                    {
                        on1minuteMethod();
                    }
                }
                else if (i == 120 && for2minutes)
                {
                    SendAnalytics(SessionThreshold.Session_2_Minutes.ToString());
                    currentMinutes = 2;
                    if (on2minuteMethod != null)
                    {
                        on2minuteMethod();
                    }
                }
                else if (i == 180 && for3minutes)
                {
                    SendAnalytics(SessionThreshold.Session_3_Minutes.ToString());
                    currentMinutes = 3;
                    if (on3minuteMethod != null)
                    {
                        on3minuteMethod();
                    }
                }
                else if (i == 240 && for4minutes)
                {
                    SendAnalytics(SessionThreshold.Session_4_Minutes.ToString());
                    currentMinutes = 4;
                    if (on4minuteMethod != null)
                    {
                        on4minuteMethod();
                    }
                }
                else if (i == 300 && for5minutes)
                {
                    SendAnalytics(SessionThreshold.Session_5_Minutes.ToString());
                    currentMinutes = 5;
                    if (on5minuteMethod != null)
                    {
                        on5minuteMethod();
                    }
                }
                else if (i == 600 && for10minutes)
                {
                    SendAnalytics(SessionThreshold.Session_10_Minutes.ToString());
                    currentMinutes = 10;
                    if (on10minuteMethod != null)
                    {
                        on10minuteMethod();
                    }
                }
                else if (i == 900 && for15minutes)
                {
                    SendAnalytics(SessionThreshold.Session_15_Minutes.ToString());
                    currentMinutes = 15;
                    if (on15minuteMethod != null)
                    {
                        on15minuteMethod();
                    }
                }
                else if (i == 1200 && for20minutes)
                {
                    SendAnalytics(SessionThreshold.Session_20_Minutes.ToString());
                    currentMinutes = 20;
                    if (on20minuteMethod != null)
                    {
                        on20minuteMethod();
                    }
                }
                else if (i == 1500 && for25minutes)
                {
                    SendAnalytics(SessionThreshold.Session_25_Minutes.ToString());
                    currentMinutes = 25;
                    if (on25minuteMethod != null)
                    {
                        on25minuteMethod();
                    }
                }
                else if (i == 1800 && for30minutes)
                {
                    SendAnalytics(SessionThreshold.Session_30_Minutes.ToString());
                    currentMinutes = 30;
                    if (on30minuteMethod != null)
                    {
                        on30minuteMethod();
                    }
                }
                else if (i == 2100 && for35minutes)
                {
                    SendAnalytics(SessionThreshold.Session_35_Minutes.ToString());
                    currentMinutes = 35;
                    if (on35minuteMethod != null)
                    {
                        on35minuteMethod();
                    }
                }
                else if (i == 2400 && for40minutes)
                {
                    SendAnalytics(SessionThreshold.Session_40_Minutes.ToString());
                    currentMinutes = 40;
                    if (on40minuteMethod != null)
                    {
                        on40minuteMethod();
                    }
                }
                else
                {


                }
            }
        }

    }


    #region Events

    private void SendAnalytics(string eventName)
    {
        if (FinzAnalysisManager.instance == null)
        {
            return;

        }
        if (!FinzAnalysisManager.instance.sendAnalytics) {
            return;
        }

#if USE_APPSFLYER || USE_APPSFLYER_6_15
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add("SESSION", eventName);
        AppsFlyer.sendEvent("SESSION", eventValues);
#endif
#if USE_FIREBASE
        if (FinzAnalysisManager.Instance && FinzAnalysisManager.Instance.sendAnalytics == false)
            return;
        try
        {
            //            Debug.LogError(eventName);
            FirebaseAnalytics.LogEvent(eventName);
           
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

#endif
    }
    #endregion



}
