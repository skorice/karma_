using DG.Tweening;
using UnityEngine;

public class CollectiblePulse : MonoBehaviour
{
    [Header("Pulse")]
    [SerializeField] private float scaleMultiplier = 1.12f;
    [SerializeField] private float pulseDuration = 0.6f;
    [SerializeField] private bool randomizeStart = true;

    private Vector3 initialScale;
    private Sequence pulseSequence;

    private void Awake()
    {
        initialScale = transform.localScale;
    }

    private void OnEnable()
    {
        StartPulse();
    }

    private void StartPulse()
    {
        pulseSequence?.Kill();

        transform.localScale = initialScale;

        pulseSequence = DOTween.Sequence();

        // Чтобы множество collectible не пульсировали синхронно.
        if (randomizeStart)
        {
            pulseSequence.AppendInterval(
                Random.Range(0f, pulseDuration)
            );
        }

        pulseSequence
            .Append(
                transform.DOScale(
                        initialScale * scaleMultiplier,
                        pulseDuration
                    )
                    .SetEase(Ease.InOutSine)
            )
            .Append(
                transform.DOScale(
                        initialScale,
                        pulseDuration
                    )
                    .SetEase(Ease.InOutSine)
            )
            .SetLoops(-1);
    }

    private void OnDisable()
    {
        pulseSequence?.Kill();
        pulseSequence = null;

        transform.localScale = initialScale;
    }
}