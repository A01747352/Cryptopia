using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class RegisterUIHandler : MonoBehaviour
{
    [SerializeField] private LoginManager loginManager;  // Referencia al LoginManager

    private VisualElement root;
    private Button backButton;

    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        // Configurar los Dropdowns
        SetupDropdowns();

        // Configurar el botón de volver
        backButton = root.Q<Button>("backButton");
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
