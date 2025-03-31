using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

public class CryptoMine : MonoBehaviour
{
    
    // Variables for keeping track of the game
    private double totalCrypto;
    private int totalBlocks;
    private int score;
    private int totalClicks;
    private HashSet<int> hashTarget;
    private HashSet<int> hashAttempts;
    private int hashUser;
    private Dictionary<string, Func<double, double>> powerUps;
    private string[] userPowerUps;
    private System.Random randomizer = new System.Random();

    
    // Units
    private double hashReward = 1;
    private int hashProbability = 5;
    private int hashPool = 100;

    // UI Elements
    private UIDocument game;
        // HUD Elements
        private Label cryptoMinedValue;
        private Label blocksMinedValue;
        private Label totalScoreValue;

        // Main Container Elements
        private VisualElement powerUp1;
        private VisualElement powerUp2;
        private VisualElement powerUp3;
        private Button mineButton;


    void OnEnable()
    {
        game = GetComponent<UIDocument>();
        VisualElement root = game.rootVisualElement;

    // Initializing uxml variables

        // HUD Elements
        cryptoMinedValue = root.Q<Label>("CryptoMinedValue");
        blocksMinedValue = root.Q<Label>("BlocksMinedValue");
        totalScoreValue = root.Q<Label>("TOtalScoreValue");

        // Main Container Elements
        powerUp1 = root.Q<VisualElement>("PowerUp1");
        powerUp2 = root.Q<VisualElement>("PowerUp2");
        powerUp3 = root.Q<VisualElement>("PowerUp3");

        mineButton = root.Q<Button>("MineButton");
        mineButton.RegisterCallback<ClickEvent>(MineCrypto);

        // Game Variables
        totalCrypto = 0;
        totalBlocks = 0;
        score = 0;
        powerUps = new Dictionary<string, Func<double, double>>()
        {
            {"DoubleTrouble", DoubleTrouble}
        };
        userPowerUps = new string[] {"DoubleTrouble"};
        hashAttempts = new HashSet<int>();
        hashTarget = new HashSet<int>(); 
        GenerateHashTarget();
        print(100 * 0.5);
    }

    

    private double ApplyPowerUps(int index, double points)
    {
        if (index == 0)
        {
            return points;
        }
        else
        {
            points = powerUps[userPowerUps[index]](points);
            return ApplyPowerUps(--index, points);
        }
    }
    private double ApplyPowerUps(double points)
    {
        return ApplyPowerUps(userPowerUps.Length-1, points);
    }
    private void MineCrypto()
    {
        ++totalClicks;
        GenerateUserHash();
        double potentialReward = ApplyPowerUps(hashReward);
        if (hashTarget.Contains(hashUser))
        {
            totalCrypto += potentialReward;
            hashAttempts.Clear();
            GenerateHashTarget();
        }
        else
        {
            hashAttempts.Add(hashUser);
        }
        print(hashUser);
        cryptoMinedValue.text = totalCrypto.ToString("F5");
    }
    private void MineCrypto(ClickEvent evt)
    {
        MineCrypto();
    }

    private double DoubleTrouble(double points)
    {
        return points*2;
    }
    private double EveryTenth(double points)
    {
        if (totalClicks % 10 ==0)
        {
            return points*10;
        }
        else
        {
            return points;
        }
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
}
