using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Systems")]
    [SerializeField] private Movement movementSystem;
    [SerializeField] private Animation animationSystem;
    
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    
    private bool isGrounded = true;
    
    public float WalkSpeed 
    { 
        get { return movementSystem?.WalkSpeed ?? 15f; } 
        set { if (movementSystem != null) movementSystem.WalkSpeed = value; } 
    }
    
    public float RunSpeed 
    { 
        get { return movementSystem?.RunSpeed ?? 37.5f; } 
        set { if (movementSystem != null) movementSystem.RunSpeed = value; } 
    }
    
    public float JumpForce 
    { 
        get { return movementSystem?.JumpForce ?? 5f; } 
        set { if (movementSystem != null) movementSystem.JumpForce = value; } 
    }
    
    public float RotationSpeed 
    { 
        get { return movementSystem?.RotationSpeed ?? 5f; } 
        set { if (movementSystem != null) movementSystem.RotationSpeed = value; } 
    }
    
    
    private void Start()
    {
        print("PlayerController Start() called!");
        
        // Check if GameManager exists
        if (GameManager.Instance != null)
        {
            print("GameManager exists!");
        }
        else
        {
            print("GameManager is NULL!");
        }
        
        if (rb == null)
            rb = GetComponent<Rigidbody>();
            
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (movementSystem == null)
            movementSystem = new Movement();
            
        if (animationSystem == null)
            animationSystem = new Animation();
            
        movementSystem.Initialize(rb);
        animationSystem.Initialize(animator);
        isGrounded = true;
        
        print("PlayerController initialization complete!");
    }

    private void Update()
    {
        HandleJump();
        HandleMovement();
    }
    
    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            movementSystem.Jump();
            animationSystem?.TriggerJump();
        }
    }
    
    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(h, 0, v);
        
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        
        movementSystem.HandleMovement(inputDirection, isRunning);
        movementSystem.handlePlayerTurning(inputDirection);
        animationSystem?.SetMovementState(inputDirection.magnitude, isRunning);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            movementSystem.IsGrounded = true;
            animationSystem?.SetGrounded(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            movementSystem.IsGrounded = false;
            animationSystem?.SetGrounded(false);
        }
    }
}