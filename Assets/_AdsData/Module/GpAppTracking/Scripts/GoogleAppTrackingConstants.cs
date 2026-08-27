using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleAppTrackingConstants : MonoBehaviour
{
    #region Sound
    public static void setAppTracking(bool target)
    {

        if (target)
        {
            PlayerPrefs.SetInt("GoogleAppTracking", 1);
        }
        else
        {
            PlayerPrefs.SetInt("GoogleAppTracking", 0);
        }
        PlayerPrefs.Save();
    }
    public static bool hasShownGpAppTrackiing()
    {
        return PlayerPrefs.GetInt("GoogleAppTracking", 0) == 1;
    }
    #endregion
}
