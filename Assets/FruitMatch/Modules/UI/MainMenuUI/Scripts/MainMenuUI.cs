using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
   public void OnClickSettings()
   {  
      SoundManager.Instance.PlayOnClickSound();
      UIManager.Instance.ShowSettings();
   }

   public void OnClickPlay()
   {
      GameManager.Instance.StartGame();
   }
}
