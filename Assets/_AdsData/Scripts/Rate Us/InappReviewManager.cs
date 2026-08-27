using System.Collections;
using UnityEngine;

#if UNITY_ANDROID && SHOW_NATIVE_RATE
using Google.Play.Review;
#endif

public class InappReviewManager : MonoBehaviour
{

    // Start is called before the first frame update
    public void showRateMenu()
    {
        StartCoroutine(requireRate());
    }

    private IEnumerator requireRate()
    {
        yield return new WaitForSecondsRealtime(0.1f);
#if UNITY_ANDROID && SHOW_NATIVE_RATE && !UNITY_EDITOR
        // Create instance of ReviewManager
        ReviewManager _reviewManager;
        // ...
        _reviewManager = new ReviewManager();
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        PlayReviewInfo _playReviewInfo = requestFlowOperation.GetResult();
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
#endif
    }
}
