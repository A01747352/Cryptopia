using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // El personaje a seguir
    public Vector2 minPosition; // Límite inferior del mapa (x, y)
    public Vector2 maxPosition; // Límite superior del mapa (x, y)
    public float smoothSpeed = 0.125f;

    private float camHalfHeight;
    private float camHalfWidth;

    void Start()
    {
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            // Aplicar suavizado
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Limitar dentro de los bordes del mapa
            float clampedX = Mathf.Clamp(smoothedPosition.x, minPosition.x + camHalfWidth, maxPosition.x - camHalfWidth);
            float clampedY = Mathf.Clamp(smoothedPosition.y, minPosition.y + camHalfHeight, maxPosition.y - camHalfHeight);

            // Alinear la posición de la cámara a la cuadrícula de píxeles
            float pixelPerUnit =128f; // Ajusta esto según los valores de tu proyecto
            clampedX = Mathf.Round(clampedX * pixelPerUnit) / pixelPerUnit;
            clampedY = Mathf.Round(clampedY * pixelPerUnit) / pixelPerUnit;

            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }
    }
}
