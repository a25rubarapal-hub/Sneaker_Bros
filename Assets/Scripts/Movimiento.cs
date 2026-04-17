using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerFinal : MonoBehaviour
{
    public float Acceleration = 15.0f;
    public float Speed = 1.0f;
    public float JumpForce = 185.0f;

    private Rigidbody2D Rigidbody2D;
    private float Horizontal;
    private float TimeBetweenJumps = 0.1f;
    private float LastJump;
    public float Velocity;

    private void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        // Flip personaje
        if (Horizontal > 0.0f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (Horizontal < 0.0f)
            transform.localScale = new Vector3(-1, 1, 1);

        if (Horizontal != 0.0f)
            Velocity = Mathf.Clamp(Velocity + Horizontal * Acceleration * Time.deltaTime, -1.0f, 1.0f);
        else
            Velocity -= Velocity * Acceleration * Time.deltaTime;

        // Jump
        if (Input.GetKey(KeyCode.Space) &&
            LastJump < Time.time - TimeBetweenJumps &&
            Mathf.Abs(Rigidbody2D.linearVelocity.y) < 0.05f)
        {
            Rigidbody2D.AddForce(Vector2.up * JumpForce);
            LastJump = Time.time;
        }
    }

    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = new Vector2(
            (Mathf.Abs(Velocity) < 0.01f ? 0.0f : Velocity) * Speed,
            Rigidbody2D.linearVelocity.y
        );
    }
}