
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    public float delay = 3f;
    public string sceneName = "MainMenu";
    // Start is called before the first frame update
    void Start()
    {
        //AdConstants.disableAds();
        Invoke(nameof(LoadScene), delay);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
