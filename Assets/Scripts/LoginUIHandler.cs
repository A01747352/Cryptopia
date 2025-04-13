using UnityEngine;
using UnityEngine.UIElements;

public class LoginUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject registerUIObject;

    private VisualElement root;

    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        var registerButton = root.Q<Button>("registerButton");

        if (registerButton != null)
        {
            registerButton.clicked -= OnRegisterClicked;
            registerButton.clicked += OnRegisterClicked;
        }
    }

    private void OnRegisterClicked()
    {
        gameObject.SetActive(false);            // Oculta Login
        registerUIObject.SetActive(true);       // Muestra Register
    }
}
