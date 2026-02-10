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
        
        if (canvasGroup == null)
        {
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

        canvasGroup.alpha = 1f;
        
        yield return new WaitForSecondsRealtime(0.1f);
        
        
        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch (System.Exception e)
        {
            if (sceneName == "GameOverScene")
            {
                SceneManager.LoadScene(3); 
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
