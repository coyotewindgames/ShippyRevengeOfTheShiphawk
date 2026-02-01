using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("Animation System")]
    [SerializeField] private Animation animationSystem;
    [SerializeField] private Animator animator;
    
    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (animationSystem == null)
            animationSystem = new Animation();
            
        animationSystem.Initialize(animator);
    }
    
}
