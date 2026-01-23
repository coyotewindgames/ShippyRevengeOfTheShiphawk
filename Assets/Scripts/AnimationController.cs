using UnityEngine;

public class AnimationController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Animator animator;
    private Rigidbody rb;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        animator.SetFloat("running", 0f);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("jump");
            animator.SetBool("jump", true);
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetBool("jump", false);
        }

        handleMovement();

    }

    void handleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(h, 0, v) * 15f;

        if (movement.magnitude > 0.1f)
        {
            Vector3 newVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
            rb.linearVelocity = newVelocity;
            animator.SetFloat("running", 1f);
        }
        else
        {
            animator.SetFloat("running", 0f);
        }
    }

}
