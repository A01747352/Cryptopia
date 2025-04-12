using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ButtonsMainMenu : MonoBehaviour
{
    private UIDocument uiDocument;

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        var playButton = root.Q<Button>("play");
        if (playButton != null)
        {
            playButton.RegisterCallback<ClickEvent>(PlayGame);
        }

        var exitButton = root.Q<Button>("exit");
        if (exitButton != null)
        {
            exitButton.RegisterCallback<ClickEvent>(QuitGame);
        }

        var creditsButton = root.Q<Button>("credits");
        if (creditsButton != null)
        {
            creditsButton.RegisterCallback<ClickEvent>(GoToCredits);
        }
    }

    private void PlayGame(ClickEvent evt)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("City");
    }

    private void QuitGame(ClickEvent evt)
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit(); 
        #endif
    }

    private void GoToCredits(ClickEvent evt)
    {
        SceneManager.LoadScene("Credits");
    }
}
