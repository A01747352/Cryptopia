using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
    public string sceneToLoad = "MainMenu"; // Nombre de la escena a cargar

    void OnMouseDown()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
