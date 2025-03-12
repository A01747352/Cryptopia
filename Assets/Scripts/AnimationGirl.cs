using UnityEngine;

public class AnimationGirl : MonoBehaviour
{
    private Animator animator;
    private MovementGirl movementScript;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        movementScript = GetComponent<MovementGirl>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Update the animator with the velocity value
        animator.SetFloat("velocity", Mathf.Abs(movementScript.rb.linearVelocity.x));

        // Flip the character based on velocity direction
        if (movementScript.rb.linearVelocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movementScript.rb.linearVelocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }
}
