using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CreditsToMainMenu : MonoBehaviour
{
    private Button toMenuButton;
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        toMenuButton = root.Q<Button>("toMenuButton");
        toMenuButton.clicked += OnLoginButtonClicked;
    }



    private void OnLoginButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
