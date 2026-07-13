using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float acceleration = 80f; // высокий = почти мгновенно
    [SerializeField] private float deceleration = 100f;
    
    private PlayerSettings settings;
    private Rigidbody2D body;
    private InputReader input;

    private void Awake()
    {   
        body = GetComponent<Rigidbody2D>();
        settings = GetComponent<PlayerSettings>();
        input = new InputReader();
    }

    private void FixedUpdate()
    {
        input.Read();
        // Проверки
        if (body == null) Debug.LogError("❌ Rigidbody2D не найден!");
        if (settings == null) Debug.LogError("❌ PlayerSettings не найден!");
        if (input == null) Debug.LogError("❌ InputReader не создан!");
        Move();
    }

    private void Move()
    {
        Vector2 move = input.MovementInput;
        Vector2 targetVelocity = Vector2.zero;

        if (move.sqrMagnitude > 0.01f)
            targetVelocity = move.normalized * settings.MoveSpeed;

        // Быстрый разгон / ещё более быстрое торможение
        float accel = targetVelocity.sqrMagnitude > 0.01f ? acceleration : deceleration;
        body.linearVelocity = Vector2.MoveTowards(
            body.linearVelocity,
            targetVelocity,
            accel * Time.fixedDeltaTime
        );
    }
}