using UnityEngine;

public class WalkBehaviorAndAnimCat : MonoBehaviour
{
    [SerializeField] private Transform[] leftPoints;  // Puntos a la izquierda
    [SerializeField] private Transform[] rightPoints; // Puntos a la derecha
    [SerializeField] private float moveSpeed = 3f;    // Velocidad de movimiento del gato
    private int currentTargetIndex = 0;               // Índice del punto de destino actual
    private bool isWalking = false;                   // Si el gato está caminando
    private Vector3 targetPosition;
    private float distanceToTarget;
    private IdleAnimationCat idleAnimationCat;       // Referencia al script de animaciones
    private SpriteRenderer spriteRenderer;           // Para obtener el SpriteRenderer y flipar el sprite
    private bool isInIdleAnimation = false;         // Flag para verificar si estamos en Idle

    private void Start()
    {
        // Obtén una referencia al script de animaciones
        idleAnimationCat = GetComponent<IdleAnimationCat>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Asignamos el SpriteRenderer

        // Empezamos con el primer punto de la izquierda
        targetPosition = leftPoints[currentTargetIndex].position;
    }

    private void Update()
    {
        // Si el ciclo de Idle ha terminado, comenzamos a caminar
        if (idleAnimationCat.isCycleComplete && !isWalking && !isInIdleAnimation)
        {
            isWalking = true;
            isInIdleAnimation = false; // Aseguramos que el flag de Idle se resetea
            idleAnimationCat.enabled = false; // Desactivamos el script de animaciones mientras caminamos
        }

        // Mover al gato si está caminando
        if (isWalking)
        {
            MoveToTarget();
        }
        else
        {
            // Si ha llegado al objetivo, esperamos en Idle
            distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            if (distanceToTarget <= 0f)  // Si está cerca del objetivo
            {
                isWalking = false;  // Dejamos de caminar
                GetComponent<Animator>().SetBool("isWalking", false); // Detenemos la animación de caminar

                if (!isInIdleAnimation)  // Si no estamos ya en Idle
                {
                    isInIdleAnimation = true;  // Marcamos que ahora estamos en Idle
                    idleAnimationCat.enabled = true;  // Activamos el ciclo de animaciones en Idle
                    idleAnimationCat.isCycleComplete = false; // Reseteamos el ciclo de Idle
                }
            }
        }
    }

    // Función para mover el gato hacia el siguiente punto
    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Activamos la animación de caminar
        GetComponent<Animator>().SetBool("isWalking", true);

        // Flipar el sprite dependiendo de la dirección en la que se mueve
        if (targetPosition.x > transform.position.x)
        {
            // Mover hacia la derecha (flip en X positivo)
            spriteRenderer.flipX = false;
        }
        else if (targetPosition.x < transform.position.x)
        {
            // Mover hacia la izquierda (flip en X negativo)
            spriteRenderer.flipX = true;
        }
    }

    // Función para ejecutar el ciclo de animaciones en Idle
    private void ExecuteCycle()
    {
        // Pasamos al siguiente punto aleatorio
        if (leftPoints.Length > 0 && rightPoints.Length > 0)
        {
            Transform[] allPoints = currentTargetIndex < leftPoints.Length ? leftPoints : rightPoints;
            int randomIndex = Random.Range(0, allPoints.Length);

            // Evitar que el gato vuelva al mismo punto
            while (randomIndex == currentTargetIndex)
            {
                randomIndex = Random.Range(0, allPoints.Length);
            }

            currentTargetIndex = randomIndex;
            targetPosition = allPoints[currentTargetIndex].position;
        }

        // El gato comienza a caminar hacia el siguiente objetivo
        isWalking = true;
        isInIdleAnimation = false;  // Resetear el flag de Idle
    }
}