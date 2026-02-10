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

    public static GameManager EnsureInstance()
    {
        if (Instance == null)
        {
            GameObject gameManagerObj = new GameObject("GameManager");
            Instance = gameManagerObj.AddComponent<GameManager>();
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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {

            Destroy(gameObject);
            return;
        }

        if (Instance == null)
            Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        sceneFader = FindFirstObjectByType<SceneFader>();
        playerController = FindFirstObjectByType<PlayerController>();

        ApplySettingsToPlayer();

        RefreshUIControllers();
    }

    private void RefreshUIControllers()
    {
        MainMenuController mainMenuController = FindFirstObjectByType<MainMenuController>();


        GameOverController gameOverController = FindFirstObjectByType<GameOverController>();

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

    // Abstraction: SaveGameSettings() hides the complex persistence logic behind a simple method call
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

        if (isWin)
        {
            SaveGameSettings();

            // Unlock cursor for UI scenes
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (sceneFader != null)
            {
                sceneFader.FadeToScene(targetScene);
            }
            else
            {
                StartCoroutine(LoadSceneDirectly(targetScene));
            }
        }
    }

    public void SetGameOver(bool isGameOver)
    {
        gameOver = isGameOver;
        if (isGameOver)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            string targetScene = ResolveGameOverSceneName();

            if (!string.IsNullOrWhiteSpace(targetScene))
            {
                if (sceneFader != null)
                {
                    sceneFader.FadeToScene(targetScene);
                }
                else
                {
                    // Fallback for WebGL builds when SceneFader isn't available
                    StartCoroutine(LoadSceneDirectly(targetScene));
                }

            }
        }
    }

    private System.Collections.IEnumerator LoadSceneDirectly(string sceneName)
    {
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

        string mainSceneName = "MainScene";
        if (sceneFader != null)
        {
            sceneFader.FadeToScene(mainSceneName);
        }
        else
        {
            StartCoroutine(RestartLevelDirectly(mainSceneName));
            StartCoroutine(RestartLevelDirectly(mainSceneName));
        }
    }

    public void QuitGame()
    {
        SaveGameSettings();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public static void ForceQuitGame()
    {

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
            // Fallback to build index for intro scene
            try
            {
                SceneManager.LoadScene(1);
            }
            catch (System.Exception e2)
            {

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
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        catch (System.Exception e)
        {
            // Fallback to build index
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