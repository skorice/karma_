using UnityEngine;
using DG.Tweening;

public class Pulse : MonoBehaviour
{
    [Header("Настройки пульсации")]
    [Tooltip("На сколько увеличивается объект относительно исходного размера")]
    [SerializeField] private float pulseScale = 1.1f;

    [Tooltip("Длительность одного цикла (увеличение или уменьшение)")]
    [SerializeField] private float duration = 0.8f;

    [Tooltip("Тип сглаживания анимации")]
    [SerializeField] private Ease easeType = Ease.InOutSine;

    private Vector3 _originalScale;
    private Tween _pulseTween;

    private void Awake()
    {
        // Запоминаем исходный масштаб объекта
        _originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        StartPulse();
    }

    private void OnDisable()
    {
        StopPulse();
    }

    private void StartPulse()
    {
        // На всякий случай убиваем предыдущий твин
        _pulseTween?.Kill();

        // Возвращаем исходный масштаб перед стартом
        transform.localScale = _originalScale;

        // Создаём бесконечную пульсацию (Yoyo — туда-обратно)
        _pulseTween = transform
            .DOScale(_originalScale * pulseScale, duration)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void StopPulse()
    {
        _pulseTween?.Kill();
        // Возвращаем объект в исходное состояние
        transform.localScale = _originalScale;
    }

    private void OnDestroy()
    {
        // Обязательно чистим твин, чтобы не было ошибок при уничтожении объекта
        _pulseTween?.Kill();
    }
}