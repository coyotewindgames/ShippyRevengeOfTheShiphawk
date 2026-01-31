using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public ZoneController zoneController;
    [SerializeField] private Transform playerTransform; // Assign in inspector or find automatically
    private AudioSource audioSource;
    private bool isChasingPlayer = false;
    private Animator animator;
    [SerializeField] private float runSpeed = 5f;

    void Start()
    {
        Debug.Log("EnemyController Started - Script is running!");
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        
        // Disable root motion so script controls movement
        if (animator != null)
            animator.applyRootMotion = false;

        // Ensure Rigidbody exists and is Dynamic (Not Kinematic) for correct collision events
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // IMPORTANT: Must be false to generate collision events while moving
        rb.isKinematic = false; 
        rb.useGravity = true; 
        // Continuous detection prevents passing through objects at high speed
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // Keep upright

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
    }

    // Use FixedUpdate for physics-based movement
    void FixedUpdate()
    {
        if (isChasingPlayer && playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            direction.y = 0;
            
            if (direction != Vector3.zero)
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                Quaternion targetRot = Quaternion.LookRotation(direction);
                
                if (rb != null && !rb.isKinematic)
                {
                    // Use Velocity for dynamic (physical) movement
                    Vector3 velocity = direction * runSpeed;
                    
                    // Unified fix: Use rb.velocity which works on all Unity versions
                    velocity.y = rb.linearVelocity.y; 
                    rb.linearVelocity = velocity;

                    rb.MoveRotation(targetRot);
                }
                else
                {
                    // Fallback for kinematic/transform movement if setup changes
                    transform.rotation = targetRot;
                    transform.position += direction * runSpeed * Time.fixedDeltaTime;
                }
            }
        }
    }
    public void OnScanHit()
    {
        audioSource.Play();
        if (zoneController != null)
        {
            var currentZone = zoneController.getCurrentZone();
      
            if(currentZone > 0)
            {
                int newZone = Mathf.Max(0, currentZone - 1);
                zoneController.setCurrentZone(newZone);
            }
        }
    }

    public void EnterFinalChaseMode()
    {
        animator.SetBool("FinalZone", true);        
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
        
        isChasingPlayer = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enemy collided with: " + collision.gameObject.name);
    }
}
