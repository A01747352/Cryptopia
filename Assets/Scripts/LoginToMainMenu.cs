using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoginToMainMenu : MonoBehaviour
{
    private Button loginButton;


    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        loginButton = root.Q<Button>("loginButton");
        loginButton.clicked += OnLoginButtonClicked;
    }

    private void OnLoginButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
