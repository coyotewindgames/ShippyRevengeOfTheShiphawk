using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Mouse Look")]
    [SerializeField] private bool enableMouseLook = true;
    [SerializeField] private bool lockCursor = true;
    [SerializeField] private float mouseYawSensitivity = 2f;
    [SerializeField] private float mousePitchSensitivity = 2f;
    [SerializeField] private float pitchClamp = 80f;

    private float pitch;
    private float yaw;

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
        Cursor.visible = false;
    }

    private static float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}