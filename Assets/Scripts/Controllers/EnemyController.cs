using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public ZoneController zoneController;
    [SerializeField] private Transform playerTransform; // Assign in inspector or find automatically
    private AudioSource audioSource;
    [SerializeField] private AudioClip hitClip;
    private bool isChasingPlayer = false;
    private Animator animator;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float collisionDistance = 1.5f; // Distance to trigger collision in WebGL builds

    private PlayerController playerController;
    private bool hasTriggeredGameOver = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        // Ensure Rigidbody exists and is Dynamic (Not Kinematic) for correct collision events
        Rigidbody rb = GetComponent<Rigidbody>();
        // IMPORTANT: Must be false to generate collision events while moving
        rb.isKinematic = false;
        rb.useGravity = true;
        // Continuous detection prevents passing through objects at high speed
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // Keep upright

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerController = player.GetComponent<PlayerController>();
            }
        }
    }

    void FixedUpdate()
    {
        if (isChasingPlayer && playerTransform != null)
        {
            // Check distance-based collision for WebGL builds (backup method)
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (!hasTriggeredGameOver && distanceToPlayer <= collisionDistance)
            {
                Debug.Log("EnemyController: Distance-based collision triggered at distance: " + distanceToPlayer);
                TriggerPlayerCaught();
                return;
            }

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

                    velocity.y = rb.linearVelocity.y;
                    rb.linearVelocity = velocity;

                    rb.MoveRotation(targetRot);
                }
                else
                {
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

            if (currentZone > 0)
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
            if (player != null) 
            {
                playerTransform = player.transform;
                playerController = player.GetComponent<PlayerController>();
            }
        }

        isChasingPlayer = true;
        hasTriggeredGameOver = false; // Reset for new chase
    }

    private void TriggerPlayerCaught()
    {
        if (hasTriggeredGameOver) return;
        
        hasTriggeredGameOver = true;
        isChasingPlayer = false;
        animator.SetBool("FinalZone", false);
        audioSource.PlayOneShot(hitClip);

        Debug.Log("EnemyController: Player caught, triggering game over");

        // Trigger game over
        if (playerController != null)
        {
            playerController.TriggerGameOver();
        }
        else
        {
            // Fallback if PlayerController component isn't found
            Debug.LogWarning("PlayerController not found, using GameManager directly");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameOver(true);
            }
        }
    }

    // Physics collision detection (primary method)
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("EnemyController: Collision detected with " + collision.gameObject.name + " (Tag: " + collision.gameObject.tag + ")");

        if (collision.gameObject.CompareTag("Player") && !hasTriggeredGameOver)
        {
            Debug.Log("EnemyController: Player collision confirmed via OnCollisionEnter");
            TriggerPlayerCaught();
        }
    }

    // Trigger collision detection (backup method for WebGL)
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("EnemyController: Trigger detected with " + other.gameObject.name + " (Tag: " + other.gameObject.tag + ")");

        if (other.CompareTag("Player") && !hasTriggeredGameOver)
        {
            Debug.Log("EnemyController: Player collision confirmed via OnTriggerEnter");
            TriggerPlayerCaught();
        }
    }
}
