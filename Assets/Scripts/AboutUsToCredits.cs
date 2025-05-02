using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AboutUsToCredits : MonoBehaviour
{
    private Button backButton;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        backButton = root.Q<Button>("back");
        backButton.clicked += ToCredits;

    }



    private void ToCredits()
    {
        SceneManager.LoadScene("Credits");
    }


}
