using UnityEngine;

public class ScannerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject scanner;
    [Header("Scan Fire")]
    [SerializeField] private Transform scannerTip;
    [SerializeField] private float scanRange = 50f;
    [SerializeField] private LayerMask hitLayers = ~0;
    [SerializeField] private GameObject hitEffect;
    void Start()
    {
        scanner = GameObject.FindGameObjectWithTag("Scanner");
        if (scannerTip == null && scanner != null)
            scannerTip = scanner.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            FireScan();
    }

    public void OnScannerPickedUp()
    {
        if (scanner == null) return;
        Vector3 euler = scanner.transform.eulerAngles;
        scanner.transform.eulerAngles = new Vector3(euler.x, euler.y, 0f);
    }

    private void FireScan()
    {
        if (scannerTip == null) return;

        Vector3 origin = scannerTip.position;
        Vector3 dir = scannerTip.forward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, scanRange, hitLayers))
        {
            Debug.DrawLine(origin, hit.point, Color.cyan, 0.5f);
            if (hitEffect != null)
                Instantiate(hitEffect, hit.point, Quaternion.identity);
        }
        else
        {
            Debug.DrawLine(origin, origin + dir * scanRange, Color.cyan, 0.5f);
        }
    }
}
