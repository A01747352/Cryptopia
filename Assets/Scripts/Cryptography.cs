using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cryptopia.Problema;
using System.Collections;
using System.IO;

using Newtonsoft.Json;



public class Cryptography : MonoBehaviour
{
    
    private TextField inputUsuario;
    private  Label displayLabel;
    private Label scoreLabel;
    private UIDocument Game;
    private int problemIndex;
    private System.Random randomIndex = new System.Random();

    // Variables for loading the problems
    private string jsonProblemas;
    private static Problema[] bancoProblemas;
    private Problema problema;

    // Variables to keep track of the game
    private int score = 0;
    private int questionsNum = 0;
    private int mistakes = 0;

    
    
    
    void OnEnable()
    {
        Game = GetComponent<UIDocument>();
        var root = Game.rootVisualElement;

        // Initializing uxml variables
        inputUsuario = root.Q<TextField>("InputUsuario");
        displayLabel = root.Q<Label>("Problem");
        scoreLabel = root.Q<Label>("Score");

        

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
        if (questionsNum == 5 && mistakes < 3) 
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

            }
            StartCoroutine(NextQuestion());
        }
    }

    

}