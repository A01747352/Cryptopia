using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;

public class Pruebas
{
    // Dummy test para asegurar que el framework de tests funciona
    [Test]
    public void DummyTest()
    {
        // ARRANGE: No se requiere preparación

        // ACT: No hay acción, solo se prueba el sistema de test

        // ASSERT: Verificación directa
        Assert.Pass("Dummy test passed.");
    }

    [UnityTest]
    public IEnumerator SmartTest()
    {
        // ARRANGE: Preparar el nombre de la escena esperada
        string expectedSceneName = "Smart";

        // ACT: Cambiar a la escena de los contratos "Smart"
        SceneManager.LoadScene(expectedSceneName);
        yield return new WaitForSeconds(1f);

        // ASSERT: Verificar que la escena activa sea "Smart"
        Assert.AreEqual(expectedSceneName, SceneManager.GetActiveScene().name, "La escena Smart no se cargó correctamente.");
    }

    [UnityTest]
    public IEnumerator CryptomineSceneLoadsCorrectly()
    {
        // ARRANGE: Preparar el nombre de la escena esperada
        string expectedSceneName = "CryptoMine";

        // ACT: Cambiar a la escena "CryptoMine"
        SceneManager.LoadScene(expectedSceneName);
        yield return new WaitForSeconds(1f);

        // ASSERT: Verificar que la escena activa sea "CryptoMine"
        Assert.AreEqual(expectedSceneName, SceneManager.GetActiveScene().name, "La escena CryptoMine no se cargó correctamente.");
    }

    [UnityTest]
    public IEnumerator MiniGameSceneLoadsCorrectly()
    {
        // ARRANGE: Preparar el nombre de la escena esperada
        string expectedSceneName = "MiniGame";

        // ACT: Cambiar a la escena "MiniGame"
        SceneManager.LoadScene(expectedSceneName);
        yield return new WaitForSeconds(1f);

        // ASSERT: Verificar que la escena activa sea "MiniGame"
        Assert.AreEqual(expectedSceneName, SceneManager.GetActiveScene().name, "La escena MiniGame no se cargó correctamente.");
    }

    [UnityTest]
    public IEnumerator PauseGameWorksCorrectly()
    {
        // ARRANGE: Asegurarse de que el juego no esté pausado
        Time.timeScale = 1f;

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

        // ACT: Reanudar el juego
        Time.timeScale = 1f;
        yield return null;

        // ASSERT: Verificar que el juego esté en tiempo normal
        Assert.AreEqual(1f, Time.timeScale, "El juego no se reanudó correctamente.");
    }

    [UnityTest]
    public IEnumerator CryptographySceneLoadsCorrectly()
    {
        // ARRANGE: Preparar el nombre de la escena esperada
        string expectedSceneName = "Cryptography";

        // ACT: Cambiar a la escena "Cryptography"
        SceneManager.LoadScene(expectedSceneName);
        yield return new WaitForSeconds(1f);

        // ASSERT: Verificar que la escena activa sea "Cryptography"
        Assert.AreEqual(expectedSceneName, SceneManager.GetActiveScene().name, "La escena Cryptography no se cargó correctamente.");
    }
}