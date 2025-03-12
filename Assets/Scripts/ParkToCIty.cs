using UnityEngine;
using UnityEngine.SceneManagement;

public class ParkToCity : MonoBehaviour
{
    public string sceneName = "City"; // Asegúrate de escribir el nombre EXACTO de la escena a cargar

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // El Player debe tener el tag "Player"
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
