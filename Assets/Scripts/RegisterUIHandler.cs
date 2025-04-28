using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System;


public class RegisterUIHandler : MonoBehaviour
{
    [SerializeField] private LoginManager loginManager;  // Referencia al LoginManager
    private VisualElement root;
    private Button backButton;
    private Button registerButton;
    private TextField userTextField;
    private TextField passwordTextField;
    private TextField firstNameTextField;
    private TextField lastNameTextField;
    private TextField dayTextField;
    private TextField monthTextField;
    private TextField yearTextField;
    private DropdownField genderDropdown;
    private DropdownField countryDropdown;
    private DropdownField occupationDropdown;
    private string url = Variables.Variables.url;
    public struct Register
    {
        public string user;
        public string password;
        public string firstName;
        public string lastName;
        public int day;
        public int month;
        public int year;
        public string dateOfBirth;
        public string gender;
        public string country;
        public string occupation;
    }

    private Label errorMessageLabel;


    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        userTextField = root.Q<TextField>("user");
        passwordTextField = root.Q<TextField>("password");
        firstNameTextField = root.Q<TextField>("firstName");
        lastNameTextField = root.Q<TextField>("lastName");
        dayTextField = root.Q<TextField>("day");
        monthTextField = root.Q<TextField>("month");
        yearTextField = root.Q<TextField>("year");
        genderDropdown = root.Q<DropdownField>("gender");
        countryDropdown = root.Q<DropdownField>("country");
        occupationDropdown = root.Q<DropdownField>("occupation");
        errorMessageLabel = root.Q<Label>("errorMessage");


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
        loginManager.ShowLoginUI();
    }

    private void HighlightField(VisualElement ve)
    {
        ve.style.borderBottomColor = Color.red;
        ve.style.borderTopColor = Color.red;
        ve.style.borderLeftColor = Color.red;
        ve.style.borderRightColor = Color.red;

        ve.style.borderBottomWidth = 2;
        ve.style.borderTopWidth = 2;
        ve.style.borderLeftWidth = 2;
        ve.style.borderRightWidth = 2;

        ve.style.backgroundColor = new StyleColor(new Color(1f, 0.95f, 0.95f)); // Light red background
    }

    private void ResetFieldStyles()
    {
        void Reset(VisualElement ve)
        {
            ve.style.borderBottomColor = Color.black;
            ve.style.borderTopColor = Color.black;
            ve.style.borderLeftColor = Color.black;
            ve.style.borderRightColor = Color.black;

            ve.style.borderBottomWidth = 1;
            ve.style.borderTopWidth = 1;
            ve.style.borderLeftWidth = 1;
            ve.style.borderRightWidth = 1;

            ve.style.backgroundColor = Color.white;
        }

        Reset(userTextField);
        Reset(passwordTextField);
        Reset(firstNameTextField);
        Reset(lastNameTextField);
        Reset(dayTextField);
        Reset(monthTextField);
        Reset(yearTextField);
        Reset(genderDropdown);
        Reset(countryDropdown);
        Reset(occupationDropdown);
    }


    private void OnRegisterButtonClicked()
    {
        // Limpiar estilos anteriores
        ResetFieldStyles();
        errorMessageLabel.text = "";

        string user = userTextField.value;
        string password = passwordTextField.value;
        string firstName = firstNameTextField.value;
        string lastName = lastNameTextField.value;
        int day = int.TryParse(dayTextField.value, out int parsedDay) ? parsedDay : 0;
        int month = int.TryParse(monthTextField.value, out int parsedMonth) ? parsedMonth : 0;
        int year = int.TryParse(yearTextField.value, out int parsedYear) ? parsedYear : 0;

        string gender = genderDropdown.value;
        string country = countryDropdown.value;
        string occupation = occupationDropdown.value;

        bool hasEmptyFields = false;

        if (string.IsNullOrWhiteSpace(user)) { HighlightField(userTextField); hasEmptyFields = true; }
        if (string.IsNullOrWhiteSpace(password)) { HighlightField(passwordTextField); hasEmptyFields = true; }
        if (string.IsNullOrWhiteSpace(firstName)) { HighlightField(firstNameTextField); hasEmptyFields = true; }
        if (string.IsNullOrWhiteSpace(lastName)) { HighlightField(lastNameTextField); hasEmptyFields = true; }
        if (string.IsNullOrWhiteSpace(dayTextField.value)) { HighlightField(dayTextField); hasEmptyFields = true; }
        if (string.IsNullOrWhiteSpace(monthTextField.value)) { HighlightField(monthTextField); hasEmptyFields = true; }
        if (string.IsNullOrWhiteSpace(yearTextField.value)) { HighlightField(yearTextField); hasEmptyFields = true; }
        if (string.IsNullOrWhiteSpace(gender)) { HighlightField(genderDropdown); hasEmptyFields = true; }
        if (string.IsNullOrWhiteSpace(country)) { HighlightField(countryDropdown); hasEmptyFields = true; }
        if (string.IsNullOrWhiteSpace(occupation)) { HighlightField(occupationDropdown); hasEmptyFields = true; }

        if (hasEmptyFields)
        {
            errorMessageLabel.text = "Please fill in all required fields.";
            return;
        }
        Debug.Log($"Parsed birth date: {parsedDay}-{parsedMonth}-{parsedYear}");

        if (!IsValidDate(day, month, year, out DateTime birthDate))
        {
            errorMessageLabel.text = "Invalid date. Please check the day, month, and year.";
            return;
        }

        if (birthDate > DateTime.Today)
        {
            errorMessageLabel.text = "Date of birth cannot be in the future.";
            return;
        }

        string dateOfBirth = $"{year:D4}-{month:D2}-{day:D2}";
        StartCoroutine(RegisterNewUser(user, password, firstName, lastName, dateOfBirth, gender, country, occupation));
    }



    private bool IsValidDate(int day, int month, int year, out DateTime date)
    {
        try
        {
            date = new DateTime(year, month, day);
            return true;
        }
        catch
        {
            Debug.LogError($"Invalid date: Day={day}, Month={month}, Year={year}. Please check the input values.");
            date = DateTime.MinValue;
            return false;
        }
    }

    private IEnumerator RegisterNewUser(string user, string password, string firstName, string lastName, string dateOfBirth, string gender, string country, string occupation)
    {
        Register reg = new Register
        {
            user = user,
            password = password,
            firstName = firstName,
            lastName = lastName,
            dateOfBirth = dateOfBirth,
            gender = gender,
            country = country,
            occupation = occupation
        };

        string JsonRegister = JsonConvert.SerializeObject(reg);
        UnityWebRequest webRequest = UnityWebRequest.Post($"{url}/register", JsonRegister, "application/json");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Dictionary<string, string> response = JsonConvert.DeserializeObject<Dictionary<string, string>>(webRequest.downloadHandler.text);
            if (response["result"] == "usuarioExiste")
            {
                errorMessageLabel.text = "This username is already registered. Please choose another.";
                HighlightField(userTextField);
            }
            else if (response["result"] == "contrasenaInvalida")
            {
                errorMessageLabel.text = "Password must be at least 8 characters long, include one uppercase, one lowercase, and one number.";
                HighlightField(passwordTextField);
            }
            else if (response["result"] == "registroExitoso")
            {
                Debug.Log("User registered successfully.");
                errorMessageLabel.style.color = Color.green; // cambia color del mensaje a verde
                errorMessageLabel.text = "Registration successful!";
                yield return new WaitForSeconds(2f); // espera 2 segundos
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
        "Student",
        "Working Professional",
        "Entrepreneur / Business Owner",
        "Academic / Researcher",
        "Teacher / Educator",
        "Creative / Artist / Content Creator",
        "Homemaker / Caregiver",
        "Unemployed / Between Jobs",
        "Career Transition / Reskilling",
        "Retired",
        "Other / Prefer not to say"
    };
    }
}
