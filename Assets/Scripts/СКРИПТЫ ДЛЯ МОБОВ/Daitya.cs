using UnityEngine;
using System.Collections;

public class Daitya : EnemyBase
{
    [Header("Defense")]
    [SerializeField] private float damageReduction = 0.15f;

    protected override IEnumerator Start()
    {
        yield return base.Start();
        
        // Daitya медленнее, но живучее
        moveSpeed *= 0.7f;
        maxHealth *= 1.5f;
        currentHealth = maxHealth;
        
        Debug.Log($"Daitya инициализирован: HP={maxHealth}, Speed={moveSpeed}");
    }

    public override void TakeDamage(float amount)
    {
        // Уменьшаем урон на процент
        float reducedDamage = amount * (1f - damageReduction);
        base.TakeDamage(reducedDamage);
        
        Debug.Log($"{name} получил {reducedDamage:F1} урона (сокращение {damageReduction * 100}%)");
    }
}