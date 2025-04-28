using UnityEngine;
using UnityEngine.UIElements;

public class UIDocumentManager : MonoBehaviour
{
    public static UIDocumentManager Instance { get; private set; }

    private UIDocument uiDocument;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes

        uiDocument = GetComponent<UIDocument>();
    }

    public UIDocument GetUIDocument()
    {
        return uiDocument;
    }
}
