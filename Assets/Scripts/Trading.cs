using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class Trading : MonoBehaviour
{
    string url = "http://localhost:8080";
    private int userId;
    [SerializeField]   
    public VisualTreeAsset cryptoItemTemplate;

    // UI Elements
    private UIDocument gameUI;
       // References to the CryptoScroll window in the UI
    private VisualElement scrollCrypto;
    private Button sCBuyButton;
    private Button sCSellButton;

       // References to BuyWindow and SellWindow in the UI
       // Trader
    private VisualElement traderWindow;
    private Label traderCryptoName;
    private VisualElement traderCryptoIcon;
    private Label traderCryptoPrice;
    private TextField tfTokens;
    private TextField tfCryptoCoin;
    private Button traderButton;

      // Wallet
    private VisualElement walletContainer;
    private Label tKNsValue;
    private Label cryptoCoinValue;
    private Label recieveTKNValue;


    public struct CryptoCurrency
    {
        public string nombre;
        public string abreviatura;
        public double cantidad;
        public double precio;
    }

    public struct Transaction
    {
        public string cryptoSold;
        public string cryptoBought;
        public double amountSold;
        public double amountBought;
    }

    private CryptoCurrency[] wallet;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        userId = PlayerPrefs.GetInt("userId", 1);
        RetrieveWallet();
    }

    void OnEnable()
    {
        gameUI = GetComponent<UIDocument>();
        var root = gameUI.rootVisualElement;
        scrollCrypto = root.Q<VisualElement>("CryptoscrollCrypto");

        foreach (var crypto in wallet)
        {
            var item = cryptoItemTemplate.CloneTree();
            item.Q<Label>("Abbreviation").text = crypto.abreviatura;
            item.Q<VisualElement>("CryptoIcon").style.backgroundImage = new StyleBackground(Resources.Load<Sprite>($"CryptoIcons/{crypto.nombre}"));
            item.Q<Label>("Price").text = crypto.precio.ToString() + " TKN";
            sCBuyButton = item.Q<Button>("Buy");
            sCBuyButton.RegisterCallback<ClickEvent>(evt => ShowBuyWindow(evt, crypto));
            sCSellButton = item.Q<Button>("Sell");
            sCSellButton.RegisterCallback<ClickEvent>(evt => ShowSellWindow(evt, crypto));

            scrollCrypto.Add(item);
        }

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

    // UI Functions
    private void ShowBuyWindow(ClickEvent evt, CryptoCurrency crypto)
    {
        scrollCrypto.style.display = DisplayStyle.None;
        traderWindow = gameUI.rootVisualElement.Q<VisualElement>("BuyWindow");
        traderWindow.style.display = DisplayStyle.Flex;

        traderCryptoName = traderWindow.Q<Label>("CryptoName");
        traderCryptoIcon = traderWindow.Q<VisualElement>("CryptoIcon");
        traderCryptoPrice = traderWindow.Q<Label>("CryptoPrice");
        tfTokens = traderWindow.Q<TextField>("Tokens");
        tfCryptoCoin = traderWindow.Q<TextField>("CryptoCoin");
        traderButton = traderWindow.Q<Button>("TraderButton");

        traderCryptoName.text = crypto.nombre;
        traderCryptoIcon.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>($"CryptoIcons/{crypto.nombre}"));
        traderCryptoPrice.text = "10000";


    }

    private void ShowSellWindow(ClickEvent evt, CryptoCurrency crypto)
    {
        scrollCrypto.style.display = DisplayStyle.None;
        traderWindow = gameUI.rootVisualElement.Q<VisualElement>("SellWindow");
        traderWindow.style.display = DisplayStyle.Flex;

        traderCryptoName = traderWindow.Q<Label>("CryptoName");
        traderCryptoIcon = traderWindow.Q<VisualElement>("CryptoIcon");
        traderCryptoPrice = traderWindow.Q<Label>("CryptoPrice");
        tfTokens = traderWindow.Q<TextField>("Tokens");
        recieveTKNValue = traderWindow.Q<Label>("RecieveTKNValue");
        traderButton = traderWindow.Q<Button>("TraderButton");

        traderCryptoName.text = crypto.nombre;
        traderCryptoIcon.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>($"CryptoIcons/{crypto.nombre}"));
        traderCryptoPrice.text = "10000";

        traderButton.RegisterCallback<ClickEvent>(evt => SellCrypto(evt, crypto, double.Parse(tfTokens.value), double.Parse(recieveTKNValue.text)));
    }

    // Buy crypto
    private void BuyCrypto(ClickEvent evt, CryptoCurrency crypto, double amountBought, double tknSpent)
    {
        // Create a transaction object
        Transaction transaction = new Transaction
        {
            cryptoSold = crypto.nombre,
            cryptoBought = "Token",
            amountSold = tknSpent,
            amountBought = amountBought
        };

        // Convert the transaction object to JSON
        string jsonTransaction = JsonConvert.SerializeObject(transaction);

        // Send the transaction to the server
        StartCoroutine(SendTransaction(jsonTransaction));
    }

    private IEnumerator SendTransaction(string jsonTransaction)
    {
        UnityWebRequest request = UnityWebRequest.Post($"{url}/trading/registerTransaction/:{userId}", jsonTransaction, "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Transaction successful: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Transaction failed: " + request.error);
        }
    }

    // Sell crypto

    private void SellCrypto(ClickEvent evt, CryptoCurrency crypto, double amountSold, double tknRecieved)
    {
        // Create a transaction object
        Transaction transaction = new Transaction
        {
            cryptoSold = crypto.nombre,
            cryptoBought = "Token",
            amountSold = amountSold,
            amountBought = tknRecieved
        };

        // Convert the transaction object to JSON
        string jsonTransaction = JsonConvert.SerializeObject(transaction);

        // Send the transaction to the server
        StartCoroutine(SendTransaction(jsonTransaction));
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
