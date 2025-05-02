using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class LoginUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject registerUIObject;

    private VisualElement root;
    private TextField userTextField;
    private TextField passwordTextField;
    private string url = Variables.Variables.url;
    private Label errorMessageLabel;


    public struct Login
    {
        public string user;
        public string password;
    }


    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        var registerButton = root.Q<Button>("registerButton");
        var loginButton = root.Q<Button>("loginButton");
        userTextField = root.Q<TextField>("user");
        passwordTextField = root.Q<TextField>("password");
        errorMessageLabel = root.Q<Label>("errorMessage");


        if (registerButton != null)
        {
            registerButton.clicked -= OnRegisterClicked;
            registerButton.clicked += OnRegisterClicked;
        }

        if (loginButton != null)
        {
            loginButton.clicked -= OnLoginClicked;
            loginButton.clicked += OnLoginClicked;
        }

    }

    private void OnRegisterClicked()
    {
        gameObject.SetActive(false);
        registerUIObject.SetActive(true);
    }

    private void OnLoginClicked()
    {
        errorMessageLabel.text = "";
        errorMessageLabel.style.color = Color.red;

        string user = userTextField.value;
        string password = passwordTextField.value;

        StartCoroutine(VerifyCredentials(user, password));
    }


    private IEnumerator VerifyCredentials(string user, string password)
    {
        Login log;
        log.user = user;
        log.password = password;

        string JsonLogin = JsonConvert.SerializeObject(log);
        UnityWebRequest webRequest = UnityWebRequest.Post($"{url}/login", JsonLogin, "application/json");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Dictionary<string, string> response = JsonConvert.DeserializeObject<Dictionary<string, string>>(webRequest.downloadHandler.text);
            if (response["result"] == "True")
            {
                Debug.Log("Login successful!");
                errorMessageLabel.style.color = Color.green;
                errorMessageLabel.text = "Login successful!";

                // Save user ID in PlayerPrefs
                if (response.ContainsKey("userId"))
                {
                    PlayerPrefs.SetInt("UserId", int.Parse(response["userId"]));
                    PlayerPrefs.Save();
                    Debug.Log($"User ID {response["userId"]} saved in PlayerPrefs.");
                }

                // Esperar 1 segundo antes de cambiar de escena
                yield return new WaitForSeconds(1f);
                SceneManager.LoadScene("MainMenu");
            }


            else
            {
                errorMessageLabel.text = "Invalid username or password.";
            }

        }
        else
        {
            errorMessageLabel.text = "Network error. Please try again.";
            Debug.LogError($"Error: {webRequest.error}");

        }
    }
}
