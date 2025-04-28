using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    public static PlayerSpawn instance;
    public Vector2 cityStartPosition = new Vector2(35f, -6.5f); // First-time start position
    public Vector2 parkSpawnPosition = new Vector2(35.5f, -7.5f); // Spawn when entering Park
    public Vector2 cityReturnPosition = new Vector2(35.5f, -7.5f); // Spawn when returning to City

    private SpriteRenderer spriteRenderer;
    private MovementGirl movementScript;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        movementScript = GetComponent<MovementGirl>();
    }

    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // Check if a saved position exists
        if (PlayerPrefs.HasKey("SavedPlayerX"))
        {
            float x = PlayerPrefs.GetFloat("SavedPlayerX");
            float y = PlayerPrefs.GetFloat("SavedPlayerY");
            float z = PlayerPrefs.GetFloat("SavedPlayerZ");

            transform.position = new Vector3(x, y, z);
            Debug.Log($"[PlayerSpawn] Loaded saved position: {transform.position}");
        }
        else
        {
            // No saved position, use default spawn rules
            if (currentScene == "City")
            {
                if (PlayerPrefs.GetInt("HasEnteredPark", 0) == 0)
                {
                    transform.position = cityStartPosition; // First-time start position
                }
                else
                {
                    transform.position = cityReturnPosition; // Returning from Park
                }
                spriteRenderer.flipX = false; // Normal direction
            }
            else if (currentScene == "Park")
            {
                transform.position = parkSpawnPosition; // Entering Park
                spriteRenderer.flipX = true; // Flip when entering Park
                PlayerPrefs.SetInt("HasEnteredPark", 1); // Save that the player has entered Park
            }
            Debug.Log($"[PlayerSpawn] No saved position found, used default spawn for scene: {currentScene}");
        }

        // Disable movement for 1 second to prevent accidental re-triggering
        if (movementScript != null)
        {
            movementScript.enabled = false;
            Invoke(nameof(EnableMovement), 1f);
        }
    }

    void EnableMovement()
    {
        if (movementScript != null)
        {
            movementScript.enabled = true;
        }
    }
}
