using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinSceneController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        Debug.Log("WinSceneController: Initializing Win scene");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (restartButton == null)
            restartButton = FindButtonByNames("RestartButton", "PlayAgainButton", "Restart", "PlayAgain", "TryAgain", "TryAgainButton");

        if (mainMenuButton == null)
            mainMenuButton = FindButtonByNames("MainMenuButton", "MenuButton", "MainMenu", "Menu");

        if (quitButton == null)
            quitButton = FindButtonByNames("QuitButton", "ExitButton", "Quit", "Exit");

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestart);
            Debug.Log("WinSceneController: Restart button configured");
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(OnMainMenu);
            Debug.Log("WinSceneController: Main Menu button configured");
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(OnQuit);
            Debug.Log("WinSceneController: Quit button configured");
        }
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
        return null;
    }

    public void OnRestart()
    {
        GameManager gm = GameManager.EnsureInstance();
        if (gm != null)
        {
            gm.RestartLevel();
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainScene");
        }
    }

    public void OnMainMenu()
    {
        GameManager gm = GameManager.EnsureInstance();
        if (gm != null)
        {
            gm.LoadIntroScene();
        }
        else
        {
            SceneManager.LoadScene("IntroScene");
        }
    }

    public void OnQuit()
    {
        GameManager gm = GameManager.EnsureInstance();
        if (gm != null)
        {
            gm.QuitGame();
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    private void OnDestroy()
    {
        if (restartButton != null) restartButton.onClick.RemoveAllListeners();
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveAllListeners();
        if (quitButton != null) quitButton.onClick.RemoveAllListeners();
    }
}
