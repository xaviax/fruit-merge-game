using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    // need sprites for settings
    
    [Header("Sprite Icons")]
    [SerializeField] private Sprite onIcon = null;
    [SerializeField] private Sprite offIcon = null;
    
    [Header("Button GameObjects")]
    [SerializeField] private Image musicButton = null;
    [SerializeField] private Image soundButton = null;


    void Awake()
    {
        UpdateIcons();
    }

    public void UpdateIcons()
    {
        musicButton.sprite = onIcon;
        soundButton.sprite = onIcon;
    }
    
    
    public void OnClickCross()
    {
        SoundManager.Instance.PlayOnClickSound();
        UIManager.Instance.ClosePopupAndResumeGame(); 
    }


    public void OnClickMusicButton()
    {
        SoundManager.Instance.PlayOnClickSound();

        if (musicButton.sprite == onIcon)
        {
            musicButton.sprite = offIcon;
            SoundManager.Instance.ToggleMusic(false);
        }

        else
        {
            musicButton.sprite = onIcon;
            SoundManager.Instance.ToggleMusic(true);
        }
            
    }
    
    public void OnClickSoundButton()
    {
        SoundManager.Instance.PlayOnClickSound();
        
        if (soundButton.sprite == onIcon)
        {
            soundButton.sprite = offIcon;
            SoundManager.Instance.ToggleSoundEffects(false);
        }

        else
        {
            soundButton.sprite = onIcon;
            SoundManager.Instance.ToggleSoundEffects(true);
        }
    }
    
    
    
    
    
    
    
}
