using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToGame : MonoBehaviour
{
    public string sceneToLoad = "City"; // Nombre de la escena a cargar

    void OnMouseDown()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}