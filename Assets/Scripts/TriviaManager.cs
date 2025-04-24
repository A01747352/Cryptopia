using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class TriviaManager : MonoBehaviour
{
    // Estructuras de datos para preguntas remotas
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

    // (Opcional) Preguntas locales de respaldo
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
    public QuestionData[] localQuestions;

    // Variables UI Toolkit
    private UIDocument uiDocument;
    private VisualElement bg;               // Contenedor de preguntas/respuestas
    private VisualElement box;              // Contenedor que siempre está activo
    private VisualElement resultPanel;      // Panel de retroalimentación
    private VisualElement finalScorePanel;  // Panel de puntaje final

    private Label questionText;
    private Label resultText;
    private Label feedbackText;
    private Label finalScoreText;

    private Button[] answerButtons = new Button[4];
    private Button restartButton;
    private Button backButton;
    
    // (Opcional) Imágenes de fondo, mascota y expresión (cat)
    private Image backgroundImageUI;
    private Image petImageUI;
    private Image catCornerImageUI;

    // Variables del juego
    private int currentQuestionIndex = 0;
    private int score = 0;
    private int correctAnswers = 0;
    private int incorrectAnswers = 0;
    private float tkns = 0f;
    private int totalQuestions = 5;
    [SerializeField] private int userId = 1;  // Changed from static to serialized field
    private int idGame;  // Changed from static to instance variable
    [SerializeField] private string url = "http://98.83.42.146:8080";  // Made configurable in Inspector

    private List<QuestionWithAnswers> loadedQuestions = new List<QuestionWithAnswers>();
    private QuestionWithAnswers currentQuestion;
    private bool usingLocalQuestions = false;

    // Añadimos este método para intentar actualizar el texto con un pequeño retraso
    private IEnumerator UpdateQuestionTextWithDelay(string text)
    {
        yield return new WaitForSeconds(0.1f);
        if (questionText != null) {
            questionText.text = text;
            Debug.Log("Texto de pregunta actualizado con retraso");
        }
    }

    void OnEnable()
    {
        // Obtener el UIDocument y el root
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component not found!");
            return;
        }
        
        var root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("Root visual element not found!");
            return;
        }

        // Obtener referencias de los elementos UI con comprobación de nombres alternativos
        bg = root.Q<VisualElement>("BG");
        if (bg == null) {
            bg = root.Q<VisualElement>("bg"); // Intentar con minúsculas
            if (bg == null)
                Debug.LogError("Elemento BG no encontrado en el UXML");
        }
        
        box = root.Q<VisualElement>("Box");
        if (box == null) {
            box = root.Q<VisualElement>("box"); // Intentar con minúsculas
            if (box == null)
                Debug.LogError("Elemento Box no encontrado en el UXML");
        }
        
        // Intentar obtener questionText con diferentes variantes de nombre
        questionText = root.Q<Label>("QuestionText");
        if (questionText == null) {
            questionText = root.Q<Label>("questionText"); // Intentar con camelCase
            if (questionText == null) {
                questionText = root.Q<Label>("Question-Text"); // Intentar con guion
                if (questionText == null) {
                    questionText = root.Q<Label>("Pregunta"); // Intentar en español
                    if (questionText == null) {
                        // Último intento: buscar cualquier Label dentro de un contenedor
                        var containers = new[] { bg, box, root };
                        foreach (var container in containers) {
                            if (container != null) {
                                var labels = container.Query<Label>().ToList();
                                if (labels.Count > 0) {
                                    Debug.Log($"Encontrado Label alternativo: {labels[0].name}");
                                    questionText = labels[0];
                                    break;
                                }
                            }
                        }
                        
                        if (questionText == null)
                            Debug.LogError("No se pudo encontrar ningún elemento Label para la pregunta");
                    }
                }
            }
        }

        resultPanel = root.Q<VisualElement>("ResultPanel");
        finalScorePanel = root.Q<VisualElement>("FinalScorePanel");

        resultText = root.Q<Label>("ResultText");
        feedbackText = root.Q<Label>("FeedbackText");
        finalScoreText = root.Q<Label>("FinalScoreText");

        restartButton = root.Q<Button>("RestartButton");
        backButton = root.Q<Button>("BackButton");

        // (Opcional) Obtener imágenes si las usas
        backgroundImageUI = root.Q<Image>("BackgroundPanel");
        petImageUI = root.Q<Image>("PetImage");
        catCornerImageUI = root.Q<Image>("CatCornerImage");

        // Configurar los botones de respuesta
        for (int i = 0; i < 4; i++)
        {
            answerButtons[i] = root.Q<Button>($"Answer{i + 1}");
            if (answerButtons[i] == null)
            {
                Debug.LogError($"Button Answer{i + 1} not found!");
                continue;
            }
            
            int index = i;
            answerButtons[i].clicked += () => CheckAnswer(index);
        }

        // Configurar botones de la pantalla final
        if (restartButton != null)
            restartButton.clicked += RestartGame;
        else
            Debug.LogError("RestartButton not found!");
            
        if (backButton != null)
            backButton.clicked += BackToMain;
        else
            Debug.LogError("BackButton not found!");

        // Inicializar el estado visual
        if (bg != null)
            bg.style.display = DisplayStyle.Flex;              // Se ve BG al inicio
        if (resultPanel != null)
            resultPanel.style.display = DisplayStyle.None;     // Oculto al inicio
        if (finalScorePanel != null)
            finalScorePanel.style.display = DisplayStyle.None; // Oculto al inicio

        // Iniciar el juego registrando una nueva partida
        StartCoroutine(RegisterNewGame());
    }

    private IEnumerator RegisterNewGame()
    {
        // Validar la URL del servidor
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("Server URL is not set!");
            LoadLocalQuestion();
            yield break;
        }

        UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/trivia/newGame/{userId}");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string idGameJSONstring = webRequest.downloadHandler.text;
            try
            {
                Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(idGameJSONstring);

                if (data != null && data.ContainsKey("idGame"))
                {
                    idGame = System.Convert.ToInt32(data["idGame"]);
                    Debug.Log($"Juego de trivia registrado con ID: {idGame}");
                    StartCoroutine(LoadAllQuestions());
                }
                else
                {
                    Debug.LogWarning("La respuesta del servidor no contiene idGame. Usando preguntas locales.");
                    LoadLocalQuestions();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error al parsear respuesta del servidor: {e.Message}");
                LoadLocalQuestions();
            }
        }
        else
        {
            Debug.LogWarning($"Error registrando nuevo juego: {webRequest.error}. Usando preguntas locales.");
            LoadLocalQuestions();
        }
    }

    private void LoadLocalQuestions()
    {
        usingLocalQuestions = true;
        // Ajustar totalQuestions basado en las preguntas locales disponibles
        if (localQuestions != null && localQuestions.Length > 0)
        {
            totalQuestions = Mathf.Min(totalQuestions, localQuestions.Length);
            LoadLocalQuestion();
        }
        else
        {
            Debug.LogError("No hay preguntas locales disponibles.");
            EndGame();
        }
    }

    private IEnumerator LoadAllQuestions()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/trivia/getAllQuestions");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string response = webRequest.downloadHandler.text;
            try
            {
                loadedQuestions = JsonConvert.DeserializeObject<List<QuestionWithAnswers>>(response);
                Debug.Log($"Cargadas {loadedQuestions.Count} preguntas de la base de datos");

                if (loadedQuestions != null && loadedQuestions.Count > 0)
                {
                    // Mezclar preguntas de forma aleatoria
                    ShuffleQuestions();
                    // Limitar al número deseado
                    if (loadedQuestions.Count > totalQuestions)
                    {
                        loadedQuestions = loadedQuestions.GetRange(0, totalQuestions);
                    }
                    LoadNextDBQuestion();
                }
                else
                {
                    Debug.LogWarning("No se encontraron preguntas en la base de datos. Usando preguntas locales.");
                    LoadLocalQuestions();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error al parsear las preguntas: {e.Message}. Usando preguntas locales.");
                LoadLocalQuestions();
            }
        }
        else
        {
            Debug.LogWarning($"Error cargando preguntas: {webRequest.error}. Usando preguntas locales.");
            LoadLocalQuestions();
        }
    }

    private void ShuffleQuestions()
    {
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

    // Cargar pregunta desde la base de datos
    private void LoadNextDBQuestion()
    {
        if (loadedQuestions == null || currentQuestionIndex >= loadedQuestions.Count)
        {
            EndGame();
            return;
        }

        currentQuestion = loadedQuestions[currentQuestionIndex];
        
        // Depuración: Imprimir la pregunta para verificar su contenido
        Debug.Log($"Pregunta a mostrar: '{currentQuestion.question.pregunta}'");
        
        if (questionText != null) {
            questionText.text = currentQuestion.question.pregunta;
            
            // Forzar la visualización del texto
            questionText.style.display = DisplayStyle.Flex;
            questionText.style.visibility = StyleKeyword.Initial;
            questionText.style.opacity = 1;
            
            // Si el texto sigue sin verse, intentar actualizarlo con un ligero retraso
            StartCoroutine(UpdateQuestionTextWithDelay(currentQuestion.question.pregunta));
        }
        else {
            Debug.LogError("El elemento questionText es NULL. Verifica el nombre en el UXML");
        }

        // Configurar botones de respuesta
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null) continue;
            
            if (i < currentQuestion.answers.Count)
            {
                answerButtons[i].text = currentQuestion.answers[i].respuesta;
                answerButtons[i].SetEnabled(true);
            }
            else
            {
                answerButtons[i].SetEnabled(false);
            }
        }

        // (Opcional) Si deseas actualizar imágenes desde preguntas locales de respaldo
        if (localQuestions != null && currentQuestionIndex < localQuestions.Length)
        {
            var localQ = localQuestions[currentQuestionIndex];
            if (backgroundImageUI != null && localQ.backgroundImage != null)
                backgroundImageUI.sprite = localQ.backgroundImage;
            if (petImageUI != null && localQ.petImage != null)
                petImageUI.sprite = localQ.petImage;
        }
    }

    // Fallback para preguntas locales si algo falla con la BD
    private void LoadLocalQuestion()
    {
        if (localQuestions == null || currentQuestionIndex >= localQuestions.Length)
        {
            EndGame();
            return;
        }

        QuestionData question = localQuestions[currentQuestionIndex];
        
        // Depuración: Imprimir la pregunta para verificar su contenido
        Debug.Log($"Pregunta local a mostrar: '{question.questionText}'");
        
        if (questionText != null) {
            questionText.text = question.questionText;
            
            // Forzar la visualización del texto
            questionText.style.display = DisplayStyle.Flex;
            questionText.style.visibility = StyleKeyword.Initial;
            questionText.style.opacity = 1;
            
            // Si el texto sigue sin verse, intentar actualizarlo con un ligero retraso
            StartCoroutine(UpdateQuestionTextWithDelay(question.questionText));
        }
        else {
            Debug.LogError("El elemento questionText es NULL. Verifica el nombre en el UXML");
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null) continue;
            
            if (question.answers != null && i < question.answers.Length)
            {
                answerButtons[i].text = question.answers[i];
                answerButtons[i].SetEnabled(true);
            }
            else
            {
                answerButtons[i].SetEnabled(false);
            }
        }

        if (backgroundImageUI != null && question.backgroundImage != null)
            backgroundImageUI.sprite = question.backgroundImage;
        if (petImageUI != null && question.petImage != null)
            petImageUI.sprite = question.petImage;
    }

    // Lógica de respuesta al hacer clic en los botones
    void CheckAnswer(int answerIndex)
    {
        foreach (var btn in answerButtons)
        {
            if (btn != null)
                btn.SetEnabled(false);
        }

        // 1. Ocultar BG (todo lo de preguntas/respuestas)
        if (bg != null)
            bg.style.display = DisplayStyle.None;

        // 2. Mostrar panel de retroalimentación
        if (resultPanel != null)
            resultPanel.style.display = DisplayStyle.Flex;

        bool isCorrect = false;
        string feedback = "";
        int points = 100; // Puntos por defecto

        if (!usingLocalQuestions && currentQuestion != null && currentQuestion.answers != null && answerIndex < currentQuestion.answers.Count)
        {
            // Usamos pregunta de la BD
            isCorrect = currentQuestion.answers[answerIndex].resultado;
            feedback = currentQuestion.answers[answerIndex].retroalimentacion;
            points = currentQuestion.question.puntos;
            
            // Solo enviar la respuesta si tenemos un ID de partida válido
            if (idGame > 0)
                StartCoroutine(SendAnswerUser(answerIndex, isCorrect));
        }
        else if (localQuestions != null && currentQuestionIndex < localQuestions.Length)
        {
            // Usamos preguntas locales
            QuestionData localQ = localQuestions[currentQuestionIndex];
            isCorrect = (answerIndex == localQ.correctAnswerIndex);
            
            // Verificar que el índice sea válido para los feedbacks
            if (localQ.feedbacks != null && answerIndex < localQ.feedbacks.Length)
                feedback = localQ.feedbacks[answerIndex];
            else
                feedback = isCorrect ? "¡Respuesta correcta!" : "Respuesta incorrecta.";
        }

        if (isCorrect)
        {
            correctAnswers++;
            score += points;
            if (resultText != null)
                resultText.text = "¡Correcto!";
            // (Opcional) Cambiar imagen de boxText o reproducir sonido
            AudioSource correctSound = GameObject.Find("SoundCorrect")?.GetComponent<AudioSource>();
            if (correctSound != null) correctSound.Play();
        }
        else
        {
            incorrectAnswers++;
            if (resultText != null)
                resultText.text = "Incorrecto :(";
            AudioSource incorrectSound = GameObject.Find("SoundIncorrect")?.GetComponent<AudioSource>();
            if (incorrectSound != null) incorrectSound.Play();
        }

        if (feedbackText != null)
            feedbackText.text = feedback;
        StartCoroutine(ShowFeedbackAndNext());
    }

    private IEnumerator SendAnswerUser(int answerIndex, bool esCorrecta)
    {
        if (currentQuestion == null || currentQuestion.answers == null || 
            answerIndex >= currentQuestion.answers.Count || 
            currentQuestion.question == null)
        {
            Debug.LogError("No se puede enviar la respuesta: datos incompletos");
            yield break;
        }
        
        var submittedAnswer = new
        {
            respuestaUsuario = currentQuestion.answers[answerIndex].respuesta,
            idPregunta = currentQuestion.question.idPregunta,
            esCorrecta = esCorrecta,
            idPartida = idGame
        };

        string jsonData = JsonConvert.SerializeObject(submittedAnswer);
        
        using (UnityWebRequest webRequest = new UnityWebRequest($"{url}/trivia/submitAnswer", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
                Debug.Log("Respuesta enviada correctamente a la BD.");
            else
                Debug.LogError($"Error enviando respuesta: {webRequest.error}");
        }
    }

    IEnumerator ShowFeedbackAndNext()
    {
        yield return new WaitForSeconds(3f);

        // Ocultar panel de retroalimentación
        if (resultPanel != null)
            resultPanel.style.display = DisplayStyle.None;

        // Actualizar índice y checar si termina o hay más preguntas
        currentQuestionIndex++;
        if (currentQuestionIndex < totalQuestions)
        {
            // Todavía hay preguntas -> Mostrar BG de nuevo
            if (bg != null)
                bg.style.display = DisplayStyle.Flex;
            
            // Cargar la siguiente pregunta
            if (!usingLocalQuestions && loadedQuestions != null && loadedQuestions.Count > 0)
                LoadNextDBQuestion();
            else
                LoadLocalQuestion();
        }
        else
        {
            // Juego terminado
            EndGame();
        }
    }

    // Al finalizar el juego, mostrar el panel final con el puntaje
    void EndGame()
    {
        // Calcular porcentaje de aciertos y tokens
        int totalAnswered = correctAnswers + incorrectAnswers;
        if (totalAnswered == 0) totalAnswered = 1;
        float percentage = (float)correctAnswers / totalAnswered * 100;
        bool isVictory = correctAnswers > (totalAnswered / 2);
        
        // Calcular tokens y asegurar un mínimo valor para evitar problemas de truncamiento
        tkns = score * 0.003f;
        tkns = Mathf.Max(tkns, 0.001f); // Asegurar un mínimo de 0.001 tokens
        
        // Llamar a SaveGame si el juego está registrado y no estamos usando preguntas locales
        if (!usingLocalQuestions && idGame > 0)
            StartCoroutine(SaveGame(isVictory, percentage));

        // 1. Ocultar BG y ResultPanel (por si acaso)
        if (bg != null)
            bg.style.display = DisplayStyle.None;
        if (resultPanel != null)
            resultPanel.style.display = DisplayStyle.None;

        // 2. Mostrar panel final con el puntaje
        if (finalScoreText != null)
            finalScoreText.text = $"¡Juego terminado! Tu puntaje final es: {score} ({percentage.ToString("F0")}%)";
        if (finalScorePanel != null)
            finalScorePanel.style.display = DisplayStyle.Flex;

        // (Opcional) Actualizar elementos visuales, por ejemplo, imagen de gato
        if (catCornerImageUI != null)
        {
            Sprite happyCat = Resources.Load<Sprite>("catHappy");
            Sprite sadCat = Resources.Load<Sprite>("catSad");
            
            catCornerImageUI.sprite = isVictory ? 
                (happyCat != null ? happyCat : catCornerImageUI.sprite) : 
                (sadCat != null ? sadCat : catCornerImageUI.sprite);
        }

        // Guardar en PlayerPrefs (acumulativo)
        int prevScore = PlayerPrefs.GetInt("TotalScore", 0);
        PlayerPrefs.SetInt("TotalScore", prevScore + score);
        float prevTKNs = PlayerPrefs.GetFloat("TKNs", 0);
        PlayerPrefs.SetFloat("TKNs", prevTKNs + tkns);
        PlayerPrefs.Save();

        Debug.Log($"Juego terminado. Aciertos: {correctAnswers}, Errores: {incorrectAnswers}, Puntaje: {score}, TKNs: {tkns}");
    }

    private IEnumerator SaveGame(bool isVictory, float percentage)
    {
        var gameStats = new
        {
            idPartida = idGame,
            resultado = isVictory ? "victoria" : "derrota",
            porcentajeAciertos = percentage,
            puntaje = score,
            TKNs = tkns,
            idMinijuego = 2, // Trivia es ID 2
            idUsuario = userId
        };

        string jsonGameStats = JsonConvert.SerializeObject(gameStats);
        
        using (UnityWebRequest webRequest = new UnityWebRequest($"{url}/trivia/saveGame", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonGameStats);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
                Debug.Log("Resultados del juego guardados correctamente.");
            else
                Debug.LogError($"Error guardando resultados: {webRequest.error}");
        }
    }

    // Función para reiniciar el juego
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Volver a la pantalla principal
    void BackToMain()
    {
        SceneManager.LoadScene("City");
    }
    
    // Método para inspeccionar y depurar elementos UI
    // Se puede llamar desde Update() si se añade un flag para inspección
    private void InspectUIElements()
    {
        if (uiDocument == null || uiDocument.rootVisualElement == null)
            return;
            
        var root = uiDocument.rootVisualElement;
        
        // Imprimir todos los elementos visuales para ayudar a identificar el correcto
        Debug.Log("=== Elementos UI disponibles ===");
        DumpVisualElement(root, 0);
        
        // Comprobar directamente el elemento questionText
        if (questionText != null)
        {
            Debug.Log($"QuestionText encontrado: {questionText.name}");
            Debug.Log($"Texto actual: '{questionText.text}'");
            Debug.Log($"Visible: {questionText.visible}, Display: {questionText.style.display.value}");
            Debug.Log($"Width: {questionText.layout.width}, Height: {questionText.layout.height}");
            Debug.Log($"Posición: X={questionText.layout.x}, Y={questionText.layout.y}");
        }
        else
        {
            Debug.Log("QuestionText es null");
        }
    }
    
    // Método recursivo para listar todos los elementos UI con sus nombres y tipos
    private void DumpVisualElement(VisualElement element, int depth)
    {
        if (element == null) return;
        
        string indent = new string(' ', depth * 2);
        string nameInfo = string.IsNullOrEmpty(element.name) ? "[Sin nombre]" : element.name;
        Debug.Log($"{indent}- {nameInfo} ({element.GetType().Name})");
        
        // Si es un Label, mostrar su texto
        if (element is Label label) {
            Debug.Log($"{indent}  Texto: '{label.text}'");
        }
        
        // Listar hijos
        foreach (var child in element.Children()) {
            DumpVisualElement(child, depth + 1);
        }
    }
    
    // Opcionalmente, agregar este código en Update() para inspeccionar mediante tecla
    /*
    void Update()
    {
        // Pulsar F1 para inspeccionar elementos UI (solo para depuración)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            InspectUIElements();
        }
    }
    */
}