using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public string mainSceneName = "City";
    private string currentSceneToLoad;
    private Vector3 savedPlayerPosition;
    private GameObject playerInstance;

    void Awake()
    {
        if (playerInstance == null)
        {
            playerInstance = GameObject.FindWithTag("Player");
            if (playerInstance != null)
            {
                DontDestroyOnLoad(playerInstance);
            }
        }
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(currentSceneToLoad) && Input.GetKeyDown(KeyCode.E))
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

        // Destroy the current player instance if it exists
        if (playerInstance != null)
        {
            Destroy(playerInstance);
            playerInstance = null;
        }

        // Load the new scene
        SceneManager.LoadScene(sceneName);
        yield return null;

        // Instantiate a new player instance after the scene is loaded
        GameObject newPlayerPrefab = Resources.Load<GameObject>("Player"); // Ensure the player prefab is in a Resources folder
        if (newPlayerPrefab != null)
        {
            playerInstance = Instantiate(newPlayerPrefab);
            playerInstance.tag = "Player"; // Ensure the tag is set correctly
            DontDestroyOnLoad(playerInstance);

            if (sceneName == mainSceneName)
            {
                playerInstance.transform.position = savedPlayerPosition;
            }
        }
        else
        {
            Debug.LogError("Player prefab not found in Resources folder.");
        }
    }
}
