using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;

public class Pruebas
{
    // 1. Verificar que la escena del menú cambie correctamente a los minijuegos
    [UnityTest]
    public IEnumerator MenuToMinigame1()
    {
        // ARRANGE: Cargar la escena "City" como punto de partida
        SceneManager.LoadScene("City");
        yield return new WaitForSeconds(1f);

        // ACT: Cambiar a la escena del minijuego "MiniGame"
        SceneManager.LoadScene("MiniGame");
        yield return new WaitForSeconds(1f);

        // ASSERT: Verificar que la escena activa sea "MiniGame"
        Assert.AreEqual("MiniGame", SceneManager.GetActiveScene().name, "La escena MiniGame no se cargó correctamente.");
    }

    [UnityTest]
    public IEnumerator MenuToMinigame2()
    {
        // ARRANGE: Cargar la escena "City" como punto de partida
        SceneManager.LoadScene("City");
        yield return new WaitForSeconds(1f);

        // ACT: Cambiar a la escena del minijuego "Cryptography"
        SceneManager.LoadScene("Cryptography");
        yield return new WaitForSeconds(1f);

        // ASSERT: Verificar que la escena activa sea "Cryptography"
        Assert.AreEqual("Cryptography", SceneManager.GetActiveScene().name, "La escena Cryptography no se cargó correctamente.");
    }

    /*
    // 4. Verificar que el personaje se mueva correctamente
    [UnityTest]
    public IEnumerator PlayerMovesCorrectly()
    {
        // ARRANGE: Cargar la escena de juego
        SceneManager.LoadScene("City");
        yield return new WaitForSeconds(1f); // Espera a que la escena cargue

        var player = GameObject.FindWithTag("Player");
        Assert.IsNotNull(player, "No se encontró el objeto Player en la escena.");

        var initialPosition = player.transform.position;

        var movementGirl = player.GetComponent<MovementGirl>();
        Assert.IsNotNull(movementGirl, "No se encontró el componente MovementGirl en el objeto Player.");

        // ACT: Simular el movimiento del jugador
        movementGirl.SimulateMovement(1f); // Este método debe estar definido en MovementGirl
        yield return new WaitForSeconds(0.2f);

        // ASSERT: Verificar que el jugador se haya movido
        Assert.AreNotEqual(initialPosition, player.transform.position, "El jugador no se movió correctamente.");
    }
    */

    // 3. Dummy test para asegurar que el framework de tests funciona
    [Test]
    public void DummyTest()
    {
        // ARRANGE: No se requiere preparación

        // ACT: No hay acción, solo se prueba el sistema de test

        // ASSERT: Verificación directa
        Assert.Pass("Dummy test passed.");
    }

    [UnityTest]
    public IEnumerator SamrtTest()
    {
        // ACT: Cambiar a la escena del minijuego "Cryptography"
        SceneManager.LoadScene("Smart");
        yield return new WaitForSeconds(1f);

        // ASSERT: Verificar que la escena activa sea "Cryptography"
        Assert.AreEqual("Smart", SceneManager.GetActiveScene().name, "La escena Cryptography no se cargó correctamente.");
    }
}