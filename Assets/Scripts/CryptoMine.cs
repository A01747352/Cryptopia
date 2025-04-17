using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;

public class CryptoMine : MonoBehaviour
{
    
    private CryptoMine instance;

    // Utility variables
    private Dictionary<string, Func<double, double>> powerUps;
    private System.Random randomizer = new System.Random();
    private string[] powerUpNames;

    // User Variables
    private double minedCrypto;
    private int totalBlocks;
    private int score;
    private int totalClicks;
    private HashSet<int> hashTarget;
    private HashSet<int> hashAttempts;
    private int hashUser;
    
    private string[] userPowerUps;
    
    private static bool rigTurnedOn = true;

    
    // Units
    private double hashReward = 1;
    private int hashProbability = 5;
    private int hashPool = 100;

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
        private Button mineButton;

        // Drill
        private Label hashLabel;


    void Awake()
    {
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

        game = GetComponent<UIDocument>();
        VisualElement root = game.rootVisualElement;

    // Initializing uxml variables

        // HUD Elements
        cryptoMinedLabel = root.Q<Label>("CryptoMinedValue");
        blocksMinedLabel = root.Q<Label>("BlocksMinedValue");
        totalScoreLabel = root.Q<Label>("TotalScoreValue");

        // Main Container Elements
        powerUp1 = root.Q<VisualElement>("PowerUp1");
        powerUp2 = root.Q<VisualElement>("PowerUp2");
        powerUp3 = root.Q<VisualElement>("PowerUp3");
        powerUp1Dropdown = root.Q<DropdownField>("PowerUp1Dropdown");
        powerUp2Dropdown = root.Q<DropdownField>("PowerUp2Dropdown");
        powerUp3Dropdown = root.Q<DropdownField>("PowerUp3Dropdown");
        

        // Drill
        mineButton = root.Q<Button>("MineButton");
        mineButton.RegisterCallback<ClickEvent>(MineCrypto);
        hashLabel = root.Q<Label>("HashLabel");

        // Game Variables
        minedCrypto = 0;
        totalBlocks = 0;
        score = 0;
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
        userPowerUps = new string[] {"", "", ""};
        powerUp1Dropdown.value = userPowerUps[0];
        powerUp2Dropdown.value = userPowerUps[1];
        powerUp3Dropdown.value = userPowerUps[2];


        hashAttempts = new HashSet<int>();
        hashTarget = new HashSet<int>(); 
        GenerateHashTarget();


        powerUp1Dropdown.RegisterCallback<ChangeEvent<string>>(evt => SelectPowerUp(evt, 0));
        powerUp2Dropdown.RegisterCallback<ChangeEvent<string>>(evt => SelectPowerUp(evt, 1));
        powerUp3Dropdown.RegisterCallback<ChangeEvent<string>>(evt => SelectPowerUp(evt, 2));

        PlayerPrefs.SetInt("AutoMine", 0);

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
            minedCrypto += ApplyPowerUps(hashReward);
            hashAttempts.Clear();
            GenerateHashTarget();
            ++totalBlocks;
            score += 100;
            blocksMinedLabel.text = totalBlocks.ToString();
            totalScoreLabel.text = score.ToString() + "pts";
        }
        else
        {
            hashAttempts.Add(hashUser);
        }
        print(hashUser);
        cryptoMinedLabel.text = minedCrypto.ToString("F5");
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


    // PowerUps
    private double DoubleReward(double points)
    {
        print("Rewarddoubled");
        return points * 2;
        
    }
    private double EveryFifth(double points)
    {
        print("Every Fifth activated");
        if (totalBlocks % 5 == 0)
        {
            minedCrypto += hashReward * 5;
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
        minedCrypto += hashReward * .10;
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
