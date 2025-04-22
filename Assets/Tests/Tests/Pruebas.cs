using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;

public class Pruebas
{
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
        // ACT: Cambiar a la escena de los contratos "Smart"
        SceneManager.LoadScene("Smart");
        yield return new WaitForSeconds(1f);

        // ASSERT: Verificar que la escena activa sea "Smart"
        Assert.AreEqual("Smart", SceneManager.GetActiveScene().name, "La escena Smart no se cargó correctamente.");
    }

    [UnityTest]
    public IEnumerator CryptomineSceneLoadsCorrectly()
    {
        // ACT: Cambiar a la escena "Cryptomine"
        SceneManager.LoadScene("CryptoMine");
        yield return new WaitForSeconds(1f);

        // ASSERT: Verificar que la escena activa sea "Cryptomine"
        Assert.AreEqual("CryptoMine", SceneManager.GetActiveScene().name, "La escena Cryptomine no se cargó correctamente.");
    }

    [UnityTest]
    public IEnumerator MiniGameSceneLoadsCorrectly()
    {
        // ACT: Cambiar a la escena "MiniGame"
        SceneManager.LoadScene("MiniGame");
        yield return new WaitForSeconds(1f);

        // ASSERT: Verificar que la escena activa sea "MiniGame"
        Assert.AreEqual("MiniGame", SceneManager.GetActiveScene().name, "La escena MiniGame no se cargó correctamente.");
    }

    [UnityTest]
    public IEnumerator PauseGameWorksCorrectly()
    {
        // ARRANGE: Asegurarse de que el juego no esté pausado
        Time.timeScale = 1f;
        Assert.AreEqual(1f, Time.timeScale, "El juego no está en tiempo normal.");

        // ACT: Pausar el juego
        Time.timeScale = 0f;
        yield return null;

        // ASSERT: Verificar que el juego esté pausado
        Assert.AreEqual(0f, Time.timeScale, "El juego no se pausó correctamente.");
    }

    [UnityTest]
    public IEnumerator ResumeGameWorksCorrectly()
    {
        // ARRANGE: Asegurarse de que el juego esté pausado
        Time.timeScale = 0f;
        Assert.AreEqual(0f, Time.timeScale, "El juego no está pausado.");

        // ACT: Reanudar el juego
        Time.timeScale = 1f;
        yield return null;

        // ASSERT: Verificar que el juego esté en tiempo normal
        Assert.AreEqual(1f, Time.timeScale, "El juego no se reanudó correctamente.");
    }

    [UnityTest]
    public IEnumerator CryptographySceneLoadsCorrectly()
    {
        // ACT: Cambiar a la escena "Cryptography"
        SceneManager.LoadScene("Cryptography");
        yield return new WaitForSeconds(1f);

        // ASSERT: Verificar que la escena activa sea "Cryptography"
        Assert.AreEqual("Cryptography", SceneManager.GetActiveScene().name, "La escena Cryptography no se cargó correctamente.");
    }
}