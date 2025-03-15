using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject[] Levels;
    public GameObject correctPanel;
    public GameObject incorrectPanel;

    private int currentLevel = 0;

    void Start()
    {
        // Asegurar que solo el primer nivel esté activo al inicio
        for (int i = 1; i < Levels.Length; i++)
        {
            Levels[i].SetActive(false);
        }
        correctPanel.SetActive(false);
        incorrectPanel.SetActive(false);
    }

    public void CorrectAnswer()
    {
        correctPanel.SetActive(true);
        Invoke("NextLevel", 1.5f); // Espera 1.5 segundos antes de pasar al siguiente nivel
    }

    public void IncorrectAnswer()
    {
        incorrectPanel.SetActive(true);
        Invoke("HideIncorrectPanel", 1.5f); // Oculta el panel después de 1.5 segundos
    }

    void NextLevel()
    {
        correctPanel.SetActive(false);
        if (currentLevel + 1 < Levels.Length)
        {
            Levels[currentLevel].SetActive(false);
            currentLevel++;
            Levels[currentLevel].SetActive(true);
        }
    }

    void HideIncorrectPanel()
    {
        incorrectPanel.SetActive(false);
    }
}