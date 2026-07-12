using UnityEngine;

/// <summary>
/// Простой squash/stretch для ощущения анимации ходьбы (Brotato-like).
/// Вешать на объект со спрайтом (или на child-спрайт).
/// </summary>
public class MovementSquashStretch : MonoBehaviour
{
    [Header("Sources")]
    [Tooltip("Откуда брать скорость. Если null — ищет Rigidbody2D на себе/родителе.")]
    [SerializeField] private Rigidbody2D body;

    [Tooltip("Если нет Rigidbody — можно скармливать velocity снаружи через SetVelocity().")]
    [SerializeField] private bool useExternalVelocity;

    [Header("Idle breath")]
    [SerializeField] private float idleBreathAmount = 0.03f; // 3%
    [SerializeField] private float idleBreathSpeed = 2.2f;

    [Header("Run pulse")]
    [SerializeField] private float runPulseAmount = 0.08f;   // сила сжатия/разжатия
    [SerializeField] private float runPulseSpeed = 10f;      // частота "шагов"
    [SerializeField] private float minSpeedToAnimate = 0.15f;

    [Header("Move stretch")]
    [Tooltip("Лёгкое вытягивание по направлению движения.")]
    [SerializeField] private float directionalStretch = 0.06f;

    [Header("Smoothing")]
    [SerializeField] private float scaleLerpSpeed = 18f;

    private Vector3 baseScale;
    private Vector2 externalVelocity;
    private float pulseTimer;

    private void Awake()
    {
        baseScale = transform.localScale;

        if (body == null && !useExternalVelocity)
            body = GetComponentInParent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        Vector2 velocity = GetVelocity();
        float speed = velocity.magnitude;

        // 1) Базовый пульс
        Vector3 targetScale = baseScale;

        if (speed > minSpeedToAnimate)
        {
            // "шаги": вверх-вниз по синусу
            pulseTimer += Time.deltaTime * runPulseSpeed;

            // sin: -1..1 → squash Y / stretch X
            float wave = Mathf.Sin(pulseTimer);

            float squashY = 1f - wave * runPulseAmount;
            float stretchX = 1f + wave * runPulseAmount;

            // 2) Лёгкое вытягивание по направлению движения
            Vector2 dir = velocity / speed; // normalized
            float face = Mathf.Abs(dir.x);  // больше по X — сильнее горизонтальный stretch

            stretchX += face * directionalStretch;
            squashY  -= face * directionalStretch * 0.5f;

            targetScale = new Vector3(
                baseScale.x * stretchX,
                baseScale.y * squashY,
                baseScale.z
            );
        }
        else
        {
            // idle "дыхание"
            pulseTimer = 0f;
            float breath = Mathf.Sin(Time.time * idleBreathSpeed) * idleBreathAmount;

            targetScale = new Vector3(
                baseScale.x * (1f + breath * 0.5f),
                baseScale.y * (1f - breath),
                baseScale.z
            );
        }

        // 3) Плавное применение
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            1f - Mathf.Exp(-scaleLerpSpeed * Time.deltaTime)
        );
    }

    private Vector2 GetVelocity()
    {
        if (useExternalVelocity)
            return externalVelocity;

        if (body != null)
            return body.linearVelocity;

        return Vector2.zero;
    }

    /// <summary>
    /// Если двигаешь игрока через transform / свой input — вызывай это из Move().
    /// </summary>
    public void SetVelocity(Vector2 velocity)
    {
        externalVelocity = velocity;
    }

    /// <summary>
    /// Если во время игры меняешь базовый scale персонажа (баффы и т.п.).
    /// </summary>
    public void SetBaseScale(Vector3 newBaseScale)
    {
        baseScale = newBaseScale;
    }

    /// <summary>
    /// Одноразовый "punch" при ударе/дэше/попадании.
    /// </summary>
    public void Punch(float amount = 0.15f, float duration = 0.08f)
    {
        StopAllCoroutines();
        StartCoroutine(PunchRoutine(amount, duration));
    }

    private System.Collections.IEnumerator PunchRoutine(float amount, float duration)
    {
        Vector3 punched = new Vector3(
            baseScale.x * (1f + amount),
            baseScale.y * (1f - amount),
            baseScale.z
        );

        float t = 0f;
        Vector3 start = transform.localScale;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = t / duration;
            transform.localScale = Vector3.Lerp(start, punched, a);
            yield return null;
        }
    }
}