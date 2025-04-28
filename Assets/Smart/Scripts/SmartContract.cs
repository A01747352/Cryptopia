using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class SmartContract : MonoBehaviour
{
    private Label[] contractTexts; // Referencias a los textos de los contratos
    private Button[] contractButtons; // Referencias a los botones de los contratos
    private Button backButton; // Botón para regresar a la escena "Park"

    private List<SmartContractData> smartContracts = new List<SmartContractData>();
    private string url = Variables.Variables.url;

    private void Start()
    {
        // Obtener referencias a los elementos del UI
        var root = GetComponent<UIDocument>().rootVisualElement;
        contractTexts = new Label[]
        {
            root.Q<Label>("TextoC1"),
            root.Q<Label>("TextoC2"),
            root.Q<Label>("TextoC3")
        };

        contractButtons = new Button[]
        {
            root.Q<Button>("ActivateButtonC1"),
            root.Q<Button>("ActivateButtonC2"),
            root.Q<Button>("ActivateButtonC3")
        };

        backButton = root.Q<Button>("Regresar");

        // Asignar eventos a los botones
        for (int i = 0; i < contractButtons.Length; i++)
        {
            int index = i; // Capturar el índice para usarlo en el callback
            contractButtons[i].clicked += () => OnContractButtonClicked(index);
        }

        backButton.clicked += OnBackButtonClicked;

        // Cargar contratos desde el servidor
        StartCoroutine(LoadSmartContractsFromServer());
    }

    private IEnumerator LoadSmartContractsFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get($"{url}/smartcontracts/random");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            SmartContractData[] contracts = JsonUtility.FromJson<SmartContractWrapper>($"{{\"contracts\":{jsonResponse}}}").contracts;

            // Asignar contratos al UI
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
        StartCoroutine(CheckConditionAndAssignReward(contract));
    }

    private IEnumerator CheckConditionAndAssignReward(SmartContractData contract)
    {
        UnityWebRequest request = UnityWebRequest.Get($"{url}/smartcontracts/check/{contract.idSmartContract}");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            bool conditionMet = bool.Parse(request.downloadHandler.text);
            if (conditionMet)
            {
                // Asignar recompensa
                StartCoroutine(AssignReward(contract.idRecompensa));
            }
            else
            {
                Debug.Log("La condición no se ha cumplido.");
            }
        }
        else
        {
            Debug.LogError($"Error al verificar la condición: {request.error}");
        }
    }

    private IEnumerator AssignReward(int rewardId)
    {
        UnityWebRequest request = UnityWebRequest.Get($"{url}/reward/{rewardId}/1"); // Cambia "1" por el ID del usuario actual
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
}

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
