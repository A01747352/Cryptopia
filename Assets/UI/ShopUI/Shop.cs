using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management
using UnityEngine.UIElements; // Required for Unity UI Toolkit

public class Shop : MonoBehaviour
{
    private Button returnButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the root VisualElement of the UI Document
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Find the button by its name
        returnButton = root.Q<Button>("return");

        // Register the click event
        if (returnButton != null)
        {
            returnButton.clicked += OnReturnButtonClicked;
        }
        else
        {
            Debug.LogError("Return button not found in the UI.");
        }
    }

    // Method to handle the button click
    private void OnReturnButtonClicked()
    {
        SceneManager.LoadScene("City"); // Load the "City" scene
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
