using UnityEngine;

public class MovimientoNPC : MonoBehaviour
{
    public float speed = 2f;
    public int maxHealth = 2;

    public float checkDistance = 0.5f;
    public LayerMask wallLayer;

    private int currentHealth;
    private bool movingRight = true;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        Vector2 direction = movingRight ? Vector2.right : Vector2.left;

        // 🔥 aquí está el truco
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, checkDistance, wallLayer);

        Debug.DrawRay(origin, direction * checkDistance, Color.red);

        if (hit.collider != null)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        speed *= -1;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

            if (collision.contacts[0].normal.y < -0.5f)
            {
                TakeDamage(1);
            }
            else
            {
                player.TakeDamage(1);
            }
        }
    }
}