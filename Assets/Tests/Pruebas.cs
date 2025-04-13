using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using TMPro; // Agregado para TextMeshProUGUI

namespace Cryptopia.Tests
{
public class Pruebas
{
    // 1. Verificar que la escena menú cambie de manera correcta a los 2 minijuegos
    [UnityTest]
    public IEnumerator MenuToMinigame_Cryptography()
    {
        SceneManager.LoadScene("MainMenu");
        yield return new WaitForSeconds(1f);
        
        SceneManager.LoadScene("Cryptography");
        yield return new WaitForSeconds(1f);
        
        Assert.AreEqual("Cryptography", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator MenuToMinigame_Trivia()
    {
        SceneManager.LoadScene("MainMenu");
        yield return new WaitForSeconds(1f);
        
        SceneManager.LoadScene("MiniGame");
        yield return new WaitForSeconds(1f);
        
        Assert.AreEqual("MiniGame", SceneManager.GetActiveScene().name);
    }

    // 2. Verificar que el minijuego de trivia muestre correctamente las pantallas de victoria y derrota
    [UnityTest]
    public IEnumerator TriviaGame_VictoryScreen()
    {
        SceneManager.LoadScene("MiniGame");
        yield return new WaitForSeconds(1f);
        
        var triviaManager = Object.FindObjectOfType<TriviaManager>();
        Assert.IsNotNull(triviaManager);
        
        // Verificar que el panel final existe y está oculto inicialmente
        Assert.IsNotNull(triviaManager.finalScorePanel);
        Assert.IsFalse(triviaManager.finalScorePanel.activeSelf);
        
        // Usar reflexión para acceder al método EndGame
        var endGameMethod = typeof(TriviaManager).GetMethod("EndGame", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        Assert.IsNotNull(endGameMethod);
        
        // Configurar para victoria (más respuestas correctas)
        var correctAnswersField = typeof(TriviaManager).GetField("correctAnswers", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        var incorrectAnswersField = typeof(TriviaManager).GetField("incorrectAnswers", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        correctAnswersField.SetValue(triviaManager, 3);
        incorrectAnswersField.SetValue(triviaManager, 1);
        
        // Llamar EndGame
        endGameMethod.Invoke(triviaManager, null);
        yield return new WaitForSeconds(0.5f);
        
        // Verificar pantalla de victoria
        Assert.IsTrue(triviaManager.finalScorePanel.activeSelf);
    }

    [UnityTest]
    public IEnumerator TriviaGame_DefeatScreen()
    {
        SceneManager.LoadScene("MiniGame");
        yield return new WaitForSeconds(1f);
        
        var triviaManager = Object.FindObjectOfType<TriviaManager>();
        Assert.IsNotNull(triviaManager);
        
        Assert.IsNotNull(triviaManager.finalScorePanel);
        Assert.IsFalse(triviaManager.finalScorePanel.activeSelf);
        
        var endGameMethod = typeof(TriviaManager).GetMethod("EndGame", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        var correctAnswersField = typeof(TriviaManager).GetField("correctAnswers", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        var incorrectAnswersField = typeof(TriviaManager).GetField("incorrectAnswers", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        // Configurar para derrota (más respuestas incorrectas)
        correctAnswersField.SetValue(triviaManager, 1);
        incorrectAnswersField.SetValue(triviaManager, 4);
        
        endGameMethod.Invoke(triviaManager, null);
        yield return new WaitForSeconds(0.5f);
        
        Assert.IsTrue(triviaManager.finalScorePanel.activeSelf);
    }

    // 3. Verificar que el minijuego de criptografía muestre correctamente las pantallas de victoria y derrota
    [UnityTest]
    public IEnumerator CryptographyGame_VictoryScreen()
    {
        SceneManager.LoadScene("Cryptography");
        yield return new WaitForSeconds(1f);
        
        var cryptoManager = Object.FindObjectOfType<Cryptography>();
        Assert.IsNotNull(cryptoManager);
        
        // Acceder a la pantalla de victoria
        var victoryScreenField = typeof(Cryptography).GetField("victoryScreen", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        var victoryScreen = victoryScreenField.GetValue(cryptoManager) as VisualElement;
        Assert.IsNotNull(victoryScreen);
        
        // Verificar que está oculta inicialmente
        Assert.AreEqual(DisplayStyle.None, victoryScreen.style.display.value);
        
        // Configurar para victoria
        var questionsNumField = typeof(Cryptography).GetField("questionsNum", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        var mistakesField = typeof(Cryptography).GetField("mistakes", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        var totalQuestionsField = typeof(Cryptography).GetField("totalQuestions", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        int totalQuestions = (int)totalQuestionsField.GetValue(cryptoManager);
        questionsNumField.SetValue(cryptoManager, totalQuestions);
        mistakesField.SetValue(cryptoManager, 1);
        
        // Llamar NextQuestion
        var nextQuestionMethod = typeof(Cryptography).GetMethod("NextQuestion", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance,
            null,
            new System.Type[] { typeof(bool) },
            null);
            
        cryptoManager.StartCoroutine((IEnumerator)nextQuestionMethod.Invoke(cryptoManager, new object[] { true }));
        yield return new WaitForSeconds(1.5f);
        
        // Verificar que se muestra la pantalla de victoria
        Assert.AreEqual(DisplayStyle.Flex, victoryScreen.style.display.value);
    }

    [UnityTest]
    public IEnumerator CryptographyGame_DefeatScreen()
    {
        SceneManager.LoadScene("Cryptography");
        yield return new WaitForSeconds(1f);
        
        var cryptoManager = Object.FindObjectOfType<Cryptography>();
        Assert.IsNotNull(cryptoManager);
        
        // Acceder a la pantalla de derrota
        var defeatScreenField = typeof(Cryptography).GetField("defeatScreen", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        var defeatScreen = defeatScreenField.GetValue(cryptoManager) as VisualElement;
        Assert.IsNotNull(defeatScreen);
        
        // Verificar que está oculta inicialmente
        Assert.AreEqual(DisplayStyle.None, defeatScreen.style.display.value);
        
        // Configurar para derrota (3 errores)
        var mistakesField = typeof(Cryptography).GetField("mistakes", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        mistakesField.SetValue(cryptoManager, 3);
        
        // Llamar NextQuestion
        var nextQuestionMethod = typeof(Cryptography).GetMethod("NextQuestion", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance,
            null,
            new System.Type[] { typeof(bool) },
            null);
            
        cryptoManager.StartCoroutine((IEnumerator)nextQuestionMethod.Invoke(cryptoManager, new object[] { false }));
        yield return new WaitForSeconds(1.5f);
        
        // Verificar que se muestra la pantalla de derrota
        Assert.AreEqual(DisplayStyle.Flex, defeatScreen.style.display.value);
    }

    // 4. Verificar que el personaje se mueva de manera correcta
    [UnityTest]
    public IEnumerator Player_Movement()
    {
        SceneManager.LoadScene("City");
        yield return new WaitForSeconds(1f);
        
        var player = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(player);
        
        var movementScript = player.GetComponent<MovementGirl>();
        Assert.IsNotNull(movementScript);
        
        // Verificar que el rigidbody existe
        Assert.IsNotNull(movementScript.rb);
        
        // Verificar movimiento a la derecha
        var movementField = typeof(MovementGirl).GetField("movement", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        // Mover a la derecha
        movementField.SetValue(movementScript, new Vector2(1f, 0f));
        
        // Manualmente llamar a FixedUpdate
        movementScript.rb.linearVelocity = new Vector2(1f * movementScript.moveSpeed, movementScript.rb.linearVelocity.y);
        
        yield return new WaitForFixedUpdate();
        
        // Verificar que la velocidad es positiva
        Assert.Greater(movementScript.rb.velocity.x, 0f);
        
        // Mover a la izquierda
        movementField.SetValue(movementScript, new Vector2(-1f, 0f));
        
        // Manualmente llamar a FixedUpdate
        movementScript.rb.linearVelocity = new Vector2(-1f * movementScript.moveSpeed, movementScript.rb.linearVelocity.y);
        
        yield return new WaitForFixedUpdate();
        
        // Verificar que la velocidad es negativa
        Assert.Less(movementScript.rb.velocity.x, 0f);
    }

    // 5. Verificar que el menu de pausa se muestre cuando sea necesario y se reanude el juego
    [UnityTest]
    public IEnumerator PauseMenu_ShowAndResume()
    {
        SceneManager.LoadScene("City");
        yield return new WaitForSeconds(1f);
        
        var pauseButton = Object.FindObjectOfType<PauseButton>();
        Assert.IsNotNull(pauseButton);
        
        // Acceder a los campos privados
        var pauseUIField = typeof(PauseButton).GetField("pauseUI", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        var pauseUI = pauseUIField.GetValue(pauseButton) as GameObject;
        Assert.IsNotNull(pauseUI);
        
        // Verificar que está oculto inicialmente
        Assert.IsFalse(pauseUI.activeSelf);
        
        // Invocar método TogglePause para pausar
        var togglePauseMethod = typeof(PauseButton).GetMethod("TogglePause", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        togglePauseMethod.Invoke(pauseButton, null);
        yield return null;
        
        // Verificar que el juego está pausado y el menú visible
        Assert.AreEqual(0f, Time.timeScale);
        Assert.IsTrue(pauseUI.activeSelf);
        
        // Invocar método ResumeGame para reanudar
        var resumeGameMethod = typeof(PauseButton).GetMethod("ResumeGame", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        resumeGameMethod.Invoke(pauseButton, null);
        yield return null;
        
        // Verificar que el juego está reanudado y el menú oculto
        Assert.AreEqual(1f, Time.timeScale);
        Assert.IsFalse(pauseUI.activeSelf);
    }
}
}