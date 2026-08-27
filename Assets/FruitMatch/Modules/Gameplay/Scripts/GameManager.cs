using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    // Instance 
    public static GameManager Instance {get; private set;}
    
    // Game states 
    public enum GameState
    {   MainMenu,
        Loading,
        Ads,
        Playing, 
        Paused,
        Lose
        
    }
    
    [Header("Scenes")]
    [SerializeField] private string gameplayScene = "Gameplay";
    [SerializeField] private string mainMenuScene = "MainMenu";
    
    
    
    public GameState CurrentGameState { get; private set; } = GameState.MainMenu; 
    
    public bool IsGameplayActive => CurrentGameState == GameState.Playing;
    
    private float minimumLoadingTime = 1f;
    
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
            SetState(GameState.MainMenu);
            UIManager.Instance.ShowMainMenu();
    }

    public void StartGame()
    {
        if (!TryChangeState(GameState.MainMenu, GameState.Loading))
        {
            return;
        }

        StartCoroutine(LoadSceneRoutine(gameplayScene, GameState.Playing));
    }

    private IEnumerator LoadSceneRoutine(
        string sceneName,
        GameState destinationState)
    {
        UIManager.Instance.ShowLoading();

        AsyncOperation loadingOperation =
            SceneManager.LoadSceneAsync(sceneName);

        loadingOperation.allowSceneActivation = false;

        float elapsedTime = 0f;

        while (
            elapsedTime < minimumLoadingTime ||
            loadingOperation.progress < 0.9f)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float timeProgress =
                minimumLoadingTime <= 0f
                    ? 1f
                    : Mathf.Clamp01(
                        elapsedTime / minimumLoadingTime
                    );

            // Unity reports 0.9 while waiting for activation.
            float sceneProgress = Mathf.Clamp01(
                loadingOperation.progress / 0.9f
            );

            float displayedProgress = Mathf.Min(
                timeProgress,
                sceneProgress
            );

            UIManager.Instance.UpdateLoadingProgress(
                displayedProgress
            );

            yield return null;
        }

        UIManager.Instance.UpdateLoadingProgress(1f);

        // Let the loading screen render 100% for one frame.
        yield return null;

        /*
         * Set the destination state before scene activation.
         * New scene components will therefore see the correct
         * state from their Awake/Start methods.
         */
        SetState(destinationState);
        Time.timeScale = 1f;

        loadingOperation.allowSceneActivation = true;

        yield return loadingOperation;

        InitializeLoadedScene(destinationState);
    }
    
    private void InitializeLoadedScene(
        GameState destinationState)
    {
        if (destinationState == GameState.MainMenu)
        {
            /*
             * GameManager.Start() does not run again because this
             * GameManager persisted from the original MainMenu.
             */
            UIManager.Instance.ShowMainMenu();
        }

        /*
         * Gameplay needs no UI initialization here.
         * GameplayHUD and ScoreManager belong to the Gameplay scene
         * and Unity creates them when that scene activates.
         */
    }
     

    public void PauseGame()
    {
        if (!TryChangeState(GameState.Playing, GameState.Paused))
        {
            return;
        }
        
        Time.timeScale = 0;
        
    }

    public void ResumeGame()
    {
        if (!TryChangeState(GameState.Paused, GameState.Playing))
        {
            return;
        }
        
        Time.timeScale = 1;
        
    }


    public void GameOver()
    {
        if (!TryChangeState(GameState.Playing, GameState.Lose))
        {
            return;
        }
        
        
        Time.timeScale = 0;
        
        UIManager.Instance.ShowGameOver();

    }

    public void LoadMainMenu()
    {
        if (!TryChangeState(GameState.Lose, GameState.Loading))
        {
            return;
        }
        
        StartCoroutine(LoadSceneRoutine(mainMenuScene,GameState.MainMenu));
    }
    
    
    private void SetState(GameState newGameState)
    {
        CurrentGameState = newGameState;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SetState(GameState.Playing);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    private bool TryChangeState(GameState requiredState, GameState newState)
    {
        if (CurrentGameState != requiredState)
        {
            return false;
        }
        
        SetState(newState);
        return true;
        
    }

    
}
