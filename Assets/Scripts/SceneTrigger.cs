using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    public string sceneToLoad;
    public SceneManagement sceneManager;

    public GameObject exclamation;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Save the player's position
            SavePlayerPosition(other.transform.position);

            // Set scene to load
            sceneManager.SetSceneToLoad(sceneToLoad);
            Debug.Log($"[SceneTrigger] Player entered. Scene set to: {sceneToLoad}");

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

    private void SavePlayerPosition(Vector3 position)
    {
        PlayerPrefs.SetFloat("SavedPlayerX", position.x);
        PlayerPrefs.SetFloat("SavedPlayerY", position.y);
        PlayerPrefs.SetFloat("SavedPlayerZ", position.z);
        PlayerPrefs.Save();

        Debug.Log($"[SceneTrigger] Saved Player Position: {position}");
    }
}
