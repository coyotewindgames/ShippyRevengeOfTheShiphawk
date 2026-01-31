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
    [SerializeField] private bool useMouseForHorizontal = false;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private Transform cameraTransform;
    private CameraController cameraController;
    
    private bool isGrounded = true;
    private float targetYaw;
    
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
        if (rb != null)
        {
            rb.constraints &= ~RigidbodyConstraints.FreezeRotationY;
            rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
        if (cameraTransform != null)
            cameraController = cameraTransform.GetComponent<CameraController>();
            
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (movementSystem == null)
            movementSystem = new Movement();
            
        if (animationSystem == null)
            animationSystem = new Animation();
        movementSystem.Initialize(rb);
        animationSystem.Initialize(animator);
        isGrounded = true;
        targetYaw = transform.eulerAngles.y;
        
    }

    private void Update()
    {
        HandleMovement();
    }

    private void FixedUpdate()
    {
        if (rb != null)
            rb.MoveRotation(Quaternion.Euler(0f, targetYaw, 0f));
    }
    
  
    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(h, 0, v);
        
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            cameraController = cameraTransform.GetComponent<CameraController>();
        }

        movementSystem.HandleMovement(inputDirection, isRunning);
        if (cameraController != null)
            targetYaw = cameraController.Yaw;
        else
            targetYaw += Input.GetAxis("Mouse X") * mouseSensitivity;
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