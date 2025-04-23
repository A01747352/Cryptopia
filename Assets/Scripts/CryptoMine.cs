using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Newtonsoft.Json;


public class CryptoMine : MonoBehaviour
{
    
    private CryptoMine instance;

    // Utility variables
    private Dictionary<string, Func<double, double>> powerUps;
    private System.Random randomizer = new System.Random();
    private string[] powerUpNames;

    // User Variables
    private double sessionMinedCrypto = 0;
    private double totalMinedCrypto;
    private int sessionMinedBlocks = 0;
    private int totalMinedBlocks;
    private int pointsPerBlock = 100;
    private int score = 0;
    private int totalClicks;
    private int sessionClicks = 0;
    private HashSet<int> hashTarget;
    private HashSet<int> hashAttempts;
    private int hashUser;
    private string[] userPowerUps;
    private  DateTime sessionStart = DateTime.Now;

    
    // Units
    private double hashReward = 1;
    private int hashProbability = 5;
    private int hashPool = 200;

    // UI Elements
    private UIDocument game;
        // HUD Elements
        private Label cryptoMinedLabel;
        private Label blocksMinedLabel;
        private Label totalScoreLabel;

        // Main Container Elements
        private VisualElement powerUp1;
        private VisualElement powerUp2;
        private VisualElement powerUp3;
        private DropdownField powerUp1Dropdown;
        private DropdownField powerUp2Dropdown;
        private DropdownField powerUp3Dropdown;
        private DropdownField cryptoRigDropdown;
        private Button mineButton;

        // Drill
        private Label hashLabel;

    // Web
    private int userId;
    private string url = "http://localhost:8080";


    public struct GameSession
    {
        public double TKNs;
        public DateTime startSession;
        public DateTime endSession;
        public double minedBlocks;
        public int clicks;
        public int score;
    }
    static GameSession session;

    void Awake()
    {
        userId = PlayerPrefs.GetInt("UserId", 0);
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates on scene load
        }
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        session.startSession = DateTime.Now;
        LoadUserData();

        game = GetComponent<UIDocument>();
        VisualElement root = game.rootVisualElement;

    // Initializing uxml variables

        // HUD Elements
        cryptoMinedLabel = root.Q<Label>("CryptoMinedValue");
        blocksMinedLabel = root.Q<Label>("BlocksMinedValue");
        totalScoreLabel = root.Q<Label>("TotalScoreValue");

        cryptoMinedLabel.text = totalMinedCrypto.ToString("F5");
        blocksMinedLabel.text = totalMinedBlocks.ToString();
        totalScoreLabel.text = score.ToString() + "pts";

        // Main Container Elements
        powerUp1 = root.Q<VisualElement>("PowerUp1");
        powerUp2 = root.Q<VisualElement>("PowerUp2");
        powerUp3 = root.Q<VisualElement>("PowerUp3");
        powerUp1Dropdown = root.Q<DropdownField>("PowerUp1Dropdown");
        powerUp2Dropdown = root.Q<DropdownField>("PowerUp2Dropdown");
        powerUp3Dropdown = root.Q<DropdownField>("PowerUp3Dropdown");
        cryptoRigDropdown = root.Q<DropdownField>("CryptoRigDropdown");
        

        // Drill
        mineButton = root.Q<Button>("MineButton");
        mineButton.RegisterCallback<ClickEvent>(MineCrypto);
        hashLabel = root.Q<Label>("HashLabel");

        // Game Variables
        // minedCrypto = 0;
        // totalBlocks = 0;
        // score = 0;
        powerUps = new Dictionary<string, Func<double, double>>()
        {
            {"DoubleReward", DoubleReward},
            {"EveryFifth", EveryFifth},
            {"Plus10", Plus10},
            {"BitOfBitcoin", BitOfBitcoin},
            {"TenfoldXP", TenfoldXP},
            {"TripleReward", TripleReward}
        };
        powerUp1Dropdown.choices = powerUps.Keys.ToList();
        powerUp2Dropdown.choices = powerUps.Keys.ToList();
        powerUp3Dropdown.choices = powerUps.Keys.ToList();

