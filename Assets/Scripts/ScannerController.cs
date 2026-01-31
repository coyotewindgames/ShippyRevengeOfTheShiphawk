using UnityEngine;

public class ScannerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject scanner;
    private Transform holderTransform;
    [Header("Scan Fire")]
    [SerializeField] private Transform scannerTip;
    [SerializeField] private float scanRange = 50f;
    [SerializeField] private LayerMask hitLayers = ~0;
    [SerializeField] private GameObject hitEffect;
    [Header("Pickup Position")]
    [SerializeField] private float pickupForwardOffset = 0.5f;
    [SerializeField] private Camera scannerCamera;
    [Header("Aim Offset")]
    [SerializeField] private float aimPitchOffset = 0f; // Adjust if scanner model is tilted
    [Header("Laser")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float laserDuration = 0.1f;

    private PickUpObject pickUpObject;
    private CameraController mainCameraController;

    void Start()
    {
        scanner = GameObject.FindGameObjectWithTag("Scanner");
        if (scannerTip == null && scanner != null)
            scannerTip = scanner.transform;
        if (scannerCamera == null && scanner != null)
            scannerCamera = scanner.GetComponentInChildren<Camera>();
        if (scanner != null)
            pickUpObject = scanner.GetComponent<PickUpObject>();
        if (Camera.main != null)
            mainCameraController = Camera.main.GetComponent<CameraController>();
        var tempParent = TempParent.Instance;
        if (tempParent != null)
            holderTransform = tempParent.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && pickUpObject != null && pickUpObject.isHolding)
            FireScan();
    }

    public void OnScannerPickedUp()
    {
        if (scanner == null) return;
        if (holderTransform != null)
        {
            Vector3 forward = holderTransform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude > 0.0001f)
                scanner.transform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
        }
        if (holderTransform != null)
        {
            float xOffset = scanner.transform.position.x - holderTransform.position.x;
            float yPos = scanner.transform.position.y + .25f;
            float zPos = holderTransform.position.z + pickupForwardOffset;
            scanner.transform.position = new Vector3(holderTransform.position.x + xOffset, yPos, zPos);
        }
        else
        {
            scanner.transform.position = new Vector3(scanner.transform.position.x, scanner.transform.position.y - 1f, scanner.transform.position.z);
        }
    }

    private void FireScan()
    {
        if (scannerTip == null && scannerCamera == null) return;

        // Use scanner tip position (front of scanner) as origin, offset slightly forward
        Vector3 origin = scannerTip != null ? scannerTip.position : scannerCamera.transform.position;
        Vector3 dir;

        // Use main camera's aim direction (toward crosshair) if available
        if (mainCameraController != null)
        {
            dir = mainCameraController.GetAimDirectionFrom(origin, scanRange, hitLayers);
            // Apply pitch offset if scanner model causes aim to be off
            if (Mathf.Abs(aimPitchOffset) > 0.01f)
            {
                Quaternion pitchAdjust = Quaternion.AngleAxis(aimPitchOffset, Vector3.Cross(Vector3.up, dir));
                dir = pitchAdjust * dir;
            }
        }
        else
        {
            dir = scanner.transform.forward;
        }

        if (laserPrefab != null)
        {
            GameObject laser = Instantiate(laserPrefab, origin, Quaternion.LookRotation(dir));
            var laserMachine = laser.GetComponent<Lightbug.LaserMachine.LaserMachine>();
            if (laserMachine != null)
            {
                laserMachine.ConfigureForSingleShot(scanRange);
            }
            Destroy(laser, laserDuration);
        }

        if (Physics.Raycast(origin, dir, out RaycastHit hit, scanRange, hitLayers))
        {
            // Debug.DrawLine(origin, hit.point, Color.cyan, 2f);
             if (hitEffect != null)
                Instantiate(hitEffect, hit.point, Quaternion.identity);
        }
        else
        {
            // Debug.DrawLine(origin, origin + dir * scanRange, Color.cyan, 2f);
        }
    }
}
