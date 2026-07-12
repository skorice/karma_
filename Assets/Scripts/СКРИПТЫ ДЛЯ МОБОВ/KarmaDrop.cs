using DG.Tweening;
using UnityEngine;

public class KarmaDrop : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] private float destroyTime = 8.4f;
    [SerializeField] private float pickupDelay = 0.5f;

    [Header("Karma")]
    [SerializeField] private int minValue = 1;
    [SerializeField] private int maxValue = 5;

    [Header("Attraction")]
    [SerializeField] private float attractionRadius = 3f;

    // Как часто обновляется направление движения к игроку.
    [SerializeField] private float homingStepDuration = 0.08f;

    // Расстояние, на котором предмет считается подобранным.
    [SerializeField] private float collectDistance = 0.15f;

    private int value;
    private float spawnTime;
    private bool canPickup;
    private bool isAttracting;
    private bool isCollected;

    private Transform player;
    private Tween attractionTween;
    private PlayerLevelManager levelManager;

    public void SetValue(int amount)
    {
        value = amount;
    }

    private void Start()
    {
        if (value == 0)
        {
            value = Random.Range(minValue, maxValue + 1);
        }

        spawnTime = Time.time;
        canPickup = false;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Объект с тегом Player не найден!");
        }

        levelManager = FindFirstObjectByType<PlayerLevelManager>();

        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        if (!canPickup && Time.time - spawnTime >= pickupDelay)
        {
            canPickup = true;
        }

        if (!canPickup || isCollected || player == null)
        {
            return;
        }

        float sqrDistance =
            (player.position - transform.position).sqrMagnitude;

        if (!isAttracting &&
            sqrDistance <= attractionRadius * attractionRadius)
        {
            StartAttraction();
        }

        if (isAttracting &&
            sqrDistance <= collectDistance * collectDistance)
        {
            Collect();
        }
    }

    private void StartAttraction()
    {
        if (isAttracting || isCollected || player == null)
        {
            return;
        }

        isAttracting = true;
        MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        if (!isAttracting || isCollected || player == null)
        {
            return;
        }

        attractionTween?.Kill();

        attractionTween = transform
            .DOMove(player.position, homingStepDuration)
            .SetEase(Ease.Linear)
            .SetLink(gameObject)
            .OnComplete(MoveToPlayer);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected || !other.CompareTag("Player"))
        {
            return;
        }

        if (!canPickup)
        {
            float remainingTime = Mathf.Max(
                0f,
                pickupDelay - (Time.time - spawnTime)
            );

            Debug.Log(
                $"Карма ещё не готова! Подождите {remainingTime:F1}с"
            );

            return;
        }

        Collect();
    }

    private void Collect()
    {
        if (isCollected)
        {
            return;
        }

        isCollected = true;
        isAttracting = false;

        attractionTween?.Kill();

        if (levelManager != null)
        {
            levelManager.AddKarma(value);
            Debug.Log($"Игрок подобрал {value} кармы!");
        }
        else
        {
            Debug.LogWarning("PlayerLevelManager не найден!");
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        attractionTween?.Kill();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectDistance);
    }
#endif
}