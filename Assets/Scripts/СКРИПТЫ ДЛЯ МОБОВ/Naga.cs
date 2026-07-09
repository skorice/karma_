using UnityEngine;

public class Naga : EnemyBase
{
    [Header("Snake Trail")]
    [SerializeField] private GameObject snakeTrailPrefab;
    [SerializeField] private float trailCooldown = 5f;

    private float baseHealth = 65f;
    private float baseSpeed;
    private float baseDamage=12f;

    private float timer;

    protected override void Update()
    {
        base.Update();

        timer += Time.deltaTime;

        if (timer >= trailCooldown)
        {
            SpawnTrail();
            timer = 0;
        }
    }

    private void SpawnTrail()
    {
        if (snakeTrailPrefab == null)
            return;

        Instantiate(
            snakeTrailPrefab,
            transform.position,
            Quaternion.identity);
    }
}