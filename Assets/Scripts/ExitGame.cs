using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ExitController : MonoBehaviour
{
    private UIDocument uiDocument;

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        var button = root.Q<Button>("exit"); // Looking for the "Exit" button

        if (button != null)
        {
            button.RegisterCallback<ClickEvent>(QuitGame);
        }
    }

    private void QuitGame(ClickEvent evt)
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode(); // Stops play mode in the Unity Editor
        #else
            Application.Quit(); // Closes the game in a built application
        #endif
    }
}
