using UnityEngine;

public class InteractableIcon : MonoBehaviour
{
    public GameObject player; 
    public SpriteRenderer icon; 
    public string triggerName = "TriggerCryptography"; //

    void Start()
    {
        if (icon != null)
        {
            icon.enabled = false; // Asegúrate de que el icono esté oculto al inicio
            Debug.Log("Icon initialized and hidden.");
        }
        else
        {
            Debug.LogError("Icon is not assigned in the inspector!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Entered trigger with: {other.gameObject.name}");
        if (icon != null)
        {
            icon.enabled = true; // Muestra el icono
            Debug.Log("Icon enabled");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Exited trigger with: {other.gameObject.name}");
        if (icon != null)
        {
            icon.enabled = false; // Oculta el icono
            Debug.Log("Icon disabled");
        }
    }
}
