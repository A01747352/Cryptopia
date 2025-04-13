using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;  // Para cargar escenas
using System.Collections; // Para el manejo de coroutines
using UnityEngine.Networking;

public class LoginUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject registerUIObject;
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Nombre de la escena

    private VisualElement root;
    private TextField userTextField;
    private TextField passwordTextField;

    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        // Obtén los campos de texto
        userTextField = root.Q<TextField>("user");
        passwordTextField = root.Q<TextField>("password");

        var loginButton = root.Q<Button>("loginButton");
        var registerButton = root.Q<Button>("registerButton");

        if (loginButton != null)
        {
            loginButton.clicked -= OnLoginClicked;
            loginButton.clicked += OnLoginClicked;
        }

        if (registerButton != null)
        {
            registerButton.clicked -= OnRegisterClicked;
            registerButton.clicked += OnRegisterClicked;
        }
    }

    // Método de login
    private void OnLoginClicked()
    {
        string user = userTextField.value;
        string password = passwordTextField.value;

        StartCoroutine(VerifyCredentials(user, password, isValid =>
        {
            if (isValid)
            {
                SceneManager.LoadScene(mainMenuSceneName);
            }
            else
            {
                Debug.LogError("Credenciales incorrectas");
            }
        }));
    }

    private IEnumerator VerifyCredentials(string user, string password, System.Action<bool> callback)
    {
        string url = "http://localhost:8080/login"; 
        var credentials = new {user, password };
        string jsonData = JsonUtility.ToJson(credentials);

        using (UnityWebRequest request = UnityWebRequest.Post(url, jsonData, "application/json"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<ServerResponse>(request.downloadHandler.text);
                callback(response.result == "Correct Login");
            }
            else
            {
                Debug.LogError($"Error in VerifyCredentials: {request.error}");
                callback(false);
            }
        }
    }

    [System.Serializable]
    private class ServerResponse
    {
        public string result;
    }

    private void OnRegisterClicked()
    {
        gameObject.SetActive(false);           
        registerUIObject.SetActive(true);    
    }
}
