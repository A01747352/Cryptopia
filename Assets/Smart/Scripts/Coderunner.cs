using UnityEngine;

public class CodeRunner : MonoBehaviour
{
    public static CodeRunner Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        print($"Instance of {nameof(CodeRunner)} created.");
    }
}