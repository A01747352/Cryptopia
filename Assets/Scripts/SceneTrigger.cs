using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    public string sceneToLoad;
    public SceneManagement sceneManager;

    // Referencia al objeto "Exclamation" asignada desde el inspector
    public GameObject exclamation;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sceneManager.SetSceneToLoad(sceneToLoad);
            Debug.Log($"[SceneTrigger] Player entered. Scene set to: {sceneToLoad}");

            // Activar el objeto "Exclamation"
            if (exclamation != null)
            {
                exclamation.SetActive(true);
            }
            else
            {
                Debug.LogWarning("[SceneTrigger] 'Exclamation' object reference is not set.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sceneManager.SetSceneToLoad(null);
            Debug.Log("[SceneTrigger] Player exited.");

            // Desactivar el objeto "Exclamation"
            if (exclamation != null)
            {
                exclamation.SetActive(false);
            }
            else
            {
                Debug.LogWarning("[SceneTrigger] 'Exclamation' object reference is not set.");
            }
        }
    }
}
