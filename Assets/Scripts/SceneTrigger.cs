using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    public string sceneToLoad;
    public SceneManagement sceneManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sceneManager.SetSceneToLoad(sceneToLoad);
            Debug.Log($"[SceneTrigger] Player entered. Scene set to: {sceneToLoad}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sceneManager.SetSceneToLoad(null);
            Debug.Log("[SceneTrigger] Player exited.");
        }
    }
}
