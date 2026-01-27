using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(-0.29f, 10.15f, 1.29f);
    [SerializeField] private float smoothSpeed = 0.125f;

    [SerializeField] private Vector3 cameraAngle = new Vector3(-7.8f, 0f, 0f);

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 rotatedOffset = target.rotation * offset;
            Vector3 desiredPosition = target.position + rotatedOffset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            
            Vector3 targetEulerAngles = target.eulerAngles;
            Vector3 desiredRotation = new Vector3(cameraAngle.x, targetEulerAngles.y + cameraAngle.y, cameraAngle.z);
            transform.rotation = Quaternion.Euler(desiredRotation);
        }
    }

}