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

    public void IncrementGamesPlayedAfterGame()
    {
        if (SmartContract.Instance != null && SmartContract.Instance.IsConditionTrackedByActiveContract("GamesPlayed"))
        {
            int current = PlayerPrefs.GetInt("GamesPlayed", 0) + 1;
            PlayerPrefs.SetInt("GamesPlayed", current);
            PlayerPrefs.Save();
            Debug.Log($"GamesPlayed actualizado: {current}");
        }
        else
        {
            Debug.Log("No hay contrato activo que requiera GamesPlayed.");
        }
    }

    public void IncrementMinedBlocksAfterAction()
    {
        if (SmartContract.Instance != null && SmartContract.Instance.IsConditionTrackedByActiveContract("MinedBlocks"))
        {
            int current = PlayerPrefs.GetInt("MinedBlocks", 0) + 1;
            PlayerPrefs.SetInt("MinedBlocks", current);
            PlayerPrefs.Save();
            Debug.Log($"MinedBlocks actualizado: {current}");
        }
        else
        {
            Debug.Log("No hay contrato activo que requiera MinedBlocks.");
        }
    }

    public void IncrementWinCryptography()
    {
        if (SmartContract.Instance != null && SmartContract.Instance.IsConditionTrackedByActiveContract("WinCryptography"))
        {
            int current = PlayerPrefs.GetInt("WinCryptography", 0) + 1;
            PlayerPrefs.SetInt("WinCryptography", current);
            PlayerPrefs.Save();
            Debug.Log($"WinCryptography actualizado: {current}");
        }
        else
        {
            Debug.Log("No hay contrato activo que requiera WinCryptography.");
        }
    }

    public void IncrementTriviaWins()
    {
        if (SmartContract.Instance != null && SmartContract.Instance.IsConditionTrackedByActiveContract("TriviaWins"))
        {
            int current = PlayerPrefs.GetInt("TriviaWins", 0) + 1;
            PlayerPrefs.SetInt("TriviaWins", current);
            PlayerPrefs.Save();
            Debug.Log($"TriviaWins actualizado: {current}");
        }
        else
        {
            Debug.Log("No hay contrato activo que requiera TriviaWins.");
        }
    }

    public void IncrementTotalScore(int score)
    {
        if (SmartContract.Instance != null && SmartContract.Instance.IsConditionTrackedByActiveContract("TotalScore"))
        {
            int current = PlayerPrefs.GetInt("TotalScore", 0) + score;
            PlayerPrefs.SetInt("TotalScore", current);
            PlayerPrefs.Save();
            Debug.Log($"TotalScore actualizado: {current}");
        }
        else
        {
            Debug.Log("No hay contrato activo que requiera TotalScore.");
        }
    }
}
