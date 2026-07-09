using UnityEngine;
using System.Collections.Generic;

public class PlayerFight : MonoBehaviour
{
    private PlayerSettings _settings;
    [SerializeField] private Transform triggerZone;
    [SerializeField] private Transform attackZone;

    private float _cooldown;
    private List<Collider2D> _enemiesInZone = new List<Collider2D>();

    public bool IsInCombat => _enemiesInZone.Count > 0;

    private void Awake()
    {
        _settings = GetComponent<PlayerSettings>();
        if (triggerZone == null) triggerZone = transform.Find("TriggerAttackZone");
        if (attackZone == null) attackZone = transform.Find("RadiusAttack");
    }

    private void Update()
    {
        if (_cooldown > 0) _cooldown -= Time.deltaTime;

        if (triggerZone != null) triggerZone.position = transform.position;
        if (attackZone != null) attackZone.position = transform.position;

        _enemiesInZone.RemoveAll(e => e == null || !e.gameObject.activeSelf);

        if (IsInCombat && _cooldown <= 0)
        {
            Attack();
        }
    }

    private void Attack()
    {
        _cooldown = _settings.AttackSpeed;

        if (attackZone != null)
        {
            var collider = attackZone.GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(
                    attackZone.position,
                    collider.radius
                );

                foreach (var hit in hits)
                {
                    if (hit.CompareTag("Mob"))
                    {
                        var enemy = hit.GetComponent<EnemyBase>();
                        if (enemy != null)
                        {
                            enemy.TakeDamage(_settings.AttackPower);
                            Debug.Log($"⚔️ Удар по {hit.name} силой {_settings.AttackPower}");
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mob") && !_enemiesInZone.Contains(other))
            _enemiesInZone.Add(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Mob") && _enemiesInZone.Contains(other))
            _enemiesInZone.Remove(other);
    }

    private void OnDrawGizmosSelected()
    {
        if (triggerZone != null)
        {
            var collider = triggerZone.GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(triggerZone.position, collider.radius);
            }
        }

        if (attackZone != null)
        {
            var collider = attackZone.GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(attackZone.position, collider.radius);
            }
        }
    }
}