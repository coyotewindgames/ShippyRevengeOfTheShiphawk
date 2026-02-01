using UnityEngine;

[System.Serializable]
public class Animation
{
    [Header("Animation Settings")]
    [SerializeField] private float walkAnimationSpeed = 1f;
    [SerializeField] private float runAnimationSpeed = 2.5f;
    
    private Animator animator;
    private bool isGrounded = true;
    
    public void Initialize(Animator animatorComponent)
    {
        animator = animatorComponent;
       
            animator.SetFloat("running", 0f);
            animator.SetBool("jump", false);
            animator.SetBool("isGrounded", true);
    }
    
    public void SetMovementState(float movementMagnitude, bool isRunning)
    {
        
        if (movementMagnitude > 0.1f)
        {
            float animationSpeed = isRunning ? runAnimationSpeed : walkAnimationSpeed;
            animator.SetFloat("running", animationSpeed);
        }
        else
        {
            animator.SetFloat("running", 0f);
        }
    }

    public void setDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("death");
        }
    }
    
    
    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
        if (animator != null)
        {
            animator.SetBool("isGrounded", grounded);
            if (grounded)
            {
                animator.SetBool("jump", false);
            }
        }
    }
}
