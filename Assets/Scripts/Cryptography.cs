using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cryptopia.Problema;
using System.Collections;
using System.IO;


using Newtonsoft.Json;
using UnityEngine.Networking;



public class Cryptography : MonoBehaviour
{
   
// UI Elements
    private UIDocument game;
    private TextField inputUsuario;
    private Label displayLabel;
    private Label qCountLabel;
    private Label scoreLabel;
    private VisualElement live1;
    private VisualElement live2;
    private VisualElement live3;
    private Label gainedPoints;
    private Button send;

    // Elementos de Victory Screen
    private Button restartVSButton;
    private Button backToMainVSButton;
    private Label totalScoreVSLabel;
    private Label totalTknVSLabel;
    private Label summaryVSResult;

    // Elementos de Defeat Screen
    private Button restartDSButton;
    private Button backToMainDSButton;
    private Label totalScoreDSLabel;
    private Label summaryDSResult;
    private VisualElement victoryScreen;
    private VisualElement defeatScreen;

// Sound Variables
    

// Utility Variabels
    public struct SubmittedAnswer
    {
        public string respuestaUsuario;
        public int idPartida;
        public int idPalabra;
    }

    public struct GameStats
    {
        public int idPartida;
        public int aciertos;
        public int errores;
        public int puntaje;
        public float TKNs;
        public bool resultado;
        public int idMinijuego;
        public int idUsuario;
    }
    

    
// Variables for loading the problems
    Problema[] problemaWrapper;
    private Problema problema;

// Variables game variables
    private int score = 0;
    private int questionsNum = 1;
    private int mistakes = 0;
    private int totalQuestions = 5;
    private float tkns;

    static int userId;
    static int idGame;
    
    private string url = Variables.Variables.url;
    
    void OnEnable()
    {
        game = GetComponent<UIDocument>();
        var root = game.rootVisualElement;

        // Initialize UI elements
        victoryScreen = root.Q<VisualElement>("VictoryScreen");
        totalScoreVSLabel = victoryScreen.Q<Label>("TotalPointsValue");
        totalTknVSLabel = victoryScreen.Q<Label>("TKNsValue");
        summaryVSResult = victoryScreen.Q<Label>("FinalResultValue");
        restartVSButton = victoryScreen.Q<Button>("RestartButton");
        restartVSButton.RegisterCallback<ClickEvent>(RestartGame);
        backToMainVSButton = victoryScreen.Q<Button>("BackToMainButton");
        backToMainVSButton.RegisterCallback<ClickEvent>(BackToMain);
        userId = PlayerPrefs.GetInt("UserId", 1);
        defeatScreen = root.Q<VisualElement>("DefeatScreen");
        totalScoreDSLabel = defeatScreen.Q<Label>("TotalPointsValue");
        summaryDSResult = defeatScreen.Q<Label>("FinalResultValue");
        restartDSButton = defeatScreen.Q<Button>("RestartButton");
        restartDSButton.RegisterCallback<ClickEvent>(RestartGame);
        backToMainDSButton = defeatScreen.Q<Button>("BackToMainButton");
        backToMainDSButton.RegisterCallback<ClickEvent>(BackToMain);

        inputUsuario = root.Q<TextField>("InputUsuario");
        inputUsuario.RegisterCallback<KeyUpEvent>(EnterText);
        send = root.Q<Button>("Send");
        send.RegisterCallback<ClickEvent>(EnterText);
        displayLabel = root.Q<Label>("Problem");
        scoreLabel = root.Q<Label>("Score");
        qCountLabel = root.Q<Label>("QuestionCount");
        gainedPoints = root.Q<Label>("GainedPoints");
        live1 = root.Q<VisualElement>("Live1");
        live2 = root.Q<VisualElement>("Live2");
        live3 = root.Q<VisualElement>("Live3");

        
        StartCoroutine(RegisterNewGame());

    }

private IEnumerator RegisterNewGame()
{
    UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/cryptography/newGame/{userId}");
    yield return webRequest.SendWebRequest();

    if (webRequest.result == UnityWebRequest.Result.Success)
    {
        string idGameJSONstring = webRequest.downloadHandler.text;
        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(idGameJSONstring);

        // Access the value by key
        if (data.ContainsKey("idGame"))
        {
            idGame = System.Convert.ToInt32(data["idGame"]);
            print($"idGame: {idGame}");
        }

        
        print($"New game registered with ID: {idGame}");

        StartCoroutine(LoadProblem());
    }
    else
    {
        Debug.LogError($"Error registering new game: {webRequest.error}");
    }
}

private IEnumerator SaveGame(bool result)
{
    GameStats game;
    game.idPartida = idGame;
    game.aciertos = questionsNum;
    game.errores = mistakes;
    game.puntaje = score;
    game.TKNs = tkns;
    game.resultado = result;
    game.idMinijuego = 1;
    game.idUsuario = userId;

    string JsonGameStats = JsonConvert.SerializeObject(game);
    UnityWebRequest webRequest = UnityWebRequest.Post($"{url}/cryptography/saveGame", JsonGameStats, "application/json");
    yield return webRequest.SendWebRequest();

    if (webRequest.result == UnityWebRequest.Result.Success)
    {
        print("Game saved successfully.");
    }
    else
    {
        Debug.LogError($"Error saving game: {webRequest.error}");
    }
}
private IEnumerator LoadProblem()
{
    
    UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/cryptography/loadEncryption");
    yield return webRequest.SendWebRequest();

    if (webRequest.result == UnityWebRequest.Result.Success)
    {
        string response = webRequest.downloadHandler.text;
        print(response);
        problemaWrapper = JsonConvert.DeserializeObject<Problema[]>(response); 
        problema = problemaWrapper[0];

        // Update the UI with the loaded problem
        displayLabel.text = problema.palabra;
        scoreLabel.text = score.ToString() + "pts";
        qCountLabel.text = questionsNum.ToString() + "/" + totalQuestions.ToString();
    }
    else
    {
        Debug.LogError($"Error loading problem: {webRequest.error}");
    }
}

