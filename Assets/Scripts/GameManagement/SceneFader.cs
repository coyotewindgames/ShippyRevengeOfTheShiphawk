using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Auto Transition (Optional)")]
    [SerializeField] private bool autoTransition = false;
    [SerializeField] private float waitSeconds = 2f;
    [SerializeField] private string nextSceneName;
    [SerializeField] private int nextSceneBuildIndex = -1;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponentInChildren<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    private void Start()
    {
        if(!autoTransition) return;
        if (canvasGroup != null)
            StartCoroutine(StartupRoutine());
    }

    public void FadeToScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName)) return;
        StartCoroutine(FadeOut(sceneName));
    }

    public void FadeToConfiguredScene()
    {
        string targetScene = ResolveSceneName();
        if (string.IsNullOrWhiteSpace(targetScene)) return;
        StartCoroutine(FadeOut(targetScene));
    }

    private IEnumerator StartupRoutine()
    {
            yield return new WaitForSeconds(waitSeconds);

        string targetScene = ResolveSceneName();
        if (!string.IsNullOrWhiteSpace(targetScene))
            yield return FadeOut(targetScene);
    }

    private IEnumerator FadeOut(string sceneName)
    {
        Debug.Log("SceneFader: Starting fade to scene: " + sceneName);
        
        if (canvasGroup == null)
        {
            Debug.LogWarning("SceneFader: CanvasGroup is null, loading scene directly");
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsedTime / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, progress);
            yield return null;
        }

        // Ensure fade is complete
        canvasGroup.alpha = 1f;
        
        // Additional delay for WebGL builds
        yield return new WaitForSecondsRealtime(0.1f);
        
        Debug.Log("SceneFader: Loading scene: " + sceneName);
        
        // Try to load the scene with error handling
        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("SceneFader: Failed to load scene '" + sceneName + "': " + e.Message);
            // Fallback - try loading by build index
            if (sceneName == "GameOverScene")
            {
                Debug.Log("SceneFader: Attempting fallback load for GameOverScene by build index");
                SceneManager.LoadScene(3); // GameOverScene is at index 3 based on build settings
            }
        }
    }

    private string ResolveSceneName()
    {
        if (nextSceneBuildIndex >= 0 && nextSceneBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneBuildIndex);
            if (!string.IsNullOrWhiteSpace(scenePath))
                return Path.GetFileNameWithoutExtension(scenePath);
        }

        return nextSceneName;
    }
}
