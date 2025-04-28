using cherrydev;
using UnityEngine;
using System.Collections;
using TMPro;

public class DialogStart : MonoBehaviour
{
    [SerializeField] private DialogBehaviour dialogBehaviour;
    [SerializeField] private DialogNodeGraph dialogGraph;
    [SerializeField] private string fontAssetName; // Nombre del TMP_FontAsset en los recursos
    [SerializeField] private TextMeshProUGUI dialogText; // Referencia al texto del diálogo

    private void Start()
    {
        // Buscar y asignar la fuente desde los recursos
        if (dialogText != null && !string.IsNullOrEmpty(fontAssetName))
        {
            TMP_FontAsset customFont = Resources.Load<TMP_FontAsset>(fontAssetName);
            if (customFont != null)
            {
                dialogText.font = customFont;
            }
            else
            {
                Debug.LogWarning($"No se encontró la fuente con el nombre '{fontAssetName}' en los recursos.");
            }
        }

        StartCoroutine(StartDialogWithDelay());
    }

    private IEnumerator StartDialogWithDelay()
    {
        // Esperar 1 segundo
        yield return new WaitForSeconds(1f);

        // Iniciar el diálogo
        dialogBehaviour.StartDialog(dialogNodeGraph: dialogGraph);
    }
}
