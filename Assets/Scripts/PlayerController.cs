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

    public ZoneController zoneController;
    
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

    public void TriggerGameOver()
    {
        Debug.Log("GAME OVER TRIGGERED!");
        this.enabled = false;
        if(movementSystem != null) movementSystem.WalkSpeed = 0;
        // Add UI or Scene Reload logic here
        
        // Example: Reload scene after delay?
        // StartCoroutine(ReloadSceneRoutine());
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("PLAYER COL: OnCollisionEnter with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Ground"))
        {
            movementSystem.IsGrounded = true;
            animationSystem?.SetGrounded(true);
        }
        CheckEnemyCollision(collision.gameObject);
    }

    // Support CharacterController physics if the player uses that instead of Rigidbody
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("PLAYER CC: Hit " + hit.gameObject.name);
        CheckEnemyCollision(hit.gameObject);
    }
    
    private void OnCollisionStay(Collision collision)
    {
        // Only check enemy collision on stay, don't log spam
        if(collision.gameObject.CompareTag("Enemy"))
            CheckEnemyCollision(collision.gameObject);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        CheckEnemyCollision(other.gameObject);
    }

    private void CheckEnemyCollision(GameObject other)
    {
        Debug.Log("Checking collision with: " + other.name);
        // Check if the object or its root has the Enemy tag
        if(other.CompareTag("Enemy") || other.transform.root.CompareTag("Enemy"))
        {
            Debug.Log("Game Over! Player collided with enemy.");
            TriggerGameOver();
        }
        else
        {
            // Debug what we are hitting to help troubleshoot
            Debug.Log($"Player hit object: '{other.name}' with Tag: '{other.tag}' (Root Tag: '{other.transform.root.tag}')");
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