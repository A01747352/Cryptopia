using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TriviaManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] answers;
        public string[] feedbacks; // Nuevo: Feedback específico para cada respuesta
        public int correctAnswerIndex;
        public Sprite backgroundImage;
        public Sprite petImage;
    }

    public List<Question> questions = new List<Question>();

    [Header("Pregunta en pantalla")]
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public Image backgroundPanel;
    public Image petImage;

    [Header("Panel de resultado por pregunta")]
    public GameObject resultadoPanel;
    public Image boxText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI feedbackText;

    [Header("Sprites para resultado")]
    public Sprite correctSprite;
    public Sprite incorrectSprite;

    [Header("Panel de Puntuación Final")]
    public GameObject finalScorePanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Estrellas")]
    public Image[] starBoxes;
    public Sprite starFilled;
    public Sprite starEmpty;

    [Header("Imagen de Gato en la Esquina")]
    public Image catCornerImage;
    public Sprite catHappy;
    public Sprite catSad;

    private int currentQuestionIndex = 0;
    private int score = 0;

    void Start()
    {
        LoadQuestions();
        ShowQuestion();
    }

    void LoadQuestions()
    {
        // Cargar preguntas con sus respectivos feedbacks
    }

    void ShowQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            ShowFinalScore();
            return;
        }

        resultadoPanel.SetActive(false);
        Question q = questions[currentQuestionIndex];
        questionText.text = q.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.answers[i];
            answerButtons[i].onClick.RemoveAllListeners();
            int index = i;
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }

        backgroundPanel.sprite = q.backgroundImage;
        petImage.sprite = q.petImage;
    }

    void CheckAnswer(int selectedIndex)
    {
        resultadoPanel.SetActive(true);
        Question q = questions[currentQuestionIndex];
        bool isCorrect = (selectedIndex == q.correctAnswerIndex);

        if (isCorrect)
        {
            boxText.sprite = correctSprite;
            resultText.text = "Correcto";
            score++;
        }
        else
        {
            boxText.sprite = incorrectSprite;
            resultText.text = "Incorrecto";
        }

        feedbackText.text = q.feedbacks[selectedIndex]; // Mostrar feedback específico para la respuesta seleccionada

        currentQuestionIndex++;
        if (currentQuestionIndex < questions.Count)
        {
            Invoke(nameof(ShowQuestion), 8f);
        }
        else
        {
            Invoke(nameof(ShowFinalScore), 2f);
        }
    }

    void ShowFinalScore()
    {
        resultadoPanel.SetActive(false);
        finalScorePanel.SetActive(true);
        float percentage = ((float)score / (float)questions.Count) * 100f;
        finalScoreText.text = string.Format("{0}%", percentage);
        int starCount = Mathf.CeilToInt(percentage / 20f);
        starCount = Mathf.Clamp(starCount, 0, 5);

        for (int i = 0; i < starBoxes.Length; i++)
        {
            starBoxes[i].sprite = i < starCount ? starFilled : starEmpty;
        }

        catCornerImage.sprite = percentage > 79f ? catHappy : catSad;
    }
}