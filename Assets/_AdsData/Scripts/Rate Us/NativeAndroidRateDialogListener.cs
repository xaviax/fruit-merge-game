using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NativeAndroidRateDialogListener : MonoBehaviour
{


    [SerializeField] Sprite passiveStar;

    [SerializeField] Sprite activeStar;

    [SerializeField] Text title;


    [SerializeField] Image[] stars;

    [SerializeField] GameObject finalPanel;

    [SerializeField] Button rateButton,exitButton;


    int index = 1;



    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;


        rateButton.interactable = false;

        title.text = "Enjoying " + Application.productName;

    }

  

    public void starsClicked(GameObject starIndex)
    {
        //if (rateButton.interactable)
        //    return;

        index = int.Parse(starIndex.name);

        for (int i = 0; i < stars.Length; i++)
        {
            if (i <= index)
            {
                stars[i].sprite = activeStar;
            }
            else
            stars[i].sprite = passiveStar;
        }

        rateButton.interactable = true;


        if (index < 3) {
        }

        else
        {

            finalPanel.SetActive(true);
        }
    }

    

    public void rateInStoreClicked()
    {
        if (index < 3)
            laterBtnClicked();

        else
        {
            AdConstants.IsAdWasShowing = true;
            AdConstants.resumeFromAds = true;
            AdController.instance?.iapInstance.showRateMenu();
           // Application.OpenURL("https://play.google.com/store/apps/details?id=" + AdConstants.packageName);

//            AnalyticsManager.Instance.miniEvents("Rate", "Visited Store");

            AdConstants.userHasRatedApp();
            laterBtnClicked();
        }
    }

    public void laterBtnClicked()
    {
        Time.timeScale = 1;
        Destroy(this.gameObject);
    }
}
