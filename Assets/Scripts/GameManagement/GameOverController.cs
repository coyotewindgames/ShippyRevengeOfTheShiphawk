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
        if (restartButton == null)
        {
            restartButton = FindButtonByNames("RestartButton", "TryAgainButton", "Restart", "RestartBtn", "TryAgain");
        }

        if (quitButton == null)
        {
            quitButton = FindButtonByNames("QuitButton", "ExitButton", "Quit", "Exit", "QuitBtn");
        }

        SetupRestartButton();
        SetupQuitButton();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

  
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
                    return button;
                }
            }
        }

        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button button in allButtons)
        {
            string buttonName = button.gameObject.name.ToLower();
            if (buttonName.Contains("restart") || buttonName.Contains("try") || buttonName.Contains("again"))
            {
                return button;
            }
        }

        return null;
    }

    private void SetupRestartButton()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            
            restartButton.onClick.AddListener(() => {
                if (enableDebugLogs)
                RestartGame();
            });

        }
    }

    private void SetupQuitButton()
    {
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            
            quitButton.onClick.AddListener(() => {
                QuitGame();
            });
        }
        
    }


    public void RestartGame()
    {
       

        try
        {
            // Try using GameManager first
            GameManager gameManager = GameManager.EnsureInstance();
            if (gameManager != null)
            {
                if (enableDebugLogs)
                    Debug.Log("GameOverController: Using GameManager.RestartLevel()");
                gameManager.RestartLevel();
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