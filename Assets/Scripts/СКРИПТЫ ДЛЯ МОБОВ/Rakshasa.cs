using UnityEngine;

public class Rakshasa : EnemyBase
{
    [Header("Pack Buff")]
    [SerializeField] private float checkRadius = 3f;
    [SerializeField] private int requiredAllies = 4;

    private float baseHealth = 45f;
    private float baseSpeed=1.2f;
    private float baseDamage=8f;
    private float buffTimer = 0f; // Добавлено объявление переменной

    protected override void Start()
    {
        base.Start();

        baseSpeed = moveSpeed;
        baseDamage = damage;
    }

    protected override void Update()
    {
        base.Update();

        buffTimer += Time.deltaTime;

        if (buffTimer >= 0.5f)
        {
            UpdatePackBonus();
            buffTimer = 0f;
        }
    }

    private void UpdatePackBonus() // Только один метод
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(
            transform.position,
            checkRadius);

        int count = 0;

        foreach (Collider2D col in nearby)
        {
            if (col.GetComponent<Rakshasa>() != null)
                count++;
        }

        if (count >= requiredAllies)
        {
            moveSpeed = baseSpeed * 1.1f;
            damage = baseDamage * 1.15f;
        }
        else
        {
            moveSpeed = baseSpeed;
            damage = baseDamage;
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
    #endif
}