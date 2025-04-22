using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardImage : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveAmount = 0.5f;     // Magnitud del movimiento vertical
    public float duration = 1.5f;       // Tiempo para cada fase del movimiento

    private Vector3 initialPosition;    // Posición inicial del objeto
    private Coroutine moveCoroutine;
    private bool isMouseOver = false;   // Indica si el mouse está sobre el objeto

    [Header("Datos de Recompensa")]
    [Tooltip("Texto que se mostrará en este objeto (asignado desde el manager).")]
    public string imageText;
    [Tooltip("Recompensa asociada a este texto (asignada desde el manager).")]
    public string reward;

    [Header("Referencias UI")]
    [Tooltip("Componente de texto que mostrará el mensaje. Debe ser hijo de este objeto.")]
    public TextMeshProUGUI textComponent;
    [Tooltip("Botón que se usará para procesar la recompensa.")]
    public Button rewardButton;

    private void Start()
    {
        initialPosition = transform.position;
        // Asigna el texto; al estar el componente como hijo el mismo se moverá junto con el objeto.
        if (textComponent != null)
            textComponent.text = imageText;
        
        // Asigna el evento del botón para procesar la recompensa al presionarlo.
        if (rewardButton != null)
            rewardButton.onClick.AddListener(OnRewardButtonClicked);
    }

    private void OnMouseEnter()
    {
        isMouseOver = true;
        if (moveCoroutine == null)
            moveCoroutine = StartCoroutine(MoveLoop());
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
    }

    private IEnumerator MoveLoop()
    {
        Vector3 topPos = initialPosition + new Vector3(0, moveAmount, 0);
        while (true)
        {
            // Mover hacia arriba
            yield return StartCoroutine(MoveY(transform.position, topPos));
            // Si ya no está el mouse, regresar a la posición inicial y salir
            if (!isMouseOver)
            {
                yield return StartCoroutine(MoveY(transform.position, initialPosition));
                break;
            }
            // Mover hacia abajo
            yield return StartCoroutine(MoveY(topPos, initialPosition));
        }
        moveCoroutine = null;
    }

    private IEnumerator MoveY(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = to;
    }

    // Este método se invoca al presionar el botón
    private void OnRewardButtonClicked()
    {
        // Se simula la comparación en la "base de datos" y se asigna la recompensa
        string condition = CompareTextInDatabase(imageText);
        AssignReward(condition, reward);
    }

    // Función simulada para comparar el texto en la base de datos
    private string CompareTextInDatabase(string text)
    {
        if (text.Contains("A"))
            return "ConditionA";
        else if (text.Contains("B"))
            return "ConditionB";
        else
            return "Default";
    }

    // Asigna y muestra la recompensa (aquí puedes implementar la lógica real)
    private void AssignReward(string condition, string rewardText)
    {
        if (condition == "ConditionA")
            Debug.Log("Recompensa asignada: " + rewardText + " (bonus A)");
        else if (condition == "ConditionB")
            Debug.Log("Recompensa asignada: " + rewardText + " (bonus B)");
        else
            Debug.Log("Recompensa asignada: " + rewardText);
    }
}
