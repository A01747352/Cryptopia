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
    private Label scoreLabel;
    private Label qCountLabel;
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
    private int totalQuestions = 10;

    
    
    
    void OnEnable()
    {
        Game = GetComponent<UIDocument>();
        var root = Game.rootVisualElement;

        // Initializing uxml variables
        inputUsuario = root.Q<TextField>("InputUsuario");
        displayLabel = root.Q<Label>("Problem");
        scoreLabel = root.Q<Label>("Score");
        live1 = root.Q<VisualElement>("Live1");
        live2 = root.Q<VisualElement>("Live2");
        live3 = root.Q<VisualElement>("Live3");
        qCountLabel = root.Q<Label>("QuestionCount");
        

        // Loading the Problem from JSON file
        jsonProblemas = File.ReadAllText(Application.streamingAssetsPath + "/Cryptography.json");
        bancoProblemas = JsonConvert.DeserializeObject<Problema[]>(jsonProblemas);

        NextQuestion();
        scoreLabel.text = score.ToString() + "pts";
        
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
        if (questionsNum == totalQuestions && mistakes < 3) 
        {
            SceneManager.LoadScene("Victoria");
        }
        else if (mistakes == 3)
        {
            SceneManager.LoadScene("Derrota");
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
                score += problema.puntaje; 
                scoreLabel.text = score.ToString() + "pts";
            }
            else 
            {
                displayLabel.text = "Incorrect :(";
                mistakes += 1;
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

    

}