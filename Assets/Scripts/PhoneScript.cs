using UnityEngine;

public class PhoneScript : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip dialogueClip;
    public AudioClip answerClick;

    [Header("Light Settings")]
    [SerializeField] private Light phoneLight;
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float maxIntensity = 2f;
    [SerializeField] private float waitTime = 5f;
    
    private bool isAnswered = false;
    private float timer = 0f;
    private bool callStarted = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (phoneLight == null) phoneLight = GetComponentInChildren<Light>();
        
        // Ensure light starts off
        if (phoneLight != null) phoneLight.intensity = 0f;
    }

    void Update()
    {
        if (!isAnswered)
        {
            if (!callStarted)
            {
                timer += Time.deltaTime;
                if (timer >= waitTime)
                {
                    callStarted = true;
                    audioSource.Play();
                }
            }
            
            if (callStarted)
            {
                HandleLightPulse();
            }
        }
    }

    void HandleLightPulse()
    {
        if (!isAnswered && phoneLight != null)
        {
            phoneLight.intensity = Mathf.PingPong(Time.time * pulseSpeed, maxIntensity);
        }
    }

    private void OnMouseDown()
    {
        if (!isAnswered)
        {
            AnswerPhone();
        }
    }

    void AnswerPhone()
    {
        isAnswered = true;
        audioSource.Stop();
        audioSource.PlayOneShot(answerClick);
        audioSource.PlayOneShot(dialogueClip);
        if (phoneLight != null) 
        {
            phoneLight.intensity = 0f;
            phoneLight.enabled = false; // Stop light when answered
        }
    }
}
