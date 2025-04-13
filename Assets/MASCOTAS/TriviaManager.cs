using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class TriviaManager : MonoBehaviour
{
    [System.Serializable]
    public class QuestionData
    {
        public string questionText;
        public string[] answers;
        public string[] feedbacks;
        public int correctAnswerIndex;
        public Sprite backgroundImage;
        public Sprite petImage;
    }

    [Header("Questions")]
    public QuestionData[] questions; // Preguntas de respaldo locales
    
    [Header("UI References")]
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public Image backgroundPanel;
    public Image petImage;
    public GameObject resultadoPanel;
    public Image boxText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI feedbackText;
    public Sprite correctSprite;
    public Sprite incorrectSprite;
    public GameObject finalScorePanel;
    public TextMeshProUGUI finalScoreText;
    public Image[] starBoxes;
    public Sprite starFilled;
    public Sprite starEmpty;
    public Image catCornerImage;
    public Sprite catHappy;
    public Sprite catSad;

    // Game state variables
    private int currentQuestionIndex = 0;
    private int score = 0;
    private int correctAnswers = 0;
    private int incorrectAnswers = 0;
    private float tkns = 0f;
    private int totalQuestions = 5;
    private static int userId = 1; // Default user ID
    private static int idGame;
    private string url = "http://localhost:8080";

    // Clases para la base de datos
    [System.Serializable]
    public class TriviaQuestion
    {
        public int idPregunta;
        public string pregunta;
        public int puntos;
    }

    [System.Serializable]
    public class TriviaAnswer
    {
        public int idRespuesta;
        public string respuesta;
        public string retroalimentacion;
        public bool resultado;
        public int idPregunta;
    }

    [System.Serializable]
    public class QuestionWithAnswers
    {
        public TriviaQuestion question;
        public List<TriviaAnswer> answers;
    }

    // Lista para almacenar las preguntas cargadas desde la BD
    private List<QuestionWithAnswers> loadedQuestions = new List<QuestionWithAnswers>();
    private QuestionWithAnswers currentQuestion;

    // Data structures
    public struct SubmittedAnswer
    {
        public string respuestaUsuario;
        public int idPregunta;
        public bool esCorrecta;
        public int idPartida;
    }

    public struct GameStats
    {
        public int idPartida;
        public string resultado;
        public float porcentajeAciertos;
        public int puntaje;
        public float TKNs;
        public int idMinijuego;
        public int idUsuario;
    }

    void Start()
    {
        // Hide panels
        resultadoPanel.SetActive(false);
        finalScorePanel.SetActive(false);

        // Set up answer buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Important for closure
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }

        // Start a new game
        StartCoroutine(RegisterNewGame());
    }

    private IEnumerator RegisterNewGame()
    {
        Debug.Log("Registrando nuevo juego de trivia...");
        UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/trivia/newGame/{userId}");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string idGameJSONstring = webRequest.downloadHandler.text;
            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(idGameJSONstring);

            // Access the value by key
            if (data.ContainsKey("idGame"))
            {
                idGame = System.Convert.ToInt32(data["idGame"]);
                Debug.Log($"Juego de trivia registrado con ID: {idGame}");
                
                // Cargar todas las preguntas disponibles
                StartCoroutine(LoadAllQuestions());
            }
            else
            {
                Debug.LogWarning("La respuesta del servidor no contiene idGame. Usando preguntas locales.");
                LoadLocalQuestion();
            }
        }
        else
        {
            Debug.LogWarning($"Error registrando nuevo juego: {webRequest.error}. Usando preguntas locales.");
            LoadLocalQuestion();
        }
    }

    private IEnumerator LoadAllQuestions()
    {
        Debug.Log("Cargando preguntas de trivia...");
        UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/trivia/getAllQuestions");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string response = webRequest.downloadHandler.text;
            try 
            {
                loadedQuestions = JsonConvert.DeserializeObject<List<QuestionWithAnswers>>(response);
                Debug.Log($"Cargadas {loadedQuestions.Count} preguntas de la base de datos");
                
                if (loadedQuestions.Count > 0)
                {
                    // Mezclar las preguntas para que sean aleatorias
                    ShuffleQuestions();
                    
                    // Limitar el número de preguntas al total deseado
                    if (loadedQuestions.Count > totalQuestions)
                    {
                        loadedQuestions = loadedQuestions.GetRange(0, totalQuestions);
                    }
                    
                    // Cargar la primera pregunta
                    LoadNextDBQuestion();
                }
                else
                {
                    Debug.LogWarning("No se encontraron preguntas en la base de datos. Usando preguntas locales.");
                    LoadLocalQuestion();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error al parsear las preguntas: {e.Message}. Usando preguntas locales.");
                LoadLocalQuestion();
            }
        }
        else
        {
            Debug.LogWarning($"Error cargando preguntas: {webRequest.error}. Usando preguntas locales.");
            LoadLocalQuestion();
        }
    }

    private void ShuffleQuestions()
    {
        // Mezclar las preguntas usando el algoritmo Fisher-Yates
        System.Random rng = new System.Random();
        int n = loadedQuestions.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            QuestionWithAnswers temp = loadedQuestions[k];
            loadedQuestions[k] = loadedQuestions[n];
            loadedQuestions[n] = temp;
        }
    }

    private void LoadNextDBQuestion()
    {
        if (currentQuestionIndex < loadedQuestions.Count)
        {
            currentQuestion = loadedQuestions[currentQuestionIndex];
            
            // Mostrar la pregunta
            questionText.text = currentQuestion.question.pregunta;
            
            // Configurar los botones de respuesta
            for (int i = 0; i < answerButtons.Length; i++)
            {
                TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                
                if (i < currentQuestion.answers.Count)
                {
                    answerButtons[i].gameObject.SetActive(true);
                    buttonText.text = currentQuestion.answers[i].respuesta;
                    answerButtons[i].interactable = true;
                }
                else
                {
                    // Ocultar botones extras si hay menos de 4 respuestas
                    answerButtons[i].gameObject.SetActive(false);
                }
            }
            
            // Si hay imágenes configuradas en las preguntas locales, usarlas como respaldo
            if (currentQuestionIndex < questions.Length)
            {
                if (backgroundPanel != null && questions[currentQuestionIndex].backgroundImage != null)
                    backgroundPanel.sprite = questions[currentQuestionIndex].backgroundImage;
                    
                if (petImage != null && questions[currentQuestionIndex].petImage != null)
                    petImage.sprite = questions[currentQuestionIndex].petImage;
            }
        }
        else
        {
            EndGame();
        }
    }

    private void LoadLocalQuestion()
    {
        if (currentQuestionIndex < questions.Length)
        {
            // Get current question
            QuestionData question = questions[currentQuestionIndex];
            
            // Set question text
            questionText.text = question.questionText;
            
            // Set answer texts
            for (int i = 0; i < answerButtons.Length; i++)
            {
                TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null && i < question.answers.Length)
                {
                    answerButtons[i].gameObject.SetActive(true);
                    buttonText.text = question.answers[i];
                    answerButtons[i].interactable = true;
                }
                else
                {
                    answerButtons[i].gameObject.SetActive(false);
                }
            }
            
            // Set background and pet images if available
            if (question.backgroundImage != null)
                backgroundPanel.sprite = question.backgroundImage;
                
            if (question.petImage != null)
                petImage.sprite = question.petImage;
        }
        else
        {
            EndGame();
        }
    }

    private void CheckAnswer(int answerIndex)
    {
        // Disable buttons to prevent multiple answers
        foreach (Button btn in answerButtons)
        {
            btn.interactable = false;
        }
        
        bool isCorrect = false;
        string feedback = "";
        int points = 100; // Puntos predeterminados
        
        // Verificar si estamos usando preguntas de la base de datos o locales
        if (currentQuestion != null && currentQuestion.answers != null && answerIndex < currentQuestion.answers.Count)
        {
            // Usando preguntas de la base de datos
            isCorrect = currentQuestion.answers[answerIndex].resultado;
            feedback = currentQuestion.answers[answerIndex].retroalimentacion;
            points = currentQuestion.question.puntos;
            
            // Enviar respuesta a la base de datos con la nueva estructura
            StartCoroutine(SendAnswerUser(answerIndex, isCorrect));
        }
        else if (currentQuestionIndex < questions.Length)
        {
            // Usando preguntas locales
            QuestionData currentLocalQuestion = questions[currentQuestionIndex];
            isCorrect = (answerIndex == currentLocalQuestion.correctAnswerIndex);
            feedback = currentLocalQuestion.feedbacks[answerIndex];
        }
        
        // Actualizar puntaje y contadores
        if (isCorrect)
        {
            correctAnswers++;
            score += points;
            
            // Mostrar retroalimentación de acierto
            resultText.text = "¡Correcto!";
            boxText.sprite = correctSprite;
            
            // Reproducir sonido si está disponible
            AudioSource correctSound = GameObject.Find("SoundCorrect")?.GetComponent<AudioSource>();
            if (correctSound != null)
                correctSound.Play();
        }
        else
        {
            incorrectAnswers++;
            
            // Mostrar retroalimentación de error
            resultText.text = "Incorrecto :(";
            boxText.sprite = incorrectSprite;
            
            // Reproducir sonido si está disponible
            AudioSource incorrectSound = GameObject.Find("SoundIncorrect")?.GetComponent<AudioSource>();
            if (incorrectSound != null)
                incorrectSound.Play();
        }
        
        // Mostrar retroalimentación
        feedbackText.text = feedback;
        resultadoPanel.SetActive(true);
        
        // Continuar al siguiente turno después de un retraso
        StartCoroutine(NextQuestion());
    }

    private IEnumerator SendAnswerUser(int answerIndex, bool esCorrecta)
    {
        SubmittedAnswer submittedAnswer;
        submittedAnswer.respuestaUsuario = currentQuestion.answers[answerIndex].respuesta;
        submittedAnswer.idPregunta = currentQuestion.question.idPregunta;
        submittedAnswer.esCorrecta = esCorrecta;
        submittedAnswer.idPartida = idGame;

        string jsonData = JsonConvert.SerializeObject(submittedAnswer);
        UnityWebRequest webRequest = UnityWebRequest.Post($"{url}/trivia/submitAnswer", jsonData, "application/json");
        webRequest.SetRequestHeader("Content-Type", "application/json");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Respuesta enviada correctamente a la base de datos.");
        }
        else
        {
            Debug.LogError($"Error enviando respuesta: {webRequest.error}");
        }
    }

    private IEnumerator NextQuestion()
    {
        // Wait for 3 seconds to show feedback
        yield return new WaitForSeconds(3f);
        
        // Hide result panel
        resultadoPanel.SetActive(false);
        
        // Move to next question
        currentQuestionIndex++;
        
        // Load next question based on mode
        if (loadedQuestions.Count > 0)
        {
            LoadNextDBQuestion();
        }
        else
        {
            LoadLocalQuestion();
        }
    }

    private void EndGame()
    {
        int totalQuestionsAnswered = correctAnswers + incorrectAnswers;
        if (totalQuestionsAnswered == 0) totalQuestionsAnswered = 1; // Evitar división por cero
        
        // Calculate percentage of correct answers
        float percentage = (float)correctAnswers / totalQuestionsAnswered * 100;
        bool isVictory = correctAnswers > (totalQuestionsAnswered / 2); // Win if more than half correct
        
        // Calculate tokens earned
        tkns = score * 0.003f;
        
        // Save game results if online
        if (idGame > 0)
        {
            StartCoroutine(SaveGame(isVictory, percentage));
        }
        
        // Show final score panel
        finalScorePanel.SetActive(true);
        
        // Update stars
        for (int i = 0; i < starBoxes.Length; i++)
        {
            if (i < starBoxes.Length)
            {
                starBoxes[i].gameObject.SetActive(true);
                starBoxes[i].sprite = (i < correctAnswers) ? starFilled : starEmpty;
            }
        }
        
        // Set final score text
        finalScoreText.text = percentage.ToString("F0") + "%";
        
        // Set cat expression if available
        if (catCornerImage != null)
        {
            catCornerImage.sprite = isVictory ? catHappy : catSad;
        }
        
        // Save to player prefs
        int previousScore = PlayerPrefs.GetInt("TotalScore", 0);
        PlayerPrefs.SetInt("TotalScore", previousScore + score);
        PlayerPrefs.SetFloat("TKNs", PlayerPrefs.GetFloat("TKNs", 0) + tkns);
        PlayerPrefs.Save();
        
        Debug.Log($"Juego terminado. Aciertos: {correctAnswers}, Errores: {incorrectAnswers}, Puntuación: {score}, TKNs: {tkns}");
    }

    private IEnumerator SaveGame(bool isVictory, float percentage)
    {
        GameStats game;
        game.idPartida = idGame;
        game.resultado = isVictory ? "victoria" : "derrota";
        game.porcentajeAciertos = percentage;
        game.puntaje = score;
        game.TKNs = tkns;
        game.idMinijuego = 2; // Assuming Trivia is ID 2
        game.idUsuario = userId;

        string jsonGameStats = JsonConvert.SerializeObject(game);
        UnityWebRequest webRequest = UnityWebRequest.Post($"{url}/trivia/saveGame", jsonGameStats, "application/json");
        webRequest.SetRequestHeader("Content-Type", "application/json");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Resultados del juego guardados correctamente.");
        }
        else
        {
            Debug.LogError($"Error guardando resultados: {webRequest.error}");
        }
    }

    // Button handlers
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("City");
    }
}