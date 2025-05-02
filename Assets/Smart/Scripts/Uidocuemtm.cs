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
            Destroy(gameObject); // Destroy duplicates
            return;
        }

        Instance = this;
        print($"Instance of {nameof(UiDocumentm)} created.");
        DontDestroyOnLoad(gameObject); // Persist across scenes

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

    // Aquí agregamos la funcionalidad del seguimiento del progreso y verificación de contratos
    public void IncrementGamesPlayed()
    {
        int current = PlayerPrefs.GetInt("GamesPlayed", 0) + 1;
        Debug.Log($"GamesPlayed antes de actualizar: {current}");  // Verificar el valor antes de actualizar
        PlayerPrefs.SetInt("GamesPlayed", current);
        PlayerPrefs.Save();
        Debug.Log($"GamesPlayed actualizado: {current}");  // Confirmar que se actualizó

        SmartContract.Instance.CheckContracts(); // Verifica si se cumplió alguna condición
    }

    // Incrementar GamesPlayed siempre que el jugador participe, independientemente del resultado.
    public void IncrementGamesPlayedAfterGame()
    {
        IncrementGamesPlayed(); // Llamada al método que incrementa GamesPlayed después de jugar
    }

    public void IncrementWinCryptography()
    {
        int current = PlayerPrefs.GetInt("WinCryptography", 0) + 1;
        PlayerPrefs.SetInt("WinCryptography", current);
        PlayerPrefs.Save();
        Debug.Log($"WinCryptography actualizado: {current}");

        SmartContract.Instance.CheckContracts();
    }

    public void IncrementTriviaWins()
    {
        int current = PlayerPrefs.GetInt("TriviaWins", 0) + 1;
        PlayerPrefs.SetInt("TriviaWins", current);
        PlayerPrefs.Save();
        Debug.Log($"TriviaWins actualizado: {current}");

        SmartContract.Instance.CheckContracts();
    }

    public void IncrementTotalScore(int score)
    {
        int current = PlayerPrefs.GetInt("TotalScore", 0) + score;
        PlayerPrefs.SetInt("TotalScore", current);
        PlayerPrefs.Save();
        Debug.Log($"TotalScore actualizado: {current}");

        SmartContract.Instance.CheckContracts();
    }

    public void IncrementMinedBlocks(int blocks)
    {
        int current = PlayerPrefs.GetInt("MinedBlocks", 0) + blocks;
        PlayerPrefs.SetInt("MinedBlocks", current);
        PlayerPrefs.Save();
        Debug.Log($"MinedBlocks actualizado: {current}");

        SmartContract.Instance.CheckContracts();
    }
}