        userPowerUps = PlayerPrefs.GetString("UserPowerUps", "") == "" ? new string[3] { "", "", "" } : PlayerPrefs.GetString("UserPowerUps", "").Split('-');
        StartCoroutine(RetrieveUserPowerUps());

        cryptoRigDropdown.choices = new List<string> { "CPU", "GPU" };
        cryptoRigDropdown.value = PlayerPrefs.GetString("CryptoRig", "CPU");

        hashAttempts = new HashSet<int>();
        hashTarget = new HashSet<int>(); 
        GenerateHashTarget();


        powerUp1Dropdown.RegisterCallback<ChangeEvent<string>>(evt => SelectPowerUp(evt, 0));
        powerUp2Dropdown.RegisterCallback<ChangeEvent<string>>(evt => SelectPowerUp(evt, 1));
        powerUp3Dropdown.RegisterCallback<ChangeEvent<string>>(evt => SelectPowerUp(evt, 2));

        cryptoRigDropdown.RegisterCallback<ChangeEvent<string>>(evt => {
            if (evt.newValue == "GPU")
            {
                StartCoroutine(AutoMine(0.5f));
                PlayerPrefs.SetInt("AutoMine", 1);
            }
            else
            {
                PlayerPrefs.SetInt("AutoMine", 0);
                //StopAllCoroutines();
            }
        });

        

