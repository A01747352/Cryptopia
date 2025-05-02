using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using UnityEngine.Rendering;

public class Shop : MonoBehaviour
{
    private Button returnButton;
    private Button[] buyButtons;
    private VisualElement containerConfirm;
    private Button acceptButton;
    private Button cancelButton;
    private VisualElement background;
    private VisualElement containerSuccess;
    private VisualElement containerDupe;
    private Button okButton;
    private Button okDupe;
    private VisualElement containerAfford;
    private Button PoorButton;
    private int userId;
    private Label qtyLabel;

    // URL from the Variables class directly
    private string url = Variables.Variables.url;

    // Store the current itemId selected by the user
    private int currentItemId = -1;  // Default to an invalid ID

    public struct Items
    {
        public int itemId;
        public int userId;
    }

    // Create a dictionary that associates button names with their corresponding itemIds
    private Dictionary<string, int> itemIdMap = new Dictionary<string, int>()
    {
        { "buy_cat01", 1 },
        { "buy_cat02", 2 },
        { "buy_cat03", 3 },
        { "buy_cat04", 4 },
        { "buy_skin01_button", 5 },
        { "buy_skin02_button", 6 },
        { "pu01", 7 },
        { "pu02", 8 },
        { "pu03", 9 },
        { "pu04", 10 },
        { "pu05", 11 }
    };
    void Awake()
    {
        userId = PlayerPrefs.GetInt("UserId", 1);
    }

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        returnButton = root.Q<Button>("return");
        containerConfirm = root.Q<VisualElement>("container_confirm");
        acceptButton = root.Q<Button>("accept");
        cancelButton = root.Q<Button>("cancel");
        background = root.Q<VisualElement>("bkg");
        containerSuccess = root.Q<VisualElement>("container_success");
        containerDupe = root.Q<VisualElement>("container_dupe");
        okButton = root.Q<Button>("ok");
        okDupe = root.Q<Button>("ffs");
        containerAfford = root.Q<VisualElement>("container_afford");
        PoorButton = root.Q<Button>("ok_poor");
        qtyLabel = root.Q<Label>("qty");
        StartCoroutine(UpdateQtyLabel());

        buyButtons = new Button[]
        {
            root.Q<Button>("buy_cat01"),
            root.Q<Button>("buy_cat02"),
            root.Q<Button>("buy_cat03"),
            root.Q<Button>("buy_cat04"),
            root.Q<Button>("buy_skin01_button"),
            root.Q<Button>("buy_skin02_button"),
            root.Q<Button>("pu01"),
            root.Q<Button>("pu02"),
            root.Q<Button>("pu03"),
            root.Q<Button>("pu04"),
            root.Q<Button>("pu05")
        };

        // Assign events to buy buttons
        for (int i = 0; i < buyButtons.Length; i++)
        {
            if (buyButtons[i] != null)
            {
                string buttonName = buyButtons[i].name;
                buyButtons[i].clicked += () =>
                {
                    OnBuyButtonClicked(buttonName);  // Set the current itemId
                };
            }
        }

        // Add listeners for the other buttons
        if (returnButton != null)
        {
            returnButton.clicked += () =>
            {
                SceneManager.LoadScene("City");  // Load the main menu scene
            };
        }
        if (acceptButton != null)
        {
            acceptButton.clicked += OnAcceptButtonClicked;
        }
        if (cancelButton != null)
        {
            cancelButton.clicked += OnCancelButtonClicked;
        }
        if (okButton != null)
        {
            okButton.clicked += OnOkButtonClicked;
        }
        if (okDupe != null)
        {
            okDupe.clicked += HideDupeContainer;
            okDupe.clicked += HideBackground;
        }
        if (PoorButton != null)
        {
            PoorButton.clicked += HideContainerAfford;
            PoorButton.clicked += HideBackground;
        }

