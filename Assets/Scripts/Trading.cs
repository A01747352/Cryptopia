using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class Trading : MonoBehaviour
{
    private string url = Variables.Variables.url;
    private int userId;
    [SerializeField]   
    public VisualTreeAsset cryptoItemTemplate;
    double userTKNs;

    // UI Elements
    private UIDocument gameUI;
    private VisualElement root;
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
    private Button goBackButton;
    private Label traderCryptoAbreviation;

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
        userId = PlayerPrefs.GetInt("UserId", 1);
        
    }

    void OnEnable()
    {
        gameUI = GetComponent<UIDocument>();
        root = gameUI.rootVisualElement;

        LoadTradeAssets();
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
            //Debug.Log($"Wallet JSON: {jsonString}");
            wallet = JsonConvert.DeserializeObject<CryptoCurrency[]>(jsonString);
            // foreach (var crypto in wallet)
            // {
            //     Debug.Log($"Nombre: {crypto.nombre}, Abreviatura: {crypto.abreviatura}, Cantidad: {crypto.cantidad}, Precio: {crypto.precio}");
            // }
        }
        else
        {
            Debug.LogError($"Error retrieving wallet: {webRequest.error}");
        }
    }

    private void LoadTradeAssets()
    {
        RetrieveWallet();
        
        scrollCrypto = root.Q<VisualElement>("ScrollCrypto");
        var cryptoContainer = root.Q<VisualElement>("CryptoContainer");
        
        userTKNs = wallet[0].cantidad;

        if (cryptoContainer.childCount > 0)
        {
            cryptoContainer.Clear();
            
        }
        
        for (int i = 1; i < wallet.Length; i++) 
        {
            var crypto = wallet[i];
            var item = cryptoItemTemplate.CloneTree();
            item.Q<Label>("Abbreviation").text = crypto.abreviatura;
            item.Q<VisualElement>("CryptoIcon").style.backgroundImage = new StyleBackground(Resources.Load<Sprite>($"CryptoIcons/{crypto.nombre}"));
            item.Q<Label>("Price").text = crypto.precio.ToString() + " TKN";
            sCBuyButton = item.Q<Button>("Buy");
            sCBuyButton.RegisterCallback<ClickEvent>(evt => ShowBuyWindow(evt, crypto));
            sCSellButton = item.Q<Button>("Sell");
            sCSellButton.RegisterCallback<ClickEvent>(evt => ShowSellWindow(evt, crypto));

            cryptoContainer.Add(item);
        }

    }

    // UI Functions
    private void ShowBuyWindow(ClickEvent evt, CryptoCurrency crypto)
    {
        scrollCrypto.style.display = DisplayStyle.None;
        traderWindow = gameUI.rootVisualElement.Q<VisualElement>("BuyWindow");
        traderWindow.style.display = DisplayStyle.Flex;

        goBackButton = root.Q<Button>("GoBackButton");
        goBackButton.style.display = DisplayStyle.Flex;
        goBackButton.RegisterCallback<ClickEvent>(evt => {
            LoadFunction(LoadTradeAssets);
            traderWindow.style.display = DisplayStyle.None;
            scrollCrypto.style.display = DisplayStyle.Flex;
            goBackButton.style.display = DisplayStyle.None;
        });

        traderCryptoName = traderWindow.Q<Label>("TraderCryptoName");
        traderCryptoIcon = traderWindow.Q<VisualElement>("TraderCryptoIcon");
        traderCryptoPrice = traderWindow.Q<Label>("TraderCryptoPrice");
        tfTokens = traderWindow.Q<TextField>("TfTokens");
        tfCryptoCoin = traderWindow.Q<TextField>("TfCryptoCoin");
        
        traderButton = root.Q<Button>("TraderBuyButton");

        walletContainer = traderWindow.Q<VisualElement>("WalletContainer");
        traderCryptoAbreviation = walletContainer.Q<Label>("CryptoCoinLabel");
        tKNsValue = walletContainer.Q<Label>("TKNsValue");
        cryptoCoinValue = walletContainer.Q<Label>("CryptoCoinValue");
        recieveTKNValue = traderWindow.Q<Label>("RecieveTKNValue");

        
        tKNsValue.text = wallet[0].cantidad.ToString();
        traderCryptoAbreviation.text = crypto.abreviatura;
        cryptoCoinValue.text = crypto.cantidad.ToString();

        traderCryptoName.text = crypto.nombre;
        traderCryptoIcon.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>($"CryptoIcons/{crypto.nombre}"));
        traderCryptoPrice.text = $"1 {crypto.abreviatura} = " + crypto.precio.ToString() + " TKN";

        bool isUpdating = false; // Flag to prevent feedback loop

        tfTokens.RegisterCallback<ChangeEvent<string>>(evt => {
            if (isUpdating) return; 
            isUpdating = true;

            tfTokens.style.color = new StyleColor(Color.black);
            double tkns;

            if (string.IsNullOrEmpty(evt.newValue) || evt.newValue == "\n")
            {
                
            }
            else if (double.TryParse(evt.newValue, out tkns))
            {
                double cryptoCoin = tkns / crypto.precio;
                tfCryptoCoin.SetValueWithoutNotify(cryptoCoin.ToString("F10")); 
            }
            else 
            {
                StartCoroutine(Alert("Invalid Input. Please input a quantity.",2));
                tfCryptoCoin.SetValueWithoutNotify(""); 
            }

            isUpdating = false;
        });

        tfCryptoCoin.RegisterCallback<ChangeEvent<string>>(evt => {
            if (isUpdating) return;
            isUpdating = true;

            tfCryptoCoin.style.color = new StyleColor(Color.black);
            double cryptoCoin;
            if (string.IsNullOrEmpty(evt.newValue) || evt.newValue == "\n")
            {
                
            }
            else if (double.TryParse(evt.newValue, out cryptoCoin))
            {
                double tkns = cryptoCoin * crypto.precio;
                tfTokens.SetValueWithoutNotify(tkns.ToString("F10")); 
            }
            else 
            {
                StartCoroutine(Alert("Invalid Input. Please input a quantity.",2));
                tfTokens.SetValueWithoutNotify(""); 
            }
            isUpdating = false;
        });

        traderButton.RegisterCallback<ClickEvent>(evt => 
        {
            try 
            {
                BuyCrypto(evt, crypto, double.Parse(tfCryptoCoin.value), double.Parse(tfTokens.value));
            }
            catch (FormatException)
            {
                StartCoroutine(Alert("Invalid Input. Please input a quantity.", 2));
            }
            catch (OverflowException)
            {
                StartCoroutine(Alert("Input too large. Please input a smaller quantity.", 2));
            }
            catch (Exception ex)
            {
                StartCoroutine(Alert($"An error occurred: {ex.Message}", 2));
            }
        // Reset the TextFields after the transaction
            tfTokens.SetValueWithoutNotify("0.000000 TKN");
            tfCryptoCoin.SetValueWithoutNotify("0.000000 " + crypto.abreviatura);
                
        
        });

        

    }

    private void ShowSellWindow(ClickEvent evt, CryptoCurrency crypto)
    {
        scrollCrypto.style.display = DisplayStyle.None;
        traderWindow = gameUI.rootVisualElement.Q<VisualElement>("SellWindow");
        traderWindow.style.display = DisplayStyle.Flex;

        goBackButton = root.Q<Button>("GoBackButton");
        goBackButton.style.display = DisplayStyle.Flex;
        goBackButton.RegisterCallback<ClickEvent>(evt => {
            LoadFunction(LoadTradeAssets);
            traderWindow.style.display = DisplayStyle.None;
            scrollCrypto.style.display = DisplayStyle.Flex;
            goBackButton.style.display = DisplayStyle.None;
        });
        

        traderCryptoName = traderWindow.Q<Label>("TraderCryptoName");
        traderCryptoIcon = traderWindow.Q<VisualElement>("TraderCryptoIcon");
        traderCryptoPrice = traderWindow.Q<Label>("TraderCryptoPrice");
        recieveTKNValue = traderWindow.Q<Label>("RecieveTKNValue");
        traderButton = traderWindow.Q<Button>("TraderSellButton");
        tfCryptoCoin = traderWindow.Q<TextField>("TfCryptoCoin");
        traderCryptoAbreviation = tfCryptoCoin.Q<Label>("Label");

        walletContainer = traderWindow.Q<VisualElement>("WalletContainer");
        traderCryptoAbreviation = walletContainer.Q<Label>("CryptoCoinLabel");
        tKNsValue = walletContainer.Q<Label>("TKNsValue");
        cryptoCoinValue = walletContainer.Q<Label>("CryptoCoinValue");
        tKNsValue.text = wallet[0].cantidad.ToString();
        cryptoCoinValue.text = crypto.cantidad.ToString();

        tfCryptoCoin.SetValueWithoutNotify("0.000000 " + crypto.abreviatura);
        traderCryptoName.text = crypto.nombre;
        traderCryptoAbreviation.text = crypto.abreviatura;
        traderCryptoIcon.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>($"CryptoIcons/{crypto.nombre}"));
        traderCryptoPrice.text = $"1 {crypto.abreviatura} = " + crypto.precio.ToString() + " TKN";

        bool isUpdating = false; 
        tfCryptoCoin.RegisterCallback<ChangeEvent<string>>(evt => {
            if (isUpdating) return;
            isUpdating = true;

            tfCryptoCoin.style.color = new StyleColor(Color.black);
            double cryptoCoin;
            if (string.IsNullOrEmpty(evt.newValue) || evt.newValue == "\n")
            {
                
            }
            else if (double.TryParse(evt.newValue, out cryptoCoin))
            {
                double tkns = cryptoCoin * crypto.precio;
                recieveTKNValue.text = tkns.ToString("F10");
            }
            else 
            {
                StartCoroutine(Alert("Invalid Input. Please input a quantity.",2));
                tfTokens.SetValueWithoutNotify(""); 
            }
            isUpdating = false;
        });

        traderCryptoName.text = crypto.nombre;
        traderCryptoIcon.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>($"CryptoIcons/{crypto.nombre}"));
        traderCryptoPrice.text = crypto.precio.ToString() + " TKN";

        traderButton.RegisterCallback<ClickEvent>(evt => 
        {
            try
            {
                SellCrypto(evt, crypto, double.Parse(tfCryptoCoin.value), double.Parse(recieveTKNValue.text));
            }
            catch (FormatException)
            {
                StartCoroutine(Alert("Invalid Input. Please input a quantity.",2));
            }
            catch (OverflowException)
            {
                StartCoroutine(Alert("Input too large. Please input a smaller quantity.",2));
            }
            catch (Exception ex)
            {
                StartCoroutine(Alert($"An error occurred: {ex.Message}",2));
            }
    
            tfCryptoCoin.SetValueWithoutNotify("0.000000 " + crypto.abreviatura);
            recieveTKNValue.text = "0.000000 TKN";
        });
    }

    // Buy crypto
    private void BuyCrypto(ClickEvent evt, CryptoCurrency crypto, double amountBought, double tknSpent)
    {
        
        if (tknSpent > userTKNs)
        {
            StartCoroutine(Alert(false));
            return;
        }
        else
        {
            StartCoroutine(Alert(true));
            // Create a transaction object
            Transaction transaction = new Transaction
            {
                cryptoSold = "Token",
                cryptoBought = crypto.nombre,
                amountSold = tknSpent,
                amountBought = amountBought
            };

            // Convert the transaction object to JSON
            string jsonTransaction = JsonConvert.SerializeObject(transaction);

            // Send the transaction to the server
            StartCoroutine(SendTransaction(jsonTransaction));

            tKNsValue.text = (userTKNs - tknSpent).ToString();
            cryptoCoinValue.text = (crypto.cantidad + amountBought).ToString();
        }
    }


    // Sell crypto

    private void SellCrypto(ClickEvent evt, CryptoCurrency crypto, double amountSold, double tknRecieved)
    {
        if (amountSold > crypto.cantidad)
        {
            StartCoroutine(Alert(false));
        }
        else
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
            StartCoroutine(Alert(true));

            tKNsValue.text = (userTKNs + tknRecieved).ToString();
            cryptoCoinValue.text = (crypto.cantidad - amountSold).ToString();
        }
    }

    // Send Transaction
    private IEnumerator SendTransaction(string jsonTransaction)
    {
        UnityWebRequest request = UnityWebRequest.Post($"{url}/trading/registerTransaction/{userId}", jsonTransaction, "application/json");
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
    
    // Retrieve crypto prices from the server

 

    private IEnumerator Alert(bool result)
    {
        var alertWindow = root.Q<VisualElement>("Alert");

        if (result)
        {
            alertWindow.Q<Label>("AlertLabel").text = "Transaction successful";
            alertWindow.style.display = DisplayStyle.Flex;
        }
        else
        {
            alertWindow.Q<Label>("AlertLabel").text = "Insufficient funds";
            alertWindow.style.display = DisplayStyle.Flex;
        }
        yield return new WaitForSeconds(2);
        alertWindow.style.display = DisplayStyle.None;
    }

    private IEnumerator Alert(string alert, int duration)
    {
        var alertWindow = root.Q<VisualElement>("Alert");
        alertWindow.Q<Label>("AlertLabel").text = alert;
        alertWindow.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(duration);
        alertWindow.style.display = DisplayStyle.None;
    }
    private void LoadFunction(Action functionToRun)
    {
        var alertWindow = root.Q<VisualElement>("Alert");
        alertWindow.Q<Label>("AlertLabel").text = "Loading...";
        alertWindow.style.display = DisplayStyle.Flex;
        print("Loading...");
        
        functionToRun();

        alertWindow.style.display = DisplayStyle.None;
    }
}
