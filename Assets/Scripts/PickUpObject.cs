using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class PickUpObject : MonoBehaviour
{
    private static PickUpObject currentHeld;

    public bool isHolding = false;
    [SerializeField]
    float throwForce = 100f;
    [SerializeField]
    float maxDistance = 3.0f;
    [SerializeField]
    Vector3 holdOffset = new Vector3(0f, 0f, 2f);
    float distance;

    TempParent tempParent;
    Rigidbody rb;
    Collider objCollider;
    Collider[] playerColliders;
    GameObject scannerObj;
    ScannerController scannerController;

    Vector3 objectPos;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        objCollider = GetComponent<Collider>();
        tempParent = TempParent.Instance;
        if (tempParent != null)
            playerColliders = tempParent.GetComponentsInParent<Collider>();

        scannerObj = GameObject.FindGameObjectWithTag("Scanner");
        if (scannerObj != null)
            scannerController = scannerObj.GetComponent<ScannerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHolding)
        {
            Hold();
        }
    }
    private void OnMouseDown()
    {
        if (tempParent == null) return;
        if (currentHeld != null && currentHeld != this) return;

        isHolding = true;
        currentHeld = this;
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.detectCollisions = false;
        objectPos = transform.position;
        transform.SetParent(tempParent.transform);

        foreach (var col in playerColliders)
        {
            if (col != null)
                Physics.IgnoreCollision(objCollider, col, true);
        }

        if (CompareTag("Scanner"))
            scannerController.OnScannerPickedUp();

    }
    private void OnMouseUp()
    {
        Drop();
    }
  
    private void Hold()
    {
        distance = Vector3.Distance(this.transform.position, tempParent.transform.position);
        if (distance <= maxDistance)
        {

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            if (Input.GetMouseButtonDown(1))
                Throw();
            else
            {
                Vector3 offsetWorld = tempParent.transform.TransformDirection(holdOffset);
                transform.position = tempParent.transform.position + offsetWorld;
                transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void Drop()
    {
        if (!isHolding) return;
        isHolding = false;
        if (currentHeld == this)
            currentHeld = null;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.detectCollisions = true;
        transform.SetParent(null);

            foreach (var col in playerColliders)
            {
                if (col != null)
                    Physics.IgnoreCollision(objCollider, col, false);
            }
    }

    private void Throw()
    {
        Drop();
        rb.AddForce(tempParent.transform.forward * throwForce, ForceMode.Impulse);
    }
}
