using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[System.Serializable]
public class TextEntry {
    public string id;
    public string message;
    public string reward;
}

[System.Serializable]
public class TextDatabase {
    public TextEntry[] texts;
}

public class TextRewardManager : MonoBehaviour {
    [Header("JSON")]
    // Nombre del archivo en Resources (sin extensión)
    public string jsonFileName = "texts";

    // Array de objetos RewardImage presentes en la escena (asigna desde el Inspector)
    public RewardImage[] rewardImages;

    private TextDatabase textDatabase;

    private void Start() {
        LoadJSON();
        AssignRandomTexts();
    }

    // Carga el JSON en un objeto TextDatabase
    void LoadJSON() {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
        if(jsonFile != null) {
            textDatabase = JsonUtility.FromJson<TextDatabase>(jsonFile.text);
            Debug.Log("JSON cargado. Textos: " + textDatabase.texts.Length);
        } else {
            Debug.LogError("No se encontró el archivo JSON: " + jsonFileName);
        }
    }

    // Asigna aleatoriamente una entrada a cada RewardImage
    void AssignRandomTexts() {
        if(textDatabase == null || textDatabase.texts.Length == 0 || rewardImages.Length == 0)
            return;

        foreach(RewardImage rImg in rewardImages) {
            int randomIndex = Random.Range(0, textDatabase.texts.Length);
            TextEntry entry = textDatabase.texts[randomIndex];
            rImg.imageText = entry.message;
            rImg.reward = entry.reward;
            // Actualiza el componente de texto si ya fue asignado
            if(rImg.textComponent != null)
                rImg.textComponent.text = entry.message;
        }
    }
}