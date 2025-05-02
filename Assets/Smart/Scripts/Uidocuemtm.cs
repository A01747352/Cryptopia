using UnityEngine;
using UnityEngine.UIElements;

public class UiDocumentm : MonoBehaviour
{
    public static UiDocumentm Instance { get; private set; }

    public UIDocument uiDocument;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        uiDocument = GetComponent<UIDocument>();
    }

    public UIDocument GetUIDocument()
    {
        return uiDocument;
    }

    public VisualElement GetRootVisualElement()
    {
        return uiDocument.rootVisualElement;
    }

    public void IncrementGamesPlayed()
    {
        int current = PlayerPrefs.GetInt("GamesPlayed", 0) + 1;
        PlayerPrefs.SetInt("GamesPlayed", current);
        PlayerPrefs.Save();
        Debug.Log($"GamesPlayed actualizado: {current}");
    }

    public void IncrementGamesPlayedAfterGame()
    {
        IncrementGamesPlayed();
    }

    public void IncrementWinCryptography()
    {
        int current = PlayerPrefs.GetInt("WinCryptography", 0) + 1;
        PlayerPrefs.SetInt("WinCryptography", current);
        PlayerPrefs.Save();
        Debug.Log($"WinCryptography actualizado: {current}");
    }

    public void IncrementTriviaWins()
    {
        int current = PlayerPrefs.GetInt("TriviaWins", 0) + 1;
        PlayerPrefs.SetInt("TriviaWins", current);
        PlayerPrefs.Save();
        Debug.Log($"TriviaWins actualizado: {current}");
    }

    public void IncrementTotalScore(int score)
    {
        int current = PlayerPrefs.GetInt("TotalScore", 0) + score;
        PlayerPrefs.SetInt("TotalScore", current);
        PlayerPrefs.Save();
        Debug.Log($"TotalScore actualizado: {current}");
    }

    // ✅ BLOQUES MINADOS (forma idéntica a GamesPlayed)
    public void IncrementMinedBlocks()
    {
        int current = PlayerPrefs.GetInt("MinedBlocks", 0) + 1;
        PlayerPrefs.SetInt("MinedBlocks", current);
        PlayerPrefs.Save();
        Debug.Log($"MinedBlocks actualizado: {current}");
    }

    public void IncrementMinedBlocksAfterAction()
    {
        IncrementMinedBlocks();
    }
}
