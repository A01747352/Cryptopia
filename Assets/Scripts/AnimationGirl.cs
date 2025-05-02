using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AnimationGirl : MonoBehaviour
{
    private Animator animator;
    private MovementGirl movementScript;
    private SpriteRenderer spriteRenderer;
    private string url = Variables.Variables.url;
    private int userId;

    void Start()
    {
        animator = GetComponent<Animator>();
        movementScript = GetComponent<MovementGirl>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    void Awake()
    {
        //userId = PlayerPrefs.GetInt("UserId", 1);
        userId = 1;
        StartCoroutine(CheckItemOwnership(5));
    }
    void Update()
    {
        // Update the animator with the velocity value
        animator.SetFloat("velocity", Mathf.Abs(movementScript.rb.linearVelocity.x));

        // Flip the character based on velocity direction
        if (movementScript.rb.linearVelocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movementScript.rb.linearVelocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private IEnumerator CheckItemOwnership(int itemId)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/cryptoShop/ownsItem/{userId}/{itemId}");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            // Check if the response is "True" or "False"
            if (webRequest.downloadHandler.text == "True")
            {
                animator.SetBool("isGirl2", true);
            }
            else
            {
                Debug.Log("Item not owned, proceed with normal flow.");
            }
        }
        else
        {
            Debug.LogError($"Failed to check item ownership. Error: {webRequest.error}");
            
        }
    }


}
