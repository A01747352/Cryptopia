using UnityEngine;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance;
    public bool canTransition = true; // Prevents looping

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
    }

    public void StartTransitionCooldown(float delay)
    {
        canTransition = false;
        Invoke(nameof(EnableTransition), delay);
    }

    private void EnableTransition()
    {
        canTransition = true;
    }
}
