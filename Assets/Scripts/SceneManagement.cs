using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    private string currentSceneToLoad;

    void Update()
    {
        if (!string.IsNullOrEmpty(currentSceneToLoad) && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(LoadLevel(currentSceneToLoad));
        }
    }

    public void SetSceneToLoad(string sceneName)
    {
        currentSceneToLoad = sceneName;
    }

    IEnumerator LoadLevel(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
}
