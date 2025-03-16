using UnityEngine;
using UnityEngine.UI;
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
    private string jsonProblemas;
    //private ProblemaWrapper problemaWrapper;
    private static Problema[] bancoProblemas;
    private Problema problema;
    private static int score = 0;

    
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
        problemIndex = randomIndex.Next(0, bancoProblemas.Length);
        problema = bancoProblemas[problemIndex];
        displayLabel.text = problema.incognita;
    }
    void EnterText(KeyUpEvent evt)
    {
        if (evt.keyCode == KeyCode.Return)
        {
            if (problema.ValidarRespuesta(inputUsuario.value) == true)
            {
                displayLabel.text = "Correcto!";
                score += problema.puntaje; 
                scoreLabel.text = score.ToString() + "pts";
            }
            else 
            {
                displayLabel.text = "Incorrecto :(";
                

            }
            StartCoroutine(NextQuestion());
        }
    }

    

}