using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseButton : MonoBehaviour
{
    private UIDocument gameUIDocument;
    private VisualElement pauseUI;
    private bool isPaused = false;

    void OnEnable()
    {
        gameUIDocument = GetComponent<UIDocument>();
        var root = gameUIDocument.rootVisualElement;

        // Get the pause button and PauseUI
        var button = root.Q<Button>("pauseButton");
        pauseUI = root.Q<VisualElement>("PauseUI");

        // Ensure PauseUI is hidden at the start
        pauseUI.style.display = DisplayStyle.None;

        // Register the button click event
        button.RegisterCallback<ClickEvent>(TogglePause);
    }

    private void TogglePause(ClickEvent evt)
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Pause the game and show PauseUI
            Time.timeScale = 0f;
            pauseUI.style.display = DisplayStyle.Flex;
        }
        else
        {
            // Resume the game and hide PauseUI
            Time.timeScale = 1f;
            pauseUI.style.display = DisplayStyle.None;
        }
    }
}