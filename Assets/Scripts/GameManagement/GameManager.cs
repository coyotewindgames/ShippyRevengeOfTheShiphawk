using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private GameSettings gameSettings;

    [SerializeField] private PlayerController playerController;

    [Header("Scene Flow")]
    [SerializeField] private SceneFader sceneFader;
    [SerializeField] private string gameOverSceneName = "GameOverScene";
    [SerializeField] private int gameOverSceneBuildIndex = -1;

    public static GameManager Instance { get; set; }

    // Method to ensure GameManager instance exists
    public static GameManager EnsureInstance()
    {
        if (Instance == null)
        {
            GameObject gameManagerObj = new GameObject("GameManager");
            Instance = gameManagerObj.AddComponent<GameManager>();
            Debug.Log("GameManager: Emergency instance created");
        }
        return Instance;
    }

    public GameSettings Settings 
    { 
        get 
        {
            if (gameSettings == null)
                gameSettings = new GameSettings();
            return gameSettings;
        } 
        set => gameSettings = value; 
    }
    private PlayerController Player { get; set; }

    public bool gameOver { get; set; } = false;

    private void Awake()
    {
        // Ensure only one GameManager exists across all scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("GameManager: Instance created and persisted across scenes");
        }
        else if (Instance != this)
        {
            Debug.Log("GameManager: Duplicate instance found and destroyed");
            Destroy(gameObject);
            return;
        }

        // Ensure the instance is always accessible
        if (Instance == null)
            Instance = this;
    }

    private void OnDestroy()
    {
        // Only unsubscribe if this is the actual singleton instance
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
Debug.Log($"GameManager: Scene '{scene.name}' loaded, refreshing references");

// Always refresh references when a new scene loads
sceneFader = FindFirstObjectByType<SceneFader>();
playerController = FindFirstObjectByType<PlayerController>();

// Ensure settings are applied to any new player controller
ApplySettingsToPlayer();

// Refresh GameManager reference in UI controllers
RefreshUIControllers();
}

