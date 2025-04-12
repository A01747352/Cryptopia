using UnityEngine;

public class IdleAnimationCat : MonoBehaviour
{
    [SerializeField] private string catId; // Identificador único para cada gato (Cat1, Cat2, etc.)
    private Animator animator;
    private float timeInState = 0f;
    private float timeToStretch = 0f; // Para controlar el tiempo antes de hacer Stretch después de Laying

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Incrementamos el tiempo en el estado actual
        timeInState += Time.deltaTime;
        timeToStretch += Time.deltaTime;

        // Transiciones entre estados de animación
        if (animator.GetCurrentAnimatorStateInfo(0).IsName($"{catId}-Idle"))
        {
            // Prioridad 1: Laying
            if (!animator.GetBool($"{catId}isLayingDone") && timeInState > 8f)
            {
                animator.SetTrigger($"{catId}ToLaying");
                timeInState = 0f;
                timeToStretch = 0f;
            }
            // Prioridad 2: Meow
            else if (!animator.GetBool($"{catId}isMeowDone") && timeInState > 5f)
            {
                animator.SetTrigger($"{catId}ToMeow");
                timeInState = 0f;
                timeToStretch = 0f;
            }
            // Prioridad 3: Licking
            else if (!animator.GetBool($"{catId}isLickingDone") && timeInState > 4f)
            {
                animator.SetTrigger($"{catId}ToLicking");
                timeInState = 0f;
                timeToStretch = 0f;
            }
            // Condición para pasar a Stretch después de Laying y estar en Idle
            else if (animator.GetBool($"{catId}isLayingDone") && timeToStretch >= 0.5f)
            {
                animator.SetTrigger($"{catId}ToStretch");
                timeToStretch = 0f;
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName($"{catId}-Licking"))
        {
            if (timeInState > 2f)
            {
                animator.SetBool($"{catId}isLickingDone", true);
                animator.SetTrigger($"{catId}ToIdle");
                timeInState = 0f;
                timeToStretch = 0f;
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName($"{catId}-Meow"))
        {
            if (timeInState > 0.6f)
            {
                animator.SetBool($"{catId}isMeowDone", true);
                animator.SetTrigger($"{catId}ToIdle");
                timeInState = 0f;
                timeToStretch = 0f;
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName($"{catId}-Laying"))
        {
            if (timeInState > 6f)
            {
                animator.SetBool($"{catId}isLayingDone", true);
                animator.SetTrigger($"{catId}ToIdle");
                timeInState = 0f;
                timeToStretch = 0f;
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName($"{catId}-Stretch"))
        {
            if (timeInState > 1.5f)
            {
                animator.SetBool($"{catId}isStretchDone", true);
                animator.SetTrigger($"{catId}ToIdle");
                timeInState = 0f;
                timeToStretch = 0f;
            }
        }

        // Reiniciar ciclo después de Stretching
        if (animator.GetCurrentAnimatorStateInfo(0).IsName($"{catId}-Idle") &&
            animator.GetBool($"{catId}isStretchDone"))
        {
            ResetCycle();
        }
    }

    // Función para reiniciar el ciclo de animaciones
    private void ResetCycle()
    {
        animator.SetBool($"{catId}isLickingDone", false);
        animator.SetBool($"{catId}isMeowDone", false);
        animator.SetBool($"{catId}isLayingDone", false);
        animator.SetBool($"{catId}isStretchDone", false);
    }
}


