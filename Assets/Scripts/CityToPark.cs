using UnityEngine;
using UnityEngine.SceneManagement;

public class CityToPark : MonoBehaviour
{
    public string sceneName = "Park"; // Nombre de la escena de destino

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Verifica que el Player tenga el tag "Player"
        {
            // Cambiar de escena
            SceneManager.LoadScene(sceneName);
        }
    }
}