private void RefreshUIControllers()
{
    // Find and refresh MainMenuController if it exists
    MainMenuController mainMenuController = FindFirstObjectByType<MainMenuController>();
    if (mainMenuController != null)
    {
        Debug.Log("GameManager: MainMenuController found, ensuring connection");
        // The MainMenuController should automatically reconnect via GameManager.Instance
    }

    // Find and refresh GameOverController if it exists  
    GameOverController gameOverController = FindFirstObjectByType<GameOverController>();
    if (gameOverController != null)
    {
        Debug.Log("GameManager: GameOverController found, ensuring connection");
        // The GameOverController should automatically reconnect via GameManager.Instance
    }

    // Any other UI controllers can be added here
}

    private void InitializeGame()
    {
        if (gameSettings == null)
            gameSettings = new GameSettings();

        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        LoadGameSettings();
    }

    private void Start()
    {
        if (sceneFader == null)
            sceneFader = FindFirstObjectByType<SceneFader>();
        ApplySettingsToPlayer();
    }

    private void LoadGameSettings()
    {
        gameSettings.walkSpeed = PlayerPrefs.GetFloat("WalkSpeed", 15f);
        gameSettings.runSpeed = PlayerPrefs.GetFloat("RunSpeed", 37.5f);
        gameSettings.jumpForce = PlayerPrefs.GetFloat("JumpForce", 5f);
        gameSettings.walkAnimationSpeed = PlayerPrefs.GetFloat("WalkAnimSpeed", 1f);
        gameSettings.runAnimationSpeed = PlayerPrefs.GetFloat("RunAnimSpeed", 2.5f);
        gameSettings.masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
    }

    public void SaveGameSettings()
    {
        PlayerPrefs.SetFloat("WalkSpeed", gameSettings.walkSpeed);
        PlayerPrefs.SetFloat("RunSpeed", gameSettings.runSpeed);
        PlayerPrefs.SetFloat("JumpForce", gameSettings.jumpForce);
        PlayerPrefs.SetFloat("WalkAnimSpeed", gameSettings.walkAnimationSpeed);
        PlayerPrefs.SetFloat("RunAnimSpeed", gameSettings.runAnimationSpeed);
        PlayerPrefs.SetFloat("MasterVolume", gameSettings.masterVolume);
        PlayerPrefs.Save();
    }

    private void ApplySettingsToPlayer()
    {
        if (playerController != null)
        {
            playerController.WalkSpeed = gameSettings.walkSpeed;
            playerController.RunSpeed = gameSettings.runSpeed;
            playerController.JumpForce = gameSettings.jumpForce;
        }
    }

    public void SetWalkSpeed(float speed)
    {
        gameSettings.walkSpeed = speed;
        if (playerController != null)
            playerController.WalkSpeed = speed;
    }

    public void SetRunSpeed(float speed)
    {
        gameSettings.runSpeed = speed;
        if (playerController != null)
            playerController.RunSpeed = speed;
    }

    public void SetJumpForce(float force)
    {
        gameSettings.jumpForce = force;
        if (playerController != null)
            playerController.JumpForce = force;
    }

    public void SetGameWin(bool isWin, string targetScene = "WinScene")
    {
        Debug.Log("GameManager: Setting gameWin to " + isWin);
        
        if (isWin)
        {
            SaveGameSettings();
            
            // Unlock cursor for UI scenes
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (sceneFader != null)
            {
                Debug.Log("GameManager: Using SceneFader to transition to " + targetScene);
                sceneFader.FadeToScene(targetScene);
            }
            else
            {
                Debug.LogWarning("GameManager: SceneFader not found, loading scene directly");
                StartCoroutine(LoadSceneDirectly(targetScene));
            }
        }
    }

    public void SetGameOver(bool isGameOver)
    {
        Debug.Log("GameManager: Setting gameOver to " + isGameOver);
        gameOver = isGameOver;
        if (isGameOver)
        {
            // Unlock cursor for UI scenes
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            string targetScene = ResolveGameOverSceneName();

            if (!string.IsNullOrWhiteSpace(targetScene))
            {
                if (sceneFader != null)
                {
                    Debug.Log("GameManager: Using SceneFader to transition to " + targetScene);
                    sceneFader.FadeToScene(targetScene);
                }
                else
                {
                    // Fallback for WebGL builds when SceneFader isn't available
                    Debug.LogWarning("GameManager: SceneFader not found, loading scene directly");
                    StartCoroutine(LoadSceneDirectly(targetScene));
                }
            }
            else
            {
                Debug.LogError("GameManager: Target scene name is null or empty!");
            }
        }
    }

    private System.Collections.IEnumerator LoadSceneDirectly(string sceneName)
    {
        // Give a brief moment for any ongoing processes to complete
        yield return new WaitForSeconds(0.5f);
        Debug.Log("GameManager: Loading scene directly: " + sceneName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    private string ResolveGameOverSceneName()
    {
        if (gameOverSceneBuildIndex >= 0 && gameOverSceneBuildIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(gameOverSceneBuildIndex);
            if (!string.IsNullOrWhiteSpace(scenePath))
                return System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }

        return gameOverSceneName;
    }

    public void SetMasterVolume(float volume)
    {
        gameSettings.masterVolume = Mathf.Clamp01(volume);
        AudioListener.volume = gameSettings.masterVolume;
    }


    // gonna implement these later
    // public void PauseGame()
    // {
    // Time.timeScale = 0f;
    // }

    // public void ResumeGame()
    // {
    // Time.timeScale = 1f;
    // }

    public void RestartLevel()
    {
        Debug.Log("GameManager: RestartLevel called");
        Time.timeScale = 1f;
        gameOver = false;

        // Use SceneFader if available, otherwise load directly
        string mainSceneName = "MainScene";
        if (sceneFader != null)
        {
            Debug.Log("GameManager: Using SceneFader to restart to " + mainSceneName);
            sceneFader.FadeToScene(mainSceneName);
        }
        else
        {
            Debug.Log("GameManager: SceneFader not found, loading scene directly");
            StartCoroutine(RestartLevelDirectly(mainSceneName));
        }
    }

    public void QuitGame()
    {
        Debug.Log("GameManager: QuitGame called");
        SaveGameSettings();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public static void ForceQuitGame()
    {
        Debug.Log("GameManager: ForceQuitGame called (static method)");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    // Static method for emergency restart when instance might be having issues
    public static void ForceRestartLevel()
    {
        Time.timeScale = 1f;

        try
        {
            SceneManager.LoadScene("MainScene");
        }
        catch (System.Exception e)
        {
            try
            {
                SceneManager.LoadScene(2); // Fallback to build index
            }
            catch (System.Exception e2)
            {
                Debug.LogError("GameManager: ForceRestartLevel by index failed: " + e2.Message);
            }
        }
    }

    public void LoadIntroScene()
    {
        string introSceneName = "IntroScene";
        try
        {
            SceneManager.LoadScene(introSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameManager: Failed to load IntroScene: " + e.Message);
            // Fallback to build index for intro scene
            try
            {
                SceneManager.LoadScene(1);
            }
            catch (System.Exception e2)
            {
                Debug.LogError("GameManager: Failed to load intro scene by index: " + e2.Message);

#if !UNITY_WEBGL
            // Only quit in non-WebGL builds
            Application.Quit();
#endif
            }
        }
    }

    private System.Collections.IEnumerator RestartLevelDirectly(string sceneName)
    {
        // Give a brief moment for any ongoing processes to complete
        yield return new WaitForSeconds(0.1f);

        try
        {
            Debug.Log("GameManager: Loading scene directly: " + sceneName);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameManager: Failed to load scene '" + sceneName + "': " + e.Message);
            // Fallback to build index
            Debug.Log("GameManager: Attempting fallback load by build index");
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveGameSettings();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            SaveGameSettings();
    }
}