    private IEnumerator NextQuestion(bool isCorrect)
    {
        yield return new WaitForSeconds(1f);

        if (isCorrect)
        {
            questionsNum += 1;
            
            print(totalQuestions);
        }
        print(questionsNum);
        gainedPoints.style.display = DisplayStyle.None;
        if (mistakes == 3)
        {
            gameObject.GetComponent<AudioSource>().Stop();
            GameObject.Find("DefeatSound").GetComponent<AudioSource>().Play();
            totalScoreDSLabel.text = score.ToString() + "pts";
            summaryDSResult.text = questionsNum.ToString() + "/" + totalQuestions.ToString();
            defeatScreen.style.display = DisplayStyle.Flex;
            score = 0;
            tkns = 0;
            StartCoroutine(SaveGame(false));
            UiDocumentm.Instance.IncrementGamesPlayedAfterGame();
            
        }
        else if (questionsNum == (totalQuestions + 1) && mistakes < 3) 
        {
            gameObject.GetComponent<AudioSource>().Stop();
            GameObject.Find("VictorySound").GetComponent<AudioSource>().Play();
            tkns = score * 0.003f;
            StartCoroutine(SaveGame(true));
            totalScoreVSLabel.text = score.ToString() + "pts";
            string strTKNs = tkns.ToString("F2");
            totalTknVSLabel.text = strTKNs;
            summaryVSResult.text = mistakes.ToString();
            victoryScreen.style.display = DisplayStyle.Flex;
            int previousScore = PlayerPrefs.GetInt("TotalScore", 0);
            PlayerPrefs.SetInt("TotalScore", previousScore + score);
            PlayerPrefs.SetFloat("TKNs", PlayerPrefs.GetFloat("TKNs", 0) + tkns);
            PlayerPrefs.Save();
            UiDocumentm.Instance.IncrementGamesPlayedAfterGame();
        }
        else
        {

            qCountLabel.text = questionsNum.ToString() + "/" + totalQuestions.ToString();
            StartCoroutine(LoadProblem());
        }
    }
    void EnterText(KeyUpEvent evt)
    {
        
        if (evt.keyCode == KeyCode.Return)
        {
            StartCoroutine(SendAnswerUser(inputUsuario.value));
            if (problema.ValidarRespuesta(inputUsuario.value))
            {
                GameObject.Find("SoundCorrect").GetComponent<AudioSource>().Play();
                displayLabel.text = "Correct!";
                gainedPoints.text = "+" + problema.puntos.ToString() + "pts";
                gainedPoints.style.display = DisplayStyle.Flex;
                score += problema.puntos; 
                scoreLabel.text = score.ToString() + "pts";
                StartCoroutine(NextQuestion(true));
            }
            else 
            {
                GameObject.Find("SoundIncorrect").GetComponent<AudioSource>().Play();
                displayLabel.text = "Incorrect :(";
                ++mistakes;
                if (mistakes == 1)
                {
                    live1.style.visibility = Visibility.Hidden;
                } 
                else if (mistakes == 2)
                {
                    live2.style.visibility = Visibility.Hidden;
                }
                else 
                {
                    live3.style.visibility = Visibility.Hidden;
                }
                StartCoroutine(NextQuestion(false));

            }

            inputUsuario.value = "";
            
        }
    }
    void EnterText(ClickEvent evt)
    {
        StartCoroutine(SendAnswerUser(inputUsuario.value));
        if (problema.ValidarRespuesta(inputUsuario.value))
        {
            GameObject.Find("SoundCorrect").GetComponent<AudioSource>().Play();
            displayLabel.text = "Correct!";
            gainedPoints.text = "+" + problema.puntos.ToString() + "pts";
            gainedPoints.style.display = DisplayStyle.Flex;
            score += problema.puntos; 
            scoreLabel.text = score.ToString() + "pts";
            StartCoroutine(NextQuestion(true));
        }
        else 
        {
            GameObject.Find("SoundIncorrect").GetComponent<AudioSource>().Play();
            displayLabel.text = "Incorrect :(";
            ++mistakes;
            if (mistakes == 1)
            {
                live1.style.visibility = Visibility.Hidden;
            } 
            else if (mistakes == 2)
            {
                live2.style.visibility = Visibility.Hidden;
            }
            else 
            {
                live3.style.visibility = Visibility.Hidden;
            }
            StartCoroutine(NextQuestion(false));

        }

        inputUsuario.value = "";
        
    }

    private void BackToMain(ClickEvent evt)
    {
        SceneManager.LoadScene("City");
    }

    private void RestartGame(ClickEvent evt)
    {
        SceneManager.LoadScene("Cryptography");
    }

    IEnumerator SendAnswerUser(string respuestaUsuario)
    {
        SubmittedAnswer submittedAnswer;
        submittedAnswer.respuestaUsuario = respuestaUsuario;
        submittedAnswer.idPartida = idGame;
        submittedAnswer.idPalabra = problema.idPalabra;

        string jsonData = JsonConvert.SerializeObject(submittedAnswer);
        UnityWebRequest webRequest = UnityWebRequest.Post($"{url}/cryptography/submitAnswer", jsonData, "application/json");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            print("Answer submitted successfully.");
        }
        else
        {
            Debug.LogError($"Error submitting answer: {webRequest.error}");
        }
    }

}