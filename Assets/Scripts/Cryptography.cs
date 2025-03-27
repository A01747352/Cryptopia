using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cryptopia.Problema;
using System.Collections;
using System.IO;

using Newtonsoft.Json;



public class Cryptography : MonoBehaviour
{
    
    // UI Elements
    private UIDocument Game;
    private TextField inputUsuario;
    private Label displayLabel;
    private Label qCountLabel;
    private Label scoreLabel;
    
    private Label gainedPoints;

    private Button restartVSButton;
    private Button backToMainVSButton;
    private Label totalScoreVSLabel;
    private Button restartDSButton;
    private Button backToMainDSButton;
    private Label totalScoreDSLabel;
    private VisualElement victoryScreen;
    private VisualElement defeatScreen;
    private VisualElement live1;
    private VisualElement live2;
    private VisualElement live3;

    // Utility Variabels
    private int problemIndex;
    private System.Random randomIndex = new System.Random();
    

    // Variables for loading the problems
    private string jsonProblemas;
    private static Problema[] bancoProblemas;
    private Problema problema;

    // Variables to keep track of the game
    private int score = 0;
    private int questionsNum = 1;
    private int mistakes = 0;
    private int totalQuestions = 5;

    
    
    
    void OnEnable()
    {
        Game = GetComponent<UIDocument>();
        var root = Game.rootVisualElement;

        // Initializing uxml variables

        // Victory Screen UI Elements
        victoryScreen = root.Q<VisualElement>("VictoryScreen");
        totalScoreVSLabel = root.Q<Label>("TotalPointsLabel");
        restartVSButton = victoryScreen.Q<Button>("RestartButton");
        restartVSButton.RegisterCallback<ClickEvent>(RestartGame);
        backToMainVSButton = victoryScreen.Q<Button>("BackToMainButton");
        backToMainVSButton.RegisterCallback<ClickEvent>(BackToMain);

        // Defeat Screen UI Elements
        defeatScreen = root.Q<VisualElement>("DefeatScreen");
        totalScoreDSLabel = defeatScreen.Q<Label>("TotalPointsLabel");
        restartDSButton = defeatScreen.Q<Button>("RestartButton");
        restartDSButton.RegisterCallback<ClickEvent>(RestartGame);
        backToMainDSButton = defeatScreen.Q<Button>("BackToMainButton");
        backToMainDSButton.RegisterCallback<ClickEvent>(BackToMain);


        // Main Game UI Elements
        inputUsuario = root.Q<TextField>("InputUsuario");
        displayLabel = root.Q<Label>("Problem");
        scoreLabel = root.Q<Label>("Score");
        qCountLabel = root.Q<Label>("QuestionCount");
        gainedPoints = root.Q<Label>("GainedPoints");
        live1 = root.Q<VisualElement>("Live1");
        live2 = root.Q<VisualElement>("Live2");
        live3 = root.Q<VisualElement>("Live3");
        
        

        // Loading the Problem from JSON file
        jsonProblemas = File.ReadAllText(Application.streamingAssetsPath + "/Cryptography.json");
        bancoProblemas = JsonConvert.DeserializeObject<Problema[]>(jsonProblemas);

        NextQuestion();
        scoreLabel.text = score.ToString() + "pts";
        qCountLabel.text = questionsNum.ToString() + "/" + totalQuestions.ToString();
        
        inputUsuario.RegisterCallback<KeyUpEvent>(EnterText);
        problemIndex = randomIndex.Next(0, bancoProblemas.Length);
        problema = bancoProblemas[problemIndex];
        displayLabel.text = problema.incognita;
        
    }

    private IEnumerator NextQuestion()
    {
        yield return new WaitForSeconds(1f);

        questionsNum += 1;
        qCountLabel.text = questionsNum.ToString() + "/" + totalQuestions.ToString();
        gainedPoints.style.display = DisplayStyle.None;
        if (mistakes == 3)
        {
            totalScoreDSLabel.text = "Lost Score: -" + score.ToString() + "pts";
            defeatScreen.style.display = DisplayStyle.Flex;
        }
        else if (questionsNum == (totalQuestions + 1) && mistakes < 3) 
        {
            totalScoreVSLabel.text = "Final Score: " + score.ToString() + "pts";
            victoryScreen.style.display = DisplayStyle.Flex;
        }
        else
        {
            problemIndex = randomIndex.Next(0, bancoProblemas.Length);
            problema = bancoProblemas[problemIndex];
            displayLabel.text = problema.incognita;
        }
    }
    void EnterText(KeyUpEvent evt)
    {
        if (evt.keyCode == KeyCode.Return)
        {
            if (problema.ValidarRespuesta(inputUsuario.value) == true)
            {
                displayLabel.text = "Correct!";
                gainedPoints.text = "+" + problema.puntaje.ToString() + "pts";
                gainedPoints.style.display = DisplayStyle.Flex;
                score += problema.puntaje; 
                scoreLabel.text = score.ToString() + "pts";
            }
            else 
            {
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

            }
            StartCoroutine(NextQuestion());
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