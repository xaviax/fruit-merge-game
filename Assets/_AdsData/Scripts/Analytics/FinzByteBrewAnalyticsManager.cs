using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

#if USE_BYTEBREW
using ByteBrewSDK;
#endif
public class FinzByteBrewAnalyticsManager : MonoBehaviour
{

#if USE_BYTEBREW
    [BoxGroup("AD DELAY")] public bool shouldUpdateTimeDelay = false;
    [BoxGroup("AD DELAY")] [ShowIf("shouldUpdateTimeDelay")] public string timeDelayKey = "";
    private void updatePluginTimeDelay()
    {
        #if USE_BYTEBREW
        if (shouldUpdateTimeDelay &&
            timeDelayKey != null)
        {
            AdConstants.adDelay = getRemoteConfigValue(timeDelayKey, 10);
        }
#endif

    }

    public static FinzByteBrewAnalyticsManager instance = null;

    public delegate void remoteConfigByteBrewFetched();
    public static event remoteConfigByteBrewFetched remoteConfigByteBrewFetchedComplete;

    private void Awake()
    {
        instance = this;
        if (shouldUpdateTimeDelay)
        {
            remoteConfigByteBrewFetchedComplete += updatePluginTimeDelay;
        }
#if USE_BYTEBREW
        ByteBrew.InitializeByteBrew();
        setupRemoteConfig();

#endif

    }

    private void setupRemoteConfig()
    {
#if USE_BYTEBREW
        if (ByteBrew.IsByteBrewInitialized())
        {
            ByteBrew_Helper.LoadRemoteConfigs();
            ByteBrew.RemoteConfigsUpdated(() =>
            {
                if (remoteConfigByteBrewFetchedComplete != null)
                {
                    remoteConfigByteBrewFetchedComplete.Invoke();
                }
            });
        }
        else
        {
            Invoke("setupRemoteConfig", 5);

        }
#endif
    }


    public bool getRemoteConfigValue(string key, bool deafultValue = false)
    {
#if USE_BYTEBREW
        if (ByteBrew.IsByteBrewInitialized())
        {
            string result = ByteBrewSDK.ByteBrew.GetRemoteConfigForKey(key, deafultValue + "");
            if (result != null)
            {
                return bool.Parse(result);
            }
            else
            {
                Debug.Log("-----Bybrew " + key + "Key not found");
                return deafultValue;
            }
        }
        else
        {

            Debug.Log("-----Bybrew not initilized yet-----");
            return deafultValue;
        }
#else
        return deafultValue;
#endif
    }
    public int getRemoteConfigValue(string key, int deafultValue = 0)
    {
#if USE_BYTEBREW
        if (ByteBrew.IsByteBrewInitialized())
        {
            try
            {
                string result = ByteBrewSDK.ByteBrew.GetRemoteConfigForKey(key, deafultValue + "");
                if (result != null)
                {
                    return int.Parse(result);
                }
                else
                {
                    Debug.Log("-----Bybrew " + key + "Key not found");
                    return deafultValue;
                }
            }
            catch (Exception e)
            {
                return deafultValue;
            }
        }
        else
        {

            Debug.Log("-----Bybrew not initilized yet-----");
            return deafultValue;
        }
#else
        return deafultValue;
#endif
    }
    public float getRemoteConfigValue(string key, float deafultValue = 0)
    {
#if USE_BYTEBREW
        if (ByteBrew.IsByteBrewInitialized())
        {
            try
            {
                string result = ByteBrewSDK.ByteBrew.GetRemoteConfigForKey(key, deafultValue + "");
                if (result != null)
                {
                    return float.Parse(result);
                }
                else
                {
                    Debug.Log("-----Bybrew " + key + "Key not found");
                    return deafultValue;
                }
            }
            catch (Exception e)
            {
                return deafultValue;
            }
        }
        else
        {

            Debug.Log("-----Bybrew not initilized yet-----");
            return deafultValue;
        }
#else
        return deafultValue;
#endif
    }
    public string getRemoteConfigValue(string key, string deafultValue = "")
    {
#if USE_BYTEBREW
        if (ByteBrew.IsByteBrewInitialized())
        {

            try
            {
                string result = ByteBrewSDK.ByteBrew.GetRemoteConfigForKey(key, deafultValue + "");
                if (result != null)
                {
                    return result;
                }
                else
                {
                    Debug.Log("-----Bybrew " + key + "Key not found");
                    return deafultValue;
                }
            }
            catch (Exception e)
            {
                return deafultValue;
            }

        }
        else
        {

            Debug.Log("-----Bybrew not initilized yet-----");
            return deafultValue;
        }
#else
        return deafultValue;
#endif
    }


    public void InterstitialPaidAdImpression(string provider, string adapterName, double revenue)
    {
#if USE_BYTEBREW
        ByteBrew.TrackAdEvent(ByteBrewAdTypes.Interstitial, provider, adapterName, revenue);
#endif
    }
    public void RewardPaidAdImpression(string provider, string adapterName, double revenue)
    {
#if USE_BYTEBREW
        ByteBrew.TrackAdEvent(ByteBrewAdTypes.Reward, provider, adapterName, revenue);
#endif
    }

#if USE_BYTEBREW
    public void LevelAnalytics(ByteBrewProgressionTypes state, int level)
    {
        Debug.Log("level cont" + level);
        string _levelState = state.ToString();
        string _levelNumber = level.ToString();
        try
        {

            if (ByteBrew.IsInitilized)
            {

                string eventName = $"LEVEL_ANALYSIS";
                var eventParameters = new Dictionary<string, string>
                {
                    { "LEVEL", _levelNumber },
                    { "STATUS" , _levelState }
                };

                ByteBrew.NewCustomEvent(eventName, eventParameters);
                switch (state)
                {
                    case ByteBrewProgressionTypes.Started:
                        {
                            ByteBrew.NewCustomEvent($"LevelStarted");
                            Debug.Log(
                                $"<color=orange>GameEventManager: bytewBrew Tracked with status LevelStarted</color>");
                            break;
                        }
                    case ByteBrewProgressionTypes.Completed:
                        {
                            ByteBrew.NewCustomEvent($"LevelEnded");
                            Debug.Log(
                                $"<color=orange>GameEventManager: bytewBrew Tracked with status LevelEnded</color>");
                            break;
                        }
                    case ByteBrewProgressionTypes.Failed:
                        {
                            ByteBrew.NewCustomEvent($"LevelFailed");
                            Debug.Log(
                                $"<color=orange>GameEventManager: bytewBrew Tracked with status LevelEnded</color>");
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                Debug.Log(
                    $"<color=orange>GameEventManager: bytewBrew Tracked {eventName} for Level {_levelNumber} with status {_levelState}</color>");
            }

            Debug.Log("<color=orange>GameEventManager: No analytics service enabled</color>");

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
#endif
#endif
}
