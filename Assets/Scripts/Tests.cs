using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;

public class Pruebas
{
    
    [UnityTest]
    public IEnumerator MenuToMinigame1()
    {
        // ARRANGE: Cargar escena de menú principal
        SceneManager.LoadScene("MainMenu");
        yield return new WaitForSeconds(1f); // Esperar a que la escena cargue

        // ACT: Cambiar a la escena del minijuego 1
        SceneManager.LoadScene("MiniGame");
        yield return new WaitForSeconds(1f); // Esperar a que la escena cargue

        // ASSERT: Verificar que la escena activa sea MiniGame
        Assert.AreEqual("MiniGame", SceneManager.GetActiveScene().name, "La escena Minigame1 no se cargó correctamente.");
    }

    [UnityTest]
    public IEnumerator MenuToMinigame2()
    {
        // ARRANGE: Cargar escena de menú principal
        SceneManager.LoadScene("MainMenu");
        yield return new WaitForSeconds(1f);

        // ACT: Cambiar a la escena del minijuego 2
        SceneManager.LoadScene("Cryptography");
        yield return new WaitForSeconds(1f);

        // ASSERT: Verificar que la escena activa sea Cryptography
        Assert.AreEqual("Cryptography", SceneManager.GetActiveScene().name, "La escena Minigame2 no se cargó correctamente.");
    }

    [UnityTest]
    public IEnumerator PlayerMovesCorrectly()
    {
        // ARRANGE: Cargar la escena de juego
        SceneManager.LoadScene("City");
        yield return new WaitForSeconds(1f);

        var player = GameObject.FindWithTag("Player");
        Assert.IsNotNull(player, "No se encontró el objeto Player en la escena.");

        var initialPosition = player.transform.position;

        var playerController = player.GetComponent<MovementGirl>();
        Assert.IsNotNull(playerController, "No se encontró el componente MovementGirl en el objeto Player.");

        // ACT: Activar el movimiento del personaje
        playerController.enabled = true;
        yield return new WaitForSeconds(1f);

        // ASSERT: Verificar que el jugador se haya movido
        Assert.AreNotEqual(initialPosition, player.transform.position, "El jugador no se movió correctamente.");
    }
}