using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
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
        Move();
    }

    private void Move()
    {
        Vector2 movement = input.MovementInput;

        if (movement.magnitude > 0.1f)
        {
            Vector2 newPos = body.position + movement.normalized * settings.MoveSpeed * Time.fixedDeltaTime;
            body.MovePosition(newPos);
        }
        else
        {
            body.linearVelocity = Vector2.zero;
        }
    }

    public bool IsMoving => input.MovementInput.magnitude > 0.1f;
}