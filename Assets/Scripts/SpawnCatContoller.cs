using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpawnCatController : MonoBehaviour
{
    [SerializeField] private int itemId;              // The ID of the item (cat)
    [SerializeField] private GameObject catObject;    // Reference to the cat GameObject in the scene
    private int userId;                               // The user ID (hardcoded for now)

    private string url = Variables.Variables.url;

    void Awake()
    {
        //userId = PlayerPrefs.GetInt("UserId", 1);
        userId = 1;
    }

    void Start()
    {
        StartCoroutine(CheckCatOwnership(itemId));
    }

    private IEnumerator CheckCatOwnership(int itemId)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/cryptoShop/ownsItem/{userId}/{itemId}");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            // Check if the response is "True" or "False"
            if (webRequest.downloadHandler.text == "True")
            {
                Debug.Log($"User owns cat with itemId {itemId}. Enabling the cat.");
                EnableCat(true);  // Enable the cat GameObject
            }
            else
            {
                Debug.Log("Item not owned, cat will not be enabled.");
                EnableCat(false); // Disable the cat GameObject
            }
        }
        else
        {
            Debug.LogError($"Failed to check item ownership. Error: {webRequest.error}");
        }
    }

    // Function to enable or disable the cat based on ownership
    private void EnableCat(bool isEnabled)
    {
        if (catObject != null)
        {
            catObject.SetActive(isEnabled); // Enable or disable the cat GameObject
        }
        else
        {
            Debug.LogWarning("Cat GameObject reference not assigned.");
        }
    }
}
