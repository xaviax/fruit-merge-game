using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image loadingFiller = null;
    [SerializeField] private TMP_Text loadingText = null;



    
    public void SetProgress(float progress)
    {   
        
        progress = Mathf.Clamp01(progress);

        loadingFiller.fillAmount = progress;

        int percentage = Mathf.RoundToInt(progress * 100f);
        loadingText.text = $"Loading... {percentage}%";
    }
}