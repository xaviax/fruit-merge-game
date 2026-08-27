using TMPro;
using UnityEngine;

public sealed class AdsDebugPanel : MonoBehaviour
{
    [Header("Debug Output")]
    [SerializeField] private TMP_Text debugText;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLogMessage;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLogMessage;
    }

    public void ShowBanner()
    {
        if (!TryGetAdsManager(out AdsManager adsManager))
        {
            return;
        }

        adsManager.ShowBanner();
    }

    public void HideBanner()
    {
        if (!TryGetAdsManager(out AdsManager adsManager))
        {
            return;
        }

        adsManager.HideBanner();
    }

    public void DestroyBanner()
    {
        if (!TryGetAdsManager(out AdsManager adsManager))
        {
            return;
        }

        adsManager.DestroyBanner();
    }

    public void ShowInterstitial()
    {
        if (!TryGetAdsManager(out AdsManager adsManager))
        {
            return;
        }

        bool requestStarted = adsManager.ShowInterstitial();

        Debug.Log(
            requestStarted
                ? "Interstitial request sent."
                : "Interstitial request could not be sent."
        );
    }

    public void ShowRewarded()
    {
        if (!TryGetAdsManager(out AdsManager adsManager))
        {
            return;
        }

        adsManager.ShowRewarded(
            onRewardGranted: HandleTestRewardGranted,
            onUnavailable: HandleTestRewardUnavailable,
            onCancelled: HandleTestRewardCancelled
        );
    }

    public void CheckRewardedAvailability()
    {
        if (!TryGetAdsManager(out AdsManager adsManager))
        {
            return;
        }

        bool available = adsManager.IsRewardedAvailable();

        Debug.Log(
            available
                ? "Rewarded ad is available."
                : "Rewarded ad is unavailable."
        );
    }

    private void HandleTestRewardGranted()
    {
        Debug.Log("TEST: Reward granted.");
    }

    private void HandleTestRewardUnavailable()
    {
        Debug.Log("TEST: Rewarded ad unavailable.");
    }

    private void HandleTestRewardCancelled()
    {
        Debug.Log("TEST: Rewarded ad cancelled.");
    }

    private void HandleLogMessage(
        string logMessage,
        string stackTrace,
        LogType logType)
    {
        if (debugText == null)
        {
            return;
        }

        debugText.text = $"{logType}: {logMessage}";
    }

    private bool TryGetAdsManager(
        out AdsManager adsManager)
    {
        adsManager = AdsManager.Instance;

        if (adsManager != null)
        {
            return true;
        }

        Debug.LogError(
            "AdsManager is not initialized.",
            this
        );

        return false;
    }
}