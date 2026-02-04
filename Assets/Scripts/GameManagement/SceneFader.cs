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
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
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
