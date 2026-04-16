using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public float Acceleration = 15.0f;
    public float Speed = 1.0f;
    public float JumpForce = 185.0f;
    public float CrouchSpeedMultiplier = 0.4f;

    private Rigidbody2D Rigidbody2D;
    private Animator animator;

    private float Horizontal;
    private float TimeBetweenJumps = 0.1f;
    private float LastJump;
    public float Velocity;
    private bool isCrouching;

    private void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        bool enSuelo = Mathf.Abs(Rigidbody2D.linearVelocity.y) < 0.05f;
        isCrouching = Input.GetKey(KeyCode.S) && enSuelo;

        if (Horizontal > 0.0f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (Horizontal < 0.0f)
            transform.localScale = new Vector3(-1, 1, 1);

        if (Horizontal != 0.0f)
            Velocity = Mathf.Clamp(Velocity + Horizontal * Acceleration * Time.deltaTime, -1.0f, 1.0f);
        else
            Velocity -= Velocity * Acceleration * Time.deltaTime;

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) && LastJump < Time.time - TimeBetweenJumps && enSuelo && !isCrouching)
        {
            Rigidbody2D.AddForce(Vector2.up * JumpForce);
            LastJump = Time.time;
        }

        animator.SetBool("EnSuelo", enSuelo);
        animator.SetBool("Agachado", isCrouching);
        animator.SetBool("Corriendo", Mathf.Abs(Velocity) > 0.1f && !isCrouching);
        animator.SetFloat("VelocidadX", Mathf.Abs(Velocity));
        animator.SetFloat("VelocidadY", Rigidbody2D.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        float currentSpeed = isCrouching ? Speed * CrouchSpeedMultiplier : Speed;
        Rigidbody2D.linearVelocity = new Vector2((Mathf.Abs(Velocity) < 0.01f ? 0.0f : Velocity) * currentSpeed, Rigidbody2D.linearVelocity.y);
    }
}