using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Document")]
    [SerializeField] private UIDocument uiDocument;

    [Header("Transition")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float titleChangeDuration = 1.5f;
    [SerializeField] private string gameSceneName = "SampleScene";

    [Header("Audio")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] [Range(0f, 1f)] private float hoverVolume = 0.3f;
    [SerializeField] [Range(0f, 1f)] private float clickVolume = 0.5f;

    private AudioSource audioSource;
    private Label titleLabel;
    private bool isTransitioning = false;
    private CanvasGroup horrorCanvas;
    private CanvasGroup peacefulCanvas;
    private AudioSource peacefulAudio;
    
    // Button references for color change
    private Button startButton;
    private Button settingsButton;
    private Button quitButton;
    
    // Settings panel elements
    private VisualElement settingsPanel;
    private Slider walkSpeedSlider;
    private Slider runSpeedSlider;
    private Slider jumpForceSlider;
    private Slider volumeSlider;
    private Toggle vsyncToggle;
    private SliderInt fpsSlider;
    
    // Value labels
    private Label walkSpeedValue;
    private Label runSpeedValue;
    private Label jumpForceValue;
    private Label volumeValue;
    private Label fpsValue;

    private void Awake()
    {
        // Get or create AudioSource
        audioSource = GetComponent<AudioSource>();
        var peacefulCanvasObj = GameObject.FindGameObjectWithTag("PeacefulCanvas");
        if (peacefulCanvasObj != null)
        {
            peacefulCanvas = peacefulCanvasObj.GetComponent<CanvasGroup>();
            peacefulAudio = peacefulCanvasObj.GetComponentInChildren<AudioSource>();
        }
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        var root = uiDocument.rootVisualElement;

        // Cache title label for transition
        titleLabel = root.Q<Label>(className: "title-label");

        startButton = root.Q<Button>("start-button");
        settingsButton = root.Q<Button>("settings-button");
        quitButton = root.Q<Button>("quit-button");
        var closeSettingsButton = root.Q<Button>("close-settings-button");
        
        // Find horror canvas by tag
        var horrorCanvasObj = GameObject.FindGameObjectWithTag("HorrorCanvas");
        if (horrorCanvasObj != null)
        {
            horrorCanvas = horrorCanvasObj.GetComponent<CanvasGroup>();
            if (horrorCanvas == null)
                horrorCanvas = horrorCanvasObj.AddComponent<CanvasGroup>();
            horrorCanvasObj.SetActive(false);
        }

        RegisterHoverSound(startButton);
        RegisterHoverSound(settingsButton);
        RegisterHoverSound(quitButton);
        RegisterHoverSound(closeSettingsButton);

       
            startButton.clicked += OnStartGame;

     
            settingsButton.clicked += OnSettings;

            quitButton.clicked += OnQuit;
            
   
            closeSettingsButton.clicked += OnCloseSettings;

        SetupSettingsPanel(root);
    }
    
    private void SetupSettingsPanel(VisualElement root)
    {
        settingsPanel = root.Q<VisualElement>("settings-panel");
       
        walkSpeedSlider = root.Q<Slider>("walk-speed-slider");
        runSpeedSlider = root.Q<Slider>("run-speed-slider");
        jumpForceSlider = root.Q<Slider>("jump-force-slider");
        volumeSlider = root.Q<Slider>("volume-slider");
        vsyncToggle = root.Q<Toggle>("vsync-toggle");
        fpsSlider = root.Q<SliderInt>("fps-slider");
        
      
        walkSpeedValue = root.Q<Label>("walk-speed-value");
        runSpeedValue = root.Q<Label>("run-speed-value");
        jumpForceValue = root.Q<Label>("jump-force-value");
        volumeValue = root.Q<Label>("volume-value");
        fpsValue = root.Q<Label>("fps-value");
        
        
        if (GameManager.EnsureInstance() != null && GameManager.Instance.Settings != null)
        {
            var settings = GameManager.Instance.Settings;
            
            if (walkSpeedSlider != null) walkSpeedSlider.value = settings.walkSpeed;
            if (runSpeedSlider != null) runSpeedSlider.value = settings.runSpeed;
            if (jumpForceSlider != null) jumpForceSlider.value = settings.jumpForce;
            if (volumeSlider != null) volumeSlider.value = settings.masterVolume;
            if (vsyncToggle != null) vsyncToggle.value = settings.enableVSync;
            if (fpsSlider != null) fpsSlider.value = settings.targetFrameRate;
            
            UpdateValueLabels();
        }
        
     
            walkSpeedSlider.RegisterValueChangedCallback(evt => {
                GameManager.Instance?.SetWalkSpeed(evt.newValue);
                if (walkSpeedValue != null) walkSpeedValue.text = evt.newValue.ToString("F1");
            });
            
   
            runSpeedSlider.RegisterValueChangedCallback(evt => {
                GameManager.Instance?.SetRunSpeed(evt.newValue);
                if (runSpeedValue != null) runSpeedValue.text = evt.newValue.ToString("F1");
            });
            
        if (jumpForceSlider != null)
            jumpForceSlider.RegisterValueChangedCallback(evt => {
                GameManager.Instance?.SetJumpForce(evt.newValue);
                if (jumpForceValue != null) jumpForceValue.text = evt.newValue.ToString("F1");
            });
            
        if (volumeSlider != null)
            volumeSlider.RegisterValueChangedCallback(evt => {
                GameManager.Instance?.SetMasterVolume(evt.newValue);
                if (volumeValue != null) volumeValue.text = Mathf.RoundToInt(evt.newValue * 100) + "%";
            });
            
        if (vsyncToggle != null)
            vsyncToggle.RegisterValueChangedCallback(evt => {
                if (GameManager.Instance != null && GameManager.Instance.Settings != null)
                {
                    GameManager.Instance.Settings.enableVSync = evt.newValue;
                    QualitySettings.vSyncCount = evt.newValue ? 1 : 0;
                }
            });
            
        if (fpsSlider != null)
            fpsSlider.RegisterValueChangedCallback(evt => {
                if (GameManager.Instance != null && GameManager.Instance.Settings != null)
                {
                    GameManager.Instance.Settings.targetFrameRate = evt.newValue;
                    Application.targetFrameRate = evt.newValue;
                }
                if (fpsValue != null) fpsValue.text = evt.newValue.ToString();
            });
    }
    
    private void UpdateValueLabels()
    {
        GameManager gameManager = GameManager.EnsureInstance();
        if (gameManager == null || gameManager.Settings == null)
        {
            Debug.LogWarning("MainMenuController: GameManager or Settings is null, cannot update value labels");
            return;
        }
        
        var settings = gameManager.Settings;
        
        if (walkSpeedValue != null) walkSpeedValue.text = settings.walkSpeed.ToString("F1");
        if (runSpeedValue != null) runSpeedValue.text = settings.runSpeed.ToString("F1");
        if (jumpForceValue != null) jumpForceValue.text = settings.jumpForce.ToString("F1");
        if (volumeValue != null) volumeValue.text = Mathf.RoundToInt(settings.masterVolume * 100) + "%";
        if (fpsValue != null) fpsValue.text = settings.targetFrameRate.ToString();
    }

    private void RegisterHoverSound(Button button)
    {

        button.RegisterCallback<MouseEnterEvent>(evt =>
        {
            PlayHoverSound();
        });

        button.RegisterCallback<ClickEvent>(evt =>
        {
            PlayClickSound();
        });
    }

    private void PlayHoverSound()
    {
 
            audioSource.PlayOneShot(hoverSound, hoverVolume);
        
    }

    private void PlayClickSound()
    {
 
            audioSource.PlayOneShot(clickSound, clickVolume);
        
    }

    private void OnStartGame()
    {
        if (isTransitioning) return;
        
        Debug.Log("Start Game clicked!");
        GameManager.EnsureInstance()?.SaveGameSettings();
        StartCoroutine(TransitionToGame());
    }
    
    private IEnumerator TransitionToGame()
    {
        isTransitioning = true;
        
        // Fade out peaceful canvas and its audio
        if (peacefulCanvas != null)
        {
            float startVolume = peacefulAudio != null ? peacefulAudio.volume : 0f;
            float elapsed = 0f;
            while (elapsed < fadeDuration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (fadeDuration * 0.5f);
                peacefulCanvas.alpha = Mathf.Lerp(1f, 0f, t);
                if (peacefulAudio != null)
                    peacefulAudio.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }
            if (peacefulAudio != null)
                peacefulAudio.Stop();
            peacefulCanvas.gameObject.SetActive(false);
        }
        
        if (settingsPanel != null)
            settingsPanel.AddToClassList("hidden");
        
        startButton?.AddToClassList("horror-button");
        settingsButton?.AddToClassList("horror-button");
        quitButton?.AddToClassList("horror-button");
        
        if (horrorCanvas != null)
        {
            horrorCanvas.gameObject.SetActive(true);
            horrorCanvas.alpha = 0f;
            
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                horrorCanvas.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            horrorCanvas.alpha = 1f;
        }
        
        if (titleLabel != null)
        {
            titleLabel.AddToClassList("title-fading");
            yield return new WaitForSeconds(titleChangeDuration * 0.5f);
            
            titleLabel.text = "Kevins Bad Day";
            
            titleLabel.RemoveFromClassList("title-fading");
            yield return new WaitForSeconds(titleChangeDuration * 0.5f);
        }
        
        yield return new WaitForSeconds(1.5f);
        
        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    private void OnSettings()
    {
     
        if (settingsPanel != null)
        {
            bool isHidden = settingsPanel.ClassListContains("hidden");
            if (isHidden)
            {
                settingsPanel.RemoveFromClassList("hidden");
                UpdateValueLabels(); 
            }
            else
            {
                settingsPanel.AddToClassList("hidden");
            }
        }
    }
    
    private void OnCloseSettings()
    {
        Debug.Log("Close Settings clicked!");
        if (settingsPanel != null)
        {
            settingsPanel.AddToClassList("hidden");
        }
        GameManager.Instance?.SaveGameSettings();
    }

    private void OnQuit()
    {
        GameManager.EnsureInstance()?.SaveGameSettings();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
