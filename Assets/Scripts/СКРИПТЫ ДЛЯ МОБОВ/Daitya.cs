using UnityEngine;

public class Daitya : EnemyBase
{
    [SerializeField]
    private float damageReduction = 0.15f;
    private float baseHealth=90f;
    private float baseSpeed;
    private float baseDamage=28f;

    public override void TakeDamage(float amount)
    {
        amount *= (1f - damageReduction);

        base.TakeDamage(amount);
    }
}