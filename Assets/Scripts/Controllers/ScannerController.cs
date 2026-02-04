using UnityEngine;

public class ScannerController : MonoBehaviour
{
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
    [SerializeField] private float aimPitchOffset = 1f; 
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
            scannerTip.transform.localPosition = new Vector3(scannerTip.transform.localPosition.x, scannerTip.transform.localPosition.y + 0.5f, scannerTip.transform.localPosition.z);
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && pickUpObject != null && pickUpObject.isHolding)
            FireScan();
    }

    public void OnScannerPickedUp()
    {
        if (scanner == null) return;
        
        scanner.transform.localRotation = Quaternion.identity;

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

        Vector3 origin = scannerTip != null ? scannerTip.position : scannerCamera.transform.position;
        Vector3 dir;

        if (mainCameraController != null)
        {
            dir = mainCameraController.GetAimDirectionFrom(origin, scanRange, hitLayers);
            // my scanner was made slightly crooked add an offset to make the aim look more accurate
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
            var enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.OnScanHit();
            }
        }
       
    }
}
