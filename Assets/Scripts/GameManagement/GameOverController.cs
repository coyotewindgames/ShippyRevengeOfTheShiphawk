using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private void Start()
    {
        if (enableDebugLogs)
            Debug.Log("GameOverController: Initializing Game Over scene");

        // Find buttons if not assigned
        if (restartButton == null)
        {
            restartButton = FindButtonByNames("RestartButton", "TryAgainButton", "Restart", "RestartBtn", "TryAgain");
        }

        if (quitButton == null)
        {
            quitButton = FindButtonByNames("QuitButton", "ExitButton", "Quit", "Exit", "QuitBtn");
        }

        // Setup button listeners with immediate verification
        SetupRestartButton();
        SetupQuitButton();

        // Ensure cursor is visible and unlocked for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Double-check button setup after a brief delay
        StartCoroutine(VerifyButtonSetup());
    }

    private Button FindButtonByNames(params string[] possibleNames)
    {
        foreach (string name in possibleNames)
        {
            GameObject buttonObj = GameObject.Find(name);
            if (buttonObj != null)
            {
                Button button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    if (enableDebugLogs)
                        Debug.Log($"GameOverController: Found button '{name}'");
                    return button;
                }
            }
        }

        // Also try searching in all Button components
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button button in allButtons)
        {
            string buttonName = button.gameObject.name.ToLower();
            if (buttonName.Contains("restart") || buttonName.Contains("try") || buttonName.Contains("again"))
            {
                if (enableDebugLogs)
                    Debug.Log($"GameOverController: Found restart button by search: '{button.gameObject.name}'");
                return button;
            }
        }

        return null;
    }

    private void SetupRestartButton()
    {
        if (restartButton != null)
        {
            // Clear any existing listeners
            restartButton.onClick.RemoveAllListeners();
            
            // Add our listener
            restartButton.onClick.AddListener(() => {
                if (enableDebugLogs)
                    Debug.Log("GameOverController: Restart button clicked via listener");
                RestartGame();
            });

            if (enableDebugLogs)
                Debug.Log($"GameOverController: Restart button '{restartButton.gameObject.name}' configured successfully");
        }
        else
        {
            Debug.LogError("GameOverController: No restart button found! Make sure you have a button named 'RestartButton', 'TryAgainButton', or 'Restart'");
        }
    }

    private void SetupQuitButton()
    {
        if (quitButton != null)
        {
            // Clear any existing listeners
            quitButton.onClick.RemoveAllListeners();
            
            // Add our listener
            quitButton.onClick.AddListener(() => {
                if (enableDebugLogs)
                    Debug.Log("GameOverController: Quit button clicked via listener");
                QuitGame();
            });

            if (enableDebugLogs)
                Debug.Log($"GameOverController: Quit button '{quitButton.gameObject.name}' configured successfully");
        }
        else if (enableDebugLogs)
        {
            Debug.Log("GameOverController: No quit button found (this is optional)");
        }
    }

    private System.Collections.IEnumerator VerifyButtonSetup()
    {
        yield return new WaitForSeconds(0.5f);

        if (restartButton != null)
        {
            int listenerCount = restartButton.onClick.GetPersistentEventCount();
            if (enableDebugLogs)
                Debug.Log($"GameOverController: Restart button '{restartButton.gameObject.name}' has {listenerCount} persistent events and runtime listeners have been set up");
        }
        else
        {
            Debug.LogError("GameOverController: Restart button is still null after setup!");
        }
    }

    public void RestartGame()
    {
        if (enableDebugLogs)
            Debug.Log("GameOverController: RestartGame called");

        try
        {
            // Try using GameManager first
            if (GameManager.Instance != null)
            {
                if (enableDebugLogs)
                    Debug.Log("GameOverController: Using GameManager.RestartLevel()");
                GameManager.Instance.RestartLevel();
                return;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameOverController: GameManager.RestartLevel() failed: " + e.Message);
        }

        // Try static method as second option
        try
        {
            if (enableDebugLogs)
                Debug.Log("GameOverController: Using GameManager.ForceRestartLevel()");
            GameManager.ForceRestartLevel();
            return;
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameOverController: GameManager.ForceRestartLevel() failed: " + e.Message);
        }

        // Final fallback
        if (enableDebugLogs)
            Debug.Log("GameOverController: All GameManager methods failed, using final fallback");

        StartCoroutine(RestartFallback());
    }

    // Alternative method name that can be called from inspector if RestartGame doesn't work
    public void OnRestartButtonPressed()
    {
        if (enableDebugLogs)
            Debug.Log("GameOverController: OnRestartButtonPressed called (alternative method)");
        RestartGame();
    }

    // Simple direct restart method for inspector use
    public void RestartGameDirect()
    {
        if (enableDebugLogs)
            Debug.Log("GameOverController: RestartGameDirect called");
        
        Time.timeScale = 1f;
        StartCoroutine(RestartFallback());
    }

    private System.Collections.IEnumerator RestartFallback()
    {
        // Reset time scale
        Time.timeScale = 1f;
        
        // Give a moment for any cleanup
        yield return new WaitForSecondsRealtime(0.2f);

        try
        {
            // Try loading by name first
            if (enableDebugLogs)
                Debug.Log("GameOverController: Attempting to load MainScene by name");
            SceneManager.LoadScene("MainScene");
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameOverController: Failed to load MainScene by name: " + e.Message);
            
            try
            {
                // Fallback to build index
                if (enableDebugLogs)
                    Debug.Log("GameOverController: Attempting to load MainScene by build index (2)");
                SceneManager.LoadScene(2);
            }
            catch (System.Exception e2)
            {
                Debug.LogError("GameOverController: Failed to load scene by build index: " + e2.Message);
            }
        }
    }

    public void QuitGame()
    {
        if (enableDebugLogs)
            Debug.Log("GameOverController: QuitGame called");

        try
        {
            // Try loading main menu first
            SceneManager.LoadScene("IntroScene");
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameOverController: Failed to load IntroScene: " + e.Message);
            
            // Fallback to build index for intro scene
            try
            {
                SceneManager.LoadScene(1);
            }
            catch (System.Exception e2)
            {
                Debug.LogError("GameOverController: Failed to load intro scene by index: " + e2.Message);
                
                #if !UNITY_WEBGL
                // Only quit in non-WebGL builds
                Application.Quit();
                #endif
            }
        }
    }

    private void OnDestroy()
    {
        // Clean up button listeners
        if (restartButton != null)
            restartButton.onClick.RemoveAllListeners();
        if (quitButton != null)
            quitButton.onClick.RemoveAllListeners();
    }

    private void OnEnable()
    {
        // Re-setup buttons when the GameObject becomes active
        if (restartButton != null)
            SetupRestartButton();
        if (quitButton != null)
            SetupQuitButton();
    }

    // Method to manually re-setup buttons if needed
    public void RefreshButtonSetup()
    {
        if (enableDebugLogs)
            Debug.Log("GameOverController: RefreshButtonSetup called");
        
        SetupRestartButton();
        SetupQuitButton();
    }
}