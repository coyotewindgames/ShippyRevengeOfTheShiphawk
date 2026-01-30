using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class PickUpObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool isHolding = false;
    [SerializeField] 
    float throwForce = 100f;
   [SerializeField] 
    float maxDistance = 3.0f;
    [SerializeField]
    Vector3 holdOffset = new Vector3(0f, 0f, 2f);
    float distance;

    TempParent tempParent;
    Rigidbody   rb;
    Collider objCollider;
    Collider playerCollider;
    GameObject scannerObj;
    ScannerController scannerController;

    Vector3 objectPos;
   void Start()
    {
        rb= GetComponent<Rigidbody>();
        objCollider = GetComponent<Collider>();
        tempParent = TempParent.Instance;
        if (tempParent != null)
            playerCollider = tempParent.GetComponentInParent<Collider>();

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

        isHolding = true;
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.detectCollisions = false;
        objectPos = transform.position;
        transform.SetParent(tempParent.transform);

        if (playerCollider != null && objCollider != null)
            Physics.IgnoreCollision(objCollider, playerCollider, true);

        if (scannerObj != null && scannerObj == gameObject)
        {
            if (scannerController != null)
                scannerController.OnScannerPickedUp();
            else
            {
                Vector3 euler = transform.eulerAngles;
                transform.eulerAngles = new Vector3(euler.x, euler.y, 0f);
            }
        }
    }
    private void OnMouseUp()
    {
        Drop();
    }
    private void OnMouseExit()
    {
        
    }
    private void Hold()
    {
        distance = Vector3.Distance(this.transform.position, tempParent.transform.position);
        if (distance <= maxDistance) {
        
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        if (Input.GetMouseButtonDown(1))
            Throw();
        else
        {
            Vector3 offsetWorld = tempParent.transform.TransformDirection(holdOffset);
            transform.position = tempParent.transform.position + offsetWorld;
        }
        }
    }   

    private void Drop()
    {
        if (!isHolding) return;
        isHolding = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.detectCollisions = true;
        transform.SetParent(null);

        if (playerCollider != null && objCollider != null)
            Physics.IgnoreCollision(objCollider, playerCollider, false);
    }

    private void Throw()
    {
        Drop();
        if (tempParent != null)
            rb.AddForce(tempParent.transform.forward * throwForce, ForceMode.Impulse);
    }
}
