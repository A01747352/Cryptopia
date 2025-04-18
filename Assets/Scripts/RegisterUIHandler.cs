using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;


public class RegisterUIHandler : MonoBehaviour
{
    [SerializeField] private LoginManager loginManager;  // Referencia al LoginManager
    private VisualElement root;
    private Button backButton;
    private Button registerButton;
    private TextField userTextField;
    private TextField passwordTextField;
    private TextField ageTextField;
    private DropdownField genderDropdown;
    private DropdownField countryDropdown;
    private DropdownField occupationDropdown;
    string url = "http://localhost:8080";
    public struct Register
    {
        public string user;
        public string password;
        public string age;
        public string gender;
        public string country;
        public string occupation;
    }

    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        userTextField = root.Q<TextField>("user");
        passwordTextField = root.Q<TextField>("password");
        ageTextField = root.Q<TextField>("ageF");
        genderDropdown = root.Q<DropdownField>("gender");
        countryDropdown = root.Q<DropdownField>("country");
        occupationDropdown = root.Q<DropdownField>("occupation");

        // Configurar los Dropdowns
        SetupDropdowns();

        // Configurar el botón de volver
        backButton = root.Q<Button>("backButton");
        registerButton = root.Q<Button>("registerButton");
        if (registerButton != null)
        {
            registerButton.clicked -= OnRegisterButtonClicked;
            registerButton.clicked += OnRegisterButtonClicked;
        }
        if (backButton != null)
        {
            backButton.clicked -= OnBackButtonClicked;
            backButton.clicked += OnBackButtonClicked;
        }
    }

    // Configurar los Dropdowns con las listas
    void SetupDropdowns()
    {
        var country = root.Q<DropdownField>("country");
        var gender = root.Q<DropdownField>("gender");
        var occupation = root.Q<DropdownField>("occupation");

        country.choices = GetCountryList();
        gender.choices = GetGenderList();
        occupation.choices = GetOccupationList();
    }

    // Lógica para regresar al LoginUI
    private void OnBackButtonClicked()
    {
        loginManager.ShowLoginUI();  // Usamos el LoginManager para cambiar a LoginUI
    }

    private void OnRegisterButtonClicked()
    {
        string user = userTextField.value;
        string password = passwordTextField.value;
        string age = ageTextField.value;
        string gender = genderDropdown.value;
        string country = countryDropdown.value;
        string occupation = occupationDropdown.value;
        StartCoroutine(RegisterNewUser(user, password, age, gender, country, occupation));
    }

    private IEnumerator RegisterNewUser(string user, string password, string age, string gender, string country, string occupation)
    {
        Register reg;
        reg.user = user;
        reg.password = password;
        reg.age = ageTextField.value;
        reg.gender = genderDropdown.value;
        reg.country = countryDropdown.value;
        reg.occupation = occupationDropdown.value; 

        string JsonRegister= JsonConvert.SerializeObject(reg);
        UnityWebRequest webRequest = UnityWebRequest.Post($"{url}/register", JsonRegister, "application/json");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Dictionary<string, string> response = JsonConvert.DeserializeObject<Dictionary<string, string>>(webRequest.downloadHandler.text);
            if (response["result"] == "usuarioExiste")
            {
                Debug.LogError("User already exists.");
            }
            else if (response["result"] == "contrasenaInvalida")
            {
                Debug.LogError("Password is invalid. Must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, and one number.");
            }
            else if (response["result"] == "registroExitoso")
            {
                Debug.Log("User registered successfully.");
                OnBackButtonClicked();
            }
            else
            {
                Debug.LogError("Registration failed: Unknown error.");
                Debug.LogError($"Error: {webRequest.error}");
            }    
        }
    }

    // Métodos para llenar los Dropdowns
    List<string> GetCountryList()
    {
        return new List<string> {
            "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Argentina", "Armenia",
            "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados",
            "Belarus", "Belgium", "Belize", "Benin", "Bhutan", "Bolivia", "Bosnia and Herzegovina",
            "Botswana", "Brazil", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cambodia",
            "Cameroon", "Canada", "Cape Verde", "Central African Republic", "Chad", "Chile", "China",
            "Colombia", "Comoros", "Costa Rica", "Croatia", "Cuba", "Cyprus", "Czech Republic",
            "Democratic Republic of the Congo", "Denmark", "Dominican Republic", "Ecuador", "Egypt",
            "El Salvador", "Estonia", "Ethiopia", "Fiji", "Finland", "France", "Gabon", "Gambia",
            "Georgia", "Germany", "Ghana", "Greece", "Guatemala", "Guinea", "Haiti", "Honduras",
            "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Israel", "Italy",
            "Jamaica", "Japan", "Jordan", "Kazakhstan", "Kenya", "Kuwait", "Laos", "Latvia",
            "Lebanon", "Liberia", "Libya", "Lithuania", "Luxembourg", "Madagascar", "Malaysia",
            "Maldives", "Mali", "Malta", "Mauritania", "Mauritius", "Mexico", "Moldova",
            "Monaco", "Mongolia", "Montenegro", "Morocco", "Mozambique", "Myanmar", "Namibia",
            "Nepal", "Netherlands", "New Zealand", "Nicaragua", "Niger", "Nigeria", "North Korea",
            "North Macedonia", "Norway", "Oman", "Pakistan", "Panama", "Paraguay", "Peru",
            "Philippines", "Poland", "Portugal", "Qatar", "Romania", "Russia", "Rwanda", "Saudi Arabia",
            "Senegal", "Serbia", "Singapore", "Slovakia", "Slovenia", "Somalia", "South Africa",
            "South Korea", "Spain", "Sri Lanka", "Sudan", "Suriname", "Sweden", "Switzerland",
            "Syria", "Taiwan", "Tajikistan", "Tanzania", "Thailand", "Togo", "Trinidad and Tobago",
            "Tunisia", "Turkey", "Turkmenistan", "Uganda", "Ukraine", "United Arab Emirates",
            "United Kingdom", "United States", "Uruguay", "Uzbekistan", "Vatican City", "Venezuela",
            "Vietnam", "Yemen", "Zambia", "Zimbabwe", "Other"
        };
    }

    List<string> GetGenderList()
    {
        return new List<string> {
            "Male", "Female", "Other"
        };
    }

    List<string> GetOccupationList()
    {
        return new List<string> {
            "Student", "Developer", "Engineer", "Designer", "Entrepreneur",
            "Researcher", "Artist", "Content Creator", "Educator", "Marketer",
            "Community Manager", "Project Manager", "Investor", "Data Analyst",
            "Blockchain Specialist", "Cybersecurity Specialist", "Product Manager",
            "Unemployed", "Other"
        };
    }
}
