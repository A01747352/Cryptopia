using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    public static CoroutineRunner Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        print($"Instance of {nameof(CoroutineRunner)} created.");
    }
}
