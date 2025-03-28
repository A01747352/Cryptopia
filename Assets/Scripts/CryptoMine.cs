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
    private Dictionary<string, Func<double, double>> powerUps;
    private string[] userPowerUps;

    
    // Units
    private double clickValue = 0.1;

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
            {"doubleTrouble", doubleTrouble}
        };
        userPowerUps = new string[] {"doubleTrouble"};
    }

    private void MineCrypto()
    {
    
        switch(userPowerUps.Length)
        {
            case 0:
                totalCrypto += clickValue;
                break;
            case 1:
                totalCrypto += powerUps[userPowerUps[0]](clickValue);
                break;
            
            case 2:
                totalCrypto += powerUps[userPowerUps[1]]
                                                (powerUps[userPowerUps[0]](clickValue));
                break;
            
            case 3:
                totalCrypto += powerUps[userPowerUps[2]]
                                    (powerUps[userPowerUps[1]]
                                                (powerUps[userPowerUps[0]](clickValue)));
                break;

            default:
                break;
        }
        
        
        cryptoMinedValue.text = totalCrypto.ToString();
    }
    private void MineCrypto(ClickEvent evt)
    {
        MineCrypto();
    }

    private double doubleTrouble(double points)
    {
        return points*2;
    }
}