        //StartCoroutine(AutoMine(0.5f));

    }

    private void SelectPowerUp(ChangeEvent<string> evt, int index)
    {
        if (index == 0)
        { 
            powerUp1Dropdown.value = evt.newValue; 
        }
        else if (index == 1)
        {
            powerUp2Dropdown.value = evt.newValue;
        }
        else
        {
            powerUp3Dropdown.value = evt.newValue;
        }
       
        userPowerUps[index] = evt.newValue;
    }

    private double ApplyPowerUps(int index, double points)
    {
        if (index == -1)
        {
            return points;
        }
        else
        {   
            print("AplliedPowerups");
            points = powerUps[userPowerUps[index]](points);
            return ApplyPowerUps(--index, points);
        }
    }
    private double ApplyPowerUps(double points)
    {
        int powerUpCount = 0;
        foreach (string powerUp in userPowerUps)
        {
            if (powerUp != "")
            {
                ++powerUpCount;
            }

        }
        return ApplyPowerUps(powerUpCount-1, points);
    }
    private void MineCrypto()
    {
        ++totalClicks;
        GenerateUserHash();
        hashLabel.text =  GenerateRandomString();
        if (hashTarget.Contains(hashUser))
        {
            double crypto = ApplyPowerUps(hashReward);
            sessionMinedCrypto += crypto;
            totalMinedCrypto += crypto;
            PlayerPrefs.SetFloat("TKNs", PlayerPrefs.GetFloat("TKNs", 0) + (float)crypto);
            hashAttempts.Clear();
            GenerateHashTarget();
            ++sessionMinedBlocks;
            ++totalMinedBlocks;

            score += pointsPerBlock;
            PlayerPrefs.SetInt("TotalScore", PlayerPrefs.GetInt("TotalScore", 0) + pointsPerBlock);
            blocksMinedLabel.text = totalMinedBlocks.ToString();
            totalScoreLabel.text = score.ToString() + "pts";
        }
        else
        {
            hashAttempts.Add(hashUser);
        }
        print(hashUser);
        cryptoMinedLabel.text = totalMinedCrypto.ToString("F5");
    }



    private void MineCrypto(ClickEvent evt)
    {
        MineCrypto();
    }

   private void GenerateHashTarget()
    {
        hashTarget.Clear();
        while (hashTarget.Count < hashProbability)
        {
            hashTarget.Add(randomizer.Next(0, hashPool));
        }
        Debug.LogWarning($"Generated Hash Target: {string.Join(", ", hashTarget)}");
    }
    
    private void GenerateUserHash()
    {
        while (hashAttempts.Contains(hashUser) == true)
        {
            hashUser = randomizer.Next(0, hashPool);
        }
    }
    // Generated by Copilot
    private string GenerateRandomString()
    {
    const string chars = "abcdefghijklmnopqrstuvwxyz0123456789012345678901234567890123456789";
    return new string(Enumerable.Repeat(chars, 8)
        .Select(s => chars[randomizer.Next(chars.Length)]).ToArray());
    }

    private IEnumerator AutoMine(float interval)
    {
        while (PlayerPrefs.GetInt("AutoMine", 0) == 1)
        {
            yield return new WaitForSeconds(interval);
            MineCrypto();
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("UserPowerUps", String.Join("-", userPowerUps));
        //SaveUserPowerUps();
        SaveGameSession();
        print("Application quitting");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "CryptoMine")
        {
            game.enabled = false; // Or rootVisualElement.visible = false
        }
        else
        {
            game.enabled = true;
        }
    }

    // Server Functions

    private IEnumerator RetrieveUserPowerUps()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/cryptomine/retrieveUserPowerUps/{userId}");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            powerUp1Dropdown.choices = webRequest.downloadHandler.text.Split('-').ToList();
            powerUp2Dropdown.choices = webRequest.downloadHandler.text.Split('-').ToList();
            powerUp3Dropdown.choices = webRequest.downloadHandler.text.Split('-').ToList();
            
        }
        else
        {
            Debug.LogError($"Error retrieving available PowerUps: {webRequest.error}");
        }
    }

    private void SaveUserPowerUps()
    {
        // Used Copilot for entering the correct format to accept strings in a Post
        UnityWebRequest webRequest = UnityWebRequest.Post($"{url}/cryptomine/saveUserPowerUps/{userId}", string.Join("-", userPowerUps), "application/x-www-form-urlencoded");
        webRequest.SendWebRequest();

        while (!webRequest.isDone) { }
        
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            print("User variables saved succesfully.");
        }
        else
        {
            Debug.LogError($"Error saving user variables: {webRequest.error}");
        }
    }

    private void SaveGameSession()
    {
        session.TKNs = sessionMinedCrypto;
        session.minedBlocks = sessionMinedBlocks;
        session.endSession = DateTime.Now;
        session.clicks = sessionClicks;
        session.score = score;

        string jsonData = JsonConvert.SerializeObject(session);

        UnityWebRequest webRequest = UnityWebRequest.Post($"{url}/cryptomine/saveSession/{userId}", jsonData, "application/json");
        webRequest.SendWebRequest();

        while (!webRequest.isDone) { }

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            print("Session saved succesfully.");
        }
        else
        {
            Debug.LogError($"Error saving game session: {webRequest.error}");
        }
    }

    private void LoadUserData()
    {
        print("Loading user data");
        UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/cryptomine/loadUserData/{userId}");
        webRequest.SendWebRequest();
        
        while (!webRequest.isDone) { }
        
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string userData = webRequest.downloadHandler.text;
            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(userData);

            foreach (var key in data.Keys)
            {
                print($"{key}: {data[key]}");
            }
            // Access the values by key
            if (data.ContainsKey("TKNs"))
            {
                totalMinedCrypto = System.Convert.ToDouble(data["TKNs"]);
            }
            if (data.ContainsKey("totalBloquesMinados"))
            {
                totalMinedBlocks = System.Convert.ToInt32(data["totalBloquesMinados"]);
            }
        }
        else
        {
            Debug.LogError($"Error loading user data: {webRequest.error}");
        }
    }

    // PowerUps
    private double DoubleReward(double points)
    {
        print("Rewarddoubled");
        return points * 2;
        
    }
    private double EveryFifth(double points)
    {
        print("Every Fifth activated");
        if (sessionMinedBlocks % 5 == 0)
        {
            sessionMinedCrypto += hashReward * 5;
            totalMinedCrypto += hashReward * 5;
            return points;
        }
        else
        {
            return points;
        }
    }
    private double Plus10(double points)
    {
        print("Plusten activated");
        sessionMinedCrypto += hashReward * .10;
        totalMinedCrypto += hashReward * .10;
        return points;
    }

    private double BitOfBitcoin(double points)
    {
        PlayerPrefs.SetFloat("BTC", PlayerPrefs.GetFloat("BTC", 0) + 0.0015f);
        return points;
    }

    private double TenfoldXP(double points)
    {
        score += 900;
        return points;
    }

    private double TripleReward(double points)
    {
        return points * 3;
    }

   
}
