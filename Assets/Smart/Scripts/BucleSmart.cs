using System.Collections;
using UnityEngine;

public class BucleSmart : MonoBehaviour
{
    public float moveAmount = 0.5f;     // Cuánto sube y baja en el eje Y
    public float duration = 1.5f;       // Tiempo que tarda en subir o bajar

    private Coroutine moveCoroutine;
    private Vector3 initialPosition;    // Posición inicial del objeto
    private bool isMouseOver = false;   // Indica si el mouse está sobre el objeto

    private void Start()
    {
        // Guarda la posición inicial del objeto
        initialPosition = transform.position;
    }

    private void OnMouseEnter()
    {
        // Marca que el mouse está sobre el objeto
        isMouseOver = true;

        // Inicia la animación si no está corriendo
        if (moveCoroutine == null)
        {
            moveCoroutine = StartCoroutine(MoveLoop());
        }
    }

    private void OnMouseExit()
    {
        // Marca que el mouse ya no está sobre el objeto
        isMouseOver = false;
    }

    private IEnumerator MoveLoop()
    {
        Vector3 topPos = initialPosition + new Vector3(0, moveAmount, 0);

        while (true)
        {
            // Subir
            yield return StartCoroutine(MoveY(transform.position, topPos));

            // Si el mouse ya no está encima, regresar a la posición inicial y salir del bucle
            if (!isMouseOver)
            {
                yield return StartCoroutine(MoveY(transform.position, initialPosition));
                break;
            }

            // Bajar
            yield return StartCoroutine(MoveY(topPos, initialPosition));
        }

        // Finaliza la corrutina
        moveCoroutine = null;
    }

    private IEnumerator MoveY(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float smoothT = Mathf.SmoothStep(0, 1, t); // Easing suave
            transform.position = Vector3.Lerp(from, to, smoothT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to; // Asegura la posición final exacta
    }
}