        HideConfirmationContainer();
        HideSuccessContainer();
    }

    private IEnumerator UpdateQtyLabel()
    {
        if (qtyLabel != null)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/cryptoShop/wallet/{userId}");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    // Parse the response as a float
                    float walletCantidad = float.Parse(webRequest.downloadHandler.text);
                    qtyLabel.text = walletCantidad.ToString("F2"); // Update the label text with two decimal places
                }
                catch (FormatException)
                {
                    Debug.LogError("Failed to parse walletCantidad from server response. Response: " + webRequest.downloadHandler.text);
                }
            }
            else
            {
                Debug.LogError($"Failed to fetch walletCantidad. Error: {webRequest.error}");
            }
        }
    }

    private void OnBuyButtonClicked(string buttonName)
    {
        if (itemIdMap.ContainsKey(buttonName))
        {
            currentItemId = itemIdMap[buttonName];  // Manually set the current itemId

            StartCoroutine(CheckItemOwnership(currentItemId));
        }
        else
        {
            Debug.LogError("No itemId found for button: " + buttonName);
        }
    }

    private IEnumerator CheckItemOwnership(int itemId)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/cryptoShop/ownsItem/{userId}/{itemId}");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            // Check if the response is "True" or "False"
            if (webRequest.downloadHandler.text == "True")
            {
                ShowDupeContainer();  // Show duplicate item container if the user owns the item
            }
            else
            {
                ShowConfirmationContainer();  // Proceed to normal purchase flow if the user doesn't own the item
            }
        }
        else
        {
            Debug.LogError($"Failed to check item ownership. Error: {webRequest.error}");
            ShowConfirmationContainer();  // Show confirmation container in case of an error
        }
    }

    private void OnAcceptButtonClicked()
    {
        if (currentItemId != -1)  // Ensure we have a valid itemId
        {
            StartCoroutine(EnoughToBuy(currentItemId));  // Use the manually assigned itemId
        }

        HideConfirmationContainer();  // Hide the confirmation container
    }

    private void EnogughTKN()
    {

        StartCoroutine(EnoughToBuy(currentItemId));  // Use the manually assigned itemId

    }
    private void StartBuy()
    {
        if (currentItemId != -1)  // Ensure we have a valid itemId
        {
            StartCoroutine(AddOwnedItem(currentItemId));  // Use the manually assigned itemId
        }
    }
    private IEnumerator EnoughToBuy(int currentItemId)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/cryptoShop/price/{userId}/{currentItemId}");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            // Check if the response is "True" or "False"
            if (webRequest.downloadHandler.text == "True")
            {
                StartBuy();
            }
            else
            {
                ShowContainerAfford();  // Show a container indicating the user cannot afford the item
            }
        }
        else
        {
            Debug.LogError($"Failed to check item ownership. Error: {webRequest.error}");
            ShowConfirmationContainer();  // Show confirmation container in case of an error
        }
    }
    private IEnumerator AddOwnedItem(int itemId)
    {
        Items item;
        item.itemId = itemId;
        item.userId = userId;
        Debug.Log($"Item ID: {item.itemId}, User ID: {item.userId}");
        string JsonItem = JsonConvert.SerializeObject(item);
        UnityWebRequest webRequest = UnityWebRequest.Post($"{url}/cryptoShop/buy/", JsonItem, "application/json");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Item successfully added to owned items.");
            ShowSuccessContainer();
        }
        else
        {
            Debug.LogError($"Failed to add item. Error: {webRequest.error}");
        }
    }

    private void ShowConfirmationContainer()
    {
        if (containerConfirm != null) containerConfirm.style.display = DisplayStyle.Flex;
        if (background != null) background.style.display = DisplayStyle.Flex;
    }

    private void HideConfirmationContainer()
    {
        if (containerConfirm != null) containerConfirm.style.display = DisplayStyle.None;
        if (background != null) background.style.display = DisplayStyle.None;
    }

    private void ShowSuccessContainer()
    {
        if (containerSuccess != null) containerSuccess.style.display = DisplayStyle.Flex;
        if (background != null) background.style.display = DisplayStyle.Flex;
    }

    private void HideSuccessContainer()
    {
        if (containerSuccess != null) containerSuccess.style.display = DisplayStyle.None;
        if (background != null) background.style.display = DisplayStyle.None;
    }

    private void OnCancelButtonClicked() => HideConfirmationContainer();
    private void OnOkButtonClicked()
    {
        HideSuccessContainer();
        HideBackground();
    }
    private void ShowDupeContainer()
    {
        if (containerDupe != null) containerDupe.style.display = DisplayStyle.Flex;  // Show the "duplicate" container
        if (background != null) background.style.display = DisplayStyle.Flex;  // Show the background
    }
    private void HideDupeContainer()
    {
        if (containerDupe != null) containerDupe.style.display = DisplayStyle.None;  // Hide the "duplicate" container
    }

    private void HideBackground()
    {
        if (background != null) background.style.display = DisplayStyle.None;
    }

    private void ShowContainerAfford()
    {
        if (containerAfford != null) containerAfford.style.display = DisplayStyle.Flex;  // Show the "cannot afford" container
        if (background != null) background.style.display = DisplayStyle.Flex;  // Show the background
    }

    private void HideContainerAfford()
    {
        if (containerAfford != null) containerAfford.style.display = DisplayStyle.None;  // Hide the "cannot afford" container
    }
    void Update()
    {
        // Update logic if needed
    }
}
