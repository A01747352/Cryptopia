using UnityEngine;
using UnityEngine.SceneManagement;

public class CityToPark : MonoBehaviour
{
    public string sceneName = "Park";
    public float transitionCooldown = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && SceneTransitionManager.instance.canTransition)
        {
            SceneTransitionManager.instance.StartTransitionCooldown(transitionCooldown);
            SceneManager.LoadScene(sceneName);
        }
    }
}
