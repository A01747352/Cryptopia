using System.Collections;
using NUnit.Framework.Interfaces;
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
    string url = "http://localhost:8080";

    public struct Login
    {
        public string user;
        public string password;
    }

    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        var registerButton = root.Q<Button>("registerButton");
        var loginButton = root.Q<Button>("loginButton");
        userTextField = root.Q<TextField>("user");
        passwordTextField = root.Q<TextField>("password");

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
        string user = userTextField.value;
        string password = passwordTextField.value;

        StartCoroutine(VerifyCredentials(user, password));
    }
    private IEnumerator VerifyCredentials(string user, string password)
        {
        Login log;
        log.user = user;
        log.password = password;

        string JsonLogin= JsonConvert.SerializeObject(log);
        UnityWebRequest webRequest = UnityWebRequest.Post($"{url}/login", JsonLogin, "application/json");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("VAMO");
        }
        else
        {
            Debug.LogError($"CHUPAME LA VERGA: {webRequest.error}");
        }
        }
    }
