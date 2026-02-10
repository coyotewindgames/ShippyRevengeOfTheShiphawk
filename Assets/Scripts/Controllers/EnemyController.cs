using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public ZoneController zoneController;
    [SerializeField] private Transform playerTransform; 
    private AudioSource audioSource;
    [SerializeField] private AudioClip hitClip;
    private bool isChasingPlayer = false;
    private Animator animator;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float collisionDistance = 1.5f; 

    private PlayerController playerController;
    private bool hasTriggeredGameOver = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
       
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
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (!hasTriggeredGameOver && distanceToPlayer <= collisionDistance)
            {
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


        if (playerController != null)
        {
            playerController.TriggerGameOver();
        }
        else
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameOver(true);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player") && !hasTriggeredGameOver)
        {
            TriggerPlayerCaught();
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") && !hasTriggeredGameOver)
        {
            TriggerPlayerCaught();
        }
    }
}
