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
    
    private static CryptoMine instance;

    
    // Utility variables
    private Dictionary<string, Func<double, double>> powerUps;
    private System.Random randomizer = new System.Random();
    private string[] powerUpNames;
    [SerializeField]
    GameObject flyingCoins;
    [SerializeField]
    VisualTreeAsset powerUpTemplate;
     


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
    private string[] activeUserPowerUps;
    private  DateTime sessionStart = DateTime.Now;

    
    // Units
    private double hashReward = 1;
    private int hashProbability = 5;
    private int hashPool = 200;

    // UI Elements
    private UIDocument game;
    private VisualElement root;
        // HUD Elements
        private Label cryptoMinedLabel;
        private Label blocksMinedLabel;
        private Label totalScoreLabel;

        // Main Container Elements
        private VisualElement powerUp1;
        private VisualElement powerUp2;
        private VisualElement powerUp3;
        
        private DropdownField cryptoRigDropdown;
        private Button changePowerUp1;
        private Button changePowerUp2;
        private Button changePowerUp3;
        private Button mineButton;

        // Drill
        private Label hashLabel;

        // PowerUp Selection
        private VisualElement powerUpSelectorContainer;
        private VisualElement selectPowerUpContainer;
        private Button exitButton;

    // Web
    private int userId;
    private string url = Variables.Variables.url;
    private bool userPowerUpsLoaded = false;
    private Coroutine AutoMineCoroutine;

    // Variable to check if CryptoMine scene is on screen
    private bool isCryptoMineSceneActive = true;

    public struct PowerUp
    {
        public string nombre;
        public string descripcion;
    }
    public struct GameSession
    {
        public double TKNs;
        public DateTime startSession;
        public DateTime endSession;
        public double minedBlocks;
        public int clicks;
        public int score;
    }
    static PowerUp[] userPowerUps;
    static GameSession session;
    static bool isInitialized = false;

    void Awake()
    {
        //userId = PlayerPrefs.GetInt("UserId", 1);
        userId = 1;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return; // ¡Muy importante salir si destruyes!
        }
    }
    
    void InitUI()
    {
        if (CoroutineRunner.Instance != null)
        
        StartCoroutine(WaitThenRun(() => WaitForUIDocument()));

        game = UIDocumentManager.Instance.uiDocument;
        root = game.rootVisualElement;
        
        print($"IsActive: {gameObject.activeInHierarchy}, IsEnabled: {this.enabled}");
       
        print("Initializing CryptoMine");
        session.startSession = DateTime.Now;
        
        StartCoroutine(WaitThenRun(() => LoadUserData()));
        StartCoroutine(WaitThenRun(() => RetrieveUserPowerUps()));
        print($"IsActive: {gameObject.activeInHierarchy}, IsEnabled: {this.enabled}");
    

        SceneManager.sceneLoaded += OnSceneLoaded;

    // Initializing uxml variables

        // HUD Elements
        cryptoMinedLabel = root.Q<Label>("CryptoMinedValue");
        blocksMinedLabel = root.Q<Label>("BlocksMinedValue");
        totalScoreLabel = root.Q<Label>("TotalScoreValue");

        
        // PowerUp Selection
        exitButton = game.rootVisualElement.Q<Button>("ExitWindow");
        exitButton.RegisterCallback<ClickEvent>(evt => {
            powerUpSelectorContainer.style.display = DisplayStyle.None;
            exitButton.style.display = DisplayStyle.None;
            print("Exit");
        });

        // Main Container Elements
        powerUp1 = root.Q<VisualElement>("PowerUp1");
        powerUp2 = root.Q<VisualElement>("PowerUp2");
        powerUp3 = root.Q<VisualElement>("PowerUp3");
        changePowerUp1 = root.Q<Button>("ChangePowerUp1");
        changePowerUp1.RegisterCallback<ClickEvent>(evt => {
            showPowerUpInfo(0);
        });
        changePowerUp2 = root.Q<Button>("ChangePowerUp2");
        changePowerUp2.RegisterCallback<ClickEvent>(evt => {
            showPowerUpInfo(1);
        });
        changePowerUp3 = root.Q<Button>("ChangePowerUp3");
        changePowerUp3.RegisterCallback<ClickEvent>(evt => {
            showPowerUpInfo(2);
        });
        
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
        

        activeUserPowerUps = PlayerPrefs.GetString("UserPowerUps", "") == "" ? new string[3] { "", "", "" } : PlayerPrefs.GetString("UserPowerUps", "").Split('-');

        hashAttempts = new HashSet<int>();
        hashTarget = new HashSet<int>(); 
        GenerateHashTarget();

        cryptoRigDropdown.RegisterCallback<ChangeEvent<string>>(evt => {
            if (evt.newValue == "GPU")
            {
                PlayerPrefs.SetInt("AutoMine", 1);
                AutoMineCoroutine = CoroutineRunner.Instance.StartCoroutine(AutoMine(0.5f));
                print("AutoMine Initiated");
            }
            else
            {
                StopCoroutine(AutoMineCoroutine);
                AutoMineCoroutine = null;
            }
        });
        
        cryptoRigDropdown.choices = new List<string> { "CPU", "GPU" };
        cryptoRigDropdown.value = PlayerPrefs.GetString("CryptoRig", "CPU");
        
        cryptoMinedLabel.text = totalMinedCrypto.ToString("F5");
        blocksMinedLabel.text = totalMinedBlocks.ToString();
        totalScoreLabel.text = score.ToString() + "pts";
        print("Init completo");
        
    }
    private IEnumerator Start()
{
    Debug.Log("🟡 Esperando CoroutineRunner y UIDocumentManager...");

    while (CoroutineRunner.Instance == null)
    {
        Debug.Log("⏳ CoroutineRunner.Instance aún es null");
        yield return null;
    }

    while (UIDocumentManager.Instance == null)
    {
        Debug.Log("⏳ UIDocumentManager.Instance aún es null");
        yield return null;
    }

    while (UIDocumentManager.Instance.GetUIDocument() == null)
    {
        Debug.Log("⏳ UIDocument aún no asignado");
        yield return null;
    }

    while (UIDocumentManager.Instance.GetUIDocument().rootVisualElement.Q<Label>("CryptoMinedValue") == null)
    {
        Debug.Log("⏳ El árbol visual aún no tiene CryptoMinedValue");
        yield return null;
    }

    Debug.Log("✅ Todo está listo");

    this.game = UIDocumentManager.Instance.GetUIDocument();
    this.root = game.rootVisualElement;

    InitUI();

    CoroutineRunner.Instance.StartCoroutine(LoadUserData());
    CoroutineRunner.Instance.StartCoroutine(RetrieveUserPowerUps());
}
    private System.Collections.IEnumerator WaitForUIDocument()
    {
        // Espera a que el singleton esté disponible
        while (UIDocumentManager.Instance == null || UIDocumentManager.Instance.uiDocument.rootVisualElement == null)
        {
            yield return null;
        }

        var root = UIDocumentManager.Instance.uiDocument.rootVisualElement;
        // Tu lógica aquí
    }

    void showPowerUpInfo(int index)
    {
        powerUpSelectorContainer = game.rootVisualElement.Q<VisualElement>("PowerUpSelectorContainer");
        selectPowerUpContainer = game.rootVisualElement.Q<VisualElement>("PowerUpSelector");
        
        
        if (selectPowerUpContainer.childCount > 0)
        {
            selectPowerUpContainer.Clear();
        }
        if (userPowerUps == null)
        {
            Debug.LogWarning("User power-ups null");
            return;
        }
        if (userPowerUps.Length == 0)
        {
            Debug.LogWarning("User power-ups empty");
            return;
        }
        foreach (PowerUp powerUp in userPowerUps)
        {
            var item = powerUpTemplate.CloneTree();
            
            item.Q<Label>("PowerUpName").text = powerUp.nombre;
            item.Q<Label>("PowerUpDescription").text = powerUp.descripcion;
            item.Q<VisualElement>("PowerUpIcon").style.backgroundImage = new StyleBackground(Resources.Load<Sprite>($"PowerUps/{powerUp.nombre}"));
            item.Q<Button>("Activate").RegisterCallback<ClickEvent>(evt => {
                if (index == 0)
                {
                    powerUp1.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>($"PowerUps/{powerUp.nombre}"));
                }
                else if (index == 1)
                {
                    powerUp2.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>($"PowerUps/{powerUp.nombre}"));
                }
                else
                {
                    powerUp3.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>($"PowerUps/{powerUp.nombre}"));
                }
                activeUserPowerUps[index] = powerUp.nombre;
            });
            selectPowerUpContainer.Add(item);  
        }
        exitButton.style.display = DisplayStyle.Flex;
        selectPowerUpContainer.style.display = DisplayStyle.Flex;
        powerUpSelectorContainer.style.display = DisplayStyle.Flex;

    }
   

    private IEnumerator WaitThenRun(Func<IEnumerator> coroutineToRun)
    {
        yield return new WaitUntil(() => CoroutineRunner.Instance != null);
        CoroutineRunner.Instance.StartCoroutine(coroutineToRun());
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
            points = powerUps[activeUserPowerUps[index]](points);
            return ApplyPowerUps(--index, points);
        }
    }
    private double ApplyPowerUps(double points)
    {
        int powerUpCount = 0;
        foreach (string powerUp in activeUserPowerUps)
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
        if (isCryptoMineSceneActive)
        {
            GameObject.Find("MineSound").GetComponent<AudioSource>().Play();
            print("Mining");
        }
        if (hashTarget.Contains(hashUser))
        {
            if (isCryptoMineSceneActive)
            {
                CoroutineRunner.Instance.StartCoroutine(ShowPink());
                flyingCoins.GetComponent<ParticleSystem>().Play();
                GameObject.Find("SoundCoins").GetComponent<AudioSource>().Play();
                print("Coins flying");
                
            }
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
    return new string(Enumerable.Repeat(chars, 12)
        .Select(s => chars[randomizer.Next(chars.Length)]).ToArray());
    }

    private IEnumerator AutoMine(float interval)
    {
        print("AutoMine started");
        while (PlayerPrefs.GetInt("AutoMine", 0) == 1)
        {
            yield return new WaitForSeconds(interval);
            print("AutoMining");
            MineCrypto();
        }
    }


    void OnApplicationQuit()
    {
        //PlayerPrefs.SetString("UserPowerUps", String.Join("-", activeUserPowerUps));
        //SaveUserPowerUps();
        SaveGameSession();
        print("Application quitting");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "CryptoMine")
        {
            //game.enabled = false; // Or rootVisualElement.visible = false
            root.style.display = DisplayStyle.None;
            isCryptoMineSceneActive = false;
        }
        else
        {
            root.style.display = DisplayStyle.Flex;
            isCryptoMineSceneActive = true;
            //game.enabled = true;
        }
    }

    // Server Functions

    private IEnumerator RetrieveUserPowerUps()
    {
        print("Retrieving user powerups");
        UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/cryptomine/retrieveUserPowerUps/{userId}");
        Debug.Log($"📦 Coroutine corriendo. gameObject activeInHierarchy: {gameObject.activeInHierarchy}, enabled: {enabled}");
        yield return webRequest.SendWebRequest();
        Debug.Log("✅ Después de yield (esta línea NO debería desaparecer)");
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            print("User powerups retrieved");
            string jsonString = webRequest.downloadHandler.text;
            print($"JSON: {jsonString}");
            userPowerUps = JsonConvert.DeserializeObject<PowerUp[]>(jsonString);
            userPowerUpsLoaded = true;

            foreach (var powerUp in userPowerUps) 
            {
                print($"PowerUp: {powerUp.nombre}, Description: {powerUp.descripcion}");
                
            } 
        }
        else
        {
            Debug.LogError($"Error retrieving available PowerUps: {webRequest.error}");
        }
    }

    private void SaveUserPowerUps()
    {
        // Used Copilot for entering the correct format to accept strings in a Post
        UnityWebRequest webRequest = UnityWebRequest.Post($"{url}/cryptomine/saveUserPowerUps/{userId}", string.Join("-", activeUserPowerUps), "application/x-www-form-urlencoded");
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

    private IEnumerator SaveGameSession()
    {
        session.TKNs = sessionMinedCrypto;
        session.minedBlocks = sessionMinedBlocks;
        session.endSession = DateTime.Now;
        session.clicks = sessionClicks;
        session.score = score;

        string jsonData = JsonConvert.SerializeObject(session);

        UnityWebRequest webRequest = UnityWebRequest.Post($"{url}/cryptomine/saveSession/{userId}", jsonData, "application/json");
        print($"Sending JSON: {jsonData}");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            print("Session saved succesfully.");
        }
        else
        {
            Debug.LogError($"Error saving game session: {webRequest.error}");
        }
    }

    private IEnumerator LoadUserData()
{
    print("Loading user data");
    UnityWebRequest webRequest = UnityWebRequest.Get($"{url}/cryptomine/loadUserData/{userId}");

    yield return webRequest.SendWebRequest();
    print($"WebRequest Result: {webRequest.result}");

    switch (webRequest.result)
    {
        case UnityWebRequest.Result.Success:
            string userData = webRequest.downloadHandler.text;
            print($"Response data: {userData}");

            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(userData);
            foreach (var key in data.Keys)
                print($"{key}: {data[key]}");

            if (data.ContainsKey("TKNs"))
                totalMinedCrypto = System.Convert.ToDouble(data["TKNs"]);
                cryptoMinedLabel.text = totalMinedCrypto.ToString("F5");
            if (data.ContainsKey("totalBloquesMinados"))
                totalMinedBlocks = System.Convert.ToInt32(data["totalBloquesMinados"]);
                blocksMinedLabel.text = totalMinedBlocks.ToString();
            if (data.ContainsKey("puntajeTotal"))
                score = System.Convert.ToInt32(data["puntajeTotal"]);
                totalScoreLabel.text = score.ToString() + "pts";
            break;

        case UnityWebRequest.Result.ConnectionError:
        case UnityWebRequest.Result.ProtocolError:
        case UnityWebRequest.Result.DataProcessingError:
            Debug.LogError($"Request error: {webRequest.error}");
            break;
    }
}
private IEnumerator DelayedLoadUserData()
{
    yield return new WaitForSeconds(0.1f); // espera de 100 ms
    CoroutineRunner.Instance.StartCoroutine(LoadUserData());
}
private IEnumerator DelayedRetrieveUserPowerUps()
{
    yield return new WaitForSeconds(0.1f); // espera de 100 ms
    CoroutineRunner.Instance.StartCoroutine(RetrieveUserPowerUps());
}
    private IEnumerator ShowPink()
    {
        var pink = game.rootVisualElement.Q<VisualElement>("CorrectColor");
        pink.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(.2f);
        pink.style.display = DisplayStyle.None;
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
