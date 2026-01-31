using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Mouse Look")]
    [SerializeField] private bool enableMouseLook = true;
    [SerializeField] private bool lockCursor = true;
    [SerializeField] private float mouseYawSensitivity = 2f;
    [SerializeField] private float mousePitchSensitivity = 2f;
    [SerializeField] private float pitchClamp = 80f;
    [Header("Crosshair")]
    [SerializeField] private bool showCrosshair = true;
    [SerializeField] private float crosshairSize = 8f;

    private Vector3 directionTarget;

    private Vector3 mouse;

    private float pitch;
    private float yaw;
    private Quaternion lookRotation;

    public float Yaw => yaw;
    public float Pitch => pitch;

    public Vector3 AimDirection => transform.forward;

 
    public Vector3 GetAimPoint(float maxDistance = 100f, LayerMask? hitLayers = null)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        LayerMask layers = hitLayers ?? Physics.DefaultRaycastLayers;
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layers))
            return hit.point;
        return transform.position + transform.forward * maxDistance;
    }


    public Vector3 GetAimDirectionFrom(Vector3 origin, float maxDistance = 100f, LayerMask? hitLayers = null)
    {
        Vector3 aimPoint = GetAimPoint(maxDistance, hitLayers);
        return (aimPoint - origin).normalized;
    }

    private void OnEnable()
    {
        ApplyCursorLock();
    }

    private void OnDisable()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void Start()
    {
        Vector3 euler = transform.localEulerAngles;
        yaw = NormalizeAngle(euler.y);
        pitch = NormalizeAngle(euler.x);
        ApplyCursorLock();
    }

    private void Update()
    {
        if (!enableMouseLook) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseYawSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mousePitchSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void ApplyCursorLock()
    {
        if (!lockCursor) return;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    private static float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    private void Aiming()
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green);
            mouse = hit.point;
        }
        directionTarget = mouse - transform.position;
        lookRotation = Quaternion.LookRotation(directionTarget.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    
    }

    private void OnGUI()
    {
        if (!showCrosshair) return;
        float size = Mathf.Max(2f, crosshairSize);
        float x = (Screen.width - size) * 0.5f;
        float y = (Screen.height - size) * 0.5f;
        GUI.DrawTexture(new Rect(x, y, size, size), Texture2D.whiteTexture);
    }
}