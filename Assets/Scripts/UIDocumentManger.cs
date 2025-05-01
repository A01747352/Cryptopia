using UnityEngine;
using UnityEngine.UIElements;

public class UIDocumentManager : MonoBehaviour
{
    public static UIDocumentManager Instance { get; private set; }

    public UIDocument uiDocument;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicates
            return;
        }

        Instance = this;
        print($"Instance of {nameof(UIDocumentManager)} created.");
        DontDestroyOnLoad(gameObject); // Persist across scenes

        uiDocument = GetComponent<UIDocument>();
    }

    public UIDocument GetUIDocument()
    {
        return uiDocument;
    }
    public VisualElement GetRootVisualElement()
    {
        return uiDocument.rootVisualElement;
    }
}
