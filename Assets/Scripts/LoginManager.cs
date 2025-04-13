using UnityEngine;
using System;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private GameObject loginUIObject;     // GameObject con UIDocument de Login
    [SerializeField] private GameObject registerUIObject;  // GameObject con UIDocument de Register

    // Evento para cambiar a LoginUI
    public static event Action OnShowLoginUI;
    
    // Evento para cambiar a RegisterUI
    public static event Action OnShowRegisterUI;

    void Start()
    {
        // Activar LoginUI y desactivar RegisterUI al principio
        loginUIObject.SetActive(true);
        registerUIObject.SetActive(false);
    }

    void OnEnable()
    {
        // Suscribirse a los eventos
        OnShowLoginUI += ShowLoginUI;
        OnShowRegisterUI += ShowRegisterUI;
    }

    void OnDisable()
    {
        // Desuscribirse de los eventos
        OnShowLoginUI -= ShowLoginUI;
        OnShowRegisterUI -= ShowRegisterUI;
    }

    // Método para cambiar a RegisterUI
    public void ShowRegisterUI()
    {
        loginUIObject.SetActive(false);
        registerUIObject.SetActive(true);
    }

    // Método para regresar a LoginUI
    public void ShowLoginUI()
    {
        registerUIObject.SetActive(false);
        loginUIObject.SetActive(true);
    }

    // Método para invocar el cambio a LoginUI desde cualquier parte
    public void InvokeShowLoginUI()
    {
        OnShowLoginUI?.Invoke();
    }

    // Método para invocar el cambio a RegisterUI desde cualquier parte
    public void InvokeShowRegisterUI()
    {
        OnShowRegisterUI?.Invoke();
    }
}
