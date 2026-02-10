using UnityEngine;

// Inheritance: PlayerController inherits from MonoBehaviour, gaining access to Unity lifecycle methods
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

    [Header("Audio")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private float stepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.3f;
    private float stepTimer;

    private GameManager gameManager;

    private bool isGrounded = true;
    private bool isDead = false;
    private float targetYaw;

    public float WalkSpeed
    {
        get => movementSystem?.WalkSpeed ?? 15f;
        set => movementSystem.WalkSpeed = value;
    }

    public float RunSpeed
    {
        get => movementSystem?.RunSpeed ?? 37.5f;
        set => movementSystem.RunSpeed = value;
    }

    public float JumpForce
    {
        get => movementSystem?.JumpForce ?? 5f;
        set => movementSystem.JumpForce = value;
    }

    public float RotationSpeed
    {
        get => movementSystem?.RotationSpeed ?? 5f;
        set => movementSystem.RotationSpeed = value;
    }


    private void Start()
    {

        gameManager = GameManager.Instance;
        rb.constraints &= ~RigidbodyConstraints.FreezeRotationY;
        rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        cameraTransform = Camera.main.transform;
        cameraController = cameraTransform.GetComponent<CameraController>();

        animator = GetComponent<Animator>();
        movementSystem = new Movement();
        animationSystem = new Animation();

        movementSystem.Initialize(rb);
        animationSystem.Initialize(animator);

        isGrounded = true;
        targetYaw = transform.eulerAngles.y;
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound
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


        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            cameraController = cameraTransform.GetComponent<CameraController>();
        }

        movementSystem.HandleMovement(inputDirection, false);

        if (inputDirection.sqrMagnitude > 0.01f && !isDead)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep(true);
                stepTimer = runStepInterval;
            }
        }
        else
        {
            stepTimer = 0.05f; 
        }

        if (cameraController != null)
            targetYaw = cameraController.Yaw;
        else
            targetYaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        animationSystem?.SetMovementState(inputDirection.magnitude, false);
    }

    public void TriggerGameOver()
    {
        if (isDead) return; 
        isDead = true;
        
        animationSystem?.setDeathAnimation();
        movementSystem.WalkSpeed = 0;
        
        StartCoroutine(SinkPlayer(2f, 5f)); 
        
        GameManager.Instance.SetGameOver(true);
        
    }
    
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        audioSource.PlayOneShot(clip, volume);
    }
    
    public void PlayFootstep(bool isRunning = false)
    {
        AudioClip clip = walkClip;;
        PlaySound(clip, 0.5f);
    }
    
    private System.Collections.IEnumerator SinkPlayer(float duration, float sinkAmount)
    {
        PlaySound(deathClip);

   
            rb.isKinematic = true;
            rb.useGravity = false;
        
        
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(startPos.x, startPos.y - sinkAmount, startPos.z);
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        
        transform.position = endPos;

        this.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TriggerGameOver();
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            movementSystem.IsGrounded = true;
            animationSystem?.SetGrounded(true);
        }

    }
}
