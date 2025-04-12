using UnityEngine;

public class IdleAnimationCat : MonoBehaviour
{
    private Animator animator;
    private float timeInState = 0f;
    private float timeToStretch = 0f;  // Para controlar el tiempo antes de hacer Stretch después de Laying

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Incrementamos el tiempo en el estado actual
        timeInState += Time.deltaTime;
        timeToStretch += Time.deltaTime;  // Aumentamos el contador para Stretch

        // Transiciones entre estados de animación
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Cat1-Idle"))
        {
            // Prioridad 1: Laying
            if (!animator.GetBool("isLayingDone") && timeInState > 8f)
            {
                animator.SetTrigger("ToLaying");
                timeInState = 0f;
                timeToStretch = 0f;
            }
            // Prioridad 2: Meow
            else if (!animator.GetBool("isMeowDone") && timeInState > 5f)
            {
                animator.SetTrigger("ToMeow");
                timeInState = 0f;
                timeToStretch = 0f;
            }
            // Prioridad 3: Licking
            else if (!animator.GetBool("isLickingDone") && timeInState > 4f)
            {
                animator.SetTrigger("ToLicking");
                timeInState = 0f;
                timeToStretch = 0f;
            }

            // Condición para pasar a Stretch después de Laying y estar en Idle
            else if (animator.GetBool("isLayingDone") && timeToStretch >= 0.5f)  // Menos de un segundo después de Idle
            {
                animator.SetTrigger("ToStretch");
                timeToStretch = 0f;  // Resetear tiempo para el siguiente ciclo
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Cat1-Licking"))
        {
            if (timeInState > 2f) // 2 segundos en licking
            {
                animator.SetBool("isLickingDone", true);  // Marcamos que Licking ha terminado
                animator.SetTrigger("ToIdle");
                timeInState = 0f;
                timeToStretch = 0f;
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Cat1-Meow"))
        {
            if (timeInState > 0.6f) // 0.5 segundos en meow
            {
                animator.SetBool("isMeowDone", true);  // Marcamos que Meow ha terminado
                animator.SetTrigger("ToIdle");  // Pasamos a Idle, evitando repetir Meow
                timeInState = 0f;
                timeToStretch = 0f;
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Cat1-Laying"))
        {
            if (timeInState > 6f) // 6 segundos en laying
            {
                animator.SetBool("isLayingDone", true);  // Marcamos que Laying ha terminado
                animator.SetTrigger("ToIdle"); // Regresamos a Idle antes de pasar a Stretch
                timeInState = 0f;
                timeToStretch = 0f;
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Cat1-Stretch"))
        {
            if (timeInState > 1.5f) // 1.5 segundos en stretch
            {
                animator.SetBool("isStretchDone", true);  // Marcamos que Stretch ha terminado
                animator.SetTrigger("ToIdle");
                timeInState = 0f;
                timeToStretch = 0f;
            }
        }

        // Reiniciar ciclo después de Stretching
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Cat1-Idle") && 
            animator.GetBool("isStretchDone")) 
        {
            ResetCycle(); // Reiniciar el ciclo de animaciones
        }
    }

    // Función para reiniciar el ciclo de animaciones
    private void ResetCycle()
    {
        animator.SetBool("isLickingDone", false);
        animator.SetBool("isMeowDone", false);
        animator.SetBool("isLayingDone", false);
        animator.SetBool("isStretchDone", false);
    }
}


