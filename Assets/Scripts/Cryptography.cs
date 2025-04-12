using UnityEngine;
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
    private int problemIndex;
    
    
// Variables for loading the problems
    private string jsonProblemas;
    private int problemsCount;
    private Problema problema;

// Variables game variables
    private static int score = 0;
    private int questionsNum = 1;
    private int mistakes = 0;
    private int totalQuestions = 5;

    
    static string idGame;
    
   string url = "http://localhost:8080";
    
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

        defeatScreen = root.Q<VisualElement>("DefeatScreen");
        totalScoreDSLabel = defeatScreen.Q<Label>("TotalPointsValue");
        summaryDSResult = defeatScreen.Q<Label>("FinalResultValue");
        restartDSButton = defeatScreen.Q<Button>("RestartButton");
        restartDSButton.RegisterCallback<ClickEvent>(RestartGame);
        backToMainDSButton = defeatScreen.Q<Button>("BackToMainButton");
        backToMainDSButton.RegisterCallback<ClickEvent>(BackToMain);

        inputUsuario = root.Q<TextField>("InputUsuario");
        displayLabel = root.Q<Label>("Problem");
        scoreLabel = root.Q<Label>("Score");
        qCountLabel = root.Q<Label>("QuestionCount");
        gainedPoints = root.Q<Label>("GainedPoints");
        live1 = root.Q<VisualElement>("Live1");
        live2 = root.Q<VisualElement>("Live2");
        live3 = root.Q<VisualElement>("Live3");

        // Start the game by registering a new game in the database
        StartCoroutine(RegisterNewGame());

    }

private IEnumerator RegisterNewGame()
{
    UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/cryptography/newGame");
    yield return webRequest.SendWebRequest();

    if (webRequest.result == UnityWebRequest.Result.Success)
    {
        idGame = webRequest.downloadHandler.text;
        Debug.Log($"New game registered with ID: {idGame}");

        // Load the first problem from the database
        StartCoroutine(LoadProblem());
    }
    else
    {
        Debug.LogError($"Error registering new game: {webRequest.error}");
    }
}

private IEnumerator LoadProblem()
{
    
    UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/cryptography/loadEncryption");
    yield return webRequest.SendWebRequest();

    if (webRequest.result == UnityWebRequest.Result.Success)
    {
        string response = webRequest.downloadHandler.text;
        problema = JsonConvert.DeserializeObject<Problema>(response);

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
            totalScoreDSLabel.text = score.ToString() + "pts";
            summaryDSResult.text = questionsNum.ToString() + "/" + totalQuestions.ToString();
            defeatScreen.style.display = DisplayStyle.Flex;
        }
        else if (questionsNum == (totalQuestions + 1) && mistakes < 3) 
        {
            GameObject.Find("CryptographySoundTrack").GetComponent<AudioSource>().Stop();
            totalScoreVSLabel.text = score.ToString() + "pts";
            totalTknVSLabel.text = (score * 0.003f).ToString("F2");
            summaryVSResult.text = mistakes.ToString();
            victoryScreen.style.display = DisplayStyle.Flex;
            int previousScore = PlayerPrefs.GetInt("TotalScore", 0);
            PlayerPrefs.SetInt("TotalScore", previousScore + score);
            PlayerPrefs.SetFloat("TKNs", PlayerPrefs.GetFloat("TKNs", 0) + score * 0.003f);
            PlayerPrefs.Save();
        }
        else
        {
            GameObject.Find("CryptographySoundTrack").GetComponent<AudioSource>().Stop();
            qCountLabel.text = questionsNum.ToString() + "/" + totalQuestions.ToString();
            StartCoroutine(LoadProblem());
        }
    }
    void EnterText(KeyUpEvent evt)
    {
        if (evt.keyCode == KeyCode.Return)
        {
            if (problema.ValidarRespuesta(inputUsuario.value) == true)
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
            
        }
    }

    private void BackToMain(ClickEvent evt)
    {
        SceneManager.LoadScene("City");
    }

    private void RestartGame(ClickEvent evt)
    {
        SceneManager.LoadScene("Cryptography");
    }

    

}