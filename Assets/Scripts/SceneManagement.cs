using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    // Variable pública para asignar el nombre de la escena desde el Inspector
    public string sceneToLoad;

    void Update()
    {
        // Comprueba si se presiona un botón del mouse
        if (Input.GetMouseButtonDown(0))
        {
            // Carga la escena configurada en el Inspector
            LoadMiniGame(sceneToLoad);
        }
    }

    public void LoadMiniGame(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }


    IEnumerator LoadLevel(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
}
