using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoginToMainMenu : MonoBehaviour
{
    private Button loginButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        loginButton = root.Q<Button>("loginButton");
        loginButton.clicked += OnLoginButtonClicked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnLoginButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
