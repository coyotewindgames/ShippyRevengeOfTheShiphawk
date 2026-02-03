using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenLoader : MonoBehaviour
{
    [SerializeField] private float waitSeconds = 2f;
    [SerializeField] private string nextSceneName = "IntroScene"; // Set this to your title scene name
    [SerializeField] private int nextSceneBuildIndex = -1; // Set to a valid build index to bypass name lookup

    private void Start()
    {
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private System.Collections.IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(waitSeconds);

        if (nextSceneBuildIndex >= 0 && nextSceneBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneBuildIndex);
            yield break;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
