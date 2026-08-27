using UnityEngine;

public class GameOverUI : MonoBehaviour
{
   public void OnClickRestart()
   {
      AdsManager.Instance.ShowInterstitial();
      GameManager.Instance.RestartGame();
   }

   public void OnClickQuit()
   {
      SoundManager.Instance.PlayOnClickSound();
      GameManager.Instance.LoadMainMenu();
      Debug.Log("Tried to get into Main Menu");
   }
}
