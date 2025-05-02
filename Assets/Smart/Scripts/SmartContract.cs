using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;

[System.Serializable]
public class SmartContractData
{
    public int idSmartContract;
    public string descripcion;
    public string condicion;
    public int idRecompensa;
}

[System.Serializable]
public class SmartContractWrapper
{
    public SmartContractData[] contracts;
}

public class SmartContract : MonoBehaviour
{
    private Label[] contractTexts;
    private Button[] contractButtons;
    private Button backButton;

    private List<SmartContractData> smartContracts = new List<SmartContractData>();
    private string url = Variables.Variables.url;
    private int userId = 1; // ⚠️ Cambiar por el ID real del jugador

    private int totalMinedBlocks;
    private int totalScore;

    private VisualElement root; // Para acceder al root de la UI
    private Coroutine contractCheckCoroutine = null;  // Para manejar la corutina del contrato activo

    public static SmartContract Instance; // Singleton
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        UIDocument uiDoc = GameObject.Find("Smart").GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        contractTexts = new Label[] {
            root.Q<Label>("TextoC1"),
            root.Q<Label>("TextoC2"),
            root.Q<Label>("TextoC3")
        };

        contractButtons = new Button[] {
            root.Q<Button>("ActivateButtonC1"),
            root.Q<Button>("ActivateButtonC2"),
            root.Q<Button>("ActivateButtonC3")
        };

        backButton = root.Q<Button>("Regresar");

        // Intentamos cargar 'TotalMinedBlocks' y 'TotalScore' desde PlayerPrefs
        if (PlayerPrefs.HasKey("TotalMinedBlocks"))
        {
            totalMinedBlocks = PlayerPrefs.GetInt("TotalMinedBlocks");
        }
        else
        {
            totalMinedBlocks = 0;
        }

        if (PlayerPrefs.HasKey("TotalScore"))
        {
            totalScore = PlayerPrefs.GetInt("TotalScore");
        }
        else
        {
            totalScore = 0;
        }

        for (int i = 0; i < contractButtons.Length; i++)
        {
            int index = i;
            contractButtons[i].clicked += () => OnContractButtonClicked(index);
        }

        backButton.clicked += OnBackButtonClicked;

