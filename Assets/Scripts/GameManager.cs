using UnityEngine;

/// <summary>
/// Manages game-wide state, settings, save/load, and coordinates between different game systems
/// This should be a persistent singleton that manages the overall game flow
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private GameSettings gameSettings;
    
    [Header("Player Reference")]
    [SerializeField] private PlayerController playerController;
    
    public static GameManager Instance { get; private set; }
    
    public GameSettings Settings { get { return gameSettings; } }
    public PlayerController Player { get { return playerController; } }

     public bool gameOver { get; private set; } = false;    
    
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
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

        ApplySettingsToPlayer();
    }
    
    private void LoadGameSettings()
    {
        // Load settings from PlayerPrefs or save file
        gameSettings.walkSpeed = PlayerPrefs.GetFloat("WalkSpeed", 15f);
        gameSettings.runSpeed = PlayerPrefs.GetFloat("RunSpeed", 37.5f);
        gameSettings.jumpForce = PlayerPrefs.GetFloat("JumpForce", 5f);
        gameSettings.walkAnimationSpeed = PlayerPrefs.GetFloat("WalkAnimSpeed", 1f);
        gameSettings.runAnimationSpeed = PlayerPrefs.GetFloat("RunAnimSpeed", 2.5f);
        gameSettings.masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
    }
    
    public void SaveGameSettings()
    {
        // Save settings to PlayerPrefs or save file
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

    public void SetGameOver(bool isGameOver)
    {
        Debug.Log("GameManager: Setting gameOver to " + isGameOver);
        gameOver = isGameOver;
    }
    
    public void SetMasterVolume(float volume)
    {
        gameSettings.masterVolume = Mathf.Clamp01(volume);
        AudioListener.volume = gameSettings.masterVolume;
    }
    
    // Game flow methods
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
    
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
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

[System.Serializable]
public class GameSettings
{
    [Header("Player Settings")]
    public float walkSpeed = 15f;
    public float runSpeed = 37.5f;
    public float jumpForce = 5f;
    
    [Header("Animation Settings")]
    public float walkAnimationSpeed = 1f;
    public float runAnimationSpeed = 2.5f;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    
    [Header("Graphics Settings")]
    public bool enableVSync = true;
    public int targetFrameRate = 60;
}