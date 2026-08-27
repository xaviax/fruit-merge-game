// FinzPlayTimeTracker.cs
// Tracks active (foreground) playtime across sessions.
// Provides GetTotalMinutes() and OnMinutePassed.
// Sends a Firebase Analytics event each time a full minute passes:
//   e.g., "game_1_minute", "game_2_minutes", etc.
// Ensures event names are valid: lowercase, underscores only, <=40 chars.

using UnityEngine;
using System;
using System.Globalization;
#if USE_FIREBASE
using Firebase.Analytics;   // Firebase must already be set up in your project
#endif
public class FinzPlayTimeTracker : MonoBehaviour
{
#if USE_FIREBASE
    [Header("Tracking")]
    [Tooltip("How often (seconds) to persist to PlayerPrefs while running.")]
    [SerializeField] private float autosaveIntervalSeconds = 5f;

    [Tooltip("Use unscaled time (keeps counting during Time.timeScale=0 pauses).")]
    [SerializeField] private bool useUnscaledTime = false;

    [Header("Firebase")]
    [Tooltip("If enabled, sends a Firebase event whenever a full minute passes.")]
    [SerializeField] private bool sendFirebaseMinuteEvents = true;

    [Tooltip("Event name prefix, e.g., 'game_' produces 'game_1_minute'.")]
    [SerializeField] private string firebaseEventPrefix = "game_";

    // ---- Public API ----
    /// <summary>Total seconds of active play across all sessions.</summary>
    public static double TotalSecondsPlayed { get; private set; }

    /// <summary>Returns total whole minutes (floor) of active play across sessions.</summary>
    public static int GetTotalMinutes() => Mathf.FloorToInt((float)(TotalSecondsPlayed / 60d));

    /// <summary>Action called each time a new full minute has passed (argument = total minutes).</summary>
    public static event Action<int> OnMinutePassed;

    /// <summary>Resets playtime to zero (persists immediately).</summary>
    public static void ResetPlaytime()
    {
        TotalSecondsPlayed = 0d;
        _lastReportedMinutes = 0;
        SaveToPrefs();
    }

    // ---- Internals ----
    private static FinzPlayTimeTracker _instance;
    private const string PrefKey = "PLAYTIME_TOTAL_SECONDS_FG_V7";

    private float _saveTimer;
    private bool _isActive;
    private static int _lastReportedMinutes;

    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Load persisted seconds
        if (PlayerPrefs.HasKey(PrefKey))
        {
            var s = PlayerPrefs.GetString(PrefKey, "0");
            if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var val))
                val = 0d;
            TotalSecondsPlayed = Math.Max(0d, val);
        }
        else
        {
            TotalSecondsPlayed = 0d;
        }

        _lastReportedMinutes = GetTotalMinutes();
        _isActive = Application.isFocused;
        _saveTimer = 0f;
    }

    private void Update()
    {
        if (!_isActive) return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        if (dt > 0f) TotalSecondsPlayed += dt;

        // Minute rollover check
        int minutesNow = GetTotalMinutes();
        if (minutesNow > _lastReportedMinutes)
        {
            _lastReportedMinutes = minutesNow;

            // Fire action (if subscribed)
            OnMinutePassed?.Invoke(minutesNow);

            // Send Firebase event (if enabled)
            if (sendFirebaseMinuteEvents)
                SendFirebaseMinuteEvent(minutesNow);
        }

        // Autosave
        _saveTimer += dt;
        if (_saveTimer >= autosaveIntervalSeconds)
        {
            _saveTimer = 0f;
            SaveToPrefs();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            _isActive = false;
            SaveToPrefs();
        }
        else
        {
            _isActive = Application.isFocused;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            _isActive = false;
            SaveToPrefs();
        }
        else
        {
            _isActive = true;
        }
    }

    private void OnApplicationQuit()
    {
        SaveToPrefs();
    }

    private static void SaveToPrefs()
    {
        PlayerPrefs.SetString(PrefKey, TotalSecondsPlayed.ToString(CultureInfo.InvariantCulture));
        PlayerPrefs.Save();
    }

    // -------- Firebase minute event --------
    private void SendFirebaseMinuteEvent(int minutes)
    {
        string suffix = minutes == 1 ? "minute" : "minutes";
        string rawName = $"{firebaseEventPrefix}{minutes}_{suffix}";

        // Normalize: lowercase, replace spaces with "_", remove invalid chars
        string eventName = rawName.ToLowerInvariant();
        eventName = eventName.Replace(" ", "_");
        foreach (char c in eventName)
        {
            if (!(char.IsLetterOrDigit(c) || c == '_'))
                eventName = eventName.Replace(c.ToString(), "_");
        }

        // Ensure starts with a letter
        if (eventName.Length == 0 || !char.IsLetter(eventName[0]))
            eventName = "event_" + eventName;

        // Trim to 40 chars
        if (eventName.Length > 40) eventName = eventName.Substring(0, 40);

        FirebaseAnalytics.LogEvent(eventName);

#if UNITY_EDITOR
        Debug.Log($"[FinzPlayTimeTracker] Sent Firebase event: {eventName}");
#endif
    }
#endif
}
