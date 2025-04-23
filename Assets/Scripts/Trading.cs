using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class Trading : MonoBehaviour
{
    string url = "http://localhost:8080";
    private int userId;

    public struct CryptoCurrency
    {
        public string nombre;
        public string abreviatura;
        public double cantidad;
    }

    private CryptoCurrency[] wallet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        userId = PlayerPrefs.GetInt("userId", 1);
        RetrieveWallet();
    }

    // Retrieving user Wallet
    private void RetrieveWallet()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/trading/getUserWallet/{userId}");
        webRequest.SendWebRequest();

        while (!webRequest.isDone) {}

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonString = webRequest.downloadHandler.text;
            Debug.Log($"Wallet JSON: {jsonString}");
            wallet = JsonConvert.DeserializeObject<CryptoCurrency[]>(jsonString);
            foreach (var crypto in wallet)
            {
                Debug.Log($"Nombre: {crypto.nombre}, Abreviatura: {crypto.abreviatura}, Cantidad: {crypto.cantidad}");
            }
        }
        else
        {
            Debug.LogError($"Error retrieving wallet: {webRequest.error}");
        }
    }

    // Retrieve crypto prices from the server

    private IEnumerator LoadCrypto()
    {
        UnityWebRequest request = UnityWebRequest.Get($"{url}/trading/retrieveCryptoPrices");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            print(request.downloadHandler.text);
        }
    }
}
