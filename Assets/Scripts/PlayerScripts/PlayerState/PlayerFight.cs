using System.Linq;
using UnityEngine;

public class PlayerFight : MonoBehaviour
{
    private PlayerSettings _settings;
    [SerializeField] private float _attackRadius = 5f;
    [SerializeField] private DamageRadiusVfx _damageRadiusVfx;

    private float _cooldown;

    private void Awake()
    {
        _settings = GetComponent<PlayerSettings>();
        _damageRadiusVfx.radius = _attackRadius;
        _damageRadiusVfx.BuildEffect();
    }

    private void Update()
    {
        if (_cooldown > 0)
        {
            _cooldown -= Time.deltaTime;
        }

        if (_cooldown <= 0)
        {
            Attack();
        }
    }

    private void Attack()
    {
        _cooldown = _settings.AttackSpeed;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            _attackRadius
        );

        if (hits.Any(e => e.CompareTag("Mob")))
        {
            AudioManager.Instance.PlayRandomAttack();
            _damageRadiusVfx.PlayAttackPulse();
        }
        
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
    }
}