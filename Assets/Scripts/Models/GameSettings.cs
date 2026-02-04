using UnityEngine;

[System.Serializable]
public class GameSettings
{
    [Header("Player Settings")]
    public float walkSpeed = 15f;
    public float runSpeed = 37.5f;
    public float jumpForce = 5f;
    
    [Header("Animation Settings")]
    public float walkAnimationSpeed = 1f;
    public float runAnimationSpeed = 2.5f;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    
    [Header("Graphics Settings")]
    public bool enableVSync = true;
    public int targetFrameRate = 60;
}