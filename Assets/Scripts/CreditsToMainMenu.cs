using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CreditsToMainMenu : MonoBehaviour
{
    private Button toMenuButton;
    private Button learnMoreButton;
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        toMenuButton = root.Q<Button>("toMenuButton");
        toMenuButton.clicked += OnLoginButtonClicked;
        learnMoreButton = root.Q<Button>("moreInfo");
        learnMoreButton.clicked += OnlearnMoreButtonClicked;
    }



    private void OnLoginButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnlearnMoreButtonClicked()
    {
        SceneManager.LoadScene("AboutUs");
    }
}
