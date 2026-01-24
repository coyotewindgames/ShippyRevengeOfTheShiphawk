using UnityEngine;

[System.Serializable]
public class Movement
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 15f;
    [SerializeField] private float runSpeed = 37.5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float rotationSpeed = 3f;
    
    private Rigidbody rb;
    
    // Properties with getters and setters
    public float WalkSpeed 
    { 
        get { return walkSpeed; } 
        set { walkSpeed = Mathf.Max(0, value); } 
    }
    
    public float RunSpeed 
    { 
        get { return runSpeed; } 
        set { runSpeed = Mathf.Max(0, value); } 
    }
    
    public float JumpForce 
    { 
        get { return jumpForce; } 
        set { jumpForce = Mathf.Max(0, value); } 
    }
    
    public float RotationSpeed 
    { 
        get { return rotationSpeed; } 
        set { rotationSpeed = Mathf.Max(0, value); } 
    }
    
    public void Initialize(Rigidbody rigidbody)
    {
        rb = rigidbody;
    }
    
    public void HandleMovement(Vector3 inputDirection, bool isRunning)
    {
        // Only use vertical input (forward/backward) for movement
        float forwardInput = inputDirection.z;
        
        if (Mathf.Abs(forwardInput) > 0.1f)
        {
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            // Move in the direction the player is facing, not input direction
            Vector3 movement = rb.transform.forward * forwardInput * currentSpeed;
            Vector3 newVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
            rb.linearVelocity = newVelocity;
        }
        else
        {
            Vector3 newVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            rb.linearVelocity = newVelocity;
        }
    }
    public void handlePlayerTurning(Vector3 inputDirection)
    {
        // Only use horizontal input for turning
        float horizontalInput = inputDirection.x;
        
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            // Rotate the player around Y-axis based on horizontal input
            float rotationAmount = horizontalInput * rotationSpeed * Time.deltaTime * 20f;
            rb.rotation = rb.rotation * Quaternion.Euler(0, rotationAmount, 0);
        }
    }   
    
    public void Jump()
    {
        if (rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}