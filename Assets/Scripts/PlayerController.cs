using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Systems")]
    [SerializeField] private Movement movementSystem;
    [SerializeField] private Animation animationSystem;
    
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    
    [Header("Input")]
    [SerializeField] private bool useMouseForHorizontal = true;
    [SerializeField] private float mouseSensitivity = 1f;
    
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
        
    }

    private void Update()
    {
        HandleMovement();
    }
    
  
    private void HandleMovement()
    {
        float h;
        if (useMouseForHorizontal)
        {
            float norm = (Input.mousePosition.x / (float)Screen.width - 0.5f) * 2f; // -1..1 across screen
            h = Mathf.Clamp(norm * mouseSensitivity, -1f, 1f);
        }
        else
        {
            h = Input.GetAxis("Horizontal");
        }
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