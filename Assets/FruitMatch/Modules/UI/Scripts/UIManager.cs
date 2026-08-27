using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Instance of UI Manager 
    public static UIManager Instance;
    
    //Full Screen UI Prefabs
    [Header("Full-Screen Prefabs")]
    [SerializeField] private MainMenuUI mainMenuUIPrefab = null;
    [SerializeField] private LoadingUI loadingUIPrefab = null;
    
    // Popup Screen UI Prefabs 
    [Header("Popup Prefabs")]
    [SerializeField] private SettingsUI settingsUIPrefab = null;
    [SerializeField] private GameOverUI gameOverUIPrefab = null;
    
    //Variables for holding references to active UI
    private LoadingUI activeLoadingUI;
    
    private GameObject activeScreen;
    private GameObject activePopup;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    
    

    public void ShowMainMenu()
    {
        ShowScreen(mainMenuUIPrefab);
    }

    public void ShowSettings()
    {   
        GameManager.Instance.PauseGame();
        ShowPopup(settingsUIPrefab);
    }

    public void ShowGameOver()
    {
        ShowPopup(gameOverUIPrefab);
    }

    public void ShowLoading()
    {
        activeLoadingUI = ShowScreen(loadingUIPrefab);
        
        activeLoadingUI.SetProgress(0f);
        
    }

    public void UpdateLoadingProgress(float progress)
    {
        activeLoadingUI.SetProgress(progress);
    }
    
    

    private T ShowScreen<T>(T screenPrefab)
        where T :Component
    {
        if (screenPrefab == null)
        {
            Debug.LogWarning("Screen prefab is null", this);
            return null;
            
        }
        
        DestroyActiveScreen();
        DestroyActivePopup();

        T screenInstance = Instantiate(screenPrefab);
        activeScreen = screenInstance.gameObject;
        activeScreen.SetActive(true);
        return screenInstance;

    }
    
    
    private void ShowPopup<T>(T popupPrefab)
        where T : Component
    {
        if (popupPrefab == null)
        {
            Debug.LogWarning("Popup prefab is null", this);
            return;
        }
        
        DestroyActivePopup();
        
        T popupInstance = Instantiate(popupPrefab);
        activePopup = popupInstance.gameObject;
        activePopup.SetActive(true);
        
        
        
    }


    private void DestroyActiveScreen()
    {
        if (activeScreen == null)
        {
            return;
        }
        
        activeScreen.SetActive(false);
        Destroy(activeScreen);
        activeScreen = null;
        
    }

    private void DestroyActivePopup()
    {
        if (activePopup == null)
        {
            return;
        }
        
        activePopup.SetActive(false);
        Destroy(activePopup);
        activePopup = null;
    }


    public void ClosePopupAndResumeGame()
    {
        DestroyActivePopup();
        GameManager.Instance.ResumeGame();
    }

}
