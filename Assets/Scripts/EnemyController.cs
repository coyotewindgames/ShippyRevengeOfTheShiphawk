using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public ZoneController zoneController;
    [SerializeField] private Transform playerTransform; // Assign in inspector or find automatically
    private AudioSource audioSource;
    private bool isGameOver = false;
    private Animator animator;
    [SerializeField] private float runSpeed = 5f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver && playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            direction.y = 0;
            
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
                transform.position += direction * runSpeed * Time.deltaTime;
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

    public void TriggerGameOver()
    {
        animator.SetBool("FinalZone", true);        
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
        
        isGameOver = true;
    }
}
