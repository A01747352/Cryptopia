using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class Trading : MonoBehaviour
{
    string url = "http://localhost:8080";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