        CodeRunner.Instance.StartCoroutine(LoadSmartContractsFromServer());
    }

    private IEnumerator LoadSmartContractsFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get($"{url}/smartcontracts/random/{userId}");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            SmartContractData[] contracts = JsonConvert.DeserializeObject<SmartContractData[]>(jsonResponse);
            AssignContractsToUI(contracts);
        }
        else
        {
            Debug.LogError($"Error al cargar los Smart Contracts: {request.error}");
        }
    }

    private void AssignContractsToUI(SmartContractData[] contracts)
    {
        smartContracts.Clear();
        smartContracts.AddRange(contracts);

        for (int i = 0; i < contracts.Length && i < contractTexts.Length; i++)
        {
            contractTexts[i].text = contracts[i].descripcion;
        }
    }

    private void OnContractButtonClicked(int index)
    {
        if (index < 0 || index >= smartContracts.Count) return;

        SmartContractData contract = smartContracts[index];

        Debug.Log($"Contrato activado: {contract.descripcion}");

        // Si ya hay una corutina corriendo, la detenemos
        if (contractCheckCoroutine != null)
        {
            StopCoroutine(contractCheckCoroutine);
        }

        // Empezamos la corutina para verificar solo este contrato
        contractCheckCoroutine = CodeRunner.Instance.StartCoroutine(CheckConditionAndAssignReward(contract));
    }

    private IEnumerator CheckConditionAndAssignReward(SmartContractData contract)
    {
        while (true)
        {
            bool conditionMet = CheckConditionLocally(contract.condicion);

            if (conditionMet)
            {
                Debug.Log($"Contrato cumplido: {contract.descripcion}");
                yield return RegisterCompletedContract(contract);
                yield break;
            }

            yield return new WaitForSeconds(5f);  // Revisa cada 5 segundos
        }
    }

    private bool CheckConditionLocally(string conditionJson)
    {
        try
        {
            // Mostrar el JSON recibido para depuración
            Debug.Log($"Verificando condición con JSON: {conditionJson}");

            // Usamos Newtonsoft.Json para deserializar el JSON en un Diccionario
            var conditionDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(conditionJson);

            // Verificamos si las condiciones fueron deserializadas correctamente
            if (conditionDict == null || conditionDict.Count == 0)
            {
                Debug.LogError("Las condiciones deserializadas son null o vacías. Verifica el formato del JSON.");
                return false;
            }

            // Verificar las condiciones solo del contrato activo
            foreach (var pair in conditionDict)
            {
                int currentValue = 0;

                // Comprobamos la condición dependiendo de la clave
                if (pair.Key == "MinedBlocks")
                {
                    currentValue = totalMinedBlocks; // Usar el valor de bloques minados desde PlayerPrefs
                }
                else if (pair.Key == "WinCryptography" || pair.Key == "TriviaWins")
                {
                    currentValue = PlayerPrefs.GetInt(pair.Key, 0);
                }
                else if (pair.Key == "TotalScore")
                {
                    currentValue = PlayerPrefs.GetInt("TotalScore", 0);
                }
                else if (pair.Key == "GamesPlayed")
                {
                    currentValue = PlayerPrefs.GetInt("GamesPlayed", 0);
                }

                // Mostrar el valor que estamos comparando
                Debug.Log($"Condición: {pair.Key} | Valor actual: {currentValue} | Valor requerido: {pair.Value}");

                if (currentValue < pair.Value)
                {
                    Debug.Log($"Condición no cumplida: {pair.Key} ({currentValue}/{pair.Value})");
                    return false;
                }
            }

            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error al verificar la condición localmente. Excepción: {ex.Message}");
            return false;
        }
    }

    private IEnumerator RegisterCompletedContract(SmartContractData contract)
    {
        string jsonBody = $"{{\"userId\": {userId} }}";
        UnityWebRequest request = new UnityWebRequest($"{url}/smartcontracts/registerCompleted/{contract.idSmartContract}", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Contrato registrado como cumplido: {contract.descripcion}");
            yield return AssignReward(contract.idRecompensa);
        }
        else
        {
            Debug.LogError($"Error al registrar el contrato como cumplido: {request.error}");
        }
    }

    private IEnumerator AssignReward(int rewardId)
    {
        UnityWebRequest request = UnityWebRequest.Get($"{url}/reward/{rewardId}/{userId}");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Recompensa asignada correctamente.");
        }
        else
        {
            Debug.LogError($"Error al asignar la recompensa: {request.error}");
        }
    }

    private void OnBackButtonClicked()
    {
        SceneManager.LoadScene("City");
    }

    private void OnApplicationQuit()
    {
        // Guardamos el progreso al salir o cambiar de escena
        PlayerPrefs.SetInt("TotalMinedBlocks", totalMinedBlocks);
        PlayerPrefs.SetInt("TotalScore", totalScore); // Guardamos también el TotalScore
        PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed", 0)); // Guardamos también GamesPlayed
        PlayerPrefs.Save();  // Aseguramos que los cambios se guarden
    }

    // Incrementa el número de juegos jugados y actualiza PlayerPrefs
    public void IncrementGamesPlayed()
    {
        int gamesPlayed = PlayerPrefs.GetInt("GamesPlayed", 0);
        gamesPlayed++;
        PlayerPrefs.SetInt("GamesPlayed", gamesPlayed);
        PlayerPrefs.Save(); // Guardar el progreso de inmediato
        Debug.Log($"Games Played: {gamesPlayed}");

        // Aquí se puede llamar a la función que verifica el progreso del contrato.
        CheckContracts();
    }

    // Función para verificar todos los contratos
    public void CheckContracts()
    {
        // Detener cualquier corutina previa
        if (contractCheckCoroutine != null)
        {
            StopCoroutine(contractCheckCoroutine);
        }

        // Empezamos la corutina para verificar los contratos activos
        contractCheckCoroutine = StartCoroutine(CheckContractsPeriodically());
    }

    // Corutina que verifica todos los contratos periódicamente
    private IEnumerator CheckContractsPeriodically()
    {
        while (true)
        {
            // Revisamos cada contrato y verificamos si se cumplen las condiciones
            foreach (var contract in smartContracts)
            {
                bool conditionMet = CheckConditionLocally(contract.condicion);

                if (conditionMet)
                {
                    Debug.Log($"Contrato cumplido: {contract.descripcion}");
                    yield return RegisterCompletedContract(contract); // Registrar el contrato como completado
                }
            }

            // Esperamos 5 segundos antes de revisar nuevamente
            yield return new WaitForSeconds(5f);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Smart")
        {
            return;
        }
    }
}
