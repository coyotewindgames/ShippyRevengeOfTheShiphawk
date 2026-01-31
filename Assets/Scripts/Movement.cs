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
    private bool isGrounded;
    
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
    
    public bool IsGrounded 
    { 
        get { return isGrounded; } 
        set { isGrounded = value; } 
    }
    
    public void Initialize(Rigidbody rigidbody)
    {
        rb = rigidbody;
        isGrounded = true;
    }
    
    public void HandleMovement(Vector3 inputDirection, bool isRunning)
    {
        float forwardInput = inputDirection.z;
        float horizontalInput = inputDirection.x;

        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 move = rb.transform.forward * forwardInput + rb.transform.right * horizontalInput;
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        Vector3 newVelocity = new Vector3(move.x * currentSpeed, rb.linearVelocity.y, move.z * currentSpeed);
        rb.linearVelocity = newVelocity;
    }
    public void handlePlayerTurning(Vector3 inputDirection)
    {
        float horizontalInput = inputDirection.x;
        
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            float rotationAmount = horizontalInput * rotationSpeed * Time.deltaTime * 20f;
            rb.rotation = rb.rotation * Quaternion.Euler(0, rotationAmount, 0);
        }
    }   
    
    public void Jump()
    {
        if (isGrounded && rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // Set to false immediately after jumping
        }
    }
}