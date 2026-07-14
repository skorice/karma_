using UnityEngine;

public class MovementSquashStretch : MonoBehaviour
{
    [Header("Squash and Stretch Settings")]
    [SerializeField] private float squashAmount = 0.8f;
    [SerializeField] private float stretchAmount = 1.2f;
    [SerializeField] private float speedMultiplier = 2f;
    
    [Header("Idle Breathing")]
    [SerializeField] private float idleBreathAmount = 0.1f;
    [SerializeField] private float idleBreathSpeed = 1f;
    
    private Vector3 originalScale;
    private Rigidbody2D rb;
    private float velocityMagnitude;
    private float breathTimer;
    
    private void Start()
    {
        originalScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        if (rb != null)
        {
            velocityMagnitude = rb.linearVelocity.magnitude;
            ApplySquashAndStretch();
        }
    }
    
    private void ApplySquashAndStretch()
    {
        if (velocityMagnitude > 0.1f)
        {
            // Движение - применяем сжатие/растяжение
            float stretch = 1f + (velocityMagnitude * speedMultiplier * 0.01f);
            float squash = 1f / Mathf.Sqrt(stretch);
            
            Vector3 newScale = originalScale;
            newScale.x *= stretch * squashAmount;
            newScale.y *= squash;
            
            transform.localScale = newScale;
        }
        else
        {
            // Idle - дыхание
            breathTimer += Time.deltaTime * idleBreathSpeed;
            float breath = 1f + Mathf.Sin(breathTimer) * idleBreathAmount;
            
            Vector3 newScale = originalScale;
            newScale.y *= breath;
            newScale.x *= 1f / Mathf.Sqrt(breath);
            
            transform.localScale = Vector3.Lerp(transform.localScale, newScale, Time.deltaTime * 5f);
        }
    }
    
    public void ResetScale()
    {
        transform.localScale = originalScale;
    }
}